using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using TransitionClassifier = WorkflowApi.Client.Model.TransitionClassifier;

namespace WorkflowApi.Client.Rpc.Test.Tests.CommandsApi;

[TestClass]
public class GetInitialCommandsTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        // Act

        var request = new GetInitialCommandsRequest(schemeCode);
        var response = await service.Client.RpcCommands.WorkflowApiRpcGetInitialCommandsAsync(request);

        // Assert

        var availableCommands = response.InitialCommands;
        Assert.IsNotNull(availableCommands);
        Assert.AreEqual(1, availableCommands.Count);

        var actualCommand = availableCommands[0];
        Assert.IsNotNull(actualCommand);
        Assert.AreEqual(nextCommand.Name, actualCommand.Name);
        Assert.AreEqual(nextCommand.Name, actualCommand.LocalizedName);
        Assert.AreEqual(0, actualCommand.Parameters.Count);
        Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
        Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
        Assert.AreEqual(false, actualCommand.IsForSubprocess);
        Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
        Assert.AreEqual(0, actualCommand.Identities.Count);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands_WithCommandNameFilter(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

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

        // Act

        var request = new GetInitialCommandsRequest(schemeCode, commandNameFilter: northCommand.Name);
        var response = await service.Client.RpcCommands.WorkflowApiRpcGetInitialCommandsAsync(request);

        // Assert

        var availableCommands = response.InitialCommands;
        Assert.IsNotNull(availableCommands);
        Assert.AreEqual(1, availableCommands.Count);

        var actualCommand = availableCommands[0];
        Assert.IsNotNull(actualCommand);
        Assert.AreEqual(northCommand.Name, actualCommand.Name);
        Assert.AreEqual(northCommand.Name, actualCommand.LocalizedName);
        Assert.AreEqual(0, actualCommand.Parameters.Count);
        Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
        Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
        Assert.AreEqual(false, actualCommand.IsForSubprocess);
        Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
        Assert.AreEqual(0, actualCommand.Identities.Count);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldFindCommands_WithCulture(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
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

        // Act

        var request = new GetInitialCommandsRequest(schemeCode, culture: germanCulture.Name);
        var response = await service.Client.RpcCommands.WorkflowApiRpcGetInitialCommandsAsync(request);

        // Assert

        var availableCommands = response.InitialCommands;
        Assert.IsNotNull(availableCommands);
        Assert.AreEqual(1, availableCommands.Count);

        var actualCommand = availableCommands[0];
        Assert.IsNotNull(actualCommand);
        Assert.AreEqual(nextCommand.Name, actualCommand.Name);
        Assert.AreEqual(germanCommandName, actualCommand.LocalizedName);
        Assert.AreEqual(0, actualCommand.Parameters.Count);
        Assert.AreEqual(initialActivity.Name, actualCommand.ValidForActivityName);
        Assert.AreEqual(initialActivity.State, actualCommand.ValidForStateName);
        Assert.AreEqual(false, actualCommand.IsForSubprocess);
        Assert.AreEqual(TransitionClassifier.Direct, actualCommand.Classifier);
        Assert.AreEqual(0, actualCommand.Identities.Count);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange
        
        var schemeCode = Guid.NewGuid().ToString();
        
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity")
            .Initial();
        
        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcCommands, OperationId.RpcGetInitialCommands).WorkflowApiRpcGetInitialCommandsAsync(new(schemeCode));

        // Assert

        Assert.IsNotNull(response);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcCommands, Array.Empty<string>()).WorkflowApiRpcGetInitialCommandsAsync(new(Guid.NewGuid().ToString())));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
