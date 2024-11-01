using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.InboxEntries;

[TestClass]
public class DeleteTests
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

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesInboxEntriesDeleteAsync(processId, model.IdentityId);

            TestLogger.LogApiCalled(new { processId, model.IdentityId }, result);

            Assert.AreEqual(1, result?.DeletedCount);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Id);
            Assert.IsNull(result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = await InboxEntryHelper.ExclusivePermissionsApi(service, "delete");

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesInboxEntriesDeleteAsync(processId, model.IdentityId);

            TestLogger.LogApiCalled(new { processId, model.IdentityId }, result);

            Assert.AreEqual(1, result?.DeletedCount);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Id);
            Assert.IsNull(result);
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
            async () => await api.WorkflowApiDataProcessesInboxEntriesDeleteAsync(Guid.NewGuid(), Guid.NewGuid().ToString())
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
        var processId = Guid.NewGuid();
        var identityId = Guid.NewGuid().ToString();

        // Act

        var result = await api.WorkflowApiDataProcessesInboxEntriesDeleteAsync(processId, identityId);

        TestLogger.LogApiCalled(new { processId, identityId }, result);

        // Assert

        Assert.AreEqual(0, result?.DeletedCount);
    }
}