using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoTransitionRepository : ITransitionRepository
{
    public MongoTransitionRepository(IMongoDatabase database)
    {
        Database = database;
        Transitions = Database.GetCollection<TransitionEntity>("WorkflowProcessTransitionHistory");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<TransitionEntity> Transitions { get; }
    
    public async Task<TransitionModel?> GetAsync(Guid id)
    {
        var entity = await Transitions.Find(e => e.Id == id).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params TransitionModel[] transitions)
    {
        var entities = transitions.Select(a => new TransitionEntity(a)).ToArray();
        await Transitions.InsertManyAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}