using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoSchemeRepository : ISchemeRepository
{
    public MongoSchemeRepository(IMongoDatabase database)
    {
        Database = database;
        Schemes = Database.GetCollection<SchemeEntity>("WorkflowScheme");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<SchemeEntity> Schemes { get; }
    
    public async Task<SchemeModel?> GetAsync(string code)
    {
        var entity = await Schemes.Find(e => e.Code == code).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([code], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params SchemeModel[] schemes)
    {
        var entities = schemes.Select(a => new SchemeEntity(a)).ToArray();
        await Schemes.InsertManyAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Code);
    }
}