namespace BlaChallenge.Domain;

public sealed class TaskItem
{
    private TaskItem(
        Guid id,
        Guid ownerId,
        string title,
        string? description,
        TaskStatus status,
        DateTime dueDateUtc)
    {
        Id = id;
        OwnerId = ownerId;
        Title = title;
        Description = description;
        Status = status;
        DueDateUtc = dueDateUtc;
    }

    public Guid Id { get; }

    public Guid OwnerId { get; }

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
        Validate(id, ownerId, title, description, status, dueDateUtc);
        return new TaskItem(id, ownerId, title.Trim(), NormalizeDescription(description), status, dueDateUtc.ToUniversalTime());
    }

    public void Update(string title, string? description, TaskStatus status, DateTime dueDateUtc)
    {
        Validate(Id, OwnerId, title, description, status, dueDateUtc);
        Title = title.Trim();
        Description = NormalizeDescription(description);
        Status = status;
        DueDateUtc = dueDateUtc.ToUniversalTime();
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
}
