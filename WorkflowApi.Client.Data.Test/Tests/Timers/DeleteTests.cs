using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Timers;

[TestClass]
public class DeleteTests
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

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesTimersDeleteAsync(processId, model.Name);
            TestLogger.LogApiCalled(new { processId, model.Name }, result);
            Assert.AreEqual(1, result?.DeletedCount);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Id);
            Assert.IsNull(result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = await TimerHelper.ExclusivePermissionsApi(service, "delete");

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesTimersDeleteAsync(processId, model.Name);
            TestLogger.LogApiCalled(new { processId, model.Name }, result);
            Assert.AreEqual(1, result?.DeletedCount);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Id);
            Assert.IsNull(result);
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
            async () => await api.WorkflowApiDataProcessesTimersDeleteAsync(Guid.NewGuid(), Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Timers;
        var processId = Guid.NewGuid();
        var name = Guid.NewGuid().ToString();

        // Act

        var result = await api.WorkflowApiDataProcessesTimersDeleteAsync(processId, name);

        // Assert

        Assert.AreEqual(0, result?.DeletedCount);
    }
}