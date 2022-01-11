#if Full || DataKit

namespace Netnr.SharedDataKit
{
    public partial class Configs
    {
        /// <summary>
        /// 获取库名
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseNameSQLServer()
        {
            return $@"SELECT name AS DatabaseName FROM sys.databases ORDER BY name";
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseSQLServer(string Where = null)
        {
            return $@"
SELECT
  t1.name AS DatabaseName,
  suser_sname(t1.owner_sid) AS DatabaseOwner,
  t1.collation_name AS DatabaseCollation,
  t2.physical_name AS DatabasePath,
  t3.physical_name AS DatabaseLogPath,
  (
    SELECT
      sum(CONVERT(bigint, f0.[size])) * 8 * 1024
    FROM
      sys.master_files f0
    WHERE
      f0.database_id = t1.database_id
      AND f0.[type] = 0
  ) AS DatabaseDataLength,
  (
    SELECT
      sum(CONVERT(bigint, f1.[size])) * 8 * 1024
    FROM
      sys.master_files f1
    WHERE
      f1.database_id = t1.database_id
      AND f1.[type] = 1
  ) AS DatabaseLogLength,
  t1.create_date AS DatabaseCreateTime
FROM
  sys.databases t1
  LEFT JOIN sys.master_files t2 ON t2.database_id = t1.database_id
  LEFT JOIN sys.master_files t3 ON t3.database_id = t1.database_id
WHERE
  t2.[type] = 0
  AND t3.[type] = 1 {Where}
ORDER BY
  t1.name
            ";
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static string GetTableSQLServer(string DatabaseName)
        {
            return $@"
USE [{DatabaseName}];
SELECT
  o.name AS TableName,
  SCHEMA_NAME(o.schema_id) AS TableSchema,
  CASE
    o.type
    WHEN 'U' THEN 'BASE TABLE'
    WHEN 'V' THEN 'VIEW'
    ELSE o.type
  END AS TableType,
  m1.TableRows,
  m1.TableDataLength,
  m2.TableIndexLength,
  o.create_date AS TableCreateTime,
  o.modify_date AS TableModifyTime,
  ep.value AS TableComment
FROM
  sys.objects o
  LEFT JOIN sys.extended_properties ep ON ep.major_id = o.object_id
  AND ep.minor_id = 0
  LEFT JOIN (
    SELECT
      t.object_id,
      p.rows AS TableRows,
      SUM(a.total_pages) * 8 * 1024 AS TableDataLength
    FROM
      sys.tables t
      INNER JOIN sys.indexes i ON t.object_id = i.object_id
      INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID
      AND i.index_id = p.index_id
      INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
    GROUP BY
      t.object_id,
      p.rows
  ) m1 ON o.object_id = m1.object_id
  LEFT JOIN (
    SELECT
      object_id,
      SUM([used_page_count]) * 8 * 1024 AS TableIndexLength
    FROM
      sys.dm_db_partition_stats
    GROUP BY
      object_id
  ) m2 ON o.object_id = m2.object_id
WHERE
  o.type IN ('U', 'V')
ORDER BY
  o.name
            ";
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="Where">条件</param>
        /// <returns></returns>
        public static string GetColumnSQLServer(string DatabaseName, string Where)
        {
            return $@"
USE [{DatabaseName}];
SELECT
  o.name AS TableName,
  SCHEMA_NAME(o.schema_id) AS TableSchema,
  ep1.value AS TableComment,
  c.name AS ColumnName,
  CASE
    WHEN c.system_type_id IN (48, 52, 56, 59, 60, 62, 106, 108, 122, 127) THEN t.name
    WHEN c.system_type_id IN (40, 41, 42, 43, 58, 61) THEN t.name
    ELSE CONCAT(t.name, '(', COLUMNPROPERTY(c.object_id, c.name, 'charmaxlen'), ')')
  END AS ColumnType,
  t.name AS DataType,
  CASE
    WHEN c.system_type_id IN (48, 52, 56, 59, 60, 62, 106, 108, 122, 127) THEN c.precision
    ELSE COLUMNPROPERTY(c.object_id, c.name, 'charmaxlen')
  END AS DataLength,
  CASE
    WHEN c.system_type_id IN (40, 41, 42, 43, 58, 61) THEN NULL
    ELSE ODBCSCALE(c.system_type_id, c.scale)
  END AS DataScale,
  c.column_id AS ColumnOrder,
  k.key_ordinal AS PrimaryKey,
  c.is_identity AS AutoIncr,
  c.is_nullable AS IsNullable,
  OBJECT_DEFINITION(c.default_object_id) AS ColumnDefault,
  ep2.value AS ColumnComment
FROM
  sys.objects o
  JOIN sys.columns c ON c.object_id = o.object_id
  LEFT JOIN sys.types t ON c.user_type_id = t.user_type_id
  LEFT JOIN (
    SELECT
      idx.object_id,
      ic1.key_ordinal,
      ic1.column_id
    FROM
      sys.indexes AS idx
      INNER JOIN sys.index_columns AS ic1 ON idx.object_id = ic1.object_id
      AND idx.index_id = ic1.index_id
    WHERE
      idx.is_primary_key = 1
  ) k ON c.object_id = k.object_id
  AND c.column_id = k.column_id
  LEFT JOIN sys.extended_properties ep1 ON c.object_id = ep1.major_id
  AND ep1.minor_id = 0
  LEFT JOIN sys.extended_properties ep2 ON ep2.major_id = c.object_id
  AND ep2.minor_id = c.column_id
WHERE
  o.type IN ('U', 'V') {Where}
ORDER BY
  o.name,
  c.column_id
            ";
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        public static string SetTableCommentSQLServer(string DatabaseName, string TableName, string TableComment)
        {
            return @$"
USE [{DatabaseName}];
IF NOT EXISTS (
  SELECT
    A.name,
    C.value
  FROM
    sys.tables A
    INNER JOIN sys.extended_properties C ON C.major_id = A.object_id
    AND minor_id = 0
  WHERE
    A.name = N'{TableName}'
) EXEC sys.sp_addextendedproperty @name = N'MS_Description',
@value = N'{TableComment.OfSql()}',
@level0type = N'SCHEMA',
@level0name = N'dbo',
@level1type = N'TABLE',
@level1name = N'{TableName}';
EXEC sp_updateextendedproperty @name = N'MS_Description',
@value = N'{TableComment.OfSql()}',
@level0type = N'SCHEMA',
@level0name = N'dbo',
@level1type = N'TABLE',
@level1name = N'{TableName}';
            ";
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableName">表名</param>
        /// <param name="ColumnName">列名</param>
        /// <param name="ColumnComment">列注释</param>
        /// <returns></returns>
        public static string SetColumnCommentSQLServer(string DatabaseName, string TableName, string ColumnName, string ColumnComment)
        {
            return @$"
USE [{DatabaseName}];
IF NOT EXISTS (
  SELECT
    C.value AS column_description
  FROM
    sys.tables A
    INNER JOIN sys.columns B ON B.object_id = A.object_id
    INNER JOIN sys.extended_properties C ON C.major_id = B.object_id
    AND C.minor_id = B.column_id
  WHERE
    A.name = N'{TableName}'
    AND B.name = N'{ColumnName}'
) EXEC sys.sp_addextendedproperty @name = N'MS_Description',
@value = N'{ColumnComment.OfSql()}',
@level0type = N'SCHEMA',
@level0name = N'dbo',
@level1type = N'TABLE',
@level1name = N'{TableName}',
@level2type = N'COLUMN',
@level2name = N'{ColumnName}';
EXEC sp_updateextendedproperty @name = N'MS_Description',
@value = N'{ColumnComment.OfSql()}',
@level0type = N'SCHEMA',
@level0name = N'dbo',
@level1type = N'TABLE',
@level1name = N'{TableName}',
@level2type = N'COLUMN',
@level2name = N'{ColumnName}';
            ";
        }

    }
}

#endif