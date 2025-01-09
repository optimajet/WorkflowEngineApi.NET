using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.GlobalParameters;

[TestClass]
public class UpdateTests
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

        await repository.CreateAsync(models);

        // Act

        var updateRequests = GlobalParameterHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(model.Type, model.Name, updateRequest);

            TestLogger.LogApiCalled(new { model.Type, model.Name, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(updateRequest.Type, updateRequest.Name);
            GlobalParameterHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = await GlobalParameterHelper.ExclusivePermissionsApi(service, "update");

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = GlobalParameterHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(model.Type, model.Name, updateRequest);

            TestLogger.LogApiCalled(new { model.Type, model.Name, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(updateRequest.Type, updateRequest.Name);
            GlobalParameterHelper.AssertUpdated(updateRequest, result);
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
            async () => await api.WorkflowApiDataGlobalParametersUpdateAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), new GlobalParameterUpdateRequest())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.GlobalParameters;

        var type = Guid.NewGuid().ToString();
        var name = Guid.NewGuid().ToString();
        var updateRequest = new GlobalParameterUpdateRequest(Guid.NewGuid().ToString());

        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(type, name, updateRequest);

        TestLogger.LogApiCalled(new { type, name, updateRequest }, result);

        // Assert

        Assert.AreEqual(0, result?.UpdatedCount);
    }

    [ClientTest]
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var exist = GlobalParameterHelper.Generate();
        var model = GlobalParameterHelper.Generate();
        var updateRequest = new GlobalParameterUpdateRequest(exist.Type, exist.Name);

        await repository.CreateAsync(exist, model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersUpdateAsync(model.Type, model.Name, updateRequest)
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task ConflictTypeOnlyTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var exist = GlobalParameterHelper.Generate();
        var model = GlobalParameterHelper.Generate();
        model.Name = exist.Name;
        var updateRequest = new GlobalParameterUpdateRequest(exist.Type);

        await repository.CreateAsync(exist, model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersUpdateAsync(model.Type, model.Name, updateRequest)
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task ConflictNameOnlyTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var exist = GlobalParameterHelper.Generate();
        var model = GlobalParameterHelper.Generate();
        model.Type = exist.Type;
        var updateRequest = new GlobalParameterUpdateRequest(null!, exist.Name);

        await repository.CreateAsync(exist, model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersUpdateAsync(model.Type, model.Name, updateRequest)
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}