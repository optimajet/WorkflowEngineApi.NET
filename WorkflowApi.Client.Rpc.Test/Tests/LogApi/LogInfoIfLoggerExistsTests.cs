﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Rpc.Test.Models;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Models;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.LogApi;

[TestClass]
public class LogInfoIfLoggerExistsTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldLogInfoIfLoggerExistsMessageWithParameters(TestService service)
    {
        // Arrange

        var logger = new LoggerMock();
        service.WorkflowRuntime.Logger = logger;

        var message = "Test message";

        var logParameters = new Dictionary<string, object>()
        {
            {"Key1", "Value1"},
            {"Key2", new Dictionary<string, object>
                {
                    {"NestedKey1", "NestedValue1"},
                    {"NestedKey2", "NestedValue2"}
                }
            }
        };

        // Act

        await service.Client.RpcLog.WorkflowApiRpcLogInfoIfLoggerExistsAsync(new(message, logParameters));

        // Assert

        Assert.AreEqual(1, logger.Calls.Count);
        Assert.AreEqual(nameof(LoggerMock.Info), logger.Calls[0].MethodName);
        Assert.AreEqual(message + " {Parameters}", logger.Calls[0].MessageTemplate);

        var normalizedParameters = logParameters.ToDictionary(
            kvp => kvp.Key,
            kvp => Converter.ToJsonString(kvp.Value)
        );

        Assert.AreEqual(1, logger.Calls[0].PropertyValues?.Length);
        Assert.AreEqual(Converter.ToJsonString(normalizedParameters), Converter.ToJsonString(logger.Calls[0].PropertyValues?.First()));
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldLogInfoIfLoggerExistsMessage(TestService service)
    {
        // Arrange

        var logger = new LoggerMock();
        service.WorkflowRuntime.Logger = logger;

        var message = "Test message";

        // Act

        await service.Client.RpcLog.WorkflowApiRpcLogInfoIfLoggerExistsAsync(new(message));

        // Assert

        Assert.AreEqual(1, logger.Calls.Count);
        Assert.AreEqual(nameof(LoggerMock.Info), logger.Calls[0].MethodName);
        Assert.AreEqual(message, logger.Calls[0].MessageTemplate);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var logger = new LoggerMock();
        service.WorkflowRuntime.Logger = logger;

        // Act

        await service.Client.ExclusivePermissions(c => c.RpcLog, OperationId.RpcLogInfoIfLoggerExists).WorkflowApiRpcLogInfoIfLoggerExistsAsync(new("Test message"));
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcLog, Array.Empty<string>()).WorkflowApiRpcLogInfoIfLoggerExistsAsync(new ("Test message")));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
