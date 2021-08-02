#if Full || DataKit

namespace Netnr.SharedDataKit
{
    public partial class Configs
    {
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
  t2.physical_name AS DatabasePath,
  t3.physical_name AS DatabaseLogPath,
  t2.[size] * 8 * 1024 AS DatabaseDataLength,
  t3.[size] * 8 * 1024 AS DatabaseLogLength,
  t1.create_date AS DatabaseCreateTime
FROM
  sys.databases t1
  LEFT JOIN sys.master_files t2 ON t2.[type] = 0
  AND t1.database_id = t2.database_id
  LEFT JOIN sys.master_files t3 ON t3.[type] = 1
  AND t1.database_id = t3.database_id
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
  t1.name AS TableName,
  ep1.value AS TableComment,
  c1.name AS ColumnName,
  t2.name AS DataType,
  c1.max_length AS DataLength,
  c1.[scale] AS DataScale,
  c1.column_id AS ColumnOrder,
  idx.index_id AS PrimaryKey,
  AutoAdd = CASE
    WHEN ic2.name IS NULL THEN ''
    ELSE 'YES'
  END,
  c1.is_nullable AS NotNull,
  dc.definition AS ColumnDefault,
  ep2.value AS ColumnComment
FROM
  sys.tables t1
  LEFT JOIN sys.columns c1 ON t1.object_id = c1.object_id
  LEFT JOIN sys.extended_properties ep1 ON t1.object_id = ep1.major_id
  AND ep1.minor_id = 0
  LEFT JOIN sys.extended_properties ep2 ON ep2.class = 1
  AND ep2.major_id = t1.object_id
  AND ep2.minor_id = c1.column_id
  AND ep2.name = 'MS_Description'
  LEFT JOIN sys.types t2 ON c1.user_type_id = t2.user_type_id
  LEFT JOIN sys.index_columns ic1 ON t1.object_id = ic1.object_id
  AND c1.column_id = ic1.column_id
  LEFT JOIN sys.indexes idx ON ic1.object_id = idx.object_id
  AND ic1.index_id = idx.index_id
  LEFT JOIN sys.identity_columns ic2 ON c1.object_id = ic2.object_id
  AND c1.column_id = ic2.column_id
  LEFT JOIN sys.default_constraints dc ON dc.parent_object_id = c1.object_id
  AND dc.parent_column_id = c1.column_id
WHERE 1 = 1 {Where}
ORDER BY
  t1.name,
  c1.column_id
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