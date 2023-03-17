import { ndkFunction } from './ndkFunction';
import { ndkEditor } from './ndkEditor';
import { ndkVary } from './ndkVary';
import { ndkI18n } from './ndkI18n';
import { ndkExecute } from './ndkExecute';
import { ndkAction } from './ndkAction';
import { ndkGenerateCode } from './ndkGenerateCode';
import { ndkStep } from './ndkStep';

// 选项卡
var ndkTab = {
    tabKeys: {},

    /**
     * 活动的选项卡连接
     * @returns 
     */
    tabActiveCp: () => {
        if (ndkVary.domTabGroupBody.activeTab) {
            return ndkStep.cpGet(ndkVary.domTabGroupBody.activeTab.panel)
        }
    },

    /**
     * 构建选项卡
     * @param {any} key
     * @param {any} title
     * @param {any} type
     */
    tabBuild: async (key, title, type) => {

        key = `tp-${key}`;
        if (!(key in ndkTab.tabKeys)) {
            ndkTab[`tabBuild_${type}`](key, title, type);
        }

        // ndkVary.domTabGroupBody.show(key); //在回调结果中调用
        return key;
    },

    /**
     * 快捷构建 SQL
     * @param {*} cp 连接信息
     * @param {*} title 标题
     * @param {*} sqlOrPromise 获取 SQL 的 Promise 
     * @param {*} isExecute 是否立即执行
     * @returns 
     */
    tabBuildFast_sql: async (cp, title, sqlOrPromise, isExecute = false) => {
        let tpkey = await ndkTab.tabBuild(ndkFunction.random(), title, ndkTab.tabType.sql);
        ndkVary.domTabGroupBody.show(tpkey);//显示选项卡
        ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName); //记录连接
        ndkStep.cpInfo(tpkey); //显示连接

        let tpobj = ndkTab.tabKeys[tpkey];
        //赋值SQL
        if (sqlOrPromise == null || typeof sqlOrPromise == "string") {
            if (sqlOrPromise == null) {
                sqlOrPromise = "";
            }

            tpobj.editor.setValue(sqlOrPromise);
            //执行脚本
            if (isExecute && sqlOrPromise.trim() != "") {
                ndkExecute.editorSql(tpkey);
            }
        } else {
            tpobj.editor.setValue(`-- ${ndkI18n.lg.generatingScript}`);
            let res = await sqlOrPromise;
            tpobj.editor.setValue(res);
            //执行脚本
            if (isExecute && res.trim() != "") {
                ndkExecute.editorSql(tpkey);
            }
        }
        return tpkey;
    },

    /**
     * 构建类型
     */
    tabType: {
        sql: "sql",
        vsql: "vsql",
        code: "code",
        faker: "faker"
    },

    /**
     * 构建 sql
     * @param {any} key
     * @param {any} title
     * @param {any} type
     */
    tabBuild_sql: (key, title, type) => {
        var pbox = `
<div class="nrg-spliter-sql nrc-spliter-vertical">
    <!--SQL-->
    <div class="nrg-editor-box nrc-spliter-item">
        <div class="nrg-editor-tool" panel="${key}">
            <sl-tooltip content="${ndkI18n.lg.sqlButtonExecute} Ctrl + Enter" placement="right">
                <sl-icon-button data-cmd="sql-execute-selected-or-all" panel="${key}" name="caret-right-fill" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="${ndkI18n.lg.sqlButtonFormat} Alt + Shift + F" placement="right">
                <sl-icon-button data-cmd="sql-formatting" panel="${key}" name="brush-fill" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="${ndkI18n.lg.sqlButtonNote}" placement="right">
                <sl-icon-button data-cmd="sql-notes" panel="${key}" name="journal-code" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="${ndkI18n.lg.sqlButtonHistory}" placement="right">
                <sl-icon-button data-cmd="sql-historys" panel="${key}" name="clock-history" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
        </div>
        <div class="nrg-editor-sql" panel="${key}"></div>
    </div>

    <div class="nrc-spliter-bar"></div>
    <div class="nrc-spliter-bar-active"></div>

    <!--Result-->
    <div class="nrc-spliter-item">
        <sl-tab-group class="nrg-tab-group-esql">
        </sl-tab-group>
    </div>
</div>`;

        //构建添加对象
        var tabbox = document.createElement("div");
        tabbox.innerHTML = `<sl-tab slot="nav" panel="${key}" closable>${title}</sl-tab><sl-tab-panel name="${key}">${pbox}</sl-tab-panel>`;
        var domTab = tabbox.querySelector('sl-tab'),
            domTabPanel = tabbox.querySelector('sl-tab-panel'),
            domEditorTool = tabbox.querySelector('.nrg-editor-tool'),
            domEditorSql = tabbox.querySelector('.nrg-editor-sql'),
            domTabGroupEsql = tabbox.querySelector('.nrg-tab-group-esql');

        //添加选项卡
        var slpanels = ndkVary.domTabGroupBody.querySelectorAll("sl-tab-panel");
        ndkVary.domTabGroupBody.insertBefore(domTab, slpanels[0]);
        ndkVary.domTabGroupBody.appendChild(domTabPanel);

        tabbox.remove();

        //存储
        ndkTab.tabKeys[key] = {
            tpkey: key,
            type,
            domTab,
            domTabPanel,
            domEditorTool,
            domEditorSql,
            domTabGroupEsql
        };

        //构建编辑器
        ndkEditor.create(domEditorSql, { language: ndkEditor.typeAsLanguage(type) }).then(editor => {
            ndkTab.tabKeys[key].editor = editor;//编辑器对象

            ndkEditor.wordWrap(editor); //换行
            ndkEditor.fullScreen(editor); //全屏

            //Ctrl + Enter 执行选中或全部脚本
            editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.Enter, function () {
                var tpobj = ndkTab.tabKeys[ndkVary.domTabGroupBody.activeTab.panel];
                ndkExecute.editorSql(tpobj.tpkey);
            })
        });

        //分离器显示第一项
        ndkAction.setSpliterSize(domTabPanel.children[0], 'no');
    },

    /**
     * 构建 vsql
     * @param {any} key
     * @param {any} title
     * @param {any} type
     */
    tabBuild_vsql: (key, title, type) => {
        var pbox = `
<div class="nrg-spliter-vsql nrc-spliter-vertical">
    <!--SQL-->
    <div class="nrg-editor-box nrc-spliter-item">
        <div class="nrg-editor-tool" panel="${key}">
            <sl-tooltip content="${ndkI18n.lg.sqlButtonExecute} Ctrl + Enter" placement="right">
                <sl-icon-button data-cmd="sql-execute-selected-or-all" panel="${key}" name="caret-right-fill" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="${ndkI18n.lg.sqlButtonFormat} Alt + Shift + F" placement="right">
                <sl-icon-button data-cmd="sql-formatting" panel="${key}" name="brush-fill" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="${ndkI18n.lg.sqlButtonNote}" placement="right">
                <sl-icon-button data-cmd="sql-notes" panel="${key}" name="journal-code" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="${ndkI18n.lg.sqlButtonHistory}" placement="right">
                <sl-icon-button data-cmd="sql-historys" panel="${key}" name="clock-history" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
        </div>
        <div class="nrg-editor-sql" panel="${key}"></div>
    </div>

    <div class="nrc-spliter-bar"></div>
    <div class="nrc-spliter-bar-active"></div>

    <!--Result-->
    <div class="nrc-spliter-item">
        <sl-tab-group class="nrg-tab-group-esql">
        </sl-tab-group>
    </div>
</div>`;

        //构建添加对象
        var tabbox = document.createElement("div");
        tabbox.innerHTML = `<sl-tab slot="nav" panel="${key}" closable>${title}</sl-tab><sl-tab-panel name="${key}">${pbox}</sl-tab-panel>`;
        var domTab = tabbox.querySelector('sl-tab'),
            domTabPanel = tabbox.querySelector('sl-tab-panel'),
            domEditorTool = tabbox.querySelector('.nrg-editor-tool'),
            domEditorSql = tabbox.querySelector('.nrg-editor-sql'),
            domTabGroupEsql = tabbox.querySelector('.nrg-tab-group-esql');

        //添加选项卡
        var slpanels = ndkVary.domTabGroupBody.querySelectorAll("sl-tab-panel");
        ndkVary.domTabGroupBody.insertBefore(domTab, slpanels[0]);
        ndkVary.domTabGroupBody.appendChild(domTabPanel);

        tabbox.remove();

        //存储
        ndkTab.tabKeys[key] = {
            tpkey: key,
            type,
            domTab,
            domTabPanel,
            domEditorTool,
            domEditorSql,
            domTabGroupEsql
        };

        //构建编辑器
        ndkEditor.create(domEditorSql, { language: ndkEditor.typeAsLanguage(type) }).then(editor => {
            ndkTab.tabKeys[key].editor = editor;//编辑器对象

            ndkEditor.wordWrap(editor); //换行
            ndkEditor.fullScreen(editor); //全屏

            //Ctrl + Enter 执行选中或全部脚本
            editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.Enter, function () {
                var tpobj = ndkTab.tabKeys[ndkVary.domTabGroupBody.activeTab.panel];
                ndkExecute.editorSql(tpobj.tpkey);
            })
        });

        //分离器显示第一项
        ndkAction.setSpliterSize(domTabPanel.children[0], 'no');
    },

    /**
     * 构建 code
     * @param {any} key
     * @param {any} title
     * @param {any} type
     */
    tabBuild_code: (key, title, type) => {
        var pbox = `
<div class="nrg-spliter-code nrc-spliter-horizontal">
    <!--Editor Code-->
    <div class="nrc-spliter-item">
        <div class="nrg-gc1-tool">
            <sl-select class="nrg-gc1-code mb-2" size="small" panel="${key}" placeholder="${ndkI18n.lg.pleaseChoose}" clearable></sl-select>
            <sl-button class="nrg-gc1-debug" variant="default" size="small">
                <sl-icon slot="prefix" name="play-circle"></sl-icon>${ndkI18n.lg.debug}
            </sl-button>
            <sl-button class="nrg-gc1-run" variant="default" size="small">
                <sl-icon slot="prefix" name="play-circle-fill"></sl-icon>${ndkI18n.lg.run}
            </sl-button>
        </div>
        <div class="nrg-editor-gc1"></div>
    </div>

    <div class="nrc-spliter-bar"></div>
    <div class="nrc-spliter-bar-active"></div>

    <!--Editor Result-->
    <div class="nrc-spliter-item">
        <div class="nrg-gc2-tool">
            <sl-select class="nrg-gc2-language mb-2" size="small" panel="${key}"></sl-select>
        </div>
        <div class="nrg-editor-gc2"></div>
    </div>
</div>`;

        // 构建添加对象
        var tabbox = document.createElement("div");
        tabbox.innerHTML = `<sl-tab slot="nav" panel="${key}" closable>${title}</sl-tab><sl-tab-panel name="${key}">${pbox}</sl-tab-panel>`;
        var domTab = tabbox.querySelector('sl-tab'),
            domTabPanel = tabbox.querySelector('sl-tab-panel'),
            domTool1 = tabbox.querySelector('.nrg-gc1-tool'),
            domTool2 = tabbox.querySelector('.nrg-gc2-tool'),
            domSelectCode = tabbox.querySelector(".nrg-gc1-code"),
            domSelectDebug = tabbox.querySelector(".nrg-gc1-debug"),
            domSelectRun = tabbox.querySelector(".nrg-gc1-run"),
            domEditor1 = tabbox.querySelector('.nrg-editor-gc1'),
            domEditor2 = tabbox.querySelector('.nrg-editor-gc2'),
            domSelectLanguage = tabbox.querySelector(".nrg-gc2-language");

        // 生成项
        ndkGenerateCode.codeContent("list.txt").then(codeList => {
            codeList = codeList.split('\n').filter(x => x != "");
            var seCodeHtml = [];
            ndkFunction.groupBy(codeList, x => x.split('_')[0]).forEach(language => {
                if (seCodeHtml.length > 0) {
                    seCodeHtml.push(`<sl-divider></sl-divider>`);
                }
                codeList.filter(x => x.startsWith(language)).forEach(item => {
                    seCodeHtml.push(`<sl-option value="${item}">${item}</sl-option>`);
                });
            });
            domSelectCode.innerHTML = seCodeHtml.join('');
        })
        // 选择代码项
        domSelectCode.addEventListener('sl-change', async function () {
            if (this.value) {
                var tpobj = ndkTab.tabKeys[domTab.panel];
                var code = await ndkGenerateCode.codeContent(this.value);
                tpobj.editor1.setValue(code);

                await codeDebugOrRun(true);
            }
        });

        var codeDebugOrRun = async (isDebug) => {
            var tpobj = ndkTab.tabKeys[domTab.panel];
            var code = tpobj.editor1.getValue().trim();
            if (code == "") {
                ndkFunction.output(ndkI18n.lg.contentNotEmpty);
            } else {
                var result = await ndkFunction.runCode(code);
                if (typeof result == "object") {
                    if (isDebug) {
                        tpobj.editor2.setValue(result.files.map(f => f.content).join('\r\n\r\n\r\n'));
                        domSelectLanguage.value = result.language || "plaintext";
                        // ndkEditor.formatter(tpobj.editor2); // 格式化代码
                    }
                    else {
                        var zip = new JSZip();
                        result.files.forEach(f => zip.file(f.fullName, f.content));
                        //下载 zip
                        var downCode = await zip.generateAsync({ type: "blob" });
                        ndkFunction.download(downCode, "code.zip");

                        ndkFunction.output(ndkI18n.lg.done);
                    }
                } else {
                    tpobj.editor2.setValue(fnEval.toString());
                }
            }

            if (isDebug) {
                // 分离器自动
                ndkAction.setSpliterSize(tpobj.domTabPanel.children[0], 'auto');
            }
        }

        // 调试代码
        domSelectDebug.addEventListener('click', async () => await codeDebugOrRun(true));
        // 运行代码并下载
        domSelectRun.addEventListener('click', async () => await codeDebugOrRun());

        //绑定语言
        var slhtm = [];
        monaco.languages.getLanguages().map(x => x.id).sort().forEach(l => {
            slhtm.push(`<sl-menu-item value="${l}">${l}</sl-menu-item>`);
        })
        domSelectLanguage.innerHTML = slhtm.join('');
        domSelectLanguage.value = "plaintext";
        // 选择语言
        domSelectLanguage.addEventListener('sl-change', function () {
            var tpobj = ndkTab.tabKeys[this.getAttribute('panel')];
            ndkEditor.setLanguage(tpobj.editor2, this.value);
        });

        // 添加到选项卡
        var slpanels = ndkVary.domTabGroupBody.querySelectorAll("sl-tab-panel");
        ndkVary.domTabGroupBody.insertBefore(domTab, slpanels[0]);
        ndkVary.domTabGroupBody.appendChild(domTabPanel);

        tabbox.remove();

        //存储
        ndkTab.tabKeys[key] = {
            tpkey: key,
            type,
            domTab,
            domTabPanel,
            domTool1,
            domTool2,
            domSelectFn: domSelectCode,
            domSelectSave: domSelectRun,
            domEditor1,
            domEditor2,
            domSelectLanguage
        };

        //构建编辑器
        ndkEditor.create(domEditor1, { language: "javascript" }).then(editor => {
            ndkTab.tabKeys[key].editor1 = editor;//编辑器对象

            ndkEditor.wordWrap(editor); //换行
            ndkEditor.fullScreen(editor); //全屏

            //Pause Break 调式
            editor.addCommand(monaco.KeyCode.PauseBreak, async () => await codeDebugOrRun(true));
        });

        //构建编辑器
        ndkEditor.create(domEditor2, { language: "csharp" }).then(editor => {
            ndkTab.tabKeys[key].editor2 = editor;//编辑器对象

            ndkEditor.wordWrap(editor); //换行
            ndkEditor.fullScreen(editor); //全屏
        });

        //分离器显示第一项
        ndkAction.setSpliterSize(domTabPanel.children[0], 'no');
    },

    /**
     * 构建 faker
     * @param {any} key
     * @param {any} title
     * @param {any} type
     */
    tabBuild_faker: (key, title, type) => {
        var pbox = `
<div class="nrg-spliter-faker nrc-spliter-horizontal">
    <div class="nrc-spliter-item">
        Dev...
    </div>

    <div class="nrc-spliter-bar"></div>
    <div class="nrc-spliter-bar-active"></div>

    <!--Editor Result-->
    <div class="nrc-spliter-item">
        Dev...
    </div>
</div>`;

        // 构建添加对象
        var tabbox = document.createElement("div");
        tabbox.innerHTML = `<sl-tab slot="nav" panel="${key}" closable>${title}</sl-tab><sl-tab-panel name="${key}">${pbox}</sl-tab-panel>`;
        var domTab = tabbox.querySelector('sl-tab'),
            domTabPanel = tabbox.querySelector('sl-tab-panel');

        // 添加到选项卡
        var slpanels = ndkVary.domTabGroupBody.querySelectorAll("sl-tab-panel");
        ndkVary.domTabGroupBody.insertBefore(domTab, slpanels[0]);
        ndkVary.domTabGroupBody.appendChild(domTabPanel);

        tabbox.remove();
    }
}

export { ndkTab }