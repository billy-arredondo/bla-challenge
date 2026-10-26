namespace BlaChallenge.Application.Tasks.Commands.UpdateTask;

using BlaChallenge.Domain;

public sealed record UpdateTaskCommand(
    Guid Id,
    Guid OwnerId,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime DueDateUtc);
