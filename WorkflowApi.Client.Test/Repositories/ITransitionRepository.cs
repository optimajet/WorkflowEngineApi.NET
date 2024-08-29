using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface ITransitionRepository
{
    Task<TransitionModel?> GetAsync(Guid id);
    Task CreateAsync(params TransitionModel[] transitions);
}