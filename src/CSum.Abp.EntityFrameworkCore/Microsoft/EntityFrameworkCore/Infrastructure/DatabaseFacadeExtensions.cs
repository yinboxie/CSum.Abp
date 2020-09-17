using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public static class DatabaseFacadeExtensions
    {
        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection connection, params object[] parameters)
        {
            var conn = facade.GetDbConnection();
            connection = conn;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            var cmd = conn.CreateCommand();
            //如果当前有事务，必须要携带事务
            if (facade.CurrentTransaction != null)
            {
                cmd.Transaction = facade.CurrentTransaction.GetDbTransaction();
            }
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(parameters);
            return cmd;
        }

        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = command.ExecuteReader();
            command.Parameters.Clear();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            if (facade.CurrentTransaction == null)
            {
                facade.CloseConnection(); //没有事务时，关闭连接
            }
            //conn.Close();  //在事务中时，不需要关闭连接
            return dt;
        }

        public static object ExecuteScalar(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var obj = command.ExecuteScalar();
            command.Parameters.Clear();
            if (facade.CurrentTransaction == null)
            {
                facade.CloseConnection(); //没有事务时，关闭连接
            }
            return obj;
        }
    }
}
