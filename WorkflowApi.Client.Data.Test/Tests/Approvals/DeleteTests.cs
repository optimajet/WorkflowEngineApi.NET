using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Approvals;

[TestClass]
public class DeleteTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result =  await api.WorkflowApiDataProcessesApprovalsDeleteAsync(processId, model.Id);

            TestLogger.LogApiCalled(new {processId, model.Id}, result);

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
        var repository = service.Repository.Approvals;
        var api = await ApprovalHelper.ExclusivePermissionsApi(service, "delete");

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesApprovalsDeleteAsync(processId, model.Id);

            TestLogger.LogApiCalled(new {processId, model.Id}, result);

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

        var api = await ApprovalHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesApprovalsDeleteAsync(Guid.NewGuid(), Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var id = Guid.NewGuid();

        // Act

        var result = await api.WorkflowApiDataProcessesApprovalsDeleteAsync(processId, id);

        TestLogger.LogApiCalled(new {processId, modelId = id}, result);

        // Assert

        Assert.AreEqual(0, result?.DeletedCount);
    }
}