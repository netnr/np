#if Full || DataKit

using System.IO;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Data.SQLite;
using Netnr.SharedAdo;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitSQLiteTo : IDataKitTo
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public DbHelper db;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="conn">连接字符串</param>
        public DataKitSQLiteTo(string conn)
        {
            db = new DbHelper(new SQLiteConnection(connectionString = conn));
        }

        /// <summary>
        /// 获取所有表信息的SQL脚本
        /// </summary>
        public static string GetTableSQL()
        {
            return @"
                    SELECT
                        tbl_name AS TableName,
                        '' AS TableComment
                    FROM
                        sqlite_master
                    WHERE
                        type = 'table'
                    ORDER BY tbl_name
                ";
        }

        /// <summary>
        /// 获取所有列信息的SQL脚本
        /// </summary>
        /// <param name="dataTableName">表名</param>
        /// <returns></returns>
        public static string GetColumnSQL(string dataTableName)
        {
            return $"SELECT '{dataTableName}' tablename, * FROM pragma_table_info('{dataTableName}')";
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <param name="listTableName">表名</param>
        /// <returns></returns>
        public List<TableColumnVM> GetColumn(List<string> listTableName = null)
        {
            if (listTableName == null)
            {
                listTableName = GetTable().Select(x => x.TableName).ToList();
            }
            else
            {
                listTableName.ForEach(x => x = x.Replace("'", ""));
            }

            var listSql = new List<string>();
            foreach (var tableName in listTableName)
            {
                listSql.Add(GetColumnSQL(tableName));
            }
            var sql = string.Join("\nUNION ALL\n", listSql);
            listSql.Clear();

            //自增信息
            sql += ";SELECT name, sql from SQLITE_MASTER WHERE 1=1";
            if (listTableName != null && listTableName.Count > 0)
            {
                sql += " AND name IN('" + string.Join("','", listTableName) + "')";
            }

            var ds = db.SqlQuery(sql);
            var dts1 = ds.Tables[0].Select();
            var dts2 = ds.Tables[1].Select();

            var listColumn = new List<TableColumnVM>();
            foreach (var tablename in listTableName)
            {
                var dts = dts1.Where(x => x["tablename"].ToString() == tablename);

                //表创建SQL （分析该SQL语句获取自增列信息）
                var aacreate = dts2.FirstOrDefault(x => x["name"].ToString() == tablename)["sql"].ToString();
                var aasi = aacreate.IndexOf('(');
                var aaei = aacreate.LastIndexOf(')');
                aacreate = aacreate[aasi..aaei];
                //有自增
                bool hasaa = aacreate.ToUpper().Contains("AUTOINCREMENT");

                int ti = 1;
                foreach (DataRow dr in dts)
                {
                    var colmo = new TableColumnVM()
                    {
                        TableName = tablename,
                        TableComment = "",
                        FieldName = dr["name"].ToString(),
                        DataTypeLength = dr["type"].ToString(),
                        FieldOrder = ti++,
                        PrimaryKey = dr["pk"].ToString() == "1" ? "YES" : "",
                        NotNull = dr["notnull"].ToString() == "1" ? "YES" : "",
                        DefaultValue = dr["dflt_value"]?.ToString(),
                        FieldComment = ""
                    };
                    if (colmo.DataTypeLength.Contains("("))
                    {
                        var tlarr = colmo.DataTypeLength.TrimEnd(')').Split('(').ToList();
                        colmo.DataType = tlarr[0];
                        var dls = tlarr[1].Split(',').ToList();
                        colmo.DataLength = dls.FirstOrDefault().Trim();
                        colmo.DataScale = dls.Count == 2 ? dls.LastOrDefault().Trim() : null;
                    }
                    else
                    {
                        colmo.DataType = colmo.DataTypeLength;
                    }

                    if (hasaa)
                    {
                        var aais = aacreate.ToUpper().Split(',').ToList().Any(x => x.Contains("AUTOINCREMENT") && x.Contains(colmo.FieldName.ToUpper()));
                        colmo.AutoAdd = aais ? "YES" : string.Empty;
                    }
                    else
                    {
                        colmo.AutoAdd = string.Empty;
                    }

                    listColumn.Add(colmo);
                }
            }

            return listColumn;
        }

        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <returns></returns>
        public List<TableNameVM> GetTable()
        {
            var sql = GetTableSQL();
            var ds = db.SqlQuery(sql);

            var list = ds.Tables[0].ToModel<TableNameVM>();

            return list;
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        public bool SetTableComment(string TableName, string TableComment)
        {
            return false;
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="FieldName">列名</param>
        /// <param name="FieldComment">列注释</param>
        /// <returns></returns>
        public bool SetColumnComment(string TableName, string FieldName, string FieldComment)
        {
            return false;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="page">页码</param>
        /// <param name="rows">页量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序方式</param>
        /// <param name="listFieldName">查询列，默认为 *</param>
        /// <param name="whereSql">条件</param>
        /// <param name="total">返回总条数</param>
        /// <returns></returns>
        public DataTable GetData(string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, out int total)
        {
            if (string.IsNullOrWhiteSpace(whereSql))
            {
                whereSql = "";
            }
            else
            {
                whereSql = "WHERE " + whereSql;
            }

            var sql = $@"
                        SELECT
                            {listFieldName}
                        FROM
                            {TableName} {whereSql}
                        ORDER BY
                            {sort} {order}
                        LIMIT
                            {rows} OFFSET {(page - 1) * rows}";

            sql += $";select count(1) as total from {TableName} {whereSql}";

            var ds = db.SqlQuery(sql);

            var dt = ds.Tables[0];
            _ = int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);
            return dt;
        }

        /// <summary>
        /// 查询数据库环境信息
        /// </summary>
        /// <returns></returns>
        public DEIVM GetDEI()
        {
            var sql = @"
                        SELECT
                          'DeiVersion' col,
                          sqlite_version() val
                        UNION ALL
                        SELECT
                          'DeiDateTime' col,
                          datetime() val
                        UNION ALL
                        SELECT
                          'DeiIgnoreCase' col,
                          'a' = 'A' val

                        ;PRAGMA encoding
                        ";

            var mo = new DEIVM();

            var connection = db.Connection;
            connection.Open();

            var ds = db.SqlQuery(sql);

            var dt = ds.Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            mo.DeiCharSet = ds.Tables[1].Rows[0][0].ToString();

            mo.DeiName = Path.GetFileName(connection.DataSource);
            mo.DeiCompile = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
            mo.DeiDirInstall = Path.GetDirectoryName(connection.DataSource);
            mo.DeiDirData = mo.DeiDirInstall;
            mo.DeiTimeout = connection.ConnectionTimeout;
            mo.DeiSystem = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }

            return mo;
        }
    }
}

#endif