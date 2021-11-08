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
            return $@"
SELECT
  name AS DatabaseName
FROM
  sys.databases
ORDER BY
  name;
            ";
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseSQLServer()
        {
            return $@"
SELECT
  t1.name AS DatabaseName,
  t1.collation_name AS DatabaseCollation,
  CASE
    WHEN t1.database_id > 4 THEN 'USER'
    ELSE 'SYSTEM'
  END AS DatabaseClassify,
  (
    SELECT
      physical_name
    FROM
      sys.master_files f
    WHERE
      f.database_id = t1.database_id
      AND f.file_id = 1
  ) AS DatabasePath,
  (
    SELECT
      physical_name
    FROM
      sys.master_files f
    WHERE
      f.database_id = t1.database_id
      AND f.file_id = 2
  ) AS DatabaseLogPath,
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
ORDER BY
  t1.name;
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
USE [{ DatabaseName}];
SELECT
  t1.name AS TableName,
  ss.name AS TableSchema,
  t1.type_desc AS TableType,
  pt.[rows] AS TableRows,
  t2.pages AS TableDataLength,
  (t2.used_pages_count - t2.pages) AS TableIndexLength,
  t1.create_date AS TableCreateTime,
  t1.modify_date AS TableModifyTime,
  c1.TableCollation,
  ep.value AS TableComment
FROM
  sys.tables t1
  LEFT JOIN sys.schemas ss ON t1.schema_id = ss.schema_id
  LEFT JOIN sys.extended_properties ep ON ep.major_id = t1.object_id
  AND ep.minor_id = 0
  LEFT JOIN sys.partitions pt ON t1.object_id = pt.object_id
  AND pt.index_id <= 1
  LEFT JOIN (
    SELECT
      ps.object_id,
      SUM (ps.used_page_count) * 8 * 1024 AS used_pages_count,
      SUM (
        CASE
          WHEN (idx.index_id < 2) THEN (
            in_row_data_page_count + lob_used_page_count + row_overflow_used_page_count
          )
          ELSE lob_used_page_count + row_overflow_used_page_count
        END
      ) * 8 * 1024 AS pages
    FROM
      sys.dm_db_partition_stats ps
      JOIN sys.indexes idx ON ps.object_id = idx.object_id
      AND ps.index_id = idx.index_id
    GROUP BY
      ps.object_id
  ) t2 ON t1.object_id = t2.object_id
  LEFT JOIN (
    SELECT
      object_id,
      max(collation_name) TableCollation
    FROM
      sys.columns
    GROUP BY
      object_id
  ) c1 ON t1.object_id = c1.object_id
  ORDER BY t1.name
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
USE [{ DatabaseName}];
SELECT
  t1.[name] AS TableName,
  ep1.[value] AS TableComment,
  c1.[name] AS ColumnName,
  ColumnType = CASE
    WHEN COLUMNPROPERTY(c1.object_id, c1.[name], 'charmaxlen') IS NULL THEN t2.[name]
    ELSE t2.[name] + '(' + CONVERT(
      VARCHAR(50),
      COLUMNPROPERTY(c1.object_id, c1.[name], 'charmaxlen')
    ) + ')'
  END,
  t2.[name] AS DataType,
  [DataLength] = ISNULL(
    COLUMNPROPERTY(c1.object_id, c1.[name], 'charmaxlen'),
    c1.[precision]
  ),
  c1.[scale] AS DataScale,
  c1.column_id AS ColumnOrder,
  ipk.key_ordinal AS PrimaryKey,
  AutoIncr = CASE
    WHEN ic2.[name] IS NOT NULL THEN 'YES'
    ELSE NULL
  END,
  c1.is_nullable AS IsNullable,
  CONVERT(
    nvarchar(4000),
    OBJECT_DEFINITION(c1.default_object_id)
  ) AS ColumnDefault,
  ep2.[value] AS ColumnComment
FROM
  sys.tables t1
  LEFT JOIN sys.columns c1 ON t1.object_id = c1.object_id
  LEFT JOIN sys.extended_properties ep1 ON t1.object_id = ep1.major_id
  AND ep1.minor_id = 0
  LEFT JOIN sys.extended_properties ep2 ON ep2.class = 1
  AND ep2.major_id = t1.object_id
  AND ep2.minor_id = c1.column_id
  LEFT JOIN sys.types t2 ON c1.user_type_id = t2.user_type_id
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
  ) ipk ON ipk.object_id = c1.object_id
  AND c1.column_id = ipk.column_id
  LEFT JOIN sys.identity_columns ic2 ON c1.object_id = ic2.object_id
  AND c1.column_id = ic2.column_id
WHERE
  1 = 1 {Where}
ORDER BY
  t1.[name],
  c1.column_id;
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
USE [{ DatabaseName}];
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
USE [{ DatabaseName}];
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