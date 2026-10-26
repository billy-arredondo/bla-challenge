namespace BlaChallenge.Application.Tasks.Contracts;

using BlaChallenge.Domain;

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime DueDateUtc,
    Guid OwnerId,
    Guid CreatedById,
    Guid AssignedToId,
    Guid? UpdatedById,
    DateTime? UpdatedAtUtc);
