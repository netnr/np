#if Full || AdoMySQL

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using MySqlConnector;
using System.Data.Common;
using System.Collections.Generic;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// MySQL操作类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 表批量写入
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <returns></returns>
        public int BulkCopyMySQL(DataTable dt, string table)
        {
            return SafeConn(() =>
            {
                var csvpath = Path.Combine(Path.GetTempPath(), "mysql_bulk_dt_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv");
                var csvcontent = DataTableToCsv(dt);
                using (var fs = new FileStream(csvpath, FileMode.Create))
                {
                    using var sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.WriteLine(csvcontent);
                }

                var connection = (MySqlConnection)Connection;
                var bulk = new MySqlBulkLoader(connection)
                {
                    FieldTerminator = ",",
                    FieldQuotationCharacter = '"',
                    EscapeCharacter = '"',
                    LineTerminator = "\r\n",
                    FileName = csvpath,
                    NumberOfLinesToSkip = 0,
                    TableName = table
                };

                var columns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                bulk.Columns.AddRange(columns);

                int rows = bulk.Load();

                File.Delete(csvpath);

                return rows;
            });
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
            StringBuilder sb = new();

            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(',');
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


        /// <summary>
        /// 执行SQL语句，返回打印信息
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns>返回打印信息</returns>
        public List<string> SqlPrintMySQL(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var connection = (MySqlConnection)Connection;

                var listPrint = new List<string>();
                connection.InfoMessage += (s, e) => listPrint.AddRange(e.Errors.Select(x => x.Message));

                var cmd = GetCommand(sql, parameters);
                cmd.ExecuteNonQuery();

                return listPrint;
            });
        }
    }
}

#endif