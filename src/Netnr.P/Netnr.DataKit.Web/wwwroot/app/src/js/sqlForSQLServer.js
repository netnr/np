var sqlForSQLServer = [
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
DECLARE @T TABLE([name] VARCHAR(100),
[rows] INT,
reserved VARCHAR(100),
data_size VARCHAR(100),
index_size VARCHAR(100),
unused VARCHAR(100));

-- 将表占用情况存放到表变量
INSERT INTO @T EXEC sp_MSforeachtable "exec sp_spaceused '?'";
SELECT * FROM @T`
    },
    {
        name: "时间函数",
        sql: `declare @local datetime=getdate()
declare @utc datetime=getutcdate()
declare @utc8 datetime=dateadd(hour, 8, getutcdate())
print N'当前时间：'+CONVERT(varchar(100), @local, 21)
print N'UTC：'+CONVERT(varchar(100), @utc, 21)
print N'UTC +8：'+CONVERT(varchar(100), @utc8, 21)
print N'格式化（yyyy-MM-dd HH:mm:ss.fff）：'+CONVERT(varchar(100), @utc8, 21)
print N'格式化（yyyy-MM-dd HH:mm:ss）：'+CONVERT(varchar(100), @utc8, 20)
print N'格式化（yyyy-MM-dd）：'+CONVERT(varchar(100), @utc8, 23)
print N'格式化（yyyy.MM.dd）：'+CONVERT(varchar(100), @utc8, 102)
print N'格式化（yyyy/MM/dd）：'+CONVERT(varchar(100), @utc8, 111)
print N'格式化（yyyyMMdd）：'+CONVERT(varchar(100), @utc8, 112)
print N'格式化（HH:mm:ss）：'+CONVERT(varchar(100), @utc8, 24)
print N'追加一天：'+CONVERT(varchar(100), dateadd(day, 1, @utc8), 21)`
    },
    {
        name: "内置方法、对象",
        sql: `print NEWID()`
    },
    {
        name: "",
        sql: ``
    },
]

export { sqlForSQLServer }