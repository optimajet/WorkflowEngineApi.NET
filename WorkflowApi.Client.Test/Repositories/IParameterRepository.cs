using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface IParameterRepository
{
    Task<ParameterModel?> GetAsync(Guid processId, string name);
    Task CreateAsync(params ParameterModel[] parameterModels);
}