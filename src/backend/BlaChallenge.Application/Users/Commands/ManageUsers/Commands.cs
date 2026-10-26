namespace BlaChallenge.Application.Users.Commands.ManageUsers;

using BlaChallenge.Domain;

public sealed record CreateManagedUserCommand(string Email, string Password, UserRole Role);

public sealed record ChangeUserRoleCommand(Guid UserId, UserRole Role);
