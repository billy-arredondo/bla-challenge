namespace BlaChallenge.Application.Users.Commands.RegisterUser;

using System.ComponentModel.DataAnnotations;
using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Application.Users.Contracts;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Domain;

public sealed class RegisterUserUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IClock clock)
{
    public async Task<RegisterUserResult> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(command.Email);
        ValidatePassword(command.Password);

        if (await userRepository.FindByEmailAsync(email, cancellationToken) is not null)
        {
            throw new DuplicateUserException(email);
        }

        var user = User.Create(Guid.NewGuid(), email, passwordHasher.Hash(command.Password), clock.UtcNow);
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
        => new(user.Id, user.Email, user.CreatedAtUtc, user.Role);
}
