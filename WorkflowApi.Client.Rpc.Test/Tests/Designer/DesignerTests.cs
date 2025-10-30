using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.Designer;

[TestClass]
public class DesignerTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldLoadScheme(TestService service)
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

        using var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri(service.Host.Uri);
        var jwt = await service.Client.CreateTokenAsync([WorkflowApiOperationId.Designer, WorkflowApiOperationId.DesignerGet]);
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JsonConvert.DeserializeObject<string>(jwt));
        httpClient.DefaultRequestHeaders.Add(WorkflowApiConstants.TenantIdHeader, service.TenantId);

        var requestUri = BuildRelativeUri(
            "/workflow-api/designer",
            new Dictionary<string, string?>
            {
                { nameof(OptimaJet.Workflow.Core.Designer.RequestElements.operation), nameof(OptimaJet.Workflow.Core.Designer.Operations.load) },
                { nameof(OptimaJet.Workflow.Core.Designer.RequestElements.schemecode), schemeCode }
            }
        );

        // Act

        var response = await httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();

        // Assert

        var expected = builder.ProcessDefinition;
        var actual = JsonConvert.DeserializeObject<ProcessDefinition>(payload);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.Activities.Count, actual.Activities.Count);
        Assert.AreEqual(expected.Commands.Count, actual.Commands.Count);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldDownloadScheme(TestService service)
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

        using var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri(service.Host.Uri);
        var jwt = await service.Client.CreateTokenAsync([WorkflowApiOperationId.Designer, WorkflowApiOperationId.DesignerGet]);
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JsonConvert.DeserializeObject<string>(jwt));
        httpClient.DefaultRequestHeaders.Add(WorkflowApiConstants.TenantIdHeader, service.TenantId);

        var content = new FormUrlEncodedContent(new Dictionary<string, string?>
        {
            { nameof(OptimaJet.Workflow.Core.Designer.RequestElements.operation), nameof(OptimaJet.Workflow.Core.Designer.Operations.downloadscheme) },
            { nameof(OptimaJet.Workflow.Core.Designer.RequestElements.data), JsonConvert.SerializeObject(builder.ProcessDefinition) }
        });

        // Act

        var response = await httpClient.PostAsync("/workflow-api/designer", content);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();

        // Assert

        var expected = builder.ProcessDefinition.Serialize();
        Assert.AreEqual(expected, payload);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldUploadScheme(TestService service)
    {
        // Arrange

        var builder = ProcessDefinitionBuilder.Create(Guid.NewGuid().ToString())
            .CreateActivity("Initial").Initial().State("InitialState").Ref(out ActivityDefinition initialActivity)
            .CreateActivity("Final").Ref(out ActivityDefinition finalActivity)
            .CreateCommand("Next").Ref(out CommandDefinition nextCommand)
            .CreateTransition("Initial_Final", initialActivity, finalActivity)
            .Direct()
            .TriggeredByCommand(nextCommand);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        using var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri(service.Host.Uri);
        var jwt = await service.Client.CreateTokenAsync([WorkflowApiOperationId.Designer, WorkflowApiOperationId.DesignerGet]);
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JsonConvert.DeserializeObject<string>(jwt));
        httpClient.DefaultRequestHeaders.Add(WorkflowApiConstants.TenantIdHeader, service.TenantId);

        var schemeCode = Guid.NewGuid().ToString();

        using var content = new MultipartFormDataContent();

        content.Add(
            new StringContent(nameof(OptimaJet.Workflow.Core.Designer.Operations.uploadscheme), Encoding.UTF8),
            nameof(OptimaJet.Workflow.Core.Designer.RequestElements.operation)
        );

        content.Add(
            new StringContent(schemeCode, Encoding.UTF8),
            nameof(OptimaJet.Workflow.Core.Designer.RequestElements.schemecode)
        );

        content.Add(
            new StringContent(builder.ProcessDefinition.Serialize(), Encoding.UTF8, "application/xml"),
            "file",
            $"{schemeCode}.xml"
        );

        // Act

        var response = await httpClient.PostAsync("/workflow-api/designer", content);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();

        // Assert

        var expected = builder.ProcessDefinition;
        var actual = JsonConvert.DeserializeObject<ProcessDefinition>(payload);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.Activities.Count, actual.Activities.Count);
        Assert.AreEqual(expected.Commands.Count, actual.Commands.Count);
    }

    private static string BuildRelativeUri(string requestPath, IDictionary<string, string?> query)
    {
        var path = "/" + requestPath.Trim('/');

        if (query.Count == 0 || query.All(kv => kv.Value is null))
        {
            return path;
        }

        var sb = new StringBuilder(path);

        var first = true;

        foreach (var (key, value) in query)
        {
            if (value is null)
            {
                continue;
            }

            if (first)
            {
                sb.Append('?');
            }
            else
            {
                sb.Append('&');
            }

            first = false;

            sb.Append(Uri.EscapeDataString(key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(value));
        }

        return sb.ToString();
    }
}
