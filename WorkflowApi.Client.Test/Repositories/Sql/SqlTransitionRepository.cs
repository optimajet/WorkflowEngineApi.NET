using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlTransitionRepository : ITransitionRepository
{
    public async Task<TransitionModel?> GetAsync(Guid id)
    {
        var db = new TestDatabase();
        var entity = await db.Transitions.GetByKeyAsync(id);

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params TransitionModel[] transitions)
    {
        var entities = transitions.Select(t => new TransitionEntity(t)).ToArray();
        var db = new TestDatabase();
        await db.Transitions.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}