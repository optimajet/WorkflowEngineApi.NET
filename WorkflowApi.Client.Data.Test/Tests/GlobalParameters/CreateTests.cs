using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.GlobalParameters;

[TestClass]
public class CreateTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateGuidParameter(TestService service)
    {
        // Arrange

        var type = typeof(Guid).ToString();
        var name = $"{Guid.NewGuid()} GuidParam";
        var value = Guid.NewGuid();
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateStringParameter(TestService service)
    {
        // Arrange

        var type = typeof(string).ToString();
        var name = $"{Guid.NewGuid()} StringParam";
        var value = "TestString";
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateIntParameter(TestService service)
    {
        // Arrange

        var type = typeof(int).ToString();
        var name = $"{Guid.NewGuid()} IntParam";
        var value = 123;
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateDateTimeParameter(TestService service)
    {
        // Arrange

        var type = typeof(DateTime).ToString();
        var name = $"{Guid.NewGuid()} DateTimeParam";
        var value = DateTime.UtcNow;
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateBoolParameter(TestService service)
    {
        // Arrange

        var type = typeof(bool).ToString();
        var name = $"{Guid.NewGuid()} BoolParam";
        var value = true;
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateByteArrayParameter(TestService service)
    {
        // Arrange

        var type = typeof(byte[]).ToString();
        var name = $"{Guid.NewGuid()} ByteArrayParam";
        var value = new byte[] {1, 2, 3};
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateStringListParameter(TestService service)
    {
        // Arrange

        var type = typeof(List<string>).ToString();
        var name = $"{Guid.NewGuid()} ListParam";
        var value = new List<string> {"One", "Two", "Three"};
        var model = GlobalParameterHelper.CreateModel(type, name, value);

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateComplexObjectParameter(TestService service)
    {
        // Arrange

        var type = typeof(ComplexTestObject).ToString();
        var name = $"{Guid.NewGuid()} ComplexParam";

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

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateDictionaryParameter(TestService service)
    {
        // Arrange

        var type = typeof(Dictionary<string, int>).ToString();
        var name = $"{Guid.NewGuid()} DictParam";

        var value = new Dictionary<string, int>
        {
            ["Key1"] = 1,
            ["Key2"] = 2,
            ["Key3"] = 3
        };

        var model = GlobalParameterHelper.CreateModel(type, name, value);

        // Act

        var api = service.Client.GlobalParameters;
        var request = GlobalParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataGlobalParametersCreateAsync(model.Type, model.Name, request);

        TestLogger.LogApiCalled(new {model.Type, model.Name, request}, result);
        
        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var dbModel = await repository.GetAsync(type, model.Name);

        GlobalParameterHelper.AssertModels(model, dbModel);
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

    [ClientTest(HostId.DataHost)]
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

    [ClientTest(HostId.DataHost)]
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

    [ClientTest(HostId.DataHost)]
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

        Assert.AreEqual(500, exception.ErrorCode);
    }
}