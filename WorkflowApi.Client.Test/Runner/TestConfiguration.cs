namespace WorkflowApi.Client.Test.Runner;

public class TestConfiguration
{
    public string Id { get; set; } = "";
    public int Port { get; set; }
    public AppCredentials AppCredentials { get; set; } = new();
    public Configuration AppConfiguration { get; set; } = new();
    public string[] AppArgs { get; set; } = [];
    //BasePath will be set by the generated Host.Url
    public WorkflowApi.Client.Client.Configuration ClientConfiguration { get; set; } = new();
}

public record AppCredentials(string Name = "admin", string Password = "admin");
