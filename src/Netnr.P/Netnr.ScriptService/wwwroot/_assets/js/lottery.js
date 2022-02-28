QueryLottery();
$("#seLotteryType").change(QueryLottery);

function QueryLottery() {
    ss.loading();

    ss.ajax({
        url: "http://cp.zgzcw.com/lottery/hisnumber.action?lotteryId=" + $("#seLotteryType").val() + "&issueLen=24",
        dataType: "json",
        success: function (data) {
            data = ss.datalocation(data);
            if (data.length) {
                var htm = [];
                $(data).each(function () {
                    var codes = (this.lotteryNumber || this.tryoutNumber).split('+'),
                        code1 = codes[0].split(','), spans1 = spans2 = '';
                    for (var k = 0; k < code1.length; k++) {
                        spans1 += '<span class="sp1">' + code1[k] + '</span>';
                    }

                    if (codes[1] != undefined) {
                        var code2 = codes[1].split(',');
                        for (var u = 0; u < code2.length; u++) {
                            spans2 += '<span class="sp2">' + code2[u] + '</span>';
                        }
                    }

                    htm.push(
                        '<div class="col-xl-3 col-md-4 col-sm-6 mb-3"><div class="card card-secondary"><div class="p-2">'
                        + '<div class="et"><label class="h5 me-2">' + this.lotteryExpect + '</label><small>' + (new Date(this.ernieDate)).toISOString().substr(0, 10) + '</small></div>'
                        + spans1 + spans2
                        + '</div></div></div>'
                    );
                });
                $("#divlottery").html(htm.join(''));
            }
        },
        error: function () {
            ss.loading(0);
            bs.msg("<h4>网络错误</h4>");
        },
        complete: function () {
            ss.loading(0);
        }
    })
}
