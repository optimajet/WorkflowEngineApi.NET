using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Runner;

public class Client
{
    private Client(TestConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public static async Task<Client> CreateAsync(TestConfiguration configuration)
    {
        var client = new Client(configuration);
        await client.AuthorizeAsync();
        return client;
    }

    public async Task AuthorizeAsync()
    {
        var appJwt = await Auth.AuthLoginAsync(
            Configuration.AppCredentials.Name, 
            Configuration.AppCredentials.Password
        );

        var authorization = $"Bearer {JsonConvert.DeserializeObject<string>(appJwt)}";
        
        Configuration.ClientConfiguration.DefaultHeaders["Authorization"] = authorization;
    }
    
    public async Task<string> CreateTokenAsync(List<string> permissions)
    {
        var hash = Hash.GenerateStringHash(permissions);
        await Auth.AuthRegisterAsync(new RegisterRequest(hash, hash, permissions));
        return await Auth.AuthLoginAsync(hash, hash);
    }
    
    public TestConfiguration Configuration { get; }
    
    public AuthApi Auth => new(Configuration.ClientConfiguration);
    public WorkflowApiApi WorkflowApi => new(Configuration.ClientConfiguration);
    public ApprovalsApi Approvals => new(Configuration.ClientConfiguration);
    public GlobalParametersApi GlobalParameters => new(Configuration.ClientConfiguration);
    public InboxEntriesApi InboxEntries => new(Configuration.ClientConfiguration);
    public ParametersApi Parameters => new(Configuration.ClientConfiguration);
    public ProcessesApi Processes => new(Configuration.ClientConfiguration);
    public RuntimesApi Runtimes => new(Configuration.ClientConfiguration);
    public SchemesApi Schemes => new(Configuration.ClientConfiguration);
    public StatusesApi Statuses => new(Configuration.ClientConfiguration);
    public TimersApi Timers => new(Configuration.ClientConfiguration);
    public TransitionsApi Transitions => new(Configuration.ClientConfiguration);
}
