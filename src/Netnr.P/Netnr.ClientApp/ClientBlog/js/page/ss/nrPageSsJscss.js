import { nrEditor } from "../../../../frame/nrEditor";
import { nrcFile } from "../../../../frame/nrcFile";
import { nrVary } from "../../nrVary";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/jscss",

    ckeyLang: "/ss/jscss/language",
    ckeyContent: "/ss/jscss/content",

    init: async () => {
        //编辑器
        nrVary.domEditor1.innerHTML = nrApp.tsLoadingHtml;
        await nrcRely.remote('terser.js'); // 在 monaco-editor 之前载入（兼容问题）
        await nrEditor.rely();
        nrVary.domEditor1.innerHTML = '';

        let defaultContent = await nrStorage.getItem(nrPage.ckeyContent) || "/* 粘贴或拖拽 JS、CSS 代码 */";
        let defaultLang = await nrStorage.getItem(nrPage.ckeyLang) || "javascript";

        nrApp.tsEditor = nrEditor.create(nrVary.domEditor1, {
            value: defaultContent,
            language: defaultLang,
        });
        nrPage.editor2 = nrEditor.create(nrVary.domEditor2, {
            value: '',
            wordWrap: "on"
        });

        nrVary.domEditor1.classList.add('border');
        nrVary.domEditor2.classList.add('border');
        nrcBase.setHeightFromBottom(nrVary.domEditor1);
        nrcBase.setHeightFromBottom(nrVary.domEditor2);

        nrPage.bindEvent();
    },

    bindEvent: () => {
        //自动保存
        nrEditor.onChange(nrApp.tsEditor, value => nrStorage.setItem(nrPage.ckeyContent, value));

        //压缩 JS
        nrVary.domBtnMinifyJs.addEventListener('click', async function () {
            try {
                nrApp.setLoading(this);

                nrEditor.setLanguage(nrApp.tsEditor, 'javascript');
                nrEditor.setLanguage(nrPage.editor2, 'javascript');
                nrVary.domCardRight.classList.remove('invisible');

                let content = nrApp.tsEditor.getValue();
                let result = await Terser.minify(content);
                nrApp.setLoading(this, true);

                nrVary.domCardInfo2.innerHTML = [
                    `压缩前：<b>${nrcBase.formatByteSize(content.length)}</b>`,
                    `压缩后：<b>${nrcBase.formatByteSize(result.code.length)}</b>`,
                    `压缩率：<b>${((content.length - result.code.length) / content.length * 100).toFixed(2)}%</b>`,
                ].join('，');

                nrEditor.keepSetValue(nrPage.editor2, result.code);
            } catch (ex) {
                nrApp.logError(ex, "压缩失败");
            }
        });

        //压缩 CSS
        nrVary.domBtnMinifyCss.addEventListener('click', async function () {
            try {
                nrApp.setLoading(this);

                await nrcBase.importScript('/file/clean-css/clean-css.min.js');

                nrEditor.setLanguage(nrApp.tsEditor, 'css');
                nrEditor.setLanguage(nrPage.editor2, 'css');
                nrVary.domCardRight.classList.remove('invisible');

                let content = nrApp.tsEditor.getValue();
                let result = new CleanCSS({
                    "compatibility": "", "format": false, "inline": ["local"], "rebase": false,
                    "level": {
                        "0": true, "1": {
                            "cleanupCharsets": true, "normalizeUrls": true,
                            "optimizeBackground": true, "optimizeBorderRadius": true,
                            "optimizeFilter": true, "optimizeFontWeight": true,
                            "optimizeOutline": true, "removeEmpty": true,
                            "removeNegativePaddings": true, "removeQuotes": true,
                            "removeWhitespace": true, "replaceMultipleZeros": true,
                            "replaceTimeUnits": true, "replaceZeroUnits": true,
                            "roundingPrecision": "", "selectorsSortingMethod": "standard",
                            "specialComments": "all", "tidyAtRules": true,
                            "tidyBlockScopes": true, "tidySelectors": true
                        }, "2": false
                    }, "sourceMap": false
                }).minify(content);

                nrApp.setLoading(this, true);
                if (result.errors.length) {
                    nrEditor.keepSetValue(nrPage.editor2, resultcout.errors.join("\r\n"));
                } else {
                    nrEditor.keepSetValue(nrPage.editor2, result.styles);

                    nrVary.domCardInfo2.innerHTML = [
                        `压缩前：<b>${nrcBase.formatByteSize(content.length)}</b>`,
                        `压缩后：<b>${nrcBase.formatByteSize(result.styles.length)}</b>`,
                        `压缩率：<b>${((content.length - result.styles.length) / content.length * 100).toFixed(2)}%</b>`,
                    ].join('，');
                }
            } catch (ex) {
                nrApp.logError(ex, "压缩失败");
            }
        });

        //下载文件
        nrVary.domBtnDownload.addEventListener("click", function () {
            let filename = nrVary.domTxtFilename.value.trim();
            if (filename == "") {
                filename = nrEditor.getLanguage(nrApp.tsEditor) == "javascript" ? "code.js" : "style.css";
            }
            nrcBase.downloadText(nrPage.editor2.getValue(), filename)
        });

        //接收文件
        nrcFile.init(async (files) => {
            let text = await nrcFile.reader(files[0]);
            nrApp.tsEditor.setValue(text);
        })
    },
}

export { nrPage };