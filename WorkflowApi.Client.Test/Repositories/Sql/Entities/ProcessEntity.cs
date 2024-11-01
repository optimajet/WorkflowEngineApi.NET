using OptimaJet.DataEngine.Attributes;
using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowProcessInstance")]
public class ProcessEntity
{
    public ProcessEntity() { }

    public ProcessEntity(ProcessModel model)
    {
        Id = model.Id;
        StateName = model.StateName;
        ActivityName = model.ActivityName;
        SchemeId = model.SchemeId;
        PreviousState = model.PreviousState;
        PreviousStateForDirect = model.PreviousStateForDirect;
        PreviousStateForReverse = model.PreviousStateForReverse;
        PreviousActivity = model.PreviousActivity;
        PreviousActivityForDirect = model.PreviousActivityForDirect;
        PreviousActivityForReverse = model.PreviousActivityForReverse;
        ParentProcessId = model.ParentProcessId;
        RootProcessId = model.RootProcessId;
        IsDeterminingParametersChanged = model.IsDeterminingParametersChanged;
        TenantId = model.TenantId;
        StartingTransition = model.StartingTransition;
        SubprocessName = model.SubprocessName;
        CreationDate = model.CreationDate.LocalDateTime;
        LastTransitionDate = model.LastTransitionDate?.LocalDateTime;
        CalendarName = model.CalendarName;
    }

    [PrimaryKey]
    public Guid Id { get; set; }
    
    public string? StateName { get; set; }
    
    [DataRequired]
    public string ActivityName { get; set; } = null!;
    
    public Guid? SchemeId { get; set; }
    
    public string? PreviousState { get; set; }
    
    public string? PreviousStateForDirect { get; set; }
    
    public string? PreviousStateForReverse { get; set; }
    
    public string? PreviousActivity { get; set; }
    
    public string? PreviousActivityForDirect { get; set; }
    
    public string? PreviousActivityForReverse { get; set; }
    
    public Guid? ParentProcessId { get; set; }
    
    public Guid RootProcessId { get; set; }
    
    public bool IsDeterminingParametersChanged { get; set; }
    
    [DataLength(1024)]
    public string? TenantId { get; set; }
    
    public string? StartingTransition { get; set; }
    
    public string? SubprocessName { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public DateTime? LastTransitionDate { get; set; }

    public string? CalendarName { get; set; }

    public ProcessModel ToModel()
    {
        return new ProcessModel(
            Id,
            StateName,
            ActivityName,
            SchemeId,
            PreviousState,
            PreviousStateForDirect,
            PreviousStateForReverse,
            PreviousActivity,
            PreviousActivityForDirect,
            PreviousActivityForReverse,
            ParentProcessId,
            RootProcessId,
            IsDeterminingParametersChanged,
            TenantId,
            StartingTransition,
            SubprocessName,
            CreationDate,
            LastTransitionDate,
            CalendarName
        );
    }
}
