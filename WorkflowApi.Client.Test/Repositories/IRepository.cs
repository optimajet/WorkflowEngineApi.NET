namespace WorkflowApi.Client.Test.Repositories;

public interface IRepository
{
    public IAsyncDisposable Use();
    public IApprovalRepository Approvals { get; }
    public IGlobalParameterRepository GlobalParameters { get; }
    public IInboxEntryRepository InboxEntries { get; }
    public IParameterRepository Parameters { get; }
    public IProcessRepository Processes { get; }
    public IRuntimeRepository Runtimes { get; }
    public ISchemeRepository Schemes { get; }
    public IStatusRepository Statuses { get; }
    public ITimerRepository Timers { get; }
    public ITransitionRepository Transitions { get; }
}