/*
 * 2016-2-25
 * https://www.netnr.com
 * netnr
 */

(function (window) {
    var jc = function (id) { return document.getElementById(id) }
    //添加事件处理程序
    jc.on = function (type, fn, obj) {
        if (obj.addEventListener) {
            (type == "mousewheel" && typeof (onmousewheel) == "undefined") && (type = "DOMMouseScroll");
            obj.addEventListener(type, fn, false);
        } else if (obj.attachEvent) {
            obj['e' + type + fn] = fn;/*对象某属性等于该处理程序 this 指向对象本身*/
            obj[type + fn] = function () { obj['e' + type + fn]() }
            obj.attachEvent("on" + type, obj[type + fn]);
        } else { obj["on" + type] = fn }
    };
    /*移除事件处理程序*/
    jc.off = function (type, fn, obj) {
        if (obj.removeEventListener) {
            (type == "mousewheel" && typeof (onmousewheel) == "undefined") && (type = "DOMMouseScroll");
            obj.removeEventListener(type, fn, false);
        } else if (obj.detachEvent) {
            obj.detachEvent("on" + type, obj[type + fn]); obj[type + fn] = null
        }
    };
    jc.event = function (e) { return window.event || e }
    jc.init = function (ops) { return new init(ops) }
    //初始化
    function init(ops) {
        var scrollId = ops.id,//id
            scrollBarW = ops.barwidth == undefined ? 8 : ops.barwidth,//滚动条宽度
            scrollBarH = ops.barheight == undefined ? 60 : ops.barheight,//滚动条高度
            scrollBarC = ops.barcolor == undefined ? "#009a61" : ops.barcolor,//滚动条颜色
            scrollBarB = ops.barbgcolor == undefined ? "#fff" : ops.barbgcolor,//滚动条背景色
            scrollH = ops.scrollheight == undefined ? 80 : ops.scrollheight;//每次滚动高度

        //只绑定一次
        if (jc(scrollId).getAttribute('data-scroll') == "bind") {
            return false;
        }

        var visualH = jc(scrollId).parentElement.offsetHeight,//内容可见高度
            contentH = jc(scrollId).scrollHeight,//内容真实高度
            contentMaxH = contentH - visualH,//内容可滚动高度
            barMaxH = visualH - scrollBarH;//滚动条可滚动高度

        //隐藏或高度不够跳过
        if (visualH == 0 || contentH == 0 || contentMaxH < 0) {
            return false;
        }

        //前提
        if (navigator.userAgent.match(/(iPhone|iPod|Android|ios)/i)) {
            scrollBarW = 0; jc(scrollId).parentElement.style.overflow = "auto";
        } else { jc(scrollId).parentElement.style.overflow = "hidden"; }
        jc(scrollId).style.top = '0';
        jc(scrollId).style.left = '0';
        jc(scrollId).style.position = "absolute";
        jc(scrollId).style.right = scrollBarW + 'px';
        jc(scrollId).parentElement.style.position = "relative";

        //添加工具
        var bar = document.createElement("div");
        bar.className = 'jsscrollbox';
        bar.style.cssText = 'top:0;right:0;bottom:0;width:' + scrollBarW + 'px;position:absolute;background-color:' + scrollBarB;
        bar.innerHTML = '<div class="jcscrollbar" style="top:0;width:' + scrollBarW + 'px;position:absolute;background-color:' + scrollBarC + ';height:' + scrollBarH + 'px"></div>';
        jc(scrollId).parentElement.appendChild(bar);

        //已绑定标识
        jc(scrollId).setAttribute('data-scroll', 'bind');

        //鼠标拖动
        jc.on("mousedown", function (e) {
            var target = jc.event(e).srcElement || jc.event(e).target;
            //滚动条对象
            if (target.className.indexOf("jcscrollbar") >= 0) {
                var xdY = jc.event(e).clientY - target.offsetTop;
                document.onmousemove = function (e) {
                    var y = jc.event(e).clientY - xdY;
                    y <= 0 && (y = 0);
                    y > barMaxH && (y = barMaxH);
                    target.style.top = y + "px";
                    contentH > visualH && (jc(scrollId).style.top = -(contentH - visualH) * (y / barMaxH) + "px");
                    return false;
                }
                document.onmouseup = function () {
                    document.onmousemove = null;
                    document.onmouseup = null;
                    this.releaseCapture && this.releaseCapture()
                }
                this.setCapture && this.setCapture();
            }
        }, jc(scrollId).parentElement);

        //鼠标点击
        jc.on("click", function (e) {
            var target = jc.event(e).srcElement || jc.event(e).target;
            //滚动条框对象
            if (target.className.indexOf("jsscrollbox") >= 0) {
                var xdY = jc.event(e).clientY - target.offsetTop,
                    top = Math.abs(target.getBoundingClientRect().top - xdY);//距离顶部的距离
                top <= 0 && (top = 0);
                top > barMaxH && (top = barMaxH);
                this.lastChild.firstChild.style.top = top + "px";
                contentH > visualH && (jc(scrollId).style.top = -(contentH - visualH) * (top / barMaxH) + "px");
            }
        }, jc(scrollId).parentElement);

        //滚轮
        jc.on("mousewheel", function (e) {
            if (contentH > visualH) {
                var ccH = parseInt(jc(scrollId).style.top.replace('px', '')),//当前内容高度
                    cbH = Math.abs(parseInt(this.lastChild.firstChild.style.top.replace('px', '')));//当前滚动条高度
                //向下滚动
                if (jc.event(e).wheelDelta == -120 || jc.event(e).detail == 3) {
                    jc.scrollrun("box", jc(scrollId), 1, scrollH, contentMaxH);
                    var bh = (ccH - scrollH) / -contentMaxH;//滚动比例
                    bh >= 1 && (bh = 1);
                    var callH = (this.offsetHeight - scrollBarH) * bh - cbH;//滚动条滚动高度
                    jc.scrollrun("bar", this.lastChild.firstChild, 1, callH, barMaxH);
                } else {
                    jc.scrollrun("box", jc(scrollId), 0, scrollH, contentMaxH);
                    var hh = (ccH + scrollH) >= 0 ? -0 : ccH + scrollH;
                    var bh = hh / -contentMaxH;//滚动比例
                    var callH = cbH - (this.offsetHeight - scrollBarH) * bh
                    jc.scrollrun("bar", this.lastChild.firstChild, 0, callH, barMaxH);
                }

                if (jc(scrollId).style.top[0] != 0 && jc(scrollId).style.top.replace('px', '') != -contentMaxH) {
                    jc.on("mousewheel", function (e) {
                        if (e && e.preventDefault) { e.preventDefault() }
                        else { window.event.returnValue = false; }
                        jc.off("mousewheel", arguments.callee, document);
                    }, document);
                }
            }
        }, jc(scrollId).parentElement);

        //滑动 目标类型、对象、方向(1下)、高度、可滚动最大高度
        jc.scrollrun = function (type, target, fx, height, maxH) {
            var top = parseInt(target.style.top.replace('px', ''));
            arguments.length == 7 && (top = arguments[5]); //初始当前高度
            var oneh = arguments.length == 5 ? 1 : arguments[6];//动态高度
            oneh > height && (oneh = height);
            if (fx == 1) {
                if (type == "bar") { target.style.top = (top + oneh) > maxH ? maxH + "px" : top + oneh + "px"; }
                else { target.style.top = (top - oneh) < -maxH ? -maxH + "px" : top - oneh + "px"; }
            } else {
                if (type == "bar") { target.style.top = (top - oneh) < 0 ? 0 + "px" : top - oneh + "px"; }
                else { target.style.top = (top + oneh) > 0 ? 0 + "px" : top + oneh + "px"; }
            } oneh < height && setTimeout(function () { jc.scrollrun(type, target, fx, height, maxH, top, oneh + 3); }, 1);
        }
    }

    window.jc = jc;
})(window)