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
        public static string GetDatabaseNameOracle()
        {
            return $@"
SELECT
	USERNAME AS DatabaseName
FROM
	ALL_USERS
ORDER BY
	USERNAME
            ";
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseOracle()
        {
            return $@"
SELECT
	t1.USERNAME AS DatabaseName,
	'DEFAULT' AS DatabaseClassify,
	t2.DEFAULT_TABLESPACE AS DatabaseSpace,
	(
	SELECT
		VALUE
	FROM
		Nls_Database_Parameters
	WHERE
		PARAMETER = 'NLS_CHARACTERSET' ) AS DatabaseCharset,
	t3.FILE_NAME AS DatabasePath,
	t3.BYTES AS DatabaseDataLength,
	t4.BYTES AS DatabaseIndexLength
FROM
	ALL_USERS t1
LEFT JOIN DBA_USERS t2 ON
	t1.USER_ID = t2.USER_ID
LEFT JOIN DBA_DATA_FILES t3 ON
	t3.TABLESPACE_NAME = t2.DEFAULT_TABLESPACE
LEFT JOIN (
	SELECT
		TABLESPACE_NAME,
		SUM(BYTES) BYTES
	FROM
		DBA_SEGMENTS
	WHERE
		SEGMENT_TYPE = 'INDEX'
	GROUP BY
		TABLESPACE_NAME ) t4 ON
	t4.TABLESPACE_NAME = t2.DEFAULT_TABLESPACE
ORDER BY
	t1.USERNAME
            ";
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static string GetTableOracle(string DatabaseName)
        {
            return $@"
SELECT
  t1.TABLE_NAME AS TableName,
  t2.TABLE_TYPE AS TableType,
  t1.NUM_ROWS AS TableRows,
  t4.BYTES AS TableDataLength,
  t5.TableIndexLength,
  t3.CREATED AS TableCreateTime,
  t2.COMMENTS AS TableComment
FROM
  ALL_TABLES t1
  LEFT JOIN ALL_TAB_COMMENTS t2 ON t1.OWNER = t2.OWNER
  AND t1.TABLE_NAME = t2.TABLE_NAME
  LEFT JOIN ALL_OBJECTS t3 ON t1.OWNER = t3.OWNER
  AND t3.OBJECT_TYPE = t2.TABLE_TYPE
  AND t3.OBJECT_NAME = t1.TABLE_NAME
  LEFT JOIN DBA_SEGMENTS t4 ON t1.OWNER = t4.OWNER
  AND t1.TABLE_NAME = t4.SEGMENT_NAME
  AND t4.SEGMENT_TYPE = t2.TABLE_TYPE
  LEFT JOIN (
    SELECT
      p1.OWNER,
      p1.TABLE_NAME,
      SUM(p2.BYTES) TableIndexLength
    FROM
      DBA_INDEXES p1
      LEFT JOIN DBA_SEGMENTS p2 ON p1.OWNER = p2.OWNER
      AND p1.INDEX_NAME = p2.SEGMENT_NAME
    GROUP BY
      P1.OWNER,
      p1.TABLE_NAME
  ) t5 ON t1.OWNER = t5.OWNER
  AND t1.TABLE_NAME = t5.TABLE_NAME
WHERE
  t1.OWNER = '{DatabaseName}'
ORDER BY
  t1.TABLE_NAME
            ";
        }

        /// <summary>
        /// 表DLL
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableNames">表名</param>
        /// <returns></returns>
        public static string GetTableDDLOracle(string DatabaseName, List<string> TableNames)
        {
            var listSql = new List<string>();
            TableNames.ForEach(table =>
            {
                listSql.Add($"SELECT DBMS_METADATA.GET_DDL('TABLE', 'PLTF_ACTIVITY','CQSME1') from dual; -- ddl");
            });
            return string.Join(";", listSql);
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="Where">条件</param>
        /// <returns></returns>
        public static string GetColumnOracle(string DatabaseName, string Where)
        {
            return $@"
SELECT
  t1.TABLE_NAME AS TableName,
  t2.COMMENTS AS TableComment,
  t3.COLUMN_NAME AS ColumnName,
  t3.DATA_TYPE || '(' || CASE
    WHEN t3.CHARACTER_SET_NAME = 'NCHAR_CS' THEN t3.DATA_LENGTH / 2
    ELSE t3.DATA_LENGTH
  END || ')' AS ColumnType,
  t3.DATA_TYPE AS DataType,
  CASE
    WHEN t3.CHARACTER_SET_NAME = 'NCHAR_CS' THEN t3.DATA_LENGTH / 2
    WHEN t3.DATA_TYPE = 'NUMBER' THEN t3.DATA_PRECISION
    ELSE t3.DATA_LENGTH
  END AS DataLength,
  t3.DATA_SCALE AS DataScale,
  t3.COLUMN_ID AS ColumnOrder,
  t5.POSITION AS PrimaryKey,
  DECODE(t3.NULLABLE, 'N', 0, 1) AS IsNullable,
  t3.DATA_DEFAULT AS ColumnDefault,
  t4.COMMENTS AS ColumnComment
FROM
  ALL_TABLES t1
  LEFT JOIN ALL_TAB_COMMENTS t2 ON t1.OWNER = t2.OWNER
  AND t1.TABLE_NAME = t2.TABLE_NAME
  LEFT JOIN ALL_TAB_COLUMNS t3 ON t1.OWNER = t3.OWNER
  AND t1.TABLE_NAME = t3.TABLE_NAME
  LEFT JOIN ALL_COL_COMMENTS t4 ON t1.OWNER = t4.OWNER
  AND t1.TABLE_NAME = t4.TABLE_NAME
  AND t3.COLUMN_NAME = t4.COLUMN_NAME
  LEFT JOIN (
    SELECT
      P1.OWNER,
      p1.TABLE_NAME,
      p2.COLUMN_NAME,
      p2.POSITION
    FROM
      ALL_CONSTRAINTS p1
      LEFT JOIN ALL_CONS_COLUMNS p2 ON p1.OWNER = p2.OWNER
      AND p1.TABLE_NAME = p2.TABLE_NAME
      AND p1.CONSTRAINT_NAME = p2.CONSTRAINT_NAME
    WHERE
      p1.CONSTRAINT_TYPE = 'P'
  ) t5 ON t1.OWNER = t5.OWNER
  AND t5.TABLE_NAME = t1.TABLE_NAME
  AND t3.COLUMN_NAME = t5.COLUMN_NAME
WHERE
  t1.OWNER = '{DatabaseName}' {Where}
ORDER BY
  t1.TABLE_NAME,
  t3.COLUMN_ID
            ";
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        public static string SetTableCommentOracle(string DatabaseName, string TableName, string TableComment)
        {
            return $"COMMENT ON TABLE \"{DatabaseName}\".\"{TableName}\" IS '{TableComment.OfSql()}'";
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="TableName">表名</param>
        /// <param name="ColumnName">列名</param>
        /// <param name="ColumnComment">列注释</param>
        /// <returns></returns>
        public static string SetColumnCommentOracle(string DatabaseName, string TableName, string ColumnName, string ColumnComment)
        {
            return $"COMMENT ON COLUMN \"{DatabaseName}\".\"{TableName}\".\"{ColumnName}\" IS '{ColumnComment.OfSql()}'";
        }

    }
}

#endif