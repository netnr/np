using System.Data.SqlClient;
using System.Data;

namespace Netnr.Data.SQLServer
{
    /// <summary>
    /// SQLServer操作类
    /// </summary>
    public class SQLServerHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString = string.Empty;

        /// <summary>
        /// 构造，初始化字符串
        /// </summary>
        /// <param name="conn"></param>
        public SQLServerHelper(string conn)
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
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlDataAdapter sda = new SqlDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            sda.Fill(ds, "ds");

            return ds;
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="commandType">类型</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string sql, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using var cmd = connection.CreateCommand();
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

        /// <summary>
        /// 执行SQL语句，返回首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using var cmd = connection.CreateCommand();
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

        /// <summary>
        /// 表批量写入
        /// </summary>
        /// <param name="sourceDataTable">数据表</param>
        /// <param name="targetTableName">数据库表名</param>
        /// <returns></returns>
        public int BulkCopy(DataTable sourceDataTable, string targetTableName)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using var bulk = new SqlBulkCopy(connection);
            connection.Open();

            bulk.DestinationTableName = targetTableName;
            bulk.BatchSize = sourceDataTable.Rows.Count;

            foreach (DataColumn dc in sourceDataTable.Columns)
            {
                bulk.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
            }

            bulk.WriteToServer(sourceDataTable);

            return sourceDataTable.Rows.Count;
        }
    }
}