using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlGlobalParameterRepository : IGlobalParameterRepository
{
    public async Task<GlobalParameterModel?> GetAsync(Guid id)
    {
        var db = new TestDatabase();
        var entity = await db.GlobalParameters.GetByKeyAsync(id);

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task<GlobalParameterModel?> GetAsync(string type, string name)
    {
        var db = new TestDatabase();
        var entity = await db.GlobalParameters.FirstAsync(gp => gp.Type == type && gp.Name == name);

        TestLogger.LogEntityGot([type, name], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params GlobalParameterModel[] globalParameters)
    {
        var entities = globalParameters.Select(gp => new GlobalParameterEntity(gp)).ToArray();
        var db = new TestDatabase();
        await db.GlobalParameters.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}