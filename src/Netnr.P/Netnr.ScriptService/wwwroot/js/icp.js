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

    ss.loading();
    ss.ajax({
        url: "http://micp.chinaz.com/" + encodeURIComponent($("#txtSearch").val()),
        success: function (data) {
            var jw = $(data);

            var v1 = jw.find('table').first();
            var trs = v1.find('tr');
            trs.last().remove();
            trs.eq(5).remove();

            if (jw.find('table').length) {
                $('#dn').html('').append(v1);
            } else {
                $('#dn').html('<p class="text-muted">无备案信息（' + $("#txtSearch").val() + '）</p>');
            }
        },
        error: function () {
            ss.loading(0);
            jz.msg("网络错误");
        },
        complete: function () {
            ss.loading(0);
        }
    })
}
