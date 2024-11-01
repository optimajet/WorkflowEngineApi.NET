using OptimaJet.Workflow.Api.Models;
using ApprovalModel = WorkflowApi.Client.Model.ApprovalModel;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

public class ApprovalEntity
{
    public ApprovalEntity() { }

    public ApprovalEntity(ApprovalModel model)
    {
        Id = model.Id;
        ProcessId = model.ProcessId;
        IdentityId = model.IdentityId;
        AllowedTo = Converter.ToCommaSeparatedString(model.AllowedTo);
        TransitionTime = model.TransitionTime?.LocalDateTime;
        Sort = model.Sort;
        InitialState = model.InitialState;
        DestinationState = model.DestinationState;
        TriggerName = model.TriggerName;
        Commentary = model.Commentary;
    }

    public Guid Id { get; set; }
    public Guid ProcessId { get; set; }
    public string? IdentityId { get; set; }
    public string? AllowedTo { get; set; }
    public DateTime? TransitionTime { get; set; }
    public long? Sort { get; set; }
    public string InitialState { get; set; } = null!;
    public string DestinationState { get; set; } = null!;
    public string? TriggerName { get; set; }
    public string? Commentary { get; set; }

    public ApprovalModel ToModel()
    {
        return new ApprovalModel(
            Id,
            ProcessId,
            IdentityId,
            Converter.FromCommaSeparatedString(AllowedTo),
            TransitionTime,
            Sort ?? 0,
            InitialState,
            DestinationState,
            TriggerName,
            Commentary
        );
    }
}
