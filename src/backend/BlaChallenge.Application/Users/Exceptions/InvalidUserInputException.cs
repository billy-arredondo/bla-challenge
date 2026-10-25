namespace BlaChallenge.Application.Users.Exceptions;

public sealed class InvalidUserInputException : Exception
{
    public InvalidUserInputException(string message)
        : base(message)
    {
    }
}
