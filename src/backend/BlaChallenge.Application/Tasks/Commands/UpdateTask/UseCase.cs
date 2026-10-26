namespace BlaChallenge.Application.Tasks.Commands.UpdateTask;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Commands.CreateTask;
using BlaChallenge.Application.Tasks.Contracts;
using BlaChallenge.Application.Tasks.Exceptions;
using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Domain;

public sealed class UpdateTaskUseCase(
    ITaskRepository taskRepository,
    IUserRepository? userRepository = null,
    IClock? clock = null)
{
    public async Task<TaskResponse> ExecuteAsync(
        UpdateTaskCommand command,
        CancellationToken cancellationToken = default)
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
            throw new TaskForbiddenException("Users can only modify pending or in-progress assigned tasks.");
        }

        if (command.ActorRole == UserRole.User && command.AssignedToId.HasValue)
        {
            throw new TaskForbiddenException("Users cannot reassign tasks.");
        }

        var assignedToId = command.ActorRole is UserRole.Admin or UserRole.Superadmin
            ? command.AssignedToId ?? task.AssignedToId
            : task.AssignedToId;
        if (command.ActorRole is UserRole.Admin or UserRole.Superadmin
            && assignedToId != task.AssignedToId
            && userRepository is not null
            && await userRepository.FindByIdAsync(assignedToId, cancellationToken) is null)
        {
            throw new InvalidUserInputException("The assigned user was not found.");
        }

        task.Update(command.Title, command.Description, command.Status, command.DueDateUtc,
            command.OwnerId, clock?.UtcNow ?? DateTime.UtcNow, assignedToId);
        await taskRepository.UpdateAsync(task, cancellationToken);
        return CreateTaskUseCase.ToResponse(task);
    }
}
