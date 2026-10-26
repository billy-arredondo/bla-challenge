namespace BlaChallenge.Application.Tasks.Commands.UpdateTask;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Commands.CreateTask;
using BlaChallenge.Application.Tasks.Contracts;
using BlaChallenge.Application.Tasks.Exceptions;

public sealed class UpdateTaskUseCase(ITaskRepository taskRepository)
{
    public async Task<TaskResponse> ExecuteAsync(
        UpdateTaskCommand command,
        CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.FindByIdAsync(command.Id, command.OwnerId, cancellationToken)
            ?? throw new TaskNotFoundException(command.Id);
        task.Update(command.Title, command.Description, command.Status, command.DueDateUtc);
        await taskRepository.UpdateAsync(task, cancellationToken);
        return CreateTaskUseCase.ToResponse(task);
    }
}
