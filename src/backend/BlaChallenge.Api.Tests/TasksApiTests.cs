namespace BlaChallenge.Api.Tests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

public sealed class TasksApiTests : IAsyncLifetime
{
    private readonly string databasePath = Path.Combine(Path.GetTempPath(), $"bla-challenge-task-api-{Guid.NewGuid():N}.db");
    private WebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;

    public Task InitializeAsync()
    {
        factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:Default", $"Data Source={databasePath}");
            builder.UseSetting("Jwt:Key", "development-only-key-that-is-at-least-32-chars");
        });
        client = factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        client.Dispose();
        factory.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(databasePath))
        {
            File.Delete(databasePath);
        }

        return Task.CompletedTask;
    }

    [Fact]
    public async Task Tasks_require_authentication()
    {
        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Authenticated_user_can_crud_own_task()
    {
        await AuthenticateAsync($"owner-{Guid.NewGuid():N}@example.com");
        var request = new
        {
            title = "Plan interview",
            description = "Review the API",
            status = "Pending",
            dueDateUtc = DateTime.UtcNow.AddDays(1)
        };

        var createResponse = await client.PostAsJsonAsync("/api/tasks", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(created);

        var getResponse = await client.GetAsync($"/api/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/tasks/{created.Id}", new
        {
            title = "Prepare interview",
            description = (string?)null,
            status = "Completed",
            dueDateUtc = request.dueDateUtc
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.Equal("Prepare interview", updated!.Title);

        var listResponse = await client.GetAsync("/api/tasks");
        var tasks = await listResponse.Content.ReadFromJsonAsync<List<TaskResponse>>();
        Assert.Single(tasks!);

        var deleteResponse = await client.DeleteAsync($"/api/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/tasks/{created.Id}")).StatusCode);
    }

    [Fact]
    public async Task User_cannot_access_another_users_task()
    {
        await AuthenticateAsync($"first-{Guid.NewGuid():N}@example.com");
        var createResponse = await client.PostAsJsonAsync("/api/tasks", new
        {
            title = "Private task",
            description = (string?)null,
            status = "Pending",
            dueDateUtc = DateTime.UtcNow.AddDays(1)
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        client.DefaultRequestHeaders.Authorization = null;
        await AuthenticateAsync($"second-{Guid.NewGuid():N}@example.com");

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/tasks/{created!.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/api/tasks/{created.Id}")).StatusCode);
    }

    [Fact]
    public async Task Invalid_task_title_returns_bad_request()
    {
        await AuthenticateAsync($"validation-{Guid.NewGuid():N}@example.com");

        var response = await client.PostAsJsonAsync("/api/tasks", new
        {
            title = "   ",
            description = (string?)null,
            status = "Pending",
            dueDateUtc = DateTime.UtcNow.AddDays(1)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task AuthenticateAsync(string email)
    {
        await client.PostAsJsonAsync("/api/auth/register", new { email, password = "password123" });
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { email, password = "password123" });
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AccessToken);
    }

    private sealed record LoginResponse(UserResponse User, string AccessToken);

    private sealed record UserResponse(Guid Id, string Email, DateTime CreatedAtUtc);

    private sealed record TaskResponse(
        Guid Id,
        string Title,
        string? Description,
        string Status,
        DateTime DueDateUtc,
        Guid OwnerId);
}
