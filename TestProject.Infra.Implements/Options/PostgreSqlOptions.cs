using Npgsql;

namespace TestProject.Infra.Implements.Options;

public class PostgreSqlOptions
{
    public required string Host { get; init; }

    public required int Port { get; init; }

    public required string DatabaseName { get; init; }

    public required string WorkUserName { get; init; }

    public required string WorkUserPassword { get; init; }

    public required string InitUserName { get; init; }

    public required string InitUserPassword { get; init; }

    public string WorkConnectionString => GetWorkConnectionString();

    public string InitConnectionString => GetInitConnectionString();

    public string TruncateConnectionString => GetTruncateConnectionString();

    private string GetWorkConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = DatabaseName,
            Username = WorkUserName,
            Password = WorkUserPassword
        };

        return builder.ToString();
    }

    private string GetInitConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = "postgres",
            Username = InitUserName,
            Password = InitUserPassword
        };

        return builder.ToString();
    }

    private string GetTruncateConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = DatabaseName,
            Username = InitUserName,
            Password = InitUserPassword
        };

        return builder.ToString();
    }
}
