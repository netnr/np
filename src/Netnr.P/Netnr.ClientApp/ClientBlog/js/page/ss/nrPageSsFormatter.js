import { nrEditor } from "../../../../frame/nrEditor";
import { nrcFile } from "../../../../frame/nrcFile";
import { nrVary } from "../../nrVary";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/formatter",

    ckeyLanguage: "/ss/formatter/language",
    ckeyContent: "/ss/formatter/content",

    init: async () => {
        //编辑器
        nrVary.domEditor.innerHTML = nrApp.tsLoadingHtml;
        await nrEditor.rely();
        nrVary.domEditor.innerHTML = '';

        let defaultLang = await nrStorage.getItem(nrPage.ckeyLanguage) || "sql";
        let defaultContent = await nrStorage.getItem(nrPage.ckeyContent);
        nrVary.domSeTo.value = defaultLang;

        nrApp.tsEditor = nrEditor.create(nrVary.domEditor, {
            value: defaultContent,
            language: defaultLang,
        });

        nrVary.domEditor.classList.add('border');
        nrcBase.setHeightFromBottom(nrVary.domEditor);

        nrPage.bindEvent();
        //恢复选择        
        nrcBase.dispatchEvent('input', nrVary.domSeTo);
        nrVary.domCardLeft.classList.remove('invisible');
    },

    bindEvent: () => {
        //自动保存
        nrEditor.onChange(nrApp.tsEditor, value => nrStorage.setItem(nrPage.ckeyContent, value));

        //切换
        nrVary.domSeTo.addEventListener('input', function () {
            nrStorage.setItem(nrPage.ckeyLanguage, this.value);

            let domCards = nrVary.domSwitch.children;
            for (let index = 0; index < domCards.length; index++) {
                const domCard = domCards[index];
                if (domCard.dataset.value == this.value) {
                    domCard.classList.remove('d-none');
                } else {
                    domCard.classList.add('d-none');
                }
            }

            switch (this.value) {
                case "javascript":
                case "html":
                case "css":
                case "sql":
                case "json":
                    nrEditor.setLanguage(nrApp.tsEditor, this.value);
                    break;
            }

            if (this.value == "sql") {

            }
        });

        //格式化
        nrVary.domBtnTo.addEventListener('click', async function () {
            nrApp.setLoading(nrVary.domBtnTo);

            try {
                let toType = nrVary.domSeTo.value;
                let domCard = nrVary.domSwitch.querySelector(`[data-value="${toType}"]`);
                let content = nrApp.tsEditor.getValue();
                let result = "";

                switch (toType) {
                    case "javascript":
                    case "html":
                    case "css":
                        {
                            await nrcRely.remote('js-beautify.js');

                            let fn = beautifier[toType == "javascript" ? "js" : toType];
                            result = fn(content, {
                                indent_size: domCard.querySelector('.flag-indented').value * 1,
                                "max-preserve-newlines": 2
                            })
                        }
                        break;
                    case "sql":
                        {
                            await nrcRely.remote('sql-formatter.js');

                            result = sqlFormatter.format(content, {
                                language: domCard.querySelector('.flag-sql').value,
                                keywordCase: domCard.querySelector('.flag-uppercase').value,
                                indentStyle: domCard.querySelector('.flag-indented').value,
                            });
                        }
                        break;
                    case "json":
                        result = JSON.stringify(JSON.parse(content), null, domCard.querySelector('.flag-indented').value * 1)
                        break;
                }

                nrEditor.keepSetValue(nrApp.tsEditor, result);
            } catch (ex) {
                nrApp.logError(ex, "格式化失败");
            }

            nrApp.setLoading(nrVary.domBtnTo, true);
        });
        //触发格式化
        nrVary.domSwitch.querySelectorAll('input,select').forEach(dom => {
            dom.addEventListener('input', () => nrVary.domBtnTo.click());
        })

        //接收文件
        nrcFile.init(async (files) => {
            let text = await nrcFile.reader(files[0]);
            nrApp.tsEditor.setValue(text);
        })
    },
}

export { nrPage };