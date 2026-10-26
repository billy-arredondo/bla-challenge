namespace BlaChallenge.Application.Tests;

using BlaChallenge.Application.Users.Abstractions;
using BlaChallenge.Application.Users.Commands.LoginUser;
using BlaChallenge.Application.Users.Commands.RegisterUser;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Application.Users.Queries.GetCurrentUser;
using BlaChallenge.Domain;

public sealed class AuthenticationTests
{
    [Fact]
    public async Task Register_rejects_duplicate_email()
    {
        var repository = new FakeUserRepository
        {
            ExistingUser = User.Create(Guid.NewGuid(), "user@example.com", "hash", DateTime.UtcNow)
        };
        var useCase = new RegisterUserUseCase(repository, new FakePasswordHasher(), new FakeClock());

        await Assert.ThrowsAsync<DuplicateUserException>(() =>
            useCase.ExecuteAsync(new RegisterUserCommand("USER@example.com", "password123")));
    }

    [Fact]
    public async Task Register_hashes_password_and_returns_user()
    {
        var repository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var useCase = new RegisterUserUseCase(repository, passwordHasher, new FakeClock());

        var result = await useCase.ExecuteAsync(new RegisterUserCommand("USER@example.com", "password123"));

        Assert.Equal("user@example.com", result.User.Email);
        Assert.NotEqual("password123", repository.AddedUser!.PasswordHash);
        Assert.Equal(1, passwordHasher.HashCalls);
    }

    [Fact]
    public async Task Login_rejects_invalid_password()
    {
        var repository = new FakeUserRepository
        {
            ExistingUser = User.Create(Guid.NewGuid(), "user@example.com", "stored-hash", DateTime.UtcNow)
        };
        var useCase = new LoginUserUseCase(repository, new FakePasswordHasher { VerifyResult = false }, new FakeTokenService());

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            useCase.ExecuteAsync(new LoginUserCommand("user@example.com", "wrong-password")));
    }

    [Fact]
    public async Task Register_rejects_short_password()
    {
        var useCase = new RegisterUserUseCase(new FakeUserRepository(), new FakePasswordHasher(), new FakeClock());

        await Assert.ThrowsAsync<InvalidUserInputException>(() =>
            useCase.ExecuteAsync(new RegisterUserCommand("user@example.com", "short")));
    }

    [Fact]
    public async Task Get_current_user_returns_user_response()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "stored-hash", DateTime.UtcNow);
        var useCase = new GetCurrentUserUseCase(new FakeUserRepository { ExistingUser = user });

        var result = await useCase.ExecuteAsync(new GetCurrentUserQuery(user.Id));

        Assert.Equal(user.Id, result!.Id);
        Assert.Equal(user.Email, result.Email);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public User? ExistingUser { get; init; }

        public User? AddedUser { get; private set; }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            AddedUser = user;
            return Task.CompletedTask;
        }

        public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(ExistingUser?.Email == email.ToLowerInvariant() ? ExistingUser : null);

        public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(ExistingUser?.Id == id ? ExistingUser : null);

        public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(ExistingUser is null ? [] : [ExistingUser]);

        public Task UpdateRoleAsync(User user, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public bool VerifyResult { get; init; } = true;

        public int HashCalls { get; private set; }

        public string Hash(string password)
        {
            HashCalls++;
            return $"hashed:{password}";
        }

        public bool Verify(string password, string passwordHash) => VerifyResult;
    }

    private sealed class FakeTokenService : ITokenService
    {
        public string CreateToken(User user) => "token";
    }

    private sealed class FakeClock : IClock
    {
        public DateTime UtcNow => new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
