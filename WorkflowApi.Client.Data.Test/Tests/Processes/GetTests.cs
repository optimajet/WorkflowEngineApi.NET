using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Processes;

[TestClass]
public class GetTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<ProcessModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesGetAsync(model.Id);

            TestLogger.LogApiCalled(new { model.Id }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            ProcessHelper.AssertModels(models[i], results[i]);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = await ProcessHelper.ExclusivePermissionsApi(service, "get");

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<ProcessModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesGetAsync(model.Id);

            TestLogger.LogApiCalled(new { model.Id }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            ProcessHelper.AssertModels(models[i], results[i]);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ProcessHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesGetAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Processes;

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesGetAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(404, exception.ErrorCode);
    }
}