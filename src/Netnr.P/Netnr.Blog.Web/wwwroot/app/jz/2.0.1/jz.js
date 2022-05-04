/*!
 * Jz JavaScript Popup
 * 
 * Date: 2018-06-29
 * Author: netnr
 */

(function (window, undefined) {

    var jz = function (se, rg) { return new jz.fn.init(se, rg); };

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
        //隐藏
        hide: function () {
            jz.each(this, function () {
                this.style["display"] = "none";
            });
            return this;
        }
    };

    jz.fn.init.prototype = jz.fn;

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

    //阻止事件冒泡
    jz.stopEvent = function (e) { if (e && e.stopPropagation) { e.stopPropagation() } else { window.event.cancelBubble = true } };

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
        version: "2.0.0",
        //叠堆起始值
        zindex: 6666,
        //配置
        config: {
            //默认自动关闭 单位（秒）
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
                title: "消息",
                closeTitle: "关闭",
                iframeTitle: "窗口",
                okValue: "确定",
                cancelValue: "取消"
            },
            en: {
                title: "MESSAGE",
                closeTitle: "CLOSE",
                iframeTitle: "WINDOW",
                okValue: "OK",
                cancelValue: "CANCEL"
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
            c != undefined && (o.className = c);
            return o;
        },
        //弹出层包
        wrap: function (obj) {
            var art = this.create('div', 'jzart');
            art.id = "jzart" + jz.art.zindex;
            obj.fixed && (art.style.position = "fixed");
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
            var ok = this.create('a', 'art-btn art-ok');
            ok.href = "javascript:void(0);";
            ok.id = "jzartok" + jz.art.zindex;
            ok.innerHTML = jz.firstV(obj.okValue, jz.art.getlangkey("okValue"));
            return ok;
        },
        //底部 - 取消按钮
        cancel: function (obj) {
            var cancel = this.create('a', 'art-btn art-cancel');
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

            //目标对位
            if (msg.fn.target) {
                var tar = msg.fn.target;

                var oH = tar.offsetHeight, oW = tar.offsetWidth,
                    x = jz.px(tar).left + jz.px(window).scrollLeft,
                    y = jz.px(tar).top + jz.px(window).scrollTop,
                    rx, ry;
                switch (obj.align) {
                    case "top":
                        ry = y - msg.offsetHeight - 16;
                        rx = (oW > 40 ? x : x - 15 + (oW / 2));
                        break;
                    case "right":
                        ry = oH > 40 ? y : y - 15 + (oH / 2);
                        rx = x + oW + 16;
                        break;
                    case "left":
                        ry = oH > 40 ? y : y - 15 + (oH / 2);
                        rx = x - msg.offsetWidth - 16;
                        break;
                    default: //默认下方
                        ry = y + oH + 16;
                        rx = oW > 40 ? x : x - 15 + (oW / 2);
                        break;  //对象小于40 三角符号指向对象中间
                }

                msg.style.top = ry + "px";
                msg.style.left = rx + "px";
            } else {
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
            }
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
        }
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
     *  focus：焦点选中目标    bool（默认false）
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
                target: undefined,
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

        //头部 有标题
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
            isNaN(opacity) && (opacity = .01);
            msub.mask.style.opacity = opacity;
            msub.mask.style["filter"] = "alpha(opacity = " + (opacity * 100) + ")";
            document.body.appendChild(msub.mask);
        }

        //目标定位
        if (obj.target) {
            msub.target = jz(obj.target)[0];
            var sj = jz.art.create('em');

            if (!obj.align || obj.align == "") {
                obj.align = "bottom";
            }
            sj.className = 'tip-em em-' + obj.align;
            msg.appendChild(sj);
        }

        //不同类型针对处理
        switch (obj.type) {
            case "msg":
                {
                    msg.className += ' jzart-msg';
                }
                break;
        }

        //msg拓展
        jz.art.fn(msg, msub, obj);

        //显示弹窗
        document.body.appendChild(msg);

        //焦点选中
        if (obj.focus) { msg.focus() }

        //自动关闭
        if (obj.time > 0) { jz.art.remove(id, obj.time); }

        //点击空白关闭
        if (obj.blank) {
            jz.blankClick(msg, function () {
                jz(msub.mask).remove();
                jz.art.removeCallBack(msg);
            })
        }

        //拖动
        if (obj.drag) {
            if (obj.title != undefined) {
                jz.drag(msg, msub.header, 1);
            } else {
                jz.drag(msg, null, 1)
            }
        }

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

    /*  msg tip */
    jz.msg = jz.tip = function (content, obj) {
        obj = jz.art.pol(content, obj);
        obj.title = jz.firstV(obj.title, false);
        obj.time = jz.firstV(obj.time, jz.art.config.autoclosetime);
        obj.type = 'msg';
        var jp = jz.popup(obj);
        return jp;
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

    /*  iframe  */
    jz.iframe = function (obj) {
        obj.title = obj.title == false ? false : jz.firstV(obj.title, jz.art.getlangkey("iframeTitle"));
        return jz.popup(jz.art.initobj(obj));
    };

    /*  iframe close  */
    jz.closeback = function (name) {
        var ac = document.getElementById(name.replace('iframe', 'topclose'));
        if (ac) {
            ac.click();
        } else if (ac = document.getElementById(name = name.replace('iframe', ''))) {
            jz.art.remove(name);
        }
    };

    //载入样式
    jz.getStyle = function (href) {
        var ele = document.createElement("LINK");
        ele.href = href;
        ele.rel = "stylesheet";
        document.getElementsByTagName("HEAD")[0].appendChild(ele);
    };

    //自身路径
    jz.selfSrc = document.scripts[document.scripts.length - 1].src;
    jz.selfPath = jz.selfSrc.substring(0, jz.selfSrc.lastIndexOf('/') + 1);
    //载入样式
    jz.getStyle(jz.selfPath + "jz.css");

    window.jz = jz;

})(window, undefined);

