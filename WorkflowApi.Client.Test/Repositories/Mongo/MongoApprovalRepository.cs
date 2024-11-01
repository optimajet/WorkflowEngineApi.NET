using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoApprovalRepository : IApprovalRepository
{
    public MongoApprovalRepository(IMongoDatabase database)
    {
        Database = database;
        Approvals = Database.GetCollection<ApprovalEntity>("WorkflowApprovalHistory");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<ApprovalEntity> Approvals { get; }

    public async Task<ApprovalModel?> GetAsync(Guid id)
    {
        var entity = await Approvals.Find(e => e.Id == id).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params ApprovalModel[] approvalModels)
    {
        var entities = approvalModels.Select(a => new ApprovalEntity(a)).ToArray();
        await Approvals.InsertManyAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}