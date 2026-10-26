namespace BlaChallenge.Application.Users.Contracts;

using BlaChallenge.Domain;

public sealed record UserResponse(Guid Id, string Email, DateTime CreatedAtUtc, UserRole Role);
