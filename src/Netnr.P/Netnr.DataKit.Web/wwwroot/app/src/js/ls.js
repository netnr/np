import { fn } from './fn';

var ls = {
    dkStore: localforage.createInstance({ name: "ndk" }),
    ccStore: localforage.createInstance({ name: "ndk-cc" }),

    keyConns: "conns",
    keySteps: "steps",

    /**
     * 设置连接字符串，根据 id 更新
     * @param {*} connObj 
     * @returns 
     */
    connsSet: connObj => new Promise((resolve) => {
        ls.connsGet().then(conns => {
            if (fn.type(connObj) != "Array") {
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

            ls.dkStore.setItem(ls.keyConns, conns).then(() => resolve(conns))
        })
    }),
    /**
     * 删除连接字符串
     * @param {*} id 
     * @returns 
     */
    connsDelete: id => new Promise((resolve) => {
        ls.connsGet().then(conns => {
            for (var i = 0; i < conns.length; i++) {
                var cobj = conns[i];
                if (cobj.id == id) {
                    conns.splice(i, 1);
                    break;
                }
            }

            ls.dkStore.setItem(ls.keyConns, conns).then(() => resolve(conns))
        })
    }),
    /**
     * 获取连接字符串
     * @param {*} id 
     * @returns 
     */
    connsGet: id => new Promise((resolve) => {
        ls.dkStore.getItem(ls.keyConns).then(conns => {
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
    stepsSet: steps => ls.dkStore.setItem(ls.keySteps, steps),
    /**
     * 获取步骤
     */
    stepsGet: () => ls.dkStore.getItem(ls.keySteps),
    /**
     * 删除步骤
     */
    stepsDelete: () => ls.dkStore.removeItem(ls.keySteps),

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
            ls.ccStore.getItem(idDatabaseTable.join('-')).then(res => resolve(res));
        } else {
            ls.ccStore.setItem(idDatabaseTable.join('-'), { data, date: Date.now() }).then(res => resolve(res));
        }
    })

}

export { ls }