/*
    upstream: From nginx upstream
    Source must support cross-domain
    v1.0.1
    by netnr
    2021-01-15
 */

(function (window) {
    var ups = function (hosts, callback, timeout) {
        //全局对象、默认请求超时、默认源过期
        var gk = "upstreamCache", dto = 3000, es = 30000;
        if (!(gk in window)) {
            try {
                window[gk] = JSON.parse(localStorage.getItem(gk)) || {};
            } catch (e) {
                window[gk] = {}
            }
        }

        var startTime = new Date().valueOf(),
            cacheKey = hosts.join(','),
            hostsCache = window[gk][cacheKey];

        if (hostsCache && startTime - hostsCache.date < es) {
            callback(hostsCache.ok[0], hostsCache.ok, true);
        } else {
            var ok = [], bad = 0, i = 0, len = hosts.length;
            for (; i < len;) {
                var host = hosts[i++];
                //自动补齐链接
                host = host.trim().toLowerCase().indexOf("//") >= 0 ? host : "//" + host;
                //发起fetch，添加成功的url（该url与hosts可能不一样），须支持跨域请求
                fetch(host).then(function (res) {
                    res.ok ? ok.push(res.url) : bad++;
                }).catch(() => bad++)
            }
            var si = setInterval(function () {
                var isc = false, now = new Date().valueOf();
                //当timeout为1，返回最快可用的host
                if (timeout == 1 && ok.length > 0) {
                    isc = true;
                }
                //所有请求结束 或 超时，返回结果
                var istimeout = now - startTime > ((timeout == 1 || !timeout) ? dto : timeout);
                if (ok.length + bad == len || istimeout) {
                    isc = true;
                }
                if (isc) {
                    clearInterval(si);
                    window[gk][cacheKey] = { date: now, ok: ok };
                    localStorage.setItem(gk, JSON.stringify(window[gk]));
                    callback(ok[0], ok, false);
                }
            }, 1)
        }
    }

    window.upstream = ups;

    return ups;
})(window);