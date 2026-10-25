using BlaChallenge.Domain;

namespace BlaChallenge.Domain.Tests;

public sealed class UserTests
{
    [Fact]
    public void Create_normalizes_email()
    {
        var user = User.Create(Guid.NewGuid(), "  USER@Example.COM ", "hash", DateTime.UtcNow);

        Assert.Equal("user@example.com", user.Email);
    }

    [Fact]
    public void Create_rejects_empty_email()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            User.Create(Guid.NewGuid(), " ", "hash", DateTime.UtcNow));

        Assert.Equal("email", exception.ParamName);
    }
}
