namespace BlaChallenge.Application.Users.Abstractions;

using BlaChallenge.Domain;

public interface ITokenService
{
    string CreateToken(User user);
}
