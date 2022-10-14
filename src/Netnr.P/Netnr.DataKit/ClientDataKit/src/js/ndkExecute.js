import { ndkTab } from './ndkTab';
import { ndkVary } from './ndkVary';
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
     * @returns 
     */
    executeSql: (cobj, databaseName, sql) => new Promise((resolve, reject) => {
        var fd = new FormData();
        fd.append('tdb', cobj.type);
        fd.append('conn', cobj.conn);
        fd.append('sql', sql);
        if (databaseName) {
            fd.append('databaseName', databaseName);
        }

        //记录历史
        if (sql.length < 9999) {
            ndkStorage.historysAdd(cobj.id, databaseName, sql);
        }

        ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiExecuteSql}`, {
            method: "POST",
            body: fd
        }).then(res => resolve(res.data)).catch(err => reject(err));
    }),

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
    editorSql: (tpkey) => {
        var tpobj = ndkTab.tabKeys[tpkey];

        var sql = ndkEditor.selectedOrAllValue(tpobj.editor);
        if (sql.trim() == "") {
            ndkFunction.output(ndkI18n.lg.contentNotEmpty);
        } else {
            var tpcp = ndkStep.cpGet(tpkey)
            ndkExecute.executeSql(tpcp.cobj, tpcp.databaseName, sql).then(esdata => {
                ndkView.viewExecuteSql(esdata, tpkey)
            })
        }
    },

}

export { ndkExecute }