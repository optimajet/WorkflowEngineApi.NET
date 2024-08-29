using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlProcessRepository : IProcessRepository
{
    public async Task<ProcessModel?> GetAsync(Guid id)
    {
        var db = new TestDatabase();
        var entity = await db.Processes.GetByKeyAsync(id);

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params ProcessModel[] processes)
    {
        var entities = processes.Select(p => new ProcessEntity(p)).ToArray();
        var db = new TestDatabase();
        await db.Processes.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}