import { nrcBase } from "./nrcBase";

// monaco-editor
let nrEditor = {

    /**
     * 资源依赖，默认远程，可重写为本地
     */
    init: () => new Promise((resolve) => {
        nrcBase.importScript('https://npmcdn.com/monaco-editor@0.36.1/min/vs/loader.js').then(() => {
            nrcBase.amdInit();

            window["require"].config({
                paths: { vs: 'https://npmcdn.com/monaco-editor@0.36.1/min/vs' },
                'vs/nls': { availableLanguages: { '*': 'zh-cn' } }
            });

            window["require"](['vs/editor/editor.main'], function () {
                //xml formatter
                monaco.languages.html.registerHTMLLanguageService('xml', {}, { documentFormattingEdits: true })

                //json comments
                monaco.languages.json.jsonDefaults.diagnosticsOptions.comments = "ignore";

                resolve();
            });
        });
    }),

    /**
     * 配置
     * @param {any} options
     */
    config: options => Object.assign({
        theme: nrcBase.isDark() ? 'vs-dark' : 'vs',
        value: "", language: 'plaintext', fontSize: 18, automaticLayout: true,
        scrollbar: { verticalScrollbarSize: 13, horizontalScrollbarSize: 13 },
        minimap: { enabled: true }
    }, options),

    /**
     * 创建
     * @param {any} dom
     * @param {any} options
     */
    create: (dom, options) => {
        let editor = monaco.editor.create(dom, nrEditor.config(options));
        nrEditor.fullScreen(editor);
        nrEditor.wordWrap(editor);

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
    selectedValue: editor => editor.getModel().getValueInRange(editor.getSelection()),

    /**
     * 保留赋值（可撤销）
     * @param {*} editor 
     * @param {*} value 
     */
    keepSetValue: (editor, value) => {
        let epos = editor.getPosition();
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
    setLanguage: (editor, lang) => monaco.editor.setModelLanguage(editor.getModel(), lang),

    /**
     * 获取语言
     */
    getLanguage: (editor) => editor.getModel().getLanguageId(),

    onChange: (editor, callback, defer) => {
        defer = defer || 500;
        editor.onDidChangeModelContent(function () {
            clearTimeout(nrEditor.defer_change);
            nrEditor.defer_change = setTimeout(function () {
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
                let ebox = event.getContainerDomNode();
                ebox.classList.toggle('nrg-fullscreen');
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
                let cdnode = event.getContainerDomNode();
                let opsWordWrap = cdnode.getAttribute('wordWrap');
                if (opsWordWrap == null) {
                    opsWordWrap = event.getRawOptions().wordWrap;
                }

                // 切换
                opsWordWrap = opsWordWrap == "on" ? "off" : "on";
                cdnode.setAttribute('wordWrap', opsWordWrap);

                event.updateOptions({ wordWrap: opsWordWrap })
            }
        });
    },
}

export { nrEditor }