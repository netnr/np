// import localforage from 'localforage';
import { ndkVary } from './ndkVary';
import { ndkFunction } from './ndkFunction';
import { ndkStep } from './ndkStep';

// 存储
var ndkStorage = {

    // 存储实例
    instanceConfig: null,
    instanceCache: null,

    // 初始化
    init: localforage => {
        ndkStorage.instanceConfig = localforage.createInstance({ name: "ndk-config" });
        ndkStorage.instanceCache = localforage.createInstance({ name: "ndk-cache" });
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
    connsSet: connObj => new Promise((resolve) => {
        ndkStorage.connsGet().then(conns => {
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
            ndkStorage.instanceConfig.setItem(ndkStorage.keys.keyConns.key, conns).then(() => resolve(conns))
        })
    }),
    /**
     * 删除连接字符串
     * @param {*} id 一个或数组
     * @returns 
     */
    connsDelete: id => new Promise((resolve) => {
        id = ndkFunction.type(id) == "Array" ? id : [id];
        ndkStorage.connsGet().then(conns => {
            for (var i = 0; i < conns.length; i++) {
                var cobj = conns[i];
                if (id.includes(cobj.id)) {
                    conns.splice(i, 1);
                }
            }
            //删除连接缓存
            ndkStorage.instanceCache.keys().then(keys => {
                keys.filter(key => id.includes(key.split("-")[0])).forEach(x => ndkStorage.instanceCache.removeItem(x))
            })

            ndkVary.envConnsChanged = true; //连接变化            
            ndkStorage.instanceConfig.setItem(ndkStorage.keys.keyConns.key, conns).then(() => resolve(conns))
        })
    }),
    /**
     * 获取连接字符串
     * @param {*} id 
     * @returns 
     */
    connsGet: id => new Promise((resolve) => {
        ndkStorage.instanceConfig.getItem(ndkStorage.keys.keyConns.key).then(conns => {
            conns = conns || [];
            if (id != null) {
                resolve(conns.filter(x => x.id == id).pop() || [])
            } else {
                resolve(conns)
            }
        })
    }),

    /**
     * 设置步骤
     * @param {any} steps
     */
    stepsSet: steps => ndkStorage.instanceConfig.setItem(ndkStorage.keys.keySteps.key, steps),
    /**
     * 获取步骤
     */
    stepsGet: () => ndkStorage.instanceConfig.getItem(ndkStorage.keys.keySteps.key),
    /**
     * 删除步骤
     */
    stepsDelete: () => ndkStorage.instanceConfig.removeItem(ndkStorage.keys.keySteps.key),

    /**
     * 添加错误信息
     * @param {*} eobj 
     * @returns 
     */
    errorsAdd: (eobj) => new Promise((resolve) => {
        ndkStorage.errorsGet().then(errors => {
            errors = errors || [];
            if (errors.length > 100) {
                errors = errors.slice(50);
            }
            errors.push(Object.assign({ date: ndkFunction.now() }, eobj));
            ndkStorage.instanceConfig.setItem(ndkStorage.keys.keyErrors.key, errors).then(() => resolve(errors))
        })
    }),
    /**
     * 获取错误信息
     * @param {*} eobj 
     * @returns 
     */
    errorsGet: () => new Promise((resolve) => {
        ndkStorage.instanceConfig.getItem(ndkStorage.keys.keyErrors.key).then(errors => {
            resolve(errors || [])
        })
    }),

    /**
     * 添加历史记录
     * @param {*} cobjId 
     * @param {*} databaseName 
     * @param {*} sql 
     * @returns 
     */
    historysAdd: (cobjId, databaseName, sql) => new Promise((resolve) => {
        ndkStorage.historysGet().then(historys => {
            historys = historys || [];
            if (historys.length > ndkVary.parameterConfig.maxSqlHistory.value * 2) {
                historys = historys.slice(ndkVary.parameterConfig.maxSqlHistory.value);
            }
            historys.unshift({ sql: sql, date: ndkFunction.now(), databaseName: databaseName, cobjId: cobjId });
            ndkStorage.instanceConfig.setItem(ndkStorage.keys.keyHistorys.key, historys).then(() => resolve(historys))
        })
    }),
    /**
     * 获取历史信息
     * @param {*} eobj 
     * @returns 
     */
    historysGet: () => ndkStorage.instanceConfig.getItem(ndkStorage.keys.keyHistorys.key),

    /**
     * 获取或设置连接缓存
     * @param {any} idDatabaseTable [连接ID,数据库名,表名]
     * @param {any} data
     */
    cc: (idDatabaseTable, data) => new Promise((resolve) => {
        switch (idDatabaseTable.length) {
            case 1: idDatabaseTable.splice(1, 0, 'database'); break;
            case 2: idDatabaseTable.splice(1, 0, 'table'); break;
            default: idDatabaseTable.splice(1, 0, 'column'); break;
        }
        if (data == null) {
            ndkStorage.instanceCache.getItem(idDatabaseTable.join('-')).then(res => resolve(res));
        } else {
            ndkStorage.instanceCache.setItem(idDatabaseTable.join('-'), { data, date: ndkFunction.now() }).then(res => resolve(res));
        }
    }),
    // 刷表
    ccBrushTable: () => {
        var cp = ndkStep.cpGet(1);
        var data = agg.getAllRows(ndkVary.gridOpsTable);
        ndkStorage.cc([cp.cobj.id, cp.databaseName], data);
    }
}

export { ndkStorage }