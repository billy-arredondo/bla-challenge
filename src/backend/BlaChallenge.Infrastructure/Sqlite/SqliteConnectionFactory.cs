namespace BlaChallenge.Infrastructure.Sqlite;

using Microsoft.Data.Sqlite;

public sealed class SqliteConnectionFactory(string connectionString)
{
    public SqliteConnection Create() => new(connectionString);
}
