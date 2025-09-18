using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.GlobalParameters;

[TestClass]
public class UpdateTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateGuidParameter(TestService service)
    {
        // Arrange

        var type = typeof(Guid).ToString();
        var name = $"{Guid.NewGuid()} GuidParam";

        var initial = GlobalParameterHelper.CreateModel(type, name, Guid.NewGuid());
        var updated = new GlobalParameterUpdateRequest(type, name, Guid.NewGuid());

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateStringParameter(TestService service)
    {
        // Arrange

        var type = typeof(string).ToString();
        var name = $"{Guid.NewGuid()} StringParam";
        var initial = GlobalParameterHelper.CreateModel(type, name, "Initial");
        var updated = new GlobalParameterUpdateRequest(type, name, "Updated");

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateIntParameter(TestService service)
    {
        // Arrange

        var type = typeof(int).ToString();
        var name = $"{Guid.NewGuid()} IntParam";
        var initial = GlobalParameterHelper.CreateModel(type, name, 1);
        var updated = new GlobalParameterUpdateRequest(type, name, 100);

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateDateTimeParameter(TestService service)
    {
        // Arrange

        var type = typeof(DateTime).ToString();
        var name = $"{Guid.NewGuid()} DateTimeParam";
        var initial = GlobalParameterHelper.CreateModel(type, name, DateTime.UtcNow);
        var updated = new GlobalParameterUpdateRequest(type, name, DateTime.UtcNow.AddHours(1));

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateBoolParameter(TestService service)
    {
        // Arrange

        var type = typeof(bool).ToString();
        var name = $"{Guid.NewGuid()} BoolParam";
        var initial = GlobalParameterHelper.CreateModel(type, name, false);
        var updated = new GlobalParameterUpdateRequest(type, name, true);

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateByteArrayParameter(TestService service)
    {
        // Arrange

        var type = typeof(byte[]).ToString();
        var name = $"{Guid.NewGuid()} ByteArrayParam";
        var initial = GlobalParameterHelper.CreateModel(type, name, new byte[] {1, 2});
        var updated = new GlobalParameterUpdateRequest(type, name, new byte[] {3, 4});

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateStringListParameter(TestService service)
    {
        // Arrange

        var type = typeof(List<string>).ToString();
        var name = $"{Guid.NewGuid()} ListParam";
        var initial = GlobalParameterHelper.CreateModel(type, name, new List<string> {"A"});
        var updated = new GlobalParameterUpdateRequest(type, name, new List<string> {"B", "C"});

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateComplexObjectParameter(TestService service)
    {
        // Arrange

        var type = typeof(ComplexTestObject).ToString();
        var name = $"{Guid.NewGuid()} ComplexParam";
        var initial = GlobalParameterHelper.CreateModel(type, name, new ComplexTestObject
        {
            Prop1 = "Initial",
            Prop2 = 1,
            Nested = new NestedObject {SubProp1 = true, SubProp2 = [1]}
        });

        var updated = new GlobalParameterUpdateRequest(type, name, new ComplexTestObject
        {
            Prop1 = "Updated",
            Prop2 = 2,
            Nested = new NestedObject {SubProp1 = false, SubProp2 = [9]}
        });

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateDictionaryParameter(TestService service)
    {
        // Arrange

        var type = typeof(Dictionary<string, int>).ToString();
        var name = $"{Guid.NewGuid()} DictParam";
        
        var initial = GlobalParameterHelper.CreateModel(type, name, new Dictionary<string, int> {["A"] = 1});
        var updated = new GlobalParameterUpdateRequest(type, name, new Dictionary<string, int> {["B"] = 2});

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;
        await repository.CreateAsync(initial);
        
        
        // Act

        var result = await api.WorkflowApiDataGlobalParametersUpdateAsync(initial.Type, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.Type, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(type, initial.Name);
        GlobalParameterHelper.AssertUpdated(updated, dbModel);
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

    [ClientTest(HostId.DataHost)]
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

    [ClientTest(HostId.DataHost)]
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

    [ClientTest(HostId.DataHost)]
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

    [ClientTest(HostId.DataHost)]
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

        Assert.AreEqual(500, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
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

        Assert.AreEqual(500, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
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

        Assert.AreEqual(500, exception.ErrorCode);
    }
}