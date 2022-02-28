var config = {
    cid: '',
    start: 1,
    count: 12,
    total: 0,
    isload: false,
    isend: false,
    icons: {
        download: '<svg viewBox="0 0 1024 1024" xmlns="http://www.w3.org/2000/svg" width="200" height="200"><path d="M640 512h118L513 784 268 512h116V144c0-8.8 7.2-16 16-16h224c8.8 0 16 7.2 16 16v368z" fill="#FFE449"/><path d="M513 804c-5.6 0-11-2.4-14.8-6.6l-245-272c-5.2-5.8-6.6-14.4-3.4-21.6 3.2-7.2 10.4-11.8 18.2-11.8h96V144c0-19.8 16.2-36 36-36h224c19.8 0 36 16.2 36 36v348h98c8 0 15 4.6 18.2 11.8 3.2 7.2 1.8 15.6-3.4 21.6l-245 272c-3.8 4.2-9.2 6.6-14.8 6.6zM313 532l200 222.2L713 532h-73c-11 0-20-9-20-20V148H404v364c0 11-9 20-20 20h-71zm583 384H128c-11 0-20-9-20-20s9-20 20-20h768c11 0 20 9 20 20s-9 20-20 20z" fill="#2660D0"/><path d="M574.4 266c-5.6 0-10-4.4-10-10v-64c0-5.6 4.4-10 10-10s10 4.4 10 10v64c0 5.6-4.4 10-10 10zm2 192c-5.6 0-10-4.4-10-10V292c0-5.6 4.4-10 10-10s10 4.4 10 10v156c0 5.6-4.4 10-10 10z" fill="#FFF"/></svg>'
    }
}, rt = {
    //随机一种布局模式
    LayoutMode: function () {
        var gn = [12],
            gi = Math.ceil(Math.random() * gn.length) - 1,
            pn = gn[gi];
        return pn;
    },
    //载入
    LoadList: function (count) {
        if (config.isload || config.isend) {
            return false;
        }

        count = count || rt.LayoutMode();
        config.count = count;
        config.isload = true;

        var uri = 'http://wallpaper.apc.360.cn/index.php?c=WallPaper&start=' + config.start + '&count=' + config.count + '&from=360chrome';
        if (config.cid && config.cid != '') {
            uri += '&a=getAppsByCategory&cid=' + config.cid;
        } else {
            uri += '&a=getAppsByOrder&order=create_time';
        }

        ss.ajax({
            url: uri,
            dataType: 'json',
            success: function (data) {
                data = ss.datalocation(data);
                if (!data.data.length) {
                    config.isend = true;
                    rt.ThEnd();
                    return false;
                }
                config.total = parseInt(data.total);
                config.start += config.count;
                config.isload = false;
                rt.RenderView(data);
            },
            error: function (ex) {
                config.isload = false;
                console.log(ex);
            },
            complete: function () {
                ss.loading(0);
            }
        });
    },
    //下载
    DownView: function (item) {
        var size = ["img_1600_900", "img_1440_900", "img_1366_768", "img_1280_800", "img_1280_1024", "img_1024_768"], htm = [];
        htm.push("<div class='list-group'>");
        htm.push("<a href='" + rt.urlProxy(item.url.replace("__85", "__100")) + "' target='_blank' class='list-group-item' download>" + config.icons.download + item.resolution + "</a>");
        $(size).each(function () {
            var url = item[this], txt = this.replace('img_', '').replace('_', 'x');
            if (url) {
                htm.push("<a href='" + rt.urlProxy(url) + "' class='list-group-item' target='_blank' download>" + config.icons.download + txt + "</a>");
            }
        });
        htm.push('</div>');
        return htm.join('');
    },
    //拼接图片
    JoinImg: function (item, cuturl) {
        var tit = item.utag;
        if (!tit) {
            var tags = item.tag.split(' ');
            tit = tags.slice(1, tags.length - 1).join(' ').replace(/_/g, "").replace(/category/g, "");
        }
        return "<img data-url='" + rt.urlProxy(item.url) + "' title='" + tit + "' src='" + cuturl + "' />"
    },
    //拼接容器
    JoinBox: function (content, pu) {
        return "<div class='col-sm-12 col-md-6" + (pu ? "pu" : "") + " '>" + content + "</div>";
    },
    //呈现
    RenderView: function (json) {
        if (json.errno == "0") {
            var max_wh = "1000_618", htm = [], datas = json.data, len = config.count, i = 0;
            switch (len) {
                case 12:
                    {
                        var col = 2;
                        for (var r = 1; r < len / col + 1; r++) {
                            htm.push("<div class='row'>");
                            for (; i < r * len / (len / col);) {
                                var item = datas[i++], url = rt.urlProxy(item.url);
                                var cuturl = url.replace("/bdr/__85", "/bdm/" + max_wh + "_80");
                                var inbox = ["<div class='col-sm-12 col-md-6 pu'>"];
                                inbox.push(rt.JoinImg(item, cuturl));
                                inbox.push(rt.DownView(item));
                                inbox.push("</div>");
                                htm.push(inbox.join(''));
                            }
                            htm.push("</div>");
                        }
                    }
                    break;
            }
            $('#divWP').append($(htm.join('')));
        } else {
            console.log(json.errmsg);
        }
    },
    //清空
    ClearPage: function () {
        config.cid = '';
        config.start = 1;
        config.isend = false;
        config.isload = false;
        $('#divWP').html('');
    },
    //完
    ThEnd: function () {
        $('#divWP').append($('<div class="row"><div class="col-sm-12 h4 text-center text-muted py-3">没有啦 ...</div></div>'));
    },
    //全屏
    FullShow: function (src) {
        document.documentElement.style.overflowY = "hidden";
        $('<div class="divFP"><a href="javascript:rt.FullClose()">❌</a><img src="' + src + '" title="拖动图片" /></div>').appendTo(document.body);
    },
    //关闭全屏
    FullClose: function () {
        document.documentElement.style.overflowY = "scroll";
        $(document.body).children('div.divFP').remove();
    },
    //代理
    urlProxy: function (url) {
        if (ss.lsStr("useProxy") == "1") {
            url = "https://image.baidu.com/search/down?tn=download&word=download&ie=utf8&fr=detail&url=" + url;
        } else {
            url = url.replace("http://", "https://");
        }
        return url;
    }
};

//点击代理
$('.nrUseProxy').change(function () {
    ss.ls["useProxy"] = this.value;
    ss.lsSave();
    location.reload(false);
}).val(ss.lsStr("useProxy") || 0);

//点击分类
$('.nrType').change(function () {
    rt.ClearPage();
    config.cid = this.value;
    location.hash = this.value;
    rt.LoadList();

    document.title = document.querySelector('.nrType').children[document.querySelector('.nrType').selectedIndex].innerHTML + " 在线壁纸 NET牛人";
    ss.loading();
});

//点击列表
$('#divWP').click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "IMG") {
        rt.FullShow(target.getAttribute('data-url'));
    }
});

//初始化载入最新
$(window).on('load', function () {
    var dtype = location.hash;
    config.cid = dtype.substring(1);
    $('.nrType').val(config.cid);
    document.title = document.querySelector('.nrType').children[document.querySelector('.nrType').selectedIndex].innerHTML + " 在线壁纸 NET牛人";
    ss.loading();
    rt.LoadList();
}).on('scroll mousewheel DOMMouseScroll', function () {
    var sb = $(document).height() - $(this).height() - $(this).scrollTop();
    if (sb < 1200) {
        rt.LoadList();
    }
}).mousedown(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "IMG" && target.parentNode.className.indexOf("divFP") >= 0) {
        var sX = e.clientX - target.offsetLeft,
            sY = e.clientY - target.offsetTop;

        window._drag = 1;
        document.ondragstart = function () { return false; }
        $(window).on('mousemove', function (e) {
            if (e && e.preventDefault) {
                e.preventDefault()
            } else {
                window.event.returnValue = false
            }

            var _self = arguments.callee;
            if (!window._drag) {
                $(window).off('mousemove', _self);
            }

            e = e || window.event;
            var eX = e.clientX, eY = e.clientY;
            var x = eX - sX, y = eY - sY;

            $(target).css('top', y).css('left', x);
            $(window).on('mouseup', function () {
                this.releaseCapture && this.releaseCapture()
                $(window).off('mouseup', arguments.callee).off('mousemove', _self);
            });

            this.setCapture && this.setCapture();
            return false;
        });
    }
}).mouseup(function () {
    window._drag = 0;
});