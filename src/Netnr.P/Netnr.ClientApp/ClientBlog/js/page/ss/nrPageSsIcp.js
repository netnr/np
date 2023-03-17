import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

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

        window['fnCall'] = function (json) {
            if (json) {
                let htm = [];
                htm.push(`
                <div class="mb-3">
                    <label class="form-label">主办单位</label>
                    <input class="form-control" readonly value="${json.ComName}" />
                </div>
                <div class="mb-3">
                    <label class="form-label">单位性质</label>
                    <input class="form-control" readonly value="${json.Typ}" />
                </div>
                <div class="mb-3">
                    <label class="form-label">备案号</label>
                    <input class="form-control" readonly value="${json.Permit}" />
                </div>
                <div class="mb-3">
                    <label class="form-label">网址名称</label>
                    <input class="form-control" readonly value="${json.WebName}" />
                </div>
                `);

                nrVary.domCardResult.innerHTML = htm.join("");
            } else {
                nrVary.domCardResult.innerHTML = `<div class="alert alert-warning">无备案信息（${nrVary.domTxtQuery.value}）</div>`;
            }

            nrApp.setLoading(nrVary.domBtnQuery, true);
            nrVary.domTxtQuery.readOnly = false;
        }

        nrVary.domBtnQuery.addEventListener('click', async function () {
            let val = nrVary.domTxtQuery.value.trim();

            if (val == "") {
                nrApp.toast("请输入域名");
            } else {
                nrApp.setLoading(nrVary.domBtnQuery);
                nrVary.domTxtQuery.readOnly = true;

                if (nrPage.domJsonp) {
                    nrPage.domJsonp.remove();
                }
                nrPage.domJsonp = document.createElement("script");
                document.head.appendChild(nrPage.domJsonp);
                nrPage.domJsonp.src = `https://micp.chinaz.com/Handle/AjaxHandler.ashx?action=GetPermit&callback=fnCall&query=${encodeURIComponent(val)}&type=host&_=${Date.now()}`
            }
        });
    },

}

export { nrPage };