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
    public class DataKitSQLServerTo : IDataKitTo
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
        public DataKitSQLServerTo(DbConnection dbConn)
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
            var sql = Configs.GetDatabaseSQLServer();
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

            var sql = Configs.GetTableSQLServer(DatabaseName);
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
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                DatabaseName = DefaultDatabaseName();
            }

            var where = string.Empty;
            if (!string.IsNullOrWhiteSpace(filterTableName))
            {
                where = $"AND t1.name IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnSQLServer(DatabaseName, where);
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

            var sql = Configs.SetTableCommentSQLServer(DatabaseName, TableName, TableComment);
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

            var sql = Configs.SetColumnCommentSQLServer(DatabaseName, TableName, ColumnName, ColumnComment);
            _ = db.SqlExecute(sql);
            return true;
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
                        select
                            *
                        from(
                            select
                                row_number() over(
                                order by
                                    {sort} {order}
                                ) as NumId,{listFieldName}
                            from
                                [{DatabaseName}].dbo.[{TableName}] {whereSql}
                            ) as t
                        where
                            NumId between {((page - 1) * rows + 1)} and {(page * rows)}";

            sql += $";select count(1) as total from [{DatabaseName}].dbo.[{TableName}] {whereSql}";

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
                            'DeiName' col,
                            SUBSTRING (@@VERSION, 1, CHARINDEX(')', @@VERSION, 1)) + ' ' + CAST (SERVERPROPERTY('Edition') AS VARCHAR) val
                        UNION ALL
                        SELECT
                            'DeiVersion' col,
                            SERVERPROPERTY('ProductVersion') val
                        UNION ALL
                        SELECT
                            'DeiDirData' col,
                            SERVERPROPERTY('InstanceDefaultDataPath') val
                        UNION ALL
                        SELECT
                            'DeiCharSet' col,
                            SERVERPROPERTY('Collation') val
                        UNION ALL
                        SELECT
                            'DeiDateTime' col,
                            GETDATE() val
                        UNION ALL
                        SELECT
                            'DeiMaxConn' col,
                            @@MAX_CONNECTIONS val
                        UNION ALL
                        SELECT
                            'DeiCurrConn' col,
                            (
                            SELECT
                                COUNT (dbid)
                            FROM
                                sys.sysprocesses
                            ) val
                        UNION ALL
                        SELECT
                            'DeiIgnoreCase' col,
                            (
                            CASE
                                WHEN 'a' = 'A' THEN 1
                                ELSE 0
                            END
                            ) val
                        UNION ALL
                        SELECT
                            'DeiSystem' col,
                            REPLACE(
                            SUBSTRING (
                                @@VERSION,
                                CHARINDEX(' on ', @@VERSION, 0) + 4,
                                LEN(@@VERSION) - CHARINDEX(' on ', @@VERSION, 1)
                            ),
                            CHAR (10),
                            ''
                            ) val

                    ;EXEC sp_configure 'remote query timeout'
                    ;EXEC master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE',N'SYSTEM\CurrentControlSet\Control\TimeZoneInformation',N'TimeZoneKeyName'";

            var mo = new DEIVM();

            var ds = db.SqlQuery(sql);
            var dt = ds.Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            if (int.TryParse(ds.Tables[1].Rows[0]["config_value"].ToString(), out int wt))
            {
                mo.DeiTimeout = wt;
            }
            mo.DeiTimeZone = ds.Tables[2].Rows[0][1].ToString();

            return mo;
        }
    }
}

#endif