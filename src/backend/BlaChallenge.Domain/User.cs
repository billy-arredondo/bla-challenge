namespace BlaChallenge.Domain;

public sealed class User
{
    private User(Guid id, string email, string passwordHash, DateTime createdAtUtc, UserRole role)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
        Role = role;
    }

    public Guid Id { get; }

    public string Email { get; }

    public string PasswordHash { get; }

    public DateTime CreatedAtUtc { get; }

    public UserRole Role { get; private set; }

    public static User Create(
        Guid id,
        string email,
        string passwordHash,
        DateTime createdAtUtc,
        UserRole role = UserRole.User)
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

        if (!Enum.IsDefined(role))
        {
            throw new ArgumentException("User role is invalid.", nameof(role));
        }

        return new User(id, email.Trim().ToLowerInvariant(), passwordHash, createdAtUtc, role);
    }

    public void ChangeRole(UserRole role)
    {
        if (Role == UserRole.Superadmin || role == UserRole.Superadmin)
        {
            throw new InvalidOperationException("Superadmin privileges cannot be changed.");
        }

        if (!Enum.IsDefined(role))
        {
            throw new ArgumentException("User role is invalid.", nameof(role));
        }

        Role = role;
    }
}
