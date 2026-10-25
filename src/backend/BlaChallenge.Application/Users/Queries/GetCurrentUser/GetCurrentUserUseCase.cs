namespace BlaChallenge.Application.Users.Queries.GetCurrentUser;

using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Application.Users.Contracts;

public sealed class GetCurrentUserUseCase
{
    private readonly IUserRepository userRepository;

    public GetCurrentUserUseCase(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<UserResponse?> ExecuteAsync(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdAsync(query.UserId, cancellationToken);
        return user is null ? null : new UserResponse(user.Id, user.Email, user.CreatedAtUtc);
    }
}
