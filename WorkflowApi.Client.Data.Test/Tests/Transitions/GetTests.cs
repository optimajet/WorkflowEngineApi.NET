using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Transitions;

[TestClass]
public class GetTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        List<TransitionModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesTransitionsGetAsync(processId, model.Id);
            TestLogger.LogApiCalled(new { processId, model.Id }, result);
            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            TransitionHelper.AssertModels(models[i], results[i], true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = await TransitionHelper.ExclusivePermissionsApi(service, "get");

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        List<TransitionModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesTransitionsGetAsync(processId, model.Id);
            TestLogger.LogApiCalled(new { processId, model.Id }, result);
            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            TransitionHelper.AssertModels(models[i], results[i], true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await TransitionHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTransitionsGetAsync(Guid.NewGuid(), Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Transitions;

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTransitionsGetAsync(Guid.NewGuid(), Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(404, exception.ErrorCode);
    }
}