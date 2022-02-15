#if Full || DataKit

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Netnr.SharedAdo;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// Oracle
    /// </summary>
    public class DataKitOracle : IDataKit
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
        public DataKitOracle(DbConnection dbConn)
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
            OracleConnectionStringBuilder builder = new(dbConnection.ConnectionString);
            return builder.UserID;
        }

        /// <summary>
        /// 获取库名
        /// </summary>
        /// <returns></returns>
        public List<string> GetDatabaseName()
        {
            var sql = Configs.GetDatabaseNameOracle();
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
                where = $"AND t1.USERNAME IN ('{string.Join("','", filterDatabaseName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetDatabaseOracle(where);
            var ds = db.SqlExecuteReader(sql);

            var list = ds.Item1.Tables[0].ToModel<DatabaseVM>();
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

            var sql = Configs.GetTableOracle(DatabaseName);
            var ds = db.SqlExecuteReader(sql);

            var list = ds.Item1.Tables[0].ToModel<TableVM>();
            return list;
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
                where = $"AND t1.TABLE_NAME IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnOracle(databaseName, where);
            var ds = db.SqlExecuteReader(sql, null, (dbc) =>
            {
                ((OracleCommand)dbc).InitialLONGFetchSize = -1;

                return dbc;
            });

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
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var sql = Configs.SetTableCommentOracle(databaseName, tableName, tableComment);
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

            var sql = Configs.SetColumnCommentOracle(databaseName, tableName, columnName, columnComment);
            _ = db.SqlExecuteNonQuery(sql);
            return true;
        }

        /// <summary>
        /// 表DDL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public string GetTableDDL(string tableName, string tableSchema = null, string databaseName = null)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultDatabaseName();
            }

            var sql = Configs.GetTableDDLOracle(databaseName, tableName);
            var ds = db.SqlExecuteReader(sql, func: cmd =>
            {
                var ocmd = (OracleCommand)cmd;

                //begin ... end;
                if (DbHelper.SqlParserBeginEnd(sql))
                {
                    //open:name for
                    var cursors = DbHelper.SqlParserCursors(sql);
                    foreach (var cursor in cursors)
                    {
                        ocmd.Parameters.Add(cursor, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output);
                    }
                }

                return cmd;
            }, includeSchemaTable: true);

            var ddlTable = ds.Item1.Tables[0].Rows[0][0].ToString().Trim();
            var ddlIndex = ds.Item1.Tables[1].Rows.Cast<DataRow>().Select(x => x[0].ToString().Trim() + ";");
            var ddlCheck = ds.Item1.Tables[2].Rows.Cast<DataRow>().Select(x => x[0].ToString().Trim() + ";");
            var ddlTableComment = ds.Item1.Tables[3].Rows[0][0].ToString().Trim();
            var ddlColumnComment = ds.Item1.Tables[4];

            var fullTableName = $"\"{databaseName}\".\"{tableName}\"";

            var ddl = new List<string>()
            {
                $"DROP TABLE {fullTableName};",
                $"{ddlTable};"
            };
            if (ddlIndex.Any())
            {
                ddl.Add("");
                ddl.AddRange(ddlIndex);
            }
            if (ddlCheck.Any())
            {
                ddl.Add("");
                ddl.AddRange(ddlCheck);
            }
            ddl.Add("");
            ddl.Add($"COMMENT ON TABLE {fullTableName} IS '{ddlTableComment.Replace("'", "''")}';");
            ddl.Add("");
            foreach (DataRow dr in ddlColumnComment.Rows)
            {
                ddl.Add($"COMMENT ON COLUMN {fullTableName}.\"{dr[0]}\" IS '{dr[1].ToString().Trim().Replace("'", "''")}';");
            }

            return string.Join("\r\n", ddl);
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

            var er = db.SqlExecuteReader(sql, func: cmd =>
            {
                var ocmd = (OracleCommand)cmd;
                ocmd.Connection.InfoMessage += (s, e) =>
                {
                    listInfo.Add(e.Message);
                };

                //begin ... end;
                if (DbHelper.SqlParserBeginEnd(sql))
                {
                    //open:name for
                    var cursors = DbHelper.SqlParserCursors(sql);
                    foreach (var cursor in cursors)
                    {
                        ocmd.Parameters.Add(cursor, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output);
                    }
                }

                return cmd;
            }, includeSchemaTable: true);

            return DataKit.ExecuteUnity(er, listInfo, st);
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

            var countWhere = string.Empty;
            if (string.IsNullOrWhiteSpace(whereSql))
            {
                whereSql = "1=1";
            }
            else
            {
                countWhere = "WHERE " + whereSql;
            }

            var sql = $@"
                        SELECT
                            *
                        FROM
                            (
                            SELECT
                                ROWNUM AS g_rowno,
                                g_t2. *
                            FROM
                                (
                                SELECT
                                    {listFieldName}
                                FROM
                                    {databaseName}.{tableName}
                                WHERE
                                    {whereSql}
                                ORDER BY
                                    {sort} {order}
                                ) g_t2
                            WHERE
                                ROWNUM <= {(page * rows)}
                            ) g_t3
                        WHERE
                            g_t3.g_rowno > {((page - 1) * rows + 1)}
             ";

            sql += $";select count(1) as total from {databaseName}.{tableName} {countWhere}";

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
                          (
                            SELECT
                              PRODUCT
                            FROM
                              (
                                SELECT
                                  ROW_NUMBER() OVER(
                                    ORDER BY
                                      PRODUCT DESC
                                  ) numid,
                                  A.*
                                FROM
                                  product_component_version A
                              )
                            WHERE
                              numid = 3
                          ) val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'Version' col,
                          (
                            SELECT
                              VERSION
                            FROM
                              product_component_version
                            WHERE
                              ROWNUM = 1
                          ) val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'Compile' col,
                          (
                            SELECT
                              STATUS
                            FROM
                              (
                                SELECT
                                  ROW_NUMBER() OVER(
                                    ORDER BY
                                      PRODUCT DESC
                                  ) numid,
                                  A.*
                                FROM
                                  product_component_version A
                              )
                            WHERE
                              numid = 3
                          ) val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'DirData' col,
                          (
                            SELECT
                              file_name
                            FROM
                              dba_data_files
                            WHERE
                              file_id = 1
                          ) val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'CharSet' col,
                          (
                            SELECT
                              VALUE
                            FROM
                              Nls_Database_Parameters
                            WHERE
                              PARAMETER = 'NLS_CHARACTERSET'
                          ) val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'TimeZone' col,
                          SESSIONTIMEZONE val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'DateTime' col,
                          TO_CHAR(SYSDATE, 'yyyy-mm-dd hh24:mi:ss') val
                        FROM
                          DUAL
                        UNION ALL
                        SELECT
                          'MaxConn',
                          (
                            SELECT
                              TO_CHAR(VALUE) AS MaxConn
                            FROM
                              v$parameter
                            WHERE
                              NAME = 'processes'
                          ) val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'CurrConn' col,
                          (
                            SELECT
                              TO_CHAR(COUNT(1))
                            FROM
                              v$process
                          ) val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'IgnoreCase' col,
                          (
                            CASE
                              WHEN 'a' = 'A' THEN '1'
                              ELSE '0'
                            END
                          ) val
                        FROM
                          dual
                        UNION ALL
                        SELECT
                          'System' col,
                          (
                            SELECT
                              REPLACE(PRODUCT, ': ', '')
                            FROM
                              (
                                SELECT
                                  ROW_NUMBER() OVER(
                                    ORDER BY
                                      PRODUCT DESC
                                  ) numid,
                                  A.*
                                FROM
                                  product_component_version A
                              )
                            WHERE
                              numid = 1
                          ) val
                        FROM
                          dual
                    ";

            var mo = new DEIVM();

            var dt = db.SqlExecuteReader(sql).Item1.Tables[0];
            mo = DataKit.TableToDEI(dt);

            return mo;
        }
    }
}

#endif