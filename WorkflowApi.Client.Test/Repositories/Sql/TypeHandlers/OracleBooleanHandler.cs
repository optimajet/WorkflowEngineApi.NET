using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using Oracle.ManagedDataAccess.Client;

namespace WorkflowApi.Client.Test.Repositories.Sql.TypeHandlers;

public class OracleBooleanHandler : BooleanHandler
{
    public override void SetValue(IDbDataParameter parameter, bool value)
    {
        if (parameter is OracleParameter oracle)
        {
            oracle.DbType = DbType.Binary;
            oracle.OracleDbType = OracleDbType.Char;
            oracle.Value = value ? "1" : "0"; 
            
            return;
        }

        base.SetValue(parameter, value);
    }
}
