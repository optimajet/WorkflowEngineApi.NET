using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.GlobalParameters;

[TestClass]
public class GetTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetGuidGlobalParameter(TestService service)
    {
        // Arrange
        
        var type = "GuidType";
        var name = $"GuidParam_{Guid.NewGuid()}";
        var value = Guid.NewGuid();
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetStringGlobalParameter(TestService service)
    {
        // Arrange

        var type = "StringType";
        var name = $"StringParam_{Guid.NewGuid()}";
        var value = "TestString";
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetIntGlobalParameter(TestService service)
    {
        // Arrange

        var type = "IntType";
        var name = $"IntParam_{Guid.NewGuid()}";
        var value = 123;
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetDateTimeGlobalParameter(TestService service)
    {
        // Arrange

        var type = "DateTimeType";
        var name = $"DateTimeParam_{Guid.NewGuid()}";
        var value = DateTime.UtcNow;
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetBoolGlobalParameter(TestService service)
    {
        // Arrange

        var type = "BoolType";
        var name = $"BoolParam_{Guid.NewGuid()}";
        var value = true;
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetByteArrayGlobalParameter(TestService service)
    {
        // Arrange

        var type = "ByteArrayType";
        var name = $"ByteArrayParam_{Guid.NewGuid()}";
        var value = new byte[] {1, 2, 3};
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetStringListGlobalParameter(TestService service)
    {
        // Arrange

        var type = "ListType";
        var name = $"ListParam_{Guid.NewGuid()}";
        var value = new List<string> {"One", "Two", "Three"};
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetComplexObjectGlobalParameter(TestService service)
    {
        // Arrange

        var type = "ComplexType";
        var name = $"ComplexParam_{Guid.NewGuid()}";
        var value = new ComplexTestObject
        {
            Prop1 = "NestedValue",
            Prop2 = 456,
            Nested = new NestedObject
            {
                SubProp1 = false,
                SubProp2 = [7, 8, 9]
            }
        };
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetDictionaryGlobalParameter(TestService service)
    {
        // Arrange

        var type = "DictType";
        var name = $"DictParam_{Guid.NewGuid()}";
        var value = new Dictionary<string, int>
        {
            ["Key1"] = 1,
            ["Key2"] = 2,
            ["Key3"] = 3
        };
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.GlobalParameters.CreateAsync(model);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetAsync(type, name);

        // Assert

        GlobalParameterHelper.AssertModels(model, result, true);
    }
    
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

        List<GlobalParameterModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataGlobalParametersGetAsync(model.Type, model.Name);

            TestLogger.LogApiCalled(new { model.Type, model.Name }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            GlobalParameterHelper.AssertModels(models[i], results[i], true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = await GlobalParameterHelper.ExclusivePermissionsApi(service, "get");

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        List<GlobalParameterModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataGlobalParametersGetAsync(model.Type, model.Name);

            TestLogger.LogApiCalled(new { model.Type, model.Name }, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            GlobalParameterHelper.AssertModels(models[i], results[i], true);
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
            async () => await api.WorkflowApiDataGlobalParametersGetAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
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

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersGetAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(404, exception.ErrorCode);
    }
}