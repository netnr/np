using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Netnr.DataKit.Application
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class DataKitMySQLService : IDataKitService
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="conn">连接字符串</param>
        public DataKitMySQLService(string conn)
        {
            connectionString = conn;
        }

        /// <summary>
        /// 获取数据库名称
        /// </summary>
        public string DataBaseName()
        {
            return connectionString.Split(';').ToList().FirstOrDefault(x => x.StartsWith("database=", StringComparison.OrdinalIgnoreCase)).Substring(9).Replace("'", "");
        }

        /// <summary>
        /// 获取所有表信息的SQL脚本
        /// </summary>
        public string GetTableSQL()
        {
            return $@"
                    SELECT
                        table_name AS TableName,
                        table_comment AS TableComment
                    FROM
                        information_schema.tables
                    WHERE
                        table_schema = '{DataBaseName()}'
                    ORDER BY table_name";
        }

        /// <summary>
        /// 获取所有列信息的SQL脚本
        /// </summary>
        /// <param name="sqlWhere">SQL条件</param>
        /// <returns></returns>
        public string GetColumnSQL(string sqlWhere)
        {
            return $@" 
                    SELECT
                        T.table_name AS TableName,
                        T.table_comment AS TableComment,
                        C.column_name AS FieldName,
                        C.column_type AS DataTypeLength,
                        C.data_type AS DataType,
                        CASE
                        WHEN C.character_maximum_length IS NOT NULL THEN C.character_maximum_length
                        WHEN C.numeric_precision IS NOT NULL THEN C.numeric_precision
                        ELSE NULL
                        end AS DataLength,
                        C.numeric_scale AS DataScale,
                        C.ordinal_position AS FieldOrder,
                        CASE
                        WHEN (
                            SELECT
                            Count(1)
                            FROM
                            information_schema.key_column_usage
                            WHERE
                            table_schema = T.table_schema
                            AND table_name = T.table_name
                            AND column_name = C.column_name
                            LIMIT
                            0, 1
                        ) = 0 THEN ''
                        ELSE 'YES'
                        end AS PrimaryKey,
                        CASE
                        WHEN C.EXTRA = 'auto_increment' THEN 'YES'
                        ELSE ''
                        END AS AutoAdd,
                        CASE
                        WHEN C.is_nullable = 'YES' THEN ''
                        ELSE 'YES'
                        end AS NotNull,
                        C.column_default AS DefaultValue,
                        C.column_comment AS FieldComment
                    FROM
                        information_schema.columns C
                        LEFT JOIN information_schema.tables T ON C.table_schema = T.table_schema
                        AND C.table_name = T.table_name
                    WHERE
                        T.table_schema = '{DataBaseName()}'
                        AND 1 = 1 {sqlWhere}
                    ORDER BY
                        T.table_name,
                        C.ordinal_position";
        }

        /// <summary>
        /// 设置表注释的SQL脚本
        /// </summary>
        /// <param name="dataTableName">表名</param>
        /// <param name="comment">注释内容</param>
        /// <returns></returns>
        public string SetTableCommentSQL(string dataTableName, string comment)
        {
            return $"ALTER TABLE `{dataTableName}` COMMENT '{comment}'";
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
            return $"ALTER TABLE `{dataTableName}` MODIFY COLUMN `{dataColumnName}` INT COMMENT '{comment}'";
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

                whereSql = "AND T.table_name in ('" + string.Join("','", listTableName) + "')";
            }

            var sql = GetColumnSQL(whereSql);
            var ds = new Data.MySQL.MySQLHelper(connectionString).Query(sql);

            var list = ds.Tables[0].ToModel<Model.DkTableColumn>();

            return list;
        }

        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <returns></returns>
        public List<Model.DkTableName> GetTable()
        {
            var ds = new Data.MySQL.MySQLHelper(connectionString).Query(GetTableSQL());

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
            var sql = SetTableCommentSQL(TableName.Replace("`", "``"), TableComment.Replace("'", "''"));
            new Data.MySQL.MySQLHelper(connectionString).ExecuteNonQuery(sql);
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
            var sql = SetColumnCommentSQL(TableName.Replace("`", "``"), FieldName.Replace("`", "``"), FieldComment.Replace("'", "''"));
            new Data.MySQL.MySQLHelper(connectionString).ExecuteNonQuery(sql);
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
                        SELECT
                            " + listFieldName + @"
                        FROM
                            " + TableName + @"
                        ORDER BY
                            " + sort + " " + order + @"
                        LIMIT
                            " + (page - 1) * rows + "," + rows;

            sql += ";select count(1) as total from " + TableName;

            var ds = new Data.MySQL.MySQLHelper(connectionString).Query(sql);

            var dt = ds.Tables[0];
            int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);
            return dt;
        }
    }
}
