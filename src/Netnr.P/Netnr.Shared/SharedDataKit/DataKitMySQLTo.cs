#if Full || DataKit

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Netnr.SharedAdo;
using System.Linq;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitMySQLTo : IDataKitTo
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
        public DataKitMySQLTo(DbConnection dbConn)
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
            var sql = Configs.GetDatabaseMySQL();
            var ds = db.SqlQuery(sql);

            var list = ds.Tables[0].ToModel<DatabaseVM>();
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

            var sql = Configs.GetTableMySQL(DatabaseName);
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

            var sql = Configs.GetTableDDLMySQL(DatabaseName, listTable);
            var ds = db.SqlQuery(sql);

            var dic = new Dictionary<string, string>();
            foreach (DataTable dt in ds.Tables)
            {
                var dr = dt.Rows[0];
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
                where = $"AND t1.TABLE_NAME IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnMySQL(DatabaseName, where);
            var ds = db.SqlQuery(sql);

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
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                DatabaseName = DefaultDatabaseName();
            }

            var sql = Configs.SetTableCommentMySQL(DatabaseName, TableName, TableComment);
            _ = db.SqlExecute(sql);
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
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                DatabaseName = DefaultDatabaseName();
            }

            var sql = Configs.SetColumnCommentMySQL(DatabaseName, TableName, ColumnName, ColumnComment);
            _ = db.SqlExecute(sql);
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

            var queryKey = "select,with,show".Split(',').ToList();
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
                        SELECT {listFieldName}
                        FROM `{DatabaseName}`.`{TableName}` {whereSql}
                        ORDER BY {sort} {order}
                        LIMIT {(page - 1) * rows},{rows}";

            sql += $";select count(1) as total from `{DatabaseName}`.`{TableName}` {whereSql}";

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
                            'Name' col,
                            @@version_comment val
                        UNION ALL
                        SELECT
                            'Version' col,
                            @@version val
                        UNION ALL
                        SELECT
                            'Compile' col,
                            @@version_compile_machine val
                        UNION ALL
                        SELECT
                            'DirInstall' col,
                            @@basedir val
                        UNION ALL
                        SELECT
                            'DirData' col,
                            @@datadir val
                        UNION ALL
                        SELECT
                            'DirTemp' col,
                            @@tmpdir val
                        UNION ALL
                        SELECT
                            'Engine' col,
                            @@default_storage_engine val
                        UNION ALL
                        SELECT
                            'CharSet' col,
                            @@collation_server val
                        UNION ALL
                        SELECT
                            'TimeZone' col,
                            @@system_time_zone val
                        UNION ALL
                        SELECT
                            'MaxConn' col,
                            @@max_connections val
                        UNION ALL
                        SELECT
                            'CurrConn' col,
                            count(1) val
                        FROM
                            information_schema.PROCESSLIST
                        WHERE
                            USER != 'event_scheduler'
                        UNION ALL
                        SELECT
                            'DateTime' col,
                            now() AS val
                        UNION ALL
                        SELECT
                            'TimeOut' col,
                            @@wait_timeout AS val
                        UNION ALL
                        SELECT
                            'IgnoreCase' col,
                            'a' = 'A' AS val
                        UNION ALL
                        SELECT
                            'System' col,
                            @@version_compile_os val
                        ";

            var mo = new DEIVM();

            var dt = db.SqlQuery(sql).Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            return mo;
        }
    }
}

#endif