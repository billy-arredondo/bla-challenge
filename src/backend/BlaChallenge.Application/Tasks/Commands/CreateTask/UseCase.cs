namespace BlaChallenge.Application.Tasks.Commands.CreateTask;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Contracts;
using DomainTask = BlaChallenge.Domain.TaskItem;

public sealed class CreateTaskUseCase(ITaskRepository taskRepository)
{
    public async Task<TaskResponse> ExecuteAsync(
        CreateTaskCommand command,
        CancellationToken cancellationToken = default)
    {
        var task = DomainTask.Create(
            Guid.NewGuid(), command.OwnerId, command.Title, command.Description, command.Status, command.DueDateUtc);
        await taskRepository.AddAsync(task, cancellationToken);
        return ToResponse(task);
    }

    internal static TaskResponse ToResponse(DomainTask task)
        => new(task.Id, task.Title, task.Description, task.Status, task.DueDateUtc, task.OwnerId);
}
