import { ndkVary } from "./ndkVary";
import { ndkI18n } from "./ndkI18n";
import { ndkTab } from "./ndkTab";
import { ndkRequest } from "./ndkRequest";

// monaco editor
var ndkEditor = {
    init: (meRequire) => {
        if (meRequire) {
            //提取脚本连接        
            var melsrc, ops = { paths: {} }
            for (var i = 0; i < document.scripts.length; i++) {
                var script = document.scripts[i];
                if (script.src.includes("monaco-editor")) {
                    melsrc = script.src;
                    break;
                }
            }
            ops.paths["vs"] = melsrc.replace("/loader.js", "");

            // 本地化
            if (ndkI18n.languageGet() == "zh-CN") {
                ops['vs/nls'] = { availableLanguages: { '*': 'zh-cn' } };
            }
            meRequire.config(ops);

            //编辑器资源载入
            meRequire(['vs/editor/editor.main'], function () {
                ndkEditor.extend();
            });
        } else {
            ndkEditor.extend();
        }
    },

    // 拓展
    extend: () => {
        //xml formatter
        monaco.languages.html.registerHTMLLanguageService('xml', {}, { documentFormattingEdits: true })

        //sql formatter
        let langs = ['sql', 'mysql', 'pgsql'];
        langs.forEach(lang => {
            monaco.languages.registerDocumentFormattingEditProvider(lang, {
                provideDocumentFormattingEdits: function (model, _options, _token) {
                    var cp = ndkTab.tabActiveCp();
                    return [{
                        text: ndkEditor.formatterSQL(model.getValue(), cp.cobj.type),
                        range: model.getFullModelRange()
                    }]
                }
            })
        });

        // sql completion
        monaco.languages.registerCompletionItemProvider('sql', {
            provideCompletionItems: async function (model, position) {
                // 找到对象中的属性
                // var textUntilPosition = model.getValueInRange({
                //     startLineNumber: 1,
                //     startColumn: 1,
                //     endLineNumber: position.lineNumber,
                //     endColumn: position.column
                // });
                //textUntilPosition.match()

                var cp = ndkTab.tabActiveCp();

                var result = [];
                //common
                var commonKeys = ["add", "add constraint", "all", "alter", "alter column", "alter table", "and", "any", "as", "asc", "backup database", "between", "case", "check", "column", "constraint", "create", "create database", "create index", "create or replace view", "create table", "create procedure", "create unique index", "create view", "database", "default", "delete", "desc", "distinct", "drop", "drop column", "drop constraint", "drop database", "drop default", "drop index", "drop table", "drop view", "exec", "exists", "foreign key", "from", "full outer join", "group by", "having", "in", "index", "inner join", "insert into", "insert into select", "is null", "is not null", "join", "left join", "like", "limit", "not", "not null", "offset", "or", "order by", "outer join", "primary key", "procedure", "right join", "rownum", "select", "select distinct", "select into", "select top", "set", "table", "top", "truncate table", "union", "union all", "unique", "update", "values", "view", "where"];
                switch (cp.cobj.type) {
                    case 'Oracle':
                        commonKeys = commonKeys.concat(['dual', 'grant', 'revoke']);
                        break;
                }
                commonKeys.map(item => result.push({
                    label: item,
                    kind: monaco.languages.CompletionItemKind.Field,
                    documentation: 'common',
                    insertText: item,
                    range: range
                }));
                var functionKeys = { avg: "avg(1)", count: "count(*)", max: "max(1)", min: "min(1)", sum: "sum(1)" };
                for (const label in functionKeys) {
                    result.push({
                        label: label,
                        kind: monaco.languages.CompletionItemKind.Function,
                        documentation: 'function',
                        insertText: functionKeys[label],
                        range: range
                    })
                }

                if (true) {
                    var word = model.getWordUntilPosition(position);
                    var range = {
                        startLineNumber: position.lineNumber,
                        endLineNumber: position.lineNumber,
                        startColumn: word.startColumn,
                        endColumn: word.endColumn
                    };

                    if (cp) {
                        //database
                        var itemDatabase = await ndkRequest.reqDatabaseName(cp.cobj);
                        itemDatabase.forEach(item => {
                            result.push({
                                label: item.DatabaseName,
                                kind: monaco.languages.CompletionItemKind.Text,
                                documentation: 'database',
                                insertText: item.DatabaseName,
                                range: range
                            });
                        })

                        if (cp.databaseName) {
                            // table
                            var itemTable = await ndkRequest.reqTable(cp.cobj, cp.databaseName);
                            itemTable.forEach(item => {
                                var labels = [];
                                labels.push(item.TableName);
                                if (item.SchemaName && cp.databaseName != item.SchemaName) {
                                    labels.push(`${cp.databaseName}.${item.SchemaName}.${item.TableName}`);
                                } else {
                                    labels.push(`${cp.databaseName}.${item.TableName}`);
                                }

                                labels.map(label => result.push({
                                    label: label,
                                    kind: monaco.languages.CompletionItemKind.Text,
                                    documentation: 'table',
                                    insertText: label,
                                    range: range
                                }));
                            });

                            // column
                            var filterSNTN = itemTable.map(x => x.SchemaName == null ? x.TableName : `${x.SchemaName}.${x.TableName}`).join(',');
                            var itemColumn = await ndkRequest.reqColumn(cp.cobj, cp.databaseName, filterSNTN);
                            var objCol = {};
                            itemColumn.forEach(item => {
                                if (!(item.ColumnName in objCol)) {
                                    objCol[item.ColumnName] = 1;

                                    result.push({
                                        label: item.ColumnName,
                                        kind: monaco.languages.CompletionItemKind.Text,
                                        documentation: 'column',
                                        insertText: item.ColumnName,
                                        range: range
                                    });
                                }
                                result.push({
                                    label: `(${item.TableName}).${item.ColumnName}`,
                                    kind: monaco.languages.CompletionItemKind.Text,
                                    documentation: 'column',
                                    insertText: item.ColumnName,
                                    range: range
                                });
                            })
                        }
                    }
                }

                return { suggestions: result };
            }
        });
    },

    /**
     * 配置
     * @param {any} ops
     */
    config: ops => Object.assign({
        value: "",
        theme: ndkVary.themeGet() == "dark" ? "vs-dark" : "vs",
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
    create: async (dom, config) => {
        var editor = monaco.editor.create(dom, ndkEditor.config(config));
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
     * 保留赋值（可撤销）
     * @param {*} editor 
     * @param {*} value 
     */
    keepSetValue: (editor, value) => {
        var cpos = editor.getPosition();
        editor.executeEdits('', [{
            range: editor.getModel().getFullModelRange(),
            text: value
        }]);
        editor.setSelection(new monaco.Range(0, 0, 0, 0));
        editor.setPosition(cpos);
    },

    /**
     * 格式化SQL
     * @param {any} text
     * @param {any} type
     */
    formatterSQL: function (text, type) {
        var sqlang;
        switch (type) {
            case "SQLite":
            case "MySQL":
            case "MariaDB":
            case "PostgreSQL":
                sqlang = type.toLowerCase();
                break;
            case "Oracle": sqlang = 'plsql'; break;
            case "SQLServer":
            default: sqlang = 'tsql'; break;
        }

        return sqlFormatter.format(text, { language: sqlang });
    },

    /**
     * 设置语言
     * @param {*} editor 
     * @param {*} lang 
     */
    setLanguage: (editor, lang) => {
        monaco.editor.setModelLanguage(editor.getModel(), lang);
    },

    /**
     * 全屏切换
     * @param {any} editor
     */
    fullScreen: function (editor) {
        editor.addAction({
            id: "meid-fullscreen",
            label: ndkI18n.lg.fullScreenSwitch,
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
            label: ndkI18n.lg.wrapSwitch,
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