namespace BlaChallenge.Application.Tasks.Commands.CreateTask;

using BlaChallenge.Domain;

public sealed record CreateTaskCommand(
    Guid OwnerId,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime DueDateUtc);
