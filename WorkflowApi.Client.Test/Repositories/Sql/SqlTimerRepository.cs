using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlTimerRepository : ITimerRepository
{
    public async Task<TimerModel?> GetAsync(Guid id)
    {
        var db = new TestDatabase();
        var entity = await db.Timers.GetByKeyAsync(id);

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task<TimerModel?> GetAsync(Guid processId, string name)
    {
        var db = new TestDatabase();
        var entity = await db.Timers.FirstAsync(t => t.ProcessId == processId && t.Name == name);

        TestLogger.LogEntityGot([processId, name], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params TimerModel[] timers)
    {
        var entities = timers.Select(t => new TimerEntity(t)).ToArray();
        var db = new TestDatabase();
        await db.Timers.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}