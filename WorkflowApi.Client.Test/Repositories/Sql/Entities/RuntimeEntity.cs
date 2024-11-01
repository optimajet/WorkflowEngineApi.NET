using OptimaJet.DataEngine.Attributes;
using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowRuntime")]
public class RuntimeEntity
{
    public RuntimeEntity() { }

    public RuntimeEntity(RuntimeModel model)
    {
        RuntimeId = model.Id;
        Lock = model.VarLock;
        Status = Enum.Parse<OptimaJet.Workflow.Core.Model.RuntimeStatus>(model.Status.GetValueOrDefault().ToString());
        RestorerId = model.RestorerId;
        NextTimerTime = model.NextTimerTime?.LocalDateTime;
        NextServiceTimerTime = model.NextServiceTimerTime?.LocalDateTime;
        LastAliveSignal = model.LastAliveSignal?.LocalDateTime;
    }

    [PrimaryKey]
    [DataRequired]
    [DataLength(450)]
    public string RuntimeId { get; set; } = null!;

    public Guid Lock { get; set; }
    
    public OptimaJet.Workflow.Core.Model.RuntimeStatus Status { get; set; }
    
    [DataLength(450)]
    public string? RestorerId { get; set; }
    
    public DateTime? NextTimerTime { get; set; }
    
    public DateTime? NextServiceTimerTime { get; set; }
    
    public DateTime? LastAliveSignal { get; set; }

    public RuntimeModel ToModel()
    {
        return new RuntimeModel
        {
            Id = RuntimeId,
            VarLock = Lock,
            Status = Enum.Parse<RuntimeStatus>(Status.ToString()),
            RestorerId = RestorerId,
            NextTimerTime = NextTimerTime,
            NextServiceTimerTime = NextServiceTimerTime,
            LastAliveSignal = LastAliveSignal
        };
    }
}
