namespace BlaChallenge.Application.Tasks.Abstractions;

using DomainTask = BlaChallenge.Domain.TaskItem;

public interface ITaskRepository
{
    Task AddAsync(DomainTask task, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DomainTask>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task<DomainTask?> FindByIdAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task UpdateAsync(DomainTask task, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
}
