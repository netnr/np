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
            return $@"
SELECT
  datname AS DatabaseName
FROM
  pg_database
ORDER BY
  datname
            ";
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public static string GetDatabasePostgreSQL()
        {
            return $@"
SELECT
  db.datname AS DatabaseName,
  'DEFAULT' AS DatabaseClassify,
  tp.spcname AS DatabaseSpace,
  u1.usename AS DatabaseOwner,
  pg_encoding_to_char (db.ENCODING) AS DatabaseCharset,
  db.datcollate AS DatabaseCollation,
  (
    SELECT
      setting
    FROM
      pg_settings
    WHERE
      NAME = 'data_directory'
  ) AS DatabasePath,
  pg_database_size (db.datname) AS DatabaseDataLength
FROM
  pg_database db
  LEFT JOIN pg_tablespace tp ON db.dattablespace = tp.oid
  LEFT JOIN pg_user u1 ON u1.usesysid = db.datdba
ORDER BY
  db.datname
            ";
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <returns></returns>
        public static string GetTablePostgreSQL()
        {
            return $@"
SELECT
  t1.tablename AS TableName,
  t1.schemaname AS TableSchema,
  pg_table_size ('""' || t1.tablename || '""') AS TableDataLength,
  pg_indexes_size('""' || t1.tablename || '""') AS TableIndexLength,
 obj_description (c1.relfilenode) AS TableComment
FROM
  pg_tables t1
  LEFT JOIN pg_class c1 ON t1.tablename = c1.relname
WHERE
  t1.schemaname != 'pg_catalog'
  AND t1.schemaname != 'information_schema'
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
    c1.relname AS TableName,
    OBJ_DESCRIPTION(relfilenode) AS TableComment,
    ab.attname AS ColumnName,
    CONCAT(tp.typname, SUBSTRING(FORMAT_TYPE(ab.atttypid, ab.atttypmod)
            FROM '\(.*\)')) AS ColumnType,
    tp.typname AS DataType,
    SUBSTRING(FORMAT_TYPE(ab.atttypid, ab.atttypmod)
        FROM '\d+') AS DataLength,
    REPLACE(SUBSTRING(FORMAT_TYPE(ab.atttypid, ab.atttypmod)
            FROM '\,\d+'), ',', '') AS DataScale,
    ab.attnum AS ColumnOrder,
    ARRAY_LENGTH(REGEXP_SPLIT_TO_ARRAY(SPLIT_PART(t2.index_order, CONCAT('""', ab.attname, '""'), 1), ','), 1) - 1 AS PrimaryKey,
    CASE ab.attnotnull
    WHEN 't' THEN
        0
    ELSE
        1
    END AS IsNullable,
    t1.adsrc AS ColumnDefault,
    COL_DESCRIPTION(ab.attrelid, ab.attnum) AS ColumnComment
FROM
    pg_class c1
    LEFT JOIN pg_attribute ab ON ab.attrelid = c1.oid
    LEFT JOIN pg_type tp ON ab.atttypid = tp.oid
    LEFT JOIN(
        SELECT
            p1.relname,
            p2.attname,
            PG_GET_EXPR(p3.adbin, p3.adrelid) AS adsrc
        FROM
            pg_class p1,
            pg_attribute p2,
            pg_attrdef p3
        WHERE
            p3.adrelid = p1.oid
            AND adnum = p2.attnum
            AND attrelid = p1.oid) t1 ON t1.relname = c1.relname
    AND t1.attname = ab.attname
    LEFT JOIN(
        SELECT
            C.COLUMN_NAME,
            tc.CONSTRAINT_NAME,
            tc.TABLE_NAME,
            CONCAT(', ', SUBSTRING(pi.indexdef
                FROM '\(.*""')) index_order
        FROM
            information_schema.table_constraints tc
            JOIN information_schema.constraint_column_usage ccu USING(CONSTRAINT_SCHEMA, CONSTRAINT_NAME)
            JOIN information_schema.COLUMNS C ON C.table_schema = tc.CONSTRAINT_SCHEMA
                AND tc.TABLE_NAME = C.TABLE_NAME
                AND ccu.COLUMN_NAME = C.COLUMN_NAME
            JOIN pg_indexes pi ON tc.TABLE_NAME = pi.tablename
                AND tc.CONSTRAINT_NAME = pi.indexname
        WHERE
            tc.constraint_type = 'PRIMARY KEY') t2 ON t2.table_name = c1.relname
    AND t2.COLUMN_NAME = ab.attname
WHERE
    c1.relname IN(
        SELECT
            tablename
        FROM
            pg_tables
        WHERE
            schemaname != 'pg_catalog'
            AND schemaname != 'information_schema')
    AND ab.attnum > 0 {Where}
ORDER BY
    c1.relname,
    ab.attnum;
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