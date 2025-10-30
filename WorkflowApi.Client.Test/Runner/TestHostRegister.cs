using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using OptimaJet.Workflow.Core;

namespace WorkflowApi.Client.Test.Runner;

public class TestHostRegister : IDisposable
{
    public static IReadOnlyCollection<string> RunningProviders { get; } = CreateRunningProviders();
    
    private static IReadOnlyCollection<string> CreateRunningProviders()
    {
        List<string> defaultProviders =
        [
            PersistenceProviderId.Mongo,
            PersistenceProviderId.Mssql,
            PersistenceProviderId.Mysql,
            PersistenceProviderId.Oracle,
            PersistenceProviderId.Postgres,
            PersistenceProviderId.Sqlite
        ];
        
        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64 || RuntimeInformation.ProcessArchitecture == Architecture.Arm)
        {
            Console.WriteLine($"Oracle is not supported on ARM architecture. Excluding '{PersistenceProviderId.Oracle}' provider");
            defaultProviders.Remove(PersistenceProviderId.Oracle);
            Console.WriteLine($"Sqlite is not supported on ARM architecture. Excluding '{PersistenceProviderId.Sqlite}' provider");
            defaultProviders.Remove(PersistenceProviderId.Sqlite);
        }
        
        var runningProviders = Environment
            .GetEnvironmentVariable("RUNNING_PROVIDERS")?
            .Split(",")
            .Select(s => s.Trim())
            .ToList() ?? defaultProviders;
        
        return new ReadOnlyCollection<string>(runningProviders);
    }
    
    public static TestHostRegister Instance => _lazyInstance.Value;
    private static readonly Lazy<TestHostRegister> _lazyInstance = new(() => CreateAsync().Result);

    private static async Task<TestHostRegister> CreateAsync()
    {
        var register = new TestHostRegister();
        
        var dataHostConfiguration = new TestConfiguration
        {
            Id = HostId.DataHost,
            Port = 8081,
            AppConfiguration =
            {
                MultipleTenantMode = true,
                TenantsConfiguration =
                [
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mongo],
                        PersistenceProviderId = PersistenceProviderId.Mongo,
                        ConnectionString = "mongodb://localhost:47017/data-tests",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mssql],
                        PersistenceProviderId = PersistenceProviderId.Mssql,
                        ConnectionString = "Server=localhost,41433;Database=data_tests;User Id=SA;Password=P@ssw0rd;TrustServerCertificate=True;",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mysql],
                        PersistenceProviderId = PersistenceProviderId.Mysql,
                        ConnectionString = "Host=localhost;Port=43306;Database=data_tests;User ID=root;Password=P@ssw0rd;",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Oracle],
                        PersistenceProviderId = PersistenceProviderId.Oracle,
                        ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=41521))(CONNECT_DATA=(SERVICE_NAME=FREEPDB1)));User Id=DATA_TESTS;Password=password;",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Postgres],
                        PersistenceProviderId = PersistenceProviderId.Postgres,
                        ConnectionString = "Host=localhost;Port=45432;Database=data_tests;User Id=postgres;Password=P@ssw0rd;",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Sqlite],
                        PersistenceProviderId = PersistenceProviderId.Sqlite,
                        ConnectionString = "Data Source=data_tests.db",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    }
                ]
            }
        };
        
        await register.CreateHostAsync(dataHostConfiguration);
        
        var rpcHostConfiguration = new TestConfiguration
        {
            Id = HostId.RpcHost,
            Port = 8082,
            AppConfiguration =
            {
                MultipleTenantMode = true,
                TenantsConfiguration =
                [
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mongo],
                        PersistenceProviderId = PersistenceProviderId.Mongo,
                        ConnectionString = "mongodb://localhost:47017/rpc-tests",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mssql],
                        PersistenceProviderId = PersistenceProviderId.Mssql,
                        ConnectionString = "Server=localhost,41433;Database=rpc_tests;User Id=SA;Password=P@ssw0rd;TrustServerCertificate=True;",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mysql],
                        PersistenceProviderId = PersistenceProviderId.Mysql,
                        ConnectionString = "Host=localhost;Port=43306;Database=rpc_tests;User ID=root;Password=P@ssw0rd;",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Oracle],
                        PersistenceProviderId = PersistenceProviderId.Oracle,
                        ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=41522))(CONNECT_DATA=(SERVICE_NAME=FREEPDB1)));User Id=RPC_TESTS;Password=password;",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Postgres],
                        PersistenceProviderId = PersistenceProviderId.Postgres,
                        ConnectionString = "Host=localhost;Port=45432;Database=rpc_tests;User Id=postgres;Password=P@ssw0rd;",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Sqlite],
                        PersistenceProviderId = PersistenceProviderId.Sqlite,
                        ConnectionString = "Data Source=rpc_tests.db",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    }
                ]
            }
        };
        
        await register.CreateHostAsync(rpcHostConfiguration);
        
        return register;
    }

    public IReadOnlyCollection<Host> Hosts => new ReadOnlyCollection<Host>(_hosts);

    public async Task<Host> CreateHostAsync(TestConfiguration configuration)
    {
        EnsureConfigurationIdIsUnique(configuration.Id);

        // Exclude configurations with data providers that are not in the list of running providers
        configuration.AppConfiguration.TenantsConfiguration = configuration.AppConfiguration.TenantsConfiguration
            .Where(option => option.PersistenceProviderId == null || RunningProviders.Contains(option.PersistenceProviderId))
            .ToArray();
        
        var host = await Host.CreateAsync(configuration);
        _hosts.Add(host);
        return host;
    }
    
    public void EnsureConfigurationIdIsUnique(string id)
    {
        if (_hosts.Any(s => s.Id == id))
        {
            throw new InvalidOperationException($"Configuration with id '{id}' already exists");
        }
    }

    private readonly List<Host> _hosts = [];

    #region IDisposable Implementation

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _hosts.ToList().ForEach(s => s.Dispose());
    }

    #endregion
}
