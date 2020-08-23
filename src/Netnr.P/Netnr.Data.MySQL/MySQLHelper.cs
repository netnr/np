using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Netnr.Data.MySQL
{
    /// <summary>
    /// MySQL操作类
    /// </summary>
    public class MySQLHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString = string.Empty;

        /// <summary>
        /// 构造，初始化字符串
        /// </summary>
        /// <param name="conn"></param>
        public MySQLHelper(string conn)
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
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlDataAdapter command = new MySqlDataAdapter(sql, connection);
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
        public int ExecuteNonQuery(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
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
        public object ExecuteScalar(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
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
            var csvpath = Path.Combine(Path.GetTempPath(), "mysql_bulk_dt_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv");
            var csvcontent = DataTableToCsv(sourceDataTable);
            using (var fs = new FileStream(csvpath, FileMode.Create))
            {
                using var sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(csvcontent);
            }

            using MySqlConnection connection = new MySqlConnection(connectionString);
            var bulk = new MySqlBulkLoader(connection)
            {
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                LineTerminator = "\r\n",
                FileName = csvpath,
                NumberOfLinesToSkip = 0,
                TableName = targetTableName
            };

            var columns = sourceDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            bulk.Columns.AddRange(columns);

            int rows = bulk.Load();

            File.Delete(csvpath);

            return rows;
        }

        /// <summary>
        /// DataTable转换为标准的CSV
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        private static string DataTableToCsv(DataTable dt)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();

            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }

                    var rc = dr[dt.Columns[i]].ToString();
                    if (rc.Contains(","))
                    {
                        rc = "\"" + rc.Replace("\"", "\"\"") + "\"";
                    }

                    sb.Append(rc);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}