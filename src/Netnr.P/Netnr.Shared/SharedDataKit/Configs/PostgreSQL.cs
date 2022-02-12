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
    WHEN 'NO' THEN 0
    ELSE 1
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

        /// <summary>
        /// 表DLL
        /// </summary>
        /// <param name="TableSchema">模式</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static string GetTableDDLPostgreSQL(string TableSchema, string TableName)
        {
            // https://stackoverflow.com/questions/2593803
            var sql = $@"
DO $$
DECLARE
  in_schema_name VARCHAR := '{TableSchema}';
  in_table_name VARCHAR := '{TableName}';
  newline VARCHAR := E'\n';
  -- the ddl we're building
  v_table_ddl TEXT;
  -- data about the target table
  v_table_oid INT;
  -- records for looping
  v_column_record record;
  v_constraint_record record;
  v_index_record record;
  v_table_comment TEXT;
  v_column_comment TEXT;
BEGIN
  SELECT
    c.oid INTO v_table_oid
  FROM
    pg_catalog.pg_class c
    LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
  WHERE
    1 = 1
    AND c.relkind = 'r'
    AND c.relname = in_table_name
    AND n.nspname = in_schema_name;
  -- the schema
  -- throw an error if table was not found
  IF (v_table_oid IS NULL) THEN
    RAISE EXCEPTION 'table(%) does not exist',in_table_name;
  END IF;
  -- start the create definition
  v_table_ddl := concat(
    'DROP TABLE IF EXISTS ',
    in_schema_name,
    '.""',
    in_table_name,
    '"";',
    newline,
    'CREATE TABLE ',
    in_schema_name,
    '.""',
    in_table_name,
    '"" (',
    newline
  );
  -- define all of the columns in the table; https://stackoverflow.com/a/8153081/3068233
  FOR v_column_record IN
  SELECT
    c.table_schema,
    c.table_name,
    c.column_name,
    c.data_type,
    c.character_maximum_length,
    c.is_nullable,
    c.column_default,
    col_description(
      format('%s.""%s""', c.table_schema, c.table_name) :: regclass :: oid,
      c.ordinal_position
    ) AS column_comment
  FROM
    information_schema.columns c
  WHERE
    (table_schema, table_name) = (in_schema_name, in_table_name)
  ORDER BY
    ordinal_position LOOP v_table_ddl := concat(
      v_table_ddl,
      '  ""',
      v_column_record.column_name,
      '"" ',
      v_column_record.data_type,
      CASE
        WHEN v_column_record.character_maximum_length IS NOT NULL THEN concat('(', v_column_record.character_maximum_length, ')')
        ELSE ''
      END,
      ' ',
      CASE
        WHEN v_column_record.is_nullable = 'NO' THEN 'NOT NULL'
        ELSE 'NULL'
      END,
      CASE
        WHEN v_column_record.column_default IS NOT NULL THEN concat(' DEFAULT ', v_column_record.column_default)
        ELSE ''
      END,
      ',',
      newline
    );
  -- column comment
  v_column_comment := concat(
    v_column_comment,    
    newline,
    'COMMENT ON COLUMN ',
    v_column_record.table_schema,
    '.""',
    v_column_record.table_name,
    '"".""',
    v_column_record.column_name,
    '"" IS ''',
    v_column_record.column_comment,
    ''';'
  );
  END LOOP;
  -- define all the constraints in the; https://dba.stackexchange.com/a/214877/75296
  FOR v_constraint_record IN
  SELECT
    con.conname AS constraint_name,
    con.contype AS constraint_type,
    CASE
      WHEN con.contype = 'p' THEN 1 -- primary key constraint
      WHEN con.contype = 'u' THEN 2 -- unique constraint
      WHEN con.contype = 'f' THEN 3 -- foreign key constraint
      WHEN con.contype = 'c' THEN 4
      ELSE 5
    END AS type_rank,
    pg_get_constraintdef(con.oid) AS constraint_definition
  FROM
    pg_catalog.pg_constraint con
    JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
    JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
  WHERE
    nsp.nspname = in_schema_name
    AND rel.relname = in_table_name
  ORDER BY
    type_rank LOOP v_table_ddl := concat(
      v_table_ddl,
      '  ',
      'CONSTRAINT ""',
      v_constraint_record.constraint_name,
      '"" ',
      v_constraint_record.constraint_definition,
      ',',
      newline
    );
  END LOOP;
  -- drop the last comma before ending the create statement
  v_table_ddl = concat(
    substr(v_table_ddl, 0, length(v_table_ddl) - 1),
    newline
  );
  -- end the create definition
  v_table_ddl := concat(v_table_ddl, ');', newline, newline);
  -- suffix create statement with all of the indexes on the table
  FOR v_index_record IN
  SELECT
    indexdef
  FROM
    pg_indexes
  WHERE
    (schemaname, tablename) = (in_schema_name, in_table_name)
    AND indexname NOT IN (
      SELECT
        conname
      FROM
        pg_catalog.pg_constraint
      WHERE
        contype = 'p'
    ) LOOP v_table_ddl := concat(v_table_ddl, v_index_record.indexdef, ';', newline);
  END LOOP;
  -- table comment
  SELECT
    concat(
      'COMMENT ON TABLE ',
      schemaname,
      '.""',
      relname,
      '""',
      ' IS ''',
      REPLACE(obj_description(relid), '''', ''''''),
      ''';',
      newline
    ) INTO v_table_comment
  FROM
    pg_stat_user_tables
  WHERE
    schemaname = in_schema_name
    AND relname = in_table_name;
  -- comment
  v_table_ddl := concat(
    v_table_ddl,
    newline,
    v_table_comment,
    v_column_comment
  );
  -- return the ddl
  RAISE NOTICE '%', REPLACE(v_table_ddl, concat(newline, newline, newline), newline);
END
$$;
";
            return sql;
        }

    }
}

#endif