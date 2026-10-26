namespace BlaChallenge.Application.Tasks.Queries.GetTasks;

using BlaChallenge.Domain;

public sealed record GetTasksQuery(Guid OwnerId, UserRole ActorRole = UserRole.User);
