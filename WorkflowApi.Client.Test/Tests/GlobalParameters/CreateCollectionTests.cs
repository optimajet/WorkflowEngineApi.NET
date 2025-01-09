using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.GlobalParameters;

[TestClass]
public class CreateCollectionTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);
        var requests = GlobalParameterHelper.CreateRequests(models);

        // Act

        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        TestLogger.LogApiCalled(new {requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = await GlobalParameterHelper.ExclusivePermissionsApi(service, "create-collection");

        var models = GlobalParameterHelper.Generate(3);
        var requests = GlobalParameterHelper.CreateRequests(models);

        // Act

        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        TestLogger.LogApiCalled(new {requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await GlobalParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersCreateCollectionAsync([])
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var model = GlobalParameterHelper.Generate();

        await repository.CreateAsync(model);

        var requests = GlobalParameterHelper.CreateRequests(model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests)
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}