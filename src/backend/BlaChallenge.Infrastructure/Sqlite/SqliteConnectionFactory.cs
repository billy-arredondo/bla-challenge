using Microsoft.Data.Sqlite;

namespace BlaChallenge.Infrastructure.Sqlite;

public sealed class SqliteConnectionFactory
{
    private readonly string connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public SqliteConnection Create() => new(connectionString);
}
