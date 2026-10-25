namespace BlaChallenge.Infrastructure.Sqlite;

using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Domain;
using Microsoft.Data.Sqlite;

public sealed class SqliteUserRepository : IUserRepository
{
    private readonly SqliteConnectionFactory connectionFactory;

    public SqliteUserRepository(SqliteConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Users (Id, Email, PasswordHash, CreatedAtUtc)
            VALUES ($id, $email, $passwordHash, $createdAtUtc);
            """;
        command.Parameters.AddWithValue("$id", user.Id.ToString());
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("$createdAtUtc", user.CreatedAtUtc.ToString("O"));

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new DuplicateUserException(user.Email);
        }
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Email, PasswordHash, CreatedAtUtc
            FROM Users
            WHERE Email = $email;
            """;
        command.Parameters.AddWithValue("$email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return User.Create(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            reader.GetString(2),
            DateTime.Parse(reader.GetString(3), null, System.Globalization.DateTimeStyles.RoundtripKind));
    }

    public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Email, PasswordHash, CreatedAtUtc
            FROM Users
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$id", id.ToString());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return User.Create(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            reader.GetString(2),
            DateTime.Parse(reader.GetString(3), null, System.Globalization.DateTimeStyles.RoundtripKind));
    }
}
