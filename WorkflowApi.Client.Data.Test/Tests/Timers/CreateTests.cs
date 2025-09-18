using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Timers;

[TestClass]
public class CreateTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        // Act

        foreach (var model in models)
        {
            var request = TimerHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataProcessesTimersCreateAsync(processId, model.Name, request);
            TestLogger.LogApiCalled(new {processId, model.Name, request}, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            TimerHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = await TimerHelper.ExclusivePermissionsApi(service, "create");

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        // Act

        foreach (var model in models)
        {
            var request = TimerHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataProcessesTimersCreateAsync(processId, model.Name, request);
            TestLogger.LogApiCalled(new {processId, model.Name, request}, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            TimerHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await TimerHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTimersCreateAsync(Guid.NewGuid(), Guid.NewGuid().ToString(), TimerHelper.CreateRequest(TimerHelper.Generate(Guid.NewGuid())))
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
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var model = TimerHelper.Generate(processId);

        await repository.CreateAsync(model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTimersCreateAsync(processId, model.Name, TimerHelper.CreateRequest(model))
        );

        // Assert

        Assert.AreEqual(500, exception.ErrorCode);
    }
}