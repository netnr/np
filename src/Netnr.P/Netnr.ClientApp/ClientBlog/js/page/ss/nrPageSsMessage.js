import { nrcBase } from "../../../../frame/nrcBase";
import { nrcLeanCloud } from "../../../../frame/nrcLeanCloud";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/message",

    init: async () => {
        nrVary.domCardMessage.innerHTML = nrApp.tsLoadingHtml;

        await nrcRely.remote("netnrmd");
        await nrcBase.importScript('/file/identicon/identicon.min.js');

        await nrPage.viewList();

        nrPage.bindEvent();
    },

    bindEvent: () => {
        //回复
        nrVary.domBtnReply.addEventListener('click', async function () {
            nrApp.setLoading(this);

            try {
                let nickname = nrVary.domTxtNickname.value.trim();
                let content = nrVary.domTxtMessage.value;

                if (content.trim() == "" || netnrmd.render(content) == "") {
                    nrApp.toast("请输入内容");
                } else {
                    let result = await nrcLeanCloud.objBatch([
                        {
                            method: "POST",
                            path: "/1.1/classes/netnr_message",
                            body: {
                                nr_id: nrcBase.snow(),
                                nr_name: nickname,
                                nr_content: content,
                                nr_create: nrcLeanCloud.wrapDate(),
                                nr_status: 1
                            }
                        }
                    ]);

                    if (result.code) {
                        nrApp.alert("保存失败");
                    } else {
                        nrApp.toast("保存成功");
                        nrVary.domTxtMessage.value = "";

                        nrPage.viewList(true);

                        //推送通知
                        let fd = new FormData();
                        fd.append("title", "留言（SS）");
                        fd.append("content", content);
                        await fetch("https://www.netnr.com/api/v1/Push", { method: 'POST', body: fd });
                    }
                }
            } catch (ex) {
                nrApp.logError(ex, '保存失败')
            }

            nrApp.setLoading(this, true);
        });

        //ref TA
        nrVary.domCardMessage.addEventListener('click', async function (event) {
            let target = event.target;
            let action = target.dataset.action;

            if (action == "ref-ta") {
                nrVary.domTxtMessage.value = `@${target.innerText} ${nrVary.domTxtMessage.value}`;
            }
        });

        //滚动
        nrVary.domBtnScroll.addEventListener('click', function () {
            let stop = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
            let ratio = stop / document.body.scrollHeight;
            if (ratio < 0.5) {
                window.scrollTo(0, document.body.scrollHeight);
            } else {
                window.scrollTo(0, 0)
            }
        });
    },

    viewList: async (toBottom) => {
        try {
            let data = await nrcLeanCloud.objQuery("netnr_message", { order: "nr_create", limit: 1000 });

            let htm = [];
            for (let index = 0; index < data.length; index++) {
                let row = data[index];

                let id = 'mi' + (index + 1);
                let nickname = nrcBase.htmlEncode(row.nr_name == "" ? "guest" : row.nr_name);
                let context = '<p><em class="badge text-bg-secondary" title="该信息已被屏蔽">Block</em></p>'

                if (row.nr_status == 1) {
                    context = netnrmd.render(netnrmd.pangu.spacing(row.nr_content)).replace(/@\S+/g, function (n) {
                        return '<a class="text-decoration-none">' + n + '</a>'
                    }).replace(/#\d+/g, function (n) {
                        return '<a href="' + n.replace("#", "#mi") + '" class="text-warning">' + n + '</a>'
                    });
                }

                let itemtmp = `<div class="d-flex mb-2" id="${id}">
                    <div class="mt-1">${iisvg({ value: nickname, size: 42 }).outerHTML}</div>
                    <div class="ms-2">
                        <a class="text-decoration-none" role="button" data-action="ref-ta">${nickname}</a>
                        <small class="opacity-75 mx-2">${nrcBase.formatDateTime('datetime', nrcLeanCloud.wrapDate(row.nr_create))}</small>
                        <a class="text-warning" href="#${id}" role="button">#${index + 1}</a>
                        <div class="text-break mt-2">${context}</div>
                    </div>
                </div>`;

                htm.push(itemtmp);
            }

            if (htm.length) {
                nrVary.domCardMessage.innerHTML = htm.join("");

                if (toBottom) {
                    window.scrollTo(0, nrVary.domCardMessage.scrollHeight);
                }
            } else {
                nrVary.domCardMessage.innerHTML = htm.join("no message");
            }
            nrVary.domCardMessage.nextElementSibling.classList.remove('d-none');
        } catch (ex) {
            nrApp.logError(ex, '查询失败')
        }
    },
}

export { nrPage };