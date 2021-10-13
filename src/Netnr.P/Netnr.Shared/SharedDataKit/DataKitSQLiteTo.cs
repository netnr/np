#if Full || DataKit

using System.Data;
using System.Data.Common;
using Netnr.SharedAdo;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitSQLiteTo : IDataKitTo
    {
        /// <summary>
        /// 连接
        /// </summary>
        public DbConnection dbConnection;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public DbHelper db;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbConn">连接</param>
        public DataKitSQLiteTo(DbConnection dbConn)
        {
            db = new DbHelper(dbConnection = dbConn);
        }

        /// <summary>
        /// 默认库名
        /// </summary>
        /// <returns></returns>
        public string DefaultDatabaseName()
        {
            return dbConnection.Database;
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public List<DatabaseVM> GetDatabase()
        {
            Microsoft.Data.Sqlite.SqliteConnectionStringBuilder builder = new(dbConnection.ConnectionString);
            var fi = new FileInfo(builder.DataSource);

            var list = new List<DatabaseVM>
            {
                new DatabaseVM
                {
                    DatabaseName = DefaultDatabaseName(),
                    DatabasePath = builder.DataSource,
                    DatabaseDataLength = fi.Length,
                    DatabaseCreateTime = fi.CreationTime
                }
            };

            return list;
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public List<TableVM> GetTable(string DatabaseName = null)
        {
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                DatabaseName = DefaultDatabaseName();
            }

            var sql = Configs.GetTableSQLite(DatabaseName);
            var ds = db.SqlQuery(sql);

            var list = ds.Tables[0].ToModel<TableVM>();
            return list;
        }

        /// <summary>
        /// 表DDL
        /// </summary>
        /// <param name="filterTableName"></param>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetTableDDL(string filterTableName = null, string DatabaseName = null)
        {
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                DatabaseName = DefaultDatabaseName();
            }

            var listTable = string.IsNullOrWhiteSpace(filterTableName)
                ? GetTable(DatabaseName).Select(x => x.TableName).ToList()
                : filterTableName.Replace("'", "").Split(',').ToList();

            var sql = Configs.GetTableDDLSQLite(DatabaseName, listTable);
            var dt = db.SqlQuery(sql).Tables[0];

            var dic = new Dictionary<string, string>();
            foreach (DataRow dr in dt.Rows)
            {
                dic.Add(dr[0].ToString(), dr[1].ToString());
            }

            return dic;
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="filterTableName"></param>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public List<ColumnVM> GetColumn(string filterTableName = null, string DatabaseName = null)
        {
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                DatabaseName = DefaultDatabaseName();
            }

            var where = string.Empty;
            if (!string.IsNullOrWhiteSpace(filterTableName))
            {
                where = $"AND m.name IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnSQLite(DatabaseName, where);
            var ds = db.SqlQuery(sql);
            ds.Tables[0].Rows.RemoveAt(0);
            var ds2 = ds.Tables[1].Select();

            var aakey = "AUTOINCREMENT";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var csql = ds2.FirstOrDefault(x => x["name"].ToString().ToLower() == dr["TableName"].ToString().ToLower())[1].ToString().ToUpper();
                if (csql.Contains(aakey))
                {
                    var isaa = csql.Split(',').Any(x => x.Contains(aakey) && x.Contains(dr["ColumnName"].ToString().ToUpper()));
                    if (isaa)
                    {
                        dr["AutoAdd"] = "YES";
                    }
                }
            }

            var list = ds.Tables[0].ToModel<ColumnVM>();

            return list;
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="TableComment"></param>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public bool SetTableComment(string TableName, string TableComment, string DatabaseName = null)
        {
            return true;
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <param name="ColumnComment"></param>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public bool SetColumnComment(string TableName, string ColumnName, string ColumnComment, string DatabaseName = null)
        {
            return true;
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="sql">脚本</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public Tuple<DataSet, object> ExecuteSql(string sql, string DatabaseName = null)
        {
            var ds = new DataSet();

            var queryKey = "select,with,pragma".Split(',').ToList();
            if (queryKey.Any(k => sql.StartsWith(k, StringComparison.OrdinalIgnoreCase)))
            {
                ds = db.SqlQuery(sql);
            }
            else
            {
                var dt = new DataTable();
                dt.Columns.Add(new DataColumn("rows"));
                var dr = dt.NewRow();
                dr[0] = db.SqlExecute(sql);
                ds.Tables.Add(dt);
            }

            return new Tuple<DataSet, object>(ds, null);
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="listFieldName"></param>
        /// <param name="whereSql"></param>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public Tuple<DataTable, int> GetData(string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, string DatabaseName = null)
        {
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                DatabaseName = DefaultDatabaseName();
            }

            if (string.IsNullOrWhiteSpace(listFieldName))
            {
                listFieldName = "*";
            }

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
            _ = int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out int total);

            return new Tuple<DataTable, int>(dt, total);
        }

        /// <summary>
        /// 查询数据库环境信息
        /// </summary>
        /// <returns></returns>
        public DEIVM GetDEI()
        {
            var sql = @"
                        SELECT
                          'Version' col,
                          sqlite_version() val
                        UNION ALL
                        SELECT
                          'DateTime' col,
                          datetime() val
                        UNION ALL
                        SELECT
                          'IgnoreCase' col,
                          'a' = 'A' val

                        ;PRAGMA encoding
                        ";

            var mo = new DEIVM();

            var connection = db.Connection;
            connection.Open();

            var ds = db.SqlQuery(sql);

            var dt = ds.Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            mo.CharSet = ds.Tables[1].Rows[0][0].ToString();

            mo.Name = Path.GetFileName(connection.DataSource);
            mo.Compile = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
            mo.DirInstall = Path.GetDirectoryName(connection.DataSource);
            mo.DirData = mo.DirInstall;
            mo.TimeOut = connection.ConnectionTimeout;
            mo.System = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }

            return mo;
        }
    }
}

#endif