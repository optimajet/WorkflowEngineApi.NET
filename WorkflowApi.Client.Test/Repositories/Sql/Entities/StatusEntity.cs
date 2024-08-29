using OptimaJet.DataEngine.Attributes;
using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowProcessInstanceStatus")]
public class StatusEntity
{
    public StatusEntity() { }

    public StatusEntity(StatusModel model)
    {
        Id = model.Id;
        Status = (byte) model.StatusCode;
        Lock = model.VarLock;
        RuntimeId = model.RuntimeId;
        SetTime = model.SetTime.LocalDateTime;
    }

    [PrimaryKey]
    public Guid Id { get; set; }
    
    public byte Status { get; set; }
    
    public Guid Lock { get; set; }

    [DataRequired]
    [DataLength(450)]
    public string RuntimeId { get; set; } = null!;

    public DateTime SetTime { get; set; }

    public StatusModel ToModel()
    {
        return new StatusModel
        {
            Id = Id,
            StatusCode = Status,
            VarLock = Lock,
            RuntimeId = RuntimeId,
            SetTime = SetTime
        };
    }
}
