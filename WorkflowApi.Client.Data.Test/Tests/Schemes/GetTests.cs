using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Schemes;

[TestClass]
public class GetTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<SchemeModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataSchemesGetAsync(model.Code);

            TestLogger.LogApiCalled(new {model.Code}, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            SchemeHelper.AssertModels(models[i], results[i]);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = await SchemeHelper.ExclusivePermissionsApi(service, "get");

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<SchemeModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataSchemesGetAsync(model.Code);

            TestLogger.LogApiCalled(new {model.Code}, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            SchemeHelper.AssertModels(models[i], results[i]);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await SchemeHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesGetAsync(Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Schemes;

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesGetAsync(Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(404, exception.ErrorCode);
    }
}