using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Netnr.DataKit.Application
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitSQLiteService : IDataKitService
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="conn">连接字符串</param>
        public DataKitSQLiteService(string conn)
        {
            connectionString = conn;
        }

        /// <summary>
        /// 获取所有表信息的SQL脚本
        /// </summary>
        public string GetTableSQL()
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
        public string GetColumnSQL(string dataTableName)
        {
            return $"PRAGMA table_info('{dataTableName}')";
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <param name="listTableName">表名</param>
        /// <returns></returns>
        public List<Model.DkTableColumn> GetColumn(List<string> listTableName = null)
        {
            if (listTableName == null)
            {
                listTableName = GetTable().Select(x => x.TableName).ToList();
            }
            else
            {
                listTableName.ForEach(x => x.Replace("'", ""));
            }

            var listSql = new List<string>();
            foreach (var tableName in listTableName)
            {
                listSql.Add(GetColumnSQL(tableName));
            }

            //自增信息
            var aasql = "SELECT name, sql from SQLITE_MASTER WHERE 1=1";
            if (listTableName != null && listTableName.Count > 0)
            {
                aasql += " AND name IN('" + string.Join("','", listTableName) + "')";
            }
            listSql.Add(aasql);

            var sql = string.Join(";", listSql);
            var ds = new Data.SQLite.SQLiteHelper(connectionString).Query(sql);

            var aadt = ds.Tables[ds.Tables.Count - 1].Select();

            var listColumn = new List<Model.DkTableColumn>();

            for (int i = 0; i < listTableName.Count; i++)
            {
                var dt = ds.Tables[i];
                var tableName = listTableName[i];

                //表创建SQL （分析该SQL语句获取自增列信息）
                var aacreate = aadt.FirstOrDefault(x => x["name"].ToString() == tableName)["sql"].ToString();
                var aasi = aacreate.IndexOf('(');
                var aaei = aacreate.LastIndexOf(')');
                aacreate = aacreate.Substring(aasi, aaei - aasi);
                //有自增
                bool hasaa = aacreate.ToUpper().Contains("AUTOINCREMENT");

                int ti = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    var colmo = new Model.DkTableColumn()
                    {
                        TableName = tableName,
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
        public List<Model.DkTableName> GetTable()
        {
            var ds = new Data.SQLite.SQLiteHelper(connectionString).Query(GetTableSQL());

            var list = ds.Tables[0].ToModel<Model.DkTableName>();

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
        /// <param name="total">返回总条数</param>
        /// <returns></returns>
        public DataTable GetData(string TableName, int page, int rows, string sort, string order, string listFieldName, out int total)
        {
            var sql = @"
                        SELECT
                            " + listFieldName + @"
                        FROM
                            " + TableName + @"
                        ORDER BY
                            " + sort + " " + order + @"
                        LIMIT
                            " + rows + " OFFSET " + (page - 1) * rows;

            sql += ";select count(1) as total from " + TableName;

            var ds = new Data.SQLite.SQLiteHelper(connectionString).Query(sql);

            var dt = ds.Tables[0];
            int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);
            return dt;
        }
    }
}
