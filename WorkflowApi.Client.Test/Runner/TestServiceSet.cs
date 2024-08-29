using System.Runtime.InteropServices;
using OptimaJet.DataEngine;

namespace WorkflowApi.Client.Test.Runner;

public class TestServiceSet : IDisposable
{
    private static readonly Lazy<TestServiceSet> LazyInstance = new(() => CreateAsync().Result);
    public static TestServiceSet Instance => LazyInstance.Value;

public static List<string> Excluded { get; } =
[
    ProviderName.Mongo,
    ProviderName.Mysql,
    ProviderName.Oracle,
    ProviderName.Postgres,
    ProviderName.Sqlite
];

    public static IReadOnlyList<TestConfiguration> Configurations { get; } = [
        new TestConfiguration
        {
            Name = ProviderName.Mongo,
            Port = 8081,
            AppConfiguration =
            {
                Provider = Provider.Mongo,
            }
        },
        new TestConfiguration
        {
            Name = ProviderName.Mssql,
            Port = 8082,
            AppConfiguration =
            {
                Provider = Provider.Mssql,
            }
        },
        new TestConfiguration
        {
            Name = ProviderName.Mysql,
            Port = 8083,
            AppConfiguration =
            {
                Provider = Provider.Mysql,
            }
        },
        new TestConfiguration
        {
            Name = ProviderName.Oracle,
            Port = 8084,
            AppConfiguration =
            {
                Provider = Provider.Oracle,
            }
        },
        new TestConfiguration
        {
            Name = ProviderName.Postgres,
            Port = 8085,
            AppConfiguration =
            {
                Provider = Provider.Postgres,
            }
        },
        new TestConfiguration
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
            Excluded.Add(ProviderName.Oracle);
            Console.WriteLine("Sqlite is not supported on ARM architecture. Excluding Sqlite configuration");
            Excluded.Add(ProviderName.Sqlite);
        }
        
        var services = await Task.WhenAll(
            Configurations
                .Where(c => !Excluded.Contains(c.Name))
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