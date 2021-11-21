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
        /// 获取DbHelper
        /// </summary>
        public DbHelper GetDbHelper() => db;

        /// <summary>
        /// 默认库名
        /// </summary>
        /// <returns></returns>
        public string DefaultDatabaseName()
        {
            return dbConnection.Database;
        }

        /// <summary>
        /// 获取库名
        /// </summary>
        /// <returns></returns>
        public List<string> GetDatabaseName()
        {
            var list = new List<string>() { DefaultDatabaseName() };

            return list;
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <param name="filterDatabaseName">数据库名</param>
        /// <returns></returns>
        public List<DatabaseVM> GetDatabase(string filterDatabaseName = null)
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
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public List<TableVM> GetTable(string databaseName = null)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var sql = Configs.GetTableSQLite(databaseName);
            var ds = db.SqlExecuteReader(sql);

            var list = ds.Item1.Tables[0].ToModel<TableVM>();
            return list;
        }

        /// <summary>
        /// 表DDL
        /// </summary>
        /// <param name="filterTableName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetTableDDL(string filterTableName = null, string databaseName = null)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var listTable = string.IsNullOrWhiteSpace(filterTableName)
                ? GetTable(databaseName).Select(x => x.TableName).ToList()
                : filterTableName.Replace("'", "").Split(',').ToList();

            var sql = Configs.GetTableDDLSQLite(databaseName, listTable);
            var dt = db.SqlExecuteReader(sql).Item1.Tables[0];

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
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public List<ColumnVM> GetColumn(string filterTableName = null, string databaseName = null)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var where = string.Empty;
            if (!string.IsNullOrWhiteSpace(filterTableName))
            {
                where = $"AND m.name IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnSQLite(databaseName, where);
            var ds = db.SqlExecuteReader(sql);
            ds.Item1.Tables[0].Rows.RemoveAt(0);
            var ds2 = ds.Item1.Tables[1].Select();

            var aakey = "AUTOINCREMENT";
            foreach (DataRow dr in ds.Item1.Tables[0].Rows)
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

            var list = ds.Item1.Tables[0].ToModel<ColumnVM>();

            return list;
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableComment"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public bool SetTableComment(string tableName, string tableComment, string databaseName = null)
        {
            return true;
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="columnComment"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public bool SetColumnComment(string tableName, string columnName, string columnComment, string databaseName = null)
        {
            return true;
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="sql">脚本</param>
        /// <param name="databaseName">数据库名</param>
        /// <returns></returns>
        public Tuple<DataSet, DataSet, object> ExecuteSql(string sql, string databaseName = null)
        {
            var st = new SharedTimingVM();
            var er = db.SqlExecuteReader(sql, includeSchemaTable: true);

            return DataKitTo.AidExecuteSql(er, new List<string> { }, st);
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="listFieldName"></param>
        /// <param name="whereSql"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public Tuple<DataTable, int> GetData(string tableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, string databaseName = null)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
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
                            {tableName} {whereSql}
                        ORDER BY
                            {sort} {order}
                        LIMIT
                            {rows} OFFSET {(page - 1) * rows}";

            sql += $";select count(1) as total from {tableName} {whereSql}";

            var ds = db.SqlExecuteReader(sql);

            var dt = ds.Item1.Tables[0];
            _ = int.TryParse(ds.Item1.Tables[1].Rows[0][0].ToString(), out int total);

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

            var ds = db.SqlExecuteReader(sql);

            var dt = ds.Item1.Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            mo.CharSet = ds.Item1.Tables[1].Rows[0][0].ToString();

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