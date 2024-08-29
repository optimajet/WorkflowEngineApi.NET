using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoTimerRepository : ITimerRepository
{
    public MongoTimerRepository(IMongoDatabase database)
    {
        Database = database;
        Timer = Database.GetCollection<TimerEntity>("WorkflowProcessTimer");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<TimerEntity> Timer { get; }
    
    public async Task<TimerModel?> GetAsync(Guid id)
    {
        var entity = await Timer.Find(e => e.Id == id).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task<TimerModel?> GetAsync(Guid processId, string name)
    {
        var entity = await Timer.Find(e => e.ProcessId == processId && e.Name == name).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([processId, name], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params TimerModel[] timers)
    {
        var entities = timers.Select(a => new TimerEntity(a)).ToArray();
        await Timer.InsertManyAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}