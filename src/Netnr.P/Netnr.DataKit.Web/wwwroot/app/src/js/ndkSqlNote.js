var ndkSqlNote = {};

ndkSqlNote["SQLite"] = [
    {
        name: "库、表信息",
        sql: `select sqlite_version(); -- 版本
PRAGMA database_list; -- 附加数据库
PRAGMA encoding; -- 编码

select * from main.sqlite_master; -- 所有表信息、DDL
PRAGMA table_info('SysButton'); -- 表信息
select cid, name, type, [notnull], quote(dflt_value) as dflt_value, pk from pragma_table_info('SysUser'); -- 表信息

select m.*, p.cid, p.name as colname, p.type as coltype, p.[notnull], quote(p.dflt_value) as dflt_value, p.pk
from main.sqlite_master m left join pragma_table_info(m.name) p ON m.name<>p.name; -- 所有表信息
`
    },
    {
        name: "时间函数",
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
select '格式化（HH mm ss fff)', strftime('%H %M %S %s','now')`
    },
    {
        name: "内置命令",
        sql: `VACUUM; -- 磁盘空间释放

select hex(randomblob(16)); -- 模拟 UUID`
    },
    {
        name: "",
        sql: ``
    },
];

ndkSqlNote["MySQL"] = ndkSqlNote["MariaDB"] = [
    {
        name: "show 常用",
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
show master status; -- 主状态`
    },
    {
        name: "时间函数",
        sql: `select now(), sysdate(), sleep(1)
union all
select now(), sysdate(), 0; -- 当前日期时间，now 执行开始得到值 sysdate 实时获取

select concat('日期：',current_date(),' , 时间：',current_time()) '日期、时间' -- 日期、时间
union all
select concat('UTC日期：',utc_date(),' , UTC时间：',utc_time());

select utc_timestamp(); -- utc日期时间
select unix_timestamp(now()); -- 转时间戳
select date_format(now(), '%Y/%m/%d %H:%i:%s'); -- 格式化`
    },
    {
        name: "内置方法、对象",
        sql: `select uuid(); -- UUID

-- 字符串字节长度、字符长度
SELECT
  LENGTH("demo") "英文(demo)",
  LENGTH("测试。") "中文符号(测试。)",
  CHAR_LENGTH("demo") "CHAR - 英文(demo)",
  CHAR_LENGTH("测试。") "CHAR - 中文符号(测试。)";`
    },
    {
        name: "",
        sql: ``
    },
];

ndkSqlNote["Oracle"] = [
    {
        name: "管理、维护",
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
-- create tablespace DSPACE datafile 'c:\oracle\oradata\test\DNAME.dbf' size 500M autoextend on next 5M maxsize unlimited;

-- 删除表空间
-- drop tablespace DSPACE including contents and datafiles cascade constraint;

-- 创建用户并指定表空间
-- create user DNAME identified by DPWD default tablespace DSPACE;

-- 修改用户密码
alter user DNAME identified by DPWD;

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
SELECT * FROM dba_profiles WHERE profile='DEFAULT' AND resource_name='PASSWORD_LIFE_TIME';`
    },
    {
        name: "导入导出",
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
create directory DP_DIR as 'C:\app\Administrator/admin/orcl/dpdump/'
-- DUMPFILE 指定的 dmp 文件应放在 DIRECTORY 目录下
-- 删除逻辑目录
drop directory DP_DIR`
    },
    {
        name: "内置方法、对象",
        sql: `select sysdate, sys_guid(), rawtohex(sys_guid()) from dual`
    },
    {
        name: "",
        sql: ``
    },
];

ndkSqlNote["SQLServer"] = [
    {
        name: "查看数据库信息",
        sql: `SELECT 'Name' col, SUBSTRING(@@VERSION, 1, CHARINDEX(' - ', @@VERSION, 1))+' '+CONVERT(varchar(99), SERVERPROPERTY('Edition')) val
UNION ALL
SELECT 'Version' col, SERVERPROPERTY('ProductVersion') val
UNION ALL
SELECT 'DirData' col, SERVERPROPERTY('InstanceDefaultDataPath') val
UNION ALL
SELECT 'CharSet' col, SERVERPROPERTY('Collation') val
UNION ALL
SELECT 'DateTime' col, GETDATE() val
UNION ALL
SELECT 'MaxConn' col, @@MAX_CONNECTIONS val
UNION ALL
SELECT 'CurrConn' col, (SELECT COUNT(dbid)FROM sys.sysprocesses) val
UNION ALL
SELECT 'IgnoreCase' col, (CASE WHEN 'a'='A' THEN 1 ELSE 0 END) val
UNION ALL
SELECT 'System' col, REPLACE(RIGHT(@@VERSION, CHARINDEX(CHAR(10), REVERSE(@@VERSION), 2)-2), CHAR(10), '') val`
    },
    {
        name: "远程查询超时",
        sql: `EXEC Sp_configure 'remote query timeout'`
    },
    {
        name: "执行脚本历史记录",
        sql: `SELECT QS.creation_time, SUBSTRING(ST.text, (QS.statement_start_offset / 2)+1, ((CASE QS.statement_end_offset WHEN-1 THEN DATALENGTH(st.text)ELSE QS.statement_end_offset END-QS.statement_start_offset)/ 2)+1) AS statement_text, ST.text, QS.total_worker_time, QS.last_worker_time, QS.max_worker_time, QS.min_worker_time
FROM sys.dm_exec_query_stats QS
     CROSS APPLY sys.dm_exec_sql_text(QS.sql_handle) ST
WHERE ST.text LIKE '%%' AND QS.creation_time BETWEEN dateadd(MM, -1, GETDATE())AND GETDATE()
ORDER BY QS.creation_time DESC`
    },
    {
        name: "获取数据库名",
        sql: `SELECT name AS DatabaseName FROM sys.databases ORDER BY name`
    },
    {
        name: "表大小情况",
        sql: `-- 定义表变量
DECLARE @T TABLE([name] VARCHAR(99),
[rows] INT,
reserved VARCHAR(99),
data_size VARCHAR(99),
index_size VARCHAR(99),
unused VARCHAR(99));

-- 将表占用情况存放到表变量
INSERT INTO @T EXEC sp_MSforeachtable "exec sp_spaceused '?'";
SELECT * FROM @T`
    },
    {
        name: "时间函数",
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
print N'追加一天：'+CONVERT(varchar(99), dateadd(day, 1, @utc8), 21)`
    },
    {
        name: "内置方法、对象",
        sql: `print NEWID()`
    },
    {
        name: "",
        sql: ``
    },
];

ndkSqlNote["PostgreSQL"] = [
    {
        name: "查看信息",
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
];

export { ndkSqlNote }