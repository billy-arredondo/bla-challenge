using System.Security.Claims;
using BlaChallenge.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlaChallenge.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly RegisterUserUseCase registerUser;
    private readonly LoginUserUseCase loginUser;

    public AuthenticationController(RegisterUserUseCase registerUser, LoginUserUseCase loginUser)
    {
        this.registerUser = registerUser;
        this.loginUser = loginUser;
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
                new RegisterUserRequest(request.Email, request.Password), cancellationToken);
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
                new BlaChallenge.Application.Authentication.LoginRequest(request.Email, request.Password), cancellationToken);
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
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        return Ok(new { id = userId, email });
    }

    public sealed record RegisterRequest(string Email, string Password);

    public sealed record LoginRequestModel(string Email, string Password);

    public sealed record LoginResponse(UserResponse User, string AccessToken);
}
