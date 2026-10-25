namespace BlaChallenge.Application.Users.Contracts;

public sealed record LoginResult(UserResponse User, string AccessToken);
