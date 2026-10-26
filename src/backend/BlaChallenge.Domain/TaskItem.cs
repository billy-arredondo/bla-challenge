namespace BlaChallenge.Domain;

public sealed class TaskItem
{
    private TaskItem(
        Guid id,
        Guid createdById,
        Guid assignedToId,
        string title,
        string? description,
        TaskStatus status,
        DateTime dueDateUtc)
    {
        Id = id;
        CreatedById = createdById;
        AssignedToId = assignedToId;
        Title = title;
        Description = description;
        Status = status;
        DueDateUtc = dueDateUtc;
    }

    public Guid Id { get; }

    public Guid CreatedById { get; }

    public Guid AssignedToId { get; private set; }

    public Guid OwnerId => AssignedToId;

    public Guid? UpdatedById { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public TaskStatus Status { get; private set; }

    public DateTime DueDateUtc { get; private set; }

    public static TaskItem Create(
        Guid id,
        Guid ownerId,
        string title,
        string? description,
        TaskStatus status,
        DateTime dueDateUtc)
    {
        return Create(id, ownerId, ownerId, title, description, status, dueDateUtc);
    }

    public static TaskItem Create(
        Guid id,
        Guid createdById,
        Guid assignedToId,
        string title,
        string? description,
        TaskStatus status,
        DateTime dueDateUtc,
        Guid? updatedById = null,
        DateTime? updatedAtUtc = null)
    {
        Validate(id, assignedToId, title, description, status, dueDateUtc);
        if (createdById == Guid.Empty)
        {
            throw new ArgumentException("Task creator id is required.", nameof(createdById));
        }

        var task = new TaskItem(
            id,
            createdById,
            assignedToId,
            title.Trim(),
            NormalizeDescription(description),
            status,
            dueDateUtc.ToUniversalTime());
        task.UpdatedById = updatedById;
        task.UpdatedAtUtc = updatedAtUtc?.ToUniversalTime();
        return task;
    }

    public void Update(string title, string? description, TaskStatus status, DateTime dueDateUtc)
    {
        Update(title, description, status, dueDateUtc, null, null);
    }

    public void Update(
        string title,
        string? description,
        TaskStatus status,
        DateTime dueDateUtc,
        Guid? updatedById,
        DateTime? updatedAtUtc,
        Guid? assignedToId = null)
    {
        var newAssignedToId = assignedToId ?? AssignedToId;
        Validate(Id, newAssignedToId, title, description, status, dueDateUtc);
        ValidateStatusTransition(Status, status);
        Title = title.Trim();
        Description = NormalizeDescription(description);
        Status = status;
        DueDateUtc = dueDateUtc.ToUniversalTime();
        AssignedToId = newAssignedToId;
        UpdatedById = updatedById;
        UpdatedAtUtc = updatedAtUtc?.ToUniversalTime();
    }

    private static void Validate(
        Guid id,
        Guid ownerId,
        string title,
        string? description,
        TaskStatus status,
        DateTime dueDateUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Task id is required.", nameof(id));
        }

        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Task owner id is required.", nameof(ownerId));
        }

        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 200)
        {
            throw new ArgumentException("Task title is required and cannot exceed 200 characters.", nameof(title));
        }

        if (description?.Length > 2000)
        {
            throw new ArgumentException("Task description cannot exceed 2000 characters.", nameof(description));
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Task status is invalid.", nameof(status));
        }

        if (dueDateUtc == default)
        {
            throw new ArgumentException("Task due date is required.", nameof(dueDateUtc));
        }
    }

    private static string? NormalizeDescription(string? description)
        => string.IsNullOrWhiteSpace(description) ? null : description.Trim();

    private static void ValidateStatusTransition(TaskStatus current, TaskStatus next)
    {
        if (current == TaskStatus.Canceled)
        {
            throw new InvalidOperationException("Canceled tasks cannot be modified.");
        }

        if (current == TaskStatus.Completed && next != TaskStatus.Completed)
        {
            throw new InvalidOperationException("Completed tasks cannot be reopened.");
        }

        if (current == TaskStatus.Pending && next is not (TaskStatus.Pending or TaskStatus.InProgress or TaskStatus.Canceled)
            || current == TaskStatus.InProgress && next is not (TaskStatus.InProgress or TaskStatus.Completed or TaskStatus.Canceled)
            || current == TaskStatus.Completed && next != TaskStatus.Completed)
        {
            throw new InvalidOperationException("The task status transition is not allowed.");
        }
    }
}
