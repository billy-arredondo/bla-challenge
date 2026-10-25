namespace BlaChallenge.Application.Users.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}
