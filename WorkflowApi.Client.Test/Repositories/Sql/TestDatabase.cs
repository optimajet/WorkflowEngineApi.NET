using OptimaJet.DataEngine;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class TestDatabase : Database
{
    public OptimaJet.DataEngine.ICollection<ApprovalEntity> Approvals => Provider.GetCollection<ApprovalEntity>();
    public OptimaJet.DataEngine.ICollection<GlobalParameterEntity> GlobalParameters => Provider.GetCollection<GlobalParameterEntity>();
    public OptimaJet.DataEngine.ICollection<InboxEntryEntity> InboxEntries => Provider.GetCollection<InboxEntryEntity>();
    public OptimaJet.DataEngine.ICollection<ParameterEntity> Parameters => Provider.GetCollection<ParameterEntity>();
    public OptimaJet.DataEngine.ICollection<ProcessEntity> Processes => Provider.GetCollection<ProcessEntity>();
    public OptimaJet.DataEngine.ICollection<RuntimeEntity> Runtimes => Provider.GetCollection<RuntimeEntity>();
    public OptimaJet.DataEngine.ICollection<SchemeEntity> Schemes => Provider.GetCollection<SchemeEntity>();
    public OptimaJet.DataEngine.ICollection<StatusEntity> Statuses => Provider.GetCollection<StatusEntity>();
    public OptimaJet.DataEngine.ICollection<TimerEntity> Timers => Provider.GetCollection<TimerEntity>();
    public OptimaJet.DataEngine.ICollection<TransitionEntity> Transitions => Provider.GetCollection<TransitionEntity>();
}
