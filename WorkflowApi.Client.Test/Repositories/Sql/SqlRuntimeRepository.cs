using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlRuntimeRepository : IRuntimeRepository
{
    public async Task CreateAsync(params RuntimeModel[] runtimes)
    {
        var entities = runtimes.Select(r => new RuntimeEntity(r)).ToArray();
        var db = new TestDatabase();
        await db.Runtimes.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.RuntimeId);
    }
}