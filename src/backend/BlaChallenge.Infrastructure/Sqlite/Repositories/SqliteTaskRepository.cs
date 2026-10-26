namespace BlaChallenge.Infrastructure.Sqlite.Repositories;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Domain;
using Microsoft.Data.Sqlite;

public sealed class SqliteTaskRepository(SqliteConnectionFactory connectionFactory) : ITaskRepository
{
    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Tasks (Id, OwnerId, Title, Description, Status, DueDateUtc)
            VALUES ($id, $ownerId, $title, $description, $status, $dueDateUtc);
            """;
        AddParameters(command, task);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, OwnerId, Title, Description, Status, DueDateUtc
            FROM Tasks
            WHERE OwnerId = $ownerId
            ORDER BY DueDateUtc, Id;
            """;
        command.Parameters.AddWithValue("$ownerId", ownerId.ToString());
        return await ReadTasksAsync(command, cancellationToken);
    }

    public async Task<TaskItem?> FindByIdAsync(
        Guid id,
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, OwnerId, Title, Description, Status, DueDateUtc
            FROM Tasks
            WHERE Id = $id AND OwnerId = $ownerId;
            """;
        command.Parameters.AddWithValue("$id", id.ToString());
        command.Parameters.AddWithValue("$ownerId", ownerId.ToString());
        var tasks = await ReadTasksAsync(command, cancellationToken);
        return tasks.SingleOrDefault();
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Tasks
            SET Title = $title, Description = $description, Status = $status, DueDateUtc = $dueDateUtc
            WHERE Id = $id AND OwnerId = $ownerId;
            """;
        AddParameters(command, task);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Tasks WHERE Id = $id AND OwnerId = $ownerId;";
        command.Parameters.AddWithValue("$id", id.ToString());
        command.Parameters.AddWithValue("$ownerId", ownerId.ToString());
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddParameters(SqliteCommand command, TaskItem task)
    {
        command.Parameters.AddWithValue("$id", task.Id.ToString());
        command.Parameters.AddWithValue("$ownerId", task.OwnerId.ToString());
        command.Parameters.AddWithValue("$title", task.Title);
        command.Parameters.AddWithValue("$description", (object?)task.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("$status", task.Status.ToString());
        command.Parameters.AddWithValue("$dueDateUtc", task.DueDateUtc.ToString("O"));
    }

    private static async Task<IReadOnlyList<TaskItem>> ReadTasksAsync(
        SqliteCommand command,
        CancellationToken cancellationToken)
    {
        var tasks = new List<TaskItem>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            tasks.Add(TaskItem.Create(
                Guid.Parse(reader.GetString(0)),
                Guid.Parse(reader.GetString(1)),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                Enum.Parse<TaskStatus>(reader.GetString(4)),
                DateTime.Parse(reader.GetString(5), null, System.Globalization.DateTimeStyles.RoundtripKind)));
        }

        return tasks;
    }
}
