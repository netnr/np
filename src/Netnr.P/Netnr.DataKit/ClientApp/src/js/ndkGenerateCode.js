import { ndkFunction } from "./ndkFunction";
import { ndkI18n } from "./ndkI18n";
import { ndkStep } from "./ndkStep";
import { ndkVary } from "./ndkVary";

// 生成代码
var ndkGenerateCode = {

    ref: {
        /**
         * 遍历表列
         * @param {*} fnCallback 
         * @returns 
         */
        echoTableColumn: function (fnCallback) {
            // 表设计（列信息）、数据库类型
            var allColumns = [], ctype;
            if (ndkVary.gridOpsColumn) {
                allColumns = ndkVary.gridOpsColumn.api.getSelectedRows();
                ctype = ndkStep.cpGet(1).cobj.type;
            }
            if (allColumns.length == 0) {
                ndkFunction.output(ndkI18n.lg.selectColumn);
                return ndkI18n.lg.selectColumn;
            }

            // 输出调试信息
            console.debug("allColumns", allColumns);
            console.debug("ctype", ctype);

            // 分组：模式名.表名
            let sntns = ndkFunction.groupBy(allColumns, x => x.SchemaName == null ? x.TableName : `${x.SchemaName}.${x.TableName}`);
            sntns.forEach(sntn => {
                let dtColumns = allColumns.filter(x => x.SchemaName == null ? x.TableName == sntn : `${x.SchemaName}.${x.TableName}` == sntn); // 单表
                let oneColumn = dtColumns[0]; // 单表第一个
                let className = sntn.split('.').pop(); // 类名 或带模式名前缀 sntn.split('.').join('_')
                let tableResult = []; // 表结果

                fnCallback(allColumns, ctype, sntns, sntn, dtColumns, oneColumn, className, tableResult);
            });
        },
        csharp: {
            //可为空
            isNullable: ["DateTime", "DateTimeOffset", "Guid", "NpgsqlTypes.NpgsqlLSeg", "NpgsqlTypes.NpgsqlLine", "NpgsqlTypes.NpgsqlPath", "NpgsqlTypes.NpgsqlPoint", "TimeSpan", "bool", "byte", "decimal", "double", "float", "int", "long"],

            // {数据库类型: [实体类类型, 参数类型]}
            SQLite: { "integer": ["int", "Integer"], "real": ["double", "Real"], "text": ["string", "Text"], "blob": ["byte[]", "Blob"] },
            MySQL: { "bigint": ["long", "Int64"], "binary": ["byte[]", "Binary"], "bit": ["ulong", "Bit"], "blob": ["byte[]", "Blob"], "char": ["string", "VarChar"], "date": ["DateTime", "Date"], "datetime": ["DateTime", "DateTime"], "decimal": ["decimal", "Decimal"], "double": ["double", "Double"], "float": ["float", "Float"], "int": ["int", "Int32"], "json": ["string", "JSON"], "longblob": ["byte[]", "LongBlob"], "longtext": ["string", "LongText"], "mediumblob": ["byte[]", "MediumBlob"], "mediumint": ["int", "Int32"], "mediumtext": ["string", "MediumText"], "text": ["string", "Text"], "time": ["DateTime", "Time"], "timestamp": ["DateTime", "Timestamp"], "tinyblob": ["byte[]", "TinyBlob"], "tinyint": ["short", "Int16"], "tinytext": ["string", "TinyText"], "varbinary": ["byte[]", "VarBinary"], "varchar": ["string", "VarChar"], "year": ["string", "Year"] },
            Oracle: { "BFILE": ["byte[]", "BFile"], "VARCHAR2": ["string", "Varchar2"], "BINARY_DOUBLE": ["double", "BinaryDouble"], "BINARY_FLOAT": ["float", "BinaryFloat"], "BLOB": ["byte[]", "Blob"], "CLOB": ["string", "Clob"], "CHAR": ["string", "Char"], "DATE": ["DateTime", "Date"], "LONG": ["Long", "Long"], "LONG RAW": ["byte[]", "LongRaw"], "NCLOB": ["string", "NClob"], "NUMBER": ["decimal", "Decimal"], "NVARCHAR2": ["string", "NVarchar2"], "RAW": ["byte[]", "Raw"], "TIMESTAMP(6)": ["DateTime", "TimeStamp"], "INTERVAL DAY(2) TO SECOND(6)": ["TimeSpan", "IntervalDS"], "INTERVAL YEAR(2) TO MONTH": ["string", "IntervalYM"], "TIMESTAMP(6) WITH LOCAL TIME ZONE": ["DateTimeOffset", "TimeStampLTZ"], "TIMESTAMP(6) WITH TIME ZONE": ["DateTimeOffset", "TimeStampTZ"] },
            SQLServer: { "bigint": ["long", "BigInt"], "binary": ["byte[]", "Binary"], "bit": ["bool", "Bit"], "char": ["string", "Char"], "date": ["DateTime", "Date"], "datetime": ["DateTime", "DateTime"], "datetime2": ["DateTime", "DateTime2"], "datetimeoffset": ["DateTimeOffset", "DateTimeOffset"], "decimal": ["decimal", "Decimal"], "float": ["double", "Float"], "image": ["byte[]", "Image"], "int": ["int", "Int"], "money": ["decimal", "Money"], "nchar": ["string", "NChar"], "ntext": ["string", "NText"], "numeric": ["decimal", "Decimal"], "nvarchar": ["string", "NVarChar"], "real": ["float", "Real"], "smalldatetime": ["DateTime", "SmallDateTime"], "smallint": ["short", "SmallInt"], "smallmoney": ["decimal", "SmallMoney"], "sql_variant": ["object", "NVarChar"], "text": ["string", "Text"], "time": ["TimeSpan", "Time"], "timestamp": ["byte[]", "Timestamp"], "tinyint": ["byte", "TinyInt"], "uniqueidentifier": ["Guid", "UniqueIdentifier"], "varbinary": ["byte[]", "VarBinary"], "varchar": ["string", "VarChar"], "xml": ["string", "Xml"] },
            PostgreSQL: { "bit": ["System.Collections.BitArray", "Bit"], "bool": ["bool", "Boolean"], "box": ["NpgsqlTypes.NpgsqlBox", "Box"], "bytea": ["byte[]", "Bytea"], "bpchar": ["string", "Varchar"], "cidr": ["ValueTuple<System.Net.IPAddress, int>", "Cidr"], "circle": ["NpgsqlTypes.NpgsqlCircle", "Circle"], "date": ["DateTime", "Date"], "numeric": ["decimal", "Numeric"], "float4": ["float", "Double"], "float8": ["double", "Double"], "inet": ["System.Net.IPAddress", "Inet"], "int2": ["short", "Integer"], "int4": ["int", "Integer"], "int8": ["long", "Bigint"], "interval": ["TimeSpan", "Interval"], "json": ["string", "Json"], "jsonb": ["string", "Jsonb"], "line": ["NpgsqlTypes.NpgsqlLine", "Line"], "lseg": ["NpgsqlTypes.NpgsqlLSeg", "LSeg"], "macaddr": ["System.Net.NetworkInformation.PhysicalAddress", "MacAddr"], "money": ["decimal", "Money"], "path": ["NpgsqlTypes.NpgsqlPath", "Path"], "point": ["NpgsqlTypes.NpgsqlPoint", "Point"], "text": ["string", "Text"], "time": ["TimeSpan", "Time"], "timestamp": ["DateTime", "Timestamp"], "timestamptz": ["DateTime", "TimestampTz"], "timetz": ["TimeSpan", "TimeTz"], "tsquery": ["NpgsqlTypes.NpgsqlTsQuery", "TsQuery"], "tsvector": ["NpgsqlTypes.NpgsqlTsVector", "TsVector"], "txid_snapshot": ["string", "Varchar"], "uuid": ["Guid", "Uuid"], "varbit": ["System.Collections.BitArray", "Varbit"], "varchar": ["string", "Varchar"], "xml": ["string", "Xml"] },
        }
    },

    fns: [
        {
            language: "csharp",
            name: "Model",
            /**
             * 生成实体
             */
            fn: () => {
                let fnout = []; //输出

                //配置项
                let ops = {
                    project: "Netnr.Blog.Domain",
                    namespace: "Netnr.Blog.Domain",
                    extensionName: ".cs",
                };
                let ref = window["ndkGenerateCode"].ref;

                // 遍历表列
                ref.echoTableColumn((allColumns, ctype, sntns, sntn, dtColumns, oneColumn, className, tableResult) => {
                    //引用
                    tableResult.push(`using System;`)
                    tableResult.push(``);

                    //命名空间
                    tableResult.push(`namespace ${ops.namespace}`);
                    tableResult.push(`{`);

                    //类注释
                    tableResult.push(`\t/// <summary>`);
                    (oneColumn.TableComment || "").split("\n").forEach(x => tableResult.push(`\t/// ${x.trim()}`));
                    tableResult.push(`\t/// </summary>`);

                    //类名
                    tableResult.push(`\tpublic partial class ${className}`);
                    tableResult.push(`\t{`);

                    //构建项
                    dtColumns.forEach(col => {
                        //列注释
                        tableResult.push(`\t\t/// <summary>`);
                        (col.ColumnComment || "").split("\n").forEach(x => tableResult.push(`\t\t/// ${x.trim()}`));
                        tableResult.push(`\t\t/// </summary>`);

                        col.DataType = ctype == "Oracle" ? col.DataType.toUpperCase() : col.DataType.toLowerCase();

                        //属性                        
                        let lgtype = 'object'; // 默认类型
                        let dbo = ref.csharp[ctype][col.DataType];
                        if (dbo) {
                            lgtype = dbo[0];
                        }
                        //类型 二次处理
                        if (ctype == "Oracle" && lgtype == "decimal" && col.DataScale == 0) {
                            lgtype = "int";
                        }

                        //数据库可为空、类型可为空
                        if (col.IsNullable == 1 && ref.csharp.isNullable.includes(lgtype)) {
                            lgtype += "?";
                        }
                        tableResult.push(`\t\tpublic ${lgtype} ${col.ColumnName} { get; set; }`);
                    });

                    //类、命名空间结束
                    tableResult.push(`\t}`);
                    tableResult.push(`}`);

                    //文件内容
                    fnout.push({
                        name: `${className}.cs`,
                        content: tableResult.join("\r\n"),
                        path: ops.project
                    })
                });

                console.debug("fnout", fnout);
                return fnout;
            }
        },

        {
            language: "csharp",
            name: "DAL",
            /**
             * 生成DAL
             */
            fn: () => {
                let fnout = []; //输出

                //配置项
                let ops = {
                    project: "Netnr.Blog.Application",
                    namespace: "Netnr.Blog.Application",
                    extensionName: ".cs",
                };
                let ref = window["ndkGenerateCode"].ref;

                // 遍历表列
                ref.echoTableColumn((allColumns, ctype, sntns, sntn, dtColumns, oneColumn, className, tableResult) => {
                    //引用
                    tableResult.push('using System;')
                    tableResult.push('using System.Data;')
                    switch (ctype) {
                        case "MySQL":
                        case "MariaDB":
                            tableResult.push('using MySql.Data.MySqlClient;')
                            break;
                        case "SQLite":
                            tableResult.push('using Microsoft.Data.Sqlite;')
                            break;
                        case "Oracle":
                            tableResult.push('using Oracle.ManagedDataAccess.Client;')
                            break;
                        case "SQLServer":
                            tableResult.push('using System.Data.SqlClient;')
                            break;
                        case "PostgreSQL":
                            tableResult.push('using Npgsql;')
                            tableResult.push('using NpgsqlTypes;')
                            break;
                    }
                    tableResult.push('');

                    //命名空间
                    tableResult.push(`namespace ${ops.namespace}`);
                    tableResult.push('{');

                    //类注释
                    tableResult.push(`\t/// <summary>`);
                    (oneColumn.TableComment || "").split("\n").forEach(x => tableResult.push(`\t/// ${x.trim()}`));
                    tableResult.push(`\t/// </summary>`);

                    //类名
                    tableResult.push(`\tpublic partial class ${className}`);
                    tableResult.push(`\t{`);

                    //构造方法
                    tableResult.push(`\t\tpublic ${className}() { }`);
                    tableResult.push('');

                    //构建方法-公共对象
                    let bo = {
                        namespaceDomain: "Netnr.Blog.Domain", // 实体命名空间
                        pprefix: ctype == "Oracle" ? ":" : "@", // 参数前缀
                        fields: dtColumns.map(x => x.ColumnName), // 字段
                        dbQuery: "DbHelper.SqlExecuteReader", //查询
                        dbExecute: "DbHelper.SqlExecuteNonQuery", // 执行
                        dbParameterClass: "", // 参数类
                        dbParameterType: "", // 参数类型
                    };
                    switch (ctype) {
                        case "MySQL":
                        case "MariaDB":
                            bo.dbParameterType = "MySqlDbType";
                            bo.dbParameterClass = "MySqlParameter";
                            break;
                        case "SQLite":
                            bo.dbParameterType = "SqliteType";
                            bo.dbParameterClass = "SqliteParameter";
                            break;
                        case "Oracle":
                            bo.dbParameterType = "OracleDbType";
                            bo.dbParameterClass = "OracleParameter";
                            break;
                        case "SQLServer":
                            bo.dbParameterType = "SqlDbType";
                            bo.dbParameterClass = "SqlParameter";
                            break;
                        case "PostgreSQL":
                            bo.dbParameterType = "NpgsqlDbType";
                            bo.dbParameterClass = "NpgsqlParameter";
                            break;
                    }

                    //构建方法-查询
                    tableResult.push(`\t\t/// <summary>`);
                    tableResult.push(`\t\t/// 查询`);
                    tableResult.push(`\t\t/// </summary>`);
                    tableResult.push(`\t\t/// <param name="whereSql">SQL条件，可选</param>`);
                    tableResult.push(`\t\t/// <returns>返回数据集</returns>`);
                    tableResult.push(`\t\tpublic DataSet Query(string whereSql = null)`);
                    tableResult.push(`\t\t{`);
                    tableResult.push(`\t\t\tvar sql = "select * from ${sntn}";`);
                    tableResult.push(`\t\t\tif (!string.IsNullOrWhiteSpace(whereSql))`);
                    tableResult.push(`\t\t\t{`);
                    tableResult.push(`\t\t\t\tsql += " where " + whereSql;`);
                    tableResult.push(`\t\t\t}`);
                    tableResult.push('');
                    //执行脚本
                    tableResult.push(`\t\t\tvar ds = ${bo.dbQuery}(sql);`);
                    tableResult.push(`\t\t\treturn ds;`);
                    tableResult.push(`\t\t}`);
                    tableResult.push('');

                    //构建方法-删除
                    tableResult.push(`\t\t/// <summary>`);
                    tableResult.push(`\t\t/// 删除`);
                    tableResult.push(`\t\t/// </summary>`);
                    tableResult.push(`\t\t/// <param name="whereSql">SQL条件</param>`);
                    tableResult.push(`\t\t/// <returns>返回受影响行数</returns>`);
                    tableResult.push(`\t\tpublic int Delete(string whereSql)`);
                    tableResult.push(`\t\t{`);
                    tableResult.push(`\t\t\tvar sql = "delete from ${sntn} where " + whereSql;`);
                    tableResult.push(`\t\t\tvar num = ${bo.dbExecute}(sql);`);
                    tableResult.push(`\t\t\treturn num;`);
                    tableResult.push(`\t\t}`);
                    tableResult.push('');

                    //构建方法-新增
                    tableResult.push(`\t\t/// <summary>`);
                    tableResult.push(`\t\t/// 新增`);
                    tableResult.push(`\t\t/// </summary>`);
                    tableResult.push(`\t\t/// <param name="model">新增实体对象</param>`);
                    tableResult.push(`\t\t/// <returns>返回受影响行数</returns>`);
                    tableResult.push(`\t\tpublic int Add(${bo.namespaceDomain}.${className} model)`);
                    tableResult.push(`\t\t{`);
                    tableResult.push(`\t\t\tvar sql = @"insert into ${sntn} `);
                    tableResult.push(`\t\t\t\t\t\t(${bo.fields.join(", ")}) values `);
                    tableResult.push(`\t\t\t\t\t\t(${bo.pprefix + bo.fields.join(', ' + bo.pprefix)})";`);
                    tableResult.push('');
                    //参数
                    tableResult.push(`\t\t\t${bo.dbParameterClass}[] parameters = {`);
                    dtColumns.forEach(col => {
                        let dbptype = ""; // 参数类型
                        col.DataType = ctype == "Oracle" ? col.DataType.toUpperCase() : col.DataType.toLowerCase();
                        let dbo = ref.csharp[ctype][col.DataType];
                        if (dbo) {
                            dbptype = dbo[1];
                        }

                        //类型 二次处理
                        if (ctype == "Oracle" && dbptype == "Decimal" && col.DataScale == 0) {
                            dbptype = "Int32";
                        }

                        //长度
                        let dbplength;
                        //忽略长度
                        if (isNaN(parseInt(col.DataLength))) {
                            dbplength = "";
                        } else if (ctype == "Oracle" && dbptype.slice(-3) == "lob") {
                            dbplength = "";
                        } else {
                            dbplength = `, ${col.DataLength}`;
                        }

                        // 参数
                        let newparam = `new ${bo.dbParameterClass}("${bo.pprefix + col.ColumnName}", ${bo.dbParameterType}.${dbptype}${dbplength})`;
                        tableResult.push(`\t\t\t\t${newparam},`);
                    })
                    tableResult.push(`\t\t\t};`);
                    tableResult.push('');
                    //填充参数值
                    for (let index = 0; index < dtColumns.length; index++) {
                        let col = dtColumns[index];
                        tableResult.push(`\t\t\tparameters[${index}].Value = model.${col.ColumnName};`);
                    }
                    tableResult.push('');
                    //执行脚本
                    tableResult.push(`\t\t\tint num = ${bo.insert}(sql, parameters);`);
                    tableResult.push(`\t\t\treturn num;`);
                    tableResult.push(`\t\t}`);
                    tableResult.push('');

                    //构建方法-修改
                    tableResult.push(`\t\t/// <summary>`);
                    tableResult.push(`\t\t/// 修改`);
                    tableResult.push(`\t\t/// </summary>`);
                    tableResult.push(`\t\t/// <param name="model">修改实体对象</param>`);
                    tableResult.push(`\t\t/// <returns>返回受影响行数</returns>`);
                    tableResult.push(`\t\tpublic int Update(${bo.namespaceDomain}.${className} model)`);
                    tableResult.push(`\t\t{`);
                    tableResult.push(`\t\t\tvar sql = @"update ${sntn} set `);
                    let setkv1 = [];
                    let setkv2 = [];
                    dtColumns.forEach(col => {
                        let setkv = `${bo.pprefix + col.ColumnName}=${bo.pprefix + col.ColumnName}`;
                        if (col.PrimaryKey > 0) {
                            setkv2.push(setkv);
                        } else {
                            setkv1.push(setkv);
                        }
                    })
                    tableResult.push(`\t\t\t\t\t\t${setkv1.join(", ")} `);
                    tableResult.push(`\t\t\t\t\t\twhere ${setkv2.join(" and ")}";`);
                    tableResult.push('');
                    //参数
                    tableResult.push(`\t\t\t${bo.dbParameterClass}[] parameters = {`);
                    dtColumns.forEach(col => {
                        let dbptype = ""; // 参数类型
                        col.DataType = ctype == "Oracle" ? col.DataType.toUpperCase() : col.DataType.toLowerCase();
                        let dbo = ref.csharp[ctype][col.DataType];
                        if (dbo) {
                            dbptype = dbo[1];
                        }

                        //类型 二次处理
                        if (ctype == "Oracle" && dbptype == "Decimal" && col.DataScale == 0) {
                            dbptype = "Int32";
                        }

                        //长度
                        let dbplength;
                        //忽略长度
                        if (isNaN(parseInt(col.DataLength))) {
                            dbplength = "";
                        } else if (ctype == "Oracle" && dbptype.slice(-3) == "lob") {
                            dbplength = "";
                        } else {
                            dbplength = `, ${col.DataLength}`;
                        }

                        // 参数
                        let newparam = `new ${bo.dbParameterClass}("${bo.pprefix + col.ColumnName}", ${bo.dbParameterType}.${dbptype}${dbplength})`;
                        tableResult.push(`\t\t\t\t${newparam},`);
                    })
                    tableResult.push(`\t\t\t};`);
                    tableResult.push('');
                    //填充参数值
                    for (let index = 0; index < dtColumns.length; index++) {
                        let col = dtColumns[index];
                        tableResult.push(`\t\t\tparameters[${index}].Value = model.${col.ColumnName};`);
                    }
                    tableResult.push('');
                    //执行脚本
                    tableResult.push(`\t\t\tint num = ${bo.dbExecute}(sql, parameters);`);
                    tableResult.push(`\t\t\treturn num;`);
                    tableResult.push(`\t\t}`);
                    tableResult.push('');

                    //构建方法-分页查询
                    tableResult.push(`\t\t/// <summary>`);
                    tableResult.push(`\t\t/// 分页查询`);
                    tableResult.push(`\t\t/// </summary>`);
                    tableResult.push(`\t\t/// <param name="pageNumber">页码</param>`);
                    tableResult.push(`\t\t/// <param name="pageSize">页量</param>`);
                    tableResult.push(`\t\t/// <param name="whereSql">SQL条件</param>`);
                    tableResult.push(`\t\t/// <param name="sortOrder">排序</param>`);
                    tableResult.push(`\t\t/// <param name="total">返回总条数</param>`);
                    tableResult.push(`\t\t/// <returns>返回数据表及总条数</returns>`);
                    tableResult.push(`\t\tpublic DataTable Query(int pageNumber, int pageSize, string whereSql, string sortOrder, out int total)`);
                    tableResult.push(`\t\t{`);
                    tableResult.push(`\t\t\tsortOrder = string.IsNullOrWhiteSpace(sortOrder) ? "" : "ORDER BY " + sortOrder;`);
                    if (ctype != "Oracle") {
                        tableResult.push(`\t\t\twhereSql = string.IsNullOrWhiteSpace(whereSql) ? "" : "WHERE " + whereSql;`);
                        tableResult.push('');
                    }
                    switch (ctype) {
                        case "MySQL":
                        case "MariaDB":
                            tableResult.push(`\t\t\tvar sql = $"SELECT * FROM ${sntn} {whereSql} {sortOrder} LIMIT {(pageNumber - 1) * pageSize}, {pageSize}";`);
                            tableResult.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${sntn} {whereSql}";`);
                            break;
                        case "SQLite":
                            tableResult.push(`\t\t\tvar sql = $"SELECT * FROM ${sntn} {whereSql} {sortOrder} LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";`);
                            tableResult.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${sntn} {whereSql}";`);
                            break;
                        case "Oracle":
                            tableResult.push(`\t\t\tvar ws1 = string.IsNullOrWhiteSpace(whereSql) ? "" : whereSql + " AND ";`);
                            tableResult.push(`\t\t\tvar ws2 = string.IsNullOrWhiteSpace(whereSql) ? "" : "WHERE " + whereSql;`);
                            tableResult.push('');
                            tableResult.push(`\t\t\tvar sql = $"SELECT * FROM ( SELECT ROWNUM AS rowno, a.* FROM ${sntn} a WHERE {ws1} ROWNUM <= {pageNumber * pageSize} {sortOrder} ) x WHERE x.rowno >= {(pageNumber - 1) * pageSize}";`);
                            tableResult.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${sntn} {ws2}";`);
                            break;
                        case "SQLServer":
                            //ROW_NUMBER分页
                            tableResult.push(`\t\t\tvar sql = $"SELECT TOP {rows} * FROM ( SELECT ROW_NUMBER() OVER( {sortOrder} ) AS numid, * FROM ${sntn} {whereSql} ) x WHERE numid > {(pageNumber - 1) * pageSize}";`);
                            //OFFSET分页，版本2012及以上
                            //tableResult.push(`\t\t\tvar sql = $"SELECT * FROM ${tableName} {whereSql} {sortOrder} OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";`);
                            tableResult.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${sntn} {whereSql}";`);
                            break;
                        case "PostgreSQL":
                            tableResult.push(`\t\t\tvar sql = $"SELECT * FROM ${sntn} {whereSql} {sortOrder} LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";`);
                            tableResult.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${sntn} {whereSql}";`);
                            break;
                    }
                    tableResult.push('');
                    tableResult.push(`\t\t\tvar ds = ${bo.dbQuery}(sql);`);
                    tableResult.push(`\t\t\ttotal = Convert.ToInt32(ds.Table[1].Rows[0][0].ToString());`);
                    tableResult.push(`\t\t\treturn ds.Table[0];`);
                    tableResult.push(`\t\t}`);
                    tableResult.push('');

                    //类、命名空间结束
                    tableResult.push(`\t}`);
                    tableResult.push(`}`);

                    //文件内容
                    fnout.push({
                        name: `${className}.cs`,
                        content: tableResult.join("\r\n"),
                        path: ops.project
                    })
                });

                console.debug("fnout", fnout);
                return fnout;
            }
        },

        {
            language: "java",
            name: "Model",
            /**
             * 生成 Model
             */
            fn: () => {
                let fnout = []; //输出

                //配置项
                let ops = {
                    project: "Netnr.Blog.Domain",
                    namespace: "Netnr.Blog.Domain",
                    extensionName: ".java",
                };
                let ref = window["ndkGenerateCode"].ref;

                // 遍历表列
                ref.echoTableColumn((allColumns, ctype, sntns, sntn, dtColumns, oneColumn, className, tableResult) => {
                    //引用
                    tableResult.push(`import javax.persistence.*;`)

                    //Waiting for you to finish the code

                    //文件内容
                    fnout.push({
                        name: `${className}.cs`,
                        content: tableResult.join("\r\n"),
                        path: ops.project
                    })
                });

                console.debug("fnout", fnout);
                return fnout;
            }
        }
    ]
}

ndkGenerateCode.ref.csharp.MariaDB = ndkGenerateCode.ref.csharp.MySQL;
Object.assign(ndkGenerateCode.ref.csharp.SQLite, ndkGenerateCode.ref.csharp.SQLServer);

ndkGenerateCode.fns.forEach(fo => {
    let reg1 = /(_ndkFunction__WEBPACK_IMPORTED_MODULE_\d+__).ndkFunction./gm;
    let reg2 = /(_ndkI18n__WEBPACK_IMPORTED_MODULE_\d+__).ndkI18n./gm;
    let reg3 = /(_ndkStep__WEBPACK_IMPORTED_MODULE_\d+__).ndkStep./gm;
    let reg4 = /(_ndkVary__WEBPACK_IMPORTED_MODULE_\d+__).ndkVary./gm;

    fo.code = fo.fn.toString()
        .replace(reg1, 'ndkFunction.')
        .replace(reg2, 'ndkI18n.')
        .replace(reg3, 'ndkStep.')
        .replace(reg4, 'ndkVary.');
})

export { ndkGenerateCode }