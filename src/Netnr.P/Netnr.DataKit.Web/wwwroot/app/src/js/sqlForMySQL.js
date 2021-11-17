var sqlForMySQL = [
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
        sql: `select uuid()`
    },
    {
        name: "",
        sql: ``
    },
]

export { sqlForMySQL }