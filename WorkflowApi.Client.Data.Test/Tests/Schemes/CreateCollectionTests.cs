using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Schemes;

[TestClass]
public class CreateCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);
        var requests = SchemeHelper.CreateRequests(models);

        // Act

        var createResult = await api.WorkflowApiDataSchemesCreateCollectionAsync(requests);

        TestLogger.LogApiCalled(new {requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Code);
            SchemeHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = await SchemeHelper.ExclusivePermissionsApi(service, "create-collection");

        var models = SchemeHelper.Generate(3);
        var requests = SchemeHelper.CreateRequests(models);

        // Act

        var createResult = await api.WorkflowApiDataSchemesCreateCollectionAsync(requests);

        TestLogger.LogApiCalled(new {requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Code);
            SchemeHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await SchemeHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesCreateCollectionAsync([])
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var model = SchemeHelper.Generate();

        await repository.CreateAsync(model);

        // Act

        var requests = SchemeHelper.CreateRequests(model);

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesCreateCollectionAsync(requests)
        );

        // Assert

        Assert.AreEqual(500, exception.ErrorCode);
    }
}