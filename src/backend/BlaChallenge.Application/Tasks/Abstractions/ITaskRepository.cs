namespace BlaChallenge.Application.Tasks.Abstractions;

using BlaChallenge.Domain;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task<TaskItem?> FindByIdAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
}
