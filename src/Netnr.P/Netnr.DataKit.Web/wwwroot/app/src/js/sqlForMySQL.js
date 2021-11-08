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
        name: "",
        sql: ``
    },
    {
        name: "",
        sql: ``
    },
]

export { sqlForMySQL }