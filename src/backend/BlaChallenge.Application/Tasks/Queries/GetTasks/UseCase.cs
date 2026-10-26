namespace BlaChallenge.Application.Tasks.Queries.GetTasks;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Commands.CreateTask;
using BlaChallenge.Application.Tasks.Contracts;
using BlaChallenge.Domain;

public sealed class GetTasksUseCase(ITaskRepository taskRepository)
{
    public async Task<IReadOnlyList<TaskResponse>> ExecuteAsync(
        GetTasksQuery query,
        CancellationToken cancellationToken = default)
    {
        var tasks = query.ActorRole is UserRole.Admin or UserRole.Superadmin
            ? await taskRepository.GetAllAsync(cancellationToken)
            : await taskRepository.GetByOwnerIdAsync(query.OwnerId, cancellationToken);
        return tasks.Select(CreateTaskUseCase.ToResponse).ToList();
    }
}
