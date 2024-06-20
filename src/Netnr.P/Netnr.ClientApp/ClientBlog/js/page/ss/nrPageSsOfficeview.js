import { nrcFile } from "../../../../frame/nrcFile";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/officeview",

    init: async () => {
        nrcBase.setHeightFromBottom(nrVary.domIframe);

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domTxtUrl.addEventListener('input', async function () {
            await nrPage.view(this.value)
        });

        //接收文件
        nrcFile.init(async (files) => {
            await nrPage.upload(files[0]);
        }, nrVary.domTxtFile);
    },

    api: "https://view.officeapps.live.com/op/view.aspx?src=",

    view: async function (url) {
        if (url) {
            url = nrPage.api + decodeURIComponent(url);
            nrVary.domAurl.href = url;
            nrVary.domAurl.innerText = url;
            nrVary.domIframe.src = url;
            nrVary.domAurl.parentNode.classList.remove("invisible");
            nrVary.domIframe.classList.remove("invisible");
        } else {
            nrVary.domAurl.href = 'javascript:void(0);';
            nrVary.domAurl.innerText = page.api;
            nrVary.domIframe.src = 'about:blank';
            nrVary.domAurl.parentNode.classList.add("invisible");
            nrVary.domIframe.classList.add("invisible");
        }
    },

    upload: async (file) => {
        let err = [];
        if (file.size > 1024 * 1024 * 20) {
            err.push('文档大小限制 20MB')
        }
        if (".doc docx .xls xlsx .ppt pptx".indexOf(file.name.slice(-4).toLowerCase()) == -1) {
            err.push('请选择 Office 文档')
        }
        nrVary.domTxtFile.value = '';

        if (err.length) {
            nrApp.alert(err.join('<hr/>'));
        } else {
            //上传
            let fd = new FormData();
            fd.append("file", file);

            let result = await nrWeb.reqServer('https://netnr.zme.ink/api/v1/Upload', { method: 'POST', body: fd });
            if (result.code == 200) {
                nrPage.view(`https://netnr.zme.ink/${result.data}`);
            } else {
                nrApp.alert(result.msg);
            }
        }
    }
}

export { nrPage };