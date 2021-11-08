#if Full || DataKit

using System.Collections.Generic;

namespace Netnr.SharedDataKit
{
    public partial class Configs
    {
        /// <summary>
        /// 获取库名
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseNameMySQL()
        {
            return $@"
SELECT
  SCHEMA_NAME AS DatabaseName
FROM
  information_schema.schemata
ORDER BY
  SCHEMA_NAME
            ";
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseMySQL()
        {
            return $@"
SELECT
  SCHEMA_NAME AS DatabaseName,
  CASE
    WHEN t3.rid > 4 THEN 'USER'
    ELSE 'SYSTEM'
  END AS DatabaseClassify,
  DEFAULT_CHARACTER_SET_NAME AS DatabaseCharset,
  DEFAULT_COLLATION_NAME AS DatabaseCollation,
  @@datadir AS DatabasePath,
  t2.DatabaseDataLength,
  t2.DatabaseIndexLength
FROM
  information_schema.schemata t1
  LEFT JOIN (
    SELECT
      TABLE_SCHEMA,
      SUM(DATA_LENGTH) AS DatabaseDataLength,
      SUM(INDEX_LENGTH) AS DatabaseIndexLength
    FROM
      information_schema.tables
    GROUP BY
      TABLE_SCHEMA
  ) t2 ON t1.SCHEMA_NAME = t2.TABLE_SCHEMA
  LEFT JOIN (
    SELECT
      TABLE_SCHEMA,
      ROW_NUMBER() OVER(
        ORDER BY
          MIN(CREATE_TIME)
      ) rid
    FROM
      information_schema.tables
    GROUP BY
      TABLE_SCHEMA
    ORDER BY
      MIN(CREATE_TIME)
  ) t3 ON t1.SCHEMA_NAME = t3.TABLE_SCHEMA
ORDER BY
  t1.SCHEMA_NAME
            ";
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static string GetTableMySQL(string DatabaseName)
        {
            return $@"
SELECT
  TABLE_NAME AS TableName,
  TABLE_TYPE AS TableType,
  ENGINE AS TableEngine,
  TABLE_ROWS AS TableRows,
  DATA_LENGTH AS TableDataLength,
  INDEX_LENGTH AS TableIndexLength,
  CREATE_TIME AS TableCreateTime,
  TABLE_COLLATION AS TableCollation,
  TABLE_COMMENT AS TableComment
FROM
  information_schema.tables
WHERE
  TABLE_SCHEMA = '{DatabaseName}'
ORDER BY
  TABLE_NAME
            ";
        }

        /// <summary>
        /// 表DLL
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableNames">表名</param>
        /// <returns></returns>
        public static string GetTableDDLMySQL(string DatabaseName, List<string> TableNames)
        {
            var listSql = new List<string>();
            TableNames.ForEach(table =>
            {
                listSql.Add($"SHOW CREATE TABLE `{DatabaseName}`.`{table}`");
            });
            return string.Join(";", listSql);
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="Where">条件</param>
        /// <returns></returns>
        public static string GetColumnMySQL(string DatabaseName, string Where = null)
        {
            return $@"
SELECT
  t2.TABLE_NAME AS TableName,
  t2.TABLE_COMMENT AS TableComment,
  t1.COLUMN_NAME AS ColumnName,
  t1.COLUMN_TYPE AS ColumnType,
  t1.DATA_TYPE AS DataType,
  CASE
    WHEN t1.CHARACTER_MAXIMUM_LENGTH IS NOT NULL THEN t1.CHARACTER_MAXIMUM_LENGTH
    WHEN t1.NUMERIC_PRECISION IS NOT NULL THEN t1.NUMERIC_PRECISION
    ELSE NULL
  END AS DataLength,
  t1.NUMERIC_SCALE AS DataScale,
  t1.ORDINAL_POSITION AS ColumnOrder,
  (
    SELECT
      ORDINAL_POSITION
    FROM
      information_schema.key_column_usage
    WHERE
      TABLE_SCHEMA = t2.TABLE_SCHEMA
      AND TABLE_NAME = t2.TABLE_NAME
      AND COLUMN_NAME = t1.COLUMN_NAME
    LIMIT 1 
  ) AS PrimaryKey,
  CASE
    WHEN t1.EXTRA = 'auto_increment' THEN 1
    ELSE NULL
  END AS AutoIncr,
  CASE
    WHEN t1.IS_NULLABLE = 'YES' THEN 1
    ELSE 0
  END AS IsNullable,
  t1.COLUMN_DEFAULT AS ColumnDefault,
  t1.COLUMN_COMMENT AS ColumnComment
FROM
  information_schema.columns t1
  LEFT JOIN information_schema.tables t2 ON t1.TABLE_SCHEMA = t2.TABLE_SCHEMA
  AND t1.TABLE_NAME = t2.TABLE_NAME
WHERE
  t2.TABLE_SCHEMA = '{DatabaseName}' {Where}
ORDER BY
  t2.TABLE_NAME,
  t1.ORDINAL_POSITION
            ";
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        public static string SetTableCommentMySQL(string DatabaseName, string TableName, string TableComment)
        {
            return $"ALTER TABLE `{DatabaseName}`.`{TableName}` COMMENT '{TableComment.OfSql()}'";
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableName">表名</param>
        /// <param name="ColumnName">列名</param>
        /// <param name="ColumnComment">列注释</param>
        /// <returns></returns>
        public static string SetColumnCommentMySQL(string DatabaseName, string TableName, string ColumnName, string ColumnComment)
        {
            return $"ALTER TABLE `{DatabaseName}`.`{TableName}` MODIFY COLUMN `{ColumnName}` int NULL COMMENT '{ColumnComment.OfSql()}'";
        }

    }
}

#endif