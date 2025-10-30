using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Rpc.Test.Models;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class SetStateWithExecutionTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetState(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var firstState = "Activity1";
        var secondState = "Activity2";
        var thirdState = "Activity3";

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial")
            .Initial()
            .CreateActivity(firstState).Ref(out var firstActivity)
            .State(firstState)
            .EnableSetState()
            .CreateActivity(secondState).Ref(out var secondActivity)
            .State(secondState)
            .EnableSetState()
            .CreateTransition("Transition1", firstActivity, secondActivity)
            .CreateActivity(thirdState).Ref(out var thirdActivity)
            .CreateTransition("Transition2", secondActivity, thirdActivity);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = firstState,
            ProcessParameters =
            [
                new ProcessParameter("TestParam1", "TestValue1", ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("TestParam2", "TestValue2", ParameterPurposeWithoutSystem.Temporary)
            ]
        };

        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);

        // Assert

        var processInstance = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new(processId));
        Assert.AreEqual(secondState, processInstance.ProcessInstance.CurrentState);

        var parameter1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "TestParam1");
        Assert.IsNotNull(parameter1);
        AssertParameter("TestValue1", parameter1.Value);

        var parameter2Exception = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
            await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "TestParam2"));
        Assert.AreEqual(404, parameter2Exception.ErrorCode); // Not persisted, should not be found
        StringAssert.Contains(parameter2Exception.Message, $"Parameter with process id {processId} and name TestParam2 not found");
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndGuidParameters(TestService service)
    {
        // Arrange
        
        var schemeCode = Guid.NewGuid().ToString();
        var state = "Activity1";
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial").Initial()
            .CreateActivity(state).State(state).EnableSetState()
            .CreateParameter("GuidParam3", typeof(Guid), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("GuidParam4", typeof(Guid));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();
        var guid4 = Guid.NewGuid();

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = state,
            ProcessParameters =
            [
                new ProcessParameter("GuidParam1", guid1, ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("GuidParam2", guid2, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("GuidParam3", guid3, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("GuidParam4", guid4, ParameterPurposeWithoutSystem.Persistence),
            ]
        };
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);
      
        // Assert

        var param1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "GuidParam1");
        AssertParameter(guid1, param1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "GuidParam2"));
        Assert.AreEqual(404, ex2.ErrorCode);

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "GuidParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter(guid3, parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "GuidParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter(guid4, parameter4.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndIntParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var state = "Activity1";
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial").Initial()
            .CreateActivity(state).State(state).EnableSetState()
            .CreateParameter("IntParam3", typeof(int), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("IntParam4", typeof(int));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = state,
            ProcessParameters =
            [
                new ProcessParameter("IntParam1", 123, ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("IntParam2", 456, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("IntParam3", 234, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("IntParam4", 877, ParameterPurposeWithoutSystem.Persistence),
            ]
        };
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);
      
        // Assert

        var param1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "IntParam1");
        AssertParameter(123, param1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "IntParam2"));
        Assert.AreEqual(404, ex2.ErrorCode);

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "IntParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter(234, parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "IntParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter(877, parameter4.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndDateTimeParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var state = "Activity1";
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial").Initial()
            .CreateActivity(state).State(state).EnableSetState()
            .CreateParameter("DateTimeParam3", typeof(DateTime), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("DateTimeParam4", typeof(DateTime));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var dt1 = DateTime.UtcNow;
        var dt2 = dt1.AddDays(1);
        var dt3 = dt1.AddDays(2);
        var dt4 = dt1.AddDays(3);

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = state,
            ProcessParameters =
            [
                new ProcessParameter("DateTimeParam1", dt1, ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("DateTimeParam2", dt2, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("DateTimeParam3", dt3, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("DateTimeParam4", dt4, ParameterPurposeWithoutSystem.Persistence),
            ]
        };
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);
      
        // Assert

        var param1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "DateTimeParam1");
        AssertParameter(dt1, param1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "DateTimeParam2"));
        Assert.AreEqual(404, ex2.ErrorCode);

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "DateTimeParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter(dt3, parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "DateTimeParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter(dt4, parameter4.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndBoolParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var state = "Activity1";
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial").Initial()
            .CreateActivity(state).State(state).EnableSetState()
            .CreateParameter("BoolParam3", typeof(bool), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("BoolParam4", typeof(bool));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = state,
            ProcessParameters =
            [
                new ProcessParameter("BoolParam1", true, ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("BoolParam2", false, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("BoolParam3", true, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("BoolParam4", false, ParameterPurposeWithoutSystem.Persistence),
            ]
        };
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);
      
        // Assert

        var param1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "BoolParam1");
        AssertParameter(true, param1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "BoolParam2"));
        Assert.AreEqual(404, ex2.ErrorCode);

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "BoolParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter(true, parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "BoolParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter(false, parameter4.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndByteArrayParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var state = "Activity1";
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial").Initial()
            .CreateActivity(state).State(state).EnableSetState()
            .CreateParameter("ByteArrayParam3", typeof(byte[]), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("ByteArrayParam4", typeof(byte[]));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var arr1 = new byte[] {1, 2, 3};
        var arr2 = new byte[] {4, 5, 6};
        var arr3 = new byte[] {4, 8, 6};
        var arr4 = new byte[] {5, 5, 6};

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = state,
            ProcessParameters =
            [
                new ProcessParameter("ByteArrayParam1", arr1, ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("ByteArrayParam2", arr2, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("ByteArrayParam3", arr3, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("ByteArrayParam4", arr4, ParameterPurposeWithoutSystem.Persistence),
            ]
        };
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);
      
        // Assert

        var param1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ByteArrayParam1");
        AssertParameter(arr1, param1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ByteArrayParam2"));
        Assert.AreEqual(404, ex2.ErrorCode);

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ByteArrayParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter(arr3, parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ByteArrayParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter(arr4, parameter4.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndStringListParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var state = "Activity1";
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial").Initial()
            .CreateActivity(state).State(state).EnableSetState()
            .CreateParameter("ListParam3", typeof(List<string>), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("ListParam4", typeof(List<string>));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var list1 = new List<string> {"A", "B"};
        var list2 = new List<string> {"C", "D"};
        var list3 = new List<string> {"O", "D"};
        var list4 = new List<string> {"C", "H"};

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = state,
            ProcessParameters =
            [
                new ProcessParameter("ListParam1", list1, ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("ListParam2", list2, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("ListParam3", list3, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("ListParam4", list4, ParameterPurposeWithoutSystem.Persistence),
            ]
        };
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);

        // Assert
        
        var param1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ListParam1");
        AssertParameter(list1, param1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ListParam2"));
        Assert.AreEqual(404, ex2.ErrorCode);

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ListParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter(list3, parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ListParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter(list4, parameter4.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndDictionaryParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var state = "Activity1";
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial").Initial()
            .CreateActivity(state).State(state).EnableSetState()
            .CreateParameter("DictParam3", typeof(Dictionary<string, int>), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("DictParam4", typeof(Dictionary<string, int>));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var dict1 = new Dictionary<string, int> {{"K1", 1}};
        var dict2 = new Dictionary<string, int> {{"K2", 2}};
        var dict3 = new Dictionary<string, int> {{"K3", 3}};
        var dict4 = new Dictionary<string, int> {{"K4", 4}};

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = state,
            ProcessParameters =
            [
                new ProcessParameter("DictParam1", dict1, ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("DictParam2", dict2, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("DictParam3", dict3, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("DictParam4", dict4, ParameterPurposeWithoutSystem.Persistence),
            ]
        };
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);
       
        // Assert

        var param1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "DictParam1");
        AssertParameter(dict1, param1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "DictParam2"));
        Assert.AreEqual(404, ex2.ErrorCode);

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "DictParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter(dict3, parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "DictParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter(dict4, parameter4.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndComplexObjectParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var state = "Activity1";
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial").Initial()
            .CreateActivity(state).State(state).EnableSetState()
            .CreateParameter("ComplexParam3", typeof(ComplexTestObject), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("ComplexParam4", typeof(ComplexTestObject));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var obj1 = new ComplexTestObject {Prop1 = "A", Prop2 = 1, Nested = new NestedObject {SubProp1 = true, SubProp2 = [1, 2]}};
        var obj2 = new ComplexTestObject {Prop1 = "B", Prop2 = 2, Nested = new NestedObject {SubProp1 = false, SubProp2 = [3, 4]}};
        var obj3 = new ComplexTestObject {Prop1 = "C", Prop2 = 3, Nested = new NestedObject {SubProp1 = true, SubProp2 = [5, 6]}};
        var obj4 = new ComplexTestObject {Prop1 = "D", Prop2 = 4, Nested = new NestedObject {SubProp1 = false, SubProp2 = [7, 8]}};
        
        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            StateName = state,
            ProcessParameters =
            [
                new ProcessParameter("ComplexParam1", obj1, ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("ComplexParam2", obj2, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("ComplexParam3", obj3, ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("ComplexParam4", obj4, ParameterPurposeWithoutSystem.Persistence),

            ]
        };
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);

        var param1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ComplexParam1");
        AssertParameter(obj1, param1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ComplexParam2"));
        Assert.AreEqual(404, ex2.ErrorCode);

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ComplexParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter(obj3, parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "ComplexParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter(obj4, parameter4.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithExecutionAndStringParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var firstState = "Activity1";
        var secondState = "Activity2";
        var identityId = "test-identity";
        var impersonatedIdentityId = "test-impersonated-identity";

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial")
            .Initial()
            .CreateActivity(firstState)
            .State(firstState)
            .EnableSetState()
            .CreateActivity(secondState)
            .State(secondState)
            .EnableSetState()
            .CreateParameter("TestParam3", typeof(string), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence)
            .CreateParameter("TestParam4", typeof(string));

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new SetStateWithExecutionRequest
        {
            ProcessId = processId,
            IdentityId = identityId,
            ImpersonatedIdentityId = impersonatedIdentityId,
            StateName = firstState,
            ProcessParameters =
            [
                new ProcessParameter("TestParam1", "TestValue1", ParameterPurposeWithoutSystem.Persistence),
                new ProcessParameter("TestParam2", "TestValue2", ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("TestParam3", "TestValue3", ParameterPurposeWithoutSystem.Temporary),
                new ProcessParameter("TestParam4", "TestValue4", ParameterPurposeWithoutSystem.Persistence),
            ]
        };

        await service.Client.RpcStates.WorkflowApiRpcSetStateWithExecutionAsync(request);

        // Assert

        var processInstance = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new(processId));
        Assert.AreEqual(firstState, processInstance.ProcessInstance.CurrentState);

        var parameter1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "TestParam1");
        Assert.IsNotNull(parameter1);
        AssertParameter("TestValue1", parameter1.Value);

        var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "TestParam2"));
        Assert.AreEqual(404, ex2.ErrorCode); // Not persisted, should not found
        StringAssert.Contains(ex2.Message, $"Parameter with process id {processId} and name TestParam2 not found");

        var parameter3 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "TestParam3");
        Assert.IsNotNull(parameter3);
        AssertParameter("TestValue3", parameter3.Value);

        var parameter4 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "TestParam4");
        Assert.IsNotNull(parameter4);
        AssertParameter("TestValue4", parameter4.Value);

        // TODO check IdentityId, ImpersonatedIdentityId
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var activityName = "Activity2";

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity(activityName)
            .State(activityName)
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        // Act

        var request = new SetStateWithExecutionRequest(processId)
        {
            ProcessId = processId,
            StateName = activityName,
        };
        await service.Client.ExclusivePermissions(c => c.RpcStates, WorkflowApiOperationId.RpcSetStateWithExecution).WorkflowApiRpcSetStateWithExecutionAsync(request);

        // Assert

        var processInstance = await service.Client.Processes.WorkflowApiDataProcessesGetAsync(processId);
        Assert.AreEqual(activityName, processInstance.StateName);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        // Act

        var request = new SetStateWithExecutionRequest(Guid.NewGuid());

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>()).WorkflowApiRpcSetStateWithExecutionAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    private void AssertParameter(object? expected, object? actual)
    {
        string expectedJson = JsonConvert.SerializeObject(expected);
        string actualJson = JsonConvert.SerializeObject(actual);

        expectedJson = expectedJson.ToLowerInvariant();
        actualJson = actualJson.ToLowerInvariant();

        Assert.AreEqual(expectedJson, actualJson);
    }
}