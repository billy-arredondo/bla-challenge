using BlaChallenge.Domain;
using BlaChallenge.Infrastructure.Sqlite;
using Microsoft.Data.Sqlite;

namespace BlaChallenge.Infrastructure.Tests;

public sealed class SqliteUserRepositoryTests : IAsyncLifetime
{
    private readonly string databasePath = Path.Combine(Path.GetTempPath(), $"bla-challenge-{Guid.NewGuid():N}.db");
    private SqliteUserRepository repository = null!;

    public async Task InitializeAsync()
    {
        var factory = new SqliteConnectionFactory($"Data Source={databasePath}");
        await new SqliteDatabaseInitializer(factory).InitializeAsync();
        repository = new SqliteUserRepository(factory);
    }

    public Task DisposeAsync()
    {
        SqliteConnection.ClearAllPools();
        File.Delete(databasePath);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Add_and_find_user_round_trip_through_sqlite()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "stored-hash", DateTime.UtcNow);

        await repository.AddAsync(user);
        var result = await repository.FindByEmailAsync("user@example.com");
        var resultById = await repository.FindByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.PasswordHash, result.PasswordHash);
        Assert.Equal(user.Id, resultById!.Id);
    }

    [Fact]
    public async Task Add_rejects_duplicate_email()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "stored-hash", DateTime.UtcNow);
        await repository.AddAsync(user);

        await Assert.ThrowsAsync<BlaChallenge.Application.Users.Exceptions.DuplicateUserException>(() => repository.AddAsync(
            User.Create(Guid.NewGuid(), "USER@example.com", "other-hash", DateTime.UtcNow)));
    }
}
