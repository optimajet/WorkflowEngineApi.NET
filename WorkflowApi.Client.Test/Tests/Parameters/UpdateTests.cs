using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Parameters;

[TestClass]
public class UpdateTests
{
    [ClientTest]
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

        var updateRequests = ParameterHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataProcessesParametersUpdateAsync(processId, model.Name, updateRequest);

            TestLogger.LogApiCalled(new { processId, model.Name, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(processId, updateRequest.Name);
            ParameterHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "update");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = ParameterHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataProcessesParametersUpdateAsync(processId, model.Name, updateRequest);

            TestLogger.LogApiCalled(new { processId, model.Name, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(processId, updateRequest.Name);
            ParameterHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersUpdateAsync(Guid.NewGuid(), Guid.NewGuid().ToString(), new ParameterUpdateRequest())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();

        if (service.Configuration.AppConfiguration.Provider == Provider.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        var name = Guid.NewGuid().ToString();
        var parameterUpdateRequest = new ParameterUpdateRequest(Guid.NewGuid().ToString());

        // Act

        var result = await api.WorkflowApiDataProcessesParametersUpdateAsync(processId, name, parameterUpdateRequest);

        TestLogger.LogApiCalled(new { processId, name, parameterUpdateRequest }, result);

        // Assert

        Assert.AreEqual(0, result?.UpdatedCount);
    }

    [ClientTest(ProviderName.Mongo)] //No constraints
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var exist = ParameterHelper.Generate(processId);
        var model = ParameterHelper.Generate(processId);
        var updateRequest = new ParameterUpdateRequest(exist.Name);

        await repository.CreateAsync(exist, model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersUpdateAsync(processId, model.Name, updateRequest)
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}