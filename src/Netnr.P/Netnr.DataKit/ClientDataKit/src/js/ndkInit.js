import { agg } from './agg';
import { ndkI18n } from './ndkI18n';
import { ndkVary } from './ndkVary'
import { ndkFunction } from './ndkFunction'
import { ndkTab } from './ndkTab'
import { ndkStep } from './ndkStep'
import { ndkStorage } from './ndkStorage';
import { ndkEditor } from './ndkEditor';
import { ndkAction } from './ndkAction';
import { ndkRequest } from './ndkRequest';

// 初始化
var ndkInit = {

    dom: () => {
        var main = document.createElement('div');
        main.className = "nr-main";
        main.style.visibility = "hidden";

        //接口服务
        var htmlApiServer = [`<sl-input class="nr-text-api-server mb-2" placeholder="${ndkI18n.lg.setServerPlaceholder}"></sl-input>`];
        ndkVary.resApiServer.forEach(item => {
            htmlApiServer.push(`
            <sl-tooltip content="${item.remark ?? item.key}">
                <sl-button class="mb-2" size="small" data-cmd="set-api-server" data-val="${item.key}" >${item.key}</sl-button>
            </sl-tooltip>
            `);
        });

        //主题项
        var htmlThemeItems = [];
        ndkVary.resTheme.forEach(item => {
            htmlThemeItems.push(`<sl-menu-item data-cmd="theme" data-val="${item.key}">${ndkVary.iconSvg(item.icon, ndkI18n.lg[`themeKey_${item.key}`], { slot: "prefix" })}</sl-menu-item>`);
        });

        //语言项
        var htmlLanguageItems = [];
        ndkI18n.resLanguage.forEach(item => {
            htmlLanguageItems.push(`<sl-menu-item data-cmd="language" data-val="${item.key}">${ndkVary.iconSvg(item.icon, ndkI18n.lg[`languageKey_${item.key.replace('-', '_')}`], { slot: "prefix" })}</sl-menu-item>`);
        });

        //配置导出
        var htmlConfigExport = [];
        htmlConfigExport.push('<div class="nr-config-export-items mb-2">');
        for (const keyo in ndkStorage.keys) {
            var val = ndkStorage.keys[keyo].key;
            var keyLabel = ndkI18n.lg[`storageKey_${val}`];
            htmlConfigExport.push(`<sl-checkbox checked class="me-3" data-val="${val}">${keyLabel}</sl-checkbox>`);
        }
        htmlConfigExport.push('</div>');
        htmlConfigExport.push(`
        <sl-button type="default" class="mb-2" data-cmd="config-export">${ndkVary.emoji.save}${ndkI18n.lg.setExportSave}</sl-button>
        <sl-button type="default" class="mb-2" data-cmd="config-export-clipboard">${ndkVary.emoji.clipboard}${ndkI18n.lg.setExportSaveClipboard}</sl-button>
        `);

        //配置导入
        var htmlConfigImport = [];
        htmlConfigImport.push(`
        <sl-textarea label="ndk.json" class="mb-2" placeholder="${ndkI18n.lg.setImportPlaceholder}"></sl-textarea>
        <sl-button type="default" data-cmd="config-import">${ndkVary.emoji.save}${ndkI18n.lg.setImportSave}</sl-button>
        `);
        if (ndkFunction.supportClipboard) {
            htmlConfigImport.push(`<sl-button type="default" data-cmd="config-import-clipboard">${ndkVary.emoji.clipboard}${ndkI18n.lg.setImportSaveClipboard}</sl-button>`);
        }

        main.innerHTML = `
        <!--顶部-->
        <div class="nr-box-head">
            <!--菜单-->
            <sl-dropdown class="nr-menu mb-2">
                <sl-button type="default" slot="trigger" size="small" caret class="nr-request-status">${ndkVary.emoji.menu}${ndkI18n.lg.menu}</sl-button>
                <sl-menu>
                    <sl-menu-item data-cmd="setting-manager">
                        ${ndkVary.iconSvg("gear", ndkI18n.lg.setting, { slot: "prefix" })}
                    </sl-menu-item>
                    <sl-divider></sl-divider>
                    ${htmlThemeItems.join('\n')}
                    <sl-divider></sl-divider>
                    ${htmlLanguageItems.join('\n')}
                    <sl-divider></sl-divider>
                    <sl-menu-item data-cmd="about">
                        ${ndkVary.iconSvg("info-circle", ndkI18n.lg.about, { slot: "prefix" })}
                    </sl-menu-item>
                </sl-menu>
            </sl-dropdown>

            <!--设置弹窗-->
            <sl-dialog label="${ndkVary.emoji.cog}${ndkI18n.lg.setTitle}" class="nr-dialog-setting dialog-width" style="--width: 70vw;">
                <sl-details summary="${ndkVary.emoji.server}${ndkI18n.lg.setServerTitle}" open>
                    ${htmlApiServer.join('\n')}
                </sl-details>
                <sl-details summary="${ndkVary.emoji.parameter}${ndkI18n.lg.setParameterConfigTitle}" open>
                    <div class="nr-parameter-config"></div>
                </sl-details>
                <sl-details summary="${ndkVary.emoji.io}${ndkI18n.lg.setExportTitle}" open>
                    ${htmlConfigExport.join('\n')}                    
                    <sl-divider></sl-divider>
                    ${htmlConfigImport.join('\n')}                    
                </sl-details>
            </sl-dialog>

            <!--新建查询-->
            <sl-button class="mb-2" data-cmd="new-query" size="small">${ndkVary.emoji.comment}${ndkI18n.lg.newQuery}</sl-button>

            <sl-divider vertical></sl-divider>
            
            <!--快捷（数据库）-->
            <sl-dropdown class="nr-quick-db mb-2">
                <sl-button slot="trigger" size="small" caret>${ndkVary.emoji.quick}${ndkI18n.lg.quick}</sl-button>
                <sl-menu>
                    <sl-menu-item data-cmd="quick-dbenvinfo">
                        ${ndkVary.iconSvg("info-circle", ndkI18n.lg.dbEnvInfo, { slot: "prefix" })}
                    </sl-menu-item>
                    <sl-menu-item data-cmd="quick-dbparamsinfo">
                        ${ndkVary.iconSvg("info-circle", ndkI18n.lg.dbParamsInfo, { slot: "prefix" })}
                    </sl-menu-item>
                    <sl-divider></sl-divider>
                    <sl-menu-item data-cmd="quick-generate-code">
                        ${ndkVary.iconSvg("file-earmark-code", ndkI18n.lg.generateCode, { slot: "prefix" })}
                    </sl-menu-item>
                    <sl-menu-item data-cmd="quick-generate-faker">
                        ${ndkVary.iconSvg("box", ndkI18n.lg.generateFaker, { slot: "prefix" })}
                    </sl-menu-item>
                    <sl-divider></sl-divider>
                    <sl-menu-item data-cmd="open-link" data-val="https://www.connectionstrings.com">
                        ${ndkVary.iconSvg("box-arrow-up-right", "ConnectionString", { slot: "prefix" })}
                    </sl-menu-item>
                    <sl-menu-item data-cmd="open-link" data-val="https://database.guide">
                        ${ndkVary.iconSvg("box-arrow-up-right", "Guide", { slot: "prefix" })}
                    </sl-menu-item>
                </sl-menu>
            </sl-dropdown>

            <!--快捷（应用）-->
            <sl-dropdown class="nr-quick-app mb-2">
                <sl-button slot="trigger" size="small" caret>${ndkVary.emoji.package}${ndkI18n.lg.package}</sl-button>
                <sl-menu>
                    <sl-menu-item data-cmd="quick-notes">
                        ${ndkVary.iconSvg("journal-code", ndkI18n.lg.note, { slot: "prefix" })}
                    </sl-menu-item>
                    <sl-menu-item data-cmd="quick-links">
                        ${ndkVary.iconSvg("link-45deg", ndkI18n.lg.link, { slot: "prefix" })}
                    </sl-menu-item>
                </sl-menu>
            </sl-dropdown>

            <sl-divider vertical></sl-divider>
            
            <!--当前选项卡连接信息-->
            <sl-dropdown class="nr-tabconns mb-2"></sl-dropdown>
            <sl-dropdown class="nr-tabdatabase mb-2"></sl-dropdown>

            <div>
                <!--输出-->
                <div class="nr-output">
                    <sl-icon-button slot="trigger" name="info-circle"></sl-icon-button>
                    <div class="nr-output-body">
                        <sl-input class="nr-filter-output mb-2" size="small" placeholder="${ndkI18n.lg.search}">
                            <sl-icon name="search" slot="prefix"></sl-icon>
                        </sl-input>
                        <div class="nr-grid-output" style="width:40em;max-width:100%;height:30vh"></div>

                        <!--队列-->
                        <div class="nr-grid-queue" style="width:40em;max-width:100%;height:5em;margin-top:0.3em"></div>
                    </div>
                </div>
            </div>

        </div>

        <!--分离器主体-->
        <div class="nr-spliter-body nrc-spliter-horizontal">

            <!--主体 左-->
            <div class="nr-box-left nrc-spliter-item">
                <!--选项卡-->
                <sl-tab-group class="nr-tab-group-tree">
                    <sl-tab slot="nav" panel="tp1-conns">${ndkVary.iconSvg("plug-fill", ndkI18n.lg.tab1Conns)}</sl-tab>
                    <sl-tab slot="nav" panel="tp1-trunk" style="display:none">${ndkVary.iconSvg("diagram-3-fill", "Trunk")}</sl-tab>
                    <sl-tab slot="nav" panel="tp1-database">${ndkVary.iconSvg("server", ndkI18n.lg.tab1Database)}</sl-tab>
                    <sl-tab slot="nav" panel="tp1-table">${ndkVary.iconSvg("table", ndkI18n.lg.tab1Table)}</sl-tab>
                    <sl-tab slot="nav" panel="tp1-column">${ndkVary.iconSvg("bookshelf", ndkI18n.lg.tab1Column)}</sl-tab>

                    <sl-tab-panel name="tp1-conns">
                        <sl-input class="nr-filter-conns mb-2" placeholder="${ndkI18n.lg.search}" size="small">
                            <sl-icon name="search" slot="prefix"></sl-icon>
                        </sl-input>
                        <div class="nr-grid-conns"></div>
                    </sl-tab-panel>
                    <sl-tab-panel name="tp1-trunk" style="display:none">
                        <sl-input class="nr-filter-trunk mb-2" placeholder="${ndkI18n.lg.search}" size="small">
                            <sl-icon name="search" slot="prefix"></sl-icon>
                        </sl-input>
                        <div class="nr-grid-trunk"></div>
                    </sl-tab-panel>
                    <sl-tab-panel name="tp1-database">
                        <sl-input class="nr-filter-database mb-2" placeholder="${ndkI18n.lg.search}" size="small">
                            <sl-icon name="search" slot="prefix"></sl-icon>
                        </sl-input>
                        <div class="nr-grid-database"></div>
                    </sl-tab-panel>
                    <sl-tab-panel name="tp1-table">
                        <sl-input class="nr-filter-table mb-2" placeholder="${ndkI18n.lg.search}" size="small">
                            <sl-icon name="search" slot="prefix"></sl-icon>
                        </sl-input>
                        <div class="nr-grid-table"></div>
                    </sl-tab-panel>
                    <sl-tab-panel name="tp1-column">
                        <sl-input class="nr-filter-column mb-2" placeholder="${ndkI18n.lg.search}" size="small">
                            <sl-icon name="search" slot="prefix"></sl-icon>
                        </sl-input>
                        <div class="nr-grid-column"></div>
                    </sl-tab-panel>
                </sl-tab-group>
            </div>
            <div class="nrc-spliter-bar"></div>
            <div class="nrc-spliter-bar-active"></div>

            <!--主体 右-->
            <div class="nr-box-right nrc-spliter-item">
                <sl-tab-group class="nr-tab-group-body">
                </sl-tab-group>
            </div>
        </div>
        `;

        document.body.appendChild(main);
    },

    //事件初始化
    event: function () {

        //菜单项事件
        document.body.addEventListener('click', function (e) {
            var cmd = e.target.getAttribute("data-cmd")
            if (cmd != null) {
                ndkAction.actionRun(cmd, e.target);
            }
        }, false);

        //分离器-调整大小
        document.body.addEventListener('mousedown', function (edown) {
            if (edown.target.classList.contains('nrc-spliter-bar')) {
                var domSpliter = edown.target.parentElement,
                    domSpliterChild = domSpliter.children,
                    domSpliterItem1 = domSpliterChild[0],
                    domSpliterActiveBar = domSpliterChild[2],
                    fnmove, fnup;

                //水平
                if (edown.target.parentElement.classList.contains('nrc-spliter-horizontal')) {
                    fnmove = function (emove) {
                        var cw = domSpliterItem1.clientWidth - (edown.clientX - emove.clientX);
                        cw = Math.max(0, cw);
                        cw = Math.min(domSpliter.clientWidth - domSpliterActiveBar.clientWidth, cw);

                        domSpliterActiveBar.style.display = 'block';
                        domSpliterActiveBar.style.left = `${cw}px`;
                    };
                    fnup = function () {
                        domSpliterActiveBar.style.display = 'none';

                        var psize = domSpliterActiveBar.style.left;
                        if (!psize.includes("%")) {
                            psize = (parseInt(domSpliterActiveBar.style.left || domSpliterItem1.clientWidth) / domSpliter.clientWidth * 100).toFixed(4) + '%'; //占比
                        }
                        ndkAction.setSpliterSize(domSpliter, psize);

                        window.removeEventListener('mousemove', fnmove)
                        window.removeEventListener('mouseup', fnup)

                        this.releaseCapture && this.releaseCapture();
                    };
                } else {
                    fnmove = function (emove) {
                        var ch = domSpliterItem1.clientHeight - (edown.clientY - emove.clientY);
                        ch = Math.max(0, ch);
                        ch = Math.min(domSpliter.clientHeight - domSpliterActiveBar.clientHeight, ch);

                        domSpliterActiveBar.style.display = 'block';
                        domSpliterActiveBar.style.top = `${ch}px`;
                    };
                    fnup = function () {
                        domSpliterActiveBar.style.display = 'none';

                        var psize = domSpliterActiveBar.style.top;
                        if (!psize.includes("%")) {
                            psize = (parseInt(domSpliterActiveBar.style.top || domSpliterItem1.clientHeight) / domSpliter.clientHeight * 100).toFixed(4) + '%'; //占比
                        }
                        domSpliterActiveBar.style.top = psize;
                        domSpliterItem1.style.height = psize;

                        domSpliterActiveBar.nextElementSibling.style.height = `calc(100% - var(--nrc-spliter-width) - ${psize})`;
                        ndkAction.size();

                        window.removeEventListener('mousemove', fnmove)
                        window.removeEventListener('mouseup', fnup)

                        this.releaseCapture && this.releaseCapture();
                    };
                }

                window.addEventListener('mousemove', fnmove, false);
                window.addEventListener('mouseup', fnup, false);

                this.setCapture && this.setCapture();
            }
        }, false);
        //分离器-双击
        document.body.addEventListener('dblclick', function (event) {
            if (event.target.classList.contains('nrc-spliter-bar')) {
                var domSpliter = event.target.parentElement, psize;
                //水平
                if (domSpliter.classList.contains('nrc-spliter-horizontal')) {
                    psize = (event.clientY - event.target.getBoundingClientRect().top) / event.target.clientHeight;
                } else {
                    psize = (event.clientX - event.target.getBoundingClientRect().left) / event.target.clientWidth;
                }

                if (psize < 0.33333) {
                    ndkAction.setSpliterSize(domSpliter, '82%');
                } else if (psize > 0.33333 && psize < 0.66666) {
                    ndkAction.setSpliterSize(domSpliter, '50%');
                } else {
                    ndkAction.setSpliterSize(domSpliter, '18%');
                }
            }
        });

        //分离器-动画
        document.body.addEventListener('transitionend', function (event) {
            if (event.target.classList.contains('nrc-spliter-item')) {
                if (['width', 'height'].indexOf(event.propertyName) > -1) {
                    var domSpliter = event.target.parentElement;

                    //需要保存
                    if (domSpliter.classList.contains("nr-spliter-body")) {
                        ndkStep.stepSave()
                    }

                    //需要调整大小
                    if (domSpliter.classList.contains("nr-spliter-sql")) {
                        ndkAction.size()
                    }
                }
            }
        });

        //选项卡 连接-连接、库、表、列 面板切换调整大小
        ndkVary.domTabGroupTree.addEventListener('sl-tab-show', function () {
            new Promise(resolve => resolve()).then(() => {
                ndkAction.size()
                ndkStep.stepSave()
            })
        }, false);
        //搜索-连接、库、表、列
        ['conns', 'database', 'table', 'column'].forEach(vkey => {
            vkey = ndkFunction.hump(vkey);
            ndkVary[`domFilter${vkey}`].addEventListener('input', function (event) {
                clearTimeout(event.target.si);
                event.target.si = setTimeout(() => {
                    var gridOps = ndkVary[`gridOps${vkey}`];
                    if (gridOps && gridOps.api) {
                        gridOps.api.setQuickFilter(this.value);
                        ndkStep.stepSave();
                    }
                }, 200)
            })
        })

        //选项卡（右侧主体）窗口-关闭
        ndkVary.domTabGroupBody.addEventListener('sl-close', async event => {
            var sltab = event.target;
            var panel = ndkVary.domTabGroupBody.querySelector(`sl-tab-panel[name="${sltab.panel}"]`);

            if (sltab.active) {
                var otab = sltab.previousElementSibling || sltab.nextElementSibling;
                if (otab.nodeName != "sl-tab-panel".toUpperCase()) {
                    ndkVary.domTabGroupBody.show(otab.panel);
                } else {
                    ndkStep.cpInfo("reset"); //重置连接信息
                }
            }

            sltab.remove();
            panel.remove();
            ndkTab.tabNavFix();

            //删除key
            delete ndkTab.tabKeys[sltab.panel];
            ndkStep.cpRemove(sltab.panel)
        });
        //选项卡 窗口-显示
        ndkVary.domTabGroupBody.addEventListener('sl-tab-show', function (event) {
            if (event.target.classList.contains("nr-tab-group-body")) {
                ndkStep.cpInfo(event.detail.name); //显示连接                
            }
            new Promise(resolve => resolve()).then(() => ndkAction.size());
        }, false);

        // 输出切换
        ndkVary.domOutput.firstElementChild.addEventListener('click', function () {
            ndkVary.domOutputBody.style.display = ndkVary.domOutputBody.style.display == "block" ? "none" : "block";
        }, false);
        // 输出鼠标移入
        ndkVary.domOutputBody.addEventListener('mouseenter', function () {
            clearTimeout(ndkVary.defer.outputAutoHide);
        }, false);
        // 输出鼠标移出
        ndkVary.domOutputBody.addEventListener('mouseleave', function () {
            ndkVary.defer.outputAutoHide = setTimeout(() => {
                ndkVary.domOutputBody.style.display = "none";
            }, 1000 * 6);
        }, false);
        // 输出初始化
        if (ndkVary.gridOpsOutput == null) {
            var opsQueue = agg.optionDef({
                rowData: [],
                defaultColDef: agg.defaultColDef(),
                columnDefs: [
                    { field: 'content', flex: 1 },
                ],
                headerHeight: 0,
                rowGroupPanelShow: 'never',
                // 单元格按键
                onCellKeyDown: function (event) {
                    //全屏切换
                    if (event.event.ctrlKey && event.event.altKey && event.event.code == "KeyM") {
                        ndkVary.domGridOutput.classList.toggle('nrc-fullscreen');
                        ndkAction.size();
                    }
                },
                getContextMenuItems: () => {
                    var result = [
                        {
                            name: "清空", icon: ndkVary.iconSvg("trash"), action: function () {
                                ndkVary.gridOpsOutput.rowData = [];
                                ndkVary.gridOpsOutput.api.setRowData([]);
                            }
                        },
                        {
                            name: ndkI18n.lg.save, icon: ndkVary.iconGrid('save'),
                            subMenu: [
                                'csvExport',
                                'excelExport'
                            ]
                        },
                        'copy',
                        'separator',
                        {
                            name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Ctrl+Alt+M',
                            action: function () {
                                ndkVary.domGridOutput.classList.toggle('nrc-fullscreen');
                            }
                        },
                    ];

                    return result;
                },
            });

            ndkView.createGrid("output", opsQueue);

            //输出搜索
            ndkVary.domFilterOutput.addEventListener('input', function () {
                ndkVary.gridOpsOutput.api.setQuickFilter(this.value);
            }, false);
        }
        // 请求队列初始化
        if (ndkVary.gridOpsQueue == null) {
            var opsQueue = agg.optionDef({
                rowData: [],
                defaultColDef: agg.defaultColDef(),
                columnDefs: [
                    { field: 'cancel', cellRenderer: () => ndkVary.emoji.loading, width: 30 },
                    { field: 'abortCtrl', flex: 1, cellRenderer: (params) => `${ndkI18n.lg.inProgress} &nbsp; ${params.data.date}` },
                ],
                headerHeight: 0,
                rowGroupPanelShow: 'never',
                // 单元格点击
                onCellClicked: function (event) {
                    //取消请求
                    if (event.column.colId == "cancel") {
                        ndkRequest.statusInfo.queueCancel(event.data);
                    }
                }
            });

            ndkView.createGrid("queue", opsQueue);
        }

        //全局错误日志
        window.addEventListener('error', function (event) {
            console.debug(event);
            if (event.error) {
                var eobj = { message: event.error.message, stack: event.error.stack };
                ndkStorage.errorsAdd(eobj); //记录错误日志
                ndkFunction.notify(ndkI18n.lg.notify, { body: eobj.message, icon: "favicon.ico", tag: "error" })
            }
        });

        //浏览器变更主题
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => {
            if (ndkVary.theme == "auto") {
                let theme = event.matches ? "dark" : "light";
                console.debug(`Change the theme to ${theme}`);

                ndkAction.actionRun("theme", 'auto');
            }
        });

        //拖拽文件读取内容
        document.addEventListener('dragover', function (e) {
            e.stopPropagation();
            e.preventDefault();
        });
        document.addEventListener('drop', function (e) {
            e.stopPropagation();
            e.preventDefault();

            var files = e.dataTransfer.files;

            var file = files[0];
            if (file) {
                ndkVary.fileObject = file;
                ndkVary.fileContent = null;

                //是文本
                if (["text", "json"].filter(x => file.type.includes(x)).length > 0 || [".sql", ".md"].filter(x => file.name.endsWith(x)).length > 0) {
                    ndkFunction.readFileContent(file, ndkVary.parameterConfig.readFileEncoding.value).then(content => {
                        ndkVary.fileContent = content;

                        if (e.target.nodeName.includes("TEXTAREA")) {
                            ndkVary.fileEditor = e.target;
                            ndkVary.fileEditor.value = content;
                        } else {
                            document.querySelectorAll(".monaco-editor").forEach(domEditor => {
                                if (domEditor.contains(e.target)) {
                                    var editor = monaco.editor.getModels().find(v => v.uri == domEditor.dataset.uri);
                                    if (editor) {
                                        ndkVary.fileEditor = editor;
                                        editor.setValue(content);
                                    }
                                }
                            })
                        }
                    }).catch(err => {
                        ndkFunction.output(err)
                    });
                } else {
                    ndkFunction.output(ndkI18n.lg.onlyTextFile);
                }
            }
        });

        //粘贴读取内容
        document.addEventListener('paste', function (e) {
            var content = e.clipboardData.getData('text/plain');

            ndkVary.pasteContent = content;
        });

        //调整窗口大小
        window.addEventListener('resize', () => ndkAction.size(), false);
    },
}

window.addEventListener("DOMContentLoaded", function () {
    agg.lk();
    ndkStorage.init(window["localforage"]);

    // 初始化语言
    ndkStorage.stepsGet().then(sobj => {
        if (sobj && sobj.language) {
            ndkI18n.language = sobj.language;
        }

        // 初始化dom
        ndkInit.dom();
        //dom对象
        document.querySelectorAll('*').forEach(node => {
            if (node.classList.value.startsWith('nr-')) {
                var vkey = 'dom';
                node.classList[0].substring(3).split('-').forEach(c => vkey += c.substring(0, 1).toUpperCase() + c.substring(1))
                ndkVary[vkey] = node;
            }
        });
        // 语言选中
        ndkAction.actionRun('language', ndkI18n.language);

        // 编辑器初始化
        ndkEditor.init(window["meRequire"]);

        //事件
        ndkInit.event();
        //步骤恢复
        ndkStep.stepStart().then(() => {
            ndkVary.domLoading.style.display = "none";
            ndkVary.domMain.style.visibility = "visible";

            //setTimeout(() => console.clear(), 1000 * 2);
        })
    });
}, false);

export { ndkInit }