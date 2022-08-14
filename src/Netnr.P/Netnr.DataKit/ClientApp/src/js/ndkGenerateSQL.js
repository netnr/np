import { ndkGenerateDDL } from './ndkGenerateDDL';
import { ndkFunction } from './ndkFunction';
import { ndkI18n } from './ndkI18n';
import { ndkStep } from './ndkStep';
import { ndkTab } from './ndkTab';
import { ndkVary } from './ndkVary';
import { ndkRequest } from './ndkRequest';

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
     * @param {*} edata 右键行数据（如果有选中表行则优选）
     * @param {*} name 菜单名
     */
    buildNewTabSql: (edata, name) => {
        //选中的表行
        var srows = ndkVary.gridOpsTable.api.getSelectedRows();
        if (srows.length == 0 && edata != null) {
            srows = [edata];
        }
        if (srows.length == 0) {
            return;
        }

        //构建选项卡
        ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("table", srows[0].TableName), 'sql').then(tpkey => {
            ndkVary.domTabGroup2.show(tpkey); //显示选项卡            
            var tpobj = ndkTab.tabKeys[tpkey];

            tpobj.editor.setValue(`-- ${ndkI18n.lg.generatingScript}`);

            var cp = ndkStep.cpGet(1);
            ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName, srows[0].TableName); //记录连接
            ndkStep.cpInfo(tpkey); //显示连接

            if (name == "DDL") {
                ndkGenerateDDL.reqDDL(cp.cobj, cp.databaseName, srows).then(ddl => {
                    tpobj.editor.setValue(ddl);
                }).catch(err => {
                    tpobj.editor.setValue(`-- ${err}`);
                });
            } else {
                //请求列                
                ndkRequest.reqColumn(cp.cobj, cp.databaseName, srows.map(x => x.TableName).join(','), true).then(tableColumns => {
                    var tpcp = ndkStep.cpGet(tpkey);

                    //按表分组依次生成
                    var sqls = [];
                    ndkFunction.groupBy(tableColumns, x => x.TableName).forEach(tableName => {
                        var tableRow = srows.find(x => x.TableName == tableName);
                        var columns = tableColumns.filter(x => x.TableName == tableName);
                        //构建SQL
                        var sql = ndkGenerateSQL[`build${name}Sql`](tpcp, tableRow, columns);
                        sqls.push(sql);
                    });
                    if (sqls.length == 1) {
                        sqls = ndkEditor.formatterSQL(sqls[0], cp.cobj.type); //格式化    
                    } else {
                        sqls = sqls.join(';\r\n');
                    }

                    tpobj.editor.setValue(sqls);
                })
            }
        })
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