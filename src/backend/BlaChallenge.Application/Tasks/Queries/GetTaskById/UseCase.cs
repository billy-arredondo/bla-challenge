namespace BlaChallenge.Application.Tasks.Queries.GetTaskById;

using BlaChallenge.Application.Tasks.Abstractions;
using BlaChallenge.Application.Tasks.Commands.CreateTask;
using BlaChallenge.Application.Tasks.Contracts;

public sealed class GetTaskByIdUseCase(ITaskRepository taskRepository)
{
    public async Task<TaskResponse?> ExecuteAsync(
        GetTaskByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.FindByIdAsync(query.Id, query.OwnerId, cancellationToken);
        return task is null ? null : CreateTaskUseCase.ToResponse(task);
    }
}
