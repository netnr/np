/*
 *  Date：2017-07-06
 *  Author：netnr
 *  Version：1.0.0
\* */

(function (window, undefined) {

    var jz = function (selector) { return new jz.fn.init(selector); };

    jz.fn = jz.prototype = {
        init: function (selector) {
            if (!selector) {
                return this;
            }

            var len, elem, match;

            if (typeof selector === "string") {
                len = selector.length;
                //#ID
                if (selector[0] === "#" && len > 1) {
                    elem = document.getElementById(selector.substring(1));
                    if (elem) {
                        this[0] = elem;
                        this.length = 1;
                        return this;
                    }
                }
                //.CLASS
                if (selector[0] === "." && len > 1) {

                }
                //DOMElement
            } else if (selector.nodeType) {
                this[0] = selector;
                this.length = 1;
                return this;
            }

            //数组或伪数组
            if (selector !== undefined) {
                var i = 0, match = selector || [];
                for (; i < match.length; i++) {
                    this[i] = match[i];
                }
                this.length = match.length;
            }
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
                pt.length && pt[0].removeChild(this);
            });
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
            obj['e' + type + callback] = callback;/*对象某属性等于该处理程序 this 指向对象本身*/
            obj[type + callback] = function () { obj['e' + type + callback]() }
            obj.attachEvent("on" + type, obj[type + callback]);
        } else {
            obj["on" + type] = callback
        }
    };

    //移除事件的处理程序
    jz.off = function (type, callback, obj) {
        if (obj.removeEventListener) {
            obj.removeEventListener(type, callback, false);
        } else if (obj.detachEvent) {
            obj.detachEvent("on" + type, obj[type + callback]); obj[type + callback] = null
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
    jz.writeStyle('.jzart{color:#333;display:block;_width:640px;min-width:10px;font-size:14px;line-height:150%;position:absolute;white-space:nowrap;background-color:#fff;border:1px solid #b4b4b4;-webkit-border-radius:5px;-moz-border-radius:5px;border-radius:5px;-webkit-box-shadow:0 0 4px #b4b4b4;-moz-box-shadow:0 0 4px #b4b4b4;box-shadow:0 0 4px #b4b4b4;font-family:"Helvetica Neue",Helvetica,Arial}.jzart a,.jzart input{outline:0;resize:none}.jzart .jzart-header{height:55px;padding:0 20px;min-width:150px;line-height:55px;white-space:nowrap;border-bottom:1px solid #ddd}.jzart .jzart-header .jzart-header-title{font-size:16px;font-weight:600;margin-right:30px;white-space:nowrap}.jzart .jzart-header .jzart-header-cloase{border:0;float:right;color:#b4b4b4;cursor:pointer;font-size:38px;line-height:55px;text-decoration:none;vertical-align:middle}.jzart .jzart-header .jzart-header-cloase:hover{color:#333;text-decoration:none}.jzart .jzart-body{_width:20px;margin:20px 25px;word-break:break-all;word-wrap:break-word}.jzart .jzart-iframe{margin:20px;width:600px;border:0;height:400px}.jzart .jzart-footer{min-width:200px;text-align:right;margin:30px 30px 20px 30px}.jzart .art-ok{color:#fff;font-size:15px;cursor:pointer;line-height:1.5;margin-left:15px;text-align:center;white-space:nowrap;display:inline-block;padding:.5em .95em;text-decoration:none;background-color:#099;border:1px solid transparent;border-radius:3px;-moz-border-radius:3px;-webkit-border-radius:3px}.jzart .art-ok:link,.jzart .art-ok:visited{color:#fff;background-color:#099}.jzart .art-ok:hover,.jzart .art-ok:active{color:#fff;text-decoration:none;background-color:#008c8c}.jzart .art-cancel{color:#333;font-size:15px;cursor:pointer;line-height:1.5;text-align:center;white-space:nowrap;display:inline-block;padding:.54em .98em;text-decoration:none;background-color:#fff;border:1px solid #ccc;border-radius:3px;-moz-border-radius:3px;-webkit-border-radius:3px}.jzart .art-cancel:link,.jzart .art-cancel:visited{color:#333;text-decoration:none;background-color:#fff}.jzart .art-cancel:hover,.jzart .art-cancel:active{color:#333;text-decoration:none;background-color:#f2f2f2}.jzart .tip-em{width:0;height:0;font-size:0;line-height:0;border-width:9px;position:absolute;border-style:dashed;border-color:transparent}.jzart .em-top1{left:15px;bottom:-18px;border-top-style:solid;border-top-color:#b4b4b4}.jzart .em-top2{left:15px;bottom:-16px;border-top-color:#fff;border-top-style:solid}.jzart .em-right1{top:15px;left:-18px;border-right-style:solid;border-right-color:#b4b4b4}.jzart .em-right2{top:15px;left:-16px;border-right-color:#fff;border-right-style:solid}.jzart .em-left1{top:15px;right:-18px;border-left-style:solid;border-left-color:#b4b4b4}.jzart .em-left2{top:15px;right:-16px;border-left-color:#fff;border-left-style:solid}.jzart .em-bottom1{top:-18px;left:15px;border-bottom-style:solid;border-bottom-color:#b4b4b4}.jzart .em-bottom2{top:-16px;left:15px;border-bottom-color:#fff;border-bottom-style:solid}.jzart-mask{top:0;left:0;right:0;bottom:0;opacity:.3;position:fixed;background-color:black;filter:alpha(opacity=30)}');

    //拖动 对象、可拖动区域（默认对象）
    jz.drag = function (t, dragT) {
        var disX = dixY = 0, id = jz(t)[0], dragId = dragT != undefined ? jz(dragT)[0] : id;
        dragId.style.cursor = "move";
        dragId.onmousedown = function (e) {
            disX = jz.event(e).clientX - id.offsetLeft;
            disY = jz.event(e).clientY - id.offsetTop;
            document.onmousemove = function (e) {
                var x = jz.event(e).clientX - disX,
                    y = jz.event(e).clientY - disY,
                    maxX = jz.px(document).width - id.offsetWidth + jz.px(document).scrollLeft,
                    maxY = jz.px(document).height - id.offsetHeight + jz.px(document).scrollTop;
                x <= 0 && (x = 0);
                y <= 0 && (y = 0);
                x >= maxX && (x = maxX);
                y >= maxY && (y = maxY);
                id.style.left = x + "px";
                id.style.top = y + "px";
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
    jz.blankClick = function (t) {
        setTimeout(function () {
            jz(t).click(function (event) { jz.stopEvent(event); });
            jz(document).click(function () {
                var elem = t.nodeType ? t : document.getElementById(t);
                if (elem) { try { elem.parentElement.removeChild(elem); } catch (e) { } }
                jz(document).off("click", arguments.callee);
            })
        }, 200)
    };

    //弹窗基础仓库
    jz.art = {
        //版本
        version: "1.0.0",
        //叠堆起始值
        zindex: 666,
        //创建
        create: function (e, c) {
            var o = document.createElement(e);
            c != undefined && c != "" && (o.className = c);
            return o;
        },
        //弹出层包
        wrap: function (obj) {
            var art = this.create('div', 'jzart');
            art.id = "jzart" + jz.random();
            if (obj.fixed) { art.style.position = "fixed"; }
            return art;
        },
        //头部
        header: function () {
            var header = this.create('div', 'jzart-header');
            header.id = "jzartheader" + jz.random();
            return header;
        },
        //头部 - 标题
        title: function (obj) {
            var title = this.create('div', 'jzart-header-title');
            title.innerHTML = obj.title == undefined ? "消息" : obj.title;
            return title;
        },
        //头部 - 关闭
        close: function () {
            var close = this.create('a', 'jzart-header-cloase');
            close.href = "javascript:void(0);";
            close.id = "jzarttopclose" + jz.random();
            close.innerHTML = "×";
            close.title = "关闭";
            return close;
        },
        //底部按钮包
        footer: function () {
            return this.create('div', 'jzart-footer');
        },
        //底部 - 确定按钮
        ok: function (obj) {
            var ok = this.create('a', 'art-ok');
            ok.href = "javascript:void(0);";
            ok.id = "jzartok" + jz.random();
            ok.innerHTML = obj.okValue == undefined ? "确定" : obj.okValue;
            return ok;
        },
        //底部 - 取消按钮
        cancel: function (obj) {
            var cancel = this.create('a', 'art-cancel');
            cancel.href = "javascript:void(0);";
            cancel.id = "jzartcancel" + jz.random();
            cancel.innerHTML = obj.cancelValue == undefined ? "取消" : obj.cancelValue;
            return cancel;
        },
        //主体
        body: function (obj) {
            var body = this.create('div', 'jzart-body');
            obj.width != undefined && (body.style.minWidth = obj.width + "px");
            body.innerHTML = obj.content == undefined ? "" : obj.content;
            return body;
        },
        //弹窗页面
        iframe: function (obj) {
            var iframe = this.create('iframe', 'jzart-iframe');
            iframe.src = obj.src;
            iframe.frameBorder = "0";
            iframe.scrolling = obj.scrolling ? "auto" : "no";
            if (typeof obj.width === "number") { iframe.style.width = obj.width + "px"; }
            if (typeof obj.height === "number") { iframe.style.height = obj.height + "px"; }
            return iframe;
        },
        //居中
        center: function (t) {
            if (t.style.position == "fixed") {
                t.style.top = (jz.px(window).height / 2) - (t.offsetHeight / 2) + "px";
                t.style.left = (jz.px(window).width / 2) - (t.offsetWidth / 2) + "px";
            } else {
                t.style.top = (jz.px(window).height / 2) + jz.px(window).scrollTop - (t.offsetHeight / 2) + "px";
                t.style.left = (jz.px(window).width / 2) + jz.px(window).scrollLeft - (t.offsetWidth / 2) + "px";
            }
        },
        //遮罩层
        mask: function () {
            var mask = this.create('div', 'jzart-mask');
            return mask;
        },
        //移除
        remove: function (id, time) {
            function re(id) {
                if (id != null && id != "") {
                    id = "#" + id;
                    jz(id).remove();
                    jz(id + "mask").remove();
                }
            }
            if (typeof time == "number") { setTimeout(function () { re(id) }, time * 1000) } else { re(id) }
        },
        //拓展方法、属性
        fn: function (msg, mask, header, body, iframe, footer) {
            msg["fn"] = {
                //自身对象
                self: msg,
                //遮罩层（启用时）
                mask: mask,
                //头部
                header: header,
                //内容主体
                body: body,
                //iframe
                iframe: iframe,
                //底部
                footer: footer,
                //隐藏
                hide: function () { msg && (msg.style["display"] = "none"); },
                //显示
                show: function () { msg && (msg.style["display"] = "block"); },
                //删除
                remove: function () {
                    msg && (msg["fn"] = null);
                    jz(msg).remove();
                    jz(mask).remove();
                }
            };
        },
        msgId: "",
        tipId: ""
    };


    /*  弹窗实现 参数列表：
    
     *  title：标题    text/html（jz.msg默认无）
     *  content：文本信息    text/html（jz.msg、jz.confirm 显示的文本或html，jz.iframe不传）
     *  time：倒计时关闭 单位：秒     number（jz.msg默认4秒，jz.confirm、jz.iframe默认不关闭）
     *  blank：点击空白关闭    bool（默认false，此关闭不触发弹窗关闭事件 有遮罩层失效）     
     *  mask：遮罩层    bool（默认 false）
     *  fixed：绝对位置  bool（position:fixed）
     *  center: 自动居中    bool（默认 false）
     *  single：只弹出一个    bool
     *  drag：拖动     bool（jz.msg默认false，jz.confirm、jz.iframe 默认true）     
     *  ok：确定回调     function（确定回调）
     *  okValue：确定按钮文本      text（默认“确定”，有确定按钮则生效）     
     *  cancel：取消回调  function/bool（取消回调/不显示取消按钮）     
     *  cancelValue：取消按钮文本      text（默认“取消”,有取消按钮则生效）     
     *  close；窗口关闭回调    function/bool（关闭回调/不显示，jz.confirm没有，合并为cancel事件）
     *  src：弹窗地址    text（jz.msg、jz.confirm不传）
     *  width：弹窗宽度 单位：px     number（默认 400）
     *  height: 弹窗高度 单位：px      number（默认 200）
     *  scrolling: 弹窗滚动条    bool（默认false）
     */
    jz.msg = function (obj) {
        var msg, mask, header, body, iframe, footer, id;
        //外包
        msg = jz.art.wrap(obj), id = msg.id;
        //弹窗堆叠顺序
        msg.style.zIndex = jz.art.zindex;
        //活动窗口 顶部显示
        jz(msg).mousedown(function () { jz.art.zindex += 1; this.style.zIndex = jz.art.zindex; });
        //头部 有标题的前提
        if (obj.title != undefined) {
            //头部包
            header = jz.art.header(),
                //标题
                title = jz.art.title(obj);
            //显示关闭按钮
            if (obj.close != false) {
                var close = jz.art.close();
                header.appendChild(close);
                //关闭回调
                jz(close).click(function () {
                    if ((obj.close == undefined || obj.close() != false)) {
                        jz.art.remove(id);
                    }
                });
            }
            //载入头部
            header.appendChild(title);
            msg.appendChild(header);
        }
        //载入内容 没传入 src的前提
        if (obj.content != undefined && obj.src == undefined) {
            body = jz.art.body(obj);
            msg.appendChild(body);
        }
        //载入iframe
        if (obj.src != undefined) {
            iframe = jz.art.iframe(obj);
            msg.appendChild(iframe);
            //关闭按钮的id 与 iframe的name属性值一样 
            //子页面调用关闭 获取window.name值 模拟点击父页面id等于window.name的关闭按钮 触发关闭回调
            if (close) { close.id = iframe.name = "jzartiframe" + jz.random(); }
        }
        //底部按钮包 有标题的前提 没传入 src
        if (obj.title != undefined && obj.src == undefined) {
            footer = jz.art.footer();
            //显示取消按钮
            if (obj.cancel != false) {
                //取消按钮
                var cancel = jz.art.cancel(obj);
                footer.appendChild(cancel);
                //取消回调
                jz(cancel).click(function () {
                    if ((obj.cancel == undefined || obj.cancel() != false)) {
                        jz.art.remove(id);
                    }
                })
            }
            //确定按钮
            var ok = jz.art.ok(obj);
            footer.appendChild(ok);
            //确定回调
            jz(ok).click(function () {
                if ((obj.ok == undefined || obj.ok() != false)) {
                    jz.art.remove(id);
                }
            });
            //载入底部按钮包
            msg.appendChild(footer);
        }
        //显示遮罩层
        if (obj.mask) {
            var mask = jz.art.mask();
            mask.id = id + "mask";
            mask.style.zIndex = jz.art.zindex - 1;
            document.body.appendChild(mask);
        }
        //弹出一个
        if (obj.single != false) { jz.art.remove(jz.art.msgId); jz.art.msgId = id; }
        //显示弹窗
        document.body.appendChild(msg);
        //默认4秒关闭
        if (obj.time == undefined) { jz.art.remove(id, 4); }
        else { if (obj.time != 0) { jz.art.remove(id, obj.time); } }
        //点击空白关闭
        if (obj.blank && !obj.mask) { jz.blankClick(msg) }
        //拖动
        if (obj.drag) { if (obj.title != undefined) { jz.drag(msg, header); } else { jz.drag(msg) } }
        //动态居中
        jz.art.center(msg);
        if (obj.center) {
            if (window.addEventListener) {
                window.addEventListener("resize", function () { jz.art.center(msg) }, false);
            } else { window.attachEvent("onresize", function () { jz.art.center(msg) }); }
        }
        jz.art.fn(msg, mask, header, body, iframe, footer);
        return msg;
    };

    /*  confirm  */
    jz.confirm = function (obj) {
        return jz.msg({
            title: obj.title == undefined ? "消息" : obj.title,
            content: obj.content,
            width: obj.width,
            time: obj.time == undefined ? 0 : obj.time,
            single: obj.single == undefined ? false : obj.single,
            drag: obj.drag == undefined ? true : obj.drag,
            blank: obj.blank,
            fixed: obj.fixed,
            mask: obj.mask,
            ok: obj.ok,
            okValue: obj.okValue,
            cancel: obj.cancel,
            cancelValue: obj.cancelValue,
            close: obj.cancel
        })
    };

    /*  子页面回调 iframe关闭事件 子页面传入window.name  title:false则失效 */
    jz.closeback = function (name) {
        var ac = document.getElementById(name);
        ac && ac.click();
    };

    /*  iframe  */
    jz.iframe = function (obj) {
        return jz.msg({
            title: obj.title == false ? undefined : obj.title == undefined ? "窗口" : obj.title,
            src: obj.src,
            time: obj.time == undefined ? 0 : obj.time,
            single: obj.single,
            drag: obj.drag == undefined ? true : obj.drag,
            blank: obj.blank,
            fixed: obj.fixed,
            mask: obj.mask,
            width: obj.width,
            height: obj.height,
            scrolling: obj.scrolling,
            close: obj.close
        })
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
        //提示目标
        var tar = jz(obj.target)[0],
            //提示包
            tip = jz.art.wrap(obj), id = tip.id;
        //弹窗堆叠顺序
        tip.style.zIndex = jz.art.zindex;
        //内容
        var content = jz.art.body(obj),
            sj1 = document.createElement("em"),//三角符号
            sj2 = document.createElement("em"), align = 'bottom';//方向        
        jz.each("top right left bottom".split(" "), function () {
            if (obj.align == this) { align = this; return false; }
        });

        tip.appendChild(content);
        tip.appendChild(sj1);
        tip.appendChild(sj2);
        sj1.className = 'tip-em em-' + align + '1';
        sj2.className = 'tip-em em-' + align + '2';

        //活动窗口 顶部显示
        jz(tip).mousedown(function () { jz.art.zindex += 1; this.style.zIndex = jz.art.zindex; });
        //载入提示
        document.body.appendChild(tip);
        //弹出一个
        if (obj.single != false) { jz.art.remove(jz.art.tipId); jz.art.tipId = id; }
        //倒计时关闭
        if (obj.time != undefined && obj.time != 0) { jz.art.remove(id, obj.time); }
        //点击空白关闭
        if (obj.blank) { jz.blankClick(tip) }
        //选中焦点
        if (obj.focus) { tar.focus() }

        function autowh(t) {
            var oH = tar.offsetHeight, oW = tar.offsetWidth,
                x = jz.px(tar).left + jz.px(window).scrollLeft,
                y = jz.px(tar).top + jz.px(window).scrollTop;

            switch (obj.align) {
                case "top":
                    t.style.top = y - tip.offsetHeight - 13 + "px";
                    t.style.left = (oW > 40 ? x : x - 25 + (oW / 2)) + "px";
                    break;
                case "right":
                    tip.style.top = oH > 40 ? y + "px" : y - 25 + (oH / 2) + "px";
                    tip.style.left = x + oW + 13 + "px";
                    break;
                case "left":
                    tip.style.top = oH > 40 ? y + "px" : y - 25 + (oH / 2) + "px";
                    tip.style.left = x - tip.offsetWidth - 13 + "px";
                    break;
                default: //默认下方
                    tip.style.top = y + oH + 13 + "px";
                    tip.style.left = oW > 40 ? x + "px" : x - 25 + (oW / 2) + "px";
                    break;  //对象小于50 三角符号指向对象中间
            }
        }; autowh(tip);

        if (window.addEventListener) {
            window.addEventListener("resize", function () { autowh(tip) }, false);
        } else { window.attachEvent("onresize", function () { autowh(tip) }); }

        return id;
    };

    window.j = jz;

})(window, undefined)