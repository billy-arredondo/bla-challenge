namespace BlaChallenge.Application.Tasks.Commands.DeleteTask;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Exceptions;
using BlaChallenge.Domain;

public sealed class DeleteTaskUseCase(ITaskRepository taskRepository)
{
    public async Task ExecuteAsync(DeleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        var task = command.ActorRole is UserRole.Admin or UserRole.Superadmin
            ? await taskRepository.FindByIdAsync(command.Id, cancellationToken)
            : await taskRepository.FindByIdAsync(command.Id, command.OwnerId, cancellationToken);
        if (task is null)
        {
            throw new TaskNotFoundException(command.Id);
        }

        if (command.ActorRole == UserRole.User && task.Status is not (TaskStatus.Pending or TaskStatus.InProgress))
        {
            throw new TaskForbiddenException("Users can only delete pending or in-progress assigned tasks.");
        }

        if (command.ActorRole is UserRole.Admin or UserRole.Superadmin)
        {
            await taskRepository.DeleteAsync(command.Id, cancellationToken);
        }
        else
        {
            await taskRepository.DeleteAsync(command.Id, command.OwnerId, cancellationToken);
        }
    }
}
