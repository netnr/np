// 脚本
var ndkNoteSQL = {};

ndkNoteSQL["SQLite"] = [
  {
    group: 'rely',
    name: "db-env-info",
    sql: `SELECT 'version' col, sqlite_version() val UNION ALL SELECT 'datetime' col, datetime() val UNION ALL SELECT 'ignore_case' col, 'a' = 'A' val;
PRAGMA encoding`,
    remark: '数据库环境信息'
  },
  {
    group: 'default',
    name: "database-name",
    sql: `PRAGMA database_list`,
    remark: '数据库名称'
  },
  {
    group: 'default',
    name: "uuid",
    sql: `select hex(randomblob(16))`,
    remark: '模拟 UUID'
  },
  {
    group: 'default',
    name: "vacuum",
    sql: `VACUUM`,
    remark: '磁盘空间释放'
  },
  {
    group: 'default',
    name: "attach",
    sql: `attach database '/tmp/dbname.db' AS main2`,
    remark: '附加数据库'
  },
  {
    group: 'default',
    name: "info",
    sql: `select sqlite_version(); -- 版本
PRAGMA encoding; -- 编码

select * from main.sqlite_master; -- 所有表信息、DDL
PRAGMA table_info('SysButton'); -- 表信息
select cid, name, type, [notnull], quote(dflt_value) as dflt_value, pk from pragma_table_info('SysUser'); -- 表信息

select m.*, p.cid, p.name as colname, p.type as coltype, p.[notnull], quote(p.dflt_value) as dflt_value, p.pk
from main.sqlite_master m left join pragma_table_info(m.name) p ON m.name<>p.name; -- 所有表信息
`,
    remark: '信息'
  },
  {
    group: 'default',
    name: "daetime-function",
    sql: `select '日期时间' as name, datetime() as value
union all
select '日期', date()
union all
select '时间', time()
union all
select '+2 天', date('now','+2 day')
union all
select '-2 年', date('now','-2 year')
union all
select '格式化（yyyy MM dd）', strftime('%Y %m %d','now')
union all
select '格式化（HH mm ss fff)', strftime('%H %M %S %s','now')`,
    remark: '时间函数'
  },
  {
    group: 'default',
    name: "empty",
    sql: ``
  },
];

ndkNoteSQL["MySQL"] = ndkNoteSQL["MariaDB"] = [
  {
    group: 'rely',
    name: "db-env-info",
    sql: `SELECT 'name' col, @@version_comment val
UNION ALL
SELECT 'version' col, @@version val
UNION ALL
SELECT 'compile' col, @@version_compile_machine val
UNION ALL
SELECT 'dir_install' col, @@basedir val
UNION ALL
SELECT 'dir_data' col, @@datadir val
UNION ALL
SELECT 'dir_temp' col, @@tmpdir val
UNION ALL
SELECT 'engine' col, @@default_storage_engine val
UNION ALL
SELECT 'charset' col, @@collation_server val
UNION ALL
SELECT 'time_zone' col, @@system_time_zone val
UNION ALL
SELECT 'max_conn' col, @@max_connections val
UNION ALL
SELECT 'curr_conn' col, count( 1 ) val FROM information_schema.PROCESSLIST WHERE USER != 'event_scheduler'
UNION ALL
SELECT 'datetime' col, now() AS val
UNION ALL
SELECT 'time_out' col, @@wait_timeout AS val
UNION ALL
SELECT 'ignore_case' col, 'a' = 'A' AS val
UNION ALL
SELECT 'system' col, @@version_compile_os val`,
    remark: '数据库环境信息'
  },
  {
    group: 'rely',
    name: "db-var-info",
    sql: `SELECT concat(VARIABLE_NAME, '=', VARIABLE_VALUE) AS params,
  CASE VARIABLE_VALUE WHEN 'OFF' THEN 'SET GLOBAL local_infile = ON' END AS idea,
  '是否允许加载本地数据，BulkCopy 需要开启，ON 开启，OFF 关闭' AS remark
FROM performance_schema.session_variables WHERE VARIABLE_NAME = 'local_infile'
UNION ALL
SELECT concat(VARIABLE_NAME, '=', VARIABLE_VALUE) AS params,
  CASE WHEN VARIABLE_VALUE < 600 THEN 'SET GLOBAL innodb_lock_wait_timeout = 600' END AS idea,
  'innodb 的 dml 操作的行级锁的等待时间，事务等待获取资源等待的最长时间，BulkCopy 量大超时设置，单位：秒' AS remark
FROM performance_schema.session_variables WHERE VARIABLE_NAME = 'innodb_lock_wait_timeout'
UNION ALL
SELECT concat(VARIABLE_NAME, '=', VARIABLE_VALUE) AS params,
  CASE WHEN VARIABLE_VALUE != 1073741824 THEN 'SET GLOBAL max_allowed_packet = 1073741824' END AS idea,
  '传输的 packet 大小限制，最大 1G，单位：B' AS remark
FROM performance_schema.session_variables WHERE VARIABLE_NAME = 'max_allowed_packet'
UNION ALL
SELECT concat(VARIABLE_NAME, '=', VARIABLE_VALUE) AS params,
  CASE WHEN VARIABLE_VALUE != 0 THEN concat('SET GLOBAL ', VARIABLE_NAME, ' = 0') END AS idea,
  '缓存中统计信息过期时间，要直接从存储引擎获取统计信息，将其设置为 0，单位：秒；expiry 为 MySQL8' AS remark
FROM performance_schema.session_variables WHERE VARIABLE_NAME IN ( 'information_schema_stats', 'information_schema_stats_expiry' )
UNION ALL
SELECT concat( t1.p1, '%, Innodb_buffer_pool_size: ', @@innodb_buffer_pool_size ) AS params,
  concat('SET GLOBAL Innodb_buffer_pool_size = ', t1.p2) AS idea,
  concat( '缓存索引和数据的内存大小，多多益善，内存读写减少磁盘读写，', '推荐设置 Innodb_buffer_pool_size 为服务器总可用内存的 75%, 过多造成浪费，', '计算 Innodb_buffer_pool_pages_data / Innodb_buffer_pool_pages_total * 100%，', '结果 > 95% 则增加 Innodb_buffer_pool_size，结果 < 95% 则减少 Innodb_buffer_pool_size' ) AS remark
FROM ( SELECT
    ROUND( ( SELECT VARIABLE_VALUE FROM performance_schema.global_status WHERE VARIABLE_NAME = 'Innodb_buffer_pool_pages_data' ) / ( SELECT VARIABLE_VALUE FROM performance_schema.global_status WHERE VARIABLE_NAME = 'Innodb_buffer_pool_pages_total' ) * 100, 0 ) p1,
    ROUND( ( SELECT VARIABLE_VALUE FROM performance_schema.global_status WHERE VARIABLE_NAME = 'Innodb_buffer_pool_pages_data' ) * ( SELECT VARIABLE_VALUE FROM performance_schema.global_status WHERE VARIABLE_NAME = 'Innodb_page_size' ) * 1.05, 0 ) AS p2 ) t1
UNION ALL
SELECT concat(VARIABLE_NAME, '=', VARIABLE_VALUE) AS params,
  'SET GLOBAL innodb_flush_log_at_trx_commit=1' AS idea,
  '1 保证事务，等待磁盘写入；0 和  2 高性能缓存再写入，宕机可能丢失数据' AS remark
FROM performance_schema.session_variables WHERE VARIABLE_NAME = 'innodb_flush_log_at_trx_commit';

select * from performance_schema.session_variables; -- show session variables;
select * from performance_schema.global_variables; -- show global variables;
    `,
    remark: '数据库参数信息'
  },
  {
    group: 'default',
    name: "database-name",
    sql: `SELECT SCHEMA_NAME AS DatabaseName FROM information_schema.schemata ORDER BY SCHEMA_NAME`,
    remark: '数据库名称'
  },
  {
    group: 'default',
    name: "uuid",
    sql: `select uuid()`,
    remark: '生成 UUID'
  },
  {
    group: 'default',
    name: "show-common",
    sql: `show databases; -- 显示数据库
show tables from information_schema; -- 显示当前数据库中所有表名称
show columns from information_schema.TABLES; -- 显示表中列名称
show table status; -- 显示表的信息
show index from information_schema.TABLES; -- 显示表索引

show grants for root; -- 显示用户权限

show status; -- 显示系统特定资源信息
show variables; -- 显示系统变量的名称和值
show variables like '%local_infile%'; -- 搜索

show processlist; -- 显示运行进程

show warnings; -- 显示最后的错误、警告和通知
show errors; -- 显示最后的错误

show slave status; -- 从状态
show master status; -- 主状态`,
    remark: 'show 常用命令'
  },
  {
    group: 'default',
    name: "sleep",
    sql: `select sysdate(), sleep(1), sysdate()`
  },
  {
    group: 'default',
    name: "datetime-function",
    sql: `select now(), sysdate(), sleep(1)
union all
select now(), sysdate(), 0; -- 当前日期时间，now 执行开始得到值 sysdate 实时获取

select concat('日期：',current_date(),' , 时间：',current_time()) '日期、时间' -- 日期、时间
union all
select concat('UTC日期：',utc_date(),' , UTC时间：',utc_time());

select utc_timestamp(); -- utc日期时间
select unix_timestamp(now()); -- 转时间戳
select date_format(now(), '%Y/%m/%d %H:%i:%s'); -- 格式化`,
    remark: '时间函数'
  },
  {
    group: 'default',
    name: "string-length",
    sql: `-- 字符串字节长度、字符长度
SELECT
  LENGTH("demo") "英文(demo)",
  LENGTH("测试。") "中文符号(测试。)",
  CHAR_LENGTH("demo") "CHAR - 英文(demo)",
  CHAR_LENGTH("测试。") "CHAR - 中文符号(测试。)";`,
    remark: '字符串长度'
  },
  {
    group: 'default',
    name: "innodb_buffer_pool_size",
    sql: `
-- Innodb buffer pool 缓存池中包含数据的页的数目，包括脏页。单位是 page
-- Innodb buffer pool 的页总数目。单位是 page
show global status where Variable_name in ('Innodb_buffer_pool_pages_data', 'Innodb_buffer_pool_pages_total', 'Innodb_page_size');

SELECT @@innodb_buffer_pool_size/1024/1024/1024; -- 字节转为 G

-- 在线调整 InnoDB 缓冲池大小，如果不设置，默认为 128M
-- set global innodb_buffer_pool_size = 1024*1024*1024*9; -- 单位字节`,
    remark: 'InnoDB 缓冲池大小'
  },
  {
    group: 'default',
    name: "empty",
    sql: ``
  },
];

ndkNoteSQL["Oracle"] = [
  {
    group: 'rely',
    name: "db-env-info",
    sql: `SELECT 'name' col, ( SELECT PRODUCT FROM ( SELECT ROW_NUMBER() OVER( ORDER BY PRODUCT DESC ) numid, A.* FROM product_component_version A ) WHERE numid = 3 ) val FROM dual
UNION ALL
SELECT 'version' col, ( SELECT VERSION FROM product_component_version WHERE ROWNUM = 1 ) val FROM dual
UNION ALL
SELECT 'compile' col, ( SELECT STATUS FROM ( SELECT ROW_NUMBER() OVER( ORDER BY PRODUCT DESC ) numid, A.* FROM product_component_version A ) WHERE numid = 3 ) val FROM dual
UNION ALL
SELECT 'dir_data' col, ( SELECT file_name FROM dba_data_files WHERE file_id = 1 ) val FROM dual
UNION ALL
SELECT 'charset' col, ( SELECT VALUE FROM Nls_Database_Parameters WHERE PARAMETER = 'NLS_CHARACTERSET' ) val FROM dual
UNION ALL
SELECT 'time_zone' col, SESSIONTIMEZONE val FROM dual
UNION ALL
SELECT 'datetime' col, TO_CHAR(SYSDATE, 'yyyy-mm-dd hh24:mi:ss') val FROM dual
UNION ALL
SELECT 'max_conn' coll, ( SELECT TO_CHAR(VALUE) FROM v$parameter WHERE NAME = 'processes' ) val FROM dual
UNION ALL
SELECT 'curr_conn' col, ( SELECT TO_CHAR(COUNT(1)) FROM v$process ) val FROM dual
UNION ALL
SELECT 'ignore_case' col, ( CASE WHEN 'a' = 'A' THEN '1' ELSE '0' END ) val FROM dual
UNION ALL
SELECT 'system' col, ( select PLATFORM_NAME from v$database ) val FROM dual`,
    desc: '数据库环境信息'
  },
  {
    group: 'rely',
    name: "db-var-info",
    sql: `SELECT NAME||'='||VALUE AS params from v$parameter WHERE NAME IN('processes','sessions','sga_max_size','cpu_count','spfile','instance_name','service_names')
UNION ALL
select 'log_mode='|| LOG_MODE from v$database
UNION ALL
select 'open_mode='|| OPEN_MODE from v$database
UNION ALL
select 'guard_status='|| GUARD_STATUS from v$database;

-- NLS
SELECT a.PARAMETER,a.VALUE GLOBAL_VALUE,b.VALUE SESSION_VALUE FROM v$NLS_PARAMETERS a FULL JOIN NLS_SESSION_PARAMETERS b ON a.PARAMETER=b.PARAMETER`,
    remark: '数据库参数信息'
  },
  {
    group: 'default',
    name: "database-name",
    sql: `SELECT USERNAME AS DatabaseName FROM ALL_USERS ORDER BY USERNAME`,
    desc: '数据库名称'
  },
  {
    group: 'default',
    name: "schema",
    sql: `-- 查询当前连接 Schema
SELECT SYS_CONTEXT('USERENV','CURRENT_SCHEMA') FROM DUAL;
-- 修改当前连接 Schema 为 SCOTT
ALTER SESSION SET CURRENT_SCHEMA=SCOTT`,
    desc: '数据库名称'
  },
  {
    group: 'default',
    name: "uuid",
    sql: `select rawtohex(sys_guid()) from dual`,
    desc: '生成 GUID'
  },
  {
    group: 'default',
    name: "manager",
    sql: `-- sqlplus 连接数据库，密码请勿带 @ 符号，巨坑
sqlplus system/oracle@(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(Host=localhost)(Port=1521))(CONNECT_DATA=(SID=orcl)))

-- 查看表空间物理文件的名称及大小
SELECT tablespace_name, file_id, file_name, round(bytes / (1024 * 1024), 0) total_space_MB FROM dba_data_files ORDER BY tablespace_name;

-- 查询用户对应表空间
select username, default_tablespace from dba_users;
-- 查看你能管理的所有用户
select * from all_users;
-- 查看当前用户信息
select * from user_users;

-- 创建表空间（大小 500M，每次 5M 自动增大，最大不限制）
-- create tablespace DSPACE datafile '/u01/app/oracle/oradata/EE/DNAME.dbf' size 500M autoextend on next 5M maxsize unlimited;
-- create tablespace DSPACE datafile 'c:\\oracle\\oradata\\test\\DNAME.dbf' size 500M autoextend on next 5M maxsize unlimited;

-- 删除表空间
-- drop tablespace DSPACE including contents and datafiles cascade constraint;

-- 创建用户并指定表空间
-- create user DNAME identified by DPWD default tablespace DSPACE;

-- 修改用户密码
alter user "DNAME" identified by "DPWD";

-- 为用户指定表空间
alter user DNAME default tablespace DSPACE;

-- 删除用户
drop user DNAME cascade;

-- 授予权限
-- connect：授予最终用户的典型权利，最基本的
-- resource：授予开发人员
-- dba：授予数据库所有的权限
grant connect, resource to DNAME;
grant dba to DNAME;
-- 撤销权限
revoke dba from DNAME;
-- 查询用户授予的角色权限
select * from dba_role_privs where grantee='DNAME';

-- 锁定用户
alter user DNAME account lock;
-- 解锁用户
alter user DNAME account unlock;

-- 账号过期查询
SELECT username, account_status, expiry_date FROM dba_users;
-- 密码过期策略
SELECT * FROM dba_profiles WHERE profile='DEFAULT' AND resource_name='PASSWORD_LIFE_TIME';`,
    desc: '管理、维护'
  },
  {
    group: 'default',
    name: "expdp-impdp",
    sql: `-- 按用户导出
expdp system/oracle@orcl schemas=$user dumpfile=$dbname.dmp logfile=$dbname.log directory=DATA_PUMP_DIR
-- 按表名导出
expdp system/oracle@orcl tables=($TABLE1,$TABLE2) dumpfile=$dbname.dmp logfile=$dbname.log directory=DATA_PUMP_DIR
-- 按用户导入（表覆盖）
impdp system/oracle@orcl schemas=$user dumpfile=$dbname.dmp logfile=$dbname.log directory=DATA_PUMP_DIR table_exists_action=REPLACE
-- 按用户导入（转换空间）
impdp system/oracle@orcl schemas=$user TRANSFORM=segment_attributes:n dumpfile=$dbname.dmp logfile=$dbname.log directory=DATA_PUMP_DIR table_exists_action=REPLACE

-- 导入导出包权限设置
chown oracle -R /u01/app/oracle/admin/EE/dpdump/

-- DIRECTORY 参数说明
-- 查看管理员目录
select * from dba_directories
-- 创建逻辑目录，还需手动创建目录，Oracle 不关心目录是否存在，不存在会报错
create directory DP_DIR as 'C:\\app\\Administrator/admin/orcl/dpdump/'
-- DUMPFILE 指定的 dmp 文件应放在 DIRECTORY 目录下
-- 删除逻辑目录
drop directory DP_DIR`,
    desc: '导入导出'
  },
  {
    group: 'default',
    name: "common-function",
    sql: `select sysdate, sys_guid(), rawtohex(sys_guid()) from dual`,
    desc: '常用函数'
  },
  {
    group: 'default',
    name: "cursor-query",
    sql: `-- 接收参数 cmd.Parameters.Add(cursor, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output)
BEGIN
    OPEN :o1 for SELECT 1,2 FROM dual;
    OPEN :o2 for SELECT 3,4 FROM dual;
END;`,
    remark: '游标多表查询'
  },
  {
    group: 'default',
    name: "empty",
    sql: ``
  },
];

ndkNoteSQL["SQLServer"] = [
  {
    group: 'rely',
    name: "db-env-info",
    sql: `SELECT 'name' col, SUBSTRING(@@VERSION, 1, CHARINDEX(' - ', @@VERSION, 1)) + ' ' + CONVERT(varchar(99), SERVERPROPERTY('Edition')) val
UNION ALL
SELECT 'version' col, SERVERPROPERTY('ProductVersion') val
UNION ALL
SELECT 'dir_data' col, SERVERPROPERTY('InstanceDefaultDataPath') val
UNION ALL
SELECT 'charset' col, SERVERPROPERTY('Collation') val
UNION ALL
SELECT 'datetime' col, GETDATE() val
UNION ALL
SELECT 'time_out' col, value_in_use val FROM sys.configurations WHERE description = 'remote query timeout'
UNION ALL
SELECT 'max_conn' col, @@MAX_CONNECTIONS val
UNION ALL
SELECT 'curr_conn' col, ( SELECT COUNT(dbid) FROM sys.sysprocesses ) val
UNION ALL
SELECT 'ignore_case' col, ( CASE WHEN 'a' = 'A' THEN 1 ELSE 0 END ) val
UNION ALL
SELECT 'system' col, REPLACE( RIGHT( @@VERSION, CHARINDEX(CHAR(10), REVERSE(@@VERSION), 2) - 2 ), CHAR(10), '' ) val;

EXEC master.dbo.Xp_instance_regread N'HKEY_LOCAL_MACHINE',
N'SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation',
N'TimeZoneKeyName'`,
    remark: '数据库环境信息'
  },
  {
    group: 'default',
    name: "database-name",
    sql: `SELECT name AS DatabaseName FROM sys.databases ORDER BY name`
  },
  {
    group: 'default',
    name: "uuid",
    sql: `select newid()`
  },
  {
    group: 'default',
    name: "query-history",
    sql: `SELECT QS.creation_time, SUBSTRING(ST.text, (QS.statement_start_offset / 2)+1, ((CASE QS.statement_end_offset WHEN-1 THEN DATALENGTH(st.text)ELSE QS.statement_end_offset END-QS.statement_start_offset)/ 2)+1) AS statement_text, ST.text, QS.total_worker_time, QS.last_worker_time, QS.max_worker_time, QS.min_worker_time
FROM sys.dm_exec_query_stats QS
     CROSS APPLY sys.dm_exec_sql_text(QS.sql_handle) ST
WHERE ST.text LIKE '%%' AND QS.creation_time BETWEEN dateadd(MM, -1, GETDATE())AND GETDATE()
ORDER BY QS.creation_time DESC`,
    remark: '查询历史'
  },
  {
    group: 'default',
    name: "connection-info",
    sql: `SELECT a.spid, b.name, 'kill ' + CONVERT(varchar, a.spid) + ';' AS kill_command FROM MASTER..SysProcesses a LEFT JOIN sys.databases b ON b.database_id = a.dbid`,
    remark: '数据库连接信息、关闭连接'
  },
  {
    group: 'default',
    name: "table-size",
    sql: `-- 定义表变量
DECLARE @T TABLE([name] VARCHAR(99),
[rows] INT,
reserved VARCHAR(99),
data_size VARCHAR(99),
index_size VARCHAR(99),
unused VARCHAR(99));

-- 将表占用情况存放到表变量
INSERT INTO @T EXEC sp_MSforeachtable "exec sp_spaceused '?'";
SELECT * FROM @T`,
    remark: '表大小情况'
  },
  {
    group: 'default',
    name: "identity_cache",
    sql: `alter database SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF -- 关闭自增缓存
select * from sys.database_scoped_configurations where name = 'IDENTITY_CACHE' -- 查询自增缓存状态`,
    remark: '自增缓存'
  },
  {
    group: 'default',
    name: "sleep",
    sql: `SELECT GETDATE()
WAITFOR DELAY N'00:00:01.500'
SELECT GETDATE()`
  },
  {
    group: 'default',
    name: "datetime-function",
    sql: `declare @local datetime=getdate()
declare @utc datetime=getutcdate()
declare @utc8 datetime=dateadd(hour, 8, getutcdate())
print N'当前时间：'+CONVERT(varchar(99), @local, 21)
print N'UTC：'+CONVERT(varchar(99), @utc, 21)
print N'UTC +8：'+CONVERT(varchar(99), @utc8, 21)
print N'格式化（yyyy-MM-dd HH:mm:ss.fff）：'+CONVERT(varchar(99), @utc8, 21)
print N'格式化（yyyy-MM-dd HH:mm:ss）：'+CONVERT(varchar(99), @utc8, 20)
print N'格式化（yyyy-MM-dd）：'+CONVERT(varchar(99), @utc8, 23)
print N'格式化（yyyy.MM.dd）：'+CONVERT(varchar(99), @utc8, 102)
print N'格式化（yyyy/MM/dd）：'+CONVERT(varchar(99), @utc8, 111)
print N'格式化（yyyyMMdd）：'+CONVERT(varchar(99), @utc8, 112)
print N'格式化（HH:mm:ss）：'+CONVERT(varchar(99), @utc8, 24)
print N'追加一天：'+CONVERT(varchar(99), dateadd(day, 1, @utc8), 21)`,
    remark: '时间函数'
  },
  {
    group: 'default',
    name: "guid",
    sql: `print newid()`
  },
  {
    group: 'default',
    name: "sp_GetDDL",
    sql: `-- ref: https://www.stormrage.com/SQLStuff/sp_GetDDL_Latest.txt`,
    remark: '获取表结构'
  },
  {
    group: 'default',
    name: "empty",
    sql: ``
  },
];

ndkNoteSQL["PostgreSQL"] = [
  {
    group: 'rely',
    name: "db-env-info",
    sql: `SELECT 'name' col, split_part(split_part(VERSION(), ',', 1), ' on ', 1) val
UNION ALL
SELECT 'version' col, ( SELECT split_part(setting, ' ', 1) FROM pg_settings WHERE NAME = 'server_version' ) val
UNION ALL
SELECT 'compile' col, split_part(VERSION(), ',', 2) val
UNION ALL
SELECT 'dir_install' col, ( SELECT split_part(setting, 'main', 1) FROM pg_settings WHERE NAME = 'archive_command' ) val
UNION ALL
SELECT 'dir_data' col, ( SELECT setting FROM pg_settings WHERE NAME = 'data_directory' ) val
UNION ALL
SELECT 'charset' col, ( SELECT setting FROM pg_settings WHERE NAME = 'server_encoding' ) val
UNION ALL
SELECT 'time_zone' col, ( SELECT setting FROM pg_settings WHERE NAME = 'TimeZone' ) val
UNION ALL
SELECT 'datetime' col, to_char(now(), 'YYYY-MM-DD HH24:MI:SS.MS') val
UNION ALL
SELECT 'max_conn' col, ( SELECT setting FROM pg_settings WHERE NAME = 'max_connections' ) val
UNION ALL
SELECT 'curr_conn' col, CAST(COUNT(1) AS VARCHAR) val FROM pg_stat_activity
UNION ALL
SELECT 'time_out' col, ( SELECT setting FROM pg_settings WHERE NAME = 'statement_timeout' ) val
UNION ALL
SELECT 'ignore_case' col, CASE 'a' = 'A' WHEN 't' THEN '1' ELSE '0' END val
UNION ALL
SELECT 'system' col, split_part(split_part(VERSION(), ',', 1), ' on ', 2) val`,
    remark: '数据库环境信息'
  },
  {
    group: 'default',
    name: "database-name",
    sql: `SELECT datname AS DatabaseName FROM pg_database ORDER BY datname`
  },
  {
    group: 'default',
    name: "sleep",
    sql: `select clock_timestamp(), pg_sleep(1), clock_timestamp();`
  },
  {
    group: 'default',
    name: "uuid",
    sql: `select uuid_generate_v4()`,
    remark: '生成 uuid , 须添加扩展：create extension "uuid-ossp";'
  },
  {
    group: 'default',
    name: "print",
    sql: `DO $$
BEGIN
    RAISE NOTICE '当前日期时间：%', now();
    RAISE NOTICE '版本信息：%', version();
END
$$;
`,
    remark: '打印输出'
  },
  {
    group: 'default',
    name: "empty",
    sql: ``
  },
];

export { ndkNoteSQL }