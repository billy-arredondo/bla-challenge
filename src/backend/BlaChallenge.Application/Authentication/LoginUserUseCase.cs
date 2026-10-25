namespace BlaChallenge.Application.Authentication;

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
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = RegisterUserUseCase.NormalizeEmail(request.Email);
        RegisterUserUseCase.ValidatePassword(request.Password);
        var user = await userRepository.FindByEmailAsync(email, cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        return new LoginResult(RegisterUserUseCase.ToResponse(user), tokenService.CreateToken(user));
    }
}
