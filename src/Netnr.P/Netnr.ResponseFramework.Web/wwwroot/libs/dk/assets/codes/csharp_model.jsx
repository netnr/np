// @namespace   netnr
// @name        csharp_model
// @date        2022-10-16
// @version     1.0.0
// @description C# 生成 Model

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
        let codes = [];

        // 类信息
        let classInfo = {
            namespace: "Netnr.Domain", //类空间
            className: tableObj.sntn.split('.').pop(), //类名
            classComment: tableObj.tableColumns[0].TableComment || "", //类注释
        }

        // 引用
        codes.push(`using System;`)
        codes.push('');

        // 命名空间
        codes.push(`namespace ${classInfo.namespace}`);
        codes.push(`{`);

        // 类注释
        codes.push(`\t/// <summary>`);
        classInfo.classComment.split("\n").forEach(x => codes.push(`\t/// ${x.trim()}`));
        codes.push(`\t/// </summary>`);

        // 类名
        codes.push(`\tpublic partial class ${classInfo.className}`);
        codes.push(`\t{`);

        // 构建项
        tableObj.tableColumns.forEach(columnObj => {
            //列注释
            codes.push(`\t\t/// <summary>`);
            (columnObj.ColumnComment || "").split("\n").forEach(x => codes.push(`\t\t/// ${x.trim()}`));
            codes.push(`\t\t/// </summary>`);

            columnObj.DataType = tableObj.ctype == "Oracle" ? columnObj.DataType.toUpperCase() : columnObj.DataType.toLowerCase();

            //属性                        
            let propType = 'object'; // 默认类型
            let dbo = typeMap[tableObj.ctype][columnObj.DataType];
            if (dbo) {
                propType = dbo[0];
            }
            //类型 二次处理
            if (tableObj.ctype == "Oracle" && propType == "decimal" && columnObj.DataScale == 0) {
                propType = "int";
            }

            //数据库可为空、类型可为空
            if (columnObj.IsNullable == 1 && typeMap.isNullable.includes(propType)) {
                propType += "?";
            }
            codes.push(`\t\tpublic ${propType} ${columnObj.ColumnName} { get; set; }`);
        });

        //类、命名空间结束
        codes.push(`\t}`);
        codes.push(`}`);

        // 添加文件项
        result.files.push({
            fullName: `Netnr.Domain/${classInfo.className}.cs`,
            content: codes.join("\r\n")
        })
    });

    // 输出
    return result;
}