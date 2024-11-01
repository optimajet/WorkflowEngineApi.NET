using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using OptimaJet.Workflow.Api.Models;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;
using SchemeUpdateRequest = WorkflowApi.Client.Model.SchemeUpdateRequest;

namespace WorkflowApi.Client.Test.Tests.Schemes;

[TestClass]
public class UpdateTests
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

        await repository.CreateAsync(models);

        // Act

        var updateRequests = SchemeHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataSchemesUpdateAsync(model.Code, updateRequest);

            TestLogger.LogApiCalled(new { model.Code, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(updateRequest.Code);
            SchemeHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task UpdateTagsOnlyTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = SchemeHelper.UpdateRequests(models);

        foreach (var updateRequest in updateRequests)
        {
            updateRequest.Code = null;
            updateRequest.Scheme = null;
            updateRequest.InlinedSchemes = null;
            updateRequest.CanBeInlined = null;
        }

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataSchemesUpdateAsync(model.Code, updateRequest);

            TestLogger.LogApiCalled(new { model.Code, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        for (var i = 0; i < models.Length; i++)
        {
            var updateRequest = updateRequests[i];
            var result = await repository.GetAsync(models[i].Code);
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(Converter.SimplifyTags(updateRequest.Tags), result.Tags);
            CollectionAssert.AreEqual(Converter.SimplifyTags(updateRequest.Tags), result.Scheme.Tags);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task UpdateSchemaTagsOnlyTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = SchemeHelper.UpdateRequests(models);

        foreach (var updateRequest in updateRequests)
        {
            updateRequest.Code = null;
            updateRequest.Scheme.Tags = Guid.NewGuid().ToString().Select(c => c.ToString()).ToList();
            updateRequest.Tags = null;
            updateRequest.InlinedSchemes = null;
            updateRequest.CanBeInlined = null;
        }

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataSchemesUpdateAsync(model.Code, updateRequest);

            TestLogger.LogApiCalled(new { model.Code, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        for (var i = 0; i < models.Length; i++)
        {
            var updateRequest = updateRequests[i];
            var result = await repository.GetAsync(models[i].Code);
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(Converter.SimplifyTags(updateRequest.Scheme.Tags), result.Tags);
            CollectionAssert.AreEqual(Converter.SimplifyTags(updateRequest.Scheme.Tags), result.Scheme.Tags);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = await SchemeHelper.ExclusivePermissionsApi(service, "update");

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = SchemeHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataSchemesUpdateAsync(model.Code, updateRequest);

            TestLogger.LogApiCalled(new { model.Code, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(updateRequest.Code);
            SchemeHelper.AssertUpdated(updateRequest, result);
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
            async () => await api.WorkflowApiDataSchemesUpdateAsync(Guid.NewGuid().ToString(), new SchemeUpdateRequest())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Schemes;
        var code = Guid.NewGuid().ToString();
        var schemeUpdateRequest = new SchemeUpdateRequest(Guid.NewGuid().ToString());

        // Act

        var result = await api.WorkflowApiDataSchemesUpdateAsync(code, schemeUpdateRequest);

        TestLogger.LogApiCalled(new { code, schemeUpdateRequest }, result);

        // Assert

        Assert.AreEqual(0, result?.UpdatedCount);
    }

    [ClientTest(ProviderName.Mongo)] //No constraints
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var exist = SchemeHelper.Generate();
        var model = SchemeHelper.Generate();
        var updateRequest = new SchemeUpdateRequest(exist.Code);

        await repository.CreateAsync(exist, model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesUpdateAsync(model.Code, updateRequest)
        );

        // Assert

        Assert.AreEqual(400, exception.ErrorCode);
    }
}