using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Schemes;

[TestClass]
public class CreateTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        // Act

        foreach (var model in models)
        {
            var request = SchemeHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataSchemesCreateAsync(model.Code, request);
            TestLogger.LogApiCalled(new { model.Code, request }, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Code);
            SchemeHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = await SchemeHelper.ExclusivePermissionsApi(service, "create");

        var models = SchemeHelper.Generate(3);

        // Act

        foreach (var model in models)
        {
            var request = SchemeHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataSchemesCreateAsync(model.Code, request);
            TestLogger.LogApiCalled(new { model.Code, request }, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Code);
            SchemeHelper.AssertModels(model, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await SchemeHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesCreateAsync(Guid.NewGuid().ToString(), SchemeHelper.CreateRequest(SchemeHelper.Generate()))
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(ProviderName.Mssql, ProviderName.Mysql, ProviderName.Oracle, ProviderName.Postgres, ProviderName.Sqlite)] //No constraints
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var model = SchemeHelper.Generate();

        await repository.CreateAsync(model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesCreateAsync(model.Code, SchemeHelper.CreateRequest(model))
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}