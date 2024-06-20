import { ndkFunction } from "./ndkFunction";
import { ndkI18n } from "./ndkI18n";
import { ndkStep } from "./ndkStep";
import { ndkVary } from "./ndkVary";

// 生成代码
var ndkGenerateCode = {
    /**
     * 代码内容
     * @param {*} name 文件名
     */
    codeContent: async (name) => {
        var paths = location.pathname.split('/').slice(0, -1);
        paths.push(`assets/codes/${name}`);
        var url = paths.join('/');
        var result = await fetch(url, { cache: 'no-cache' }).then(resp => resp.text());
        return result;
    },

    /**
     * 遍历表列 (所有列 表列 数据库类型 模式名.表名数组 模式名.表名)
     * @param {*} fnCallback (allColumns, tableColumns, ctype, sntns, sntn)
     * @returns 
     */
    echoTableColumn: (fnCallback) => {
        // 表设计（列信息）、数据库类型
        var allColumns = [], ctype;
        if (ndkVary.gridOpsColumn) {
            allColumns = ndkVary.gridOpsColumn.getSelectedRows();
            ctype = ndkStep.cpGet(1).cobj.type;
        }
        if (allColumns.length == 0) {
            ndkFunction.output(ndkI18n.lg.selectColumn);
            return ndkI18n.lg.selectColumn;
        }

        // 分组：模式名.表名
        let sntns = ndkFunction.groupBy(allColumns, x => x.SchemaName == null ? x.TableName : `${x.SchemaName}.${x.TableName}`);
        sntns.forEach(sntn => {
            let tableColumns = allColumns.filter(x => x.SchemaName == null ? x.TableName == sntn : `${x.SchemaName}.${x.TableName}` == sntn); // 单表
            var obj = { allColumns, tableColumns, ctype, sntns, sntn };
            fnCallback(obj);
        });
    },
}

export { ndkGenerateCode }