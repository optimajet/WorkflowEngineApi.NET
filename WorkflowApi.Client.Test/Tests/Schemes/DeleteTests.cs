﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Schemes;

[TestClass]
public class DeleteTests
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

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataSchemesDeleteAsync(model.Code);

            TestLogger.LogApiCalled(new {model.Code}, result);

            Assert.AreEqual(1, result?.DeletedCount);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Code);
            Assert.IsNull(result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = await SchemeHelper.ExclusivePermissionsApi(service, "delete");

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataSchemesDeleteAsync(model.Code);

            TestLogger.LogApiCalled(new {model.Code}, result);

            Assert.AreEqual(1, result?.DeletedCount);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Code);
            Assert.IsNull(result);
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
            async () => await api.WorkflowApiDataSchemesDeleteAsync(Guid.NewGuid().ToString())
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

        // Act

        var result = await api.WorkflowApiDataSchemesDeleteAsync(code);

        TestLogger.LogApiCalled(new {code}, result);

        // Assert

        Assert.AreEqual(0, result?.DeletedCount);
    }
}