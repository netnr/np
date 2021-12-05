import { ndkFn } from './ndkFn';
import { ndkTab } from './ndkTab';
import { ndkVary } from './ndkVary';

var ndkBuild = {
    sqlDataType: (tdb, column) => {
        var colComment = (column.ColumnComment || "").replaceAll("\n", " "),
            vlen = column.DataLength > 0 ? column.DataLength : 999,
            dval = colComment.replaceAll("'", "''").substring(0, vlen);

        var dtout = {
            category: 'string', // 大类：string, number, boolean, date
            comment: colComment, // 注释
            defaultValue: "''", // 默认值
            mockValue: `'${dval}'` // 模拟值
        };

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
                dtout = Object.assign(dtout, {
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
                dtout = Object.assign(dtout, {
                    category: 'date',
                    mockValue: `'${ndkFn.formatDateTime(column.DataType)}'`
                });
                break;

            case "bool":
            case "boolean":
                dtout = Object.assign(dtout, {
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
        var ftn = [ndkBuild.sqlQuote(cp.cobj.type, cp.databaseName)];
        if (tableRow.TableSchema != null && tableRow.TableSchema != "") {
            ftn.push(ndkBuild.sqlQuote(cp.cobj.type, tableRow.TableSchema))
        }
        ftn.push(ndkBuild.sqlQuote(cp.cobj.type, tableRow.TableName))

        return ftn.join('.')
    },

    /**
     * 生成 SQL 语句
     * @param {*} edata 右键行数据（如果有选中表行则优选）
     * @param {*} name 菜单名
     */
    buildNewTabSql: (edata, name) => {
        //选中的表行
        var srows = ndkVary.gridOpsTable.api.getSelectedRows();
        if (srows.length == 0) {
            srows = [edata];
        }

        //构建选项卡
        ndkTab.tabBuild(ndkFn.random(), ndkVary.icons.connTable + srows[0].TableName, 'sql').then(tpkey => {
            ndkVary.domTabGroup2.show(tpkey); //显示选项卡            
            var tpobj = ndkTab.tabKeys[tpkey];

            tpobj.editor.setValue("-- 正在生成脚本");

            var cp = ndkStep.cpGet(1);
            ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName, srows[0].TableName); //记录连接
            ndkStep.cpInfo(tpkey); //显示连接

            //请求列
            ndkDb.reqColumn(cp.cobj, cp.databaseName, srows.map(x => x.TableName).join(','), true).then(tableColumns => {
                var tpcp = ndkStep.cpGet(tpkey);

                //按表分组依次生成
                var sqls = [];
                ndkFn.groupBy(tableColumns, x => x.TableName).forEach(tableName => {
                    var tableRow = srows.find(x => x.TableName == tableName);
                    var columns = tableColumns.filter(x => x.TableName == tableName);
                    //构建SQL
                    var sql = ndkBuild[`build${name}Sql`](tpcp, tableRow, columns);
                    sqls.push(sql);
                });
                if (sqls.length == 1) {
                    sqls = ndkEditor.formatterSQL(sqls[0], cp.cobj.type); //格式化    
                } else {
                    sqls = sqls.join(';\r\n');
                }

                tpobj.editor.setValue(sqls);
            })
        })
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     * @param {any} columns
     */
    buildSelectSql: (cp, tableRow, columns) => {
        var tableName = ndkBuild.sqlFullTableName(cp, tableRow), cols = [];
        if (columns) {
            columns.forEach(column => cols.push(ndkBuild.sqlQuote(cp.cobj.type, column.ColumnName)));
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
        var tableName = ndkBuild.sqlFullTableName(cp, tableRow), cols = [], vals = [],
            withComment = ndkVary.parameterConfig.buildSqlWithComment.value;
        columns.forEach(column => {
            cols.push(ndkBuild.sqlQuote(cp.cobj.type, column.ColumnName));
            var sdt = ndkBuild.sqlDataType(cp.cobj.type, column);
            //带注释
            if (withComment) {
                var fieldComment = `-- ${column.ColumnName}(${column.PrimaryKey > 0 ? ndkVary.icons.key + column.PrimaryKey + ", " : ""}${column.ColumnType}, ${column.IsNullable == 1 ? "null" : "not null"}) ${sdt.comment}`;
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
        var tableName = ndkBuild.sqlFullTableName(cp, tableRow), cols = [], wcol = [],
            withComment = ndkVary.parameterConfig.buildSqlWithComment.value;
        columns.forEach(column => {
            var field = ndkBuild.sqlQuote(cp.cobj.type, column.ColumnName);
            var sdt = ndkBuild.sqlDataType(cp.cobj.type, column), setkv;
            //带注释
            if (withComment) {
                var fieldComment = `-- ${column.ColumnName}(${column.PrimaryKey > 0 ? ndkVary.icons.key + column.PrimaryKey + ", " : ""}${column.ColumnType}, ${column.IsNullable == 1 ? "null" : "not null"}) ${sdt.comment}`;
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
        var tableName = ndkBuild.sqlFullTableName(cp, tableRow), cols = [], wcol = [];
        columns.forEach(column => {
            var field = ndkBuild.sqlQuote(cp.cobj.type, column.ColumnName);
            var sdt = ndkBuild.sqlDataType(cp.cobj.type, column);
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
        var tableName = ndkBuild.sqlFullTableName(cp, tableRow);

        return `TRUNCATE TABLE ${tableName}`
    },
    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} tableRow
     */
    buildDropSql: (cp, tableRow) => {
        var tableName = ndkBuild.sqlFullTableName(cp, tableRow);

        return `DROP TABLE ${tableName}`
    },
}

export { ndkBuild }