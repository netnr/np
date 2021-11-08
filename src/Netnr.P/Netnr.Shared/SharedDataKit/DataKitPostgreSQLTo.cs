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
            var sql = Configs.GetDatabaseNamePostgreSQL();
            var dt = db.SqlExecuteReader(sql).Item1.Tables[0];

            var list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr[0].ToString());
            }

            return list;
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public List<DatabaseVM> GetDatabase()
        {
            var sql = Configs.GetDatabasePostgreSQL();
            var ds = db.SqlExecuteReader(sql);

            var list = ds.Item1.Tables[0].ToModel<DatabaseVM>();
            return list;
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public List<TableVM> GetTable(string databaseName = null)
        {
            var sql = Configs.GetTablePostgreSQL();
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

            return null;
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="filterTableName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public List<ColumnVM> GetColumn(string filterTableName = null, string databaseName = null)
        {
            var where = string.Empty;
            if (!string.IsNullOrWhiteSpace(filterTableName))
            {
                where = $"AND c1.relname IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnPostgreSQL(where);
            var ds = db.SqlExecuteReader(sql);

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
            var sql = Configs.SetTableCommentPostgreSQL(tableName, tableComment);
            _ = db.SqlExecuteNonQuery(sql);
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
            var sql = Configs.SetColumnCommentPostgreSQL(tableName, columnName, columnComment);
            _ = db.SqlExecuteNonQuery(sql);
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

            if (listFieldName != "*")
            {
                listFieldName = "\"" + string.Join("\",\"", listFieldName.Split(',')) + "\"";
            }

            tableName = "\"" + tableName + "\"";
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

            var dt = db.SqlExecuteReader(sql).Item1.Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            return mo;
        }
    }
}

#endif