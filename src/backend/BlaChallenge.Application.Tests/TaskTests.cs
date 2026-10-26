namespace BlaChallenge.Application.Tests;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Commands.CreateTask;
using BlaChallenge.Application.Tasks.Commands.UpdateTask;
using BlaChallenge.Application.Tasks.Exceptions;
using BlaChallenge.Application.Tasks.Queries.GetTasks;
using BlaChallenge.Domain;

public sealed class TaskTests
{
    [Fact]
    public async Task Create_assigns_the_authenticated_owner()
    {
        var ownerId = Guid.NewGuid();
        var repository = new FakeTaskRepository();
        var result = await new CreateTaskUseCase(repository).ExecuteAsync(
            new CreateTaskCommand(ownerId, "Task", null, TaskStatus.Pending, DateTime.UtcNow));

        Assert.Equal(ownerId, result.OwnerId);
        Assert.Equal(ownerId, repository.Stored!.OwnerId);
    }

    [Fact]
    public async Task Get_tasks_returns_only_repository_results_for_owner()
    {
        var ownerId = Guid.NewGuid();
        var repository = new FakeTaskRepository();
        repository.Stored = BlaChallenge.Domain.TaskItem.Create(
            Guid.NewGuid(), ownerId, "Task", null, TaskStatus.Pending, DateTime.UtcNow);

        var result = await new GetTasksUseCase(repository).ExecuteAsync(new GetTasksQuery(ownerId));

        var task = Assert.Single(result);
        Assert.Equal(ownerId, task.OwnerId);
    }

    [Fact]
    public async Task Update_missing_task_returns_not_found_error()
    {
        var exception = await Assert.ThrowsAsync<TaskNotFoundException>(() =>
            new UpdateTaskUseCase(new FakeTaskRepository()).ExecuteAsync(
                new UpdateTaskCommand(Guid.NewGuid(), Guid.NewGuid(), "Task", null, TaskStatus.Pending, DateTime.UtcNow)));

        Assert.Contains("was not found", exception.Message);
    }

    private sealed class FakeTaskRepository : ITaskRepository
    {
        public BlaChallenge.Domain.TaskItem? Stored { get; set; }

        public Task AddAsync(BlaChallenge.Domain.TaskItem task, CancellationToken cancellationToken = default)
        {
            Stored = task;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<BlaChallenge.Domain.TaskItem>> GetByOwnerIdAsync(
            Guid ownerId,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<BlaChallenge.Domain.TaskItem>>(
                Stored is { OwnerId: var storedOwnerId } && storedOwnerId == ownerId
                    ? [Stored]
                    : []);

        public Task<BlaChallenge.Domain.TaskItem?> FindByIdAsync(
            Guid id,
            Guid ownerId,
            CancellationToken cancellationToken = default)
            => Task.FromResult(Stored is { Id: var storedId, OwnerId: var storedOwnerId }
                && storedId == id && storedOwnerId == ownerId ? Stored : null);

        public Task UpdateAsync(BlaChallenge.Domain.TaskItem task, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task DeleteAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
        {
            Stored = null;
            return Task.CompletedTask;
        }
    }
}
