using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoStatusRepository : IStatusRepository
{
    public MongoStatusRepository(IMongoDatabase database)
    {
        Database = database;
        Processes = Database.GetCollection<ProcessEntity>("WorkflowProcessInstance");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<ProcessEntity> Processes { get; }

    public async Task CreateAsync(params StatusModel[] statuses)
    {
        var processEntities = statuses.Select(s => new ProcessEntity { Id = s.Id, Status = new StatusEntity(s) }).ToArray();
        await Processes.InsertManyAsync(processEntities);

        TestLogger.LogEntitiesCreated(processEntities.Select(entity => entity.Status), status => processEntities.First(p => p.Status == status).Id.ToString());
    }
}