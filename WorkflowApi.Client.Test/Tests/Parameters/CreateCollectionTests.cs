using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Parameters;

[TestClass]
public class CreateCollectionTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);
        var requests = ParameterHelper.CreateRequests(models);

        if (service.Configuration.AppConfiguration.Provider == Provider.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act

        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);

        TestLogger.LogApiCalled(new {processId, requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "create-collection");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);
        var requests = ParameterHelper.CreateRequests(models);

        if (service.Configuration.AppConfiguration.Provider == Provider.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act

        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);

        TestLogger.LogApiCalled(new {processId, requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(Guid.NewGuid(), [])
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [Ignore] //No constraints
    [ClientTest]
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var model = ParameterHelper.Generate(processId);

        await repository.CreateAsync(model);

        // Act

        var requests = ParameterHelper.CreateRequests(model);

        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);

        TestLogger.LogApiCalled(new {processId, requests}, createResult);

        // Assert

        Assert.AreEqual(0, createResult?.CreatedCount);
    }
}