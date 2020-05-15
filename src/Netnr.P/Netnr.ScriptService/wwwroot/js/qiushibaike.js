var page = 1, pageSize = 9, type = 'latest';

function queryP() {
    try {
        var hp = location.hash,
            cpage = getQueryString("page", hp) || page,
            ctype = getQueryString("type", hp) || type;

        if (!isNaN(cpage = Number(cpage))) {
            page = Math.max(cpage, 1);
        }

        switch (ctype.toLowerCase()) {
            case "text":
            case "image":
            case "video":
                type = ctype;
                break;
            default:
                type = "latest";
                break;
        }

        $("#navtype").children().removeClass('active').end().find('a').each(function () {
            if (this.hash.indexOf(type) >= 0) {
                $(this).parent().addClass('active');
                return false;
            }
        });
    } catch (e) { }
}

//请求
QueryJoke();
function QueryJoke() {
    queryP();

    loading();

    ss.ajax({
        url: "http://m2.qiushibaike.com/article/list/" + type + "?type=refresh&page=" + page + "&count=" + pageSize,
        dataType: "json",
        success: function (data) {
            data = ss.datalocation(data);
            if (data.err == 0) {
                var htm = [], json = {};
                $(data.items).each(function () {
                    var di = this;
                    htm.push('<div class="joke-item">');
                    switch (di.format) {
                        case "word":
                            htm.push(di.content);
                            break;
                        case "image":
                            htm.push(di.content + '<div><img src="' + (di.high_loc || di.high_url).replace("http:", "https:") + '"/></div>');
                            break;
                        case "video":
                            htm.push(di.content + '<div><video controls="controls" preload="meta" src="' + di.high_url.replace("http:", "https:") + '"></video></div>');
                            break;
                        case "multi":
                            htm.push(di.content);
                            $(di.attachments).each(function () {
                                switch (this.format) {
                                    case "image":
                                        htm.push('<div><img src="' + this.high_url.replace("http:", "https:") + '"/></div>');
                                        break;
                                    case "gif":
                                    case "video":
                                        htm.push('<div><video controls="controls" preload="meta" src="' + this.high_url.replace("http:", "https:") + '"></video></div>');
                                        break;
                                }
                            });
                            break;
                    }
                    htm.push('</div>');
                });

                $("#divJoke").html(htm.join(''));

                json.page = page;
                json.pageSize = pageSize;
                json.total = data.total;
                //生成分页按钮
                var pcm = PageControlMake(json);
                $('#tabPage').html(pcm.button + '<div class="float-left ml-3 mt-2">' + pcm.explain + '</div>');
                //分页按钮绑定事件
                BindPageButtonEvent(json);

                //回到顶部
                window.scrollTo(0, 0);
            }
        },
        error: function () {
            loading(0);
            jz.msg("网络错误");
        },
        complete: function () {
            loading(0);
        }
    })
}

//点击视频播放/暂停
$("#divJoke").click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    $(this).find('video').each(function () {
        if (this.contains(target)) {
            if (this.paused) {
                this.play();
            } else {
                this.pause();
            }
            return false;
        }
    })
});


//生成分页按钮 需手动绑定当前页样式和点击事件
function PageControlMake(json) {
    var pi = json.page, ps = json.pageSize, total = json.total, pc = Math.ceil(total / ps),
        txtHtml = '第 ' + (total == 0 ? 0 : pi) + ' 页，共 ' + pc + '页，共 ' + total + ' 条记录',
        btnHtml = '<ul class="pagination float-left">'
            + '<li class="page-item"><a class="page-link" class="page-link" href="javascript:void(0)" >«</a></li>';
    pi = parseInt(pi);
    if (total) {
        if (pc < 3) {
            for (var i = 1; i <= pc; i++) {
                btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">' + i + '</a></li>';
            }
        } else {
            btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">1</a></li>';
            pi - 3 > 1 && (btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">...</a></li>');
            pi - 2 > 1 && (btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">' + (pi - 2) + '</a></li>');
            pi - 1 > 1 && (btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">' + (pi - 1) + '</a></li>');
            pi > 1 && pi < pc && (btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">' + pi + '</a></li>');
            pi + 1 < pc && (btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">' + (pi + 1) + '</a></li>');
            pi + 2 < pc && (btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">' + (pi + 2) + '</a></li>');
            pi + 3 < pc && (btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">...</a></li>');
            btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)">' + pc + '</a></li>';
        }
        btnHtml += '<li class="page-item"><a class="page-link" href="javascript:void(0)" >»</a></li></ul>';
    } else {
        btnHtml = '';
    }
    return { button: btnHtml, explain: txtHtml }
}

//给分页按钮绑定事件
function BindPageButtonEvent(json) {
    var pi = json.page, ps = json.pageSize, total = json.total, pc = Math.ceil(total / ps),
        tp = document.getElementById("tabPage"), btns = tp.getElementsByTagName("a");
    for (var i = 0; i < btns.length; i++) {
        if (parseInt(btns[i].innerHTML) == pi) {
            btns[i].parentElement.className = "active";
            break;
        }
    }
    tp.onclick = function (e) {
        e = e || window.event;
        var target = e.target || e.srcElement;
        if (target.nodeName == 'A') {
            if (target.innerHTML.indexOf("«") > -1 && pi != 1) {
                page = pi - 1;
                location.hash = "#type=" + type + "&page=" + page;
                QueryJoke();
            } else if (target.innerHTML.indexOf("»") > -1 && pi != pc) {
                page = pi * 1 + 1;
                location.hash = "#type=" + type + "&page=" + page;
                QueryJoke();
            } else if (!isNaN(parseInt(target.innerHTML))) {
                page = target.innerHTML;
                location.hash = "#type=" + type + "&page=" + page;
                QueryJoke();
            }
        }
    }
}

//获取参数
function getQueryString(name, sp) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = (sp || window.location.search).substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

//切换分类
$("#navtype").click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "A") {
        location.hash = target.hash;
        pageIndex = 1;
        QueryJoke();
    }
})