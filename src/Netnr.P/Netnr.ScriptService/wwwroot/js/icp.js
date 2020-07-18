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
        url: "https://icp.aizhan.com/" + encodeURIComponent($("#txtSearch").val()),
        success: function (data) {
            var jw = $(data);

            var v1 = jw.find('#icp-html')
            v1.find('li').each(function () {
                if ($(this).text().indexOf('认证') >= 0) {
                    $(this).remove();
                    return false;
                }
            });
            v1.find('tr').each(function () {
                if ($(this).children().first().text().indexOf('认证') >= 0) {
                    $(this).remove();
                    return false;
                }
            });

            var v2 = jw.find('#icp-company');
            if (v1.children().length > 1) {
                $('#dn').html('').append(v1).append($('<hr/>')).append(v2);
            } else {
                $('#dn').html('<p class="text-muted">无备案信息</p>');
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
