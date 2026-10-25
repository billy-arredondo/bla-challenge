using BlaChallenge.Domain;

namespace BlaChallenge.Application.Authentication;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}

public interface ITokenService
{
    string CreateToken(User user);
}

public interface IClock
{
    DateTime UtcNow { get; }
}

public sealed record RegisterUserRequest(string Email, string Password);

public sealed record LoginRequest(string Email, string Password);

public sealed record UserResponse(Guid Id, string Email, DateTime CreatedAtUtc);

public sealed record RegisterUserResult(UserResponse User);

public sealed record LoginResult(UserResponse User, string AccessToken);

public sealed class DuplicateUserException : Exception
{
    public DuplicateUserException(string email)
        : base($"A user with email '{email}' already exists.")
    {
    }
}

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid email or password.")
    {
    }
}

public sealed class InvalidUserInputException : Exception
{
    public InvalidUserInputException(string message)
        : base(message)
    {
    }
}
