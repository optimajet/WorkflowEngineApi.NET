using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Test.Repositories;

public interface ISchemeRepository
{
    Task<SchemeModel?> GetAsync(string code);
    Task CreateAsync(params SchemeModel[] schemes);
}