using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlStatusRepository : IStatusRepository
{
    public async Task CreateAsync(params StatusModel[] statuses)
    {
        var entities = statuses.Select(s => new StatusEntity(s)).ToArray();
        var db = new TestDatabase();
        await db.Statuses.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}