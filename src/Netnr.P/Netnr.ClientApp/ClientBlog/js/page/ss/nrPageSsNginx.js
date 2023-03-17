import { nrEditor } from "../../../../frame/nrEditor";
import { nrcFile } from "../../../../frame/nrcFile";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/nginx",

    ckey: '/ss/nginx/content',

    init: async () => {
        //编辑器
        nrApp.setLoading(nrVary.domBtnFormat);
        await nrEditor.init();
        await nrcRely.remote('editor-nginx');
        nrApp.setLoading(nrVary.domBtnFormat, true);

        let defaultLang = 'nginx';
        let defaultContent = await nrStorage.getItem(nrPage.ckey) || "";

        //注册
        monaco.languages.register({ id: defaultLang });
        monaco.languages.setLanguageConfiguration(defaultLang, {
            autoClosingPairs: [
                { open: '{', close: '}' },
                { open: '"', close: '"' },
            ],
        });
        monaco.languages.setMonarchTokensProvider(defaultLang, tokenConf);
        monaco.editor.defineTheme(themeConfig1.base, themeConfig1);
        monaco.editor.defineTheme(themeConfig2.base, themeConfig2);

        //格式化
        monaco.languages.registerDocumentFormattingEditProvider('nginx', {
            provideDocumentFormattingEdits: function (model, _options, _token) {
                return [{
                    text: nrPage.formatterNginx(model.getValue()),
                    range: model.getFullModelRange()
                }];
            }
        });

        nrcBase.setHeightFromBottom(nrVary.domEditor);

        //创建
        nrApp.tsEditor = nrEditor.create(nrVary.domEditor, {
            language: defaultLang,
            value: defaultContent,
        });
        nrVary.domEditor.classList.add('border');

        //自动保存
        nrEditor.onChange(nrApp.tsEditor, value => nrStorage.setItem(nrPage.ckey, value));

        nrPage.bindEvent();
    },

    bindEvent: () => {
        //格式化
        nrVary.domBtnFormat.addEventListener('click', function () {
            nrEditor.formatter(nrApp.tsEditor)
        });

        //接收文件
        nrcFile.init(async (files) => {
            let text = await nrcFile.reader(files[0]);
            nrEditor.keepSetValue(nrApp.tsEditor, text);
        });
    },

    /**
     * 格式化 Nginx
     * @param {*} text 
     * @returns 
     */
    formatterNginx: (text) => {
        let indent = "    ";
        modifyOptions({ INDENTATION: indent });

        let cleanLines = clean_lines(text);
        modifyOptions({ trailingBlankLines: false });
        cleanLines = join_opening_bracket(cleanLines);
        cleanLines = perform_indentation(cleanLines, indent);

        return cleanLines.join("\n")
    },

}

export { nrPage };