using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface IRuntimeRepository
{
    Task CreateAsync(params RuntimeModel[] runtimes);
}