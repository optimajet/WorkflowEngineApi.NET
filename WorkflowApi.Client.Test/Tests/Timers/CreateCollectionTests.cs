﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Timers;

[TestClass]
public class CreateCollectionTests
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
        var requests = TimerHelper.CreateRequests(models);

        // Act

        var createResult = await api.WorkflowApiDataProcessesTimersCreateCollectionAsync(processId, requests);

        TestLogger.LogApiCalled(new { processId, models }, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            TimerHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = await TimerHelper.ExclusivePermissionsApi(service, "create-collection");

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);
        var requests = TimerHelper.CreateRequests(models);

        // Act

        var createResult = await api.WorkflowApiDataProcessesTimersCreateCollectionAsync(processId, requests);

        TestLogger.LogApiCalled(new { processId, models }, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            TimerHelper.AssertModels(model, result);
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
            async () => await api.WorkflowApiDataProcessesTimersCreateCollectionAsync(Guid.NewGuid(), [])
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
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var model = TimerHelper.Generate(processId);

        await repository.CreateAsync(model);

        // Act

        var requests = TimerHelper.CreateRequests(model);

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTimersCreateCollectionAsync(processId, requests)
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}
