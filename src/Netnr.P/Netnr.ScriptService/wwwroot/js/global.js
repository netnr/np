/*
    upstream: From nginx upstream
    - Source must support cross-domain
    v1.0.0
    2019-04-02
    https://github.com/netnr/upstream
 */
(function (window) {
    var ups = function (hosts, callback, timeout) {
        window.upstreamCache = window.upstreamCache || {};
        //10秒内缓存
        var startTime = new Date().valueOf(), cacheKey = hosts.join(','),
            hostsCache = window.upstreamCache[cacheKey];
        if (hostsCache && startTime - hostsCache.date < 10000) {
            callback(hostsCache.ok[0], hostsCache.ok);
        } else {
            var ok = [], bad = 0, i = 0, len = hosts.length;
            for (; i < len;) {
                var host = hosts[i++];
                //自动补齐链接
                host = host.trim().toLowerCase().indexOf("//") >= 0 ? host : "//" + host;
                //发起fetch，添加成功的url（该url与hosts可能不一样），须支持跨域请求
                fetch(host).then(function (res) {
                    res.ok ? ok.push(res.url) : bad++;
                }).catch(function () {
                    bad++
                })
            }
            var si = setInterval(function () {
                var isc = false, now = new Date().valueOf();
                //当timeout为1，返回最快可用的host
                if (timeout == 1 && ok.length > 0) {
                    isc = true;
                }
                //所有请求结束 或 超时（默认3000毫秒），返回结果
                var istimeout = now - startTime > ((timeout == 1 || !timeout) ? 3000 : timeout);
                if (ok.length + bad == len || istimeout) {
                    isc = true;
                }
                if (isc) {
                    clearInterval(si);
                    window.upstreamCache[cacheKey] = { date: now, ok: ok };
                    callback(ok[0], ok);
                }
            }, 1)
        }
    }

    window.upstream = ups;

    return ups;
})(window);


/* ScriptService */
var ss = {
    init: function () {
        ss.lsInit();

        $(function () {
            $('#LoadingMask').fadeOut();

            //Monaco Editor 编辑器全屏切换
            $('.me-full-btn').click(function () {
                var mebox = $(this).parent();
                if (mebox.hasClass('me-full')) {
                    mebox.removeClass('me-full')
                    mebox.addClass('position-relative')
                } else {
                    mebox.addClass('me-full')
                    mebox.removeClass('position-relative')
                }
            })
        })
    },

    bmobInit: function () {
        //比目初始化
        window.Bmob && Bmob.initialize("59a522843b951532546934352166df80", "97fcbeae1457621def948aba1db01821");
    },

    /**
     * 代理请求
     * @param {any} obj 请求参数
     * @param {any} hi 指定代理
     */
    ajax: function (obj, hi) {
        var hosts = ["https://netnr-proxy.openode.io/", "https://bird.ioliu.cn/v2?url="];
        if (hi != null) {
            obj.url = hosts[hi] + encodeURIComponent(obj.url);
            $.ajax(obj)
        } else {
            upstream(hosts, function (fast) {
                obj.url = fast + encodeURIComponent(obj.url);
                $.ajax(obj);
            }, 1);
        }
    },

    /**
     * 代理数据处理
     * @param {any} data
     */
    datalocation: function (data) {
        return data || {};
        ss.loading(0);
    },

    /**
     * html 编码
     * @param {any} html
     */
    htmlEncode: function (html) {
        return document.createElement('a').appendChild(document.createTextNode(html)).parentNode.innerHTML;
    },

    /**
     * 回到顶部
     */
    toTop: function () {
        $('html,body').animate({ scrollTop: 0 }, 400)
    },

    /**
     * 加载
     * @param {any} close
     */
    loading: function (close) {
        if (close === 0 || close === false) {
            clearTimeout(window.loadingdefer);
            window.loadingdom.hide();
        } else {
            if (!window.loadingdom) {
                window.loadingdom = $('<div class="loading"></div>').appendTo(document.body);
            }
            window.loadingdom.hide();
            window.loadingdefer = setTimeout(function () {
                window.loadingdom.show();
            }, 1000);
        }
    },

    /* localStorage */
    lsKey: location.pathname,
    ls: {},
    lsInit: function () {
        var lsv = localStorage.getItem(this.lsKey);
        if (lsv && (lsv = JSON.parse(lsv))) {
            this.ls = lsv;
        }
    },
    lsArr: function (key) {
        return this.ls[key] = this.ls[key] || [];
    },
    lsObj: function (key) {
        return this.ls[key] = this.ls[key] || {};
    },
    lsStr: function (key) {
        return this.ls[key] = this.ls[key] || "";
    },
    lsSave: function () {
        localStorage.setItem(this.lsKey, JSON.stringify(this.ls));
    },

    sizeOf: function (size) {
        var u = 'B', s = size;
        if (size >= 1024) {
            u = 'K';
            s = size / 1024.0
        }
        if (size >= 1024 * 1024) {
            u = 'M'
            s = size / 1024.0 / 1024;
        }
        if (size >= 1024 * 1024 * 1024) {
            u = 'G'
            s = size / 1024.0 / 1024 / 1024;
        }
        return s.toFixed(1) + u;
    }
}

ss.init();