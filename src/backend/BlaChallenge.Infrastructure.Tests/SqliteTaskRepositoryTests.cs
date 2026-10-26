namespace BlaChallenge.Infrastructure.Tests;

using BlaChallenge.Domain;
using BlaChallenge.Infrastructure.Sqlite;
using BlaChallenge.Infrastructure.Sqlite.Repositories;
using Microsoft.Data.Sqlite;

public sealed class SqliteTaskRepositoryTests : IAsyncLifetime
{
    private readonly string databasePath = Path.Combine(Path.GetTempPath(), $"bla-challenge-tasks-{Guid.NewGuid():N}.db");
    private SqliteTaskRepository repository = null!;
    private Guid ownerId;
    private Guid otherOwnerId;

    public async Task InitializeAsync()
    {
        var factory = new SqliteConnectionFactory($"Data Source={databasePath}");
        await new SqliteDatabaseInitializer(factory).InitializeAsync();
        repository = new SqliteTaskRepository(factory);
        ownerId = Guid.NewGuid();
        otherOwnerId = Guid.NewGuid();
        await AddUserAsync(factory, ownerId);
        await AddUserAsync(factory, otherOwnerId);
    }

    public Task DisposeAsync()
    {
        SqliteConnection.ClearAllPools();
        File.Delete(databasePath);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Crud_round_trip_and_owner_filtering_work()
    {
        var task = TaskItem.Create(Guid.NewGuid(), ownerId, "Task", "Description", TaskStatus.Pending, DateTime.UtcNow.AddDays(1));
        var otherTask = TaskItem.Create(Guid.NewGuid(), otherOwnerId, "Other", null, TaskStatus.Completed, DateTime.UtcNow.AddDays(2));

        await repository.AddAsync(task);
        await repository.AddAsync(otherTask);
        var tasks = await repository.GetByOwnerIdAsync(ownerId);

        var stored = Assert.Single(tasks);
        Assert.Equal(task.Id, stored.Id);
        Assert.Equal(task.Description, stored.Description);
        Assert.Null(await repository.FindByIdAsync(task.Id, otherOwnerId));

        stored.Update("Updated", null, TaskStatus.InProgress, task.DueDateUtc.AddDays(1));
        await repository.UpdateAsync(stored);
        Assert.Equal("Updated", (await repository.FindByIdAsync(task.Id, ownerId))!.Title);

        await repository.DeleteAsync(task.Id, ownerId);
        Assert.Null(await repository.FindByIdAsync(task.Id, ownerId));
    }

    private static async Task AddUserAsync(SqliteConnectionFactory factory, Guid id)
    {
        await using var connection = factory.Create();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Users (Id, Email, PasswordHash, CreatedAtUtc) VALUES ($id, $email, $hash, $created);";
        command.Parameters.AddWithValue("$id", id.ToString());
        command.Parameters.AddWithValue("$email", $"{id:N}@example.com");
        command.Parameters.AddWithValue("$hash", "hash");
        command.Parameters.AddWithValue("$created", DateTime.UtcNow.ToString("O"));
        await command.ExecuteNonQueryAsync();
    }
}
