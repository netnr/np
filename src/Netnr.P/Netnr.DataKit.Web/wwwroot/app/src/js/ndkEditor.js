import { ndkVary } from "./ndkVary";
import { ndkTab } from "./ndkTab";
import { ndkStep } from "./ndkStep";

var ndkEditor = {

    // 拓展
    extend: () => {
        //xml formatter
        monaco.languages.html.registerHTMLLanguageService('xml', {}, { documentFormattingEdits: true })

        //sql formatter
        let langs = ['sql', 'mysql', 'pgsql'];
        langs.forEach(lang => {
            monaco.languages.registerDocumentFormattingEditProvider(lang, {
                provideDocumentFormattingEdits: function (model, _options, _token) {
                    let dbtype;
                    ndkVary.domTabGroup2.querySelectorAll('.nr-editor-sql').forEach(node => {
                        let tpkey = node.getAttribute('panel');
                        let tpobj = ndkTab.tabKeys[tpkey];
                        if (tpobj.editor.getModel() == model) {
                            dbtype = ndkStep.cpGet(tpkey).cobj.type;
                        }
                    })

                    return [{
                        text: ndkEditor.formatterSQL(model.getValue(), dbtype),
                        range: model.getFullModelRange()
                    }]
                }
            })
        });
    },

    /**
     * 配置
     * @param {any} ops
     */
    config: ops => Object.assign({
        value: "",
        theme: ndkVary.theme == "dark" ? "vs-dark" : "vs",
        fontSize: ndkVary.parameterConfig.editorFontSize.value,
        language: 'sql',
        lineNumbers: ndkVary.parameterConfig.editorLineNumbers.value,
        wordWrap: ndkVary.parameterConfig.editorWordWrap.value,
        automaticLayout: true,
        scrollbar: {
            verticalScrollbarSize: 13,
            horizontalScrollbarSize: 13
        },
        minimap: {
            enabled: false
        }
    }, ops),

    /**
     * 创建
     * @param {any} dom
     * @param {any} config
     */
    create: (dom, config) => new Promise(resolve => {
        var editor = monaco.editor.create(dom, ndkEditor.config(config));
        resolve(editor);
    }),

    /**
     * 执行格式化
     * @param {any} editor
     */
    formatter: editor => editor.trigger('a', 'editor.action.formatDocument'),

    /**
     * 获取选中值
     * @param {any} editor
     */
    selectedValue: editor => {
        return editor.getModel().getValueInRange(editor.getSelection());
    },

    /**
     * 获取选中或全部值
     * @param {any} editor
     */
    selectedOrAllValue: editor => {
        var val = ndkEditor.selectedValue(editor);
        if (val.trim() == "") {
            val = editor.getValue();
        }
        return val;
    },

    /**
     * 格式化SQL
     * @param {any} text
     * @param {any} type
     */
    formatterSQL: function (text, type) {
        var sqlang;
        switch (type) {
            case "MySQL":
            case "MariaDB":
            case "PostgreSQL":
                sqlang = type.toLowerCase();
                break;
            case "Oracle": sqlang = 'plsql'; break;
            case "SQLite":
            case "SQLServer":
            default: sqlang = 'tsql'; break;
        }

        return sqlFormatter.format(text, {
            language: sqlang,
            uppercase: true
        });
    },

    /**
     * 全屏切换
     * @param {any} editor
     */
    fullScreen: function (editor) {
        editor.addAction({
            id: "meid-fullscreen",
            label: "全屏切换",
            keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyM],
            contextMenuGroupId: "me-01",
            run: function (event) {
                var ebox = event.getContainerDomNode();
                ebox.classList.toggle('nrc-fullscreen');
            }
        });
    },

    /**
     * 数据库类型转编辑器语言
     * @param {any} type
     */
    typeAsLanguage: function (type) {
        var sqlang;
        switch (type) {
            case "MySQL":
            case "MariaDB":
                sqlang = "mysql";
                break;
            case "PostgreSQL":
                sqlang = "pgsql";
                break;
            default: sqlang = 'sql'; break;
        }
        return sqlang;
    },
}

export { ndkEditor }