using OptimaJet.Workflow.Api.Options;
using OptimaJet.Workflow.Core;

namespace WorkflowApi;

/// <summary>
/// Configuration class for the WorkflowApi.
/// </summary>
public record Configuration
{
    /// <summary>
    /// Basic settings of the Workflow Engine API.
    /// If the <see cref="MultipleTenantMode"/> is set to <c>true</c>,
    /// <see cref="OptimaJet.Workflow.Api.Options.WorkflowApiCoreOptions.DefaultTenantId"/> property will be forced to null.
    /// </summary>
    public WorkflowApiCoreOptions WorkflowApiCoreOptions { get; set; } = new();

    /// <summary>
    /// Security settings of the Workflow Engine API.
    /// </summary>
    public WorkflowApiSecurityOptions WorkflowApiSecurityOptions { get; set; } = new();

    /// <summary>
    /// Workflow Engine runtime options for the Workflow Engine API in single tenant mode.
    /// If the <see cref="MultipleTenantMode"/> is set to <c>true</c>, this option will be ignored.
    /// </summary>
    public WorkflowEngineTenantCreationOptions WorkflowEngineTenantCreationOptions { get; set; } = new()
    {
        DataProviderId = PersistenceProviderId.Sqlite,
        ConnectionString = DefaultConnectionStrings.Sqlite
    };
    
    /// <summary>
    /// Indicates whether the Workflow Engine API is running in multiple tenant mode.
    /// </summary>
    public bool MultipleTenantMode { get; set; }
    
    /// <summary>
    /// Workflow Engine runtime tenants configuration for the Workflow Engine API in multiple tenant mode.
    /// If the <see cref="MultipleTenantMode"/> is set to <c>false</c>, this option will be ignored.
    /// </summary>
    public WorkflowEngineTenantCreationOptions[] TenantsConfiguration { get; set; } = [];

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
    /// Connection strings for the database providers.
    /// </summary>
    public Dictionary<string, string> ConnectionStrings { get; set; } = new()
    {
        ["Default"] = "InMemoryDatabase"
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
/// Default connection strings for the database providers.
/// </summary>
public static class DefaultConnectionStrings
{
    public const string Mongo = "mongodb://localhost:47017/workflow-engine";
    public const string Mssql = "Server=localhost,41433;Database=workflow_engine;User Id=SA;Password=P@ssw0rd;TrustServerCertificate=True;";
    public const string Mysql = "Host=localhost;Port=43306;Database=workflow_engine;User ID=root;Password=P@ssw0rd;";
    public const string Oracle = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=41523))(CONNECT_DATA=(SERVICE_NAME=FREEPDB1)));User Id=WORKFLOW_ENGINE;Password=password;";
    public const string Postgres = "Host=localhost;Port=45432;Database=workflow_engine;User Id=postgres;Password=P@ssw0rd;";
    public const string Sqlite = "Data Source=workflow_engine.db";
}
