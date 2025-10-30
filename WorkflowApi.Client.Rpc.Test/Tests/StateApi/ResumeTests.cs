using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

//TODO Simplify

[TestClass]
public class ResumeTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldResume(TestService service)
    {
        // Arrange
        
        var schemeCode = $"ResumeTest_{Guid.NewGuid()}";

        var builder = ProcessDefinitionBuilder.Create(schemeCode);

        builder
            .CreateActivity("Activity1").Ref(out var activity1)
            .Initial()
            .EnableSetState()
            .State("State1")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("ExecuteOnce").ActionParameter("Activity1")
            .CreateActivity("Activity2").Ref(out var activity2)
            .EnableSetState()
            .State("State2")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureParameterIsTrue").ActionParameter("Parameter1")
            .CreateImplementationAtEnd("ExecuteOnce").ActionParameter("Activity2")
            .CreateTransition("Transition1", activity1, activity2)
            .Conditional()
            .CreateCondition("EnsureParameterIsTrueOrNull").ConditionParameter("Parameter1")
            .CreateActivity("Activity3").Ref(out var activity3)
            .EnableSetState()
            .State("State3")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureNotExecuted")
            .CreateActivity("Activity4").Ref(out var activity4)
            .Final()
            .EnableSetState()
            .State("State4")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureIdentities")
            .CreateImplementationAtEnd("EnsureParameterIsTrue").ActionParameter("Parameter2")
            .CreateTransition("Transition2", activity3, activity4)
            .Conditional()
            .CreateCondition("EnsureParameterIsTrue").ConditionParameter("Parameter2");

        // ExecuteOnce
        builder.CreateCodeAction("ExecuteOnce").Code(
            """
            if (processInstance.IsParameterExisting(parameter))
                            throw new Exception("Executed more than once");

                        processInstance.SetParameter(parameter,true);
            """
        );

        // EnsureParameterIsTrue
        builder.CreateCodeAction("EnsureParameterIsTrue").Code(
            """
            if (processInstance.IsParameterExisting(parameter))
                            if (processInstance.GetParameter<bool>(parameter))
                                return;
                        throw new Exception ("Parameter is not true.");
            """
        );

        // EnsureParameterIsTrueOrNull (Condition)
        builder.CreateCodeCondition("EnsureParameterIsTrueOrNull").Code(
            """
            return !processInstance.IsParameterExisting(parameter) || processInstance.GetParameter<bool>(parameter);
            """
        );

        // EnsureIdentities
        builder.CreateCodeAction("EnsureIdentities").Code(
            """
            if (processInstance.IdentityId != "identity")
                            throw new Exception ("IdentityId not found");
                        if (processInstance.ImpersonatedIdentityId != "impersonatedIdentity")
                            throw new Exception ("ImpersonatedIdentityId not found");
            """
        );

        // EnsureNotExecuted
        builder.CreateCodeAction("EnsureNotExecuted").Code(
            """
            throw new Exception("Can't be executed");
            """
        );

        // EnsureParameterIsTrue (Condition)
        builder.CreateCodeCondition("EnsureParameterIsTrue").Code(
            """
            return processInstance.GetParameter<bool>(parameter);
            """
        );

        var processId = Guid.NewGuid();
        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        bool exceptionWasThrown = false;

        // Act
        
        try
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }
        catch (Exception e)
        {
            exceptionWasThrown = true;
            Console.WriteLine(e);
        }

        // Assert 
        
        Assert.IsTrue(exceptionWasThrown);

        var processStatus = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new(processId));
        Assert.AreEqual("Terminated", processStatus.ProcessStatus.ToString());

        var currentActivity = await service.Client.RpcStates.WorkflowApiRpcGetCurrentActivityNameAsync(new(processId));
        var currentState = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(new(processId));

        Assert.AreEqual("Activity1", currentActivity.CurrentActivityName);
        Assert.AreEqual("State1", currentState.CurrentState.Name);


        // Act Temporary parameter
        
        var resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ProcessParameters = [new("Parameter1", false, ParameterPurposeWithoutSystem.Temporary)]
        };

        var res = await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest);

        // Assert
        
        Assert.IsFalse(res.WasResumed);
        Assert.AreEqual("Activity1", res.ProcessInstance.CurrentActivityName);
        Assert.AreEqual("State1", res.ProcessInstance.CurrentState);

        processStatus = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new(processId));
        Assert.AreEqual("Idled", processStatus.ProcessStatus.ToString());


        // Act Temporary parameter
        
        resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ProcessParameters = [new("Parameter1", true, ParameterPurposeWithoutSystem.Temporary)]
        };

        res = await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest);

        // Assert
        
        Assert.IsTrue(res.WasResumed);
        Assert.AreEqual("Activity2", res.ProcessInstance.CurrentActivityName);
        Assert.AreEqual("State2", res.ProcessInstance.CurrentState);

        currentActivity = await service.Client.RpcStates.WorkflowApiRpcGetCurrentActivityNameAsync(new(processId));
        currentState = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(new(processId));

        Assert.AreEqual("Activity2", currentActivity.CurrentActivityName);
        Assert.AreEqual("State2", currentState.CurrentState.Name);


        // Act Persistence parameter
        
        resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ActivityName = "Activity3",
            ProcessParameters = [new("Parameter2", false, ParameterPurposeWithoutSystem.Persistence)]
        };

        res = await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest);

        // Assert
        
        Assert.IsFalse(res.WasResumed);
        Assert.AreEqual("Activity2", res.ProcessInstance.CurrentActivityName);
        Assert.AreEqual("State2", res.ProcessInstance.CurrentState);


        // Act Persistence parameter
        
        resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ActivityName = "Activity3",
            IdentityId = "identity",
            ImpersonatedIdentityId = "impersonatedIdentity",
            ProcessParameters = [new("Parameter2", true, ParameterPurposeWithoutSystem.Persistence)]
        };

        res = await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest);

        // Assert
        
        Assert.IsTrue(res.WasResumed);
        Assert.AreEqual("Activity4", res.ProcessInstance.CurrentActivityName);
        Assert.AreEqual("State4", res.ProcessInstance.CurrentState);
        Assert.AreEqual("identity", res.ProcessInstance.IdentityId);
        Assert.AreEqual("impersonatedIdentity", res.ProcessInstance.ImpersonatedIdentityId);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldResumeWithExplicitParameters(TestService service)
    {
        // Arrange

        var schemeCode = $"ResumeTest_{Guid.NewGuid()}";

        var builder = ProcessDefinitionBuilder.Create(schemeCode);

        builder
            .CreateActivity("Activity1").Ref(out var activity1)
            .Initial()
            .EnableSetState()
            .State("State1")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("ExecuteOnce").ActionParameter("Activity1")
            .CreateActivity("Activity2").Ref(out var activity2)
            .EnableSetState()
            .State("State2")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureParameterIsTrue").ActionParameter("Parameter1")
            .CreateImplementationAtEnd("ExecuteOnce").ActionParameter("Activity2")
            .CreateTransition("Transition1", activity1, activity2)
            .Conditional()
            .CreateCondition("EnsureParameterIsTrueOrNull").ConditionParameter("Parameter1")
            .CreateActivity("Activity3").Ref(out var activity3)
            .EnableSetState()
            .State("State3")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureNotExecuted")
            .CreateActivity("Activity4").Ref(out var activity4)
            .Final()
            .EnableSetState()
            .State("State4")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureIdentities")
            .CreateImplementationAtEnd("EnsureParameterIsTrue").ActionParameter("Parameter2")
            .CreateTransition("Transition2", activity3, activity4)
            .Conditional()
            .CreateCondition("EnsureParameterIsTrue").ConditionParameter("Parameter2")
            .CreateParameter("Parameter1", typeof(bool))
            .CreateParameter("Parameter2", typeof(bool), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence);

        // ExecuteOnce
        builder.CreateCodeAction("ExecuteOnce").Code(
            """
            if (processInstance.IsParameterExisting(parameter))
                            throw new Exception("Executed more than once");

                        processInstance.SetParameter(parameter,true);
            """
        );

        // EnsureParameterIsTrue
        builder.CreateCodeAction("EnsureParameterIsTrue").Code(
            """
            if (processInstance.IsParameterExisting(parameter))
                            if (processInstance.GetParameter<bool>(parameter))
                                return;
                        throw new Exception ("Parameter is not true.");
            """
        );

        // EnsureParameterIsTrueOrNull (Condition)
        builder.CreateCodeCondition("EnsureParameterIsTrueOrNull").Code(
            """
            return !processInstance.IsParameterExisting(parameter) || processInstance.GetParameter<bool>(parameter);
            """
        );

        // EnsureIdentities
        builder.CreateCodeAction("EnsureIdentities").Code(
            """
            if (processInstance.IdentityId != "identity")
                            throw new Exception ("IdentityId not found");
                        if (processInstance.ImpersonatedIdentityId != "impersonatedIdentity")
                            throw new Exception ("ImpersonatedIdentityId not found");
            """
        );

        // EnsureNotExecuted
        builder.CreateCodeAction("EnsureNotExecuted").Code(
            """
            throw new Exception("Can't be executed");
            """
        );

        // EnsureParameterIsTrue (Condition)
        builder.CreateCodeCondition("EnsureParameterIsTrue").Code(
            """
            return processInstance.GetParameter<bool>(parameter);
            """
        );

        var processId = Guid.NewGuid();
        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        bool exceptionWasThrown = false;
        
        // Act

        try
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }
        catch (Exception)
        {
            exceptionWasThrown = true;
        }

        // Assert 

        Assert.IsTrue(exceptionWasThrown);

        var processStatus = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new(processId));
        Assert.AreEqual("Terminated", processStatus.ProcessStatus.ToString());

        var currentActivity = await service.Client.RpcStates.WorkflowApiRpcGetCurrentActivityNameAsync(new(processId));
        var currentState = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(new(processId));

        Assert.AreEqual("Activity1", currentActivity.CurrentActivityName);
        Assert.AreEqual("State1", currentState.CurrentState.Name);
        
        // Act Temporary parameter

        var resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ProcessParameters = [new("Parameter1", false, ParameterPurposeWithoutSystem.Temporary)]
        };

        var res = await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest);

        // Assert

        Assert.IsFalse(res.WasResumed);
        Assert.AreEqual("Activity1", res.ProcessInstance.CurrentActivityName);
        Assert.AreEqual("State1", res.ProcessInstance.CurrentState);

        processStatus = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new(processId));
        Assert.AreEqual("Idled", processStatus.ProcessStatus.ToString());
        
        // Act Temporary parameter

        resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ProcessParameters = [new("Parameter1", true, ParameterPurposeWithoutSystem.Temporary)]
        };

        res = await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest);

        // Assert

        Assert.IsTrue(res.WasResumed);
        Assert.AreEqual("Activity2", res.ProcessInstance.CurrentActivityName);
        Assert.AreEqual("State2", res.ProcessInstance.CurrentState);

        currentActivity = await service.Client.RpcStates.WorkflowApiRpcGetCurrentActivityNameAsync(new(processId));
        currentState = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(new(processId));

        Assert.AreEqual("Activity2", currentActivity.CurrentActivityName);
        Assert.AreEqual("State2", currentState.CurrentState.Name);


        // Act Persistence parameter

        resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ActivityName = "Activity3",
            ProcessParameters = [new("Parameter2", false, ParameterPurposeWithoutSystem.Persistence)]
        };

        res = await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest);

        // Assert

        Assert.IsFalse(res.WasResumed);
        Assert.AreEqual("Activity2", res.ProcessInstance.CurrentActivityName);
        Assert.AreEqual("State2", res.ProcessInstance.CurrentState);


        // Act Persistence parameter

        resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ActivityName = "Activity3",
            IdentityId = "identity",
            ImpersonatedIdentityId = "impersonatedIdentity",
            ProcessParameters = [new("Parameter2", true, ParameterPurposeWithoutSystem.Persistence)]
        };

        res = await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest);

        // Assert

        Assert.IsTrue(res.WasResumed);
        Assert.AreEqual("Activity4", res.ProcessInstance.CurrentActivityName);
        Assert.AreEqual("State4", res.ProcessInstance.CurrentState);
        Assert.AreEqual("identity", res.ProcessInstance.IdentityId);
        Assert.AreEqual("impersonatedIdentity", res.ProcessInstance.ImpersonatedIdentityId);

        var parameter1 = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, "Parameter2");
        Assert.IsNotNull(parameter1);
        Assert.AreEqual("Parameter2", parameter1.Name);
        Assert.AreEqual(true, parameter1.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldThrowError_WhenInvalidTemporaryParameterValue(TestService service)
    {
        // Arrange

        var schemeCode = $"ResumeTest_{Guid.NewGuid()}";

        var builder = ProcessDefinitionBuilder.Create(schemeCode);

        builder
            .CreateActivity("Activity1").Ref(out var activity1)
            .Initial()
            .EnableSetState()
            .State("State1")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("ExecuteOnce").ActionParameter("Activity1")
            .CreateActivity("Activity2").Ref(out var activity2)
            .EnableSetState()
            .State("State2")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureParameterIsTrue").ActionParameter("Parameter1")
            .CreateImplementationAtEnd("ExecuteOnce").ActionParameter("Activity2")
            .CreateTransition("Transition1", activity1, activity2)
            .Conditional()
            .CreateCondition("EnsureParameterIsTrueOrNull").ConditionParameter("Parameter1")
            .CreateActivity("Activity3").Ref(out var activity3)
            .EnableSetState()
            .State("State3")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureNotExecuted")
            .CreateActivity("Activity4").Ref(out var activity4)
            .Final()
            .EnableSetState()
            .State("State4")
            .EnableAutoSchemeUpdate()
            .CreateImplementationAtEnd("EnsureIdentities")
            .CreateImplementationAtEnd("EnsureParameterIsTrue").ActionParameter("Parameter2")
            .CreateTransition("Transition2", activity3, activity4)
            .Conditional()
            .CreateCondition("EnsureParameterIsTrue").ConditionParameter("Parameter2")
            .CreateParameter("Parameter1", typeof(bool))
            .CreateParameter("Parameter2", typeof(bool), OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence);

        // ExecuteOnce
        builder.CreateCodeAction("ExecuteOnce").Code(
            """
            if (processInstance.IsParameterExisting(parameter))
                            throw new Exception("Executed more than once");

                        processInstance.SetParameter(parameter,true);
            """
        );

        // EnsureParameterIsTrue
        builder.CreateCodeAction("EnsureParameterIsTrue").Code(
            """
            if (processInstance.IsParameterExisting(parameter))
                            if (processInstance.GetParameter<bool>(parameter))
                                return;
                        throw new Exception ("Parameter is not true.");
            """
        );

        // EnsureParameterIsTrueOrNull (Condition)
        builder.CreateCodeCondition("EnsureParameterIsTrueOrNull").Code(
            """
            return !processInstance.IsParameterExisting(parameter) || processInstance.GetParameter<bool>(parameter);
            """
        );

        // EnsureIdentities
        builder.CreateCodeAction("EnsureIdentities").Code(
            """
            if (processInstance.IdentityId != "identity")
                            throw new Exception ("IdentityId not found");
                        if (processInstance.ImpersonatedIdentityId != "impersonatedIdentity")
                            throw new Exception ("ImpersonatedIdentityId not found");
            """
        );

        // EnsureNotExecuted
        builder.CreateCodeAction("EnsureNotExecuted").Code(
            """
            throw new Exception("Can't be executed");
            """
        );

        // EnsureParameterIsTrue (Condition)
        builder.CreateCodeCondition("EnsureParameterIsTrue").Code(
            """
            return processInstance.GetParameter<bool>(parameter);
            """
        );

        var processId = Guid.NewGuid();
        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        bool exceptionWasThrown = false;
        
        // Act

        try
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        }
        catch (Exception)
        {
            exceptionWasThrown = true;
        }

        // Assert 

        Assert.IsTrue(exceptionWasThrown);

        var processStatus = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new(processId));
        Assert.AreEqual("Terminated", processStatus.ProcessStatus.ToString());

        var currentActivity = await service.Client.RpcStates.WorkflowApiRpcGetCurrentActivityNameAsync(new(processId));
        var currentState = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(new(processId));

        Assert.AreEqual("Activity1", currentActivity.CurrentActivityName);
        Assert.AreEqual("State1", currentState.CurrentState.Name);


        // Act Temporary parameter

        var resumeRequest = new ResumeRequest()
        {
            ProcessId = processId,
            ProcessParameters = [new("Parameter1", "InvalidValue", ParameterPurposeWithoutSystem.Temporary)]
        };

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.RpcStates.WorkflowApiRpcResumeAsync(resumeRequest));

        StringAssert.Contains(exception.Message, $"Unable to deserialize process parameter 'Parameter1' to type '{typeof(bool).FullName}'");
        Assert.AreEqual(exception.ErrorCode, 500);
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
            .CreateActivity("Activity1").Initial()
            .CreateActivity(activityName)
            .State(activityName)
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();

        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcStates, WorkflowApiOperationId.RpcResume).WorkflowApiRpcResumeAsync(new(processId));

        // Assert
        
        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new IsProcessExistsRequest(Guid.NewGuid());

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcIsProcessExistsAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}