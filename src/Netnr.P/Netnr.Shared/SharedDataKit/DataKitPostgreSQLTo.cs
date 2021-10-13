#if Full || DataKit

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Netnr.SharedAdo;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitPostgreSQLTo : IDataKitTo
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
        public DataKitPostgreSQLTo(DbConnection dbConn)
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
            var sql = Configs.GetDatabasePostgreSQL();
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
            var sql = Configs.GetTablePostgreSQL();
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

            return null;
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="filterTableName"></param>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public List<ColumnVM> GetColumn(string filterTableName = null, string DatabaseName = null)
        {
            var where = string.Empty;
            if (!string.IsNullOrWhiteSpace(filterTableName))
            {
                where = $"AND c1.relname IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnPostgreSQL(where);
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
            var sql = Configs.SetTableCommentPostgreSQL(TableName, TableComment);
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
            var sql = Configs.SetColumnCommentPostgreSQL(TableName, ColumnName, ColumnComment);
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

            if (listFieldName != "*")
            {
                listFieldName = "\"" + string.Join("\",\"", listFieldName.Split(',')) + "\"";
            }

            TableName = "\"" + TableName + "\"";
            sort = "\"" + sort + "\"";

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
                          'Name' col,
                          split_part(split_part(VERSION (), ',', 1),' on ',1)
                        UNION ALL
                        SELECT
                          'Version' col,
                          (
                            SELECT
                              setting
                            FROM
                              pg_settings
                            WHERE
                              NAME = 'server_version'
                          ) val
                        UNION ALL
                        SELECT
                          'Compile' col,
                          split_part(VERSION (), ',', 2) val
                        UNION ALL
                        SELECT
                          'DirInstall' col,
                          (
                            SELECT
                              split_part(setting, 'main', 1)
                            FROM
                              pg_settings
                            WHERE
                              NAME = 'archive_command'
                          ) val
                        UNION ALL
                        SELECT
                          'DirData' col,
                          (
                            SELECT
                              setting
                            FROM
                              pg_settings
                            WHERE
                              NAME = 'data_directory'
                          ) val
                        UNION ALL
                        SELECT
                          'CharSet' col,
                          (
                            SELECT
                              setting
                            FROM
                              pg_settings
                            WHERE
                              NAME = 'server_encoding'
                          ) val
                        UNION ALL
                        SELECT
                          'TimeZone' col,
                          (
                            SELECT
                              setting
                            FROM
                              pg_settings
                            WHERE
                              NAME = 'TimeZone'
                          ) val
                        UNION ALL
                        SELECT
                          'DateTime' col,
                          to_char(now(), 'YYYY-MM-DD HH24:MI:SS.MS') val
                        UNION ALL
                        SELECT
                          'MaxConn' col,
                          (
                            SELECT
                              setting
                            FROM
                              pg_settings
                            WHERE
                              NAME = 'max_connections'
                          ) val
                        UNION ALL
                        SELECT
                          'CurrConn' col,
                          CAST (COUNT (1) AS VARCHAR) val
                        FROM
                          pg_stat_activity
                        UNION ALL
                        SELECT
                          'TimeOut' col,
                          (
                            SELECT
                              setting
                            FROM
                              pg_settings
                            WHERE
                              NAME = 'statement_timeout'
                          ) val
                        UNION ALL
                        SELECT
                          'IgnoreCase' col,
                          CASE
                            'a' = 'A'
                            WHEN 't' THEN '1'
                            ELSE '0'
                          END val
                        UNION ALL
                        SELECT
                          'System' col,
                          split_part(split_part(VERSION (), ',', 1), ' on ', 2) val
                        ";

            var mo = new DEIVM();

            var dt = db.SqlQuery(sql).Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            return mo;
        }
    }
}

#endif