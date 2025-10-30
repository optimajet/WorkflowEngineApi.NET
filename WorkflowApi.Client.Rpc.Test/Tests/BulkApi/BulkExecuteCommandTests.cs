using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using BulkTaskState = WorkflowApi.Client.Model.BulkTaskState;
using ParameterPurpose = WorkflowApi.Client.Model.ParameterPurpose;

namespace WorkflowApi.Client.Rpc.Test.Tests.BulkApi;

[TestClass]
public class BulkExecuteCommandTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecuteCommands(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        foreach (var processId in processIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }

        var commands = processIds.Select(processId => new WorkflowCommand(
            processId,
            nextCommand.Name,
            validForActivityName: initialActivity.Name
        )).ToList();

        var request = new BulkExecuteCommandRequest(commands);

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkExecuteCommandAsync(request);

        // Assert

        foreach (var command in request.Commands)
        {
            var result = response.FirstOrDefault(pair => pair.Key == command.ProcessId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var executionResult = result.Result;

            Assert.IsNotNull(executionResult);
            Assert.AreEqual(nextCommand.Name, executionResult.CommandName);
            Assert.IsTrue(executionResult.WasExecuted);
            Assert.IsNotNull(executionResult.ProcessInstance);
            Assert.IsNull(executionResult.Message);

            var processInstance = executionResult.ProcessInstance;
            Assert.AreEqual(command.ProcessId, processInstance.ProcessId);
            Assert.AreEqual(finalActivity.Name, processInstance.CurrentActivityName);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecuteCommands_WithParameter(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        foreach (var processId in processIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }

        var commands = processIds.Select(processId => new WorkflowCommand(
            processId,
            nextCommand.Name,
            validForActivityName: initialActivity.Name,
            parameters: [new CommandParameter()
            {
                Name = "Stark_Motto",
                TypeName = typeof(string).FullName,
                Value = "Winter is coming",
                Purpose = ParameterPurposeWithoutSystem.Persistence
            }]
        )).ToList();

        var request = new BulkExecuteCommandRequest(commands);

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkExecuteCommandAsync(request);

        // Assert

        foreach (var command in request.Commands)
        {
            var result = response.FirstOrDefault(pair => pair.Key == command.ProcessId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var executionResult = result.Result;

            Assert.IsNotNull(executionResult);
            Assert.AreEqual(nextCommand.Name, executionResult.CommandName);
            Assert.IsTrue(executionResult.WasExecuted);
            Assert.IsNull(executionResult.Message);

            var processInstance = executionResult.ProcessInstance;
            Assert.IsNotNull(processInstance);
            Assert.AreEqual(command.ProcessId, processInstance.ProcessId);
            Assert.AreEqual(finalActivity.Name, processInstance.CurrentActivityName);

            var parameters = processInstance.ProcessParameters;
            Assert.IsNotNull(parameters);
            Assert.AreNotEqual(0, parameters.Count);

            var expectedParameter = command.Parameters.Single();
            var actualParameter = parameters.Find(p => p.Name == expectedParameter.Name);
            Assert.IsNotNull(actualParameter);
            Assert.AreEqual(expectedParameter.Name, actualParameter.Name);
            Assert.AreEqual(expectedParameter.Value, actualParameter.Value);
            Assert.AreEqual(expectedParameter.TypeName, actualParameter.Type);
            Assert.AreEqual(ParameterPurpose.Persistence, actualParameter.Purpose);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecuteCommands_WithIdentityId(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
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

        foreach (var processId in processIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }

        var commands = processIds.Select(processId => new WorkflowCommand(
            processId,
            northCommand.Name,
            validForActivityName: initialActivity.Name
        )).ToList();

        var request = new BulkExecuteCommandRequest(commands, identityId);

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkExecuteCommandAsync(request);

        // Assert

        foreach (var command in request.Commands)
        {
            var result = response.FirstOrDefault(pair => pair.Key == command.ProcessId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var executionResult = result.Result;

            Assert.IsNotNull(executionResult);
            Assert.AreEqual(northCommand.Name, executionResult.CommandName);
            Assert.IsTrue(executionResult.WasExecuted);
            Assert.IsNull(executionResult.Message);

            var processInstance = executionResult.ProcessInstance;
            Assert.IsNotNull(processInstance);
            Assert.AreEqual(command.ProcessId, processInstance.ProcessId);
            Assert.AreEqual(northActivity.Name, processInstance.CurrentActivityName);
            Assert.AreEqual(identityId, processInstance.IdentityId);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecuteCommands_WithImpersonatedIdentityId_AndRestrictionCheck(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
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

        foreach (var processId in processIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }

        var commands = processIds.Select(processId => new WorkflowCommand(
            processId,
            northCommand.Name,
            validForActivityName: initialActivity.Name
        )).ToList();

        var request = new BulkExecuteCommandRequest(commands, impersonatedIdentityId: identityId, checkRestrictions: true);

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkExecuteCommandAsync(request);

        // Assert

        foreach (var command in request.Commands)
        {
            var result = response.FirstOrDefault(pair => pair.Key == command.ProcessId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var executionResult = result.Result;

            Assert.IsNotNull(executionResult);
            Assert.AreEqual(northCommand.Name, executionResult.CommandName);
            Assert.IsTrue(executionResult.WasExecuted);
            Assert.IsNull(executionResult.Message);

            var processInstance = executionResult.ProcessInstance;
            Assert.IsNotNull(processInstance);
            Assert.AreEqual(command.ProcessId, processInstance.ProcessId);
            Assert.AreEqual(northActivity.Name, processInstance.CurrentActivityName);
        }
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

        var command = new WorkflowCommand(processId, nextCommand.Name, validForActivityName: initialActivity.Name);
        var request = new BulkExecuteCommandRequest([command]);

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcBulk, WorkflowApiOperationId.RpcBulkExecuteCommand).WorkflowApiRpcBulkExecuteCommandAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new BulkExecuteCommandRequest([]);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcBulk, Array.Empty<string>()).WorkflowApiRpcBulkExecuteCommandAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
