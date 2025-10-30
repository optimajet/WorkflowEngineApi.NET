using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.PreExecutionAPI;

[TestClass]
public class PreExecuteFromCurrentActivityTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetPreExecutionActivitiesSequence_FromInitialActivity(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Final().Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity).Ref(out TransitionDefinition transition)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var response = await service.Client.RpcPreExecution.WorkflowApiRpcPreExecuteFromCurrentActivityAsync(new(processId));

        // Assert

        Assert.IsNotNull(response);

        var activities = response.ActivitiesSequence;
        Assert.IsNotNull(activities);
        Assert.AreEqual(2, activities.Count);

        var activity1 = activities[0];
        Assert.IsNotNull(activity1);
        Assert.AreEqual(initialActivity.Name, activity1.Name);
        Assert.AreEqual(initialActivity.State, activity1.State);
        Assert.IsTrue(activity1.IsCurrent);
        Assert.IsFalse(activity1.IsFinal);
        Assert.IsTrue(activity1.IsInitial);
        Assert.AreEqual(0, activity1.SubprocessLevel);
        Assert.AreEqual(initialActivity.State, activity1.LocalizedState);

        var transitions1 = activity1.Transitions;
        Assert.IsNotNull(transitions1);
        Assert.AreEqual(1, transitions1.Count);

        var transition1 = transitions1[0];
        Assert.IsNotNull(transition1);
        Assert.AreEqual(nextCommand.Name, transition1.CommandName);
        Assert.AreEqual(finalActivity.Name, transition1.NextActivityName);
        Assert.AreEqual(transition.Trigger.Type.ToString(), transition1.TriggerType);
        Assert.AreEqual(TransitionClassifier.Direct.ToString(), transition1.Classifier.ToString());
        Assert.AreEqual(nextCommand.Name, transition1.LocalizedCommandName);

        var activity2 = activities[1];
        Assert.IsNotNull(activity2);
        Assert.AreEqual(finalActivity.Name, activity2.Name);
        Assert.AreEqual(finalActivity.State, activity2.State);
        Assert.IsFalse(activity2.IsCurrent);
        Assert.IsTrue(activity2.IsFinal);
        Assert.IsFalse(activity2.IsInitial);
        Assert.AreEqual(0, activity2.SubprocessLevel);
        Assert.AreEqual(String.Empty, activity2.LocalizedState);

        var transitions2 = activity2.Transitions;
        Assert.IsNotNull(transitions2);
        Assert.AreEqual(0, transitions2.Count);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetPreExecutionActivitiesSequence_FromFinalActivity(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Final().Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity).Ref(out TransitionDefinition transition)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        await service.Client.RpcCommands.WorkflowApiRpcExecuteCommandAsync(new(new(processId, nextCommand.Name, validForActivityName: initialActivity.Name), ""));

        // Act

        var response = await service.Client.RpcPreExecution.WorkflowApiRpcPreExecuteFromCurrentActivityAsync(new(processId));

        // Assert

        Assert.IsNotNull(response);

        var activities = response.ActivitiesSequence;
        Assert.IsNotNull(activities);
        Assert.AreEqual(1, activities.Count);

        var activity1 = activities.Single();
        Assert.IsNotNull(activity1);
        Assert.AreEqual(finalActivity.Name, activity1.Name);
        Assert.AreEqual(finalActivity.State, activity1.State);
        Assert.IsTrue(activity1.IsCurrent);
        Assert.IsTrue(activity1.IsFinal);
        Assert.IsFalse(activity1.IsInitial);
        Assert.AreEqual(0, activity1.SubprocessLevel);
        Assert.AreEqual(String.Empty, activity1.LocalizedState);

        var transitions1 = activity1.Transitions;
        Assert.IsNotNull(transitions1);
        Assert.AreEqual(0, transitions1.Count);
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
            .CreateActivity("Final").Final().Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity).Ref(out TransitionDefinition transition)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcPreExecution, [WorkflowApiOperationId.RpcPreExecuteFromCurrentActivity]).WorkflowApiRpcPreExecuteFromCurrentActivityAsync(new(processId));

        // Assert

        Assert.IsNotNull(response);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange


        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcPreExecution, Array.Empty<string>()).WorkflowApiRpcPreExecuteFromCurrentActivityAsync(new ())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
