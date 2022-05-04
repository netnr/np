nr.onReady = function () {
    nr.domTxtQuery.addEventListener('keydown', function (e) {
        if (e.keyCode == 13) {
            nr.domBtnQuery.click();
        }
    });

    nr.domBtnQuery.addEventListener('click', function () {
        var val = nr.domTxtQuery.value.trim();

        if (val == "") {
            nr.alert("请输入域名");
        } else {
            nr.domBtnQuery.loading = true;

            ss.fetch({
                url: "https://whois.west.cn/icp/" + encodeURIComponent(val),
                encoding: "GBK"
            }).then(res => {
                nr.domBtnQuery.loading = false;
                var html = [];

                var dom = (new DOMParser()).parseFromString(res, "text/html");
                var table = dom.querySelector('.info-table');
                if (table) {
                    table.querySelectorAll('tr').forEach(tr => {
                        var tds = tr.querySelectorAll('td');
                        html.push(`<sl-input class="mt-3" label="${tds[0].innerText.trim()}" value="${tds[1].innerText.trim()}"></sl-input>`);
                    })
                    nr.domCardResult.innerHTML = html.join("");
                } else {
                    nr.domCardResult.innerHTML = `<div class="mt-3">无备案信息（${val}）</div>`;
                }
            }).catch(ex => {
                console.debug(ex);
                nr.alert("网络错误");
            })
        }
    });
}