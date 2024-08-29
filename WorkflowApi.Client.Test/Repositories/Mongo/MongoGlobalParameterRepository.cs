using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoGlobalParameterRepository : IGlobalParameterRepository
{
    public MongoGlobalParameterRepository(IMongoDatabase database)
    {
        Database = database;
        GlobalParameters = Database.GetCollection<GlobalParameterEntity>("WorkflowGlobalParameter");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<GlobalParameterEntity> GlobalParameters { get; }
    
    public async Task<GlobalParameterModel?> GetAsync(Guid id)
    {
        var entity = await GlobalParameters.Find(e => e.Id == id).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task<GlobalParameterModel?> GetAsync(string type, string name)
    {
        var entity = await GlobalParameters.Find(e => e.Type == type && e.Name == name).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([type, name], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params GlobalParameterModel[] globalParameters)
    {
        var entities = globalParameters.Select(a => new GlobalParameterEntity(a)).ToArray();
        await GlobalParameters.InsertManyAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}