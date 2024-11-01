using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoRuntimeRepository : IRuntimeRepository
{
    public MongoRuntimeRepository(IMongoDatabase database)
    {
        Database = database;
        Runtimes = Database.GetCollection<RuntimeEntity>("WorkflowRuntime");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<RuntimeEntity> Runtimes { get; }

    public async Task CreateAsync(params RuntimeModel[] runtimes)
    {
        var entities = runtimes.Select(a => new RuntimeEntity(a)).ToArray();
        await Runtimes.InsertManyAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.RuntimeId);
    }
}