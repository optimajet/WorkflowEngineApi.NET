using Microsoft.Extensions.DependencyInjection;
using WorkflowApi.Client.Test.Repositories;
using WorkflowApi.Client.Test.Repositories.Mongo;
using WorkflowApi.Client.Test.Repositories.Sql;
using WorkflowApi.Exceptions;

namespace WorkflowApi.Client.Test.Runner;

public sealed class TestService : IDisposable
{
    public static async Task<TestService> CreateAsync(TestConfiguration configuration)
    {
        var service = new TestService(configuration);
        await service.InitializeAsync();
        return service;
    }
    
    private TestService(TestConfiguration configuration)
    {
        Configuration = configuration;
        Repository = Configuration.AppConfiguration.Provider switch
        {
            Provider.Mongo => new MongoRepository(this),
            Provider.Mssql => new SqlRepository(this),
            Provider.Mysql => new SqlRepository(this),
            Provider.Oracle => new SqlRepository(this),
            Provider.Postgres => new SqlRepository(this),
            Provider.Sqlite => new SqlRepository(this),
            _ => throw new ArgumentOutOfRangeException(nameof(Provider))
        };
    }

    private async Task InitializeAsync()
    {
        var builder = new AppBuilder(Configuration.AppArgs);
        builder.Builder.Configuration.AddObject(Configuration.AppConfiguration);
        builder.Builder.Services.AddMvc().AddApplicationPart(typeof(AppBuilder).Assembly);
        _host = new Host(builder);
        
        await _host.StartAsync(Configuration.Port);
        
        Configuration.ClientConfiguration.BasePath = _host.Uri;
        _client = await Client.CreateAsync(Configuration);
    }

    public string Name => Configuration.Name;
    public TestConfiguration Configuration { get; }
    public Client Client => _client ?? throw new NotInitializedException(nameof(Client));
    public IRepository Repository { get; }
    
    private Host? _host;
    private Client? _client;

    #region IDisposable Implementation

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing) return;
        _host?.Dispose();
    }

    #endregion

    public override int GetHashCode()
    {
        return Configuration.GetHashCode();
    }
}