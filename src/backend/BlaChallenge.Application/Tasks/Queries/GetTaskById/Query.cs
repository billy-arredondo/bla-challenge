namespace BlaChallenge.Application.Tasks.Queries.GetTaskById;

using BlaChallenge.Domain;

public sealed record GetTaskByIdQuery(Guid Id, Guid OwnerId, UserRole ActorRole = UserRole.User);
