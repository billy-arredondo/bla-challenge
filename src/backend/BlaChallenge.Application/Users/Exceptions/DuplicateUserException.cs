namespace BlaChallenge.Application.Users.Exceptions;

public sealed class DuplicateUserException : Exception
{
    public DuplicateUserException(string email)
        : base($"A user with email '{email}' already exists.")
    {
    }
}
