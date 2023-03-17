// @namespace   netnr
// @name        csharp_nrf_table_config
// @date        2022-10-16
// @version     1.0.0
// @description NRF 生成表配置 SysTableConfig

async () => {
    // 输出结果
    let result = { language: "sql", files: [] };

    // 遍历表列
    ndkGenerateCode.echoTableColumn(tableObj => {
        console.debug(tableObj)
        var codes = [];

        tableObj.tableColumns.forEach(columnObj => {
            var rowData = {
                Id: ndkFunction.UUID(),
                TableName: `${columnObj.TableName}`,
                ColField: columnObj.ColumnName,
                DvTitle: columnObj.ColumnComment || columnObj.ColumnName,
                ColTitle: columnObj.ColumnComment || columnObj.ColumnName,
                ColWidth: 100,
                ColAlign: 1,
                ColHide: 0,
                ColOrder: columnObj.ColumnOrder,
                ColFrozen: 0,
                ColFormat: "0",
                ColSort: 0,
                ColExport: 1,
                ColQuery: 0,
                ColRelation: "",
                FormArea: 1,
                FormUrl: null,
                FormType: "text",
                FormSpan: 1,
                FormHide: 0,
                FormOrder: columnObj.ColumnOrder,
                FormRequired: 0,
                FormPlaceholder: null,
                FormValue: null,
                FormText: null,
                FormMaxlength: columnObj.DataLength > 0 ? columnObj.DataLength : null
            };

            var values = [];
            for (let key in rowData) {
                var val = rowData[key];
                switch (ndkFunction.type(val)) {
                    case "Null":
                        values.push('null');
                        break;
                    case "Number":
                        values.push(val);
                        break;
                    default:
                        if (tableObj.ctype == "SQLServer") {
                            values.push(`N'${val.replaceAll("'", "''")}'`);
                        } else {
                            values.push(`'${val.replaceAll("'", "''")}'`);
                        }
                        break;
                }
            }
            codes.push(`INSERT INTO ${tableObj.sntn} VALUES(${values.join(', ')})`);
        })

        // 添加文件项
        result.files.push({
            fullName: `SysTableConfig.sql`,
            content: codes.join(";\r\n")
        })
    });

    // 输出
    return result;
}