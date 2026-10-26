namespace BlaChallenge.Application.Users.Commands.ManageUsers;

using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Application.Users.Commands.RegisterUser;
using BlaChallenge.Application.Users.Contracts;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Domain;

public sealed class ManageUsersUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IClock clock)
{
    public async Task<UserResponse> CreateAsync(
        CreateManagedUserCommand command,
        UserRole actorRole,
        CancellationToken cancellationToken = default)
    {
        EnsureSuperadmin(actorRole);
        var email = RegisterUserUseCase.NormalizeEmail(command.Email);
        RegisterUserUseCase.ValidatePassword(command.Password);
        if (command.Role == UserRole.Superadmin)
        {
            throw new UserManagementForbiddenException("Superadmin accounts can only be created through secure configuration.");
        }

        if (await userRepository.FindByEmailAsync(email, cancellationToken) is not null)
        {
            throw new DuplicateUserException(email);
        }

        var user = User.Create(Guid.NewGuid(), email, passwordHasher.Hash(command.Password), clock.UtcNow, command.Role);
        await userRepository.AddAsync(user, cancellationToken);
        return RegisterUserUseCase.ToResponse(user);
    }

    public async Task<UserResponse> ChangeRoleAsync(
        ChangeUserRoleCommand command,
        UserRole actorRole,
        CancellationToken cancellationToken = default)
    {
        EnsureSuperadmin(actorRole);
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken)
            ?? throw new UserManagementForbiddenException("The target user was not found.");
        if (user.Role == UserRole.Superadmin || command.Role == UserRole.Superadmin)
        {
            throw new UserManagementForbiddenException("Superadmin privileges cannot be changed.");
        }

        user.ChangeRole(command.Role);
        await userRepository.UpdateRoleAsync(user, cancellationToken);
        return RegisterUserUseCase.ToResponse(user);
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(
        UserRole actorRole,
        CancellationToken cancellationToken = default)
    {
        EnsureSuperadmin(actorRole);
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(RegisterUserUseCase.ToResponse).ToList();
    }

    private static void EnsureSuperadmin(UserRole role)
    {
        if (role != UserRole.Superadmin)
        {
            throw new UserManagementForbiddenException("Only a Superadmin can manage users.");
        }
    }
}
