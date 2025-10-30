using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class SetProcessNewStatusTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetNewProcessStatus(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        await CreateInitialData(service, processId);

        // Act
        var runningProcessStatus = Enum.Parse<ProcessStatusAvailableToSet>(OptimaJet.Workflow.Core.Persistence.ProcessStatus.Running.Name);
        var setRunningStatusRequest = new SetProcessNewStatusRequest(processId, runningProcessStatus);
        await service.Client.RpcInstance.WorkflowApiRpcSetProcessNewStatusAsync(setRunningStatusRequest);

        // Assert

        var process = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new (processId));
        Assert.AreEqual(OptimaJet.Workflow.Core.Persistence.ProcessStatus.Running.Name, process.ProcessStatus.ToString());

        // Act

        var idledProcessStatus = Enum.Parse<ProcessStatusAvailableToSet>(OptimaJet.Workflow.Core.Persistence.ProcessStatus.Idled.Name);

        var setIdledStatusRequest = new SetProcessNewStatusRequest(processId, idledProcessStatus);
        await service.Client.RpcInstance.WorkflowApiRpcSetProcessNewStatusAsync(setIdledStatusRequest);

        // Assert
        
        process = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new (processId));
        Assert.AreEqual(OptimaJet.Workflow.Core.Persistence.ProcessStatus.Idled.Name, process.ProcessStatus.ToString());
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnBadRequestIfNewStatusNotExist(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        await CreateInitialData(service, processId);

        // Act
        var notExistProcessStatus = (ProcessStatusAvailableToSet) 999;
        var setRunningStatusRequest = new SetProcessNewStatusRequest(processId, notExistProcessStatus);
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(() => service.Client.RpcInstance.WorkflowApiRpcSetProcessNewStatusAsync(setRunningStatusRequest));

        // Assert
        
        Assert.AreEqual(exception.ErrorCode, 400);

        string errorMessage = GetExceptionErrorContentMessage(exception);
        Assert.AreEqual(errorMessage, $"Process status '{notExistProcessStatus}' not found.");
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        await CreateInitialData(service, processId);

        // Act

        var runningProcessStatus = Enum.Parse<ProcessStatusAvailableToSet>(OptimaJet.Workflow.Core.Persistence.ProcessStatus.Running.Name);
        var setRunningStatusRequest = new SetProcessNewStatusRequest(processId, runningProcessStatus);
        await service.Client.ExclusivePermissions(c => c.RpcInstance, WorkflowApiOperationId.RpcSetProcessNewStatus).WorkflowApiRpcSetProcessNewStatusAsync(setRunningStatusRequest);

        // Assert

        var process = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new (processId));
        Assert.AreEqual(OptimaJet.Workflow.Core.Persistence.ProcessStatus.Running.Name, process.ProcessStatus.ToString());
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new SetProcessNewStatusRequest(Guid.NewGuid(), ProcessStatusAvailableToSet.Error);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcSetProcessNewStatusAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    private static string GetExceptionErrorContentMessage(ApiException exception)
    {
        var errorContent = JObject.Parse(exception.ErrorContent.ToString()!);
        var errorMessage = errorContent["message"]!.ToString();
        return errorMessage;
    }

    private static async Task CreateInitialData(TestService service, Guid processId)
    {
        var schemeCode = Guid.NewGuid().ToString();
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity")
            .Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var createInstanceRequest = new CreateInstanceRequest
        {
            ProcessId = processId,
            SchemeCode = schemeCode,
            IdentityId = Guid.NewGuid().ToString(),
            ImpersonatedIdentityId = Guid.NewGuid().ToString(),
            CalendarName = Guid.NewGuid().ToString()
        };
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(createInstanceRequest);
    }
}