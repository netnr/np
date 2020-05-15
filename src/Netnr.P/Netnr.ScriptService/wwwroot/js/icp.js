$("#txtSearch").keydown(function (e) {
    e = e || window.event;
    var keys = e.keyCode || e.which || e.charCode;
    keys == 13 && $("#btnSearch")[0].click();
})[0].focus();
$("#btnSearch").click(QueryDomainName);
function QueryDomainName() {
    if ($("#txtSearch").val() == "") {
        jz.tip({
            target: "#txtSearch",
            content: "请输入域名",
            align: "bottom",
            time: 4,
            blank: true,
            focus: true
        })
        return
    }

    loading();
    ss.ajax({
        url: "http://icp.chinaz.com/info?q=" + encodeURIComponent($("#txtSearch").val()),
        success: function (data) {
            var tbs = $(data).find('table');
            var mb = tbs.eq(1);
            mb.find('tr').last().remove();
            mb.addClass('table table-bordered table-sm');
            var ws = tbs.eq(3);
            ws.find('tr').last().remove();
            var wstr = ws.find('tr');

            var tdfix = wstr.eq(1).find('td').last();
            tdfix.html(tdfix.find('div').html());

            mb.append(wstr)

            $('#dn').html('').append(mb);
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
