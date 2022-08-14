import { ndkFunction } from './ndkFunction';
import { ndkEditor } from './ndkEditor';
import { ndkVary } from './ndkVary';
import { ndkI18n } from './ndkI18n';
import { ndkExecute } from './ndkExecute';
import { ndkAction } from './ndkAction';
import { ndkGenerateCode } from './ndkGenerateCode';

// 选项卡
var ndkTab = {
    tabKeys: {},

    /**
     * 构建选项卡
     * @param {any} key
     * @param {any} title
     * @param {any} type
     */
    tabBuild: (key, title, type) => new Promise(resolve => {

        key = `tp-${key}`;
        if (!(key in ndkTab.tabKeys)) {

            var pbox = '';
            switch (type) {
                // 代码生成
                case "code":
                    {
                        pbox = `
<div class="nr-spliter3 nrc-spliter-horizontal">
    <!--Editor Code-->
    <div class="nrc-spliter-item">
        <div class="nr-gc1-tool">
            <sl-select class="nr-gc1-fn mb-2" size="small" panel="${key}" placeholder="${ndkI18n.lg.pleaseChoose}" multiple clearable></sl-select>            
            <sl-tooltip content="Debug">
                <sl-icon-button class="nr-gc1-debug mb-2" name="play-circle" label="${ndkI18n.lg.debug}" panel="${key}"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="Run">
                <sl-icon-button class="nr-gc1-run mb-2" name="play-circle-fill" label="${ndkI18n.lg.run}" panel="${key}"></sl-icon-button>
            </sl-tooltip>
        </div>
        <div class="nr-editor-gc1"></div>
    </div>

    <div class="nrc-spliter-bar"></div>
    <div class="nrc-spliter-bar-active"></div>

    <!--Editor Result-->
    <div class="nrc-spliter-item">
        <div class="nr-gc2-tool">
            <sl-select class="nr-gc2-language mb-2" size="small" panel="${key}"></sl-select>
        </div>
        <div class="nr-editor-gc2"></div>
    </div>
</div>
                        `;

                        // 构建添加对象
                        var tabbox = document.createElement("div");
                        tabbox.innerHTML = `<sl-tab slot="nav" panel="${key}" closable>${title}</sl-tab><sl-tab-panel name="${key}">${pbox}</sl-tab-panel>`;
                        var domTab = tabbox.querySelector('sl-tab'),
                            domTabPanel = tabbox.querySelector('sl-tab-panel'),

                            domTool1 = tabbox.querySelector('.nr-gc1-tool'),
                            domTool2 = tabbox.querySelector('.nr-gc2-tool'),
                            domSelectFn = tabbox.querySelector(".nr-gc1-fn"),
                            domSelectDebug = tabbox.querySelector(".nr-gc1-debug"),
                            domSelectRun = tabbox.querySelector(".nr-gc1-run"),
                            domEditor1 = tabbox.querySelector('.nr-editor-gc1'),
                            domEditor2 = tabbox.querySelector('.nr-editor-gc2'),
                            domSelectLanguage = tabbox.querySelector(".nr-gc2-language");

                        // 生成项
                        var fnhtm = [];
                        ndkFunction.groupBy(ndkGenerateCode.fns, x => x.language).forEach(language => {
                            if (fnhtm.length > 0) {
                                fnhtm.push(`<sl-divider></sl-divider>`);
                            }
                            fnhtm.push(`<sl-menu-label>${language}</sl-menu-label>`);
                            ndkGenerateCode.fns.filter(x => x.language == language).forEach(item => {
                                fnhtm.push(`<sl-menu-item value="${language}:${item.name}">${language} - ${item.name}</sl-menu-item>`);
                            });
                        });
                        domSelectFn.innerHTML = fnhtm.join('');
                        // 选择生成项
                        domSelectFn.addEventListener('sl-change', function () {
                            var tpobj = ndkTab.tabKeys[this.getAttribute('panel')];

                            if (this.value.length == 1) {
                                var vals = this.value[0].split(':');
                                var code = ndkGenerateCode.fns.find(x => x.language == vals[0] && x.name == vals[1]).code;
                                tpobj.editor1.setValue(`//${vals.join(' - ')}\r\n${code}`);
                                ndkEditor.formatter(tpobj.editor1); // 格式化代码
                            } else {
                                tpobj.editor1.setValue("");
                            }
                        });

                        // 调试代码
                        domSelectDebug.addEventListener('click', function () {
                            var tpobj = ndkTab.tabKeys[this.getAttribute('panel')];
                            var code = tpobj.editor1.getValue().trim();
                            if (code == "") {
                                ndkFunction.output(ndkI18n.lg.contentNotEmpty);
                                return;
                            }

                            //执行代码
                            var fnout = eval('(' + code + ')');
                            if (typeof fnout == 'function') {
                                fnout = fnout();
                            }

                            // 输出结果
                            if (typeof fnout == 'object') {
                                tpobj.editor2.setValue(fnout.map(x => x.content).join('\r\n\r\n\r\n'));
                                domSelectLanguage.value = tpobj.domSelectFn.value[0].split(':')[0];
                                ndkEditor.formatter(tpobj.editor2); // 格式化代码
                            } else {
                                tpobj.editor2.setValue(fnout.toString());
                            }

                            // 分离器自动
                            ndkAction.setSpliterSize(tpobj.domTabPanel.children[0], 'auto');
                        });

                        // 运行代码
                        domSelectRun.addEventListener('click', function () {
                            var tpobj = ndkTab.tabKeys[this.getAttribute('panel')];

                            if (tpobj.domSelectFn.value.length) {

                                var zip = new JSZip();

                                //遍历项
                                tpobj.domSelectFn.value.forEach(item => {
                                    var vals = item.split(':');
                                    var code = ndkGenerateCode.fns.find(x => x.language == vals[0] && x.name == vals[1]).code;

                                    //执行代码
                                    var fnout = eval('(' + code + ')');
                                    if (typeof fnout == 'function') {
                                        fnout = fnout();
                                    }

                                    // 填充到zip
                                    if (typeof fnout == 'object') {
                                        fnout.forEach(x => {
                                            //构建路径
                                            var filePath = [x.name];
                                            if (x.path != "") {
                                                filePath.unshift(x.path);
                                            }

                                            zip.file(filePath.join('/'), x.content);
                                        });
                                    } else {
                                        ndkFunction.output(fnout.toString());
                                    }
                                });

                                //下载zip
                                zip.generateAsync({ type: "blob" }).then(function (content) {
                                    ndkFunction.download(content, "code.zip");
                                });

                                ndkFunction.output(ndkI18n.lg.done);
                            } else {
                                ndkFunction.output(ndkI18n.lg.selectAnItem);
                            }
                        });

                        //绑定语言
                        var slhtm = [];
                        monaco.languages.getLanguages().map(x => x.id).sort().forEach(l => {
                            slhtm.push(`<sl-menu-item value="${l}">${l}</sl-menu-item>`);
                        })
                        domSelectLanguage.innerHTML = slhtm.join('');
                        domSelectLanguage.value = "csharp";
                        // 选择语言
                        domSelectLanguage.addEventListener('sl-change', function () {
                            var tpobj = ndkTab.tabKeys[this.getAttribute('panel')];
                            ndkEditor.setLanguage(tpobj.editor2, this.value);
                        });

                        // 添加到选项卡
                        var slpanels = ndkVary.domTabGroup2.querySelectorAll("sl-tab-panel");
                        ndkVary.domTabGroup2.insertBefore(domTab, slpanels[0]);
                        ndkVary.domTabGroup2.appendChild(domTabPanel);
                        ndkTab.tabNavFix();

                        tabbox.remove();

                        //存储
                        ndkTab.tabKeys[key] = {
                            tpkey: key,
                            type,
                            domTab,
                            domTabPanel,
                            domTool1,
                            domTool2,
                            domSelectFn,
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

                            //Ctrl + R 运行
                            editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyR, function () {

                            })
                        });

                        //构建编辑器
                        ndkEditor.create(domEditor2, { language: "csharp" }).then(editor => {
                            ndkTab.tabKeys[key].editor2 = editor;//编辑器对象

                            ndkEditor.wordWrap(editor); //换行
                            ndkEditor.fullScreen(editor); //全屏
                        });

                        //分离器显示第一项
                        ndkAction.setSpliterSize(domTabPanel.children[0], 'no');
                    }
                    break;
                // 假数据
                case "faker":
                    {
                        pbox = `
<div class="nr-spliter4 nrc-spliter-horizontal">
    <div class="nrc-spliter-item">
        Dev...
    </div>

    <div class="nrc-spliter-bar"></div>
    <div class="nrc-spliter-bar-active"></div>

    <!--Editor Result-->
    <div class="nrc-spliter-item">
        Dev...
    </div>
</div>
                        `;

                        // 构建添加对象
                        var tabbox = document.createElement("div");
                        tabbox.innerHTML = `<sl-tab slot="nav" panel="${key}" closable>${title}</sl-tab><sl-tab-panel name="${key}">${pbox}</sl-tab-panel>`;
                        var domTab = tabbox.querySelector('sl-tab'),
                            domTabPanel = tabbox.querySelector('sl-tab-panel');

                        // 添加到选项卡
                        var slpanels = ndkVary.domTabGroup2.querySelectorAll("sl-tab-panel");
                        ndkVary.domTabGroup2.insertBefore(domTab, slpanels[0]);
                        ndkVary.domTabGroup2.appendChild(domTabPanel);
                        ndkTab.tabNavFix();

                        tabbox.remove();
                    }
                    break;
                // sql
                default:
                    {
                        pbox = `
<div class="nr-spliter2 nrc-spliter-vertical">
    <!--SQL-->
    <div class="nr-editor-box nrc-spliter-item">
        <div class="nr-editor-tool" panel="${key}">
            <sl-tooltip content="${ndkI18n.lg.sqlButtonExecute} Ctrl + R" placement="right">
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
        <div class="nr-editor-sql" panel="${key}"></div>
    </div>

    <div class="nrc-spliter-bar"></div>
    <div class="nrc-spliter-bar-active"></div>

    <!--Result-->
    <div class="nrc-spliter-item">
        <sl-tab-group class="nr-tab-group-3">
        </sl-tab-group>
    </div>
</div>
                        `;

                        //构建添加对象
                        var tabbox = document.createElement("div");
                        tabbox.innerHTML = `<sl-tab slot="nav" panel="${key}" closable>${title}</sl-tab><sl-tab-panel name="${key}">${pbox}</sl-tab-panel>`;
                        var domTab = tabbox.querySelector('sl-tab'),
                            domTabPanel = tabbox.querySelector('sl-tab-panel'),
                            domEditorTool = tabbox.querySelector('.nr-editor-tool'),
                            domEditorSql = tabbox.querySelector('.nr-editor-sql'),
                            domTabGroup3 = tabbox.querySelector('.nr-tab-group-3');

                        //添加选项卡
                        var slpanels = ndkVary.domTabGroup2.querySelectorAll("sl-tab-panel");
                        ndkVary.domTabGroup2.insertBefore(domTab, slpanels[0]);
                        ndkVary.domTabGroup2.appendChild(domTabPanel);
                        ndkTab.tabNavFix();

                        tabbox.remove();

                        //存储
                        ndkTab.tabKeys[key] = {
                            tpkey: key,
                            type,
                            domTab,
                            domTabPanel,
                            domEditorTool,
                            domEditorSql,
                            domTabGroup3
                        };

                        //构建编辑器
                        ndkEditor.create(domEditorSql, { language: ndkEditor.typeAsLanguage(type) }).then(editor => {
                            ndkTab.tabKeys[key].editor = editor;//编辑器对象

                            ndkEditor.wordWrap(editor); //换行
                            ndkEditor.fullScreen(editor); //全屏

                            //Ctrl + R 执行选中或全部脚本
                            editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyR, function () {
                                var tpobj = ndkTab.tabKeys[ndkVary.domTabGroup2.activeTab.panel];
                                ndkExecute.editorSql(tpobj.tpkey);
                            })
                        });

                        //分离器显示第一项
                        ndkAction.setSpliterSize(domTabPanel.children[0], 'no');
                    }
                    break;
            }
        }

        // ndkVary.domTabGroup2.show(key); //在回调结果中调用
        resolve(key);
    }),

    /**
     * 导航修复
     */
    tabNavFix: () => {
        document.body.style.zoom = document.body.style.zoom == "1" ? 1.00001 : 1;
    }
}

export { ndkTab }