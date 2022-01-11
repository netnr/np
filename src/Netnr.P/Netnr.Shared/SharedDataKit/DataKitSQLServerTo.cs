#if Full || DataKit

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Netnr.SharedAdo;
using Msm = Microsoft.SqlServer.Management;

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
            var sql = Configs.GetDatabaseNameSQLServer();
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
        /// <param name="filterDatabaseName">数据库名</param>
        /// <returns></returns>
        public List<DatabaseVM> GetDatabase(string filterDatabaseName = null)
        {
            var where = string.Empty;
            if (!string.IsNullOrWhiteSpace(filterDatabaseName))
            {
                where = $"AND t1.name IN ('{string.Join("','", filterDatabaseName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetDatabaseSQLServer(where);
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
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var sql = Configs.GetTableSQLServer(databaseName);
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

            var ftn = new List<string>();
            if (filterTableName != null)
            {
                ftn = filterTableName.Split(',').ToList();
            }

            var dicDDL = new Dictionary<string, string>();

            var csb = new SqlConnectionStringBuilder(dbConnection.ConnectionString);
            Msm.Smo.Server smoServer = new(new Msm.Common.ServerConnection(csb.DataSource, csb.UserID, csb.Password));
            Msm.Smo.Database smoDatabase = smoServer.Databases[databaseName];

            Msm.Smo.Scripter smoScripter = new(smoServer);
            smoScripter.Options.Indexes = true;
            smoScripter.Options.WithDependencies = true;
            smoScripter.Options.DriAllConstraints = true;

            foreach (Msm.Smo.Table smoTable in smoDatabase.Tables)
            {
                if (smoTable.IsSystemObject || (filterTableName != null && !ftn.Contains(smoTable.Name)))
                {
                    continue;
                }

                var ddl = new List<string>();

                smoScripter.Options.ScriptDrops = true;
                var sc1 = smoScripter.Script(new Msm.Sdk.Sfc.Urn[] { smoTable.Urn });
                for (int i = 0; i < sc1.Count; i++)
                {
                    ddl.Add(sc1[i].ToString().Trim());
                }

                smoScripter.Options.ScriptDrops = false;
                var sc2 = smoScripter.Script(new Msm.Sdk.Sfc.Urn[] { smoTable.Urn });
                for (int i = 0; i < sc2.Count; i++)
                {
                    ddl.Add(sc2[i].ToString().Trim());
                }

                var ddlComment = new List<string>();
                //表注释
                var tableComment = smoTable.ExtendedProperties["MS_Description"];
                if (tableComment != null)
                {
                    ddlComment.Add($"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{tableComment.Value.ToString().OfSql()}', @level0type=N'SCHEMA', @level0name=N'{smoTable.Schema}', @level1type=N'TABLE', @level1name=N'{smoTable.Name}'");
                }

                //列注释
                foreach (Msm.Smo.Column smoColumn in smoTable.Columns)
                {
                    var columnComment = smoColumn.ExtendedProperties["MS_Description"];
                    if (columnComment != null)
                    {
                        ddlComment.Add($"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{columnComment.Value.ToString().OfSql()}', @level0type=N'SCHEMA', @level0name=N'{smoTable.Schema}', @level1type=N'TABLE', @level1name=N'{smoTable.Name}', @level2type=N'COLUMN', @level2name=N'{smoColumn.Name}'");
                    }
                }
                if (ddlComment.Count > 0)
                {
                    ddl.Add(string.Join(";\r\n", ddlComment));
                }

                dicDDL.Add(smoTable.Name, string.Join(";\r\n\r\n", ddl) + ";");
            }

            return dicDDL;
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
                where = $"AND o.name IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnSQLServer(databaseName, where);
            var ds = db.SqlExecuteReader(sql);

            var list = ds.Item1.Tables[0].ToModel<ColumnVM>();
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
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var sql = Configs.SetColumnCommentSQLServer(databaseName, tableName, columnName, columnComment);
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

            //消息
            var listInfo = new List<string>();
            var dbConn = (Microsoft.Data.SqlClient.SqlConnection)db.Connection;
            dbConn.InfoMessage += (s, e) =>
            {
                listInfo.Add(e.Message);
            };

            var er = db.SqlExecuteReader(sql, includeSchemaTable: true);

            return DataKitTo.AidExecuteSql(er, listInfo, st);
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
                        select
                            *
                        from(
                            select
                                row_number() over(
                                order by
                                    {sort} {order}
                                ) as NumId,{listFieldName}
                            from
                                [{databaseName}].dbo.[{tableName}] {whereSql}
                            ) as t
                        where
                            NumId between {((page - 1) * rows + 1)} and {(page * rows)}";

            sql += $";select count(1) as total from [{databaseName}].dbo.[{tableName}] {whereSql}";

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
                          SUBSTRING(@@VERSION, 1, CHARINDEX(' - ', @@VERSION, 1))
                          + ' '
                          + CONVERT(varchar(99), SERVERPROPERTY('Edition')) val
                        UNION ALL
                        SELECT
                          'Version' col,
                          SERVERPROPERTY('ProductVersion') val
                        UNION ALL
                        SELECT
                          'DirData' col,
                          SERVERPROPERTY('InstanceDefaultDataPath') val
                        UNION ALL
                        SELECT
                          'CharSet' col,
                          SERVERPROPERTY('Collation') val
                        UNION ALL
                        SELECT
                          'DateTime' col,
                          GETDATE() val
                        UNION ALL
                        SELECT
                          'MaxConn' col,
                          @@MAX_CONNECTIONS val
                        UNION ALL
                        SELECT
                          'CurrConn' col,
                          (SELECT
                            COUNT(dbid)
                          FROM sys.sysprocesses)
                          val
                        UNION ALL
                        SELECT
                          'IgnoreCase' col,
                          (CASE
                            WHEN 'a' = 'A' THEN 1
                            ELSE 0
                          END) val
                        UNION ALL
                        SELECT
                          'System' col,
                          REPLACE(RIGHT(@@VERSION, CHARINDEX(CHAR(10), REVERSE(@@VERSION), 2) - 2), CHAR(10), '') val;

                        EXEC Sp_configure 'remote query timeout';

                        EXEC master.dbo.Xp_instance_regread N'HKEY_LOCAL_MACHINE',
                                                            N'SYSTEM\CurrentControlSet\Control\TimeZoneInformation',
                                                            N'TimeZoneKeyName'
            ";

            var mo = new DEIVM();

            var ds = db.SqlExecuteReader(sql);
            var dt = ds.Item1.Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            if (int.TryParse(ds.Item1.Tables[1].Rows[0]["config_value"].ToString(), out int wt))
            {
                mo.TimeOut = wt;
            }
            mo.TimeZone = ds.Item1.Tables[2].Rows[0][1].ToString();

            return mo;
        }
    }
}

#endif