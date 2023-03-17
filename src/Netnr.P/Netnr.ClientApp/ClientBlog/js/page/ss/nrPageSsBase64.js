import { nrEditor } from "../../../../frame/nrEditor";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrcFile } from "../../../../frame/nrcFile";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/base64",

    init: async () => {
        nrVary.domEditor.innerHTML = nrApp.tsLoadingHtml;

        window["magicBytes"] = await import('magic-bytes.js');
        await nrEditor.init();
        nrVary.domEditor.innerHTML = '';

        nrApp.tsEditor = nrEditor.create(nrVary.domEditor, { wordWrap: "on" });

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrEditor.onChange(nrApp.tsEditor, (value) => {
            nrVary.domCardInfo.innerHTML = `大小：<b>${nrcBase.formatByteSize(value.length)}</b>`;
        })

        //接收文件
        nrcFile.init(async (files) => {
            let file = files[0];

            // file to base64
            let text = await nrcFile.reader(file, 'DataURL');

            nrEditor.keepSetValue(nrApp.tsEditor, text);
            nrVary.domTxtMime.value = text.substring(5, text.indexOf(';'));
        }, nrVary.domTxtFile)

        // base64 to file
        nrVary.domBtnTofile.addEventListener('click', async function () {
            let code = nrApp.tsEditor.getValue();

            let blob = await nrPage.base64AsBlob(code, nrVary.domTxtMime.value);

            //显示
            let vnode;
            if (blob.type.indexOf("image") >= 0) {
                vnode = document.createElement("img");
            }
            if (blob.type.indexOf("audio") >= 0) {
                vnode = document.createElement("audio");
                vnode.controls = true;
            }
            if (blob.type.indexOf("video") >= 0) {
                vnode = document.createElement("video");
                vnode.controls = true;
            }
            if (vnode) {
                vnode.src = URL.createObjectURL(blob);
            } else {
                vnode = document.createElement("a");
                vnode.href = URL.createObjectURL(blob);
                vnode.download = "file.bin";
                vnode.innerHTML = "下载";
            }
            vnode.style.maxWidth = "100%";

            nrVary.domCardResult.innerHTML = "";
            nrVary.domCardResult.appendChild(vnode);
        });

        //base64 to text
        nrVary.domBtnDecode.addEventListener('click', function () {
            try {
                nrEditor.keepSetValue(nrApp.tsEditor, window.atob(nrApp.tsEditor.getValue()));
            } catch (ex) {
                nrApp.logError(ex, 'Base64 解码失败')
            }
        });

        //text to base64
        nrVary.domBtnEncode.addEventListener('click', function () {
            try {
                nrEditor.keepSetValue(nrApp.tsEditor, window.btoa(nrApp.tsEditor.getValue()));
            } catch (ex) {
                nrApp.logError(ex, 'Base64 编码失败')
            }
        });

        //检测 base64 
        nrVary.domDdDetect.addEventListener('show.bs.dropdown', async function () {
            let domBtnToggle = nrVary.domDdDetect.querySelector('button');
            nrApp.setLoading(domBtnToggle);

            let data = nrApp.tsEditor.getValue();
            let parts = data.split(';base64,');
            if (parts.length == 2) {
                data = parts[1];
            }

            let bin = window.atob(data);
            let u8arr = new Uint8Array(bin.length);
            for (let i = 0; i < bin.length; i++) {
                u8arr[i] = bin.charCodeAt(i);
            }

            let ftinfo = magicBytes.filetypeinfo(u8arr);
            console.debug(`识别文件格式：\r\n${JSON.stringify(ftinfo, null, 2)}`);

            let domMenu = nrVary.domDdDetect.querySelector('.dropdown-menu');
            if (ftinfo.length) {
                domMenu.innerHTML = ftinfo.map(x => x ? `<li><a class="dropdown-item">${x.mime} (.${x.extension})</a></li>` : "").join('');
            } else {
                domMenu.innerHTML = '<li><a class="dropdown-item">未检测到</a></li>';
            }

            nrApp.setLoading(domBtnToggle, true);
        });
    },

    /**
     * base64 to blob
     * @param {*} data 
     * @param {*} mimeType 
     * @returns 
     */
    base64AsBlob: async (data, mimeType) => {
        let parts = data.split(';base64,');
        if (parts.length == 2) {
            mimeType = [parts[0].split(':')[1]];
            data = parts[1];
        }

        let bin = window.atob(data);
        let u8arr = new Uint8Array(bin.length);
        for (let i = 0; i < bin.length; i++) {
            u8arr[i] = bin.charCodeAt(i);
        }

        if (mimeType == null || mimeType == "") {
            let ftinfo = magicBytes.filetypeinfo(u8arr);
            console.debug(`识别文件格式：\r\n${JSON.stringify(ftinfo, null, 2)}`);

            if (ftinfo.length) {
                mimeType = ftinfo[0].name;
            }
        }

        return new Blob([u8arr], { type: mimeType });
    },
}

export { nrPage };