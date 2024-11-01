using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlParameterRepository : IParameterRepository
{
    public async Task<ParameterModel?> GetAsync(Guid processId, string name)
    {
        var db = new TestDatabase();
        var entity = await db.Parameters.FirstAsync(p => p.ProcessId == processId && p.ParameterName == name);

        TestLogger.LogEntityGot([processId, name], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params ParameterModel[] parameterModels)
    {
        var entities = parameterModels.Select(p => new ParameterEntity(p)).ToArray();
        var db = new TestDatabase();
        await db.Parameters.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}