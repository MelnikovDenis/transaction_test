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
        var builder = new NpgsqlConnectionStringBuilder();

        builder.Host = Host;
        builder.Port = Port;
        builder.Database = DatabaseName;
        builder.Username = WorkUserName;
        builder.Password = WorkUserPassword;

        return builder.ToString();
    }

    private string GetInitConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder();

        builder.Host = Host;
        builder.Port = Port;
        builder.Database = "postgres";
        builder.Username = InitUserName;
        builder.Password = InitUserPassword;

        return builder.ToString();
    }

    private string GetTruncateConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder();

        builder.Host = Host;
        builder.Port = Port;
        builder.Database = DatabaseName;
        builder.Username = InitUserName;
        builder.Password = InitUserPassword;

        return builder.ToString();
    }
}
