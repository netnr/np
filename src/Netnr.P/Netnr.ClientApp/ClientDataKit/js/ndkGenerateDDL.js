import { ndkI18n } from './ndkI18n';
import { ndkFunction } from './ndkFunction';
import { ndkRequest } from './ndkRequest';
import { ndkVary } from './ndkVary';
import { ndkEditor } from './ndkEditor';

// 获取 DDL
var ndkGenerateDDL = {

    /**
     * 循环单表
     * @param {*} cobj 连接
     * @param {*} databaseName 数据库名
     * @param {*} tableAndSchemaArray 表名和模式名数组
     * @param {*} index 索引
     * @param {*} success 成功回调
     * @param {*} error 错误回调
     */
    loopTable: (cobj, databaseName, tableAndSchemaArray, index, success, error) => {
        let tableName = tableAndSchemaArray[index].TableName;
        let schemaName = tableAndSchemaArray[index].SchemaName || "";
        let sntnShow = ["SQLServer", "PostgreSQL"].includes(cobj.type)
            ? [schemaName, tableName].filter(x => x != "").join(".")
            : tableName;

        // 输出进度
        ndkFunction.output(`${sntnShow} ${index + 1}/${tableAndSchemaArray.length}`);
        // 请求单表
        ndkRequest.reqTableDDL(cobj, databaseName, schemaName, tableName).then(res => {
            var ddl = res.data;
            if (cobj.type == "ClickHouse") {
                ddl = ndkEditor.formatterSQL(ddl, cobj.type);
            }

            var tableDesc = '-- --------------------';
            ndkVary.generateDDL.push(`${tableDesc}`);
            ndkVary.generateDDL.push(`-- ${sntnShow}`);
            ndkVary.generateDDL.push(`${tableDesc}`);
            ndkVary.generateDDL.push(ddl);
            ndkVary.generateDDL.push("");

            //下一个表
            if (++index < tableAndSchemaArray.length) {
                ndkGenerateDDL.loopTable(cobj, databaseName, tableAndSchemaArray, index, success, error);
            } else {
                ndkFunction.output(ndkI18n.lg.done); //完成
                success();
            }
        }).catch(err => {
            error(err.msg);
        });
    },

    /**
     * 请求 DDL
     * @param {*} cobj 
     * @param {*} databaseName 数据库名
     * @param {*} tableAndSchemaArray [{TableName: "", SchemaName: ""}]
     * @returns 
     */
    reqDDL: (cobj, databaseName, tableAndSchemaArray) => new Promise((resolve, reject) => {
        ndkVary.generateDDL = [];

        switch (cobj.type) {
            case "SQLite":
                {
                    ndkVary.generateDDL.push(`-- ${cobj.type}`);
                    ndkVary.generateDDL.push(`-- ${cobj.conn}`);
                    ndkVary.generateDDL.push(`-- Database: ${databaseName}`);
                    ndkVary.generateDDL.push(`-- Date: ${ndkFunction.now()}`);
                    ndkVary.generateDDL.push("");
                    ndkVary.generateDDL.push(`PRAGMA foreign_keys = false;`);
                    ndkVary.generateDDL.push("");

                    ndkGenerateDDL.loopTable(cobj, databaseName, tableAndSchemaArray, 0, function () {
                        ndkVary.generateDDL.push(`PRAGMA foreign_keys = true;`);
                        resolve(ndkVary.generateDDL.join('\r\n'));
                    }, function (err) {
                        reject(err)
                    });
                }
                break;
            case 'MySQL':
            case 'MariaDB':
                {
                    ndkVary.generateDDL.push(`-- ${cobj.type}`);
                    ndkVary.generateDDL.push(`-- Database: ${databaseName}`);
                    ndkVary.generateDDL.push(`-- Date: ${ndkFunction.now()}`);
                    ndkVary.generateDDL.push("");
                    ndkVary.generateDDL.push(`SET FOREIGN_KEY_CHECKS = 0;`);
                    ndkVary.generateDDL.push("");

                    ndkGenerateDDL.loopTable(cobj, databaseName, tableAndSchemaArray, 0, function () {
                        ndkVary.generateDDL.push(`SET FOREIGN_KEY_CHECKS = 1;`);
                        resolve(ndkVary.generateDDL.join('\r\n'));
                    }, function (err) {
                        reject(err)
                    });
                }
                break;
            case 'Oracle':
            case 'Dm':
            case 'SQLServer':
            case 'PostgreSQL':
            case 'ClickHouse':
                {
                    ndkVary.generateDDL.push(`-- ${cobj.type}`);
                    ndkVary.generateDDL.push(`-- Database: ${databaseName}`);
                    ndkVary.generateDDL.push(`-- Date: ${ndkFunction.now()}`);
                    ndkVary.generateDDL.push("");

                    ndkGenerateDDL.loopTable(cobj, databaseName, tableAndSchemaArray, 0, function () {
                        resolve(ndkVary.generateDDL.join('\r\n'));
                    }, function (err) {
                        reject(err)
                    });
                }
                break;
            default:
                reject(ndkI18n.lg.unsupported);
                break;
        }
    })

}

export { ndkGenerateDDL }