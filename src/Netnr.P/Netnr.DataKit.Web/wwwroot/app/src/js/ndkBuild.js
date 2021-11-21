import { ndkFn } from './ndkFn';
import { ndkVary } from './ndkVary';

var ndkBuild = {
    sqlDataType: (tdb, column) => {
        var dtout = {
            category: 'string', // 大类：string, number, boolean, datetime
            defaultValue: '', // 默认值
            mockValue: '' // 模拟值
        };

        switch (tdb) {
            case "SQLite":
                break;
            case "MySQL":
            case "MariaDB":
                {
                    switch (column.DataType) {
                        case "int":
                        case "bigint":
                        case "tinyint":
                        case "smallint":
                        case "mediumint":
                        case "decimal":
                        case "float":
                        case "double":
                            dtout = {
                                category: 'number',
                                defaultValue: '0',
                                mockValue: '0'
                            }
                            break;

                        case "date":
                        case "datetime":
                        case "timestamp":
                        case "time":
                        case "year":
                            dtout = {
                                category: 'datetime',
                                defaultValue: ndkFn.formatDateTime(column.DataType),
                            }
                            break;

                        case "bool":
                        case "boolean":
                            dtout = {
                                category: 'boolean',
                                defaultValue: 'false',
                                mockValue: 'false'
                            }
                            break;

                        default:
                            dtout = {
                                category: 'string',
                                defaultValue: '',
                                mockValue: ''
                            }
                            break;
                    }
                }
                break;
            case "Oracle":
                switch (column.DataType) {
                    case "decimal":
                    case "float":
                    case "number":
                        dtout = {
                            category: 'number',
                            defaultValue: '0',
                            mockValue: '0'
                        }
                        break;
                }
                break;
            case "SQLServer":
                switch (column.DataType) {
                    case "int":
                    case "bigint":
                    case "float":
                    case "money":
                    case "numeric":
                    case "smallint":
                    case "smallmoney":
                    case "tinyint":
                        dtout = {
                            category: 'number',
                            defaultValue: '0',
                            mockValue: '0'
                        }
                        break;
                }
                break;
            case "PostgreSQL":
                switch (column.DataType) {
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
                        dtout = {
                            category: 'number',
                            defaultValue: '0',
                            mockValue: '0'
                        }
                        break;
                }
                break;
        }

        return dtout;
    },

    /**
     * SQL 引用符号
     * @param {any} type
     * @param {any} key
     */
    sqlQuote: (type, key) => {
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
     * @param {any} table
     */
    sqlFullTableName: (cp, table) => {
        var ftn = [ndkBuild.sqlQuote(cp.cobj.type, cp.databaseName)];
        if (table.TableSchema != null && table.TableSchema != "") {
            ftn.push(ndkBuild.sqlQuote(cp.cobj.type, table.TableSchema))
        }
        ftn.push(ndkBuild.sqlQuote(cp.cobj.type, table.TableName))

        return ftn.join('.')
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} table
     * @param {any} columns
     */
    buildSelectSql: (cp, table, columns) => {
        var table = ndkBuild.sqlFullTableName(cp, table), cols = [];
        if (columns) {
            columns.forEach(column => cols.push(ndkBuild.sqlQuote(cp.cobj.type, column.ColumnName)));
            return `SELECT ${cols.join(',')} FROM ${table}`
        } else {
            var topn = ndkVary.config.selectDataLimit;
            switch (cp.cobj.type) {
                case "SQLServer":
                    return `SELECT TOP ${topn} * FROM ${table}`
                case "MySQL":
                case "MariaDB":
                    return `SELECT * FROM ${table} LIMIT 0,${topn}`
                case "Oracle":
                    return `SELECT * FROM ${table} WHERE ROWNUM <= ${topn}`
                case "SQLite":
                case "PostgreSQL":
                    return `SELECT * FROM ${table} LIMIT ${topn} OFFSET 0`
            }
        }
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} table
     * @param {any} columns
     */
    buildInsertSql: (cp, table, columns) => {
        var table = ndkBuild.sqlFullTableName(cp, table), cols = [], vals = [];
        columns.forEach(column => {
            cols.push(ndkBuild.sqlQuote(cp.cobj.type, column.ColumnName));

            var colComment = (column.ColumnComment || "").replaceAll("\n", " "),
                vlen = column.DataLength > 0 ? column.DataLength : 999,
                dval = `'${colComment.replaceAll("'", "''").substring(0, vlen)}'`;
            if (["bool", "boolean"].includes(column.DataType)) {
                dval = 'false';
            } else if (ndkBuild.sqlDataType(cp.cobj.type, column).category == "number") {
                dval = 0;
            } else if (cp.cobj.type == "SQLite") {
                if (column.PrimaryKey > 0 && ["nvarchar", "varchar", "text"].includes(column.DataType)) {
                    dval = "hex(randomblob(16))";
                } else if (["datetime", "date", "time"].includes(column.DataType)) {
                    dval = `'${ndkFn.formatDateTime(column.DataType)}'`;
                }
            } else if (cp.cobj.type == "MySQL") {
                if (column.PrimaryKey > 0 && ["varchar"].includes(column.DataType)) {
                    dval = "uuid()";
                } else if (["datetime", "date", "time"].includes(column.DataType)) {
                    dval = `'${ndkFn.formatDateTime(column.DataType)}'`;
                }
            } else if (cp.cobj.type == "SQLServer") {
                if (column.PrimaryKey > 0 && ["nvarchar", "varchar"].includes(column.DataType)) {
                    dval = "newid()";
                } else if (["nvarchar", "nchar", "ntext"].includes(column.DataType)) {
                    dval = "N" + dval;
                } else if (["datetime", "date", "time"].includes(column.DataType)) {
                    dval = `'${ndkFn.formatDateTime(column.DataType)}'`;
                }
            }
            vals.push(`-- ${column.ColumnName}(${column.PrimaryKey > 0 ? ndkVary.icons.key + column.PrimaryKey + ", " : ""}${column.ColumnType}, ${column.IsNullable == 1 ? "null" : "not null"}) ${colComment} \n${dval}`);

        });
        return `INSERT INTO ${table}(${cols.join(',')}) VALUES(${vals.join(',\n')})`
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} table
     * @param {any} columns
     * @param {any} rows
     */
    buildUpdateSql: (cp, table, columns, rows) => {
        var table = ndkBuild.sqlFullTableName(cp, table), cols = [], wcol = [];
        columns.forEach(column => {
            var field = ndkBuild.sqlQuote(cp.cobj.type, column.ColumnName);
            var value = "''";

            if (column.PrimaryKey > 0) {
                wcol.push(`${field} = ${value}`);
            } else {
                cols.push(`${field} = ${value}`);
            }
        });
        if (wcol.length == 0) {
            wcol.push(cols[0]);
            cols = cols.splice(0, 1);
        }

        return `UPDATE ${table} SET ${cols.join(',')} WHERE ${wcol.join(' AND ')}`
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} table
     * @param {any} columns
     * @param {any} rows
     */
    buildDeleteSql: (cp, table, columns, rows) => {
        var table = ndkBuild.sqlFullTableName(cp, table), cols = [], wcol = [];
        columns.forEach(column => {
            var field = ndkBuild.sqlQuote(cp.cobj.type, column.ColumnName);
            var value = "''";

            if (cols.length == 0) {
                cols.push(`${field} = ${value}`);
            }
            if (column.PrimaryKey > 0) {
                wcol.push(`${field} = ${value}`);
            }
        });
        if (wcol.length == 0) {
            wcol = cols;
        }

        return `DELETE FROM ${table} WHERE ${wcol.join(' AND ')}`
    },
}

export { ndkBuild }