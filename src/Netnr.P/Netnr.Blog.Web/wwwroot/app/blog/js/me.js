// monaco editor
var me = {
    init: () => new Promise((resolve) => {
        meRequire(['vs/editor/editor.main'], function () {
            //xml formatter
            monaco.languages.html.registerHTMLLanguageService('xml', {}, { documentFormattingEdits: true })

            resolve();
        });
    }),

    /**
     * 配置
     * @param {any} options
     */
    config: options => Object.assign({
        value: "",
        theme: nr.isDark() ? 'vs-dark' : 'vs',
        language: 'plaintext',
        fontSize: 18,
        automaticLayout: true,
        scrollbar: {
            verticalScrollbarSize: 13,
            horizontalScrollbarSize: 13
        },
        minimap: {
            enabled: true
        }
    }, options),

    /**
     * 创建
     * @param {any} dom
     * @param {any} options
     */
    create: (dom, options) => {
        var editor = monaco.editor.create(dom, me.config(options));
        me.fullScreen(editor);
        me.wordWrap(editor);

        return editor;
    },

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
     * 保留赋值（可撤销）
     * @param {*} editor 
     * @param {*} value 
     */
    keepSetValue: (editor, value) => {
        var epos = editor.getPosition();
        editor.executeEdits('', [{
            range: editor.getModel().getFullModelRange(),
            text: value
        }]);
        editor.setSelection(new monaco.Range(0, 0, 0, 0));
        editor.setPosition(epos);
    },

    /**
     * 设置语言
     * @param {*} editor 
     * @param {*} lang 
     */
    setLanguage: (editor, lang) => {
        monaco.editor.setModelLanguage(editor.getModel(), lang);
    },

    getLanguage: (editor) => editor.getModel().getLanguageId(),

    onChange: (editor, callback, defer) => {
        defer = defer || 500;
        editor.onDidChangeModelContent(function () {
            clearTimeout(me.defer_change);
            me.defer_change = setTimeout(function () {
                callback(editor.getValue());
            }, defer)
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
            keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode.KeyM],
            contextMenuGroupId: "me-01",
            run: function (event) {
                var ebox = event.getContainerDomNode();
                ebox.classList.toggle('nrc-fullscreen');
            }
        });
    },

    /**
     * 是否换行
     * @param {any} editor
     */
    wordWrap: function (editor) {
        editor.addAction({
            id: "meid-wordwrap",
            label: "换行切换",
            keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode.KeyN],
            contextMenuGroupId: "me-01",
            run: function (event) {
                // 读取容器特性值或原始值
                var cdnode = event.getContainerDomNode();
                var opsWordWrap = cdnode.getAttribute('wordWrap');
                if (opsWordWrap == null) {
                    opsWordWrap = event.getRawOptions().wordWrap;
                }

                // 切换
                opsWordWrap = opsWordWrap == "on" ? "off" : "on";
                cdnode.setAttribute('wordWrap', opsWordWrap);

                event.updateOptions({ wordWrap: opsWordWrap })
            }
        });
    }
}

export { me }