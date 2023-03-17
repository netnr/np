import { nrcFile } from "../../../../frame/nrcFile";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/gitstorage",
    ckey: "/ss/gitstorage/config",

    init: async () => {
        let config = await nrStorage.getItem(nrPage.ckey);
        if (config) {
            nrVary.domSeName.value = config.name;
            nrVary.domTxtOr.value = config.or;
            nrVary.domTxtToken.value = config.token;

            nrVary.domTxtOr.previousElementSibling.href = `https://github.com/${config.or}`;
        }

        nrPage.bindEvent();
    },

    bindEvent: () => {

        nrVary.domSeName.addEventListener('input', async function () {
            await nrPage.saveConfig();
        });
        nrVary.domTxtOr.addEventListener('input', async function () {
            if (this.value.includes('/')) {
                await nrPage.saveConfig();
                this.previousElementSibling.href = `https://github.com/${this.value}`;
            }
        });
        nrVary.domTxtToken.addEventListener('input', async function () {
            if (this.value == "" || this.value.length > 10) {
                await nrPage.saveConfig();
            }
        });

        //接收文件
        nrcFile.init(async (files) => {
            for (const file of files) {
                await nrPage.addFile(file);
            }
            nrVary.domTxtFile.value = "";
        }, nrVary.domTxtFile);

        //上传
        nrVary.domBtnUpload.addEventListener('click', async function () {
            if (nrPage.uploading) {
                nrApp.alert('Uploading ...');
            } else if (nrPage.files.length == 0) {
                nrApp.alert('Please select or drag and drop files');
            } else {
                nrPage.uploading = true;
                nrPage.uploadIndex = 0;

                let next = async () => {
                    if (nrPage.uploadIndex < nrPage.files.length) {
                        let fobj = nrPage.files[nrPage.uploadIndex++];
                        if (fobj.status == "ready") {
                            await nrPage.upload(fobj);
                        }
                        await next();
                    } else {
                        nrPage.uploading = false;
                    }
                }
                await next();
            }
        }, false);
    },

    saveConfig: async () => {
        let config = {
            name: nrVary.domSeName.value,
            or: nrVary.domTxtOr.value,
            token: nrVary.domTxtToken.value
        }
        await nrStorage.setItem(nrPage.ckey, config);
    },

    files: [],
    uploading: false,
    uploadIndex: 0,

    getNewName: (fileName) => {
        if (nrVary.domSeName.value == "1") {
            let ext = fileName.split('.').pop();
            let newName = `${nrcBase.formatDateTime('yyyy/MMddHHmmssfff')}.${ext}`;
            return newName;
        }
        return fileName;
    },

    addFile: async (file) => {
        let dataURL = await nrcFile.reader(file, 'DataURL');
        if (!dataURL.includes(":image")) {
            dataURL = '/favicon.svg';
        }

        let domCol = document.createElement('div');
        domCol.classList.add('col-lg-6', 'mt-4');
        domCol.innerHTML = `<div class="d-flex">
        <img src='${dataURL}' class="rounded me-2" style="width:2.5em;height:2.5em" />
        <input class="form-control" placeholder="yyyy/MMddxx.ext" value="${nrPage.getNewName(file.name)}" />
        <div class="spinner-border ms-2 mt-1 d-none"><span class="visually-hidden">Loading...</span></div>
        </div>`;
        nrVary.domCardList.appendChild(domCol);

        nrPage.files.push({
            file: file,
            node: domCol.querySelector('input'),
            base64: dataURL.split(',').pop(),
            status: 'ready'
        })
    },

    upload: async (fobj) => {
        let or = nrVary.domTxtOr.value.trim();
        let token = nrVary.domTxtToken.value.trim();
        let path = fobj.node.value.trim();

        if (or.length > 2 && token.length > 10 && path != "") {
            fobj.node.nextElementSibling.classList.remove('d-none');

            let url = `https://api.github.com/repos/${or}/contents/${path}`;
            let proxy = nrVary.domSeProxy.value;
            if (proxy != "") {
                url = proxy + encodeURIComponent(url);
            }

            let result = await nrWeb.reqServer(url, {
                method: 'PUT', headers: {
                    Accept: 'application/vnd.github.v3+json',
                    Authorization: `token ${token}`,
                    'Content-Type': 'application/json'
                }, body: JSON.stringify({ message: 'm', content: fobj.base64 })
            });
            console.debug(result);
            fobj.node.readOnly = true;
            fobj.node.nextElementSibling.classList.add('d-none');

            if (result.content) {
                nrApp.toast(`上传成功：${path}`);
                fobj.node.classList.add('is-valid');
                fobj.status = "ok";

                fobj.node.value = result.content.download_url;
            } else {
                nrApp.toast('上传失败');
                fobj.node.classList.add('is-invalid');
                fobj.status = "fail";
            }
        } else {
            nrApp.toast('Check : owner repo token path');
        }
    }
}

export { nrPage };