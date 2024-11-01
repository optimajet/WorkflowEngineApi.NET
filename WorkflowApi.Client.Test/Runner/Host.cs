using Microsoft.AspNetCore.Builder;

namespace WorkflowApi.Client.Test.Runner;

public class Host : IDisposable
{
    public Host(AppBuilder builder)
    {
        _app = builder.Build();
    }
    
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