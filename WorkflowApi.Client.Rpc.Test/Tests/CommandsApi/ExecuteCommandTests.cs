using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using ParameterPurpose = WorkflowApi.Client.Model.ParameterPurpose;

namespace WorkflowApi.Client.Rpc.Test.Tests.CommandsApi;

[TestClass]
public class ExecuteCommandTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecuteCommand(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var command = new WorkflowCommand(
            processId,
            nextCommand.Name,
            validForActivityName: initialActivity.Name
        );

        var response = await service.Client.RpcCommands.WorkflowApiRpcExecuteCommandAsync(new(command));

        // Assert

        Assert.IsNotNull(response);
        Assert.AreEqual(nextCommand.Name, response.CommandName);
        Assert.IsTrue(response.WasExecuted);
        Assert.IsNotNull(response.ProcessInstance);
        Assert.IsNull(response.Message);

        var processInstance = response.ProcessInstance;
        Assert.AreEqual(processId, processInstance.ProcessId);
        Assert.AreEqual(finalActivity.Name, processInstance.CurrentActivityName);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecuteCommand_WithParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var parameter = new CommandParameter()
        {
            Name = "Stark_Motto",
            TypeName = typeof(string).FullName,
            Value = "Winter is coming",
            Purpose = ParameterPurposeWithoutSystem.Persistence
        };

        var command = new WorkflowCommand(
            processId,
            nextCommand.Name,
            validForActivityName: initialActivity.Name,
            parameters: [parameter]
        );

        var response = await service.Client.RpcCommands.WorkflowApiRpcExecuteCommandAsync(new(command));

        // Assert

        Assert.IsNotNull(response);
        Assert.AreEqual(nextCommand.Name, response.CommandName);
        Assert.IsTrue(response.WasExecuted);
        Assert.IsNull(response.Message);

        var processInstance = response.ProcessInstance;
        Assert.IsNotNull(processInstance);
        Assert.AreEqual(processId, processInstance.ProcessId);
        Assert.AreEqual(finalActivity.Name, processInstance.CurrentActivityName);

        var parameters = processInstance.ProcessParameters;
        Assert.IsNotNull(parameters);
        Assert.AreNotEqual(0, parameters.Count);

        var actualParameter = parameters.Find(p => p.Name == parameter.Name);
        Assert.IsNotNull(actualParameter);
        Assert.AreEqual(parameter.Name, actualParameter.Name);
        Assert.AreEqual(parameter.Value, actualParameter.Value);
        Assert.AreEqual(parameter.TypeName, actualParameter.Type);
        Assert.AreEqual(ParameterPurpose.Persistence, actualParameter.Purpose);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecuteCommand_WithIdentityId(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();
        var identityId = Guid.NewGuid().ToString();

        var validRuleName = service.RuleProvider.CreateRule(
            // ReSharper disable once InconsistentNaming
            (_, _, _identityId, _) => Task.FromResult(_identityId == identityId),
            (_, _, _, _) => Task.FromResult<IEnumerable<string>>([identityId])
        );

        var invalidRuleName = service.RuleProvider.CreateRule(
            (_, _, _, _) => Task.FromResult(false),
            (_, _, _, _) => Task.FromResult<IEnumerable<string>>([])
        );

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").State("InitialState").Initial().Ref(out ActivityDefinition initialActivity)
            .CreateActivity("North").Ref(out ActivityDefinition northActivity)
            .CreateActivity("South").Ref(out ActivityDefinition southActivity)
            .CreateActivity("East").Ref(out ActivityDefinition eastActivity)
            .CreateActor("The winter is coming", validRuleName).Ref(out ActorDefinition validActor)
            .CreateActor("The winter is not coming", invalidRuleName).Ref(out ActorDefinition invalidActor)
            .CreateCommand("Go_North").Ref(out CommandDefinition northCommand)
            .CreateCommand("Go_South").Ref(out CommandDefinition southCommand)
            .CreateCommand("Go_East").Ref(out CommandDefinition eastCommand)
            .CreateTransition("Initial_North", initialActivity, northActivity)
            .Direct()
            .TriggeredByCommand(northCommand)
            .CreateRestriction(validActor, RestrictionType.Allow)
            .CreateTransition("Initial_South", initialActivity, southActivity)
            .TriggeredByCommand(southCommand)
            .CreateRestriction(invalidActor, RestrictionType.Allow)
            .CreateTransition("Initial_East", initialActivity, eastActivity)
            .TriggeredByCommand(eastCommand)
            .CreateRestriction(validActor, RestrictionType.Restrict);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var command = new WorkflowCommand(
            processId,
            northCommand.Name,
            validForActivityName: initialActivity.Name
        );

        var response = await service.Client.RpcCommands.WorkflowApiRpcExecuteCommandAsync(new(command, identityId));

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(northCommand.Name, response.CommandName);
        Assert.IsTrue(response.WasExecuted);
        Assert.IsNull(response.Message);

        var processInstance = response.ProcessInstance;
        Assert.IsNotNull(processInstance);
        Assert.AreEqual(processId, processInstance.ProcessId);
        Assert.AreEqual(northActivity.Name, processInstance.CurrentActivityName);
        Assert.AreEqual(identityId, processInstance.IdentityId);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecuteCommand_WithImpersonatedIdentityId_AndRestrictionCheck(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();
        var identityId = Guid.NewGuid().ToString();

        var validRuleName = service.RuleProvider.CreateRule(
            // ReSharper disable once InconsistentNaming
            (_, _, _identityId, _) => Task.FromResult(_identityId == identityId),
            (_, _, _, _) => Task.FromResult<IEnumerable<string>>([identityId])
        );

        var invalidRuleName = service.RuleProvider.CreateRule(
            (_, _, _, _) => Task.FromResult(false),
            (_, _, _, _) => Task.FromResult<IEnumerable<string>>([])
        );

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").State("InitialState").Initial().Ref(out ActivityDefinition initialActivity)
            .CreateActivity("North").Ref(out ActivityDefinition northActivity)
            .CreateActivity("South").Ref(out ActivityDefinition southActivity)
            .CreateActivity("East").Ref(out ActivityDefinition eastActivity)
            .CreateActor("The winter is coming", validRuleName).Ref(out ActorDefinition validActor)
            .CreateActor("The winter is not coming", invalidRuleName).Ref(out ActorDefinition invalidActor)
            .CreateCommand("Go_North").Ref(out CommandDefinition northCommand)
            .CreateCommand("Go_South").Ref(out CommandDefinition southCommand)
            .CreateCommand("Go_East").Ref(out CommandDefinition eastCommand)
            .CreateTransition("Initial_North", initialActivity, northActivity)
            .Direct()
            .TriggeredByCommand(northCommand)
            .CreateRestriction(validActor, RestrictionType.Allow)
            .CreateTransition("Initial_South", initialActivity, southActivity)
            .TriggeredByCommand(southCommand)
            .CreateRestriction(invalidActor, RestrictionType.Allow)
            .CreateTransition("Initial_East", initialActivity, eastActivity)
            .TriggeredByCommand(eastCommand)
            .CreateRestriction(validActor, RestrictionType.Restrict);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var command = new WorkflowCommand(
            processId,
            northCommand.Name,
            validForActivityName: initialActivity.Name
        );

        var response = await service.Client.RpcCommands.WorkflowApiRpcExecuteCommandAsync(new(command, impersonatedIdentityId: identityId, checkRestrictions: true));

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(northCommand.Name, response.CommandName);
        Assert.IsTrue(response.WasExecuted);
        Assert.IsNull(response.Message);

        var processInstance = response.ProcessInstance;
        Assert.IsNotNull(processInstance);
        Assert.AreEqual(processId, processInstance.ProcessId);
        Assert.AreEqual(northActivity.Name, processInstance.CurrentActivityName);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var command = new WorkflowCommand(processId, nextCommand.Name, validForActivityName: initialActivity.Name);
        var response = await service.Client.ExclusivePermissions(c => c.RpcCommands, WorkflowApiOperationId.RpcExecuteCommand).WorkflowApiRpcExecuteCommandAsync(new(command));

        // Assert

        Assert.IsNotNull(response);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcCommands, Array.Empty<string>()).WorkflowApiRpcExecuteCommandAsync(new(new(Guid.NewGuid()))));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
