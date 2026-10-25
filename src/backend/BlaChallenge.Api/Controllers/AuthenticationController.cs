using System.Security.Claims;
using BlaChallenge.Application.Users.Commands.LoginUser;
using BlaChallenge.Application.Users.Commands.RegisterUser;
using BlaChallenge.Application.Users.Contracts;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Application.Users.Queries.GetCurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlaChallenge.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly RegisterUserUseCase registerUser;
    private readonly LoginUserUseCase loginUser;
    private readonly GetCurrentUserUseCase getCurrentUser;

    public AuthenticationController(
        RegisterUserUseCase registerUser,
        LoginUserUseCase loginUser,
        GetCurrentUserUseCase getCurrentUser)
    {
        this.registerUser = registerUser;
        this.loginUser = loginUser;
        this.getCurrentUser = getCurrentUser;
    }

    [AllowAnonymous]
    [HttpGet("ping")]
    public IActionResult Ping() => Ok(new { status = "ok" });

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await registerUser.ExecuteAsync(
                new RegisterUserCommand(request.Email, request.Password), cancellationToken);
            return Created($"api/auth/users/{result.User.Id}", result.User);
        }
        catch (InvalidUserInputException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (DuplicateUserException exception)
        {
            return Conflict(new { error = exception.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequestModel request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await loginUser.ExecuteAsync(
                new LoginUserCommand(request.Email, request.Password), cancellationToken);
            return Ok(new LoginResponse(result.User, result.AccessToken));
        }
        catch (InvalidUserInputException)
        {
            return Unauthorized(new { error = "Invalid email or password." });
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { error = "Invalid email or password." });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        var user = await getCurrentUser.ExecuteAsync(new GetCurrentUserQuery(userId), cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    public sealed record RegisterRequest(string Email, string Password);

    public sealed record LoginRequestModel(string Email, string Password);

    public sealed record LoginResponse(UserResponse User, string AccessToken);
}
