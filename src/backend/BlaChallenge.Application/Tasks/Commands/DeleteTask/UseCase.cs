namespace BlaChallenge.Application.Tasks.Commands.DeleteTask;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Exceptions;
public sealed class DeleteTaskUseCase(ITaskRepository taskRepository)
{
    public async Task ExecuteAsync(DeleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        if (await taskRepository.FindByIdAsync(command.Id, command.OwnerId, cancellationToken) is null)
        {
            throw new TaskNotFoundException(command.Id);
        }

        await taskRepository.DeleteAsync(command.Id, command.OwnerId, cancellationToken);
    }
}
