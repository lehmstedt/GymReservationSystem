namespace Infrastructure;

public class DatabaseOptions
{
    public const string Database = "Database";
    public string ConnectionString { get; init; } = string.Empty;
}