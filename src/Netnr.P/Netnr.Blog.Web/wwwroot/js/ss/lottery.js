nr.onReady = function () {
    page.load();

    nr.domSeType.addEventListener('sl-change', function () {
        page.load();
    });
}

var page = {
    load: function () {
        ss.loading(true);
        nr.domSeType.disabled = true;

        ss.fetch({
            url: `http://cp.zgzcw.com/lottery/hisnumber.action?lotteryId=${nr.domSeType.value}&issueLen=36`
        }).then(res => {
            ss.loading(false);
            nr.domSeType.disabled = false;

            res = JSON.parse(res);

            if (res.length) {
                var htm = [];
                res.forEach(item => {
                    var codes = (item.lotteryNumber || item.tryoutNumber).split('+'),
                        code1 = codes[0].split(','), spans1 = "", spans2 = '';
                    for (var k = 0; k < code1.length; k++) {
                        spans1 += '<b class="border border-danger me-1 rounded-circle text-danger text-center d-inline-block" style="min-width:1.6em;">' + code1[k] + '</b>';
                    }

                    if (codes[1] != undefined) {
                        var code2 = codes[1].split(',');
                        for (var u = 0; u < code2.length; u++) {
                            spans2 += '<b class="border border-primary me-1 rounded-circle text-primary text-center d-inline-block" style="min-width:1.6em">' + code2[u] + '</b>';
                        }
                    }

                    htm.push(`<div class="col-xxl-2 col mt-4 p-3">
                    <div class="text-nowrap">
                        <b class="h5 me-2">${item.lotteryExpect}</b>
                        <span>${(new Date(item.ernieDate)).toISOString().substring(0, 10)}</span>
                    </div>
                    <div class="text-nowrap">${spans1}${spans2}</div>
                    </div>`);
                })

                nr.domCardResult.innerHTML = htm.join('');
            }
        }).catch(ex => {
            console.debug(ex);
            ss.loading(false);
            nr.domSeType.disabled = false;
            nr.alert('网络错误');
        })
    }
}