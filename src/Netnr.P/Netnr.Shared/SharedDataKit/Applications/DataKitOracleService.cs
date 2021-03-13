#if Full || DataKit

using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Netnr.SharedDataKit.Models;
using Netnr.SharedAdo;

namespace Netnr.SharedDataKit.Applications
{
    /// <summary>
    /// Oracle
    /// </summary>
    public class DataKitOracleService : IDataKitService
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public DbHelper db;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="conn">连接字符串</param>
        public DataKitOracleService(string conn)
        {
            db = new DbHelper(new OracleConnection(connectionString = conn));
        }

        /// <summary>
        /// 获取所有表信息的SQL脚本
        /// </summary>
        public static string GetTableSQL()
        {
            return @"  
                    SELECT
                        A.table_name AS TableName,
                        B.comments AS TableComment
                    FROM
                        user_tables A,
                        user_tab_comments B
                    WHERE
                        A.table_name = B.table_name
                    ORDER BY A.table_name
                    ";
        }

        /// <summary>
        /// 获取所有列信息的SQL脚本
        /// </summary>
        /// <param name="sqlWhere">SQL条件</param>
        /// <returns></returns>
        public static string GetColumnSQL(string sqlWhere)
        {
            return $@"
                    SELECT
                        A.TABLE_NAME AS TableName,
                        B.COMMENTS AS TableComment,
                        C.COLUMN_NAME AS FieldName,
                        C.DATA_TYPE || '(' || CASE
                        WHEN C.CHARACTER_SET_NAME = 'NCHAR_CS' THEN C.DATA_LENGTH / 2
                        ELSE C.DATA_LENGTH
                        END || ')' AS DataTypeLength,
                        C.DATA_TYPE AS DataType,
                        CASE
                        WHEN C.CHARACTER_SET_NAME = 'NCHAR_CS' THEN C.DATA_LENGTH / 2
                        WHEN C.DATA_TYPE = 'NUMBER' THEN C.DATA_PRECISION
                        ELSE C.DATA_LENGTH
                        END AS DataLength,
                        C.DATA_SCALE AS DataScale,
                        C.COLUMN_ID AS FieldOrder,
                        DECODE(PK.COLUMN_NAME, C.COLUMN_NAME, 'YES', '') AS PrimaryKey,
                        DECODE(C.NULLABLE, 'N', 'YES', '') AS NotNull,
                        C.DATA_DEFAULT AS DefaultValue,
                        D.COMMENTS AS FieldComment
                    FROM
                        USER_TABLES A
                        LEFT JOIN USER_TAB_COMMENTS B ON A.TABLE_NAME = B.TABLE_NAME
                        LEFT JOIN USER_TAB_COLUMNS C ON A.TABLE_NAME = C.TABLE_NAME
                        LEFT JOIN USER_COL_COMMENTS D ON A.TABLE_NAME = D.TABLE_NAME
                        AND C.COLUMN_NAME = D.COLUMN_NAME
                        LEFT JOIN (
                        SELECT
                            E.TABLE_NAME,
                            F.COLUMN_NAME
                        FROM
                            USER_CONSTRAINTS E
                            LEFT JOIN USER_CONS_COLUMNS F ON E.TABLE_NAME = F.TABLE_NAME
                            AND E.CONSTRAINT_NAME = F.CONSTRAINT_NAME
                        WHERE
                            E.CONSTRAINT_TYPE = 'P'
                        ) PK ON PK.TABLE_NAME = A.TABLE_NAME
                        AND C.COLUMN_NAME = PK.COLUMN_NAME
                    WHERE
                        1 = 1 {sqlWhere} 
                    ORDER BY
                        A.TABLE_NAME,
                        C.COLUMN_ID
                ";
        }

        /// <summary>
        /// 设置表注释的SQL脚本
        /// </summary>
        /// <param name="dataTableName">表名</param>
        /// <param name="comment">注释内容</param>
        /// <returns></returns>
        public static string SetTableCommentSQL(string dataTableName, string comment)
        {
            return $"comment on table \"{dataTableName}\" is '{comment}'";
        }

        /// <summary>
        /// 设置列注释的SQL脚本
        /// </summary>
        /// <param name="dataTableName">表名</param>
        /// <param name="dataColumnName">列名</param>
        /// <param name="comment">注释内容</param>
        /// <returns></returns>
        public static string SetColumnCommentSQL(string dataTableName, string dataColumnName, string comment)
        {
            return $"comment on column \"{dataTableName}\".\"{dataColumnName}\" is '{comment}'";
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <param name="listTableName">表名</param>
        /// <returns></returns>
        public List<DkTableColumn> GetColumn(List<string> listTableName = null)
        {
            var whereSql = string.Empty;

            if (listTableName?.Count > 0)
            {
                listTableName.ForEach(x => x = x.Replace("'", ""));

                whereSql = "AND A.TABLE_NAME IN ('" + string.Join("','", listTableName) + "')";
            }

            var sql = GetColumnSQL(whereSql);
            var ds = db.SqlQuery(sql, null, (dbc) =>
            {
                ((OracleCommand)dbc).InitialLONGFetchSize = -1;
            });

            var list = ds.Tables[0].ToModel<DkTableColumn>();

            return list;
        }

        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <returns></returns>
        public List<DkTableName> GetTable()
        {
            var sql = GetTableSQL();
            var ds = db.SqlQuery(sql);

            var list = ds.Tables[0].ToModel<DkTableName>();

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
            var sql = SetTableCommentSQL(TableName.Replace("\"", ""), TableComment.Replace("'", "''"));
            _ = db.SqlExecute(sql);
            return true;
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
            var sql = SetColumnCommentSQL(TableName.Replace("\"", ""), FieldName.Replace("\"", ""), FieldComment.Replace("'", "''"));
            _ = db.SqlExecute(sql);
            return true;
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
        /// <param name="whereSql">条件</param>
        /// <param name="total">返回总条数</param>
        /// <returns></returns>
        public DataTable GetData(string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, out int total)
        {
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
                                    {TableName}
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

            sql += $";select count(1) as total from {TableName} {countWhere}";

            var ds = db.SqlQuery(sql);
            var dt = ds.Tables[0];
            _ = int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);

            return dt;
        }

        /// <summary>
        /// 查询数据库环境信息
        /// </summary>
        /// <returns></returns>
        public DkDEI GetDEI()
        {
            var mo = new DkDEI();

            var sql = "select * from product_component_version";
            sql += @"
                    SELECT file_name
                        FROM dba_data_files
                        WHERE file_id = 1
                    UNION ALL
                    SELECT value AS CharSet
                        FROM Nls_Database_Parameters
                        WHERE PARAMETER = 'NLS_CHARACTERSET'
                    UNION ALL
                    SELECT SESSIONTIMEZONE AS TimeZone
                        FROM DUAL
                    UNION ALL
                    SELECT TO_CHAR(SYSDATE, 'yyyy-mm-dd hh24:mi:ss') AS DateTime
                        FROM DUAL
                    UNION ALL
                    SELECT TO_CHAR(value) AS MaxConn
                        FROM v$parameter
                        WHERE name = 'processes'
                    UNION ALL
                    SELECT TO_CHAR(count(1)) AS CurrConn
                        FROM v$process
                    ";
            var ds = db.SqlQuery(sql);
            var ts1 = ds.Tables[0].Select();
            var ts2 = ds.Tables[1].Select();

            mo.DeiName = ts1[1][0].ToString().Trim();
            mo.DeiVersion = ts1[1][1].ToString();
            mo.DeiCompile = ts1[1][2].ToString();

            mo.DeiDirData = Path.GetDirectoryName(ts2[0][0].ToString());
            mo.DeiCharSet = ts2[1][0].ToString();
            mo.DeiTimeZone = ts2[2][0].ToString();
            if (DateTime.TryParse(ts2[3][0].ToString(), out DateTime now))
            {
                mo.DeiDateTime = now;
            }
            if (int.TryParse(ts2[4][0].ToString(), out int mc))
            {
                mo.DeiMaxConn = mc;
            }
            if (int.TryParse(ts2[5][0].ToString(), out int cc))
            {
                mo.DeiCurrConn = cc;
            }
            mo.DeiSystem = ts1[3][0].ToString().Trim().TrimEnd(':');

            return mo;
        }
    }
}

#endif