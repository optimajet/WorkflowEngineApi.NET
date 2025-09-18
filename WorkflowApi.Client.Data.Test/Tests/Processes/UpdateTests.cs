using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Processes;

[TestClass]
public class UpdateTests
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

        var updateRequests = ProcessHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataProcessesUpdateAsync(model.Id, updateRequest);

            TestLogger.LogApiCalled(new { model.Id, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await repository.GetAsync(model.Id);
            ProcessHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = await ProcessHelper.ExclusivePermissionsApi(service, "update");

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = ProcessHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataProcessesUpdateAsync(model.Id, updateRequest);

            TestLogger.LogApiCalled(new { model.Id, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await repository.GetAsync(model.Id);
            ProcessHelper.AssertUpdated(updateRequest, result);
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
            async () => await api.WorkflowApiDataProcessesUpdateAsync(Guid.NewGuid(), new ProcessUpdateRequest())
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
        var id = Guid.NewGuid();
        var processUpdateRequest = new ProcessUpdateRequest(Guid.NewGuid().ToString());

        // Act

        var result = await api.WorkflowApiDataProcessesUpdateAsync(id, processUpdateRequest);

        TestLogger.LogApiCalled(new { id, processUpdateRequest }, result);

        // Assert

        Assert.AreEqual(0, result?.UpdatedCount);
    }
}