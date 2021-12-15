import { ndkFn } from './ndkFn';
import { ndkVary } from './ndkVary';

var ndkLs = {
    storeConfig: localforage.createInstance({ name: "ndk-config" }),
    storeCache: localforage.createInstance({ name: "ndk-cache" }),

    //键配置
    keys: {
        keyConns: {
            label: "连接信息",
            key: "conns",
        },
        keySteps: {
            label: "步骤信息",
            key: "steps",
        },
        keyNotes: {
            label: "笔记信息",
            key: "notes",
        },
        keyErrors: {
            label: "错误信息",
            key: "errors",
        }
    },

    /**
     * 设置连接字符串，根据 id 更新
     * @param {*} connObj 
     * @returns 
     */
    connsSet: connObj => new Promise((resolve) => {
        ndkLs.connsGet().then(conns => {
            if (ndkFn.type(connObj) != "Array") {
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
            ndkLs.storeConfig.setItem(ndkLs.keys.keyConns.key, conns).then(() => resolve(conns))
        })
    }),
    /**
     * 删除连接字符串
     * @param {*} id 一个或数组
     * @returns 
     */
    connsDelete: id => new Promise((resolve) => {
        id = ndkFn.type(id) == "Array" ? id : [id];
        ndkLs.connsGet().then(conns => {
            for (var i = 0; i < conns.length; i++) {
                var cobj = conns[i];
                if (id.includes(cobj.id)) {
                    conns.splice(i, 1);
                }
            }
            //删除连接缓存
            ndkLs.storeCache.keys().then(keys => {
                keys.filter(key => id.includes(key.split("-")[0])).forEach(x => ndkLs.storeCache.removeItem(x))
            })

            ndkVary.envConnsChanged = true; //连接变化            
            ndkLs.storeConfig.setItem(ndkLs.keys.keyConns.key, conns).then(() => resolve(conns))
        })
    }),
    /**
     * 获取连接字符串
     * @param {*} id 
     * @returns 
     */
    connsGet: id => new Promise((resolve) => {
        ndkLs.storeConfig.getItem(ndkLs.keys.keyConns.key).then(conns => {
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
    stepsSet: steps => ndkLs.storeConfig.setItem(ndkLs.keys.keySteps.key, steps),
    /**
     * 获取步骤
     */
    stepsGet: () => ndkLs.storeConfig.getItem(ndkLs.keys.keySteps.key),
    /**
     * 删除步骤
     */
    stepsDelete: () => ndkLs.storeConfig.removeItem(ndkLs.keys.keySteps.key),

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
            ndkLs.storeCache.getItem(idDatabaseTable.join('-')).then(res => resolve(res));
        } else {
            ndkLs.storeCache.setItem(idDatabaseTable.join('-'), { data, date: Date.now() }).then(res => resolve(res));
        }
    }),

    /**
     * 添加错误信息
     * @param {*} eobj 
     * @returns 
     */
    errorsAdd: (eobj) => new Promise((resolve) => {
        ndkLs.errorsGet().then(errors => {
            if (errors.length > 100) {
                errors.length = 50;
            }
            errors.push(Object.assign({ date: ndkFn.formatDateTime('datetime') }, eobj));
            ndkLs.storeConfig.setItem(ndkLs.keys.keyErrors.key, errors).then(() => resolve(errors))
        })
    }),
    /**
     * 获取错误信息
     * @param {*} eobj 
     * @returns 
     */
    errorsGet: () => new Promise((resolve) => {
        ndkLs.storeConfig.getItem(ndkLs.keys.keyErrors.key).then(errors => {
            resolve(errors || [])
        })
    }),

}

export { ndkLs }