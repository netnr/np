using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Netnr.Data.Oracle
{
    /// <summary>
    /// Oracle操作类
    /// </summary>
    public class OracleHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString = string.Empty;

        /// <summary>
        /// 构造，初始化字符串
        /// </summary>
        /// <param name="conn"></param>
        public OracleHelper(string conn = null)
        {
            connectionString = conn;
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DataTable</returns>
        public DataSet Query(string sql)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                OracleDataAdapter command = new OracleDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                command.Fill(ds, "ds");

                return ds;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="commandType">类型</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string sql, OracleParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();

                    cmd.CommandText = sql;
                    cmd.CommandType = commandType;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    int rows = cmd.ExecuteNonQuery();

                    return rows;
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, OracleParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();

                    cmd.CommandText = sql;
                    cmd.CommandType = commandType;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    var obj = cmd.ExecuteScalar();

                    return obj;
                }
            }
        }
    }
}
