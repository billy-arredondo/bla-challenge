namespace BlaChallenge.Application.Tasks.Commands.DeleteTask;

using BlaChallenge.Domain;

public sealed record DeleteTaskCommand(Guid Id, Guid OwnerId, UserRole ActorRole = UserRole.User);
