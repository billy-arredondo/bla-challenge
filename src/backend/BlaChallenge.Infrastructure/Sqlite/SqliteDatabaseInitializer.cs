namespace BlaChallenge.Infrastructure.Sqlite;

using System.ComponentModel.DataAnnotations;
using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Domain;
using Microsoft.Data.Sqlite;

public sealed class SqliteDatabaseInitializer(
    SqliteConnectionFactory connectionFactory,
    IPasswordHasher passwordHasher)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Users (
                Id TEXT NOT NULL PRIMARY KEY,
                Email TEXT NOT NULL COLLATE NOCASE UNIQUE,
                PasswordHash TEXT NOT NULL,
                CreatedAtUtc TEXT NOT NULL,
                Role TEXT NOT NULL DEFAULT 'User'
            );
            CREATE TABLE IF NOT EXISTS Tasks (
                Id TEXT NOT NULL PRIMARY KEY,
                OwnerId TEXT NOT NULL,
                CreatedById TEXT NOT NULL,
                AssignedToId TEXT NOT NULL,
                Title TEXT NOT NULL,
                Description TEXT NULL,
                Status TEXT NOT NULL,
                DueDateUtc TEXT NOT NULL,
                UpdatedById TEXT NULL,
                UpdatedAtUtc TEXT NULL,
                FOREIGN KEY (AssignedToId) REFERENCES Users (Id) ON DELETE CASCADE
            );
            """;
        await command.ExecuteNonQueryAsync(cancellationToken);
        await AddColumnIfMissingAsync(connection, "Users", "Role", "TEXT NOT NULL DEFAULT 'User'", cancellationToken);
        await AddColumnIfMissingAsync(connection, "Tasks", "CreatedById", "TEXT NOT NULL DEFAULT ''", cancellationToken);
        await AddColumnIfMissingAsync(connection, "Tasks", "AssignedToId", "TEXT NOT NULL DEFAULT ''", cancellationToken);
        await AddColumnIfMissingAsync(connection, "Tasks", "UpdatedById", "TEXT NULL", cancellationToken);
        await AddColumnIfMissingAsync(connection, "Tasks", "UpdatedAtUtc", "TEXT NULL", cancellationToken);

        await using var migration = connection.CreateCommand();
        migration.CommandText = "UPDATE Tasks SET CreatedById = OwnerId, AssignedToId = OwnerId WHERE CreatedById = '' OR AssignedToId = '';";
        await migration.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SeedSuperadminAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email)
            || string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new InvalidOperationException("Superadmin configuration must contain a valid email and password of at least 8 characters.");
        }

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Users (Id, Email, PasswordHash, CreatedAtUtc, Role)
            SELECT $id, $email, $passwordHash, $createdAtUtc, $role
            WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = $email);
            UPDATE Users SET Role = $role WHERE Email = $email;
            """;
        command.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("$email", email.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("$passwordHash", passwordHasher.Hash(password));
        command.Parameters.AddWithValue("$createdAtUtc", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("$role", UserRole.Superadmin.ToString());
        await command.ExecuteNonQueryAsync(cancellationToken);
    }


    private static async Task AddColumnIfMissingAsync(
        SqliteConnection connection,
        string table,
        string column,
        string definition,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition};";
        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 1)
        {
            // The column already exists.
        }
    }
}
