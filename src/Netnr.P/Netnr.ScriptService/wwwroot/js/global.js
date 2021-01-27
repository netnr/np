/*
    upstream: From nginx upstream
    Source must support cross-domain
    v1.0.1
    by netnr
    2021-01-15
    https://github.com/netnr/upstream
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
    apiServer: "https://api.netnr.eu.org",
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
        var hosts = ["https://cors.eu.org/", "https://bird.ioliu.cn/v2?url=", "https://netnr-proxy.openode.io/"];
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

    /**
     * 大小可视化
     * @param {any} size
     */
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
    },

    /**
     * 接收文件
     * @param {any} fn 回调
     * @param {any} fileNode 选择文件的节点
     */
    receiveFiles: function (fn, fileNode) {

        //拖拽
        $(document).on("dragleave dragenter dragover", function (e) {
            if (e && e.stopPropagation) { e.stopPropagation() } else { window.event.cancelBubble = true }
            if (e && e.preventDefault) { e.preventDefault() } else { window.event.returnValue = false }
        }).on("drop", function (e) {
            if (e && e.preventDefault) { e.preventDefault() } else { window.event.returnValue = false }
            e = e || window.event;
            var files = (e.dataTransfer || e.originalEvent.dataTransfer).files;
            if (files && files.length) {
                fn(files, 'drag');
            }
        });

        //浏览
        $(fileNode).change(function () {
            var files = this.files;
            if (files.length) {
                fn(files, 'change');
            }
        });

        //粘贴
        document.addEventListener('paste', function (event) {
            if (event.clipboardData || event.originalEvent) {
                var clipboardData = (event.clipboardData || event.originalEvent.clipboardData);
                if (clipboardData.items) {
                    var items = clipboardData.items, len = items.length, files = [];
                    for (var i = 0; i < len; i++) {
                        var blob = items[i].getAsFile();
                        blob && files.push(blob);
                    }
                    if (files.length) {
                        fn(files, 'paste');
                    }
                }
            }
        })
    }
}

ss.init();