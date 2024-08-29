using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Statuses;

[TestClass]
public class GetTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<StatusModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataStatusesGetAsync(model.Id);

            TestLogger.LogApiCalled(new { model.Id }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            StatusHelper.AssertModels(models[i], results[i]);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = await StatusHelper.ExclusivePermissionsApi(service, "get");

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<StatusModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataStatusesGetAsync(model.Id);

            TestLogger.LogApiCalled(new { model.Id }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            StatusHelper.AssertModels(models[i], results[i]);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await StatusHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataStatusesGetAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Statuses;

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataStatusesGetAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(404, exception.ErrorCode);
    }
}