namespace BlaChallenge.Application.Users.Abstractions;

using BlaChallenge.Domain;
public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpdateRoleAsync(User user, CancellationToken cancellationToken = default);
}
