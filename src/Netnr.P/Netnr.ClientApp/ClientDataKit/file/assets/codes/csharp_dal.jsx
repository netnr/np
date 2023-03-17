// @namespace   netnr
// @name        csharp_dal
// @date        2022-10-16
// @version     1.0.0
// @description C# 生成 Dal

async () => {
    // 输出结果
    let result = { language: "csharp", files: [] };

    let typeMap = {
        // 可为空
        isNullable: ["DateTime", "DateTimeOffset", "Guid", "NpgsqlTypes.NpgsqlLSeg", "NpgsqlTypes.NpgsqlLine", "NpgsqlTypes.NpgsqlPath", "NpgsqlTypes.NpgsqlPoint", "TimeSpan", "bool", "byte", "decimal", "double", "float", "int", "long"],

        // {数据库类型: [实体类类型, 参数类型]}
        SQLite: { "integer": ["int", "Integer"], "real": ["double", "Real"], "text": ["string", "Text"], "blob": ["byte[]", "Blob"] },
        MySQL: { "bigint": ["long", "Int64"], "binary": ["byte[]", "Binary"], "bit": ["ulong", "Bit"], "blob": ["byte[]", "Blob"], "char": ["string", "VarChar"], "date": ["DateTime", "Date"], "datetime": ["DateTime", "DateTime"], "decimal": ["decimal", "Decimal"], "double": ["double", "Double"], "float": ["float", "Float"], "int": ["int", "Int32"], "json": ["string", "JSON"], "longblob": ["byte[]", "LongBlob"], "longtext": ["string", "LongText"], "mediumblob": ["byte[]", "MediumBlob"], "mediumint": ["int", "Int32"], "mediumtext": ["string", "MediumText"], "text": ["string", "Text"], "time": ["DateTime", "Time"], "timestamp": ["DateTime", "Timestamp"], "tinyblob": ["byte[]", "TinyBlob"], "tinyint": ["short", "Int16"], "tinytext": ["string", "TinyText"], "varbinary": ["byte[]", "VarBinary"], "varchar": ["string", "VarChar"], "year": ["string", "Year"] },
        Oracle: { "BFILE": ["byte[]", "BFile"], "VARCHAR2": ["string", "Varchar2"], "BINARY_DOUBLE": ["double", "BinaryDouble"], "BINARY_FLOAT": ["float", "BinaryFloat"], "BLOB": ["byte[]", "Blob"], "CLOB": ["string", "Clob"], "CHAR": ["string", "Char"], "DATE": ["DateTime", "Date"], "LONG": ["Long", "Long"], "LONG RAW": ["byte[]", "LongRaw"], "NCLOB": ["string", "NClob"], "NUMBER": ["decimal", "Decimal"], "NVARCHAR2": ["string", "NVarchar2"], "RAW": ["byte[]", "Raw"], "TIMESTAMP(6)": ["DateTime", "TimeStamp"], "INTERVAL DAY(2) TO SECOND(6)": ["TimeSpan", "IntervalDS"], "INTERVAL YEAR(2) TO MONTH": ["string", "IntervalYM"], "TIMESTAMP(6) WITH LOCAL TIME ZONE": ["DateTimeOffset", "TimeStampLTZ"], "TIMESTAMP(6) WITH TIME ZONE": ["DateTimeOffset", "TimeStampTZ"] },
        SQLServer: { "bigint": ["long", "BigInt"], "binary": ["byte[]", "Binary"], "bit": ["bool", "Bit"], "char": ["string", "Char"], "date": ["DateTime", "Date"], "datetime": ["DateTime", "DateTime"], "datetime2": ["DateTime", "DateTime2"], "datetimeoffset": ["DateTimeOffset", "DateTimeOffset"], "decimal": ["decimal", "Decimal"], "float": ["double", "Float"], "image": ["byte[]", "Image"], "int": ["int", "Int"], "money": ["decimal", "Money"], "nchar": ["string", "NChar"], "ntext": ["string", "NText"], "numeric": ["decimal", "Decimal"], "nvarchar": ["string", "NVarChar"], "real": ["float", "Real"], "smalldatetime": ["DateTime", "SmallDateTime"], "smallint": ["short", "SmallInt"], "smallmoney": ["decimal", "SmallMoney"], "sql_variant": ["object", "NVarChar"], "text": ["string", "Text"], "time": ["TimeSpan", "Time"], "timestamp": ["byte[]", "Timestamp"], "tinyint": ["byte", "TinyInt"], "uniqueidentifier": ["Guid", "UniqueIdentifier"], "varbinary": ["byte[]", "VarBinary"], "varchar": ["string", "VarChar"], "xml": ["string", "Xml"] },
        PostgreSQL: { "bit": ["System.Collections.BitArray", "Bit"], "bool": ["bool", "Boolean"], "box": ["NpgsqlTypes.NpgsqlBox", "Box"], "bytea": ["byte[]", "Bytea"], "bpchar": ["string", "Varchar"], "cidr": ["ValueTuple<System.Net.IPAddress, int>", "Cidr"], "circle": ["NpgsqlTypes.NpgsqlCircle", "Circle"], "date": ["DateTime", "Date"], "numeric": ["decimal", "Numeric"], "float4": ["float", "Double"], "float8": ["double", "Double"], "inet": ["System.Net.IPAddress", "Inet"], "int2": ["short", "Integer"], "int4": ["int", "Integer"], "int8": ["long", "Bigint"], "interval": ["TimeSpan", "Interval"], "json": ["string", "Json"], "jsonb": ["string", "Jsonb"], "line": ["NpgsqlTypes.NpgsqlLine", "Line"], "lseg": ["NpgsqlTypes.NpgsqlLSeg", "LSeg"], "macaddr": ["System.Net.NetworkInformation.PhysicalAddress", "MacAddr"], "money": ["decimal", "Money"], "path": ["NpgsqlTypes.NpgsqlPath", "Path"], "point": ["NpgsqlTypes.NpgsqlPoint", "Point"], "text": ["string", "Text"], "time": ["TimeSpan", "Time"], "timestamp": ["DateTime", "Timestamp"], "timestamptz": ["DateTime", "TimestampTz"], "timetz": ["TimeSpan", "TimeTz"], "tsquery": ["NpgsqlTypes.NpgsqlTsQuery", "TsQuery"], "tsvector": ["NpgsqlTypes.NpgsqlTsVector", "TsVector"], "txid_snapshot": ["string", "Varchar"], "uuid": ["Guid", "Uuid"], "varbit": ["System.Collections.BitArray", "Varbit"], "varchar": ["string", "Varchar"], "xml": ["string", "Xml"] },
    };
    typeMap["MariaDB"] = typeMap.MySQL;
    Object.assign(typeMap.SQLite, typeMap.SQLServer);

    // 遍历表列
    ndkGenerateCode.echoTableColumn(tableObj => {
        console.debug(tableObj)
        var codes = [];

        // 类信息
        let classInfo = {
            namespace: "Netnr.Application", // 类空间
            modelClass: `Netnr.Domain.${tableObj.sntn.split('.').pop()}`, // 实体
            className: `${tableObj.sntn.split('.').pop()}Service`, //类名
            classComment: tableObj.tableColumns[0].TableComment || "", //类注释
            pprefix: tableObj.ctype == "Oracle" ? ":" : "@", // 参数前缀
            fields: tableObj.tableColumns.map(x => x.ColumnName), // 字段
            dbQuery: "DbHelper.SqlExecuteReader", //查询
            dbExecute: "DbHelper.SqlExecuteNonQuery", // 执行
            dbParameterClass: "", // 参数类
            dbParameterType: "", // 参数类型
        }

        //引用
        codes.push('using System;')
        codes.push('using System.Data;')
        switch (tableObj.ctype) {
            case "MySQL":
            case "MariaDB":
                codes.push('using MySql.Data.MySqlClient;')

                classInfo.dbParameterType = "MySqlDbType";
                classInfo.dbParameterClass = "MySqlParameter";
                break;
            case "SQLite":
                codes.push('using Microsoft.Data.Sqlite;')

                classInfo.dbParameterType = "SqliteType";
                classInfo.dbParameterClass = "SqliteParameter";
                break;
            case "Oracle":
                codes.push('using Oracle.ManagedDataAccess.Client;')

                classInfo.dbParameterType = "OracleDbType";
                classInfo.dbParameterClass = "OracleParameter";
                break;
            case "SQLServer":
                codes.push('using System.Data.SqlClient;')

                classInfo.dbParameterType = "SqlDbType";
                classInfo.dbParameterClass = "SqlParameter";
                break;
            case "PostgreSQL":
                codes.push('using Npgsql;')
                codes.push('using NpgsqlTypes;')

                classInfo.dbParameterType = "NpgsqlDbType";
                classInfo.dbParameterClass = "NpgsqlParameter";
                break;
        }
        codes.push('');

        //命名空间
        codes.push(`namespace ${classInfo.namespace}`);
        codes.push('{');

        //类注释
        codes.push(`\t/// <summary>`);
        classInfo.classComment.split("\n").forEach(x => codes.push(`\t/// ${x.trim()}`));
        codes.push(`\t/// </summary>`);

        //类名
        codes.push(`\tpublic partial class ${classInfo.className}`);
        codes.push(`\t{`);

        //构造方法
        codes.push(`\t\tpublic ${classInfo.className}() { }`);
        codes.push('');

        //构建方法-查询
        codes.push(`\t\t/// <summary>`);
        codes.push(`\t\t/// 查询`);
        codes.push(`\t\t/// </summary>`);
        codes.push(`\t\t/// <param name="whereSql">SQL条件，可选</param>`);
        codes.push(`\t\t/// <returns>返回数据集</returns>`);
        codes.push(`\t\tpublic DataSet Query(string whereSql = null)`);
        codes.push(`\t\t{`);
        codes.push(`\t\t\tvar sql = "select * from ${tableObj.sntn}";`);
        codes.push(`\t\t\tif (!string.IsNullOrWhiteSpace(whereSql))`);
        codes.push(`\t\t\t{`);
        codes.push(`\t\t\t\tsql += " where " + whereSql;`);
        codes.push(`\t\t\t}`);
        codes.push('');
        //执行脚本
        codes.push(`\t\t\tvar ds = ${classInfo.dbQuery}(sql);`);
        codes.push(`\t\t\treturn ds;`);
        codes.push(`\t\t}`);
        codes.push('');

        //构建方法-删除
        codes.push(`\t\t/// <summary>`);
        codes.push(`\t\t/// 删除`);
        codes.push(`\t\t/// </summary>`);
        codes.push(`\t\t/// <param name="whereSql">SQL条件</param>`);
        codes.push(`\t\t/// <returns>返回受影响行数</returns>`);
        codes.push(`\t\tpublic int Delete(string whereSql)`);
        codes.push(`\t\t{`);
        codes.push(`\t\t\tvar sql = "delete from ${tableObj.sntn} where " + whereSql;`);
        codes.push(`\t\t\tvar num = ${classInfo.dbExecute}(sql);`);
        codes.push(`\t\t\treturn num;`);
        codes.push(`\t\t}`);
        codes.push('');

        //构建方法-新增
        codes.push(`\t\t/// <summary>`);
        codes.push(`\t\t/// 新增`);
        codes.push(`\t\t/// </summary>`);
        codes.push(`\t\t/// <param name="model">新增实体对象</param>`);
        codes.push(`\t\t/// <returns>返回受影响行数</returns>`);
        codes.push(`\t\tpublic int Add(${classInfo.modelClass} model)`);
        codes.push(`\t\t{`);
        codes.push(`\t\t\tvar sql = @"insert into ${tableObj.sntn} `);
        codes.push(`\t\t\t\t\t\t(${classInfo.fields.join(", ")}) values `);
        codes.push(`\t\t\t\t\t\t(${classInfo.pprefix + classInfo.fields.join(', ' + classInfo.pprefix)})";`);
        codes.push('');
        //参数
        codes.push(`\t\t\t${classInfo.dbParameterClass}[] parameters = {`);
        tableObj.tableColumns.forEach(col => {
            let dbptype = ""; // 参数类型
            col.DataType = tableObj.ctype == "Oracle" ? col.DataType.toUpperCase() : col.DataType.toLowerCase();
            let dbo = typeMap[tableObj.ctype][col.DataType];
            if (dbo) {
                dbptype = dbo[1];
            }

            //类型 二次处理
            if (tableObj.ctype == "Oracle" && dbptype == "Decimal" && col.DataScale == 0) {
                dbptype = "Int32";
            }

            //长度
            let dbplength;
            //忽略长度
            if (isNaN(parseInt(col.DataLength))) {
                dbplength = "";
            } else if (tableObj.ctype == "Oracle" && dbptype.slice(-3) == "lob") {
                dbplength = "";
            } else {
                dbplength = `, ${col.DataLength}`;
            }

            // 参数
            let newparam = `new ${classInfo.dbParameterClass}("${classInfo.pprefix + col.ColumnName}", ${classInfo.dbParameterType}.${dbptype}${dbplength})`;
            codes.push(`\t\t\t\t${newparam},`);
        })
        codes.push(`\t\t\t};`);
        codes.push('');
        //填充参数值
        for (let index = 0; index < tableObj.tableColumns.length; index++) {
            let col = tableObj.tableColumns[index];
            codes.push(`\t\t\tparameters[${index}].Value = model.${col.ColumnName};`);
        }
        codes.push('');
        //执行脚本
        codes.push(`\t\t\tint num = ${classInfo.dbExecute}(sql, parameters);`);
        codes.push(`\t\t\treturn num;`);
        codes.push(`\t\t}`);
        codes.push('');

        //构建方法-修改
        codes.push(`\t\t/// <summary>`);
        codes.push(`\t\t/// 修改`);
        codes.push(`\t\t/// </summary>`);
        codes.push(`\t\t/// <param name="model">修改实体对象</param>`);
        codes.push(`\t\t/// <returns>返回受影响行数</returns>`);
        codes.push(`\t\tpublic int Update(${classInfo.modelClass} model)`);
        codes.push(`\t\t{`);
        codes.push(`\t\t\tvar sql = @"update ${tableObj.sntn} set `);
        let setkv1 = [];
        let setkv2 = [];
        tableObj.tableColumns.forEach(col => {
            let setkv = `${classInfo.pprefix + col.ColumnName}=${classInfo.pprefix + col.ColumnName}`;
            if (col.PrimaryKey > 0) {
                setkv2.push(setkv);
            } else {
                setkv1.push(setkv);
            }
        })
        codes.push(`\t\t\t\t\t\t${setkv1.join(", ")} `);
        codes.push(`\t\t\t\t\t\twhere ${setkv2.join(" and ")}";`);
        codes.push('');
        //参数
        codes.push(`\t\t\t${classInfo.dbParameterClass}[] parameters = {`);
        tableObj.tableColumns.forEach(col => {
            let dbptype = ""; // 参数类型
            col.DataType = tableObj.ctype == "Oracle" ? col.DataType.toUpperCase() : col.DataType.toLowerCase();
            let dbo = typeMap[tableObj.ctype][col.DataType];
            if (dbo) {
                dbptype = dbo[1];
            }

            //类型 二次处理
            if (tableObj.ctype == "Oracle" && dbptype == "Decimal" && col.DataScale == 0) {
                dbptype = "Int32";
            }

            //长度
            let dbplength;
            //忽略长度
            if (isNaN(parseInt(col.DataLength))) {
                dbplength = "";
            } else if (tableObj.ctype == "Oracle" && dbptype.slice(-3) == "lob") {
                dbplength = "";
            } else {
                dbplength = `, ${col.DataLength}`;
            }

            // 参数
            let newparam = `new ${classInfo.dbParameterClass}("${classInfo.pprefix + col.ColumnName}", ${classInfo.dbParameterType}.${dbptype}${dbplength})`;
            codes.push(`\t\t\t\t${newparam},`);
        })
        codes.push(`\t\t\t};`);
        codes.push('');
        //填充参数值
        for (let index = 0; index < tableObj.tableColumns.length; index++) {
            let col = tableObj.tableColumns[index];
            codes.push(`\t\t\tparameters[${index}].Value = model.${col.ColumnName};`);
        }
        codes.push('');
        //执行脚本
        codes.push(`\t\t\tint num = ${classInfo.dbExecute}(sql, parameters);`);
        codes.push(`\t\t\treturn num;`);
        codes.push(`\t\t}`);
        codes.push('');

        //构建方法-分页查询
        codes.push(`\t\t/// <summary>`);
        codes.push(`\t\t/// 分页查询`);
        codes.push(`\t\t/// </summary>`);
        codes.push(`\t\t/// <param name="pageNumber">页码</param>`);
        codes.push(`\t\t/// <param name="pageSize">页量</param>`);
        codes.push(`\t\t/// <param name="whereSql">SQL条件</param>`);
        codes.push(`\t\t/// <param name="sortOrder">排序</param>`);
        codes.push(`\t\t/// <param name="total">返回总条数</param>`);
        codes.push(`\t\t/// <returns>返回数据表及总条数</returns>`);
        codes.push(`\t\tpublic DataTable Query(int pageNumber, int pageSize, string whereSql, string sortOrder, out int total)`);
        codes.push(`\t\t{`);
        codes.push(`\t\t\tsortOrder = string.IsNullOrWhiteSpace(sortOrder) ? "" : "ORDER BY " + sortOrder;`);
        if (tableObj.ctype != "Oracle") {
            codes.push(`\t\t\twhereSql = string.IsNullOrWhiteSpace(whereSql) ? "" : "WHERE " + whereSql;`);
            codes.push('');
        }
        switch (tableObj.ctype) {
            case "MySQL":
            case "MariaDB":
                codes.push(`\t\t\tvar sql = $"SELECT * FROM ${tableObj.sntn} {whereSql} {sortOrder} LIMIT {(pageNumber - 1) * pageSize}, {pageSize}";`);
                codes.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${tableObj.sntn} {whereSql}";`);
                break;
            case "SQLite":
                codes.push(`\t\t\tvar sql = $"SELECT * FROM ${tableObj.sntn} {whereSql} {sortOrder} LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";`);
                codes.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${tableObj.sntn} {whereSql}";`);
                break;
            case "Oracle":
                codes.push(`\t\t\tvar ws1 = string.IsNullOrWhiteSpace(whereSql) ? "" : whereSql + " AND ";`);
                codes.push(`\t\t\tvar ws2 = string.IsNullOrWhiteSpace(whereSql) ? "" : "WHERE " + whereSql;`);
                codes.push('');
                codes.push(`\t\t\tvar sql = $"SELECT * FROM ( SELECT ROWNUM AS rowno, a.* FROM ${tableObj.sntn} a WHERE {ws1} ROWNUM <= {pageNumber * pageSize} {sortOrder} ) x WHERE x.rowno >= {(pageNumber - 1) * pageSize}";`);
                codes.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${tableObj.sntn} {ws2}";`);
                break;
            case "SQLServer":
                //ROW_NUMBER分页
                codes.push(`\t\t\tvar sql = $"SELECT TOP {rows} * FROM ( SELECT ROW_NUMBER() OVER( {sortOrder} ) AS numid, * FROM ${tableObj.sntn} {whereSql} ) x WHERE numid > {(pageNumber - 1) * pageSize}";`);
                //OFFSET分页，版本2012及以上
                //tableResult.push(`\t\t\tvar sql = $"SELECT * FROM ${tableName} {whereSql} {sortOrder} OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";`);
                codes.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${tableObj.sntn} {whereSql}";`);
                break;
            case "PostgreSQL":
                codes.push(`\t\t\tvar sql = $"SELECT * FROM ${tableObj.sntn} {whereSql} {sortOrder} LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";`);
                codes.push(`\t\t\tsql += $";SELECT COUNT(1) AS TOTAL FROM ${tableObj.sntn} {whereSql}";`);
                break;
        }
        codes.push('');
        codes.push(`\t\t\tvar ds = ${classInfo.dbQuery}(sql);`);
        codes.push(`\t\t\ttotal = Convert.ToInt32(ds.Table[1].Rows[0][0].ToString());`);
        codes.push(`\t\t\treturn ds.Table[0];`);
        codes.push(`\t\t}`);
        codes.push('');

        //类、命名空间结束
        codes.push(`\t}`);
        codes.push(`}`);

        // 添加文件项
        result.files.push({
            fullName: `Netnr.Application/${classInfo.className}.cs`,
            content: codes.join("\r\n")
        })
    });

    // 输出
    return result;
}