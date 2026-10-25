namespace BlaChallenge.Application.Users.Contracts;

public sealed record UserResponse(Guid Id, string Email, DateTime CreatedAtUtc);
