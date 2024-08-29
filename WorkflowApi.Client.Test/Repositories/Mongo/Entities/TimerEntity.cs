using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

public class TimerEntity
{
    public TimerEntity() { }

    public TimerEntity(TimerModel model)
    {
        Id = model.Id;
        ProcessId = model.ProcessId;
        RootProcessId = model.RootProcessId;
        Name = model.Name;
        NextExecutionDateTime = model.NextExecutionDateTime.LocalDateTime;
        Ignore = model.Ignore;
    }

    public Guid Id { get; set; }
    public Guid ProcessId { get; set; }
    public Guid RootProcessId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime NextExecutionDateTime { get; set; }
    public bool Ignore { get; set; }

    public TimerModel ToModel()
    {
        return new TimerModel
        {
            Id = Id,
            ProcessId = ProcessId,
            RootProcessId = RootProcessId,
            Name = Name,
            NextExecutionDateTime = NextExecutionDateTime,
            Ignore = Ignore
        };
    }
}
