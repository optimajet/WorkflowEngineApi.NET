using MongoDB.Driver;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Mongo.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Mongo;

public class MongoInboxEntryRepository : IInboxEntryRepository
{
    public MongoInboxEntryRepository(IMongoDatabase database)
    {
        Database = database;
        InboxEntries = Database.GetCollection<InboxEntryEntity>("WorkflowInbox");
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<InboxEntryEntity> InboxEntries { get; }

    public async Task<InboxEntryModel?> GetAsync(Guid id)
    {
        var entity = await InboxEntries.Find(e => e.Id == id).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task<InboxEntryModel?> GetAsync(Guid processId, string identityId)
    {
        var entity = await InboxEntries.Find(e => e.ProcessId == processId && e.IdentityId == identityId).FirstOrDefaultAsync();

        TestLogger.LogEntityGot([processId, identityId], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params InboxEntryModel[] inboxEntries)
    {
        var entities = inboxEntries.Select(a => new InboxEntryEntity(a)).ToArray();
        await InboxEntries.InsertManyAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}