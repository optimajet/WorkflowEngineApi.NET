using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlApprovalRepository : IApprovalRepository
{
    public async Task<ApprovalModel?> GetAsync(Guid id)
    {
        var db = new TestDatabase();

        var entity = await db.Approvals.GetByKeyAsync(id);

        TestLogger.LogEntityGot([id], entity);

        return entity?.ToModel();
    }

    public async Task CreateAsync(params ApprovalModel[] approvalModels)
    {
        var entities = approvalModels.Select(a => new ApprovalEntity(a)).ToArray();
        var db = new TestDatabase();
        await db.Approvals.InsertAsync(entities);

        TestLogger.LogEntitiesCreated(entities, e => e.Id.ToString());
    }
}