using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using OptimaJet.Workflow.Core;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Parameters;

[TestClass]
public class UpdateTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateGuidParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "GuidParam";

        var initial = ParameterHelper.CreateModel(processId, name, Guid.NewGuid());
        var updated = new ParameterUpdateRequest(name, Guid.NewGuid());

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateStringParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "StringParam";
        var initial = ParameterHelper.CreateModel(processId, name, "Initial");
        var updated = new ParameterUpdateRequest(name, "Updated");

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);

        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateIntParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "IntParam";
        var initial = ParameterHelper.CreateModel(processId, name, 1);
        var updated = new ParameterUpdateRequest(name, 100);

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateDateTimeParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "DateTimeParam";
        var initial = ParameterHelper.CreateModel(processId, name, DateTime.UtcNow);
        var updated = new ParameterUpdateRequest(name, DateTime.UtcNow.AddHours(1));

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
       
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateBoolParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "BoolParam";
        var initial = ParameterHelper.CreateModel(processId, name, false);
        var updated = new ParameterUpdateRequest(name, true);

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateByteArrayParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "ByteArrayParam";
        var initial = ParameterHelper.CreateModel(processId, name, new byte[] {1, 2});
        var updated = new ParameterUpdateRequest(name, new byte[] {3, 4});

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateStringListParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "ListParam";
        var initial = ParameterHelper.CreateModel(processId, name, new List<string> {"A"});
        var updated = new ParameterUpdateRequest(name, new List<string> {"B", "C"});

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateComplexObjectParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "ComplexParam";
        var initial = ParameterHelper.CreateModel(processId, name, new ComplexTestObject
        {
            Prop1 = "Initial",
            Prop2 = 1,
            Nested = new NestedObject {SubProp1 = true, SubProp2 = [1]}
        });

        var updated = new ParameterUpdateRequest(name, new ComplexTestObject
        {
            Prop1 = "Updated",
            Prop2 = 2,
            Nested = new NestedObject {SubProp1 = false, SubProp2 = [9]}
        });

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldUpdateDictionaryParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "DictParam";
        var initial = ParameterHelper.CreateModel(processId, name, new Dictionary<string, int> {["A"] = 1});
        var updated = new ParameterUpdateRequest(name, new Dictionary<string, int> {["B"] = 2});

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        await repository.CreateAsync(initial);

        // Act

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersUpdateAsync(initial.ProcessId, initial.Name, updated);
        TestLogger.LogApiCalled(new {initial.ProcessId, initial.Name, initial}, result);
        
        // Assert

        Assert.AreEqual(1, result?.UpdatedCount);
        
        var dbModel = await repository.GetAsync(initial.ProcessId, initial.Name);
        ParameterHelper.AssertUpdated(updated, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = ParameterHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataProcessesParametersUpdateAsync(processId, model.Name, updateRequest);

            TestLogger.LogApiCalled(new { processId, model.Name, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(processId, updateRequest.Name);
            ParameterHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "update");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        var updateRequests = ParameterHelper.UpdateRequests(models);

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var updateRequest = updateRequests[i];
            var result = await api.WorkflowApiDataProcessesParametersUpdateAsync(processId, model.Name, updateRequest);

            TestLogger.LogApiCalled(new { processId, model.Name, updateRequest }, result);

            Assert.AreEqual(1, result?.UpdatedCount);
        }

        // Assert

        foreach (var updateRequest in updateRequests)
        {
            var result = await repository.GetAsync(processId, updateRequest.Name);
            ParameterHelper.AssertUpdated(updateRequest, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersUpdateAsync(Guid.NewGuid(), Guid.NewGuid().ToString(), new ParameterUpdateRequest())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();

        if (service.TenantOptions.DataProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        var name = Guid.NewGuid().ToString();
        var parameterUpdateRequest = new ParameterUpdateRequest(name, Guid.NewGuid().ToString());

        // Act

        var result = await api.WorkflowApiDataProcessesParametersUpdateAsync(processId, name, parameterUpdateRequest);

        TestLogger.LogApiCalled(new { processId, name, parameterUpdateRequest }, result);

        // Assert

        Assert.AreEqual(0, result?.UpdatedCount);
    }

    [ClientTest(HostId.DataHost, ExcludeProviders = [ProviderName.Mongo])] //No constraints
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var exist = ParameterHelper.Generate(processId);
        var model = ParameterHelper.Generate(processId);
        var updateRequest = new ParameterUpdateRequest(exist.Name);

        await repository.CreateAsync(exist, model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersUpdateAsync(processId, model.Name, updateRequest)
        );

        // Assert

        Assert.AreEqual(500, exception.ErrorCode);
    }
}