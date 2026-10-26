namespace BlaChallenge.Application.Users.Exceptions;

public sealed class UserManagementForbiddenException : Exception
{
    public UserManagementForbiddenException(string message)
        : base(message)
    {
    }
}
