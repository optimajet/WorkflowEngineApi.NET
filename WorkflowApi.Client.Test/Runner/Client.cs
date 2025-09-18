using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;

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
        var jwtResponse = await Auth.AuthLoginAsync(
            Configuration.AppCredentials.Name, 
            Configuration.AppCredentials.Password
        );
        
        _jwt = JsonConvert.DeserializeObject<string>(jwtResponse) ?? "";
    }
    
    public async Task<string> CreateTokenAsync(List<string> permissions)
    {
        var hash = Hash.GenerateStringHash(permissions);
        var api = Auth;
        api.Configuration.DefaultHeaders.Remove("Authorization");
        await api.AuthRegisterAsync(new RegisterRequest(hash, hash, permissions));
        return await api.AuthLoginAsync(hash, hash);
    }
    
    public TestService Service { get; }
    public TestConfiguration Configuration => Service.Configuration;
    public WorkflowApi.Client.Client.Configuration LocalClientConfiguration => CreateConfiguration();
    
    public AuthApi Auth => new(LocalClientConfiguration);
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

    public TApi ExclusivePermissions<TApi>(Func<Client, TApi> apiSelector, string permission) where TApi : IApiAccessor
    {
        return ExclusivePermissions(apiSelector, [permission]);
    }

    public TApi ExclusivePermissions<TApi>(Func<Client, TApi> apiSelector, string[] permissions) where TApi : IApiAccessor
    {
        var api = apiSelector(this);
        var token = CreateTokenAsync(permissions.ToList()).Result;
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
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

    private string _jwt = "";
}
