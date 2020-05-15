var pageIndex = 1, pageSize = 20, type = 'xiaohua';

navigator.userAgent.match(/(iPhone|iPod|Android|ios)/i) && type == "tupian" && (pageSize = 5);

location.hash == "#tupian" && (type = "tupian");

type == "xiaohua" ? $("#navtype").find("li")[0].className = "active" : $("#navtype").find("li")[1].className = "active";

QueryJoke();
function QueryJoke() {
    loading();

    ss.ajax({
        url: "http://api.laifudao.com/open/" + type + ".json",
        dataType: "json",
        success: function (data) {
            data = ss.datalocation(data);
            if (data.length) {
                var htm = [];
                if (type == "xiaohua") {
                    $(data).each(function () {
                        htm.push('<div class="joke-item">' + '<p>' + this.title + '</p><span>' + this.content + '</span></div>');
                    })
                } else if (type == "tupian") {
                    $(data).each(function () {
                        var imgurl = this.thumburl.toLocaleLowerCase(),
                            newurl = imgurl.replace("http://", "https://");
                        htm.push('<div class="joke-item"><p>' + this.title + '</p><img src="' + newurl + '" onerror="this.src=\'' + imgurl + '\';this.onerror=null;" /></div>');
                    })
                }
                $("#divJoke").html(htm.join(''));
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

$("#navtype").click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "A") {
        $("#navtype").find("li").each(function () { this.className = ""; });
        target.parentElement.className = "active";
        pageIndex = 1;
        type = target.hash.substr(1);
        QueryJoke();
    }
})