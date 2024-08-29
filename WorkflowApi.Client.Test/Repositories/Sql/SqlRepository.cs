using OptimaJet.DataEngine;
using OptimaJet.DataEngine.Metadata;
using OptimaJet.DataEngine.Mssql;
using OptimaJet.DataEngine.Mysql;
using OptimaJet.DataEngine.Oracle;
using OptimaJet.DataEngine.Postgres;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using OptimaJet.DataEngine.Sqlite;
using WorkflowApi.Client.Test.Repositories.Sql.Entities;
using WorkflowApi.Client.Test.Repositories.Sql.TypeHandlers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Repositories.Sql;

public class SqlRepository : IRepository
{
    static SqlRepository()
    {
        //Mysql configuration

        TypeHandlerRegistry.Register(new MysqlGuidHandler(), ProviderName.Mysql);

        //Oracle configuration

        TypeHandlerRegistry.Register(new OracleGuidHandler(), ProviderName.Oracle);
        TypeHandlerRegistry.Register(new OracleBooleanHandler(), ProviderName.Oracle);

        //ParameterEntity
        {
            var metadata = MetadataPool<ParameterEntity>.GetMetadata(ProviderName.Oracle);
            metadata.GetNameFn = _ => "WORKFLOWPROCESSINSTANCEP";
        }

        //RuntimeEntity
        {
            var metadata = MetadataPool<RuntimeEntity>.GetMetadata(ProviderName.Oracle);
            var column = metadata.Columns.First(c => c.OriginalName == nameof(RuntimeEntity.Lock));
            column.GetNameFn = _ => "LOCKFLAG";
        }

        //StatusEntity
        {
            var metadata = MetadataPool<StatusEntity>.GetMetadata(ProviderName.Oracle);

            metadata.GetNameFn = _ => "WORKFLOWPROCESSINSTANCES";
            var column = metadata.Columns.First(c => c.OriginalName == nameof(StatusEntity.Lock));
            column.GetNameFn = _ => "LOCKFLAG";
        }

        //TransitionEntity
        {
            var metadata = MetadataPool<TransitionEntity>.GetMetadata(ProviderName.Oracle);
            metadata.GetNameFn = _ => "WORKFLOWPROCESSTRANSITIONH";
        }
    }

    public SqlRepository(TestService testService)
    {
        _provider = testService.Configuration.AppConfiguration.Provider;
        _connectionString = testService.Configuration.AppConfiguration.ConnectionStrings[_provider.ToString()];

        Approvals = new SqlApprovalRepository();
        GlobalParameters = new SqlGlobalParameterRepository();
        InboxEntries = new SqlInboxEntryRepository();
        Parameters = new SqlParameterRepository();
        Processes = new SqlProcessRepository();
        Runtimes = new SqlRuntimeRepository();
        Schemes = new SqlSchemeRepository();
        Statuses = new SqlStatusRepository();
        Timers = new SqlTimerRepository();
        Transitions = new SqlTransitionRepository();
    }

    public IAsyncDisposable Use()
    {
        return _provider switch
        {
            Provider.Mssql => MssqlProvider.Use(_connectionString),
            Provider.Mysql => MysqlProvider.Use(_connectionString),
            Provider.Oracle => OracleProvider.Use(_connectionString),
            Provider.Postgres => PostgresProvider.Use(_connectionString),
            Provider.Sqlite => SqliteProvider.Use(_connectionString),
            _ => throw new ArgumentOutOfRangeException(nameof(_provider))
        };
    }

    public IApprovalRepository Approvals { get; }
    public IGlobalParameterRepository GlobalParameters { get; }
    public IInboxEntryRepository InboxEntries { get; }
    public IParameterRepository Parameters { get; }
    public IProcessRepository Processes { get; }
    public IRuntimeRepository Runtimes { get; }
    public ISchemeRepository Schemes { get; }
    public IStatusRepository Statuses { get; }
    public ITimerRepository Timers { get; }
    public ITransitionRepository Transitions { get; }

    private readonly Provider _provider;
    private readonly string _connectionString;
}
