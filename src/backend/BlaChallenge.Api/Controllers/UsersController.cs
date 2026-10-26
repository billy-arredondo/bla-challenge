namespace BlaChallenge.Api.Controllers;

using System.Security.Claims;
using BlaChallenge.Application.Users.Commands.ManageUsers;
using BlaChallenge.Application.Users.Contracts;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(ManageUsersUseCase manageUsers) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Superadmin)}")]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
        => Ok(await manageUsers.GetAllAsync(GetRole(), cancellationToken));

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Superadmin))]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await manageUsers.CreateAsync(
                new CreateManagedUserCommand(request.Email, request.Password, request.Role),
                GetRole(), cancellationToken);
            return Created($"api/users/{user.Id}", user);
        }
        catch (InvalidUserInputException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (DuplicateUserException)
        {
            return Conflict(new { error = "Unable to create the account with the provided information." });
        }
        catch (UserManagementForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = exception.Message });
        }
    }

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = nameof(UserRole.Superadmin))]
    public async Task<ActionResult<UserResponse>> ChangeRole(
        Guid id,
        ChangeRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await manageUsers.ChangeRoleAsync(
                new ChangeUserRoleCommand(id, request.Role), GetRole(), cancellationToken));
        }
        catch (UserManagementForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = exception.Message });
        }
    }

    private UserRole GetRole()
        => Enum.TryParse<UserRole>(User.FindFirstValue(ClaimTypes.Role), out var role) ? role : UserRole.User;

    public sealed record CreateUserRequest(string Email, string Password, UserRole Role);

    public sealed record ChangeRoleRequest(UserRole Role);
}
