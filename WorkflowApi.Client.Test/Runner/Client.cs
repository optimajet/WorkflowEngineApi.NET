using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Client;

namespace WorkflowApi.Client.Test.Runner;

public class Client
{
    private Client(TestService service)
    {
        Service = service;
    }
    
    public static async Task<Client> CreateAsync(TestService service)
    {
        var client = new Client(service);
        await client.AuthorizeAsync();
        return client;
    }

    public async Task AuthorizeAsync()
    {
        _jwt = await CreateTokenAsync(permissions => permissions.AllowAllOperations().AllowAllTenants());
    }

    public Task<string> CreateTokenAsync(List<string> permissions, List<string>? tenantIds = null)
    {
        return CreateTokenAsync(builder => ConfigurePermissions(builder, permissions, tenantIds));
    }

    public Task<string> CreateTokenAsync(Action<IWorkflowApiPermissionsBuilder> configurePermissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.AppConfiguration.Jwt.Key));
        
        Claim[] claims = 
        [
            Service.Host.PermissionsService.BuildClaim(configurePermissions),
            new (ClaimTypes.Name, Configuration.AppCredentials.Name)
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMilliseconds(Configuration.AppConfiguration.Jwt.Expires),
            Issuer = Configuration.AppConfiguration.Jwt.Issuer,
            Audience = Configuration.AppConfiguration.Jwt.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
    
    public TestService Service { get; }
    public TestConfiguration Configuration => Service.Configuration;
    public WorkflowApi.Client.Client.Configuration LocalClientConfiguration => CreateConfiguration();

    public RootApi RootApi => new(LocalClientConfiguration);
    
    //Data & Search
    public ApprovalsApi Approvals => new(LocalClientConfiguration);
    public GlobalParametersApi GlobalParameters => new(LocalClientConfiguration);
    public InboxEntriesApi InboxEntries => new(LocalClientConfiguration);
    public ParametersApi Parameters => new(LocalClientConfiguration);
    public ProcessesApi Processes => new(LocalClientConfiguration);
    public RuntimesApi Runtimes => new(LocalClientConfiguration);
    public SchemesApi Schemes => new(LocalClientConfiguration);
    public StatusesApi Statuses => new(LocalClientConfiguration);
    public TimersApi Timers => new(LocalClientConfiguration);
    public TransitionsApi Transitions => new(LocalClientConfiguration);
    
    //Rpc
    public RpcBulkApi RpcBulk => new(LocalClientConfiguration);
    public RpcCommandsApi RpcCommands => new(LocalClientConfiguration);
    public RpcInstanceApi RpcInstance => new(LocalClientConfiguration);
    public RpcLogApi RpcLog => new(LocalClientConfiguration);
    public RpcPreExecutionApi RpcPreExecution => new(LocalClientConfiguration);
    public RpcRuntimeApi RpcRuntime => new(LocalClientConfiguration);
    public RpcSchemeApi RpcScheme => new(LocalClientConfiguration);
    public RpcStateApi RpcStates => new(LocalClientConfiguration);
    
    public TApi WithPermissions<TApi>(Func<Client, TApi> apiSelector, string permission) where TApi : IApiAccessor
    {
        return WithPermissions(apiSelector, [permission]);
    }

    public TApi WithPermissions<TApi>(Func<Client, TApi> apiSelector, string[] permissions, string[]? tenantIds = null) where TApi : IApiAccessor
    {
        return WithPermissions(apiSelector, builder => ConfigurePermissions(builder, permissions, tenantIds));
    }

    public TApi WithPermissions<TApi>(Func<Client, TApi> apiSelector, Action<IWorkflowApiPermissionsBuilder> configurePermissions) where TApi : IApiAccessor
    {
        var api = apiSelector(this);
        var token = CreateTokenAsync(configurePermissions).Result;
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {token}";
        return api;
    }

    private WorkflowApi.Client.Client.Configuration CreateConfiguration()
    {
        var oldConfiguration = Configuration.ClientConfiguration;
        
        var newConfiguration = new WorkflowApi.Client.Client.Configuration
        {
            BasePath = Service.Host.Uri,
            DefaultHeaders =
            {
                [WorkflowApiConstants.TenantIdHeader] = Service.TenantId,
                ["Authorization"] = $"Bearer {_jwt}",
            }
        };
        
        var apiKey = oldConfiguration.ApiKey.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        var apiKeyPrefix = oldConfiguration.ApiKeyPrefix.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        var defaultHeaders = oldConfiguration.DefaultHeaders.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var kvp in newConfiguration.ApiKey) apiKey[kvp.Key] = kvp.Value;
        foreach (var kvp in newConfiguration.ApiKeyPrefix) apiKeyPrefix[kvp.Key] = kvp.Value;
        foreach (var kvp in newConfiguration.DefaultHeaders) defaultHeaders[kvp.Key] = kvp.Value;

        var configuration = new WorkflowApi.Client.Client.Configuration
        {
            ApiKey = apiKey,
            ApiKeyPrefix = apiKeyPrefix,
            DefaultHeaders = defaultHeaders,
            BasePath = newConfiguration.BasePath ?? oldConfiguration.BasePath,
            Timeout = newConfiguration.Timeout,
            Proxy = newConfiguration.Proxy ?? oldConfiguration.Proxy,
            UserAgent = newConfiguration.UserAgent ?? oldConfiguration.UserAgent,
            Username = newConfiguration.Username ?? oldConfiguration.Username,
            Password = newConfiguration.Password ?? oldConfiguration.Password,
            AccessToken = newConfiguration.AccessToken ?? oldConfiguration.AccessToken,
            TempFolderPath = newConfiguration.TempFolderPath ?? oldConfiguration.TempFolderPath,
            DateTimeFormat = newConfiguration.DateTimeFormat ?? oldConfiguration.DateTimeFormat,
            ClientCertificates = newConfiguration.ClientCertificates ?? oldConfiguration.ClientCertificates,
            UseDefaultCredentials = newConfiguration.UseDefaultCredentials,
            RemoteCertificateValidationCallback = newConfiguration.RemoteCertificateValidationCallback ?? oldConfiguration.RemoteCertificateValidationCallback,
        };
        
        return configuration;
    }

    private void ConfigurePermissions(
        IWorkflowApiPermissionsBuilder builder,
        IEnumerable<string> permissions,
        IEnumerable<string>? tenantIds
    )
    {
        var configuredBuilder = builder.DenyAllOperations().Allow(permissions);

        if (tenantIds == null && Service.TenantId == WorkflowApiConstants.SingleTenantId)
        {
            return;
        }

        configuredBuilder.DenyAllTenantsExcept(tenantIds ?? [Service.TenantId]);
    }

    private string _jwt = "";
}
