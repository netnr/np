import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcUpstream } from "../../../../frame/nrcUpstream";

let nrPage = {
    pathname: "/ss/icp",

    init: async () => {
        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domTxtQuery.addEventListener('keydown', function (e) {
            if (e.keyCode == 13) {
                nrVary.domBtnQuery.click();
            }
        });

        nrVary.domBtnQuery.addEventListener('click', async function () {
            let domain = nrVary.domTxtQuery.value.trim();

            if (domain == "") {
                nrApp.toast("请输入域名");
            } else {
                nrApp.setLoading(nrVary.domBtnQuery);
                nrVary.domTxtQuery.readOnly = true;

                try {
                    let url = `https://micp.chinaz.com/${domain}`;
                    let result = await nrcUpstream.fetch(url);
                    if (result.includes("主办单位")) {
                        let dom = document.createElement("div");
                        dom.innerHTML = result;
                        var headers = "主办单位,单位性质,备案号,网站名称,审核时间".split(',');
                        let htm = [];
                        dom.querySelectorAll("table tr").forEach(tr => {
                            let tds = tr.querySelectorAll("td");
                            if (tds.length == 2) {
                                let key = tds[0].innerText.trim();
                                let value = tds[1].innerText.trim();
                                key = headers.find(x => key.startsWith(x));
                                if (key) {
                                    htm.push(`<div class="mb-3"><label class="form-label">${key}</label>
                                    <input class="form-control" readonly value="${value}" /></div>`)
                                }
                            }
                        })
                        nrVary.domCardResult.innerHTML = htm.join("");
                    } else {
                        nrVary.domCardResult.innerHTML = `<div class="alert alert-warning">无备案信息（${nrVary.domTxtQuery.value}）</div>`;
                    }
                } catch (error) {
                    console.debug(error)
                    nrVary.domCardResult.innerHTML = `<div class="alert alert-warning">查询失败</div>`;
                }

                nrApp.setLoading(nrVary.domBtnQuery, true);
                nrVary.domTxtQuery.readOnly = false;
            }
        });
    },
}

export { nrPage };