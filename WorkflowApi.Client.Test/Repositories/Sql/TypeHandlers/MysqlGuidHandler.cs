using System.Data;
using MySqlConnector;
using OptimaJet.DataEngine.Sql.TypeHandlers;

namespace WorkflowApi.Client.Test.Repositories.Sql.TypeHandlers;

public class MysqlGuidHandler : GuidHandler
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        if (parameter is MySqlParameter mysql)
        {
            mysql.DbType = DbType.Binary;
            mysql.MySqlDbType = MySqlDbType.Binary;
            mysql.Value = value.ToByteArray(); 
            
            return;
        }

        base.SetValue(parameter, value);
    }
}
