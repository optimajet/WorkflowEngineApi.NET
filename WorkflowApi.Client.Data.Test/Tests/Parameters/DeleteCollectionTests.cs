using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Core;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Parameters;

[TestClass]
public class DeleteCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var extraModel = ParameterHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesParametersDeleteCollectionAsync(processId);

        TestLogger.LogApiCalled(new { processId }, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount == models.Length);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.ProcessId, model.Name);
            Assert.IsNull(result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, expected.Id),
            // new (FilterType.Equal, null, ParameterField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, expected.Name),
            new (FilterType.Equal, null, ParameterField.Value, expected.Value),
        ];

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesParametersDeleteCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new { processId, filters }, deleteResult);

        // Assert

        Assert.AreEqual(1, deleteResult.DeletedCount);
        Assert.IsNull(await repository.GetAsync(processId, expected.Name));

        foreach (var model in models.Where(m => m.Id != expected.Id))
        {
            var notDeletedModel = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, notDeletedModel, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, models[0].Id),
            // new (FilterType.Equal, null, ParameterField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, models[0].Name),
            new (FilterType.Equal, null, ParameterField.Value, models[1].Value),
        ];

        List<ParameterFieldFilter> orFilter = [new ParameterFieldFilter(FilterType.Or, filters)];

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesParametersDeleteCollectionAsync(processId, filters: orFilter);

        TestLogger.LogApiCalled(new { processId, filters = orFilter }, deleteResult);

        // Assert

        Assert.AreEqual(filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            Assert.IsNull(await repository.GetAsync(processId, models[i].Name));
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            var notDeletedModel = await repository.GetAsync(processId, models[i].Name);
            ParameterHelper.AssertModels(models[i], notDeletedModel, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, models[0].Id),
            // new (FilterType.Equal, null, ParameterField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, models[0].Name),
            new (FilterType.Equal, null, ParameterField.Value, models[1].Value),
        ];

        var notFilters = filters.Select(f => new ParameterFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesParametersDeleteCollectionAsync(processId, filters: notFilters);

        TestLogger.LogApiCalled(new { processId, filters = notFilters }, deleteResult);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            var notDeletedModel = await repository.GetAsync(processId, models[i].Name);
            ParameterHelper.AssertModels(models[i], notDeletedModel, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            Assert.IsNull(await repository.GetAsync(processId, models[i].Name));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "delete-collection");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var extraModel = ParameterHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesParametersDeleteCollectionAsync(processId);

        TestLogger.LogApiCalled(new { processId }, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount == models.Length);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.ProcessId, model.Name);
            Assert.IsNull(result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersDeleteCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}