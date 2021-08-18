#if Full || DataKit

namespace Netnr.SharedDataKit
{
    public partial class Configs
    {
        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static string GetTableSQLite(string DatabaseName)
        {
            return $@"
SELECT
  tbl_name AS TableName
FROM
  {DatabaseName}.sqlite_master
WHERE
  type = 'table'
ORDER BY
  tbl_name
            ";
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <param name="Where">条件</param>
        /// <returns></returns>
        public static string GetColumnSQLite(string DatabaseName, string Where)
        {
            return $@"
SELECT
  '' AS TableName,
  '' AS ColumnName,
  '' AS ColumnType,
  '' AS DataType,
  0 AS DataLength,
  0 AS DataScale,
  0 AS ColumnOrder,
  '' AS PrimaryKey,
  '' AS [NotNull],
  '' AS ColumnDefault
UNION ALL
SELECT
  m.name AS TableName,
  p.name AS ColumnName,
  p.type AS ColumnType,
  CASE
    WHEN instr(p.type, '(') = 0 THEN p.type
    ELSE substr(p.type, 0, instr(p.type, '('))
  END AS DataType,
  CASE
    WHEN instr(p.type, ',') <> 0 THEN substr(
      p.type,
      instr(p.type, '(') + 1,
      instr(p.type, ',') - instr(p.type, '(') -1
    )
    WHEN instr(p.type, '(') <> 0 THEN substr(
      p.type,
      instr(p.type, '(') + 1,
      LENGTH(p.type) - instr(p.type, '(') -1
    )
    ELSE NULL
  END AS DataLength,
  CASE
    WHEN instr(p.type, ',') <> 0 THEN substr(
      p.type,
      instr(p.type, ',') + 1,
      LENGTH(p.type) - instr(p.type, ',') -1
    )
    ELSE NULL
  END AS DataScale,
  p.cid + 1 AS ColumnOrder,
	CASE
    WHEN p.pk = 1 THEN 'YES'
    ELSE ''
  END AS PrimaryKey,
  CASE
    WHEN p.[notnull] = 1 THEN 'YES'
    ELSE ''
  END AS [NotNull],
  p.dflt_value AS ColumnDefault
FROM
  {DatabaseName}.sqlite_master m
  LEFT OUTER JOIN pragma_table_info (m.name) p ON m.name <> p.name
WHERE
  m.type = 'table' {Where}
ORDER BY
  TableName,
  ColumnOrder;

SELECT name, sql FROM {DatabaseName}.sqlite_master m WHERE 1=1 {Where}
            ";
        }
    }
}

#endif