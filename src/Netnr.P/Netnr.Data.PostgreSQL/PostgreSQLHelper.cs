using Npgsql;
using System.Data;
using System.Linq;

namespace Netnr.Data.PostgreSQL
{
    /// <summary>
    /// PostgreSQL操作类
    /// </summary>
    public class PostgreSQLHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString = string.Empty;

        /// <summary>
        /// 构造，初始化字符串
        /// </summary>
        /// <param name="conn"></param>
        public PostgreSQLHelper(string conn)
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
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            NpgsqlDataAdapter command = new NpgsqlDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            command.Fill(ds, "ds");

            return ds;
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="commandType">类型</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string sql, NpgsqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
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
        public object ExecuteScalar(string sql, NpgsqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
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
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var columns = sourceDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            string copyString = "COPY " + "\"" + targetTableName + "\"" + " (\"" + string.Join("\",\"", columns) + "\") FROM STDIN (FORMAT BINARY)";
            using (var writer = connection.BeginTextImport(copyString))
            {
                foreach (DataRow dr in sourceDataTable.Rows)
                {
                    var rv = string.Empty;
                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (i > 0)
                        {
                            rv += ",";
                        }

                        var rc = dr[columns[i]].ToString();
                        if (rc.Contains(","))
                        {
                            rc = "\"" + rc.Replace("\"", "\"\"") + "\"";
                        }

                        rv += rc;
                    }

                    writer.WriteLine(rv);
                }
            }

            return sourceDataTable.Rows.Count;
        }
    }
}
