using OptimaJet.DataEngine.Attributes;
using OptimaJet.Workflow.Api.Models;
using InboxEntryModel = WorkflowApi.Client.Model.InboxEntryModel;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowInbox")]
public class InboxEntryEntity
{
    public InboxEntryEntity() { }

    public InboxEntryEntity(InboxEntryModel model)
    {
        Id = model.Id;
        ProcessId = model.ProcessId;
        IdentityId = model.IdentityId;
        AddingDate = model.AddingDate.LocalDateTime;
        AvailableCommands = Converter.ToCommaSeparatedString(model.AvailableCommands);
    }

    [PrimaryKey]
    public Guid Id { get; set; }
    
    public Guid ProcessId { get; set; }
    
    [DataRequired]
    [DataLength(256)]
    public string IdentityId { get; set; } = null!;
    
    public DateTime AddingDate { get; set; }
    
    [DataRequired]
    public string AvailableCommands { get; set; } = null!;

    public InboxEntryModel ToModel()
    {
        return new InboxEntryModel(
            Id,
            ProcessId,
            IdentityId,
            AddingDate,
            Converter.FromCommaSeparatedString(AvailableCommands)
        );
    }
}
