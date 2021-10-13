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
    public class DataKitOracleTo : IDataKitTo
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
        public DataKitOracleTo(DbConnection dbConn)
        {
            db = new DbHelper(dbConnection = dbConn);
        }

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
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public List<DatabaseVM> GetDatabase()
        {
            var sql = Configs.GetDatabaseOracle();
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

            var sql = Configs.GetTableOracle(DatabaseName);
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
                where = $"AND t1.TABLE_NAME IN ('{string.Join("','", filterTableName.Replace("'", "").Split(','))}')";
            }

            var sql = Configs.GetColumnOracle(DatabaseName, where);
            var ds = db.SqlQuery(sql, null, (dbc) =>
            {
                ((OracleCommand)dbc).InitialLONGFetchSize = -1;

                return dbc;
            });

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

            var sql = Configs.SetTableCommentOracle(DatabaseName, TableName, TableComment);
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

            var sql = Configs.SetColumnCommentOracle(DatabaseName, TableName, ColumnName, ColumnComment);
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

            var listSql = sql.Split(';').ToList();
            var queryKey = "select,with".Split(',').ToList();
            for (int i = 0; i < listSql.Count; i++)
            {
                var ls = listSql[i].ToString().Trim();
                if (queryKey.Any(k => ls.StartsWith(k, StringComparison.OrdinalIgnoreCase)))
                {
                    var dsout = db.SqlQuery(ls);
                    var dtout = dsout.Tables[0];
                    dsout.Tables.RemoveAt(0);
                    ds.Tables.Add(dtout);
                }
                else
                {
                    var dt = new DataTable();
                    dt.Columns.Add(new DataColumn("rows"));
                    var dr = dt.NewRow();
                    dr[0] = db.SqlExecute(ls);
                    ds.Tables.Add(dt);
                }
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
                                    {DatabaseName}.{TableName}
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

            sql += $";select count(1) as total from {DatabaseName}.{TableName} {countWhere}";

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

            var dt = db.SqlQuery(sql).Tables[0];
            mo = DataKitTo.TableToDEI(dt);

            return mo;
        }
    }
}

#endif