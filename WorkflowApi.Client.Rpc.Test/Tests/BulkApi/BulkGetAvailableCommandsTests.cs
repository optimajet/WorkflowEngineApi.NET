﻿using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using BulkTaskState = WorkflowApi.Client.Model.BulkTaskState;
using ParameterPurpose = OptimaJet.Workflow.Core.Model.ParameterPurpose;
using TransitionClassifier = WorkflowApi.Client.Model.TransitionClassifier;

namespace WorkflowApi.Client.Rpc.Test.Tests.BulkApi;

[TestClass]
public class BulkGetAvailableCommandsTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var schemeCode = Guid.NewGuid().ToString();
        var identityId = Guid.NewGuid().ToString();

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

        var request = new BulkGetAvailableCommandsRequest(processIds, [identityId]);

        // Act
        
        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkGetAvailableCommandsAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var availableCommands = result.Result;

            Assert.IsNotNull(availableCommands);
            Assert.AreEqual(1, availableCommands.Count);

            var actualCommand = availableCommands[0];
            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(processId, actualCommand.ProcessId);
            Assert.AreEqual(nextCommand.Name, actualCommand.Name);
            Assert.AreEqual(nextCommand.Name, actualCommand.LocalizedName);
            Assert.AreEqual(0, actualCommand.Parameters.Count);
            Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
            Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
            Assert.AreEqual(false, actualCommand.IsForSubprocess);
            Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
            Assert.AreEqual(1, actualCommand.Identities.Count);
            Assert.AreEqual(identityId, actualCommand.Identities[0]);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands_WithParameters(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var schemeCode = Guid.NewGuid().ToString();
        var identityId = Guid.NewGuid().ToString();
        var defaultParameterValue = "Winter is coming";

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateParameter("Stark_Motto", typeof(string), ParameterPurpose.Persistence).Ref(out var startMottoParameter)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateCommandParameter("Stark_Motto", startMottoParameter).Required().DefaultValue(defaultParameterValue)
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        foreach (var processId in processIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }

        var request = new BulkGetAvailableCommandsRequest(processIds, [identityId]);

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkGetAvailableCommandsAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var availableCommands = result.Result;

            Assert.IsNotNull(availableCommands);
            Assert.AreEqual(1, availableCommands.Count);

            var actualCommand = availableCommands[0];
            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(processId, actualCommand.ProcessId);
            Assert.AreEqual(nextCommand.Name, actualCommand.Name);
            Assert.AreEqual(nextCommand.Name, actualCommand.LocalizedName);
            Assert.AreEqual(1, actualCommand.Parameters.Count);
            Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
            Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
            Assert.AreEqual(false, actualCommand.IsForSubprocess);
            Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
            Assert.AreEqual(1, actualCommand.Identities.Count);
            Assert.AreEqual(identityId, actualCommand.Identities[0]);

            var parameter = actualCommand.Parameters[0];
            Assert.IsNotNull(parameter);
            Assert.AreEqual(startMottoParameter.Name, parameter.Name);
            Assert.AreEqual(startMottoParameter.Type.AssemblyQualifiedName, parameter.TypeName);
            Assert.AreEqual(ParameterPurposeWithoutSystem.Persistence, parameter.Purpose);
            Assert.IsTrue(parameter.IsRequired);
            Assert.AreEqual(defaultParameterValue, parameter.DefaultValue);
            Assert.IsNull(parameter.Value);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands_WithRestrictions(TestService service)
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

        var request = new BulkGetAvailableCommandsRequest(processIds, [identityId]);

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkGetAvailableCommandsAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var availableCommands = result.Result;
            Assert.IsNotNull(availableCommands);
            Assert.AreEqual(1, availableCommands.Count);

            var actualCommand = availableCommands[0];
            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(processId, actualCommand.ProcessId);
            Assert.AreEqual(northCommand.Name, actualCommand.Name);
            Assert.AreEqual(northCommand.Name, actualCommand.LocalizedName);
            Assert.AreEqual(0, actualCommand.Parameters.Count);
            Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
            Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
            Assert.AreEqual(false, actualCommand.IsForSubprocess);
            Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
            Assert.AreEqual(1, actualCommand.Identities.Count);
            Assert.AreEqual(identityId, actualCommand.Identities[0]);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands_WithCommandNameFilter(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var schemeCode = Guid.NewGuid().ToString();
        var identityId = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").State("InitialState").Initial().Ref(out ActivityDefinition initialActivity)
            .CreateActivity("North").Ref(out ActivityDefinition northActivity)
            .CreateActivity("South").Ref(out ActivityDefinition southActivity)
            .CreateActivity("East").Ref(out ActivityDefinition eastActivity)
            .CreateCommand("Go_North").Ref(out CommandDefinition northCommand)
            .CreateCommand("Go_South").Ref(out CommandDefinition southCommand)
            .CreateCommand("Go_East").Ref(out CommandDefinition eastCommand)
            .CreateTransition("Initial_North", initialActivity, northActivity)
            .Direct()
            .TriggeredByCommand(northCommand)
            .CreateTransition("Initial_South", initialActivity, southActivity)
            .Direct()
            .TriggeredByCommand(southCommand)
            .CreateTransition("Initial_East", initialActivity, eastActivity)
            .TriggeredByCommand(eastCommand)
            .Direct();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        foreach (var processId in processIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }

        var request = new BulkGetAvailableCommandsRequest(processIds, [identityId], commandNameFilter: northCommand.Name);

        // Act
        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkGetAvailableCommandsAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var availableCommands = result.Result;
            Assert.IsNotNull(availableCommands);
            Assert.AreEqual(1, availableCommands.Count);

            var actualCommand = availableCommands[0];
            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(processId, actualCommand.ProcessId);
            Assert.AreEqual(northCommand.Name, actualCommand.Name);
            Assert.AreEqual(northCommand.Name, actualCommand.LocalizedName);
            Assert.AreEqual(0, actualCommand.Parameters.Count);
            Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
            Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
            Assert.AreEqual(false, actualCommand.IsForSubprocess);
            Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
            Assert.AreEqual(1, actualCommand.Identities.Count);
            Assert.AreEqual(identityId, actualCommand.Identities[0]);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands_WithCulture(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var schemeCode = Guid.NewGuid().ToString();
        var identityId = Guid.NewGuid().ToString();
        var germanCulture = new CultureInfo("de-DE");
        var frenchCulture = new CultureInfo("fr-FR");
        var germanCommandName = "Nächster Schritt";


        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateOrUpdateLocalization(germanCulture, germanCommandName)
            .CreateOrUpdateLocalization(frenchCulture, "Prochaine étape")
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        foreach (var processId in processIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }

        var request = new BulkGetAvailableCommandsRequest(processIds, [identityId], culture: germanCulture.Name);

        // Act
        
        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkGetAvailableCommandsAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var availableCommands = result.Result;
            Assert.IsNotNull(availableCommands);
            Assert.AreEqual(1, availableCommands.Count);

            var actualCommand = availableCommands[0];
            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(processId, actualCommand.ProcessId);
            Assert.AreEqual(nextCommand.Name, actualCommand.Name);
            Assert.AreEqual(germanCommandName, actualCommand.LocalizedName);
            Assert.AreEqual(0, actualCommand.Parameters.Count);
            Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
            Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
            Assert.AreEqual(false, actualCommand.IsForSubprocess);
            Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
            Assert.AreEqual(1, actualCommand.Identities.Count);
            Assert.AreEqual(identityId, actualCommand.Identities[0]);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands_WithConditionCheck(TestService service)
    {
        // Arrange

        List<Guid> processIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var schemeCode = Guid.NewGuid().ToString();
        var identityId = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").State("InitialState").Initial().Ref(out ActivityDefinition initialActivity)
            .CreateActivity("North").Ref(out ActivityDefinition northActivity)
            .CreateActivity("South").Ref(out ActivityDefinition southActivity)
            .CreateActivity("East").Ref(out ActivityDefinition eastActivity)
            .CreateCommand("Go_North").Ref(out CommandDefinition northCommand)
            .CreateCommand("Go_South").Ref(out CommandDefinition southCommand)
            .CreateCommand("Go_East").Ref(out CommandDefinition eastCommand)
            .CreateTransition("Initial_North", initialActivity, northActivity)
            .Direct()
            .TriggeredByCommand(northCommand)
            .Conditional().CreateExpression("true")
            .CreateTransition("Initial_South", initialActivity, southActivity)
            .Direct()
            .TriggeredByCommand(southCommand)
            .Conditional().CreateExpression("false")
            .CreateTransition("Initial_East", initialActivity, eastActivity)
            .Direct()
            .TriggeredByCommand(eastCommand)
            .Conditional().CreateExpression("false");

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        foreach (var processId in processIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }

        var request = new BulkGetAvailableCommandsRequest(processIds, [identityId], conditionCheck: true);

        // Act
        
        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkGetAvailableCommandsAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var availableCommands = result.Result;
            Assert.IsNotNull(availableCommands);
            Assert.AreEqual(1, availableCommands.Count);

            var actualCommand = availableCommands[0];
            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(processId, actualCommand.ProcessId);
            Assert.AreEqual(northCommand.Name, actualCommand.Name);
            Assert.AreEqual(northCommand.Name, actualCommand.LocalizedName);
            Assert.AreEqual(0, actualCommand.Parameters.Count);
            Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
            Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
            Assert.AreEqual(false, actualCommand.IsForSubprocess);
            Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
            Assert.AreEqual(1, actualCommand.Identities.Count);
            Assert.AreEqual(identityId, actualCommand.Identities[0]);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();

        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));

        var request = new BulkGetAvailableCommandsRequest([processId]);

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcBulk, OperationId.RpcBulkGetAvailableCommands).WorkflowApiRpcBulkGetAvailableCommandsAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new BulkGetAvailableCommandsRequest([]);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcBulk, Array.Empty<string>()).WorkflowApiRpcBulkGetAvailableCommandsAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
