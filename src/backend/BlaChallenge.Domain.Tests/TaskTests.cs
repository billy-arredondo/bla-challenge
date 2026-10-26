namespace BlaChallenge.Domain.Tests;

using BlaChallenge.Domain;

public sealed class TaskTests
{
    [Fact]
    public void Create_trims_fields_and_preserves_status()
    {
        var dueDate = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);

        var task = TaskItem.Create(Guid.NewGuid(), Guid.NewGuid(), "  title  ", "  details  ", TaskStatus.InProgress, dueDate);

        Assert.Equal("title", task.Title);
        Assert.Equal("details", task.Description);
        Assert.Equal(TaskStatus.InProgress, task.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_empty_title(string title)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            TaskItem.Create(Guid.NewGuid(), Guid.NewGuid(), title, null, TaskStatus.Pending, DateTime.UtcNow));

        Assert.Equal("title", exception.ParamName);
    }

    [Fact]
    public void Update_changes_all_mutable_fields()
    {
        var task = TaskItem.Create(Guid.NewGuid(), Guid.NewGuid(), "old", null, TaskStatus.Pending, DateTime.UtcNow);
        var dueDate = DateTime.UtcNow.AddDays(1);

        task.Update("new", "description", TaskStatus.InProgress, dueDate);

        Assert.Equal("new", task.Title);
        Assert.Equal("description", task.Description);
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.Equal(dueDate, task.DueDateUtc);
    }

    [Fact]
    public void Create_rejects_unknown_status()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            TaskItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Task", null, (TaskStatus)99, DateTime.UtcNow));

        Assert.Equal("status", exception.ParamName);
    }

    [Fact]
    public void Update_rejects_pending_to_completed_transition()
    {
        var task = TaskItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Task", null, TaskStatus.Pending, DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            task.Update("Task", null, TaskStatus.Completed, DateTime.UtcNow));
    }

    [Fact]
    public void Canceled_task_cannot_be_modified()
    {
        var task = TaskItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Task", null, TaskStatus.InProgress, DateTime.UtcNow);
        task.Update("Task", null, TaskStatus.Canceled, DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            task.Update("Changed", null, TaskStatus.Canceled, DateTime.UtcNow));
    }
}
