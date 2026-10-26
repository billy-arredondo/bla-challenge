namespace BlaChallenge.Application.Tasks.Queries.GetTasks;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Commands.CreateTask;
using BlaChallenge.Application.Tasks.Contracts;

public sealed class GetTasksUseCase(ITaskRepository taskRepository)
{
    public async Task<IReadOnlyList<TaskResponse>> ExecuteAsync(
        GetTasksQuery query,
        CancellationToken cancellationToken = default)
    {
        var tasks = await taskRepository.GetByOwnerIdAsync(query.OwnerId, cancellationToken);
        return tasks.Select(CreateTaskUseCase.ToResponse).ToList();
    }
}
