#if Full || DataKit

namespace Netnr.SharedDataKit
{
    public partial class Configs
    {
        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public static string GetDatabasePostgreSQL()
        {
            return $@"
SELECT
  db.datname AS DatabaseName,
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
  obj_description (relfilenode) AS TableComment,
  ab.attname AS ColumnName,
  concat_ws (
    '',
    tp.typname,
    SUBSTRING (
      format_type (ab.atttypid, ab.atttypmod)
      FROM
        '\(.*\)'
    )
  ) AS ColumnType,
  tp.typname AS DataType,
  SUBSTRING (
    format_type (ab.atttypid, ab.atttypmod)
    FROM
      '\d+'
  ) AS DataLength,
  REPLACE (
    SUBSTRING (
      format_type (ab.atttypid, ab.atttypmod)
      FROM
        '\,\d+'
    ),
    ',',
    ''
  ) AS DataScale,
  ab.attnum AS ColumnOrder,
  CASE
    WHEN EXISTS (
      SELECT
        pg_attribute.attname
      FROM
        pg_constraint
        INNER JOIN pg_class ON pg_constraint.conrelid = pg_class.oid
        INNER JOIN pg_attribute ON pg_attribute.attrelid = pg_class.oid
        AND pg_attribute.attnum = pg_constraint.conkey [ 1 ]
      WHERE
        relname = c1.relname
        AND attname = ab.attname
    ) THEN 'YES'
    ELSE ''
  END AS PrimaryKey,
  CASE
    ab.attnotnull
    WHEN 't' THEN 'YES'
    ELSE ''
  END AS NOTNULL,
  t1.adsrc AS ColumnDefault,
  col_description (ab.attrelid, ab.attnum) AS ColumnComment
FROM
  pg_class c1
  LEFT JOIN pg_attribute ab ON ab.attrelid = c1.oid
  LEFT JOIN pg_type tp ON ab.atttypid = tp.oid
  LEFT JOIN (
    SELECT
      p1.relname,
      p2.attname,
      pg_get_expr (p3.adbin, p3.adrelid) AS adsrc
    FROM
      pg_class p1,
      pg_attribute p2,
      pg_attrdef p3
    WHERE
      p3.adrelid = p1.oid
      AND adnum = p2.attnum
      AND attrelid = p1.oid
  ) t1 ON t1.relname = c1.relname
  AND t1.attname = ab.attname
WHERE
  c1.relname IN (
    SELECT
      tablename
    FROM
      pg_tables
    WHERE
      schemaname != 'pg_catalog'
      AND schemaname != 'information_schema'
  )
  AND ab.attnum > 0 {Where}
ORDER BY
  c1.relname,
  ab.attnum
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