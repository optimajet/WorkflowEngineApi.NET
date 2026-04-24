using System.Collections.ObjectModel;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
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
            Port = register.GetFreePort(),
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

        CleanupSqliteDatabases(dataHostConfiguration);
        await register.CreateHostAsync(dataHostConfiguration);
        
        var rpcHostConfiguration = new TestConfiguration
        {
            Id = HostId.RpcHost,
            Port = register.GetFreePort(),
            AppConfiguration =
            {
                MultipleTenantMode = true,
                TenantsConfiguration =
                [
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mongo],
                        PersistenceProviderId = PersistenceProviderId.Mongo,
                        ConnectionString = "mongodb://localhost:47017/rpc-tests"
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mssql],
                        PersistenceProviderId = PersistenceProviderId.Mssql,
                        ConnectionString = "Server=localhost,41433;Database=rpc_tests;User Id=SA;Password=P@ssw0rd;TrustServerCertificate=True;"
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Mysql],
                        PersistenceProviderId = PersistenceProviderId.Mysql,
                        ConnectionString = "Host=localhost;Port=43306;Database=rpc_tests;User ID=root;Password=P@ssw0rd;"
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Oracle],
                        PersistenceProviderId = PersistenceProviderId.Oracle,
                        ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=41522))(CONNECT_DATA=(SERVICE_NAME=FREEPDB1)));User Id=RPC_TESTS;Password=password;"
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Postgres],
                        PersistenceProviderId = PersistenceProviderId.Postgres,
                        ConnectionString = "Host=localhost;Port=45432;Database=rpc_tests;User Id=postgres;Password=P@ssw0rd;"
                    },
                    new()
                    {
                        TenantIds = [PersistenceProviderId.Sqlite],
                        PersistenceProviderId = PersistenceProviderId.Sqlite,
                        ConnectionString = "Data Source=rpc_tests.db"
                    }
                ]
            }
        };

        CleanupSqliteDatabases(rpcHostConfiguration);
        await register.CreateHostAsync(rpcHostConfiguration);
        
        var multitenantHostConfiguration = new TestConfiguration
        {
            Id = HostId.MultiTenantHost,
            Port = register.GetFreePort(),
            AppConfiguration =
            {
                MultipleTenantMode = true,
                TenantsConfiguration =
                [
                    new()
                    {
                        TenantIds = MultitenantTestTenants.TenantAIds,
                        PersistenceProviderId = PersistenceProviderId.Sqlite,
                        ConnectionString = "Data Source=rpc_multitenant_tenant_a.db",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = MultitenantTestTenants.TenantBIds,
                        PersistenceProviderId = PersistenceProviderId.Sqlite,
                        ConnectionString = "Data Source=rpc_multitenant_tenant_b.db",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    },
                    new()
                    {
                        TenantIds = MultitenantTestTenants.TenantCIds,
                        PersistenceProviderId = PersistenceProviderId.Sqlite,
                        ConnectionString = "Data Source=rpc_multitenant_tenant_c.db",
                        WorkflowRuntimeCreationOptions =
                        {
                            DisableRuntimeAutoStart = true
                        }
                    }
                ]
            }
        };

        CleanupSqliteDatabases(multitenantHostConfiguration);
        await register.CreateHostAsync(multitenantHostConfiguration);

        var singleTenantHostConfiguration = new TestConfiguration
        {
            Id = HostId.SingleTenantHost,
            Port = register.GetFreePort(),
            AppConfiguration =
            {
                MultipleTenantMode = false,
                WorkflowTenantCreationOptions = new()
                {
                    PersistenceProviderId = PersistenceProviderId.Sqlite,
                    ConnectionString = "Data Source=single_tenant_tests.db",
                    WorkflowRuntimeCreationOptions =
                    {
                        DisableRuntimeAutoStart = true
                    }
                }
            }
        };

        CleanupSqliteDatabases(singleTenantHostConfiguration);
        await register.CreateHostAsync(singleTenantHostConfiguration);
        
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
    private readonly HashSet<int> _reservedPorts = [];

    private static void CleanupSqliteDatabases(TestConfiguration configuration)
    {
        var tenants = configuration.AppConfiguration.TenantsConfiguration
            .Where(option => option.PersistenceProviderId == PersistenceProviderId.Sqlite);
        
        foreach (var tenant in tenants)
        {
            CleanupSqliteDatabase(tenant.ConnectionString);
        }
    }

    private static void CleanupSqliteDatabase(string? connectionString)
    {
        if (String.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        if (!builder.TryGetValue("Data Source", out var dataSourceValue) || dataSourceValue is not string dataSource)
        {
            return;
        }

        if (String.IsNullOrWhiteSpace(dataSource) || dataSource == ":memory:")
        {
            return;
        }

        foreach (var databasePath in GetDatabasePaths(dataSource))
        {
            DeleteIfExists(databasePath);
            DeleteIfExists($"{databasePath}-wal");
            DeleteIfExists($"{databasePath}-shm");
            DeleteIfExists($"{databasePath}-journal");
        }
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static IEnumerable<string> GetDatabasePaths(string dataSource)
    {
        if (Path.IsPathRooted(dataSource))
        {
            yield return dataSource;
            yield break;
        }

        var seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var candidatePaths = new[]
        {
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, dataSource)),
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, dataSource))
        };

        foreach (var candidatePath in candidatePaths)
        {
            if (seenPaths.Add(candidatePath))
            {
                yield return candidatePath;
            }
        }
    }

    private int GetFreePort()
    {
        for (var port = PreferredPortRangeStart; port <= PreferredPortRangeEnd; port++)
        {
            if (_reservedPorts.Contains(port) || !IsPortAvailable(port))
            {
                continue;
            }

            _reservedPorts.Add(port);
            return port;
        }

        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var fallbackPort = ((IPEndPoint)listener.LocalEndpoint).Port;
        _reservedPorts.Add(fallbackPort);
        return fallbackPort;
    }

    private static bool IsPortAvailable(int port)
    {
        try
        {
            using var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private const int PreferredPortRangeStart = 8081;
    private const int PreferredPortRangeEnd = 8099;

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
