using OptimaJet.DataEngine.Attributes;
using WorkflowApi.Client.Test.Models;
using ApprovalModel = WorkflowApi.Client.Model.ApprovalModel;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowApprovalHistory")]
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

    [PrimaryKey]
    public Guid Id { get; set; }
    
    [DataRequired]
    public Guid ProcessId { get; set; }
    
    [DataLength(256)]
    public string? IdentityId { get; set; }
    
    public string? AllowedTo { get; set; }
    
    public DateTime? TransitionTime { get; set; }
    
    public long? Sort { get; set; }
    
    [DataRequired]
    [DataLength(1024)]
    public string InitialState { get; set; } = null!;
    
    [DataRequired]
    [DataLength(1024)]
    public string DestinationState { get; set; } = null!;
    
    [DataLength(1024)]
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
