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
            INSERT INTO Tasks (Id, OwnerId, CreatedById, AssignedToId, Title, Description, Status, DueDateUtc, UpdatedById, UpdatedAtUtc)
            VALUES ($id, $ownerId, $createdById, $assignedToId, $title, $description, $status, $dueDateUtc, $updatedById, $updatedAtUtc);
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
        command.CommandText = SelectSql + " WHERE AssignedToId = $ownerId ORDER BY DueDateUtc, Id;";
        command.Parameters.AddWithValue("$ownerId", ownerId.ToString());
        return await ReadTasksAsync(command, cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = SelectSql + " ORDER BY DueDateUtc, Id;";
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
        command.CommandText = SelectSql + " WHERE Id = $id AND AssignedToId = $ownerId;";
        command.Parameters.AddWithValue("$id", id.ToString());
        command.Parameters.AddWithValue("$ownerId", ownerId.ToString());
        var tasks = await ReadTasksAsync(command, cancellationToken);
        return tasks.SingleOrDefault();
    }

    public async Task<TaskItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = SelectSql + " WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id.ToString());
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
            SET Title = $title, Description = $description, Status = $status, DueDateUtc = $dueDateUtc,
                OwnerId = $assignedToId, AssignedToId = $assignedToId, UpdatedById = $updatedById, UpdatedAtUtc = $updatedAtUtc
            WHERE Id = $id;
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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Tasks WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id.ToString());
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private const string SelectSql = "SELECT Id, OwnerId, CreatedById, AssignedToId, Title, Description, Status, DueDateUtc, UpdatedById, UpdatedAtUtc FROM Tasks";

    private static void AddParameters(SqliteCommand command, TaskItem task)
    {
        command.Parameters.AddWithValue("$id", task.Id.ToString());
        command.Parameters.AddWithValue("$ownerId", task.OwnerId.ToString());
        command.Parameters.AddWithValue("$createdById", task.CreatedById.ToString());
        command.Parameters.AddWithValue("$assignedToId", task.AssignedToId.ToString());
        command.Parameters.AddWithValue("$title", task.Title);
        command.Parameters.AddWithValue("$description", (object?)task.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("$status", task.Status.ToString());
        command.Parameters.AddWithValue("$dueDateUtc", task.DueDateUtc.ToString("O"));
        command.Parameters.AddWithValue("$updatedById", (object?)task.UpdatedById?.ToString() ?? DBNull.Value);
        command.Parameters.AddWithValue("$updatedAtUtc", (object?)task.UpdatedAtUtc?.ToString("O") ?? DBNull.Value);
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
                Guid.Parse(reader.GetString(2)),
                Guid.Parse(reader.GetString(3)),
                reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                Enum.Parse<TaskStatus>(reader.GetString(6)),
                DateTime.Parse(reader.GetString(7), null, System.Globalization.DateTimeStyles.RoundtripKind),
                reader.IsDBNull(8) ? null : Guid.Parse(reader.GetString(8)),
                reader.IsDBNull(9) ? null : DateTime.Parse(reader.GetString(9), null, System.Globalization.DateTimeStyles.RoundtripKind)));
        }

        return tasks;
    }
}
