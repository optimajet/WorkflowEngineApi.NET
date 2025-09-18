namespace WorkflowApi.Client.Test.Models;

public class DisposableCollection : IDisposable, IAsyncDisposable
{
    public DisposableCollection(params IDisposable[] disposables)
    {
        foreach (var disposable in disposables)
        {
            if (disposable is IAsyncDisposable asyncDisposable)
            {
                _asyncDisposables.Add(asyncDisposable);
            }
            else
            {
                _disposables.Add(disposable);
            }
        }
    }

    private readonly List<IDisposable> _disposables = new();
    private readonly List<IAsyncDisposable> _asyncDisposables = new();

    #region IDisposable implementation

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncInternal();
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncInternal()
    {
        foreach (var disposable in _asyncDisposables.ToList())
        {
            await disposable.DisposeAsync();
            _asyncDisposables.Remove(disposable);
        }

        foreach (var disposable in _disposables.ToList())
        {
            disposable.Dispose();
            _disposables.Remove(disposable);
        }
    }

    #endregion
}