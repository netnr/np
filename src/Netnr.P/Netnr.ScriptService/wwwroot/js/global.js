/*
 * netnrnav v1.1.0
 * 
 * https://github.com/netnr/nav
 * 
 * Fork: https://github.com/zhoufengjob/SuiNav
 * 
 * Date: 2019-08-18
 * 
 */

(function (window) {

    var netnrnav = function (se) { return new netnrnav.fn.init(se); };

    netnrnav.fn = netnrnav.prototype = {
        init: function (se) {
            var that = this;
            this.ele = $(se);
            this.eventCount = 0;
            this.isHiding = false;

            if (that.ele.hasClass('horizontal')) {
                that.ele.find('li').hover(function () {
                    $(this).children('ul').show();
                }, function () {
                    $(this).children('ul').hide();
                });
            } else {
                that.ele.find('li').click(function () {
                    if (that.eventCount != 0) {
                        if ($(this).parent().parent().parent().hasClass('netnrnav')) {
                            that.eventCount = 0;
                        }
                        return;
                    }
                    if ($(this).children('ul').is(":hidden"))
                        $(this).children('ul').show();
                    else {
                        $(this).find('ul').hide();
                    }
                    that.eventCount++;
                    if ($(this).parent().parent().parent().hasClass('netnrnav')) {
                        that.eventCount = 0;
                    }
                });
            }
            return this;
        }
    };

    netnrnav.show = function (that) {
        if (!that.isHiding) {
            $(document.body).append('<div class="netnrnav slide-netnrnav"></div><div class="netnrnav netnrnav-mask"></div>');
            $('.slide-netnrnav').html(that.ele.html()).find('li').click(function () {
                if (that.eventCount != 0) {
                    if ($(this).parent().parent().parent().hasClass('netnrnav')) {
                        that.eventCount = 0;
                    }
                    return;
                }
                if ($(this).children('ul').is(":hidden"))
                    $(this).children('ul').show();
                else {
                    $(this).find('ul').hide();
                }
                that.eventCount++;
                if ($(this).parent().parent().parent().hasClass('netnrnav')) {
                    that.eventCount = 0;
                }
            });
            $('.netnrnav-mask').click(function () {
                netnrnav.hide(that);
            });
            setTimeout(function () {
                $('.slide-netnrnav').toggleClass('active');
                $('.netnrnav-mask').toggleClass('active');
            }, 20);
        }
    };

    netnrnav.hide = function (that) {
        if (!that.isHiding) {
            that.isHiding = true;
            $('.slide-netnrnav').find('li').unbind();
            $('.slide-netnrnav').removeClass('active');
            $('.netnrnav-mask').removeClass('active');
            setTimeout(function () {
                $('.slide-netnrnav').remove();
                $('.netnrnav-mask').remove();
                that.isHiding = false;
            }, 600);
        }
    };

    netnrnav.toggle = function (that) {
        $('.slide-netnrnav').length > 0 ? netnrnav.hide(that) : netnrnav.show(that)
    }

    netnrnav.fn.init.prototype = netnrnav.fn;

    window.netnrnav = netnrnav;

})(window);

if (!Array.prototype.forEach) {
    Array.prototype.forEach = function forEach(callback, thisArg) {
        var T, k;
        if (this == null) {
            throw new TypeError("this is null or not defined");
        }
        var O = Object(this);
        var len = O.length >>> 0;
        if (typeof callback !== "function") {
            throw new TypeError(callback + " is not a function");
        }
        if (arguments.length > 1) {
            T = thisArg;
        }
        k = 0;
        while (k < len) {
            var kValue;
            if (k in O) {
                kValue = O[k];
                callback.call(T, kValue, k, O);
            }
            k++;
        }
    };
}

$(function () {
    $.nrnav = netnrnav(".netnrnav");
    $('.MenuToggle').click(function () {
        netnrnav.toggle($.nrnav);
    });
});


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



/* ScriptServices */
var ss = {
    ajax: function (obj) {
        var hosts = ["https://cors.zme.ink/", "https://bird.ioliu.cn/v2?url="];
        upstream(hosts, function (fast, ok, bad) {
            obj.url = fast + obj.url;
            $.ajax(obj);
        }, 1);
    },
    datalocation: function (data) {
        return data || {};
        loading(0);
    },
    csqm: function (q) {
        return q.replace(/\'/g, "\\'");
    },
    bmob: {
        init: function () {
            Bmob && Bmob.initialize("59a522843b951532546934352166df80", "97fcbeae1457621def948aba1db01821");
        }
    }
}

function loading(close) {
    if (close === 0 || close === false) {
        if (window.loading) {
            clearTimeout(window.loadingdefer);
            window.loadingdom.hide();
        }
    } else {
        if (!window.loadingdom) {
            window.loadingdom = $('<div class="loading"></div>').appendTo(document.body);
        }
        window.loadingdom.hide();
        window.loadingdefer = setTimeout(function () {
            window.loadingdom.show();
        }, 1000);
    }
}

function totop() {
    $('html,body').animate({ scrollTop: 0 }, 400)
};

function htmlEncode(html) {
    return document.createElement('a').appendChild(document.createTextNode(html)).parentNode.innerHTML;
};

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