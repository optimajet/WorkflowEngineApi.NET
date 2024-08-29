using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlInboxEntryRepository : IInboxEntryRepository
{
    public async Task<InboxEntryModel?> GetAsync(Guid id)
    {
        var db = new TestDatabase();
        var entity = await db.InboxEntries.GetByKeyAsync(id);

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task<InboxEntryModel?> GetAsync(Guid processId, string identityId)
    {
        var db = new TestDatabase();
        var entity = await db.InboxEntries.FirstAsync(i => i.ProcessId == processId && i.IdentityId == identityId);

        TestLogger.LogEntityGot([processId, identityId], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params InboxEntryModel[] inboxEntries)
    {
        var entities = inboxEntries.Select(i => new InboxEntryEntity(i)).ToArray();
        var db = new TestDatabase();
        await db.InboxEntries.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}