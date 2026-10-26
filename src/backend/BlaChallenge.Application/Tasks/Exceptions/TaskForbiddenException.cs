namespace BlaChallenge.Application.Tasks.Exceptions;

public sealed class TaskForbiddenException(string message) : Exception(message);
