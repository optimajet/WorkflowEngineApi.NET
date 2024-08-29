using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoParameterRepository : IParameterRepository
{
    public MongoParameterRepository(IMongoDatabase database)
    {
        Database = database;
        Processes = Database.GetCollection<ProcessEntity>("WorkflowProcessInstance");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<ProcessEntity> Processes { get; }

    public async Task<ParameterModel?> GetAsync(Guid processId, string name)
    {
        var processEntity = await Processes.Find(e => e.Id == processId).FirstOrDefaultAsync();
        var entity = processEntity?.Persistence.FirstOrDefault(p => p.ParameterName == name);

        TestLogger.LogEntityGot([processId], entity);

        return entity?.ToModel(processId);
    }

    public async Task CreateAsync(params ParameterModel[] parameterModels)
    {
        var processEntities = parameterModels
            .GroupBy(p => p.ProcessId)
            .Select(g => new ProcessEntity { Id = g.Key, Persistence = g.Select(p => new ParameterEntity(p)).ToList() })
            .ToArray();

        await Processes.InsertManyAsync(processEntities);

        TestLogger.LogEntitiesCreated(processEntities.SelectMany(p => p.Persistence), e => e.ParameterName);
    }
}