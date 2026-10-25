using System.ComponentModel.DataAnnotations;
using BlaChallenge.Domain;

namespace BlaChallenge.Application.Authentication;

public sealed class RegisterUserUseCase
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly IClock clock;

    public RegisterUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher, IClock clock)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.clock = clock;
    }

    public async Task<RegisterUserResult> ExecuteAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        ValidatePassword(request.Password);

        if (await userRepository.FindByEmailAsync(email, cancellationToken) is not null)
        {
            throw new DuplicateUserException(email);
        }

        var user = User.Create(Guid.NewGuid(), email, passwordHasher.Hash(request.Password), clock.UtcNow);
        await userRepository.AddAsync(user, cancellationToken);

        return new RegisterUserResult(ToResponse(user));
    }

    internal static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email.Trim()))
        {
            throw new InvalidUserInputException("A valid email is required.");
        }

        return email.Trim().ToLowerInvariant();
    }

    internal static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new InvalidUserInputException("Password must contain at least 8 characters.");
        }
    }

    internal static UserResponse ToResponse(User user)
        => new(user.Id, user.Email, user.CreatedAtUtc);
}
