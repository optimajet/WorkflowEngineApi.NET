using System.Runtime.InteropServices;
using OptimaJet.DataEngine;

namespace WorkflowApi.Client.Test.Runner;

public class TestServiceSet : IDisposable
{
    private static readonly Lazy<TestServiceSet> LazyInstance = new(() => CreateAsync().Result);
    public static TestServiceSet Instance => LazyInstance.Value;
    
    public static List<string> DefaultRunningConfigurations { get; } = [
        ProviderName.Mongo,
        ProviderName.Mssql,
        ProviderName.Mysql,
        ProviderName.Oracle,
        ProviderName.Postgres,
        ProviderName.Sqlite
    ];

    public static List<string> RunningConfigurations { get; } = Environment
        .GetEnvironmentVariable("RUNNING_CONFIGURATIONS")?
        .Split(",")
        .Select(s => s.Trim())
        .ToList() ?? DefaultRunningConfigurations;

    public static IReadOnlyList<TestConfiguration> Configurations { get; } = [
        new()
        {
            Name = ProviderName.Mongo,
            Port = 8081,
            AppConfiguration =
            {
                Provider = Provider.Mongo,
            }
        },
        new()
        {
            Name = ProviderName.Mssql,
            Port = 8082,
            AppConfiguration =
            {
                Provider = Provider.Mssql,
            }
        },
        new()
        {
            Name = ProviderName.Mysql,
            Port = 8083,
            AppConfiguration =
            {
                Provider = Provider.Mysql,
            }
        },
        new()
        {
            Name = ProviderName.Oracle,
            Port = 8084,
            AppConfiguration =
            {
                Provider = Provider.Oracle,
            }
        },
        new()
        {
            Name = ProviderName.Postgres,
            Port = 8085,
            AppConfiguration =
            {
                Provider = Provider.Postgres,
            }
        },
        new()
        {
            Name = ProviderName.Sqlite,
            Port = 8086,
            AppConfiguration =
            {
                Provider = Provider.Sqlite,
            }
        }
    ];

    public static async Task<TestServiceSet> CreateAsync()
    {
        EnsureConfigurationNamesIsUnique();

        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64 || RuntimeInformation.ProcessArchitecture == Architecture.Arm)
        {
            Console.WriteLine("Oracle is not supported on ARM architecture. Excluding Oracle configuration");
            RunningConfigurations.Remove(ProviderName.Oracle);
            Console.WriteLine("Sqlite is not supported on ARM architecture. Excluding Sqlite configuration");
            RunningConfigurations.Remove(ProviderName.Sqlite);
        }
        
        var services = await Task.WhenAll(
            Configurations
                .Where(c => RunningConfigurations.Contains(c.Name))
                .Select(TestService.CreateAsync)
        );

        var servicesSet = new TestServiceSet();
        servicesSet.TestServices = services;
        return servicesSet;
    }

    public IReadOnlyList<TestService> TestServices { get; private set; } = [];

    private static void EnsureConfigurationNamesIsUnique()
    {
        var names = Configurations.Select(configuration => configuration.Name).ToList();

        if (names.Count != names.Distinct().Count())
        {
            throw new InvalidOperationException("Configuration names must be unique");
        }
    }

    private TestServiceSet() { }

    #region IDisposable Implementation

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        TestServices.ToList().ForEach(s => s.Dispose());
    }

    #endregion
}