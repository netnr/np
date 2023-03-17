let nrcLeanCloud = {
    baseUrl: "https://lc.netnr.com/1.1",
    appId: "86m0YudK96J2iAcOSjbOMhFt-gzGzoHsz",
    appKey: "nea9RxxRgGVNd1xAgOf4biXa",

    fetch: async (url, ops) => {
        ops = ops || {};
        Object.assign(ops, {
            headers: {
                "X-LC-Id": nrcLeanCloud.appId,
                "X-LC-Key": nrcLeanCloud.appKey,
                "Content-Type": "application/json"
            }
        });

        let resp = await fetch(url, ops);
        let result = await resp.json();
        return result;
    },

    /**
     * 对象操作
     * @param {any} requests 数据，一个或数组
     */
    objBatch: async (requests) => {
        if (nrcLeanCloud.type(requests) != "Array") {
            requests = [requests];
        }

        let result = await nrcLeanCloud.fetch(`${nrcLeanCloud.baseUrl}/batch`, {
            method: "POST", body: JSON.stringify({ requests })
        });
        return result;
    },

    /**
     * 对象查询
     * @param {any} tableName 表名
     * @param {any} jsonArgs 参数
     */
    objQuery: async (tableName, jsonArgs) => {
        let url = `${nrcLeanCloud.baseUrl}/classes/${tableName}?${new URLSearchParams(jsonArgs)}`;
        let result = await nrcLeanCloud.fetch(url);

        return result.results ? result.results : result;
    },

    type: (obj) => {
        let tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },

    /**
     * 日期包裹
     * @param {any} date
     */
    wrapDate: (date) => {
        if (date && date.iso) {
            return new Date(date.iso)
        } else {
            return { __type: "Date", iso: date || new Date() }
        }
    },

    /**
     * 测试查询
     * @returns 
     */
    testObj: async () => {
        let result = await nrcLeanCloud.objBatch([
            {
                method: "POST",
                path: "/1.1/classes/tableName",
                body: {
                    nr_id: Date.now(),
                    nr_create: nrcLeanCloud.wrapDate(),
                    nr_status: 2
                }
            }
        ]);
        return result;
    }
}

export { nrcLeanCloud };