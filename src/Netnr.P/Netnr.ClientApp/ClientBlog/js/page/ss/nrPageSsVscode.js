import { nrEditor } from "../../../../frame/nrEditor";
import { nrcFile } from "../../../../frame/nrcFile";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/vscode",

    ckeyLanguage: "/ss/vscode/language",
    ckeyContent: "/ss/vscode/content",

    init: async () => {
        //编辑器
        nrVary.domEditor.innerHTML = nrApp.tsLoadingHtml;
        await nrEditor.init();
        nrVary.domEditor.innerHTML = '';

        let modesIds = monaco.languages.getLanguages().map(lang => lang.id).sort();
        modesIds = modesIds.filter(x => !x.includes('.'));
        // 语言列表
        modesIds.forEach(lang => {
            let domItem = document.createElement('option');
            domItem.value = lang;
            domItem.innerHTML = lang;
            nrVary.domSeLanguage.appendChild(domItem);
        });

        let defaultLang = await nrStorage.getItem(nrPage.ckeyLanguage) || 'javascript';
        let defaultContent = await nrStorage.getItem(nrPage.ckeyContent) || 'console.log("Hello world!");';

        nrVary.domSeLanguage.classList.remove('invisible');
        nrVary.domSeLanguage.value = defaultLang;
        if (defaultLang == "javascript") {
            nrVary.domBtnRun.classList.remove('d-none');
        }

        nrApp.tsEditor = nrEditor.create(nrVary.domEditor, {
            value: defaultContent,
            language: defaultLang,
        });
        nrVary.domEditor.editor = nrApp.tsEditor;

        nrVary.domEditor.classList.add('border');

        nrcBase.setHeightFromBottom(nrVary.domEditor);

        nrPage.bindEvent();
    },

    bindEvent: () => {

        //修改语言
        nrVary.domSeLanguage.addEventListener('input', function () {
            nrEditor.setLanguage(nrApp.tsEditor, this.value)

            nrStorage.setItem(nrPage.ckeyLanguage, this.value);

            if (this.value == "javascript") {
                nrVary.domBtnRun.classList.remove('d-none');
            } else {
                nrVary.domBtnRun.classList.add('d-none');
            }
        });

        //变动自动保存
        nrEditor.onChange(nrApp.tsEditor, (value) => nrStorage.setItem(nrPage.ckeyContent, value));

        //运行
        nrApp.tsEditor.addCommand(monaco.KeyCode.PauseBreak, () => {
            nrVary.domBtnRun.click();
        });
        nrVary.domBtnRun.addEventListener('click', async function () {
            switch (nrEditor.getLanguage(nrApp.tsEditor)) {
                case "javascript":
                    try {
                        window.ee = new Function(nrApp.tsEditor.getValue());
                        ee();
                    } catch (ex) {
                        console.error(ex);
                    }
                    break;
            }
        });

        //接收文件
        nrcFile.init(async (files) => {
            let text = await nrcFile.reader(files[0]);
            nrApp.tsEditor.setValue(text);
        })
    },
}

export { nrPage };