namespace BlaChallenge.Application.Users.Commands.LoginUser;

using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Application.Users.Commands.RegisterUser;
using BlaChallenge.Application.Users.Contracts;
using BlaChallenge.Application.Users.Exceptions;

public sealed class LoginUserUseCase
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly ITokenService tokenService;

    public LoginUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.tokenService = tokenService;
    }

    public async Task<LoginResult> ExecuteAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var email = RegisterUserUseCase.NormalizeEmail(command.Email);
        RegisterUserUseCase.ValidatePassword(command.Password);
        var user = await userRepository.FindByEmailAsync(email, cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        return new LoginResult(RegisterUserUseCase.ToResponse(user), tokenService.CreateToken(user));
    }
}
