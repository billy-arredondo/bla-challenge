namespace BlaChallenge.Application.Tasks.Contracts;

using BlaChallenge.Domain;

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime DueDateUtc,
    Guid OwnerId);
