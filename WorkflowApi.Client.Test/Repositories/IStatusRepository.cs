using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface IStatusRepository
{
    Task CreateAsync(params StatusModel[] statuses);
}