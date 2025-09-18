using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.GlobalParameters;

[TestClass]
public class DeleteTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataGlobalParametersDeleteAsync(model.Type, model.Name);

            TestLogger.LogApiCalled(new { model.Type, model.Name }, result);

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
        var repository = service.Repository.GlobalParameters;
        var api = await GlobalParameterHelper.ExclusivePermissionsApi(service, "delete");

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataGlobalParametersDeleteAsync(model.Type, model.Name);

            TestLogger.LogApiCalled(new { model.Type, model.Name }, result);

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

        var api = await GlobalParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersDeleteAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.GlobalParameters;
        var type = Guid.NewGuid().ToString();
        var name = Guid.NewGuid().ToString();

        // Act

        var result = await api.WorkflowApiDataGlobalParametersDeleteAsync(type, name);

        TestLogger.LogApiCalled(new { type, name }, result);

        // Assert

        Assert.AreEqual(0, result?.DeletedCount);
    }
}