var sqlForPostgreSQL = [
    {
        name: "",
        sql: `SELECT
  'Name' col,
  split_part(split_part(VERSION(), ',', 1), ' on ', 1)
UNION ALL
SELECT
  'Version' col,
  (
    SELECT
      setting
    FROM
      pg_settings
    WHERE
      NAME = 'server_version'
  ) val
UNION ALL
SELECT
  'Compile' col,
  split_part(VERSION(), ',', 2) val
UNION ALL
SELECT
  'DirInstall' col,
  (
    SELECT
      split_part(setting, 'main', 1)
    FROM
      pg_settings
    WHERE
      NAME = 'archive_command'
  ) val
UNION ALL
SELECT
  'DirData' col,
  (
    SELECT
      setting
    FROM
      pg_settings
    WHERE
      NAME = 'data_directory'
  ) val
UNION ALL
SELECT
  'CharSet' col,
  (
    SELECT
      setting
    FROM
      pg_settings
    WHERE
      NAME = 'server_encoding'
  ) val
UNION ALL
SELECT
  'TimeZone' col,
  (
    SELECT
      setting
    FROM
      pg_settings
    WHERE
      NAME = 'TimeZone'
  ) val
UNION ALL
SELECT
  'DateTime' col,
  to_char(now(), 'YYYY-MM-DD HH24:MI:SS.MS') val
UNION ALL
SELECT
  'MaxConn' col,
  (
    SELECT
      setting
    FROM
      pg_settings
    WHERE
      NAME = 'max_connections'
  ) val
UNION ALL
SELECT
  'CurrConn' col,
  CAST(COUNT(1) AS VARCHAR) val
FROM
  pg_stat_activity
UNION ALL
SELECT
  'TimeOut' col,
  (
    SELECT
      setting
    FROM
      pg_settings
    WHERE
      NAME = 'statement_timeout'
  ) val
UNION ALL
SELECT
  'IgnoreCase' col,
  CASE
    'a' = 'A'
    WHEN 't' THEN '1'
    ELSE '0'
  END val
UNION ALL
SELECT
  'System' col,
  split_part(split_part(VERSION(), ',', 1), ' on ', 2) val
`
    },
    {
        name: "内置方法、对象",
        sql: `DO $$
BEGIN
    RAISE NOTICE '当前日期时间：%', now();
    RAISE NOTICE '版本信息：%', version();
    RAISE NOTICE 'UUID：%', uuid_generate_v4(); -- create extension "uuid-ossp"; -- 为表添加扩展
END;
$$;
`
    },
    {
        name: "",
        sql: ``
    },
]

export { sqlForPostgreSQL }