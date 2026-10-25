using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlaChallenge.Application.Authentication;
using BlaChallenge.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlaChallenge.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "BlaChallenge";

    public string Audience { get; set; } = "BlaChallenge.Client";

    public string Key { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 60;
}

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        this.options = options.Value;
    }

    public string CreateToken(User user)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
            SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(options.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
