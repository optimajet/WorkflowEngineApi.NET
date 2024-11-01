using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Parameters;

[TestClass]
public class CreateTests
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

        if (service.Configuration.AppConfiguration.Provider == Provider.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act

        foreach (var model in models)
        {
            var request = ParameterHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataProcessesParametersCreateAsync(processId, model.Name, request);

            TestLogger.LogApiCalled(new { processId, model.Name, request }, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "create");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        if (service.Configuration.AppConfiguration.Provider == Provider.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act

        foreach (var model in models)
        {
            var request = ParameterHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataProcessesParametersCreateAsync(processId, model.Name, request);

            TestLogger.LogApiCalled(new { processId, model.Name, request }, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
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
            async () => await api.WorkflowApiDataProcessesParametersCreateAsync(Guid.NewGuid(), Guid.NewGuid().ToString(), ParameterHelper.CreateRequest(ParameterHelper.Generate(Guid.NewGuid())))
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [Ignore] //No constraints
    [ClientTest]
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var model = ParameterHelper.Generate(processId);

        if (service.Configuration.AppConfiguration.Provider == Provider.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        await repository.CreateAsync(model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersCreateAsync(processId, model.Name, ParameterHelper.CreateRequest(model))
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}