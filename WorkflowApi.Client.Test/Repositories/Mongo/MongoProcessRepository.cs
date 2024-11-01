using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoProcessRepository : IProcessRepository
{
    public MongoProcessRepository(IMongoDatabase database)
    {
        Database = database;
        Processes = Database.GetCollection<ProcessEntity>("WorkflowProcessInstance");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<ProcessEntity> Processes { get; }
    
    public async Task<ProcessModel?> GetAsync(Guid id)
    {
        var entity = await Processes.Find(e => e.Id == id).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params ProcessModel[] processes)
    {
        var entities = processes.Select(a => new ProcessEntity(a)).ToArray();
        await Processes.InsertManyAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}