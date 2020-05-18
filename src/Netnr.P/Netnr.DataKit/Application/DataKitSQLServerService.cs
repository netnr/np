using System.Collections.Generic;
using System.Data;

namespace Netnr.DataKit.Application
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitSQLServerService : IDataKitService
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="conn">连接字符串</param>
        public DataKitSQLServerService(string conn)
        {
            connectionString = conn;
        }

        /// <summary>
        /// 获取所有表信息的SQL脚本
        /// </summary>
        public string GetTableSQL()
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
        public string GetColumnSQL(string sqlWhere)
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
        public string SetTableCommentSQL(string dataTableName, string comment)
        {
            return $"EXECUTE sp_updateextendedproperty 'MS_Description',N'{comment}','user','dbo','table','{dataTableName}',NULL,NULL";
        }

        /// <summary>
        /// 设置列注释的SQL脚本
        /// </summary>
        /// <param name="dataTableName">表名</param>
        /// <param name="dataColumnName">列名</param>
        /// <param name="comment">注释内容</param>
        /// <returns></returns>
        public string SetColumnCommentSQL(string dataTableName, string dataColumnName, string comment)
        {
            return $"EXECUTE sp_updateextendedproperty 'MS_Description',N'{comment}','user','dbo','table',N'{dataTableName}','column',N'{dataColumnName}'";
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <param name="listTableName">表名</param>
        /// <returns></returns>
        public List<Model.DkTableColumn> GetColumn(List<string> listTableName = null)
        {
            var whereSql = string.Empty;

            if (listTableName?.Count > 0)
            {
                listTableName.ForEach(x => x = x.Replace("'", ""));

                whereSql = "AND d.name in ('" + string.Join("','", listTableName) + "')";
            }

            var sql = GetColumnSQL(whereSql);
            var ds = new Data.SQLServer.SQLServerHelper(connectionString).Query(sql);

            var list = ds.Tables[0].ToModel<Model.DkTableColumn>();

            return list;
        }

        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <returns></returns>
        public List<Model.DkTableName> GetTable()
        {
            var ds = new Data.SQLServer.SQLServerHelper(connectionString).Query(GetTableSQL());

            var list = ds.Tables[0].ToModel<Model.DkTableName>();

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
            var sql = SetTableCommentSQL(TableName.Replace("'", ""), TableComment.Replace("'", "''"));
            new Data.SQLServer.SQLServerHelper(connectionString).ExecuteNonQuery(sql);
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
            var sql = SetColumnCommentSQL(TableName.Replace("'", ""), FieldName.Replace("'", ""), FieldComment.Replace("'", "''"));
            new Data.SQLServer.SQLServerHelper(connectionString).ExecuteNonQuery(sql);
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
        /// <param name="total">返回总条数</param>
        /// <returns></returns>
        public DataTable GetData(string TableName, int page, int rows, string sort, string order, string listFieldName, out int total)
        {
            var sql = @"
                        select
                            *
                        from(
                            select
                                row_number() over(
                                order by
                                    " + sort + " " + order + @"
                                ) as NumId," + listFieldName + @"
                            from
                                " + TableName + @"
                            ) as t
                        where
                            NumId between " + ((page - 1) * rows + 1) + " and " + (page * rows);

            sql += ";select count(1) as total from " + TableName;

            var ds = new Data.SQLServer.SQLServerHelper(connectionString).Query(sql);

            var dt = ds.Tables[0];
            int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);
            return dt;
        }
    }
}