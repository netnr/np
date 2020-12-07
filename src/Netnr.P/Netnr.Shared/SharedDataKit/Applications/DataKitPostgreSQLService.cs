#if Full || DataKit

using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Netnr.SharedDataKit.Models;
using Netnr.SharedAdo;

namespace Netnr.SharedDataKit.Applications
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitPostgreSQLService : IDataKitService
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
        public DataKitPostgreSQLService(string conn)
        {
            db = new DbHelper(new Npgsql.NpgsqlConnection(connectionString = conn));
        }

        /// <summary>
        /// 获取所有表信息的SQL脚本
        /// </summary>
        public static string GetTableSQL()
        {
            return @"
                    SELECT
                        relname AS TableName,
                        Cast (
                        Obj_description (relfilenode, 'pg_class') AS VARCHAR
                        ) AS TableComment
                    FROM
                        pg_class C
                    WHERE
                        relkind = 'r'
                        AND relname NOT LIKE 'pg_%'
                        AND relname NOT LIKE 'sql_%'
                    ORDER BY
                        relname
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
                        C.relname AS TableName,
                        CAST(
                        obj_description(relfilenode, 'pg_class') AS VARCHAR
                        ) AS TableComment,
                        A.attname AS FieldName,
                        concat_ws(
                        '',
                        T.typname,
                        SUBSTRING(
                            format_type(A.atttypid, A.atttypmod)
                            FROM
                            '\(.*\)'
                        )
                        ) AS DataTypeLength,
                        T.typname AS DataType,
                        SUBSTRING(
                        format_type(A.atttypid, A.atttypmod)
                        FROM
                            '\d+'
                        ) AS DataLength,
                        REPLACE(
                        SUBSTRING(
                            format_type(A.atttypid, A.atttypmod)
                            FROM
                            '\,\d+'
                        ),
                        ',',
                        ''
                        ) AS DataScale,
                        A.attnum AS FieldOrder,
                        CASE
                        WHEN EXISTS (
                            SELECT
                            pg_attribute.attname
                            FROM
                            pg_constraint
                            INNER JOIN pg_class ON pg_constraint.conrelid = pg_class.oid
                            INNER JOIN pg_attribute ON pg_attribute.attrelid = pg_class.oid
                            AND pg_attribute.attnum = pg_constraint.conkey [1]
                            WHERE
                            relname = C.relname
                            AND attname = A.attname
                        ) THEN 'YES'
                        ELSE ''
                        END AS PrimaryKey,
                        CASE
                        A.attnotnull
                        WHEN 't' THEN 'YES'
                        ELSE ''
                        END AS NotNull,
                        D.adsrc AS DefaultValue,
                        col_description(A.attrelid, A.attnum) AS FieldComment
                    FROM
                        pg_class C
                        LEFT JOIN pg_attribute A ON A.attrelid = C.oid
                        LEFT JOIN pg_type T ON A.atttypid = T.oid
                        LEFT JOIN (
                        SELECT
                            T1.relname,
                            T2.attname,
                            pg_get_expr(T3.adbin,T3.adrelid) as adsrc
                        FROM
                            pg_class T1,
                            pg_attribute T2,
                            pg_attrdef T3
                        WHERE
                            T3.adrelid = T1.oid
                            AND adnum = T2.attnum
                            AND attrelid = T1.oid
                        ) D ON D.relname = C.relname
                        AND D.attname = A.attname
                    WHERE
                        C.relname IN (
                            SELECT
                            relname
                            FROM
                            pg_class
                            WHERE
                            relkind = 'r'
                            AND relname NOT LIKE 'pg_%'
                            AND relname NOT LIKE 'sql_%'
                        )
                        AND A.attnum > 0 {sqlWhere}
                    ORDER BY
                        C.relname,
                        A.attnum
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
            return $"COMMENT ON TABLE \"{dataTableName}\" IS '{comment}'";
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
            return $"COMMENT ON COLUMN \"{dataTableName}\".\"{dataColumnName}\" IS '{comment}'";
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

                whereSql = "AND C.relname IN ('" + string.Join("','", listTableName) + "')";
            }

            var sql = GetColumnSQL(whereSql);
            var ds = db.SqlQuery(sql);

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

            var ds = db.SqlQuery(@"
                                    SELECT version();
                                    show all;
                                    select now();
                                    select count(1) from pg_stat_activity
                                  ");
            var ts1 = ds.Tables[0].Rows[0][0].ToString().Split(',');
            var ts2 = ds.Tables[1].Select();

            mo.DeiName = ts1.FirstOrDefault();
            mo.DeiVersion = ts2.FirstOrDefault(x => x[0].ToString() == "server_version")?[1].ToString();
            mo.DeiCompile = ts1[1].ToString();
            mo.DeiDirInstall = ts2.FirstOrDefault(x => x[0].ToString() == "archive_command")?[1].ToString();
            if (mo.DeiDirInstall.Contains("main"))
            {
                mo.DeiDirInstall = mo.DeiDirInstall.Substring(0, mo.DeiDirInstall.IndexOf("main"));
            }
            mo.DeiDirData = mo.DeiDirInstall;
            mo.DeiCharSet = ts2.FirstOrDefault(x => x[0].ToString() == "server_encoding")?[1].ToString();
            mo.DeiTimeZone = ts2.FirstOrDefault(x => x[0].ToString() == "TimeZone")?[1].ToString();
            if (DateTime.TryParse(ds.Tables[2].Rows[0][0].ToString(), out DateTime now))
            {
                mo.DeiDateTime = now;
            }
            if (int.TryParse(ts2.FirstOrDefault(x => x[0].ToString() == "max_connections")?[1].ToString(), out int mc))
            {
                mo.DeiMaxConn = mc;
            }
            if (int.TryParse(ds.Tables[3].Rows[0][0].ToString(), out int cc))
            {
                mo.DeiCurrConn = cc;
            }
            if (int.TryParse(ts2.FirstOrDefault(x => x[0].ToString() == "statement_timeout")?[1].ToString(), out int wt))
            {
                mo.DeiTimeout = wt;
            }
            mo.DeiSystem = ts1.LastOrDefault();

            return mo;
        }
    }
}

#endif