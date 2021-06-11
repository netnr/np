#if Full || DataKit

using System.Data;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Netnr.SharedAdo;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitSQLServerTo : IDataKitTo
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
        public DataKitSQLServerTo(string conn)
        {
            db = new DbHelper(new SqlConnection(connectionString = conn));
        }

        /// <summary>
        /// 获取所有表信息的SQL脚本
        /// </summary>
        public static string GetTableSQL()
        {
            return @"
                    SELECT
                        a.name AS TableName,
                        b.value AS TableComment
                    FROM
                        sys.TABLES a
                        left join sys.extended_properties b ON b.major_id = a.object_id AND b.minor_id = 0
                    ORDER BY a.name
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
                        SELECT TableName = d.name,
                                TableComment = ISNULL(f.value, ''),
                                FieldName = a.name,
                                DataTypeLength = b.name + '(' + CONVERT(VARCHAR(10), COLUMNPROPERTY(a.id, a.name, 'PRECISION')) + ')',
                                DataType = b.name,
                                [DataLength] = COLUMNPROPERTY(a.id, a.name, 'PRECISION'),
                                DataScale = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0),
                                FieldOrder = a.colorder,
                                PrimaryKey = CASE
                                                WHEN EXISTS
                                                        (
                                                            SELECT 1
                                                            FROM sysobjects
                                                            WHERE xtype = 'PK'
                                                                AND name IN
                                                                    (
                                                                        SELECT name
                                                                        FROM sysindexes
                                                                        WHERE indid IN
                                                                                (
                                                                                    SELECT indid FROM sysindexkeys WHERE id = a.id AND colid = a.colid
                                                                                )
                                                                    )
                                                        ) THEN
                                                    'YES'
                                                ELSE
                                                    ''
                                            END,
                                AutoAdd = CASE
                                                WHEN i.name IS NULL THEN
                                                    ''
                                                ELSE
                                                    'YES'
                                            END,
                                NotNull = CASE
                                                WHEN a.isnullable = 1 THEN
                                                    ''
                                                ELSE
                                                    'YES'
                                            END,
                                DefaultValue = e.text,
                                FieldComment = ISNULL(g.[value], '')
                        FROM syscolumns a
                            LEFT JOIN systypes b
                                ON a.xtype = b.xusertype
                            INNER JOIN sysobjects d
                                ON a.id = d.id
                                    AND d.xtype = 'U'
                                    AND d.name != 'dtproperties'
                            LEFT JOIN syscomments e
                                ON a.cdefault = e.id
                            LEFT JOIN sys.extended_properties g
                                ON a.id = g.major_id
                                    AND a.colid = g.minor_id
                            LEFT JOIN sys.extended_properties f
                                ON d.id = f.major_id
                                    AND f.minor_id = 0
                            LEFT JOIN sys.identity_columns i
                                ON i.[object_id] = OBJECT_ID(d.name)
                                    AND i.name = a.name
                        WHERE 1 = 1 {sqlWhere}
                        ORDER BY d.name,
                                    a.colorder;
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
            return $@"
                        IF NOT EXISTS
                        (
                            SELECT A.name,
                                   C.value
                            FROM sys.tables A
                                INNER JOIN sys.extended_properties C
                                    ON C.major_id = A.object_id
                                       AND minor_id = 0
                            WHERE A.name = N'{dataTableName}'
                        )
                            EXEC sys.sp_addextendedproperty @name = N'MS_Description',
                                                            @value = N'{comment}',
                                                            @level0type = N'SCHEMA',
                                                            @level0name = N'dbo',
                                                            @level1type = N'TABLE',
                                                            @level1name = N'{dataTableName}';

                        EXEC sp_updateextendedproperty @name = N'MS_Description',
                                                       @value = N'{comment}',
                                                       @level0type = N'SCHEMA',
                                                       @level0name = N'dbo',
                                                       @level1type = N'TABLE',
                                                       @level1name = N'{dataTableName}';";
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
            return $@"
                       IF NOT EXISTS
                        (
                            SELECT C.value AS column_description
                            FROM sys.tables A
                                INNER JOIN sys.columns B
                                    ON B.object_id = A.object_id
                                INNER JOIN sys.extended_properties C
                                    ON C.major_id = B.object_id
                                        AND C.minor_id = B.column_id
                            WHERE A.name = N'{dataTableName}'
                                    AND B.name = N'{dataColumnName}'
                        )
                            EXEC sys.sp_addextendedproperty @name = N'MS_Description',
                                                            @value = N'{comment}',
                                                            @level0type = N'SCHEMA',
                                                            @level0name = N'dbo',
                                                            @level1type = N'TABLE',
                                                            @level1name = N'{dataTableName}',
                                                            @level2type = N'COLUMN',
                                                            @level2name = N'{dataColumnName}';

                        EXEC sp_updateextendedproperty @name = N'MS_Description',
                                                        @value = N'{comment}',
                                                        @level0type = N'SCHEMA',
                                                        @level0name = N'dbo',
                                                        @level1type = N'TABLE',
                                                        @level1name = N'{dataTableName}',
                                                        @level2type = N'COLUMN',
                                                        @level2name = N'{dataColumnName}';";
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <param name="listTableName">表名</param>
        /// <returns></returns>
        public List<TableColumnVM> GetColumn(List<string> listTableName = null)
        {
            var whereSql = string.Empty;

            if (listTableName?.Count > 0)
            {
                listTableName.ForEach(x => x = x.Replace("'", ""));

                whereSql = "AND d.name in ('" + string.Join("','", listTableName) + "')";
            }

            var sql = GetColumnSQL(whereSql);
            var ds = db.SqlQuery(sql);

            var list = ds.Tables[0].ToModel<TableColumnVM>();

            return list;
        }

        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <returns></returns>
        public List<TableNameVM> GetTable()
        {
            var sql = GetTableSQL();
            var ds = db.SqlQuery(sql);

            var list = ds.Tables[0].ToModel<TableNameVM>();

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
            TableComment ??= "";
            var sql = SetTableCommentSQL(TableName.Replace("'", ""), TableComment.Replace("'", "''"));
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
            FieldComment ??= "";
            var sql = SetColumnCommentSQL(TableName.Replace("'", ""), FieldName.Replace("'", ""), FieldComment.Replace("'", "''"));
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
                                {TableName} {whereSql}
                            ) as t
                        where
                            NumId between {((page - 1) * rows + 1)} and {(page * rows)}";

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