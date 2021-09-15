/*  *\
 *  Date：2018-01-16
 *  Author：netnr
 *  Version：1.0.1
\*  */

(function (window, undefined) {

    var jz = function (se, rg) { return new jz.fn.init(se, rg); };

    /* IE8- */
    jz.OldIE = function () { return typeof document.createElement == "object" || false };

    //字符串两边去空 ie8-
    if (jz.OldIE()) { String.prototype.trim = function () { return this.replace(/(^\s+)|(\s+$)/g, "") } };

    jz.fn = jz.prototype = {
        init: function (se, rg) {
            if (!se) {
                return this;
            }

            var len, elem, match;

            if (typeof se === "string") {
                len = se.length;
                //#ID
                if (se[0] === "#" && len > 1) {
                    elem = document.getElementById(se.substring(1));
                    if (elem) {
                        this[0] = elem;
                        this.length = 1;
                        return this;
                    } else {
                        this.length = 0;
                        return this;
                    }
                }
                //.CLASS
                if (se[0] === "." && len > 1) {
                    var cln = se.substring(1);
                    if (document.getElementsByClassName) {
                        elem = document.getElementsByClassName(cln);
                        jz.each(elem, function (k, v) {
                            this[k] = v;
                        });
                        this.length = elem.length;
                        return this;
                    } else {
                        var rgs = (rg || document).getElementsByTagName('*'), mcln = ' ' + cln.trim() + ' ', k = 0;
                        jz.each(rgs, function () {
                            if ((" " + this.className + " ").indexOf(mcln) >= 0) {
                                this[k++] = this;
                            }
                        });
                        this.length = k;
                        return this;
                    }

                }
                //DOMElement
            } else if (se.nodeType) {
                this[0] = se;
                this.length = 1;
                return this;
            }

            //window
            if (se == window) {
                this[0] = se;
                this.length = 1;
                return this;
            }

            //数组或伪数组
            if (se && se.length) {
                var i = 0, match = se || [];
                for (; i < match.length; i++) {
                    this[i] = match[i];
                }
                this.length = match.length;
                return this;
            }

            this[0] = se;
            this.length = 1;
            return this;
        },
        length: 0,
        //遍历
        each: function (callback) {
            jz.each(this, callback);
            return this;
        },
        //事件添加处理程序
        on: function (type, callback) {
            jz.each(this, function () {
                jz.on(type, callback, this);
            });
        },
        //事件移除处理程序
        off: function (type, callback) {
            jz.each(this, function () {
                jz.off(type, callback, this);
            });
        },
        //宽高、边距、滚动条间距、内容宽高
        px: function () {
            return jz.px(this[0]);
        },
        //父级节点
        parent: function () {
            var match = [];
            jz.each(this, function () {
                var m = jz.dir(this, "parentNode");
                m.length && match.push(m[0]);
            });
            return new jz.fn.init(match);
        },
        //移除
        remove: function () {
            jz.each(this, function () {
                var pt = jz(this).parent();
                if (pt.length && document.documentElement.contains(this)) {
                    pt[0].removeChild(this);
                }
            });
        },
        //显示
        show: function () {
            jz.each(this, function () {
                this.style["display"] = "block";
            });
            return this;
        },
        //影藏
        hide: function () {
            jz.each(this, function () {
                this.style["display"] = "none";
            });
            return this;
        },
        //添加样式
        addClass: function (className) {
            className = className.toString().trim();
            jz.each(this, function () {
                (" " + this.className + " ").indexOf(" " + className + " ") == -1 && (this.className += " " + className);
            });
            return this;
        }
    };

    jz.fn.init.prototype = jz.prototype;

    //遍历 object、array
    jz.each = function (object, callback) {
        var k, i = 0, len = object.length, isObj = len === undefined || typeof object == "function";
        if (isObj) {
            for (k in object) {
                if (callback.call(object[k], k, object[k]) === false) {
                    break;
                }
            }
        } else {
            for (; i < len;) {
                if (callback.call(object[i], i, object[i++]) === false) {
                    break;
                }
            }
        }
    };

    //事件添加处理程序
    jz.on = function (type, callback, obj) {
        if (obj.addEventListener) {
            obj.addEventListener(type, callback, false);
        } else if (obj.attachEvent) {
            obj.attachEvent("on" + type, callback["_eid"] = function () {
                callback.apply(obj, arguments)
            });
        } else {
            obj["on" + type] = callback
        }
    };

    //移除事件的处理程序
    jz.off = function (type, callback, obj) {
        if (obj.removeEventListener) {
            obj.removeEventListener(type, callback, false);
        } else if (obj.detachEvent) {
            obj.detachEvent("on" + type, callback["_eid"]);
        }
    };

    //添加处理事件
    jz.each(("blur focus focusin focusout load resize scroll unload click dblclick "
        + "mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave "
        + "change select submit keydown keypress keyup error contextmenu").split(" ")
        , function (i, name) {
            jz.fn[name] = function (callback) {
                jz.each(this, function () { jz.on(name, callback, this); });
                return this;
            }
        });

    //event
    jz.event = function (e) { return e || window.event };

    //target
    jz.target = function (e) { e = e || window; return e.target || e.srcElement; };

    //阻止事件冒泡
    jz.stopEvent = function (e) { if (e && e.stopPropagation) { e.stopPropagation() } else { window.event.cancelBubble = true } };

    //随机数字 长度（默认4位）1到15
    jz.random = function (len) { len = arguments.length ? len > 15 ? 15 : len : 4; return Math.random().toString().substr(2, len) };

    //输出<style>样式
    jz.writeStyle = function (css) {
        var s = document.createElement("style");
        s.type = "text/css";
        s.styleSheet ? s.styleSheet.cssText = css : s.innerHTML = css;
        document.getElementsByTagName("HEAD")[0].appendChild(s);
    };

    //检索一个节点某个方向的节点 dir可选值：parentNode nextSibling previousSibling
    jz.dir = function (t, dir) {
        var match = [], cur = t[dir];
        while (cur && cur.nodeType != 9) {
            cur.nodeType == 1 && match.push(cur);
            cur = cur[dir];
        }
        return match;
    }

    //宽高、边距、滚动条间距、内容宽高
    jz.px = function (element) {
        var result = {
            //宽
            width: null,
            //高
            height: null,
            //上边距
            top: null,
            //左边距
            left: null,
            //垂直滚动条上间距
            scrollTop: null,
            //水平滚动条左间距
            scrollLeft: null,
            //内容高度
            scrollHeight: null,
            //内容宽度
            scrollWidth: null
        }, docEle = document.documentElement, body = document.body;
        if (element === window || element === document) {
            result.width = docEle.clientWidth || body.clientWidth;
            result.height = docEle.clientHeight || body.clientHeight;
            result.scrollTop = docEle.scrollTop || body.scrollTop;
            result.scrollLeft = docEle.scrollLeft || body.scrollLeft;
            result.scrollWidth = docEle.scrollWidth || body.scrollWidth;
            result.scrollHeight = docEle.scrollHeight || body.scrollHeight;
        } else {
            result.width = element.offsetWidth;
            result.height = element.offsetHeight;
            var mg = element.getBoundingClientRect();
            result.top = mg.top;
            result.left = mg.left;
            result.scrollTop = element.scrollTop;
            result.scrollLeft = element.scrollLeft;
            result.scrollWidth = element.scrollWidth;
            result.scrollHeight = element.scrollHeight;
        }
        return result;
    }

    //输出样式
    jz.writeStyle('.jzart{position:absolute;color:#333;display:block;min-width:10px;font-size:14px;line-height:150%;box-sizing:border-box;background-color:#fff;border:1px solid #c3c3c3;box-shadow:0 2px 4px #c3c3c3;font-family:Microsoft YaHei}.jzart a,.jzart input{outline:0;resize:none}.jzart .jzart-header{height:40px;padding:0 15px;min-width:150px;line-height:40px;white-space:nowrap;background-color:#fdfafa;border-bottom:1px solid #ddd}.jzart .jzart-header .jzart-header-title{font-size:16px;margin-right:50px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis}.jzart .jzart-header .jzart-header-cloase{border:0;float:right;color:#b4b4b4;cursor:pointer;font-size:18px;line-height:40px;text-decoration:none;vertical-align:middle}.jzart .jzart-header .jzart-header-cloase:hover{color:#333;text-decoration:none}.jzart .jzart-body{overflow-y:auto;overflow-x:hidden;padding:20px 25px;word-break:break-all;word-wrap:break-word;box-sizing:border-box}.jzart .jzart-iframe{width:100%;border:0;height:400px;box-sizing:border-box}.jzart .jzart-footer{min-width:200px;text-align:right;box-sizing:border-box;padding:10px 20px 10px 30px}.jzart .art-ok{color:#fff;font-size:15px;cursor:pointer;line-height:1.5;margin-left:15px;text-align:center;border-radius:3px;white-space:nowrap;display:inline-block;padding:.3em .85em;text-decoration:none;background-color:#17a2b8;border:1px solid transparent}.jzart .art-ok:link,.jzart .art-ok:visited{color:#fff;background-color:#17a2b8}.jzart .art-ok:hover,.jzart .art-ok:active{color:#fff;text-decoration:none;background-color:#138496}.jzart .art-cancel{color:#333;font-size:15px;cursor:pointer;line-height:1.5;text-align:center;border-radius:3px;white-space:nowrap;display:inline-block;padding:.34em .88em;text-decoration:none;background-color:#fff;border:1px solid #ccc}.jzart .art-cancel:link,.jzart .art-cancel:visited{color:#333;text-decoration:none;background-color:#fff}.jzart .art-cancel:hover,.jzart .art-cancel:active{color:#333;text-decoration:none;border:1px solid #999;background-color:#f2f2f2}.jzart .tip-em{position:absolute;width:0;height:0;font-size:0;line-height:0;border-width:9px;border-style:dashed;border-color:transparent}.jzart .em-top1{left:15px;bottom:-18px;border-top-style:solid;border-top-color:#b4b4b4}.jzart .em-top2{left:15px;bottom:-16px;border-top-color:#fff;border-top-style:solid}.jzart .em-right1{top:15px;left:-18px;border-right-style:solid;border-right-color:#b4b4b4}.jzart .em-right2{top:15px;left:-16px;border-right-color:#fff;border-right-style:solid}.jzart .em-left1{top:15px;right:-18px;border-left-style:solid;border-left-color:#b4b4b4}.jzart .em-left2{top:15px;right:-16px;border-left-color:#fff;border-left-style:solid}.jzart .em-bottom1{top:-18px;left:15px;border-bottom-style:solid;border-bottom-color:#b4b4b4}.jzart .em-bottom2{top:-16px;left:15px;border-bottom-color:#fff;border-bottom-style:solid}.jzart-mask{top:0;left:0;right:0;bottom:0;opacity:.3;position:fixed;background-color:black;filter:alpha(opacity=30)}');

    //拖动 对象、可拖动区域（默认对象）、仅可视拖动
    jz.drag = function (t, dragT, v) {
        var ele = jz(t)[0], de = dragT != undefined ? jz(dragT)[0] : ele;
        de.style.cursor = "move";
        de.onmousedown = function (e) {
            var eL = ele.offsetLeft, eT = ele.offsetTop,
                eW = ele.offsetWidth, eH = ele.offsetHeight,
                disX = jz.event(e).clientX - eL,
                disY = jz.event(e).clientY - eT,
                dw = jz.px(document).width,
                dh = jz.px(document).height,
                minY = jz.px(document).scrollTop,
                minX = jz.px(document).scrollLeft;

            document.onmousemove = function (e) {
                var x = jz.event(e).clientX - disX,
                    y = jz.event(e).clientY - disY;
                if (v) {
                    var maxX = dw - eW, maxY = dh - eH;
                    if (ele.style.position == "fixed") {
                        minY = 0;
                        minX = 0;
                    } else {
                        maxX += minX;
                        maxY += minY;
                    }
                    x <= minX && (x = minX);
                    y <= minY && (y = minY);
                    x >= maxX && (x = maxX);
                    y >= maxY && (y = maxY);
                }

                ele.style.left = x + "px";
                ele.style.top = y + "px";

                return false
            };

            document.onmouseup = function () {
                document.onmousemove = null;
                document.onmouseup = null;
                this.releaseCapture && this.releaseCapture()
            };
            this.setCapture && this.setCapture();
            return false;
        }
    };

    //点击对象的空白地方 事件
    jz.blankClick = function (t, callback) {
        setTimeout(function () {
            jz(t).click(function (event) { jz.stopEvent(event); });
            jz(document).click(function () {
                var elem = t.nodeType ? t : document.getElementById(t);
                if (elem) { try { elem.parentElement.removeChild(elem); } catch (e) { } }
                jz(document).off("click", arguments.callee);
                typeof callback == "function" && callback();
            })
        }, 200)
    };

    //优先取值模式
    jz.firstV = function (v, dv) {
        return v == undefined ? dv : v;
    };

    //弹窗基础仓库
    jz.art = {
        //版本
        version: "1.0.1",
        //叠堆起始值
        zindex: 6666,
        //配置
        config: {
            //默认4秒关闭
            autoclosetime: 4,
            //设置语言
            setlang: "zh_CN",
            //右上角关闭按钮文字
            closeValue: "✖",
            //小窗口默认宽度，如：alert、confirm
            smallwidth: "340px"
        },
        //语言包
        locale: {
            zh_CN: {
                "title": "消息",
                "closeTitle": "关闭",
                "iframeTitle": "窗口",
                "okValue": "确定",
                "cancelValue": "取消"
            },
            en: {
                "title": "Message",
                "closeTitle": "Close",
                "iframeTitle": "Window",
                "okValue": "Ok",
                "cancelValue": "Cancel"
            }
        },
        //弹窗参数初始化
        initobj: function (obj) {
            obj.title = obj.title == false ? false : jz.firstV(obj.title, jz.art.getlangkey("title"));
            obj.width = jz.firstV(obj.width, jz.art.config.smallwidth);
            obj.single = jz.firstV(obj.single, false);
            obj.align = jz.firstV(obj.align, 5);
            obj.time = jz.firstV(obj.time, 0);
            obj.drag = jz.firstV(obj.drag, true);
            obj.mask = jz.firstV(obj.mask, true);
            obj.fixed = jz.firstV(obj.fixed, true);
            return obj;
        },
        //获取语言
        getlangkey: function (key) {
            return jz.art.locale[jz.art.config.setlang][key];
        },
        //创建
        create: function (e, c) {
            var o = document.createElement(e);
            c != undefined && c != "" && (o.className = c);
            return o;
        },
        //弹出层包
        wrap: function (obj) {
            var art = this.create('div', 'jzart');
            art.id = "jzart" + jz.art.zindex;
            if (obj.fixed) { art.style.position = "fixed"; }
            return art;
        },
        //头部
        header: function () {
            var header = this.create('div', 'jzart-header');
            header.id = "jzartheader" + jz.art.zindex;
            return header;
        },
        //头部 - 标题
        title: function (obj) {
            var title = this.create('div', 'jzart-header-title');
            title.id = "jzartheadertitle" + jz.art.zindex;
            title.innerHTML = jz.firstV(obj.title, jz.art.getlangkey("title"));
            return title;
        },
        //头部 - 关闭
        close: function () {
            var close = this.create('a', 'jzart-header-cloase');
            close.href = "javascript:void(0);";
            close.id = "jzarttopclose" + jz.art.zindex;
            close.innerHTML = jz.art.config.closeValue;
            close.title = jz.art.getlangkey("closeTitle");
            return close;
        },
        //底部按钮包
        footer: function () {
            var footer = this.create('div', 'jzart-footer');
            footer.id = "jzartfooter" + jz.art.zindex;
            return footer;
        },
        //底部 - 确定按钮
        ok: function (obj) {
            var ok = this.create('a', 'art-ok');
            ok.href = "javascript:void(0);";
            ok.id = "jzartok" + jz.art.zindex;
            ok.innerHTML = jz.firstV(obj.okValue, jz.art.getlangkey("okValue"));
            return ok;
        },
        //底部 - 取消按钮
        cancel: function (obj) {
            var cancel = this.create('a', 'art-cancel');
            cancel.href = "javascript:void(0);";
            cancel.id = "jzartcancel" + jz.art.zindex;
            cancel.innerHTML = jz.firstV(obj.cancelValue, jz.art.getlangkey("cancelValue"));
            return cancel;
        },
        //主体
        body: function (obj) {
            var body = this.create('div', 'jzart-body');
            body.id = "jzartbody" + jz.art.zindex;
            body.innerHTML = jz.firstV(obj.content, "");
            return body;
        },
        //弹窗页面
        iframe: function (obj) {
            var iframe = this.create('iframe', 'jzart-iframe');
            iframe.id = iframe.name = "jzartiframe" + jz.art.zindex;
            iframe.src = obj.src;
            iframe.frameBorder = "0";
            iframe.scrolling = obj.scrolling ? "auto" : "no";
            return iframe;
        },
        //补充单位
        supunit: function (n) {
            n = n.toString();
            if (n.indexOf("px") == -1 && n.indexOf("em") == -1) {
                n += "px";
            }
            return n;
        },
        //对齐方式
        align: function (msg, obj) {
            obj = obj || msg.fn.obj;

            var x = 0, y = 0, nf = msg.style.position != "fixed",
                ch = jz.px(window).height, cw = jz.px(window).width,
                oh = msg.offsetHeight, ow = msg.offsetWidth,
                st = jz.px(window).scrollTop, sl = jz.px(window).scrollLeft;

            switch (obj.align) {
                case 1:
                    x = ch - oh;
                    break;
                case 2:
                    x = ch - oh;
                    y = (cw / 2) - (ow / 2);
                    break;
                case 3:
                    x = ch - oh;
                    y = cw - ow;
                    break;
                case 4:
                    x = (ch / 2) - (oh / 2);
                    break;
                case 5:
                default:
                    x = (ch / 2) - (oh / 2);
                    y = (cw / 2) - (ow / 2);
                    break;
                case 6:
                    x = (ch / 2) - (oh / 2);
                    y = cw - ow;
                    break;
                case 7:
                    break;
                case 8:
                    y = (cw / 2) - (ow / 2);
                    break;
                case 9:
                    y = cw - ow;
                    break;
            }
            if (nf) {
                x += st;
                y += sl;
            }
            msg.style.top = x + "px";
            msg.style.left = y + "px";
        },
        //tip对齐方式
        tipalign: function (tip, obj, tar) {
            obj = obj || tip.fn.obj;
            tar = tar || tip.fn.target;

            var oH = tar.offsetHeight, oW = tar.offsetWidth,
                x = jz.px(tar).left + jz.px(window).scrollLeft,
                y = jz.px(tar).top + jz.px(window).scrollTop,
                rx, ry;
            switch (obj.align) {
                case "top":
                    rx = y - tip.offsetHeight - 13;
                    ry = (oW > 40 ? x : x - 25 + (oW / 2));
                    break;
                case "right":
                    rx = oH > 40 ? y : y - 25 + (oH / 2);
                    ry = x + oW + 13;
                    break;
                case "left":
                    rx = oH > 40 ? y : y - 25 + (oH / 2);
                    ry = x - tip.offsetWidth - 13;
                    break;
                default: //默认下方
                    rx = y + oH + 13;
                    ry = oW > 40 ? x : x - 25 + (oW / 2);
                    break;  //对象小于40 三角符号指向对象中间
            }
            tip.style.top = rx + "px";
            tip.style.left = ry + "px";
        },
        //大小
        setsize: function (msg, obj) {
            obj = obj || msg.fn.obj;
            var ps = false, iw = false, ih = false;
            if (obj.width != undefined) {
                iw = obj.width;
                if (iw.indexOf('%') >= 0) {
                    ps = true;
                    iw = jz.px(window).width * parseFloat(iw.replace('%', '')) / 100;
                }
            }
            if (obj.height != undefined) {
                ih = obj.height;
                if (ih.indexOf('%') >= 0) {
                    ps = true;
                    ih = jz.px(window).height * parseFloat(ih.replace('%', '')) / 100;
                }
            }
            if (iw) {
                msg.style.width = jz.art.supunit(iw);
            }
            if (ih) {
                msg.style.height = jz.art.supunit(ih);
                var bh = msg.clientHeight;
                if (msg.fn.header) {
                    bh -= msg.fn.header.offsetHeight;
                }
                if (msg.fn.iframe) {
                    msg.fn.iframe.style["height"] = bh + "px";
                } else {
                    if (msg.fn.footer) {
                        bh = bh - msg.fn.footer.offsetHeight;
                    }
                    msg.fn.body.style["height"] = bh + "px";
                }
            }
            return ps;
        },
        //遮罩层
        mask: function () {
            var mask = this.create('div', 'jzart-mask');
            return mask;
        },
        //移除
        remove: function (id, time) {
            time = typeof time == "number" ? time : 0;
            setTimeout(function () {
                if (id != null && id != "") {
                    var rt = jz(id = "#" + id);
                    rt.remove();
                    jz(id = id + "mask").remove();
                    jz.art.removeCallBack(rt[0]);
                }
            }, time * 1000)
        },
        //移除回调
        removeCallBack: function (art) {
            if (art && typeof art.fn.obj.remove == "function") {
                art.fn.obj.remove();
            }
            art = null;
        },
        //重载
        pol: function (content, obj) {
            if (obj) {
                obj.content = content;
            } else {
                if (typeof content == "object") {
                    obj = content;
                } else {
                    obj = {};
                    obj.content = content;
                }
            }
            return obj;
        },
        //弹窗拓展，参数：弹窗、弹窗内包含对象，配置参数
        fn: function (art, sub, obj) {
            sub.global = art;
            sub.obj = obj;
            //隐藏
            sub.hide = function () {
                jz(art).hide();
                sub.mask && jz(sub.mask).hide();
            };
            //显示
            sub.show = function () {
                jz(art).show();
                sub.mask && jz(sub.mask).show();
            }
            //删除
            sub.remove = function () {
                jz.art.remove(art.id);
            }

            art["fn"] = sub;
        },
        msgId: "",
        tipId: ""
    };


    /*  jz弹窗

     *  参数列表：
    
     *  title：标题    text/html/false     false不显示标题
     *  content：文本信息    text/html（jz.iframe不传）
     *  footer：底部按钮包    bool    false不显示
     *  time：倒计时关闭 单位：秒     number（jz.msg默认4秒）
     *  blank：点击空白关闭    bool（默认false）     
     *  mask：遮罩层    true、false、0-1不透明度
     *  fixed：绝对位置  bool（position:fixed）    即启用不受滚动条影响
     *  align: 对齐方式 数字键盘1-9，默认5水平垂直，false、0忽略对齐（仅初始化居中）
     *  single：只弹出一个    bool
     *  drag：拖动     bool（jz.msg默认false）     
     *  ok：确定回调     function/false（确定回调/不显示确定按钮）
     *  okValue：确定按钮文本
     *  cancel：取消回调  function/false（取消回调/不显示取消按钮）     
     *  cancelValue：取消按钮文本
     *  close：窗口关闭回调    function/bool（关闭回调/不显示，有标题时）
     *  remove：移除回调  function
     *  src：弹窗地址    text（jz.iframe用）
     *  width：弹窗宽度 如"400px"、"40em"、"90%"，百分比或固定的单位值
     *  height: 弹窗高度 如"400px"、"40em"、"90%"，百分比或固定的单位值
     *  scrolling: iframe是否有滚动条    bool（默认false）

     *  popup为基方法，msg、alert、confirm、iframe都基于popup提供支持，popup返回整个弹窗对象
     *  popup.fn为拓展，涵盖对象、方法

     */
    jz.popup = function (obj) {
        jz.art.zindex += 1;
        //外包、ID、子对象
        var msg = jz.art.wrap(obj), id = msg.id,
            msub = {
                mask: undefined,
                header: undefined,
                title: undefined,
                close: undefined,
                body: undefined,
                iframe: undefined,
                footer: undefined,
                ok: undefined,
                cancel: undefined,
                zindex: jz.art.zindex
            };
        //弹窗堆叠顺序
        msg.style.zIndex = jz.art.zindex;
        //活动窗口 顶部显示
        jz(msg).mousedown(function () { this.style.zIndex = jz.art.zindex += 1; });
        //头部 有标题的前提
        if (obj.title != false) {
            //头部包、标题
            msub.header = jz.art.header(), msub.title = jz.art.title(obj);
            //显示关闭按钮
            if (obj.close != false) {
                msub.close = jz.art.close();
                msub.header.appendChild(msub.close);
                //关闭回调
                jz(msub.close).click(function () {
                    if ((obj.close == undefined || obj.close() != false)) {
                        jz.art.remove(id);
                    }
                });
            }
            //载入头部
            msub.header.appendChild(msub.title);
            msg.appendChild(msub.header);
        }
        //载入内容 没传入 src的前提
        if (obj.content != undefined && obj.src == undefined) {
            msub.body = jz.art.body(obj);
            msg.appendChild(msub.body);
        }
        //载入iframe
        if (obj.src != undefined) {
            msub.iframe = jz.art.iframe(obj);
            msg.appendChild(msub.iframe);
        }
        //底部按钮包
        if (obj.title != false && obj.src == undefined && obj.footer !== false) {
            msub.footer = jz.art.footer();
            //显示取消按钮
            if (obj.cancel != false) {
                //取消按钮
                msub.cancel = jz.art.cancel(obj);
                msub.footer.appendChild(msub.cancel);
                //取消回调
                jz(msub.cancel).click(function () {
                    if ((obj.cancel == undefined || obj.cancel() != false)) {
                        jz.art.remove(id);
                    }
                })
            }
            //确定按钮
            if (obj.ok != false) {
                msub.ok = jz.art.ok(obj);
                msub.footer.appendChild(msub.ok);
                //确定回调
                jz(msub.ok).click(function () {
                    if ((obj.ok == undefined || obj.ok() != false)) {
                        jz.art.remove(id);
                    }
                });
            }
            //载入底部按钮包
            msg.appendChild(msub.footer);
        }
        //显示遮罩层
        if (obj.mask != undefined && obj.mask != false) {
            msub.mask = jz.art.mask();
            msub.mask.id = id + "mask";
            msub.mask.style.zIndex = jz.art.zindex - 1;

            //不透明度
            var opacity = parseFloat(obj.mask);
            isNaN(opacity) && (opacity = .3);
            msub.mask.style.opacity = opacity;
            msub.mask.style["filter"] = "alpha(opacity = " + (opacity * 100) + ")";
            document.body.appendChild(msub.mask);
        }

        //msg拓展
        jz.art.fn(msg, msub, obj);

        //弹出一个
        if (obj.single != false) { jz.art.remove(jz.art.msgId); jz.art.msgId = id; }
        //显示弹窗
        document.body.appendChild(msg);
        //默认4秒关闭
        if (obj.time == undefined) { jz.art.remove(id, jz.art.config.autoclosetime); }
        else { if (obj.time != 0) { jz.art.remove(id, obj.time); } }
        //点击空白关闭
        if (obj.blank) {
            jz.blankClick(msg, function () {
                jz(msub.mask).remove();
                jz.art.removeCallBack(msg);
            })
        }
        //拖动
        if (obj.drag) { if (obj.title != undefined) { jz.drag(msg, msub.header, 1); } else { jz.drag(msg, null, 1) } }
        //初始化大小
        if (jz.art.setsize(msg, obj)) {
            //百分比大小
            jz(window).resize(function () { jz.art.setsize(msg) })
        }
        //初始化对齐方式
        jz.art.align(msg, obj);
        //对齐方式
        jz(window).resize(function () { jz.art.align(msg) });
        return msg;
    };

    /*  msg  */
    jz.msg = function (content, obj) {
        obj = jz.art.pol(content, obj);
        obj.title = jz.firstV(obj.title, false);
        return jz.popup(obj);
    };

    /*  alert  */
    jz.alert = function (content, obj) {
        obj = jz.art.pol(content, obj);
        obj.cancel = jz.firstV(obj.cancel, false);
        return jz.popup(jz.art.initobj(obj));
    };

    /*  confirm  */
    jz.confirm = function (content, obj) {
        obj = jz.art.pol(content, obj);
        return jz.popup(jz.art.initobj(obj));
    };

    /*  关闭iframe  */
    jz.closeback = function (name) {
        var ac = document.getElementById(name.replace('iframe', 'topclose'));
        if (ac) {
            ac.click();
        } else if (ac = document.getElementById(name = name.replace('iframe', ''))) {
            jz.art.remove(name);
        }
    };

    /*  iframe  */
    jz.iframe = function (obj) {
        obj.title = obj.title == false ? false : jz.firstV(obj.title, jz.art.getlangkey("iframeTitle"));
        return jz.popup(jz.art.initobj(obj));
    };


    /*  小提示 tip  参数列表：
     *  target：id/object（提示目标id或对象）
     *  content：提示信息    text/html
     *  single：只弹出一个    bool
     *  time：倒计时关闭 单位：秒     number（默认不关闭）
     *  blank：点击空白关闭    bool（默认false）
     *  focus：焦点选中目标    bool（默认false）
     */
    jz.tip = function (obj) {
        //定位目标、提示包
        var tar = jz(obj.target)[0], tip = jz.art.wrap(obj), id = tip.id,
            tsub = {
                target: tar,
                body: undefined,
                zindex: jz.art.zindex += 1
            };
        //弹窗堆叠顺序
        tip.style.zIndex = jz.art.zindex;
        //内容
        tsub.body = jz.art.body(obj),
            sj1 = jz.art.create('em'),//三角符号
            sj2 = jz.art.create('em'),
            align = "bottom";//方向
        jz.each("top right left bottom".split(" "), function () {
            if (obj.align == this) { align = this; return false; }
        });

        tip.appendChild(tsub.body);
        tip.appendChild(sj1);
        tip.appendChild(sj2);
        sj1.className = 'tip-em em-' + align + '1';
        sj2.className = 'tip-em em-' + align + '2';

        //活动窗口 顶部显示
        jz(tip).mousedown(function () { this.style.zIndex = jz.art.zindex += 1; });
        //弹出一个
        if (obj.single != false) { jz.art.remove(jz.art.tipId); jz.art.tipId = id; }

        //拓展
        jz.art.fn(tip, tsub, obj);

        //载入提示
        document.body.appendChild(tip);

        //倒计时关闭
        if (obj.time != undefined && obj.time != 0) { jz.art.remove(id, obj.time); }
        //点击空白关闭
        if (obj.blank) { jz.blankClick(tip, function () { jz.art.removeCallBack(tip) }) }
        //选中焦点
        if (obj.focus) { tar.focus() }

        //调整定位
        jz.art.tipalign(tip);
        jz(window).resize(function () { jz.art.tipalign(tip) });

        return tip;
    };

    window.j = jz;

})(window, undefined)