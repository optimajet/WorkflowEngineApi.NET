using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.GlobalParameters;

[TestClass]
public class CreateTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);

        // Act

        foreach (var model in models)
        {
            var request = GlobalParameterHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

            TestLogger.LogApiCalled(new { model.Type, model.Name, request}, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = await GlobalParameterHelper.ExclusivePermissionsApi(service, "create");

        var models = GlobalParameterHelper.Generate(3);

        // Act

        foreach (var model in models)
        {
            var request = GlobalParameterHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

            TestLogger.LogApiCalled(new { model.Type, model.Name, request}, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await GlobalParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersCreateAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), GlobalParameterHelper.CreateRequest(GlobalParameterHelper.Generate()))
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(ProviderName.Mongo, ProviderName.Mysql, ProviderName.Oracle, ProviderName.Postgres, ProviderName.Sqlite)] //No constraints
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var model = GlobalParameterHelper.Generate();

        await repository.CreateAsync(model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, GlobalParameterHelper.CreateRequest(model))
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}