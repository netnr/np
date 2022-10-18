// @namespace   netnr
// @name        java_model
// @date        2022-10-16
// @version     1.0.0
// @description JAVA 生成 Model

async () => {
    // 输出结果
    let result = { language: "java", files: [] };

    let typeMap = {
        BigDecimal: ["decimal", "numeric"],
        Boolean: ["bit"],
        Date: ["date", "datetime", "timestamp"],
        Double: ["BINARY_DOUBLE", "double"],
        Float: ["BINARY_FLOAT", "float"],
        Integer: ["BINARY_INTEGER", "int", "int2", "int4", "integer", "mediumint", "NUMBER", "smallint"],
        Long: ["bigint", "int8"],
        String: ["char", "CLOB", "longtext", "mediumtext", "NVARCHAR", "NVARCHAR2", "text", "tinytext", "varchar", "VARCHAR2"]
    };

    // 遍历表列
    ndkGenerateCode.echoTableColumn(tableObj => {
        console.debug(tableObj)
        let codes = [];

        // 类信息
        let classInfo = {
            packageName: "project.name", // 项目包称
            moduleName: "netnr", // 模块名称
            tableName: tableObj.sntn.split('.').pop(), // 表名
            className: `${ndkFunction.handleClassName(tableObj.sntn.split('.').pop())}`, // 类名
            classComment: tableObj.tableColumns[0].TableComment || "", // 类注释
        }

        // 引用
        codes.push(`package ${classInfo.packageName}.${classInfo.moduleName}.entity;`);
        codes.push('');
        codes.push(`import lombok.Data;`);
        codes.push(`import lombok.EqualsAndHashCode;`);
        codes.push(`import com.baomidou.mybatisplus.annotation.*;`);
        codes.push('');
        codes.push('import java.math.BigDecimal;');
        codes.push('import java.util.Date;');
        codes.push('');

        // 类注释
        codes.push(`/**`);
        classInfo.classComment.split("\n").forEach(x => codes.push(` * ${x.trim()}`));
        codes.push(` */`);
        codes.push('');

        // 类
        codes.push(`@Data`);
        codes.push(`@TableName("${classInfo.tableName}")`);
        codes.push(`public class ${classInfo.className}Entity {`);

        // 构建项
        tableObj.tableColumns.forEach(columnObj => {
            //列注释
            codes.push('');
            codes.push(`\t/**`);
            (columnObj.ColumnComment || "").split("\n").forEach(x => codes.push(`\t * ${x.trim()}`));
            codes.push(`\t */`);

            columnObj.DataType = tableObj.ctype == "Oracle" ? columnObj.DataType.toUpperCase() : columnObj.DataType.toLowerCase();

            //属性
            let propType = 'String'; // 默认类型
            for (const pType in typeMap) {
                let dataTypes = typeMap[pType];
                if (dataTypes.includes(columnObj.DataType)) {
                    propType = pType;
                    break;
                }
            }
            //类型 二次处理
            if (tableObj.ctype == "Oracle" && propType == "decimal" && columnObj.DataScale == 0) {
                propType = "Integer";
            }

            let propField = ndkFunction.handleClassName(columnObj.ColumnName, true);
            codes.push(`\tprivate ${propType} ${propField};`);
        });

        //类结束
        codes.push(`}`);

        // 添加文件项
        result.files.push({
            fullName: `src/main/java/${classInfo.packageName.replaceAll('.', '/')}/${classInfo.moduleName}/entity/${classInfo.className}Entity.java`,
            content: codes.join("\r\n")
        })


        // 资源文件 xml
        codes = [];
        codes.push('<?xml version="1.0" encoding="UTF-8"?>');
        codes.push('<!DOCTYPE mapper PUBLIC "-//mybatis.org//DTD Mapper 3.0//EN" "http://mybatis.org/dtd/mybatis-3-mapper.dtd">');
        codes.push('');
        codes.push(`<mapper namespace="${classInfo.packageName}.${classInfo.moduleName}.dao.${classInfo.className}Dao">`);
        let idTable = ndkFunction.handleClassName(classInfo.className, true);
        codes.push(`\t<resultMap type="${classInfo.packageName}.${classInfo.moduleName}.entity.${classInfo.className}Entity" id="${idTable}Map">`);
        tableObj.tableColumns.forEach(columnObj => {
            let propField = ndkFunction.handleClassName(columnObj.ColumnName, true);
            codes.push(`\t\t<result property="${propField}" column="${columnObj.ColumnName}"/>`);
        })
        codes.push('\t<resultMap>');
        codes.push('</mapper>');

        // 添加文件项
        result.files.push({
            fullName: `src/main/resources/mapper/${classInfo.moduleName}/${classInfo.className}Dao.xml`,
            content: codes.join("\r\n")
        })
    });

    // 输出
    return result;
}