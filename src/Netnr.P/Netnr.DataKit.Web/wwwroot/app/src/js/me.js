import { vary } from "./vary";

var me = {

    /**
     * 配置
     * @param {any} ops
     */
    meConfig: ops => Object.assign({
        value: "",
        theme: vary.theme == "dark" ? "vs-dark" : "vs",
        fontSize: 18,
        language: 'sql',
        automaticLayout: true,
        scrollbar: {
            verticalScrollbarSize: 13,
            horizontalScrollbarSize: 13
        },
        minimap: {
            enabled: true
        }
    }, ops),

    /**
     * 创建
     * @param {any} dom
     * @param {any} config
     */
    meCreate: (dom, config) => new Promise(resolve => {
        var editor = monaco.editor.create(dom, me.meConfig(config));
        resolve(editor);
    }),

    /**
     * 执行格式化
     * @param {any} editor
     */
    meFormatter: editor => editor.trigger('a', 'editor.action.formatDocument'),

    /**
     * 获取选中值
     * @param {any} editor
     */
    meSelectedValue: editor => {
        return editor.getModel().getValueInRange(editor.getSelection());
    },

    /**
     * 获取选中或全部值
     * @param {any} editor
     */
    meSelectedOrAllValue: editor => {
        var val = me.meSelectedValue(editor);
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
    meFormatterSQL: function (text, type) {
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
}

export { me }