namespace BlaChallenge.Application.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id, Guid OwnerId);
