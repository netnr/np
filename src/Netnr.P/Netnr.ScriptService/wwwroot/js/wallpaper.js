var config = {
    cid: '',
    start: 1,
    count: 12,
    total: 0,
    isload: false,
    isend: false
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
        htm.push("<a href='" + item.url.replace("__85", "__100") + "' target='_blank' class='list-group-item' download>▼原图(" + item.resolution + ")</a>");
        $(size).each(function () {
            var url = item[this], txt = this.replace('img_', '').replace('_', 'x');
            if (url) {
                htm.push("<a href='" + url + "' class='list-group-item' target='_blank' download>▼" + txt + "</a>");
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
        return "<img data-url='" + item.url + "' title='" + tit + "' src='" + cuturl + "' />"
    },
    //拼接容器
    JoinBox: function (content, pu) {
        return "<div class='col-sm-12 col-md-6 " + (pu ? "pu" : "") + " '>" + content + "</div>";
    },
    //呈现
    RenderView: function (json) {
        if (json.errno == "0") {
            var max_wh = "1000_618", min_wh = "890_550",
                htm = [], datas = json.data, len = config.count, i = 0;
            switch (len) {
                case 12:
                    {
                        var col = 2;
                        for (var r = 1; r < len / col + 1; r++) {
                            htm.push("<div class='row'>");
                            for (; i < r * len / (len / col);) {
                                var item = datas[i++], url = item.url;
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
        $('<div class="divFP"><a href="javascript:rt.FullClose()">✖</a><img src="' + src + '" title="拖动图片" /></div>').appendTo(document.body);
    },
    //关闭全屏
    FullClose: function () {
        document.documentElement.style.overflowY = "scroll";
        $(document.body).children('div.divFP').remove();
    }
};

//点击分类
$('#dmType').click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "A") {
        rt.ClearPage();
        config.cid = target.hash.substring(1);
        rt.LoadList();
        document.title = target.innerHTML + " 在线壁纸 NET牛人";
        ss.loading();
    }
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
    $('#dmType').find('a').each(function () {
        if (this.hash == dtype) {
            config.cid = dtype.substring(1);
            document.title = this.innerHTML + " 在线壁纸 NET牛人";
            return false;
        }
    });
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
