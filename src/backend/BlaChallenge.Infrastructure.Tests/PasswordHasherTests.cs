using BlaChallenge.Infrastructure.Authentication;

namespace BlaChallenge.Infrastructure.Tests;

public sealed class PasswordHasherTests
{
    [Fact]
    public void Hash_does_not_store_plaintext_and_verifies_original_password()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.Hash("password123");

        Assert.DoesNotContain("password123", hash);
        Assert.True(hasher.Verify("password123", hash));
        Assert.False(hasher.Verify("wrong-password", hash));
    }
}
