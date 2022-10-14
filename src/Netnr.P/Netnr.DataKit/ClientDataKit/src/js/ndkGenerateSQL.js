import { ndkGenerateDDL } from './ndkGenerateDDL';
import { ndkFunction } from './ndkFunction';
import { ndkI18n } from './ndkI18n';
import { ndkStep } from './ndkStep';
import { ndkTab } from './ndkTab';
import { ndkVary } from './ndkVary';
import { ndkRequest } from './ndkRequest';
import { ndkExecute } from './ndkExecute';
import { agg } from './agg';

// 生成 sql 语句
var ndkGenerateSQL = {
    sqlDataType: (tdb, column) => {
        var colComment = (column.ColumnComment || "").trim().replace(/[\r\n]/g, "").replace(/\s+/g, " "),
            vlen = column.DataLength > 0 ? column.DataLength : 999,
            dval = colComment.replaceAll("'", "''").substring(0, vlen);

        var dtout = {
            category: 'string', // 大类：string, number, boolean, date
            comment: colComment, // 注释
            defaultValue: "''", // 默认值
            mockValue: `'${dval}'` // 模拟值
        };

        column.DataType = column.DataType.toLowerCase();
        //数据库类型
        switch (column.DataType) {
            case "int":
            case "bigint":
            case "tinyint":
            case "smallint":
            case "mediumint":
            case "decimal":
            case "float":
            case "double":
            case "number":
            case "int2":
            case "int4":
            case "int8":
            case "integer":
            case "serial":
            case "float4":
            case "float8":
            case "real":
            case "numeric":
            case "money":
                Object.assign(dtout, {
                    category: 'number',
                    defaultValue: 0,
                    mockValue: 0
                });
                break;

            case "date":
            case "datetime":
            case "timestamp":
            case "time":
            case "year":
                Object.assign(dtout, {
                    category: 'date',
                    mockValue: `'${ndkFunction.formatDateTime(column.DataType)}'`
                });
                break;

            case "bool":
            case "boolean":
                Object.assign(dtout, {
                    category: 'boolean',
                    defaultValue: "false",
                    mockValue: "false"
                });
                break;
        }

        switch (tdb) {
            case "SQLite":
                {
                    //主键
                    if (["nvarchar", "varchar", "text"].includes(column.DataType) && column.PrimaryKey > 0) {
                        dtout.mockValue = "hex(randomblob(16))"
                    }
                }
                break;
            case "MySQL":
            case "MariaDB":
                {
                    //主键
                    if (['varchar'].includes(column.DataType) && column.DataLength >= 36 && column.PrimaryKey > 0) {
                        dtout.mockValue = "uuid()"
                    }
                }
                break;
            case "Oracle":
                {
                    //主键
                    if (['varchar', 'varchar2'].includes(column.DataType) && column.DataLength >= 36 && column.PrimaryKey > 0) {
                        dtout.mockValue = "sys_guid()"
                    }
                }
                break;
            case "SQLServer":
                {
                    //主键
                    if (['varchar', 'nvarchar'].includes(column.DataType) && column.DataLength >= 36 && column.PrimaryKey > 0) {
                        dtout.mockValue = "newid()"
                    } else if (["nvarchar", "nchar", "ntext"].includes(column.DataType)) {
                        dtout.mockValue = `N'${dval}'`
                    }
                }
                break;
            case "PostgreSQL":
                {
                    //主键
                    if (['varchar', 'varchar2'].includes(column.DataType) && column.DataLength >= 36 && column.PrimaryKey > 0) {
                        dtout.mockValue = "uuid_generate_v4()"
                    }
                }
                break;
        }

        return dtout;
    },

    /**
     * SQL 引用符号
     * @param {any} type
     * @param {any} key
     * @param {any} force
     */
    sqlQuote: (type, key, force = false) => {
        //非强制、不带符号的直接返回
        if (force == false && ndkVary.parameterConfig.buildSqlWithQuote.value == false) {
            return key;
        }

        switch (type) {
            case "SQLite":
            case "SQLServer":
                return `[${key}]`
            case "MySQL":
            case "MariaDB":
                return '`' + key + '`'
            case "Oracle":
            case "PostgreSQL":
                return `"${key}"`
            default:
                return key
        }
    },

    /**
     * SQL 完整表名
     * @param {any} cp
     * @param {any} tableRow
     */
    sqlFullTableName: (cp, tableRow) => {
        var ftn = [ndkGenerateSQL.sqlQuote(cp.cobj.type, cp.databaseName)];
        if (!["MySQL", "MariaDB"].includes(cp.cobj.type) && tableRow.SchemaName != null && tableRow.SchemaName != "") {
            ftn.push(ndkGenerateSQL.sqlQuote(cp.cobj.type, tableRow.SchemaName))
        }
        ftn.push(ndkGenerateSQL.sqlQuote(cp.cobj.type, tableRow.TableName))

        return ftn.join('.')
    },

    /**
     * 生成 SQL 语句
     * @param {*} name 菜单名
     */
    buildNewTabSql: (name) => {
        var srows = agg.getSelectedOrRangeRows(ndkVary.gridOpsTable);
        if (srows.length) {
            var cp = ndkStep.cpGet(1);

            var tabTitle = ndkVary.iconSvg("table", srows[0].TableName);
            var sqlOrPromise;

            if (name == "DDL") {
                sqlOrPromise = ndkGenerateDDL.reqDDL(cp.cobj, cp.databaseName, srows);
            } else {
                //请求列
                sqlOrPromise = new Promise((resolve) => {
                    ndkRequest.reqColumn(cp.cobj, cp.databaseName, srows.map(x => x.TableName).join(','), true).then(tableColumns => {
                        //按表分组依次生成
                        var sqls = [];
                        ndkFunction.groupBy(tableColumns, x => x.TableName).forEach(tableName => {
                            var tableRow = srows.find(x => x.TableName == tableName);
                            var columns = tableColumns.filter(x => x.TableName == tableName);
                            //构建SQL
                            var sql = ndkGenerateSQL[`build${name}Sql`](cp, tableRow, columns);
                            sqls.push(sql);
                        });
                        if (name == "Count") {
                            sqls = sqls.join('\r\nUNION ALL\r\n');
                        }
                        else {
                            sqls = sqls.join(';\r\n');
                        }

                        resolve(sqls);
                    })
                });
            }

            ndkTab.tabBuildFast_sql(cp, tabTitle, sqlOrPromise);
        }
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     * @param {any} columns
     */
    buildSelectSql: (cp, tableRow, columns) => {
        var tableName = ndkGenerateSQL.sqlFullTableName(cp, tableRow), cols = [];
        if (columns) {
            columns.forEach(column => cols.push(ndkGenerateSQL.sqlQuote(cp.cobj.type, column.ColumnName)));
            return `SELECT ${cols.join(', ')} FROM ${tableName}`
        } else {
            var topn = ndkVary.parameterConfig.selectDataLimit.value;
            switch (cp.cobj.type) {
                case "SQLServer":
                    return `SELECT TOP ${topn} * FROM ${tableName}`
                case "MySQL":
                case "MariaDB":
                    return `SELECT * FROM ${tableName} LIMIT 0,${topn}`
                case "Oracle":
                    return `SELECT * FROM ${tableName} WHERE ROWNUM <= ${topn}`
                case "SQLite":
                case "PostgreSQL":
                    return `SELECT * FROM ${tableName} LIMIT ${topn} OFFSET 0`
            }
        }
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     * @param {any} columns
     */
    buildInsertSql: (cp, tableRow, columns) => {
        var tableName = ndkGenerateSQL.sqlFullTableName(cp, tableRow), cols = [], vals = [],
            withComment = ndkVary.parameterConfig.buildSqlWithComment.value;
        columns.forEach(column => {
            cols.push(ndkGenerateSQL.sqlQuote(cp.cobj.type, column.ColumnName));
            var sdt = ndkGenerateSQL.sqlDataType(cp.cobj.type, column);
            //带注释
            if (withComment) {
                var fieldComment = `-- ${column.ColumnName}(${column.PrimaryKey > 0 ? ndkVary.emoji.key + column.PrimaryKey + ", " : ""}${column.ColumnType}, ${column.IsNullable == 1 ? "null" : "not null"}) ${sdt.comment}`;
                vals.push(`\n  ${fieldComment}\n  ${sdt.mockValue}`);
            } else {
                vals.push(`${sdt.mockValue}`);
            }
        });
        return `INSERT INTO ${tableName}(${cols.join(', ')}) VALUES (${vals.join(', ')})`
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     * @param {any} columns
     * @param {any} rows
     */
    buildUpdateSql: (cp, tableRow, columns, rows) => {
        var tableName = ndkGenerateSQL.sqlFullTableName(cp, tableRow), cols = [], wcol = [],
            withComment = ndkVary.parameterConfig.buildSqlWithComment.value;
        columns.forEach(column => {
            var field = ndkGenerateSQL.sqlQuote(cp.cobj.type, column.ColumnName);
            var sdt = ndkGenerateSQL.sqlDataType(cp.cobj.type, column), setkv;
            //带注释
            if (withComment) {
                var fieldComment = `-- ${column.ColumnName}(${column.PrimaryKey > 0 ? ndkVary.emoji.key + column.PrimaryKey + ", " : ""}${column.ColumnType}, ${column.IsNullable == 1 ? "null" : "not null"}) ${sdt.comment}`;
                setkv = `\n  ${fieldComment}\n  ${field} = ${sdt.defaultValue}`;
            } else {
                setkv = `${field} = ${sdt.defaultValue}`;
            }

            if (column.PrimaryKey > 0) {
                wcol.push(setkv);
            } else {
                cols.push(setkv);
            }
        });
        if (wcol.length == 0) {
            wcol.push(cols[0]);
            cols = cols.splice(0, 1);
        }
        var where = withComment ? "\nWHERE" : "WHERE";

        return `UPDATE ${tableName} SET ${cols.join(', ')} ${where} ${wcol.join(' AND ')}`
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     * @param {any} columns
     * @param {any} rows
     */
    buildDeleteSql: (cp, tableRow, columns, rows) => {
        var tableName = ndkGenerateSQL.sqlFullTableName(cp, tableRow), cols = [], wcol = [];
        columns.forEach(column => {
            var field = ndkGenerateSQL.sqlQuote(cp.cobj.type, column.ColumnName);
            var sdt = ndkGenerateSQL.sqlDataType(cp.cobj.type, column);
            var setkv = `${field} = ${sdt.defaultValue}`;

            if (cols.length == 0) {
                cols.push(setkv);
            }
            if (column.PrimaryKey > 0) {
                wcol.push(setkv);
            }
        });
        if (wcol.length == 0) {
            wcol = cols;
        }

        return `DELETE FROM ${tableName} WHERE ${wcol.join(' AND ')}`
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     */
    buildTruncateSql: (cp, tableRow) => {
        var tableName = ndkGenerateSQL.sqlFullTableName(cp, tableRow);

        return `TRUNCATE TABLE ${tableName}`
    },
    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     */
    buildDropSql: (cp, tableRow) => {
        var tableName = ndkGenerateSQL.sqlFullTableName(cp, tableRow);

        return `DROP TABLE ${tableName}`
    },
    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     */
    buildCountSql: (cp, tableRow) => {
        var tableName = ndkGenerateSQL.sqlFullTableName(cp, tableRow);

        return `SELECT '${tableRow.TableName}' AS name, COUNT(*) AS total FROM ${tableName}`
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} databaseRows
     */
    buildDropDatabaseSql: (cp, databaseRows) => new Promise((resolve) => {
        var names = ndkFunction.arrayDistinct(databaseRows.map(x => x.DatabaseName));
        var tableSpace = ndkFunction.arrayDistinct(databaseRows.filter(x => x.DatabaseSpace != null).map(x => x.DatabaseSpace));
        var sqls = [];
        switch (cp.cobj.type) {
            case "MySQL":
            case "MariaDB":
                {
                    names.forEach(name => sqls.push(`drop database if exists ${name}`));
                    resolve(sqls.join(';\r\n'));
                }
                break;
            case "Oracle":
                {
                    names.forEach(name => sqls.push(`drop user "${name}" cascade;`));
                    if (tableSpace.length) {
                        sqls.push('\r\n-- drop tablespace 删除表空间（可选）');
                        tableSpace.forEach(ts => sqls.push(`-- drop tablespace "${ts}" including contents and datafiles cascade constraint;`));
                    }
                    resolve(sqls.join('\r\n'));
                }
                break;
            case "SQLServer":
                {
                    //https://stackoverflow.com/questions/33890085
                    sqls.push('USE master;');
                    names.forEach(name => sqls.push(`alter database [${name}] set single_user with rollback immediate;\r\ndrop database [${name}];`));
                    resolve(sqls.join('\r\n\r\n'));
                }
                break;
            case "PostgreSQL":
                {
                    //https://dba.stackexchange.com/questions/11893
                    //查询版本号
                    var sqlVersion = "SELECT setting FROM pg_settings WHERE NAME = 'server_version'";
                    ndkExecute.executeSql(cp.cobj, null, sqlVersion).then(data => {
                        var ver = parseFloat(ndkExecute.getDataCell(data));
                        if (ver >= 13) {
                            names.forEach(name => sqls.push(`drop database if exists "${name}" with (force);`));
                            if (tableSpace.length) {
                                sqls.push('\r\n-- drop tablespace 删除表空间（可选）');
                                tableSpace.forEach(ts => sqls.push(`-- drop tablespace if exists "${ts}";`));
                            }
                            resolve(sqls.join('\r\n'));
                        } else {
                            names.forEach(name => {
                                sqls.push(`-- Disallow new connections 禁止新连接
UPDATE pg_database SET datallowconn = 'false' WHERE datname = '${name}';
alter database "${name}" connection limit 1;
-- Terminate existing connections 终止现有连接
SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '${name}';
-- Drop database 删除库
drop database "${name}";`);
                            });
                            if (tableSpace.length) {
                                sqls.push('\r\n-- drop tablespace 删除表空间（可选）');
                                tableSpace.forEach(ts => sqls.push(`-- drop tablespace if exists "${ts}";`));
                            }
                            resolve(sqls.join('\r\n\r\n'));
                        }
                    })
                }
                break;
        }
    }),

    /**
     * 构建SQL
     * @param {*} cp 
     * @param {*} databaseRow 
     */
    buildCreateDatabaseSql: async (cp, databaseRow) => {
        var isOracle = cp.cobj.type == "Oracle";
        databaseRow = Object.assign(databaseRow || {}, {
            DatabaseName: isOracle ? "NEWDB" : "newdb",
            DatabaseSpace: "NEWTS"
        });

        var sqls = [];
        switch (cp.cobj.type) {
            case "MySQL":
            case "MariaDB":
                //默认
                Object.assign(databaseRow, { DatabaseCharset: "utf8mb4", DatabaseCollation: "utf8mb4_0900_ai_ci" });
                sqls.push(`create database if not exists ${ndkGenerateSQL.sqlQuote(cp.cobj.type, databaseRow.DatabaseName)} character set ${ndkGenerateSQL.sqlQuote(cp.cobj.type, databaseRow.DatabaseCharset)} collate ${ndkGenerateSQL.sqlQuote(cp.cobj.type, databaseRow.DatabaseCollation)};`);
                break;
            case "Oracle":
                {
                    if (databaseRow.DatabasePath == null || databaseRow.DatabasePath == "") {
                        var sqlTableSpacePath = "SELECT file_name FROM dba_data_files WHERE tablespace_name IN('SYSTEM','USERS') AND rownum=1";
                        var edata = await ndkExecute.executeSql(cp.cobj, cp.databaseName, sqlTableSpacePath);
                        databaseRow.DatabasePath = ndkExecute.getDataCell(edata);
                    }
                    var path = ndkFunction.getFullPath(databaseRow.DatabasePath);
                    databaseRow.DatabasePath = `${path}${databaseRow.DatabaseSpace}01.dbf`;

                    sqls.push(`-- create tablespace 创建表空间（起始大小 200M，每次 100M 自动增大，最大不限制）`);
                    sqls.push(`create tablespace "${databaseRow.DatabaseSpace}" datafile '${databaseRow.DatabasePath}' size 200M autoextend on next 100M maxsize unlimited;`);
                    sqls.push('-- create user 创建用户并指定表空间');
                    sqls.push(`create user "${databaseRow.DatabaseName}" identified by "NewPwd" default tablespace "${databaseRow.DatabaseSpace}";`);
                    sqls.push('-- grant 授予权限（connect：基本；resource：开发；dba：管理）');
                    sqls.push(`grant connect, resource, dba to "${databaseRow.DatabaseName}"`);
                }
                break;
            case "SQLServer":
                //默认
                Object.assign(databaseRow, { DatabaseCollation: "Chinese_PRC_CI_AS" });
                sqls.push(`use master;
create database [${databaseRow.DatabaseName}] on primary
(
    name='${databaseRow.DatabaseName}', filename=N'/var/opt/mssql/data/${databaseRow.DatabaseName}.mdf',
    size=20mb, filegrowth=10mb
)
log on
(
    name='${databaseRow.DatabaseName}_log', filename=N'/var/opt/mssql/data/${databaseRow.DatabaseName}_log.ldf',
    size=20mb, filegrowth=10mb
)
collate ${databaseRow.DatabaseCollation}`);
                break;
            case "PostgreSQL":

                break;
        }

        return sqls.join('\r\n');
    },

    /**
     * 构建SQL
     * @param {*} cp 
     * @param {*} name 
     */
    buildConnSeeSql: (cp, name) => {
        var sqls = [];
        switch (cp.cobj.type) {
            case "SQLite":
                switch (name) {
                    case "CharSet":
                        sqls.push("PRAGMA encoding");
                        break;
                    case "Collation":
                        sqls.push(`-- https://www.sqlite.org/datatype3.html
select 'COLLATE BINARY' name, '二进制比较（默认）' description
union all
select 'COLLATE NOCASE', '转小写比较'
union all
select 'COLLATE RTRIM', '忽略末尾空格比较'`);
                        break;
                    case "DataType":
                        sqls.push(`-- https://www.sqlite.org/datatype3.html
select 'NULL' name, 'NULL 值' description
union all
select 'INTEGER', '有符号整数，按大小被存储成1,2,3,4,6或8字节'
union all
select 'REAL', '浮点数，以8字节指数形式存储'
union all
select 'TEXT', '字符串，以数据库编码方式存储（UTF-8, UTF-16BE 或 UTF-16-LE）'
union all
select 'BLOB', 'BLOB 数据不做任何转换，以输入形式存储'`);
                        break;
                }
                break;
            case "MySQL":
            case "MariaDB":
                switch (name) {
                    case "Status":
                        sqls.push(`select * from performance_schema.session_status; -- show session status;
select * from performance_schema.global_status; -- show global status;`);
                        break;
                    case "User":
                        sqls.push("select * from mysql.user");
                        break;
                    case "Session":
                        sqls.push(`show full processlist
-- show engine innodb status
-- show open tables where In_use > 0`);
                        break;
                    case "CharSet":
                        sqls.push("show charset");
                        break;
                    case "Collation":
                        sqls.push("show collation");
                        break;
                    case "DataType":
                        sqls.push('SELECT DISTINCT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS');
                        break;
                }
                break;
            case "Oracle":
                switch (name) {
                    case "User":
                        sqls.push("SELECT * FROM DBA_USERS");
                        break;
                    case "Role":
                        sqls.push("SELECT * FROM DBA_ROLES");
                        break;
                    case "Session":
                        sqls.push("SELECT SID,MACHINE,USERNAME,STATUS,OSUSER,PROGRAM,MODULE,SQL_EXEC_START,LOGON_TIME FROM V$SESSION WHERE TYPE = 'USER'");
                        break;
                    case "Lock":
                        sqls.push("SELECT a.SESSION_ID,a.ORACLE_USERNAME,a.OS_USER_NAME,a.PROCESS,b.OWNER,b.OBJECT_NAME FROM V$LOCKED_OBJECT a,ALL_OBJECTS b WHERE a.OBJECT_ID = b.OBJECT_ID");
                        break;
                    case "TableSpace":
                        sqls.push("SELECT * FROM DBA_DATA_FILES");
                        break;
                    case "Charset":
                        sqls.push(`SELECT VALUE, UTL_I18N.MAP_CHARSET(VALUE) AS IANA_VALUE
FROM V$NLS_VALID_VALUES WHERE PARAMETER = 'CHARACTERSET' 
ORDER BY IANA_VALUE, VALUE;`);
                        break;
                    case "Collation":
                        sqls.push(`-- NLS_SORT、NLS_COMP 排序比较 BINARY/linguistic
SELECT * FROM v$NLS_PARAMETERS WHERE PARAMETER IN('NLS_SORT', 'NLS_COMP');
-- NLS_SORT=linguistic
SELECT VALUE FROM V$NLS_VALID_VALUES WHERE PARAMETER = 'SORT' ORDER BY VALUE`);
                        break;
                    case "DataType":
                        sqls.push("SELECT TYPE_NAME FROM SYS.DBA_TYPES WHERE OWNER IS NULL ORDER BY TYPE_NAME");
                        break;
                }
                break;
            case "SQLServer":
                switch (name) {
                    case "User":
                        sqls.push("select * from sysusers");
                        break;
                    case "Role":
                        sqls.push("select * from sysusers where issqlrole = 1");
                        break;
                    case "Session":
                        sqls.push('exec sp_who');
                        break;
                    case "CharSet":
                    case "Collation":
                        sqls.push("select * from sys.fn_helpcollations()");
                        break;
                    case "DataType":
                        sqls.push('-- select * from sys.systypes;\r\nselect * from sys.types');
                        break;
                }
                break;
            case "PostgreSQL":
                switch (name) {
                    case "Role":
                        sqls.push("select * from pg_catalog.pg_roles");
                        break;
                    case "Session":
                        sqls.push('select * from pg_stat_activity');
                        break;
                    case "Lock":
                        sqls.push('select pg_blocking_pids(pid) as blocked_by,* from pg_stat_activity where cardinality(pg_blocking_pids(pid)) > 0');
                        break;
                    case "CharSet":
                        sqls.push("select distinct pg_encoding_to_char(conforencoding) from pg_catalog.pg_conversion order BY pg_encoding_to_char(conforencoding)");
                        break;
                    case "Collation":
                        sqls.push("select * from pg_collation");
                        break;
                    case "DataType":
                        sqls.push(`-- select * from pg_type;
SELECT pg_catalog.format_type(t.oid, NULL) AS name, pg_catalog.obj_description(t.oid, 'pg_type') as description
FROM pg_catalog.pg_type t LEFT JOIN pg_catalog.pg_namespace n ON n.oid = t.typnamespace
WHERE (t.typrelid = 0 OR (SELECT c.relkind = 'c' FROM pg_catalog.pg_class c WHERE c.oid = t.typrelid))
  AND NOT EXISTS(SELECT 1 FROM pg_catalog.pg_type el WHERE el.oid = t.typelem AND el.typarray = t.oid)
  AND pg_catalog.pg_type_is_visible(t.oid)
ORDER BY 1, 2`);
                        break;
                }
                break;
        }
        return sqls.join('\r\n');
    },

    /**
     * 数据 完整表名
     * @param {any} cp
     * @param {any} tableRow
     */
    dataFullTableName: (cp, dtSchema) => {
        var tableName = dtSchema.BaseTableName, databaseName, modeName;
        switch (cp.cobj.type) {
            case "SQLite":
                databaseName = dtSchema.BaseCatalogName;
                break;
            case "MySQL":
            case "MariaDB":
            case "Oracle":
                databaseName = dtSchema.BaseSchemaName;
                break;
            case "SQLServer":
            case "PostgreSQL":
                databaseName = dtSchema.BaseCatalogName;
                modeName = dtSchema.BaseSchemaName;
                break;
        }

        var ftn = [ndkGenerateSQL.sqlQuote(cp.cobj.type, databaseName)];
        if (modeName != null) {
            ftn.push(ndkGenerateSQL.sqlQuote(cp.cobj.type, modeName))
        }
        ftn.push(ndkGenerateSQL.sqlQuote(cp.cobj.type, tableName))
        console.debug(ftn);

        return ftn.join('.')
    },


    /**
     * 数据生成 SQL
     * @param {*} event 
     * @param {*} name 菜单名
     */
    dataNewSql: (event, name) => {
        var dom = agg.getContainer(event), tabkey = dom.getAttribute("data-key"),
            tabobj = ndkTab.tabKeys[tabkey], dtSchema;

        if (tabobj.esdata.Item2) {
            for (var i = 0; i < tabobj.grids.length; i++) {
                var grid = tabobj.grids[i];
                if (grid.domGridExecuteSql == dom) {
                    dtSchema = tabobj.esdata.Item2[Object.keys(tabobj.esdata.Item2)[i]];
                    break;
                }
            }
        }
        if (dtSchema) {
            var cp = ndkStep.cpGet(tabkey), rowData = event.api.getSelectedRows();
            if (rowData.length) {
                var result = ndkGenerateSQL[`data${name}Sql`](cp, rowData, dtSchema);
                //复制到剪贴板
                ndkFunction.clipboard(result).then(() => {
                    ndkFunction.output("Done!");
                })
            } else {
                ndkFunction.output(ndkI18n.lg.selectDataRows);
            }
        } else {
            ndkFunction.output(ndkI18n.lg.unsupported);
        }
    },

    /**
     * 数据 SQL
     * @param {*} rowData 
     * @param {*} dtSchema 
     * @returns 
     */
    dataInsertSql: (cp, rowData, dtSchema) => {
        var tableName = ndkGenerateSQL.dataFullTableName(cp, dtSchema[0]), outSqls = [], tableColumnType = {};
        //表列类型
        dtSchema.forEach(colSchema => {
            var colType = colSchema.DataTypeName;
            if (colType == null) {
                colType = colSchema.DataType.split(',')[0].split('.')[1].toLowerCase();
            }
            var column = {
                DataType: colSchema.DataTypeName == null ? colType.replace(/\d+/g, '') : colSchema.DataTypeName,
                DataLength: colSchema.ColumnSize,
                AutoIncrement: colSchema.IsAutoIncrement,
                PrimaryKey: colSchema.IsKey ? 1 : 0
            }
            var sdt = ndkGenerateSQL.sqlDataType(cp.cobj.type, column);
            tableColumnType[colSchema.ColumnName] = { sdt, column };
        });

        //批量插入行数
        var biRows = ndkVary.parameterConfig.dataSqlBulkInsert.value, cols = [], vcols = [];
        if (biRows <= 1 || cp.cobj.type == "Oracle") {
            biRows = 1;
        }

        //带自增列
        var wai = ndkVary.parameterConfig.dataSqlWithAutoIncrement.value;

        //遍历数据
        rowData.forEach(row => {
            cols = [];
            var vcol = [];
            for (var key in row) {
                var field = ndkGenerateSQL.sqlQuote(cp.cobj.type, key), val = row[key];
                const { sdt, column } = tableColumnType[key];

                //跳过自增列
                if (wai == false && column.AutoIncrement) {
                    continue;
                }

                cols.push(field);
                if (val == null) {
                    vcol.push('null');
                } else if (typeof (val) == "number" || ["number", "boolean"].includes(sdt.category)) {
                    vcol.push(val);
                } else {
                    if (cp.cobj.type == "SQLServer" && sdt.mockValue.startsWith(`N'`)) {
                        vcol.push(`N'${val.replaceAll("'", "''")}'`);
                    } else {
                        vcol.push(`'${val.replaceAll("'", "''")}'`);
                    }
                }
            }
            vcols.push(vcol);
            if (vcols.length >= biRows) {
                var isone = vcols.length == 1 ? "" : "\n"
                outSqls.push(`INSERT INTO ${tableName} (${cols.join(', ')}) VALUES ${isone}${vcols.map(v => `(${v.join(', ')})`).join(',\n')}`);
                vcols = [];
            }
        });
        if (vcols.length > 0) {
            var isone = vcols.length == 1 ? "" : "\n"
            outSqls.push(`INSERT INTO ${tableName} (${cols.join(', ')}) VALUES ${isone}${vcols.map(v => `(${v.join(', ')})`).join(',\n')}`);
        }

        return outSqls.join(';\n')
    },

    /**
     * 数据 SQL
     * @param {*} rowData 
     * @param {*} dtSchema 
     * @returns 
     */
    dataUpdateSql: (cp, rowData, dtSchema) => {
        var tableName = ndkGenerateSQL.dataFullTableName(cp, dtSchema[0]), outSqls = [], tableColumnType = {};
        //表列类型
        dtSchema.forEach(colSchema => {
            var colType = colSchema.DataTypeName;
            if (colType == null) {
                colType = colSchema.DataType.split(',')[0].split('.')[1].toLowerCase();
            }
            var column = {
                DataType: colSchema.DataTypeName == null ? colType.replace(/\d+/g, '') : colSchema.DataTypeName,
                DataLength: colSchema.ColumnSize,
                AutoIncrement: colSchema.IsAutoIncrement,
                PrimaryKey: colSchema.IsKey ? 1 : 0
            }
            var sdt = ndkGenerateSQL.sqlDataType(cp.cobj.type, column);
            tableColumnType[colSchema.ColumnName] = { sdt, column };
        });

        //遍历数据
        rowData.forEach(row => {
            var ucol = [], wcol = [];
            for (var key in row) {

                var field = ndkGenerateSQL.sqlQuote(cp.cobj.type, key), val = row[key], setkv;
                const { sdt, column } = tableColumnType[key];
                if (val == null) {
                    setkv = `${field} = null`;
                } else if (typeof (val) == "number" || ["number", "boolean"].includes(sdt.category)) {
                    setkv = `${field} = ${val}`;
                } else {
                    if (cp.cobj.type == "SQLServer" && sdt.mockValue.startsWith(`N'`)) {
                        setkv = `${field} = N'${val.replaceAll("'", "''")}'`;
                    } else {
                        setkv = `${field} = '${val.replaceAll("'", "''")}'`;
                    }
                }
                if (column.PrimaryKey > 0) {
                    wcol.push(setkv);
                } else {
                    ucol.push(setkv);
                }
            }

            if (wcol.length == 0) {
                wcol = ucol;
            }
            outSqls.push(`UPDATE ${tableName} SET ${ucol.join(', ')} WHERE ${wcol.join(' AND ')}`);
        });

        return outSqls.join(';\n')
    },

    /**
     * 数据 SQL
     * @param {*} rowData 
     * @param {*} dtSchema 
     * @returns 
     */
    dataDeleteSql: (cp, rowData, dtSchema) => {
        var tableName = ndkGenerateSQL.dataFullTableName(cp, dtSchema[0]), outSqls = [], tableColumnType = {};
        //表列类型
        dtSchema.forEach(colSchema => {
            var colType = colSchema.DataTypeName;
            if (colType == null) {
                colType = colSchema.DataType.split(',')[0].split('.')[1].toLowerCase();
            }
            var column = {
                DataType: colSchema.DataTypeName == null ? colType.replace(/\d+/g, '') : colSchema.DataTypeName,
                DataLength: colSchema.ColumnSize,
                PrimaryKey: colSchema.IsKey ? 1 : 0
            }
            var sdt = ndkGenerateSQL.sqlDataType(cp.cobj.type, column);
            tableColumnType[colSchema.ColumnName] = { sdt, column };
        });

        //遍历数据
        rowData.forEach(row => {
            var cols = [], wcol = [];
            for (var key in row) {

                var field = ndkGenerateSQL.sqlQuote(cp.cobj.type, key), val = row[key], setkv;
                const { sdt, column } = tableColumnType[key];
                if (val == null) {
                    setkv = `${field} = null`;
                } else if (typeof (val) == "number" || ["number", "boolean"].includes(sdt.category)) {
                    setkv = `${field} = ${val}`;
                } else {
                    if (cp.cobj.type == "SQLServer" && sdt.mockValue.startsWith(`N'`)) {
                        setkv = `${field} = 'N${val.replaceAll("'", "''")}'`;
                    } else {
                        setkv = `${field} = '${val.replaceAll("'", "''")}'`;
                    }
                }
                if (column.PrimaryKey > 0) {
                    wcol.push(setkv);
                } else {
                    cols.push(setkv);
                }
            }

            if (wcol.length == 0) {
                wcol = cols;
            }
            outSqls.push(`DELETE FROM ${tableName} WHERE ${wcol.join(' AND ')}`);
        });

        return outSqls.join(';\n')
    },
}

export { ndkGenerateSQL }