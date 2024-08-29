using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

public class StatusEntity
{
    public StatusEntity() { }

    public StatusEntity(StatusModel model)
    {
        Status = (byte) model.StatusCode;
        Lock = model.VarLock;
        RuntimeId = model.RuntimeId;
        SetTime = model.SetTime.LocalDateTime;
    }

    public byte Status { get; set; }
    public Guid Lock { get; set; }
    public string RuntimeId { get; set; } = null!;
    public DateTime SetTime { get; set; }

    public StatusModel ToModel(Guid processId)
    {
        return new StatusModel
        {
            Id = processId,
            StatusCode = Status,
            VarLock = Lock,
            RuntimeId = RuntimeId,
            SetTime = SetTime
        };
    }
}
