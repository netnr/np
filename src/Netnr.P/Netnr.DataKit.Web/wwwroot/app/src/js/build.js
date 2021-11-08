import { fn } from './fn';

var build = {

    buildSqlNumberType: {
        "SQLite": "",
        "MySQL": "bigint,int,tinyint,float,integer,year,double,real",
        "Oracle": "decimal,float,integer,number",
        "SQLServer": "bigint,int,float,money,numeric,smallint,smallmoney,tinyint",
        "PostgreSQL": "int2,int4,int8,integer,smallint,serial,float4,float8,real,numeric,money"
    },

    /**
     * 构建SQL引用符号
     * @param {any} type
     * @param {any} key
     */
    buildSqlQuote: (type, key) => {
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
     * 构建SQL完整表名
     * @param {any} cp
     * @param {any} table
     */
    buildSqlFullTableName: (cp, table) => {
        var ftn = [build.buildSqlQuote(cp.cobj.type, cp.databaseName)];
        if (table.TableSchema != null && table.TableSchema != "") {
            ftn.push(build.buildSqlQuote(cp.cobj.type, table.TableSchema))
        }
        ftn.push(build.buildSqlQuote(cp.cobj.type, table.TableName))

        return ftn.join('.')
    },

    /**
     * 构建SQL
     * @param {any} cp
     * @param {any} table
     * @param {any} columns
     */
    buildSqlSelect: (cp, table, columns) => {
        var table = build.buildSqlFullTableName(cp, table), cols = [];
        if (columns) {
            columns.forEach(column => cols.push(build.buildSqlQuote(cp.cobj.type, column.ColumnName)));
            return `SELECT ${cols.join(',')} FROM ${table}`
        } else {
            var topn = 200;
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
    buildSqlInsert: (cp, table, columns) => {
        var table = build.buildSqlFullTableName(cp, table), cols = [], vals = [];
        columns.forEach(column => {
            cols.push(build.buildSqlQuote(cp.cobj.type, column.ColumnName));

            var colComment = (column.ColumnComment || "").replaceAll("\n", " "),
                vlen = column.DataLength > 0 ? column.DataLength : 999,
                dval = `'${colComment.replaceAll("'", "''").substring(0, vlen)}'`;
            if (["bool", "boolean"].includes(column.DataType)) {
                dval = 'false';
            } else if (build.buildSqlNumberType[cp.cobj.type].indexOf(column.DataType) >= 0) {
                dval = 0;
            } else if (cp.cobj.type == "SQLServer") {
                if (column.PrimaryKey > 0 && ["nvarchar", "varchar"].includes(column.DataType)) {
                    dval = "newid()";
                } else if (["nvarchar", "nchar", "ntext"].includes(column.DataType)) {
                    dval = "N" + dval;
                } else if (["datetime", "date", "time"].includes(column.DataType)) {
                    dval = `'${fn.formatDateTime(column.DataType)}'`;
                }
            } else if (cp.cobj.type == "SQLite") {
                if (column.PrimaryKey > 0 && ["nvarchar", "varchar", "text"].includes(column.DataType)) {
                    dval = "hex(randomblob(16))";
                } else if (["datetime", "date", "time"].includes(column.DataType)) {
                    dval = `'${fn.formatDateTime(column.DataType)}'`;
                }
            }
            vals.push(`-- ${column.ColumnName}(${column.PrimaryKey > 0 ? vary.icons.key + column.PrimaryKey + ", " : ""}${column.ColumnType}, ${column.IsNullable == 1 ? "null" : "not null"}) ${colComment} \n${dval}`);

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
    buildSqlUpdate: (cp, table, columns, rows) => {
        var table = build.buildSqlFullTableName(cp, table), cols = [], wcol = [];
        columns.forEach(column => {
            var field = build.buildSqlQuote(cp.cobj.type, column.ColumnName);
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
    buildSqlDelete: (cp, table, columns, rows) => {
        var table = build.buildSqlFullTableName(cp, table), cols = [], wcol = [];
        columns.forEach(column => {
            var field = build.buildSqlQuote(cp.cobj.type, column.ColumnName);
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

export { build }