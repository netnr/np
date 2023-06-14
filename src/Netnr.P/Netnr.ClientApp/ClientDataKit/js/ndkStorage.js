import { ndkVary } from './ndkVary';
import { ndkFunction } from './ndkFunction';
import { ndkStep } from './ndkStep';
import { nrcIndexedDB } from '../../frame/nrcIndexedDB';
import { nrGrid } from '../../frame/nrGrid';

// 存储
var ndkStorage = {

    // 存储实例
    instanceConfig: null,
    instanceCache: null,

    // 初始化
    init: async () => {
        //存储初始化
        ndkStorage.instanceConfig = await new nrcIndexedDB({ name: "ndk-config" }).init();
        ndkStorage.instanceCache = await new nrcIndexedDB({ name: "ndk-cache" }).init();
    },

    //键配置
    keys: {
        keyConns: {
            key: "conns"
        },
        keySteps: {
            key: "steps"
        },
        keyErrors: {
            key: "errors"
        },
        keyNotes: {
            key: "notes"
        },
        keyHistorys: {
            key: "historys"
        },
    },

    /**
     * 设置连接字符串，根据 id 更新
     * @param {*} connObj 
     * @returns 
     */
    connsSet: async (connObj) => {
        let conns = await ndkStorage.connsGet();
        if (ndkFunction.type(connObj) != "Array") {
            connObj = [connObj];
        }

        connObj.forEach(x => {
            for (var i = conns.length - 1; i >= 0; i--) {
                var cobj = conns[i];
                if (cobj.id == x.id) {
                    conns.splice(i, 1);
                    break;
                }
            }
        })

        conns = conns.concat(connObj)

        ndkVary.envConnsChanged = true; //连接变化
        await ndkStorage.instanceConfig.setItem(ndkStorage.keys.keyConns.key, conns);
        return conns;
    },
    /**
     * 删除连接字符串
     * @param {*} id 一个或数组
     * @returns 
     */
    connsDelete: async (id) => {
        id = ndkFunction.type(id) == "Array" ? id : [id];
        let conns = await ndkStorage.connsGet();
        for (var i = 0; i < conns.length; i++) {
            var cobj = conns[i];
            if (id.includes(cobj.id)) {
                conns.splice(i, 1);
            }
        }
        //删除连接缓存
        let keys = await ndkStorage.instanceCache.keys();
        keys.filter(key => id.includes(key.split("-")[0])).forEach(x => ndkStorage.instanceCache.removeItem(x))

        ndkVary.envConnsChanged = true; //连接变化            
        await ndkStorage.instanceConfig.setItem(ndkStorage.keys.keyConns.key, conns);
        return conns;
    },
    /**
     * 获取连接字符串
     * @param {*} id 
     * @returns 
     */
    connsGet: async (id) => {
        let conns = await ndkStorage.instanceConfig.getItem(ndkStorage.keys.keyConns.key);
        conns = conns || [];
        if (id != null) {
            conns = conns.filter(x => x.id == id).pop() || [];
        }
        return conns;
    },

    /**
     * 设置步骤
     * @param {any} steps
     */
    stepsSet: async (steps) => await ndkStorage.instanceConfig.setItem(ndkStorage.keys.keySteps.key, steps),
    /**
     * 获取步骤
     */
    stepsGet: async () => await ndkStorage.instanceConfig.getItem(ndkStorage.keys.keySteps.key),
    /**
     * 删除步骤
     */
    stepsDelete: async () => await ndkStorage.instanceConfig.removeItem(ndkStorage.keys.keySteps.key),

    /**
     * 添加错误信息
     * @param {*} eobj 
     * @returns 
     */
    errorsAdd: async (eobj) => {
        let errors = await ndkStorage.errorsGet();
        if (errors.length > 100) {
            errors = errors.slice(50);
        }
        errors.push(Object.assign({ date: ndkFunction.now() }, eobj));
        await ndkStorage.instanceConfig.setItem(ndkStorage.keys.keyErrors.key, errors);
        return errors;
    },
    /**
     * 获取错误信息
     * @param {*} eobj 
     * @returns 
     */
    errorsGet: async () => await ndkStorage.instanceConfig.getItem(ndkStorage.keys.keyErrors.key) || [],

    /**
     * 添加历史记录
     * @param {*} cobjId 
     * @param {*} databaseName 
     * @param {*} sql 
     * @returns 
     */
    historysAdd: async (cobjId, databaseName, sql) => {
        let historys = await ndkStorage.historysGet();
        if (historys.length > ndkVary.parameterConfig.maxSqlHistory.value * 2) {
            historys = historys.slice(ndkVary.parameterConfig.maxSqlHistory.value);
        }
        historys.unshift({ sql: sql, date: ndkFunction.now(), databaseName: databaseName, cobjId: cobjId });
        await ndkStorage.instanceConfig.setItem(ndkStorage.keys.keyHistorys.key, historys);
        return historys;
    },
    /**
     * 获取历史信息
     * @param {*} eobj 
     * @returns 
     */
    historysGet: async () => await ndkStorage.instanceConfig.getItem(ndkStorage.keys.keyHistorys.key) || [],

    /**
     * 获取或设置连接缓存
     * @param {any} idDatabaseTable [连接ID,数据库名,表名]
     * @param {any} data
     */
    cc: async (idDatabaseTable, data) => {
        switch (idDatabaseTable.length) {
            case 1: idDatabaseTable.splice(1, 0, 'database'); break;
            case 2: idDatabaseTable.splice(1, 0, 'table'); break;
            default: idDatabaseTable.splice(1, 0, 'column'); break;
        }
        if (data == null) {
            let result = await ndkStorage.instanceCache.getItem(idDatabaseTable.join('-'));
            return result;
        } else {
            let result = await ndkStorage.instanceCache.setItem(idDatabaseTable.join('-'), { data, date: ndkFunction.now() });
            return result;
        }
    },
    // 刷表
    ccBrushTable: async () => {
        var cp = ndkStep.cpGet(1);
        var data = nrGrid.getAllRows(ndkVary.gridOpsTable);
        var result = await ndkStorage.cc([cp.cobj.id, cp.databaseName], data);
        return result;
    }
}

export { ndkStorage }