using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.InboxEntries;

[TestClass]
public class GetTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        List<InboxEntryModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesInboxEntriesGetAsync(processId, model.IdentityId);

            TestLogger.LogApiCalled(new { processId, model.IdentityId }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            InboxEntryHelper.AssertModels(models[i], results[i], true);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = await InboxEntryHelper.ExclusivePermissionsApi(service, "get");

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        List<InboxEntryModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesInboxEntriesGetAsync(processId, model.IdentityId);

            TestLogger.LogApiCalled(new { processId, model.IdentityId }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            InboxEntryHelper.AssertModels(models[i], results[i], true);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await InboxEntryHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesInboxEntriesGetAsync(Guid.NewGuid(), Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.InboxEntries;

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesInboxEntriesGetAsync(Guid.NewGuid(), Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(404, exception.ErrorCode);
    }
}