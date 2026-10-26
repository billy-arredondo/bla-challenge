namespace BlaChallenge.Application.Tasks.Exceptions;

public sealed class TaskNotFoundException(Guid id) : Exception($"Task '{id}' was not found.");
