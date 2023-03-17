import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: '/doc/item/*/*',

    init: async () => {

        //markdown 编辑器
        await nrcRely.remote("netnrmdAce.js");
        await nrcRely.remote("netnrmd");
        nrApp.tsMd = netnrmd.init('.nrg-editor', {
            theme: nrcBase.isDark() ? "dark" : "light",
            autosave: false
        });

        nrApp.tsMd.setmd(nrVary.domEditor.dataset.value);
        await nrPage.bindCatalog(nrVary.domHidCode.value);

        nrPage.bindEvent();
    },

    event_resize: (ch) => {
        if (nrApp.tsMd) {
            let vh = ch - nrApp.tsMd.domContainer.getBoundingClientRect().top - 40;
            nrApp.tsMd.height(Math.max(200, vh));
        }
    },

    bindEvent: () => {
        //快捷键
        nrApp.tsMd.addCommand("Ctrl+S", () => nrVary.domBtnSave.click());

        //点击
        document.body.addEventListener('click', async function (event) {
            let target = event.target;

            let action = target.dataset.action;
            switch (action) {
                case "template-api":
                case "template-dic":
                    await nrPage.insertTemplate(action.split('-').pop())
                    break;
            }
        })

        //保存
        nrVary.domBtnSave.addEventListener("click", async function () {
            let obj = {
                DsdContentMd: nrApp.tsMd.getmd(),
                DsdContentHtml: nrApp.tsMd.gethtml(),
                DsdPid: nrVary.domSePid.value
            }
            document.querySelectorAll('input').forEach(dom => {
                if (dom.name) {
                    obj[dom.name] = dom.value.trim();
                }
            })

            let errMsg = [];
            if (obj.DsdTitle == "") {
                errMsg.push('标题 必填')
            }
            if (obj.DsdContentMd == "") {
                errMsg.push("内容 必填");
            }

            if (errMsg.length) {
                nrApp.alert(errMsg.join('<hr/>'));
            } else {
                nrApp.setLoading(nrVary.domBtnSave);

                let fd = nrcBase.jsonToFormData(obj);
                let result = await nrWeb.reqServer('/Doc/ItemSave', { method: "POST", body: fd, redirect: 'manual' });

                nrApp.setLoading(nrVary.domBtnSave, true);

                if (result.code == 200) {
                    nrVary.domHidId.value = result.data;
                    nrApp.toast('保存成功');
                } else {
                    nrApp.alert(result.msg);
                }
            }
        });
    },

    /**
     * 绑定目录
     * @param {*} code 
     */
    bindCatalog: async (code) => {
        let result = await nrWeb.reqServer(`/Doc/MenuTree/${code}`);
        if (result.code == 200) {
            let tree = nrPage.recursionCatalog(result.data) || [];

            let htm = [`<option value="${nrVary.flagGuidEmpty}">(none)</option>`];
            tree.forEach(function (item) {
                for (let key in item) {
                    htm.push(`<option value="${key}">${item[key]}</option>`);
                }
            })
            nrVary.domSePid.innerHTML = htm.join('');
        }
    },

    /**
     * 递归目录
     * @param {*} json 
     * @param {*} deep 
     * @param {*} ptitle 
     * @returns 
     */
    recursionCatalog: (json, deep, ptitle) => {
        let arr = [];
        deep = deep || 0;
        ptitle = ptitle || [];
        for (let i = 0; i < json.length; i++) {
            let ji = json[i], child = ji.children;
            if (!ji.IsCatalog) {
                continue
            }
            let obj = {};
            ptitle.push(ji.DsdTitle);
            obj[ji.DsdId] = ptitle.join(' / ');
            arr.push(obj);
            if (child) {
                deep += 1;
                let arrc = nrPage.recursionCatalog(child, deep, ptitle);
                if (arrc.length) {
                    arr = arr.concat(arrc);
                }
                deep -= 1;
            }
            ptitle.length = deep;
        }
        return arr;
    },

    /**
     * 插入模版
     * @param {*} name 
     */
    insertTemplate: async (name) => {
        let resp = await fetch(`/file/doc-template-${name}.md`);
        let result = await resp.text();
        if (nrApp.tsMd) {
            nrApp.tsMd.insert(result);
        }
    }
}

export { nrPage };