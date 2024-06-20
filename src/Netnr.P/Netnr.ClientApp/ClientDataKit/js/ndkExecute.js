import { ndkTab } from './ndkTab';
import { ndkVary } from './ndkVary';
import { ndkStep } from './ndkStep';
import { ndkView } from './ndkView';
import { ndkEditor } from './ndkEditor';
import { ndkFunction } from './ndkFunction';
import { ndkRequest } from './ndkRequest';
import { ndkStorage } from './ndkStorage';

// 执行脚本
var ndkExecute = {

    /**
     * 执行SQL
     * @param {*} cobj
     * @param {*} forcedReload
     * @returns 返回执行结果数据
     */
    executeSql: async (cobj, databaseName, sql) => {
        var fd = new FormData();
        fd.append('dbt', cobj.type);
        fd.append('conn', cobj.conn);
        fd.append('sql', sql);
        fd.append("openTransaction", ndkVary.parameterConfig.openTransaction.value);
        if (databaseName) {
            fd.append('databaseName', databaseName);
        }

        //记录历史
        if (sql.length < 9999) {
            await ndkStorage.historysAdd(cobj.id, databaseName, sql);
        }

        let res = await ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiExecuteSql}`, { method: "POST", body: fd });
        return res.data;
    },

    /**
     * 获取 首行首列
     * @param {*} data 
     * @returns 
     */
    getDataCell: (data) => Object.values(data.Item1.table1[0])[0],
    /**
     * 获取 数据表
     * @param {*} data 
     * @returns 
     */
    getDataTable: (data) => data.Item1.table1,

    /**
     * 执行编辑器 SQL
     * @param {any} tpkey
     */
    editorSql: async (tpkey) => {
        var tpobj = ndkTab.tabKeys[tpkey];

        var sql = ndkEditor.selectedOrAllValue(tpobj.editor);
        if (sql.trim() == "") {
            ndkFunction.output(ndkI18n.lg.contentNotEmpty);
        } else {
            var tpcp = ndkStep.cpGet(tpkey);
            var esdata = await ndkExecute.executeSql(tpcp.cobj, tpcp.databaseName, sql);
            var tpobj = await ndkView.viewExecuteSql(esdata, tpkey);
            return tpobj;
        }
    },

}

export { ndkExecute }