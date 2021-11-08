var sqlForSQLite = [
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
        name: "内置命令",
        sql: `VACUUM; -- 磁盘空间释放

select hex(randomblob(16)); -- 模拟 UUID`
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
    }
]

export { sqlForSQLite }