import { nrEditor } from "../../../../frame/nrEditor";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: ['/gist/index', '/gist/edit/*'],

    tableKey: "NoteId",
    rowId: 0,
    ckey: "/gist/index/content",

    init: async () => {
        nrVary.domEditor.innerHTML = nrApp.tsLoadingHtml;
                
        //初始化编辑器
        await nrEditor.init();

        let modesIds = monaco.languages.getLanguages().map(lang => lang.id).sort();
        modesIds = modesIds.filter(x => !x.includes('.'));

        //语言列表
        modesIds.forEach(language => {
            let domItem = document.createElement("option");
            domItem.value = language;
            domItem.innerHTML = language;
            nrVary.domSeLanguage.appendChild(domItem);
        });
        nrVary.domSeLanguage.value = nrVary.domSeLanguage.dataset.value;

        nrVary.domEditor.innerHTML = '';
        nrApp.tsEditor = nrEditor.create(nrVary.domEditor, {
            value: nrVary.domHidContent.value,
            language: nrVary.domSeLanguage.value,
            scrollbar: {
                verticalScrollbarSize: 0,
                horizontalScrollbarSize: 12
            },
            minimap: {
                enabled: true
            }
        });
        nrcBase.setHeightFromBottom(nrVary.domEditor);
        nrVary.domEditor.classList.add('border');

        //新增，自动保存
        if (location.pathname == "/gist") {
            nrVary.domHidContent.value = await nrStorage.getItem(nrPage.ckey) || "";

            nrApp.tsEditor.setValue(nrVary.domHidContent.value);
            nrEditor.onChange(nrApp.tsEditor, (value) => {
                nrStorage.setItem(nrPage.ckey, value)
            })
        }

        nrVary.domBtnSave.classList.remove('d-none');

        nrPage.bindEvent();
    },

    bindEvent: () => {

        //语言切换
        nrVary.domSeLanguage.addEventListener('change', function () {
            nrEditor.setLanguage(nrApp.tsEditor, this.value);
        });

        //快捷键
        nrApp.tsEditor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyS, () => nrVary.domBtnSave.click())

        //保存
        nrVary.domBtnSave.addEventListener('click', async function () {
            let code = nrApp.tsEditor.getValue(), arrv = code.split('\n'), row = arrv.length;
            let obj = {
                GistCode: nrVary.domHidCode.value,
                GistRemark: nrVary.domTxtRemark.value,
                GistFilename: nrVary.domTxtFilename.value,
                GistLanguage: nrVary.domSeLanguage.value,
                GistTheme: nrcBase.isDark() ? "vs-dark" : "vs",
                GistContent: code,
                GistContentPreview: arrv.slice(0, 10).join('\n'),
                GistRow: row,
                GistOpen: 1
            };

            let errMsg = [];
            if (obj.GistRemark.trim() == "") {
                errMsg.push('description');
            }
            if (obj.GistLanguage == "") {
                errMsg.push('language');
            }
            if (obj.GistFilename.trim() == "") {
                errMsg.push('Filename including extension');
            }
            if (obj.GistContent.trim() == "") {
                errMsg.push('content');
            }

            if (obj.GistContent.length > 10000 * 50) {
                errMsg.push('content is too long ( less than 500000 )');
            }

            if (errMsg.length) {
                nrApp.alert(errMsg.join('<hr/>'));
            } else {
                nrApp.setLoading(nrVary.domBtnSave);

                let fd = nrcBase.fromKeyToFormData(obj);

                let result = await nrWeb.reqServer("/Gist/Save", { method: "POST", redirect: 'manual', body: fd });

                nrApp.setLoading(nrVary.domBtnSave, true);

                if (result.code == 200) {
                    if (location.pathname == "/gist") {
                        await nrStorage.removeItem(nrPage.ckey);
                    }

                    location.href = "/gist/code/" + result.data;
                } else {
                    nrApp.alert(result.msg);
                }
            }
        });
    },

}

export { nrPage };