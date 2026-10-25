namespace BlaChallenge.Domain;

public sealed class User
{
    private User(Guid id, string email, string passwordHash, DateTime createdAtUtc)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }

    public string Email { get; }

    public string PasswordHash { get; }

    public DateTime CreatedAtUtc { get; }

    public static User Create(Guid id, string email, string passwordHash, DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        return new User(id, email.Trim().ToLowerInvariant(), passwordHash, createdAtUtc);
    }
}
