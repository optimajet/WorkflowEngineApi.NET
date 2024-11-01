using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface IProcessRepository
{
    Task<ProcessModel?> GetAsync(Guid id);
    Task CreateAsync(params ProcessModel[] processes);
}