using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface ITimerRepository
{
    Task<TimerModel?> GetAsync(Guid id);
    Task<TimerModel?> GetAsync(Guid processId, string name);
    Task CreateAsync(params TimerModel[] timers);
}