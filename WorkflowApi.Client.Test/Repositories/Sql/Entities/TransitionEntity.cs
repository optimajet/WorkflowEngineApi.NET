using OptimaJet.DataEngine.Attributes;
using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowProcessTransitionHistory")]
public class TransitionEntity
{
    public TransitionEntity() { }

    public TransitionEntity(TransitionModel model)
    {
        Id = model.Id;
        ProcessId = model.ProcessId;
        ExecutorIdentityId = model.ExecutorIdentityId;
        ActorIdentityId = model.ActorIdentityId;
        ExecutorName = model.ExecutorName;
        ActorName = model.ActorName;
        FromActivityName = model.FromActivityName;
        ToActivityName = model.ToActivityName;
        ToStateName = model.ToStateName;
        TransitionTime = model.TransitionTime.LocalDateTime;
        TransitionClassifier = model.TransitionClassifier;
        IsFinalised = model.IsFinalised;
        FromStateName = model.FromStateName;
        TriggerName = model.TriggerName;
        StartTransitionTime = model.StartTransitionTime?.LocalDateTime;
        TransitionDuration = model.TransitionDuration;
    }

    [PrimaryKey]
    public Guid Id { get; set; }
    
    public Guid ProcessId { get; set; }
    
    [DataLength(256)]
    public string? ExecutorIdentityId { get; set; }
    
    [DataLength(256)]
    public string? ActorIdentityId { get; set; }
    
    [DataLength(256)]
    public string? ExecutorName { get; set; }
    
    [DataLength(256)]
    public string? ActorName { get; set; }
    
    [DataRequired]
    public string FromActivityName { get; set; } = null!;
    
    [DataRequired]
    public string ToActivityName { get; set; } = null!;
    
    public string? ToStateName { get; set; }
    
    public DateTime TransitionTime { get; set; }
    
    [DataRequired]
    public string TransitionClassifier { get; set; } = null!;

    public bool IsFinalised { get; set; }
    
    public string? FromStateName { get; set; }
    
    public string? TriggerName { get; set; }
    
    public DateTime? StartTransitionTime { get; set; }
    
    public long? TransitionDuration { get; set; }

    public TransitionModel ToModel()
    {
        return new TransitionModel
        {
            Id = Id,
            ProcessId = ProcessId,
            ExecutorIdentityId = ExecutorIdentityId,
            ActorIdentityId = ActorIdentityId,
            ExecutorName = ExecutorName,
            ActorName = ActorName,
            FromActivityName = FromActivityName,
            ToActivityName = ToActivityName,
            ToStateName = ToStateName,
            TransitionTime = TransitionTime,
            TransitionClassifier = TransitionClassifier,
            IsFinalised = IsFinalised,
            FromStateName = FromStateName,
            TriggerName = TriggerName,
            StartTransitionTime = StartTransitionTime,
            TransitionDuration = TransitionDuration
        };
    }
}
