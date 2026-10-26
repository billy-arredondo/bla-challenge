namespace BlaChallenge.Application.Tasks.Commands.CreateTask;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Contracts;
using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Domain;

public sealed class CreateTaskUseCase(
    ITaskRepository taskRepository,
    IUserRepository? userRepository = null)
{
    public async Task<TaskResponse> ExecuteAsync(
        CreateTaskCommand command,
        CancellationToken cancellationToken = default)
    {
        var assignedToId = command.ActorRole == UserRole.User
            ? command.OwnerId
            : command.AssignedToId ?? command.OwnerId;
        if (command.ActorRole is UserRole.Admin or UserRole.Superadmin
            && userRepository is not null
            && await userRepository.FindByIdAsync(assignedToId, cancellationToken) is null)
        {
            throw new InvalidUserInputException("The assigned user was not found.");
        }

        var task = TaskItem.Create(
            Guid.NewGuid(), command.OwnerId, assignedToId, command.Title, command.Description, command.Status, command.DueDateUtc);
        await taskRepository.AddAsync(task, cancellationToken);
        return ToResponse(task);
    }

    internal static TaskResponse ToResponse(TaskItem task)
        => new(task.Id, task.Title, task.Description, task.Status, task.DueDateUtc, task.OwnerId,
            task.CreatedById, task.AssignedToId, task.UpdatedById, task.UpdatedAtUtc);
}
