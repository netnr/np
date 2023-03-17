/**
 * upstream: From nginx upstream
 * Source must support cross-domain
 * 
 * @param {*} servers 
 * @param {*} timeout 
 */
let nrcUpstream = {

    /**
     * 获取代理
     * @param {*} proxys 
     * @param {*} timeout 
     * @returns 
     */
    getProxy: (proxys, timeout = 5000) => new Promise((resolve) => {
        let result = { date: null, ok: [], bad: [] }
        try {
            let json = localStorage.getItem(nrcUpstream.storageKey);
            if (json != null && json != "") {
                result = JSON.parse(json);
            }
        } catch (ex) { }

        if (result && result.ok.length && Date.now() - result.date < nrcUpstream.expiredTime) {
            //从缓存
            resolve(result.ok[0], result.ok);
        } else {
            Object.assign(result, { date: Date.now(), ok: [], bad: [] });

            //开始时间
            let startTime = Date.now();

            //请求源
            proxys.forEach(proxy => {
                fetch(proxy)
                    .then(resp => resp.ok ? result.ok.push(proxy) : result.bad.push(proxy))
                    .catch(() => result.bad.push(proxy));
            });

            //开始监听
            let si = setInterval(() => {
                let isc = false; //取消监听

                if (timeout == 1 && result.ok.length > 0) {
                    //当 timeout 为 1，返回最快可用
                    isc = true;
                } else {
                    //所有请求结束 或 超时，返回结果
                    let isTimeout = Date.now() - startTime > ((timeout == 1 || !timeout) ? nrcUpstream.expiredTime : timeout);
                    if (isTimeout || result.ok.length + result.bad.len == proxys.length) {
                        isc = true;
                    }
                }

                //结束监听
                if (isc) {
                    clearInterval(si);

                    //存储
                    localStorage.setItem(nrcUpstream.storageKey, JSON.stringify(result));

                    resolve(result.ok[0], result.ok);
                }
            }, 10)
        }
    }),

    storageKey: "upstream",
    expiredTime: 120 * 1000,

    /**
     * 请求
     * @param {*} url 
     * @param {*} option 
     * @param {*} encoding 编码，utf-8 GBK
     */
    fetch: (url, option, encoding = "utf-8") => new Promise((resolve, reject) => {
        let proxyServers = ["https://cors.zme.ink/", "https://netnr.zme.ink/api/v1/Proxy?url="];

        nrcUpstream.getProxy(proxyServers).then((fastProxy) => {
            if (fastProxy) {
                url = `${fastProxy}${encodeURIComponent(url)}`;

                fetch(url, option).then(resp => resp.blob()).then(blob => {
                    let reader = new FileReader();
                    reader.onload = (e) => resolve(e.target.result);
                    reader.readAsText(blob, encoding)
                }).catch(ex => reject(ex))
            } else {
                reject(ok);
            }
        })
    })
}

export { nrcUpstream }