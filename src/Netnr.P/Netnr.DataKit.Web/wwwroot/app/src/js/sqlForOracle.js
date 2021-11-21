var sqlForOracle = [
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
]

export { sqlForOracle }