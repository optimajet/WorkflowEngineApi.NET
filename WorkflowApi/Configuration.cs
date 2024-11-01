using OptimaJet.Workflow.Api.Options;

namespace WorkflowApi;

/// <summary>
/// Configuration class for the WorkflowApi.
/// </summary>
public record Configuration
{
    /// <summary>
    /// Basic settings of the Workflow Engine API.
    /// </summary>
    public WorkflowApiCoreOptions WorkflowApiCoreOptions { get; set; } = new();

    /// <summary>
    /// Security settings of the Workflow Engine API.
    /// </summary>
    public WorkflowApiSecurityOptions WorkflowApiSecurityOptions { get; set; } = new();

    /// <summary>
    /// Jwt Authentication settings.
    /// </summary>
    public Jwt Jwt { get; set; } = new();

    /// <summary>
    /// Logging settings.
    /// </summary>
    public Logging Logging { get; set; } = new();

    /// <summary>
    /// Allowed hosts for the Workflow Engine API.
    /// </summary>
    public string AllowedHosts { get; set; } = "*";

    /// <summary>
    /// Database provider to use.
    /// </summary>
    public Provider Provider { get; set; } = Provider.Sqlite;

    /// <summary>
    /// Connection strings for the database providers.
    /// </summary>
    public Dictionary<string, string> ConnectionStrings { get; set; } = new()
    {
        ["Default"] = "InMemoryDatabase",
        ["Mongo"] = "mongodb://localhost:47017/workflow-engine",
        ["Mssql"] = "Server=localhost,41433;Database=master;User Id=SA;Password=P@ssw0rd;TrustServerCertificate=True;",
        ["Mysql"] = "Host=localhost;Port=43306;Database=workflow_engine;User ID=root;Password=P@ssw0rd;",
        ["Oracle"] = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=41521))(CONNECT_DATA=(SERVICE_NAME=FREEPDB1)));User Id=WORKFLOW_ENGINE;Password=password;",
        ["Postgres"] = "Host=localhost;Port=45432;Database=postgres;User Id=postgres;Password=P@ssw0rd;",
        ["Sqlite"] = "Data Source=workflow_engine.db"
    };
}

/// <summary>
/// Jwt Authentication settings.
/// </summary>
public record Jwt
{
    /// <summary>
    /// The key to use for Jwt Authentication.
    /// </summary>
    public string Key { get; set; } = "7896dd52-90a7-4e3f-9e85-7986bf374f54";

    /// <summary>
    /// The issuer of the Jwt token.
    /// </summary>
    public string Issuer { get; set; } = "Default issuer";

    /// <summary>
    /// The audience of the Jwt token.
    /// </summary>
    public string Audience { get; set; } = "Default audience";

    /// <summary>
    /// The expiration time of the Jwt token specified in milliseconds.
    /// </summary>
    public long Expires { get; set; } = 24 * 60 * 60 * 1000; // 24 hours
}

/// <summary>
/// Logging settings.
/// </summary>
public record Logging
{
    public Dictionary<string, string> LogLevel { get; set; } = new()
    {
        ["Default"] = "Debug"
    };
}

/// <summary>
/// Enum representing the database providers.
/// </summary>
public enum Provider
{
    Mongo,
    Mssql,
    Mysql,
    Oracle,
    Postgres,
    Sqlite,
}
