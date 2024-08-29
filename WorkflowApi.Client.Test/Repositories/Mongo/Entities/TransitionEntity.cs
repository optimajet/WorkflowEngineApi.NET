using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

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

    public Guid Id { get; set; }
    public Guid ProcessId { get; set; }
    public string? ExecutorIdentityId { get; set; }
    public string? ActorIdentityId { get; set; }
    public string? ExecutorName { get; set; }
    public string? ActorName { get; set; }
    public string FromActivityName { get; set; } = null!;
    public string ToActivityName { get; set; } = null!;
    public string? ToStateName { get; set; }
    public DateTime TransitionTime { get; set; }
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
