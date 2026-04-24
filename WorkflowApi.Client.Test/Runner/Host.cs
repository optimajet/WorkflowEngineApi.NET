using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OptimaJet.Workflow.Api;

namespace WorkflowApi.Client.Test.Runner;

public class Host : IDisposable
{
    public static async Task<Host> CreateAsync(TestConfiguration configuration)
    {
        var builder = new AppBuilder(configuration.AppArgs);
        builder.Builder.Configuration.AddObject(configuration.AppConfiguration);
        builder.Builder.Services.AddMvc().AddApplicationPart(typeof(AppBuilder).Assembly);
        var host = new Host(configuration, builder);
        
        await host.StartAsync(configuration.Port);
        
        host.Configuration.ClientConfiguration.BasePath = host.Uri;
        
        var services = await Task.WhenAll(host.TenantRegistry.Tenants
            .SelectMany(tenant => tenant.Ids)
            .Select(id => TestService.CreateAsync(host, id))
        );
        
        host._testServices.AddRange(services);
        
        return host;
    }
    
    private Host(TestConfiguration configuration, AppBuilder builder)
    {
        Configuration = configuration;
        _app = builder.Build();
        TenantRegistry = _app.Services.GetRequiredService<IWorkflowTenantRegistry>();
    }
    
    public TestConfiguration Configuration { get; }
    public IReadOnlyCollection<TestService> TestServices => _testServices;
    public IWorkflowTenantRegistry TenantRegistry { get; }
    public IWorkflowApiPermissions PermissionsService => _app.Services.GetRequiredService<IWorkflowApiPermissions>();
    public string Id => Configuration.Id;
    public IReadOnlyCollection<string> TenantIds => TenantRegistry.Tenants.SelectMany(tenant => tenant.Ids).ToArray();
    public bool IsRunning { get; private set; }
    public string Uri => _app.Urls.First();
    
    public Task StartAsync(int port)
    {
        return StartAsync($"http://localhost:{port}/");
    }
    
    public Task StartAsync(string uri)
    {
        IsRunning = true;
        _app.RunAsync(uri);
        return Task.CompletedTask;
    }
    
    public async Task StopAsync()
    {
        await _app.StopAsync();
        IsRunning = false;
    }

    private readonly WebApplication _app;
    private readonly List<TestService> _testServices = [];
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    #region IDisposable Implementation

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        ((IDisposable) _app).Dispose();
    }

    #endregion
}
