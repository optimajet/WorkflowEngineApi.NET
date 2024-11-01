using System.Data;
using OptimaJet.DataEngine.Sql.TypeHandlers;
using Oracle.ManagedDataAccess.Client;

namespace WorkflowApi.Client.Test.Repositories.Sql.TypeHandlers;

public class OracleGuidHandler : GuidHandler
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        if (parameter is OracleParameter oracle)
        {
            oracle.DbType = DbType.Binary;
            oracle.OracleDbType = OracleDbType.Raw;
            oracle.Value = value.ToByteArray(); 
            
            return;
        }

        base.SetValue(parameter, value);
    }
}
