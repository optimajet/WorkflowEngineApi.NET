using OptimaJet.Workflow.Api.Models;
using InboxEntryModel = WorkflowApi.Client.Model.InboxEntryModel;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

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

    public Guid Id { get; set; }
    public Guid ProcessId { get; set; }
    public string IdentityId { get; set; } = null!;
    public DateTime AddingDate { get; set; }
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
