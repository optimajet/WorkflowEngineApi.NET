using MongoDB.Driver;
using WorkflowApi.Client.Test.Models;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoRepository : IRepository
{
    public MongoRepository(TestService testService)
    {
        var provider = testService.Configuration.AppConfiguration.Provider.ToString();
        var url = testService.Configuration.AppConfiguration.ConnectionStrings[provider];

        var client = new MongoClient(url);
        var database = client.GetDatabase(new MongoUrl(url).DatabaseName);

        Approvals = new MongoApprovalRepository(database);
        GlobalParameters = new MongoGlobalParameterRepository(database);
        InboxEntries = new MongoInboxEntryRepository(database);
        Parameters = new MongoParameterRepository(database);
        Processes = new MongoProcessRepository(database);
        Runtimes = new MongoRuntimeRepository(database);
        Schemes = new MongoSchemeRepository(database);
        Statuses = new MongoStatusRepository(database);
        Timers = new MongoTimerRepository(database);
        Transitions = new MongoTransitionRepository(database);
    }

    public IAsyncDisposable Use()
    {
        return new DisposableCollection();
    }

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