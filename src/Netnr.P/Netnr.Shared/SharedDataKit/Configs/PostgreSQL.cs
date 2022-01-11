#if Full || DataKit

namespace Netnr.SharedDataKit
{
    public partial class Configs
    {
        /// <summary>
        /// 获取库名
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseNamePostgreSQL()
        {
            return $@"SELECT datname AS DatabaseName FROM pg_database ORDER BY datname";
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public static string GetDatabasePostgreSQL(string Where = null)
        {
            return $@"
SELECT
  t1.datname AS DatabaseName,
  pg_get_userbyid(t1.datdba) AS DatabaseOwner,
  t2.spcname AS DatabaseSpace,
  pg_encoding_to_char(t1.encoding) AS DatabaseCharset,
  t1.datcollate AS DatabaseCollation,
  (
    SELECT
      setting
    FROM
      pg_settings
    WHERE
      NAME = 'data_directory'
  ) AS DatabasePath,
  pg_catalog.pg_database_size(t1.oid) AS DatabaseDataLength
FROM
  pg_database t1
  LEFT JOIN pg_tablespace t2 ON t1.dattablespace = t2.oid
WHERE
  1 = 1 {Where}
ORDER BY
  t1.datname
            ";
        }

        /// <summary>
        /// 获取表
        /// 预估数据行：https://stackoverflow.com/questions/2596670
        /// </summary>
        /// <returns></returns>
        public static string GetTablePostgreSQL()
        {
            return $@"
SELECT
  t1.table_name AS TableName,
  t1.table_schema AS TableSchema,
  t3.tableowner AS TableOwner,
  t3.tablespace AS TableSpace,
  t1.table_type AS TableType,
  t2.n_live_tup AS TableRows,
  pg_relation_size(t2.relid) AS TableDataLength,
  pg_indexes_size(t2.relid) AS TableIndexLength,
  obj_description(t2.relid) AS TableComment
FROM
  information_schema.tables t1
  LEFT JOIN pg_stat_user_tables t2 ON t1.table_name = t2.relname
  AND t1.table_schema = t2.schemaname
  LEFT JOIN pg_tables t3 ON t1.table_name = t3.tablename
  AND t1.table_schema = t3.schemaname
WHERE
  t1.table_type = 'BASE TABLE'
  AND t1.table_schema NOT IN('pg_catalog', 'information_schema')
ORDER BY
  t1.table_name
            ";
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <returns></returns>
        public static string GetColumnPostgreSQL(string Where)
        {
            return $@"
SELECT
  t1.table_name AS TableName,
  t1.table_schema AS TableSchema,
  obj_description (
    format ('%s.""%s""', t1.table_schema, t1.table_name) :: regclass :: oid,
    'pg_class'
  ) AS TableComment,
  t1.column_name AS ColumnName,
  CASE
    WHEN t1.character_maximum_length IS NULL THEN t1.udt_name
    ELSE CONCAT(t1.udt_name, '(', t1.character_maximum_length, ')')
  END AS ColumnType,
  t1.udt_name AS DataType,
  COALESCE(t1.character_maximum_length, numeric_precision) AS DataLength,
  numeric_scale AS DataScale,
  t1.ordinal_position AS ColumnOrder,
  CASE
    t1.is_identity
    WHEN 'YES' THEN 1
    ELSE 0
  END AS AutoIncr,
  t2.ordinal_position AS PrimaryKey,
  CASE
    t1.is_nullable
    WHEN 'NO' THEN 1
    ELSE 0
  END AS IsNullable,
  t1.column_default AS ColumnDefault,
  col_description (
    format ('%s.""%s""', t1.table_schema, t1.table_name) :: regclass :: oid,
    t1.ordinal_position
  ) AS ColumnComment
FROM
  information_schema.columns t1
  LEFT JOIN information_schema.key_column_usage t2 ON t1.table_name = t2.table_name
  AND t1.table_schema = t2.table_schema
  AND t1.column_name = t2.column_name
WHERE
  t1.table_schema NOT IN('pg_catalog', 'information_schema') {Where}
ORDER BY
  t1.table_name,
  t1.ordinal_position
            ";
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        public static string SetTableCommentPostgreSQL(string TableName, string TableComment)
        {
            return $"COMMENT ON TABLE public.\"{TableName}\" IS '{TableComment.OfSql()}'";
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="ColumnName">列名</param>
        /// <param name="ColumnComment">列注释</param>
        /// <returns></returns>
        public static string SetColumnCommentPostgreSQL(string TableName, string ColumnName, string ColumnComment)
        {
            return $"COMMENT ON COLUMN public.\"{TableName}\".\"{ColumnName}\" IS '{ColumnComment.OfSql()}'";
        }

    }
}

#endif