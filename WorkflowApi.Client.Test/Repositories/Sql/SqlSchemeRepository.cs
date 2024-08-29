using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlSchemeRepository : ISchemeRepository
{
    public async Task<SchemeModel?> GetAsync(string code)
    {
        var db = new TestDatabase();
        var entity = await db.Schemes.FirstAsync(s => s.Code == code);

        TestLogger.LogEntityGot([code], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params SchemeModel[] schemes)
    {
        var entities = schemes.Select(s => new SchemeEntity(s)).ToArray();
        var db = new TestDatabase();
        await db.Schemes.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Code);
    }
}