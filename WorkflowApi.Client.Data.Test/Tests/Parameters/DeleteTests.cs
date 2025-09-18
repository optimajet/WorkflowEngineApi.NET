using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Core;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Parameters;

[TestClass]
public class DeleteTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesParametersDeleteAsync(processId, model.Name);

            TestLogger.LogApiCalled(new { processId, model.Name }, result);

            Assert.AreEqual(1, result?.DeletedCount);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.ProcessId, model.Name);
            Assert.IsNull(result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "delete");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesParametersDeleteAsync(processId, model.Name);

            TestLogger.LogApiCalled(new { processId, model.Name }, result);

            Assert.AreEqual(1, result?.DeletedCount);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.ProcessId, model.Name);
            Assert.IsNull(result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersDeleteAsync(Guid.NewGuid(), Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();

        if (service.TenantOptions.DataProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        var name = Guid.NewGuid().ToString();

        // Act

        var result = await api.WorkflowApiDataProcessesParametersDeleteAsync(processId, name);

        TestLogger.LogApiCalled(new { processId, name }, result);

        // Assert

        Assert.AreEqual(0, result?.DeletedCount);
    }
}