using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface IApprovalRepository
{
    Task<ApprovalModel?> GetAsync(Guid id);
    Task CreateAsync(params ApprovalModel[] approvalModels);
}