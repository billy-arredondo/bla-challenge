using BlaChallenge.Domain;

namespace BlaChallenge.Application.Users.Abstractions;

public interface ITokenService
{
    string CreateToken(User user);
}
