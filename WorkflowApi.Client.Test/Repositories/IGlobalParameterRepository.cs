using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface IGlobalParameterRepository
{
    Task<GlobalParameterModel?> GetAsync(Guid id);
    Task<GlobalParameterModel?> GetAsync(string type, string name);
    Task CreateAsync(params GlobalParameterModel[] globalParameters);
}