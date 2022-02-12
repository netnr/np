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
    -- decimal/numeric
    WHEN c.system_type_id IN (106, 108) THEN CONCAT(t.name, '(', c.precision, ',', c.scale, ')')
    -- int/real/float/money
    WHEN c.system_type_id IN (48, 52, 56, 59, 60, 62, 122, 127) THEN t.name
    -- datetime/smalldatetime
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
FROM sys.objects o
JOIN sys.columns c
  ON c.object_id = o.object_id
LEFT JOIN sys.types t
  ON c.user_type_id = t.user_type_id
LEFT JOIN (SELECT
  idx.object_id,
  ic1.key_ordinal,
  ic1.column_id
FROM sys.indexes AS idx
INNER JOIN sys.index_columns AS ic1
  ON idx.object_id = ic1.object_id
  AND idx.index_id = ic1.index_id
WHERE idx.is_primary_key = 1) k
  ON c.object_id = k.object_id
  AND c.column_id = k.column_id
LEFT JOIN sys.extended_properties ep1
  ON c.object_id = ep1.major_id
  AND ep1.minor_id = 0
LEFT JOIN sys.extended_properties ep2
  ON ep2.major_id = c.object_id
  AND ep2.minor_id = c.column_id
WHERE o.type IN ('U', 'V') {Where}
ORDER BY o.name,
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

        /// <summary>
        /// 表DLL
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableSchema">模式</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static string GetTableDDLSQLServer(string DatabaseName, string TableSchema, string TableName)
        {
            // http://www.stormrage.com/SQLStuff/sp_GetDDL_Latest.txt
            var sql = $@"
USE [{DatabaseName}];

DECLARE @TBL VARCHAR(255) = '{TableSchema}.[{TableName}]'

DECLARE @TBLNAME varchar(200),
        @SCHEMANAME varchar(255),
        @STRINGLEN int,
        @TABLE_ID int,
        @FINALSQL varchar(max),
        @CONSTRAINTSQLS varchar(max),
        @CHECKCONSTSQLS varchar(max),
        @RULESCONSTSQLS varchar(max),
        @FKSQLS varchar(max),
        @TRIGGERSTATEMENT varchar(max),
        @EXTENDEDPROPERTIES varchar(max),
        @INDEXSQLS varchar(max),
        @MARKSYSTEMOBJECT varchar(max),
        @vbCrLf char(2),
        @ISSYSTEMOBJECT int,
        @input varchar(max),
        @ObjectTypeFound varchar(255),
        @ObjectDataTypeLen int;
-- ####################
-- INITIALIZE
-- ####################
SET @input = '';
--does the tablename contain a schema?
SET @vbCrLf = CHAR(10);
SELECT
  @SCHEMANAME = ISNULL(PARSENAME(@TBL, 2), 'dbo'),
  @TBLNAME = PARSENAME(@TBL, 1);
SELECT
  @TBLNAME = [OBJS].[name],
  @TABLE_ID = [OBJS].[object_id]
FROM [sys].[objects] [OBJS]
WHERE [OBJS].[type] IN ('S', 'U')
AND [OBJS].[name] <> 'dtproperties'
AND [OBJS].[name] = @TBLNAME
AND [OBJS].[schema_id] = SCHEMA_ID(@SCHEMANAME);
SELECT
  @ObjectDataTypeLen = MAX(LEN([name]))
FROM [sys].[types];
-- ####################
-- Valid Table, Continue Processing
-- ####################
SELECT
  @FINALSQL
  = 'IF OBJECT_ID(''' + QUOTENAME(@SCHEMANAME) + '.' + QUOTENAME(@TBLNAME) + ''') IS NOT NULL ' + @vbcrlf
  + 'DROP TABLE ' + QUOTENAME(@SCHEMANAME) + '.' + QUOTENAME(@TBLNAME) + ';' + @vbcrlf + @vbcrlf
  + 'CREATE TABLE ' + QUOTENAME(@SCHEMANAME) + '.' + QUOTENAME(@TBLNAME) + ' ( ';
--removed invalid code here which potentially selected wrong table--thanks David Grifiths @SSC!
SELECT
  @STRINGLEN = MAX(LEN([COLS].[name])) + 1
FROM [sys].[objects] [OBJS]
INNER JOIN [sys].[columns] [COLS]
  ON [OBJS].[object_id] = [COLS].[object_id]
  AND [OBJS].[object_id] = @TABLE_ID;
-- ####################
--Get the columns, their definitions and defaults.
-- ####################
SELECT
  @FINALSQL
  = @FINALSQL
  + CASE
    WHEN [COLS].[is_computed] = 1 THEN @vbCrLf + QUOTENAME([COLS].[name]) + ' AS '
      + ISNULL([CALC].[definition], '') + CASE
        WHEN [CALC].[is_persisted] = 1 THEN ' PERSISTED'
        ELSE ''
      END
    ELSE @vbCrLf + QUOTENAME([COLS].[name]) + ' ' + SPACE(@STRINGLEN - LEN([COLS].[name]))
      + UPPER(TYPE_NAME([COLS].[user_type_id]))
      + CASE
        -- data types with precision and scale  IE DECIMAL(18,3), NUMERIC(10,2)
        WHEN TYPE_NAME([COLS].[user_type_id]) IN ('decimal', 'numeric') THEN '(' + CONVERT(varchar, [COLS].[precision]) + ',' + CONVERT(varchar, [COLS].[scale]) + ') '
          + CASE
            WHEN COLUMNPROPERTY(@TABLE_ID, [COLS].[name], 'IsIdentity') = 0 THEN ''
            ELSE ' IDENTITY(' + CONVERT(varchar, ISNULL(IDENT_SEED(@TBLNAME), 1)) + ','
              + CONVERT(varchar, ISNULL(IDENT_INCR(@TBLNAME), 1)) + ')'
          END
          + CASE
            WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
            ELSE ' '
          END + CASE
            WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
            ELSE ' NULL'
          END -- data types with scale  IE datetime2(7),TIME(7)
        WHEN TYPE_NAME([COLS].[user_type_id]) IN ('datetime2', 'datetimeoffset', 'time') THEN CASE
            WHEN [COLS].[scale] < 7 THEN '(' + CONVERT(varchar, [COLS].[scale]) + ') '
            ELSE ' '
          END
          + CASE
            WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
            ELSE ' '
          END + CASE
            WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
            ELSE ' NULL'
          END --data types with no/precision/scale,IE  FLOAT
        WHEN TYPE_NAME([COLS].[user_type_id]) IN ('float') --,'real')
        THEN --addition: if 53, no need to specifically say (53), otherwise display it
          CASE
            WHEN [COLS].[precision] = 53 THEN CASE
                WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
                ELSE ' '
              END + CASE
                WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
                ELSE ' NULL'
              END
            ELSE '(' + CONVERT(varchar, [COLS].[precision]) + ') '
              + CASE
                WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
                ELSE ' '
              END + CASE
                WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
                ELSE ' NULL'
              END
          END --data type with max_length  	ie CHAR (44), VARCHAR(40), BINARY(5000),
        -- ####################
        -- COLLATE STATEMENTS
        -- personally i do not like collation statements,
        -- but included here to make it easy on those who do
        -- ####################
        WHEN TYPE_NAME([COLS].[user_type_id]) IN ('char', 'varchar', 'binary', 'varbinary') THEN CASE
            WHEN [COLS].[max_length] = -1 THEN '(max)' --collate to comment out when not desired
              + CASE
                WHEN COLS.collation_name IS NULL THEN ''
                ELSE ' COLLATE ' + COLS.collation_name
              END
              + CASE
                WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
                ELSE ' '
              END + CASE
                WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
                ELSE ' NULL'
              END
            ELSE '(' + CONVERT(varchar, [COLS].[max_length]) + ') ' --collate to comment out when not desired
              + CASE
                WHEN COLS.collation_name IS NULL THEN ''
                ELSE ' COLLATE ' + COLS.collation_name
              END
              + CASE
                WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
                ELSE ' '
              END + CASE
                WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
                ELSE ' NULL'
              END
          END --data type with max_length ( BUT DOUBLED) ie NCHAR(33), NVARCHAR(40)
        WHEN TYPE_NAME([COLS].[user_type_id]) IN ('nchar', 'nvarchar') THEN CASE
            WHEN [COLS].[max_length] = -1 THEN '(max)' --collate to comment out when not desired
              + CASE
                WHEN COLS.collation_name IS NULL THEN ''
                ELSE ' COLLATE ' + COLS.collation_name
              END
              + CASE
                WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
                ELSE ' '
              END + CASE
                WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
                ELSE ' NULL'
              END
            ELSE '(' + CONVERT(varchar, ([COLS].[max_length] / 2)) + ') '
              + CASE
                WHEN COLS.collation_name IS NULL THEN ''
                ELSE ' COLLATE ' + COLS.collation_name
              END
              + CASE
                WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
                ELSE ' '
              END + CASE
                WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
                ELSE ' NULL'
              END
          END
        WHEN TYPE_NAME([COLS].[user_type_id]) IN ('datetime', 'money', 'text', 'image', 'real') THEN ' '
          + CASE
            WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
            ELSE ' '
          END + CASE
            WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
            ELSE ' NULL'
          END --  other data type 	IE INT, DATETIME, MONEY, CUSTOM DATA TYPE,...
        ELSE ' '
          + CASE
            WHEN COLUMNPROPERTY(@TABLE_ID, [COLS].[name], 'IsIdentity') = 0 THEN ' '
            ELSE ' IDENTITY(' + CONVERT(varchar, ISNULL(IDENT_SEED(@TBLNAME), 1)) + ','
              + CONVERT(varchar, ISNULL(IDENT_INCR(@TBLNAME), 1)) + ')'
          END
          + CASE
            WHEN [COLS].[is_sparse] = 1 THEN ' sparse'
            ELSE ' '
          END + CASE
            WHEN [COLS].[is_nullable] = 0 THEN ' NOT NULL'
            ELSE ' NULL'
          END
      END
      + CASE
        WHEN [COLS].[default_object_id] = 0 THEN '' --ELSE ' DEFAULT '  + ISNULL(def.[definition] ,'')
        --optional section in case NAMED default constraints are needed:
        ELSE '  CONSTRAINT ' + QUOTENAME([DEF].[name]) + ' DEFAULT ' + ISNULL([DEF].[definition], '') --i thought it needed to be handled differently! NOT!
      END --CASE cdefault
  END --iscomputed
  + ','
FROM [sys].[columns] [COLS]
LEFT OUTER JOIN [sys].[default_constraints] [DEF]
  ON [COLS].[default_object_id] = [DEF].[object_id]
LEFT OUTER JOIN [sys].[computed_columns] [CALC]
  ON [COLS].[object_id] = [CALC].[object_id]
  AND [COLS].[column_id] = [CALC].[column_id]
WHERE [COLS].[object_id] = @TABLE_ID
ORDER BY [COLS].[column_id];
-- ####################
--used for formatting the rest of the constraints:
-- ####################
SELECT
  @STRINGLEN = MAX(LEN([OBJS].[name])) + 1
FROM [sys].[objects] [OBJS];
-- ####################
--PK/Unique Constraints and Indexes, using the 2005/08 INCLUDE syntax
-- ####################
DECLARE @Results TABLE (
  [SCHEMA_ID] int,
  [SCHEMA_NAME] varchar(255),
  [OBJECT_ID] int,
  [OBJECT_NAME] varchar(255),
  [index_id] int,
  [index_name] varchar(255),
  [ROWS] bigint,
  [SizeMB] decimal(19, 3),
  [IndexDepth] int,
  [TYPE] int,
  [type_desc] varchar(30),
  [fill_factor] int,
  [is_unique] int,
  [is_primary_key] int,
  [is_unique_constraint] int,
  [index_columns_key] varchar(max),
  [index_columns_include] varchar(max),
  [has_filter] bit,
  [filter_definition] varchar(max),
  [currentFilegroupName] varchar(128),
  [CurrentCompression] varchar(128)
);
INSERT INTO @Results
  SELECT
    [SCH].[schema_id],
    [SCH].[name] AS [SCHEMA_NAME],
    [OBJS].[object_id],
    [OBJS].[name] AS [OBJECT_NAME],
    [IDX].[index_id],
    ISNULL([IDX].[name], '---') AS [index_name],
    [partitions].[ROWS],
    [partitions].[SizeMB],
    INDEXPROPERTY([OBJS].[object_id], [IDX].[name], 'IndexDepth') AS [IndexDepth],
    [IDX].[type],
    [IDX].[type_desc],
    [IDX].[fill_factor],
    [IDX].[is_unique],
    [IDX].[is_primary_key],
    [IDX].[is_unique_constraint],
    ISNULL([Index_Columns].[index_columns_key], '---') AS [index_columns_key],
    ISNULL([Index_Columns].[index_columns_include], '---') AS [index_columns_include],
    [IDX].[has_filter],
    [IDX].[filter_definition],
    [filz].[name],
    ISNULL([p].[data_compression_desc], '')
  FROM [sys].[objects] [OBJS]
  INNER JOIN [sys].[schemas] [SCH]
    ON [OBJS].[schema_id] = [SCH].[schema_id]
  INNER JOIN [sys].[indexes] [IDX]
    ON [OBJS].[object_id] = [IDX].[object_id]
  INNER JOIN [sys].[filegroups] [filz]
    ON [IDX].[data_space_id] = [filz].[data_space_id]
  INNER JOIN [sys].[partitions] [p]
    ON [IDX].[object_id] = [p].[object_id]
    AND [IDX].[index_id] = [p].[index_id]
  INNER JOIN (SELECT
    [STATS].[object_id],
    [STATS].[index_id],
    SUM([STATS].[row_count]) AS [ROWS],
    CONVERT(
    numeric(19, 3),
    CONVERT(
    numeric(19, 3),
    SUM([STATS].[in_row_reserved_page_count] + [STATS].[lob_reserved_page_count]
    + [STATS].[row_overflow_reserved_page_count])) / CONVERT(numeric(19, 3), 128)) AS [SizeMB]
  FROM [sys].[dm_db_partition_stats] [STATS]
  GROUP BY [STATS].[object_id],
           [STATS].[index_id]) AS [partitions]
    ON [IDX].[object_id] = [partitions].[OBJECT_ID]
    AND [IDX].[index_id] = [partitions].[index_id]
  CROSS APPLY (SELECT
    LEFT([Index_Columns].[index_columns_key], LEN([Index_Columns].[index_columns_key]) - 1) AS [index_columns_key],
    LEFT([Index_Columns].[index_columns_include], LEN([Index_Columns].[index_columns_include]) - 1) AS [index_columns_include]
  FROM (SELECT (SELECT
                 QUOTENAME([COLS].[name])
                 + CASE
                   WHEN [IXCOLS].[is_descending_key] = 0 THEN ' asc'
                   ELSE ' desc'
                 END + ',' + ' '
               FROM [sys].[index_columns] [IXCOLS]
               INNER JOIN [sys].[columns] [COLS]
                 ON [IXCOLS].[column_id] = [COLS].[column_id]
                 AND [IXCOLS].[object_id] = [COLS].[object_id]
               WHERE [IXCOLS].[is_included_column] = 0
               AND [IDX].[object_id] = [IXCOLS].[object_id]
               AND [IDX].[index_id] = [IXCOLS].[index_id]
               ORDER BY [IXCOLS].[key_ordinal]
               FOR xml PATH (''))
               AS [index_columns_key],
               (SELECT
                 QUOTENAME([COLS].[name]) + ',' + ' '
               FROM [sys].[index_columns] [IXCOLS]
               INNER JOIN [sys].[columns] [COLS]
                 ON [IXCOLS].[column_id] = [COLS].[column_id]
                 AND [IXCOLS].[object_id] = [COLS].[object_id]
               WHERE [IXCOLS].[is_included_column] = 1
               AND [IDX].[object_id] = [IXCOLS].[object_id]
               AND [IDX].[index_id] = [IXCOLS].[index_id]
               ORDER BY [IXCOLS].[index_column_id]
               FOR xml PATH (''))
               AS [index_columns_include]) AS [Index_Columns]) AS [Index_Columns]
  WHERE [SCH].[name] LIKE CASE
    WHEN @SCHEMANAME = '' COLLATE Chinese_PRC_CI_AS THEN [SCH].[name]
    ELSE @SCHEMANAME
  END
  AND [OBJS].[name] LIKE CASE
    WHEN @TBLNAME = '' COLLATE Chinese_PRC_CI_AS THEN [OBJS].[name]
    ELSE @TBLNAME
  END
  ORDER BY [SCH].[name],
  [OBJS].[name],
  [IDX].[name];
--@Results table has both PK,s Uniques and indexes in thme...pull them out for adding to funal results:
SET @CONSTRAINTSQLS = '';
SET @INDEXSQLS = '';
-- ####################
--constriants
-- ####################
SELECT
  @CONSTRAINTSQLS
  = @CONSTRAINTSQLS
  + CASE
    WHEN [is_primary_key] = 1 OR
      [is_unique] = 1 THEN @vbCrLf + 'CONSTRAINT   ' COLLATE Chinese_PRC_CI_AS + QUOTENAME([index_name]) + ' '
      + CASE
        WHEN [is_primary_key] = 1 THEN ' PRIMARY KEY '
        ELSE CASE
            WHEN [is_unique] = 1 THEN ' UNIQUE      '
            ELSE ''
          END
      END + [type_desc] + CASE
        WHEN [type_desc] = 'NONCLUSTERED' THEN ''
        ELSE '   '
      END + ' (' + [index_columns_key]
      + ')' + CASE
        WHEN [index_columns_include] <> '---' THEN ' INCLUDE (' + [index_columns_include] + ')'
        ELSE ''
      END + CASE
        WHEN [has_filter] = 1 THEN ' ' -- + [filter_definition]
        ELSE ' '
      END
      + CASE
        WHEN [fill_factor] <> 0 OR
          [CurrentCompression] <> 'NONE' THEN ' WITH ('
          + CASE
            WHEN [fill_factor] <> 0 THEN 'FILLFACTOR = ' + CONVERT(varchar(30), [fill_factor])
            ELSE ''
          END
          + CASE
            WHEN [fill_factor] <> 0 AND
              [CurrentCompression] <> 'NONE' THEN ',DATA_COMPRESSION = ' + [CurrentCompression] + ' '
            WHEN [fill_factor] <> 0 AND
              [CurrentCompression] = 'NONE' THEN ''
            WHEN [fill_factor] = 0 AND
              [CurrentCompression] <> 'NONE' THEN 'DATA_COMPRESSION = ' + [CurrentCompression] + ' '
            ELSE ''
          END + ')'
        ELSE ''
      END
    ELSE ''
  END + ','
FROM @RESULTS
WHERE [type_desc] != 'HEAP'
AND [is_primary_key] = 1
OR [is_unique] = 1
ORDER BY [is_primary_key] DESC,
[is_unique] DESC;
-- ####################
--indexes
-- ####################
SELECT
  @INDEXSQLS
  = @INDEXSQLS
  + CASE
    WHEN [is_primary_key] = 0 OR
      [is_unique] = 0 THEN @vbCrLf + 'CREATE ' COLLATE Chinese_PRC_CI_AS + [type_desc] + ' INDEX ' COLLATE Chinese_PRC_CI_AS
      + QUOTENAME([index_name]) + ' ' + @vbCrLf + '   ON ' COLLATE Chinese_PRC_CI_AS
      + QUOTENAME([schema_name]) + '.' + QUOTENAME([OBJECT_NAME])
      + CASE
        WHEN [CurrentCompression] = 'COLUMNSTORE' COLLATE Chinese_PRC_CI_AS THEN ' (' + [index_columns_include] + ')'
        ELSE ' (' + [index_columns_key] + ')'
      END
      + CASE
        WHEN [CurrentCompression] = 'COLUMNSTORE' COLLATE Chinese_PRC_CI_AS THEN '' COLLATE Chinese_PRC_CI_AS
        ELSE CASE
            WHEN [index_columns_include] <> '---' THEN @vbCrLf + '   INCLUDE (' COLLATE Chinese_PRC_CI_AS + [index_columns_include]
              + ')' COLLATE Chinese_PRC_CI_AS
            ELSE '' COLLATE Chinese_PRC_CI_AS
          END
      END --2008 filtered indexes syntax
      + CASE
        WHEN [has_filter] = 1 THEN @vbCrLf + '   WHERE ' COLLATE Chinese_PRC_CI_AS + [filter_definition]
        ELSE ''
      END
      + CASE
        WHEN [fill_factor] <> 0 OR
          [CurrentCompression] <> 'NONE' COLLATE Chinese_PRC_CI_AS THEN ' WITH (' COLLATE Chinese_PRC_CI_AS
          + CASE
            WHEN [fill_factor] <> 0 THEN 'FILLFACTOR = ' COLLATE Chinese_PRC_CI_AS + CONVERT(varchar(30), [fill_factor])
            ELSE ''
          END
          + CASE
            WHEN [fill_factor] <> 0 AND
              [CurrentCompression] <> 'NONE' THEN ',DATA_COMPRESSION = ' + [CurrentCompression] + ' '
            WHEN [fill_factor] <> 0 AND
              [CurrentCompression] = 'NONE' THEN ''
            WHEN [fill_factor] = 0 AND
              [CurrentCompression] <> 'NONE' THEN 'DATA_COMPRESSION = ' + [CurrentCompression] + ' '
            ELSE ''
          END + ')'
        ELSE ''
      END
  END
FROM @RESULTS
WHERE [type_desc] != 'HEAP'
AND [is_primary_key] = 0
AND [is_unique] = 0
ORDER BY [is_primary_key] DESC,
[is_unique] DESC;
IF @INDEXSQLS <> '' COLLATE Chinese_PRC_CI_AS
  SET @INDEXSQLS = @vbCrLf + ';' COLLATE Chinese_PRC_CI_AS + @vbCrLf + @INDEXSQLS;
-- ####################
--CHECK Constraints
-- ####################
SET @CHECKCONSTSQLS = '' COLLATE Chinese_PRC_CI_AS;
SELECT
  @CHECKCONSTSQLS
  = @CHECKCONSTSQLS + @vbCrLf
  + ISNULL(
  'CONSTRAINT ' + QUOTENAME([OBJS].[name]) + ' CHECK '
  + ISNULL([CHECKS].[definition], '') + ',',
  '')
FROM [sys].[objects] [OBJS]
INNER JOIN [sys].[check_constraints] [CHECKS]
  ON [OBJS].[object_id] = [CHECKS].[object_id]
WHERE [OBJS].[type] = 'C'
AND [OBJS].[parent_object_id] = @TABLE_ID;
-- ####################
--FOREIGN KEYS
-- ####################
SET @FKSQLS = '';
SELECT
  @FKSQLS = @FKSQLS + @vbCrLf + [MyAlias].[Command]
FROM (SELECT DISTINCT --FK must be added AFTER the PK/unique constraints are added back.
  850 AS [ExecutionOrder],
  'CONSTRAINT ' + QUOTENAME([conz].[name]) + ' FOREIGN KEY (' + [ChildCollection].[ChildColumns]
  + ') REFERENCES ' + QUOTENAME(SCHEMA_NAME([conz].[schema_id])) + '.'
  + QUOTENAME(OBJECT_NAME([conz].[referenced_object_id])) + ' (' + [ParentCollection].[ParentColumns]
  + ') ' + CASE [conz].[update_referential_action]
    WHEN 0 THEN '' --' ON UPDATE NO ACTION '
    WHEN 1 THEN ' ON UPDATE CASCADE '
    WHEN 2 THEN ' ON UPDATE SET NULL '
    ELSE ' ON UPDATE SET DEFAULT '
  END + CASE [conz].[delete_referential_action]
    WHEN 0 THEN '' --' ON DELETE NO ACTION '
    WHEN 1 THEN ' ON DELETE CASCADE '
    WHEN 2 THEN ' ON DELETE SET NULL '
    ELSE ' ON DELETE SET DEFAULT '
  END
  + CASE [conz].[is_not_for_replication]
    WHEN 1 THEN ' NOT FOR REPLICATION '
    ELSE ''
  END + ',' AS [Command]
FROM [sys].[foreign_keys] [conz]
INNER JOIN [sys].[foreign_key_columns] [colz]
  ON [conz].[object_id] = [colz].[constraint_object_id]
INNER JOIN (
--gets my child tables column names
SELECT
  [conz].[name],
  --technically, FK's can contain up to 16 columns, but real life is often a single column. coding here is for all columns
  [ChildColumns] = STUFF((SELECT
    ',' + QUOTENAME([REFZ].[name])
  FROM [sys].[foreign_key_columns] [fkcolz]
  INNER JOIN [sys].[columns] [REFZ]
    ON [fkcolz].[parent_object_id] = [REFZ].[object_id]
    AND [fkcolz].[parent_column_id] = [REFZ].[column_id]
  WHERE [fkcolz].[parent_object_id] = [conz].[parent_object_id]
  AND [fkcolz].[constraint_object_id] = [conz].[object_id]
  ORDER BY [fkcolz].[constraint_column_id]
  FOR xml PATH (''), TYPE)
  .[value]('.', 'varchar(max)'),
  1,
  1,
  '')
FROM [sys].[foreign_keys] [conz]
INNER JOIN [sys].[foreign_key_columns] [colz]
  ON [conz].[object_id] = [colz].[constraint_object_id]
WHERE [conz].[parent_object_id] = @TABLE_ID
GROUP BY [conz].[name],
         [conz].[parent_object_id],
         --- without GROUP BY multiple rows are returned
         [conz].[object_id]) [ChildCollection]
  ON [conz].[name] = [ChildCollection].[name]
INNER JOIN (
--gets the parent tables column names for the FK reference
SELECT
  [conz].[name],
  [ParentColumns] = STUFF((SELECT
    ',' + [REFZ].[name]
  FROM [sys].[foreign_key_columns] [fkcolz]
  INNER JOIN [sys].[columns] [REFZ]
    ON [fkcolz].[referenced_object_id] = [REFZ].[object_id]
    AND [fkcolz].[referenced_column_id] = [REFZ].[column_id]
  WHERE [fkcolz].[referenced_object_id] = [conz].[referenced_object_id]
  AND [fkcolz].[constraint_object_id] = [conz].[object_id]
  ORDER BY [fkcolz].[constraint_column_id]
  FOR xml PATH (''), TYPE)
  .[value]('.', 'varchar(max)'),
  1,
  1,
  '')
FROM [sys].[foreign_keys] [conz]
INNER JOIN [sys].[foreign_key_columns] [colz]
  ON [conz].[object_id] = [colz].[constraint_object_id] -- AND colz.parent_column_id
GROUP BY [conz].[name],
         [conz].[referenced_object_id],
         --- without GROUP BY multiple rows are returned
         [conz].[object_id]) [ParentCollection]
  ON [conz].[name] = [ParentCollection].[name]) [MyAlias];
-- ####################
--RULES
-- ####################
SET @RULESCONSTSQLS = '';
SELECT
  @RULESCONSTSQLS
  = @RULESCONSTSQLS
  + ISNULL(
  @vbCrLf
  + 'if not exists(SELECT [name] FROM sys.objects WHERE TYPE=''R'' AND schema_id = ' COLLATE Chinese_PRC_CI_AS
  + CONVERT(varchar(30), [OBJS].[schema_id]) + ' AND [name] = ''' COLLATE Chinese_PRC_CI_AS
  + QUOTENAME(OBJECT_NAME([COLS].[rule_object_id])) + ''')' COLLATE Chinese_PRC_CI_AS + @vbCrLf
  + [MODS].[definition] + @vbCrLf + ';' COLLATE Chinese_PRC_CI_AS + @vbCrLf + 'EXEC sp_binderule  '
  + QUOTENAME([OBJS].[name]) + ', ''' + QUOTENAME(OBJECT_NAME([COLS].[object_id])) + '.'
  + QUOTENAME([COLS].[name]) + '''' COLLATE Chinese_PRC_CI_AS + @vbCrLf + ';' COLLATE Chinese_PRC_CI_AS,
  '')
FROM [sys].[columns] [COLS]
INNER JOIN [sys].[objects] [OBJS]
  ON [OBJS].[object_id] = [COLS].[object_id]
INNER JOIN [sys].[sql_modules] [MODS]
  ON [COLS].[rule_object_id] = [MODS].[object_id]
WHERE [COLS].[rule_object_id] <> 0
AND [COLS].[object_id] = @TABLE_ID;
-- ####################
--TRIGGERS
-- ####################
SET @TRIGGERSTATEMENT = '';
SELECT
  @TRIGGERSTATEMENT = @TRIGGERSTATEMENT + @vbCrLf + [MODS].[definition] + @vbCrLf + ';'
FROM [sys].[sql_modules] [MODS]
WHERE [MODS].[object_id] IN (SELECT
  [OBJS].[object_id]
FROM [sys].[objects] [OBJS]
WHERE [OBJS].[type] = 'TR'
AND [OBJS].[parent_object_id] = @TABLE_ID);
IF @TRIGGERSTATEMENT <> '' COLLATE Chinese_PRC_CI_AS
  SET @TRIGGERSTATEMENT = @vbCrLf + ';' COLLATE Chinese_PRC_CI_AS + @vbCrLf + @TRIGGERSTATEMENT;
-- ####################
--NEW SECTION QUERY ALL EXTENDED PROPERTIES
-- ####################
SET @EXTENDEDPROPERTIES = '';
SELECT
  @EXTENDEDPROPERTIES
  = @EXTENDEDPROPERTIES + @vbCrLf + 'EXEC sys.sp_addextendedproperty @name = N''' COLLATE Chinese_PRC_CI_AS + [name] + ''', @value = N''' COLLATE Chinese_PRC_CI_AS
  + REPLACE(CONVERT(varchar(max), [VALUE]), '''', '''''')
  + ''', @level0type = N''SCHEMA'', @level0name = ' COLLATE Chinese_PRC_CI_AS + QUOTENAME(@SCHEMANAME)
  + ', @level1type = N''TABLE'', @level1name = ' COLLATE Chinese_PRC_CI_AS + QUOTENAME(@TBLNAME) + ';' + @vbCrLf
FROM [sys].[fn_listextendedproperty](NULL, 'schema', @SCHEMANAME, 'table', @TBLNAME, NULL, NULL);

WITH [obj]
AS (SELECT
  [split].[a].[value]('.', 'VARCHAR(20)') AS [name]
FROM (SELECT
  CAST('<M>' + REPLACE('column,constraint,index,trigger,parameter', ',', '</M><M>') + '</M>' AS xml) AS [data]) AS [A]
CROSS APPLY [data].[nodes]('/M') AS [split] ([a]))
SELECT
  @EXTENDEDPROPERTIES
  = @EXTENDEDPROPERTIES + @vbCrLf + 'EXEC sys.sp_addextendedproperty @name = N''' COLLATE Chinese_PRC_CI_AS + [lep].[name] + ''', @value = N''' COLLATE Chinese_PRC_CI_AS
  + REPLACE(CONVERT(varchar(max), [lep].[value]), '''', '''''')
  + ''', @level0type = N''SCHEMA'', @level0name = ' COLLATE Chinese_PRC_CI_AS + QUOTENAME(@SCHEMANAME)
  + ', @level1type = N''TABLE'', @level1name = ' COLLATE Chinese_PRC_CI_AS + QUOTENAME(@TBLNAME)
  + ', @level2type = N''' COLLATE Chinese_PRC_CI_AS + UPPER([obj].[name])
  + ''', @level2name = ' COLLATE Chinese_PRC_CI_AS + QUOTENAME([lep].[objname]) + ';' COLLATE Chinese_PRC_CI_AS --SELECT objtype, objname, name, value
FROM [obj]
CROSS APPLY [sys].[fn_listextendedproperty](NULL, 'schema', @SCHEMANAME, 'table', @TBLNAME, [obj].[name], NULL) AS [lep];
IF @EXTENDEDPROPERTIES <> '' COLLATE Chinese_PRC_CI_AS
  SET @EXTENDEDPROPERTIES = @vbCrLf + ';' COLLATE Chinese_PRC_CI_AS + @vbCrLf + @EXTENDEDPROPERTIES;
-- ####################
--FINAL CLEANUP AND PRESENTATION
-- ####################
--at this point, there is a trailing comma, or it blank
SELECT
  @FINALSQL = @FINALSQL + @CONSTRAINTSQLS + @CHECKCONSTSQLS + @FKSQLS;
--note that this trims the trailing comma from the end of the statements
SET @FINALSQL = SUBSTRING(@FINALSQL, 1, LEN(@FINALSQL) - 1);
SET @FINALSQL = @FINALSQL + ')' COLLATE Chinese_PRC_CI_AS;
SET @input = ' ' + @FINALSQL + @INDEXSQLS + @RULESCONSTSQLS + @TRIGGERSTATEMENT + @EXTENDEDPROPERTIES;

SELECT
  @TBLNAME AS [Table],
  @input AS [Create Table];
";
            return sql;
        }
    }
}

#endif