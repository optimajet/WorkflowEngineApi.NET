using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Timers;

[TestClass]
public class UpdateTests
{
    [ClientTest]
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

        var updateRequests = TimerHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataProcessesTimersUpdateAsync(processId, model.Name, updateRequest);

            TestLogger.LogApiCalled(new { processId, model.Name, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(processId, updateRequest.Name);
            TimerHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = await TimerHelper.ExclusivePermissionsApi(service, "update");

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = TimerHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataProcessesTimersUpdateAsync(processId, model.Name, updateRequest);

            TestLogger.LogApiCalled(new { processId, model.Name, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(processId, updateRequest.Name);
            TimerHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await TimerHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTimersUpdateAsync(Guid.NewGuid(), Guid.NewGuid().ToString(), new TimerUpdateRequest())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Timers;
        var processId = Guid.NewGuid();
        var name = Guid.NewGuid().ToString();
        var timerUpdateRequest = new TimerUpdateRequest(Guid.NewGuid().ToString());

        // Act

        var result = await api.WorkflowApiDataProcessesTimersUpdateAsync(processId, name, timerUpdateRequest);

        TestLogger.LogApiCalled(new { processId, name, timerUpdateRequest }, result);

        // Assert

        Assert.AreEqual(0, result?.UpdatedCount);
    }

    [ClientTest]
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var exist = TimerHelper.Generate(processId);
        var model = TimerHelper.Generate(processId);
        var updateRequest = new TimerUpdateRequest(exist.Name);

        await repository.CreateAsync(exist, model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTimersUpdateAsync(processId, model.Name, updateRequest)
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}