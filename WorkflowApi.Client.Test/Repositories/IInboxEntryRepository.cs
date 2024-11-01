using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface IInboxEntryRepository
{
    Task<InboxEntryModel?> GetAsync(Guid id);
    Task CreateAsync(params InboxEntryModel[] inboxEntries);
}