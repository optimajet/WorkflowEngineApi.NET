using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Runtimes;

[TestClass]
public class GetTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<RuntimeModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataRuntimesGetAsync(model.Id);

            TestLogger.LogApiCalled(new { model.Id }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            RuntimeHelper.AssertModels(models[i], results[i]);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = await RuntimeHelper.ExclusivePermissionsApi(service, "get");

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<RuntimeModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataRuntimesGetAsync(model.Id);

            TestLogger.LogApiCalled(new { model.Id }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            RuntimeHelper.AssertModels(models[i], results[i]);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await RuntimeHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataRuntimesGetAsync(Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Runtimes;

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataRuntimesGetAsync(Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(404, exception.ErrorCode);
    }
}