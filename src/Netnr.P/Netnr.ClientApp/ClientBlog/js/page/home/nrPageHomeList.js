import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrWeb } from "../../nrWeb";

let nrPage = {
    pathname: "/home/list/*",

    init: async () => {
        await nrcRely.remote("netnrmd");

        //目录
        netnrmd.toc();

        //保存
        if (nrVary.domBtnReply) {
            nrVary.domBtnReply.addEventListener('click', async function () {
                if (nrVary.domEditor.value.trim() == "") {
                    nrApp.alert("写点什么...")
                } else {
                    nrApp.setLoading(nrVary.domBtnReply);

                    let fd = new FormData();
                    fd.append("Uid", nrVary.domHidUserId.value);
                    fd.append("UrTargetId", nrVary.domHidWriteId.value);
                    fd.append("UrContent", netnrmd.render(nrVary.domEditor.value));
                    fd.append("UrContentMd", nrVary.domEditor.value);

                    let result = await nrWeb.reqServer('/Home/ReplySave', { method: "POST", body: fd });

                    nrApp.setLoading(nrVary.domBtnReply, true);

                    if (result.code == 200) {
                        nrVary.domEditor.value = "";
                        location.reload(false);
                    } else {
                        nrApp.alert(res.msg);
                    }
                }
            })

            //预览
            nrVary.domBtnPreview.addEventListener('click', function () {
                nrVary.domPreview.innerHTML = netnrmd.render(nrVary.domEditor.value);
            })

            //点赞
            nrVary.domBtnLaud.addEventListener('click', async function () {
                await ConnSave(1)
            })
            //收藏
            nrVary.domBtnMark.addEventListener('click', async function () {
                await ConnSave(2)
            })
            async function ConnSave(action) {

                nrApp.setLoading(nrVary.domBtnLaud);
                nrApp.setLoading(nrVary.domBtnMark);

                let url = `/Home/ConnSave/${nrVary.domHidWriteId.value}?a=${action}`;
                let result = await nrWeb.reqServer(url);

                nrApp.setLoading(nrVary.domBtnLaud, true);
                nrApp.setLoading(nrVary.domBtnMark, true);

                if (result.code == 200) {
                    location.reload(false);
                } else {
                    nrApp.alert(result.msg);
                }
            }
        }

        //代码可编辑
        document.querySelectorAll(".markdown-body pre>code").forEach(domCode => {
            nrcBase.editDOM(domCode.parentElement);
        })

        //阅读量
        if (location.hostname != "localhost" && sessionStorage.getItem("wid") != nrVary.domHidWriteId.value) {
            sessionStorage.setItem("wid", nrVary.domHidWriteId.value);
            fetch("/Home/ReadPlus/" + nrVary.domHidWriteId.value);
        }
    }
}

export { nrPage };