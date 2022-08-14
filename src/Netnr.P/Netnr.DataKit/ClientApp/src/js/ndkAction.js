import { ndkEditor } from "./ndkEditor";
import { ndkStep } from "./ndkStep";
import { ndkTab } from "./ndkTab";
import { ndkVary } from './ndkVary';
import { ndkView } from './ndkView';
import { ndkStorage } from "./ndkStorage";
import { ndkNoteSQL } from "./ndkNoteSQL";
import { ndkI18n } from "./ndkI18n";
import { ndkExecute } from "./ndkExecute";
import { ndkFunction } from "./ndkFunction";

// 动作
var ndkAction = {

    /**
     * 动作命令
     * @param {*} cmd
     * @param {*} args
     */
    actionRun: function (cmd, args) {
        var dval = typeof (args) == "object" ? args.getAttribute('data-val') : args;
        switch (cmd) {
            //主题
            case "theme":
                {
                    ndkVary.theme = dval;
                    ndkAction.themeSL();
                    ndkAction.themeGrid();
                    ndkAction.themeEditor();

                    ndkStep.stepSave();
                }
                break;
            //语言
            case "language":
                {
                    ndkI18n.language = dval;
                    ndkVary.domMenu.querySelectorAll('[data-cmd="language"]').forEach(item => {
                        item.checked = item.getAttribute('data-val') == ndkI18n.language;
                    });
                    //点击菜单时
                    if (typeof (args) == "object") {
                        ndkFunction.output(ndkI18n.lg.reloadDone);
                        ndkStep.stepSave();
                    }
                }
                break;
            //设置
            case "setting-manager":
                {
                    ndkVary.domDialogSetting.show();
                    //回填
                    ndkVary.domTextApiServer.value = ndkVary.apiServer;

                    var pchtm = [];
                    for (const key in ndkVary.parameterConfig) {
                        const item = ndkVary.parameterConfig[key], label = ndkI18n.lg[`parameterKey_${key}`];
                        switch (item.type) {
                            case "number":
                                pchtm.push(`<div><sl-input name="${key}" label="${label}" type="${item.type}" required placeholder="${ndkI18n.lg.default} ${item.defaultValue}" value="${item.value}"></sl-input></div>`);
                                break;
                            case "select":
                            case "boolean":
                                {
                                    pchtm.push(`<div><sl-select name="${key}" label="${label}" value="${item.value}" hoist="true">`);
                                    item.list.forEach(obj => {
                                        var txt = ndkI18n.lg[`parameterKey_${key}_${obj.val}`.replace('-', '_')];
                                        var ic = item.defaultValue == obj.val ? "class='nrc-item-default'" : "";
                                        pchtm.push(`<sl-menu-item ${ic} value="${obj.val}">${txt}</sl-menu-item>`);
                                    })
                                    pchtm.push(`</sl-select></div>`);
                                }
                                break;
                        }
                    }
                    pchtm.push(`<div><sl-button size="large" pill>${ndkVary.emoji.success}${ndkI18n.lg.save}</sl-button></div>`);
                    ndkVary.domParameterConfig.innerHTML = pchtm.join('');

                    if (ndkVary.domDialogSetting.getAttribute('event-bind') != 1) {
                        ndkVary.domDialogSetting.setAttribute('event-bind', 1);

                        //接口服务
                        ndkVary.domTextApiServer.addEventListener('input', function () {
                            ndkVary.apiServer = this.value;
                            ndkStep.stepSave();
                        }, false);
                        ndkVary.domTextApiServer.addEventListener('blur', function () {
                            if (this.value.endsWith('/')) {
                                ndkVary.apiServer = this.value.substring(0, this.value.length - 1);
                                this.value = ndkVary.apiServer;
                            }
                            ndkStep.stepSave();
                        }, false);

                        //参数保存
                        ndkVary.domParameterConfig.addEventListener('click', function (e) {
                            if (e.target.tagName == "SL-BUTTON") {
                                for (const key in ndkVary.parameterConfig) {
                                    const item = ndkVary.parameterConfig[key];
                                    let val = ndkVary.domParameterConfig.querySelector(`[name="${key}"]`).value;
                                    if (item.type == "number") {
                                        val = parseInt(val);
                                    } else if (item.type == "boolean") {
                                        val = val == "true";
                                    }
                                    item.value = val;
                                }
                                ndkStep.stepSave();
                                ndkFunction.output(ndkI18n.lg.done);
                            }
                        }, false);
                    }
                }
                break;
            case "about":
                {
                    ndkFunction.alert(`
                    <h3>NDK (Netnr.DataKit)</h3>
                    <p>${ndkI18n.lg.version}: ${ndkVary.version}</p>
                    <p><a href="https://github.com/netnr/np" target="_blank">https://github.com/netnr/np</a></p>
                    <p><a href="https://zme.ink" target="_blank">Sponsors</a></p>
                    `, '25vw');
                }
                break;
            //设置接口服务
            case "set-api-server":
                {
                    ndkVary.domTextApiServer.value = ndkVary.apiServer = dval;
                    ndkStep.stepSave();
                }
                break;
            //导出配置
            //导出配置（剪贴板）
            case "config-export":
            case "config-export-clipboard":
                {
                    //导出项
                    var keys = [], parr = [], configObj = {
                        date: ndkFunction.now(),
                        version: ndkVary.version
                    };
                    ndkVary.domConfigExportItems.querySelectorAll('sl-checkbox').forEach(item => {
                        if (item.checked) {
                            var val = item.getAttribute('data-val');
                            keys.push(val);
                            var keyo = Object.values(ndkStorage.keys).find(x => x.key == val);
                            parr.push(ndkStorage.instanceConfig.getItem(keyo.key))
                        }
                    });
                    Promise.all(parr).then(arr => {
                        for (var i = 0; i < arr.length; i++) {
                            configObj[keys[i]] = arr[i];
                        }

                        //配置加密
                        ndkAction.safeConfig(configObj, "AESEncrypt").then(() => {
                            if (cmd.endsWith("clipboard")) {
                                var content = JSON.stringify(configObj, null, 4);
                                ndkFunction.clipboard(content).then(() => {
                                    ndkFunction.output(ndkI18n.lg.done)
                                    ndkFunction.toast(ndkI18n.lg.done)
                                })
                            } else {
                                var content = JSON.stringify(configObj);
                                ndkFunction.download(content, "ndk.json");
                            }
                        });
                    })
                }
                break;
            //导入配置
            //导入配置（剪贴板）
            case "config-import":
            case "config-import-clipboard":
                {
                    try {
                        new Promise((resolve) => {
                            if (cmd.endsWith("clipboard")) {
                                ndkFunction.clipboard().then(content => {
                                    resolve(content);
                                })
                            } else {
                                resolve(args.previousElementSibling.value);
                            }
                        }).then(content => {
                            if (content.trim() == "") {
                                ndkFunction.output(ndkI18n.lg.contentNotEmpty);
                            } else {
                                var configObj = JSON.parse(content), parr = [];

                                //配置解密
                                ndkAction.safeConfig(configObj, "AESDecrypt").then(() => {
                                    var keys = Object.keys(configObj);
                                    Object.values(ndkStorage.keys).forEach(keyo => {
                                        if (keys.includes(keyo.key)) {
                                            parr.push(ndkStorage.instanceConfig.setItem(keyo.key, configObj[keyo.key]))
                                        }
                                    })

                                    Promise.all(parr).then(() => {
                                        ndkFunction.output(ndkI18n.lg.done);
                                        ndkFunction.toast(ndkI18n.lg.done)
                                        location.reload(false);
                                    })
                                });
                            }
                        });
                    } catch (e) {
                        console.debug(e)
                        ndkFunction.output(e)
                    }
                }
                break;
            //新建查询
            case "new-query":
                {
                    switch (ndkVary.domTabGroup1.activeTab.panel) {
                        case "tp1-conns":
                            {
                                //选中或范围项
                                var edata = agg.getSelectedOrRangeRow(ndkVary.gridOpsConns);

                                if (edata) {
                                    ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("plug-fill", edata.alias), 'sql').then(tpkey => {
                                        ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                        ndkStep.cpSet(tpkey, edata); //记录连接
                                        ndkStep.cpInfo(tpkey); //显示连接
                                    })
                                } else {
                                    ndkFunction.output(ndkI18n.lg.selectDataRows);
                                }
                            }
                            break;
                        case "tp1-database":
                            {
                                //选中或范围项
                                var edata = agg.getSelectedOrRangeRow(ndkVary.gridOpsDatabase);

                                if (edata) {
                                    ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("server", edata.DatabaseName), 'sql').then(tpkey => {
                                        ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                        var cp = ndkStep.cpGet(1);
                                        ndkStep.cpSet(tpkey, cp.cobj, edata.DatabaseName); //记录连接
                                        ndkStep.cpInfo(tpkey); //显示连接
                                    })
                                } else {
                                    var cp = ndkStep.cpGet(1);
                                    edata = cp.cobj;

                                    ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("plug-fill", edata.alias), 'sql').then(tpkey => {
                                        ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                        ndkStep.cpSet(tpkey, edata); //记录连接
                                        ndkStep.cpInfo(tpkey); //显示连接
                                    })
                                }
                            }
                            break;
                        case "tp1-table":
                        case "tp1-column":
                            {
                                var cp = ndkStep.cpGet(1);
                                if (cp && cp.cobj && cp.databaseName) {
                                    ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("server", cp.databaseName), 'sql').then(tpkey => {
                                        ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                        var cp = ndkStep.cpGet(1);
                                        ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName); //记录连接
                                        ndkStep.cpInfo(tpkey); //显示连接
                                    })
                                } else {
                                    ndkFunction.output(ndkI18n.lg.selectDataRows);
                                }
                            }
                            break;
                    }
                }
                break;
            //执行选中或全部脚本
            case "sql-execute-selected-or-all":
                {
                    var tpkey = args.getAttribute('panel');
                    var tpobj = ndkTab.tabKeys[tpkey];

                    ndkExecute.editorSql(tpkey)
                }
                break;
            //格式化SQL
            case "sql-formatting":
                {
                    var tpkey = args.getAttribute('panel');
                    var tpobj = ndkTab.tabKeys[tpkey];

                    ndkEditor.formatter(tpobj.editor);
                }
                break
            //生成代码
            case "quick-generate-code":
                {
                    var cp = ndkStep.cpCurrent();
                    if (cp) {
                        var edata = cp.cobj;
                        ndkTab.tabBuild(10099, ndkVary.iconSvg("file-earmark-code", ndkI18n.lg.generateCode), 'code').then(tpkey => {
                            ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                            ndkStep.cpSet(tpkey, edata); //记录连接
                            ndkStep.cpInfo(tpkey); //显示连接
                        })
                    }
                }
                break;
            //生成假数据
            case "quick-generate-faker":
                {
                    var cp = ndkStep.cpCurrent();
                    if (cp) {
                        var edata = cp.cobj;
                        ndkTab.tabBuild(10098, ndkVary.iconSvg("box", ndkI18n.lg.generateFaker), 'faker').then(tpkey => {
                            ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                            ndkStep.cpSet(tpkey, edata); //记录连接
                            ndkStep.cpInfo(tpkey); //显示连接
                        })
                    }
                }
                break;
            //数据库环境信息
            case "quick-dbenvinfo":
                {
                    var cp = ndkStep.cpCurrent();
                    if (cp) {
                        var edata = cp.cobj;
                        ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("info-circle", edata.alias), 'sql').then(tpkey => {
                            ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                            ndkStep.cpSet(tpkey, edata); //记录连接
                            ndkStep.cpInfo(tpkey); //显示连接

                            //赋值脚本
                            var sql = ndkNoteSQL[edata.type].find(x => x.name == "db-env-info").sql;
                            var tpobj = ndkTab.tabKeys[tpkey];
                            tpobj.editor.setValue(sql);

                            //执行脚本
                            ndkExecute.editorSql(tpkey);
                        })
                    }
                }
                break;
            //数据库参数信息
            case "quick-dbparamsinfo":
                {
                    var cp = ndkStep.cpCurrent();
                    if (cp) {
                        var edata = cp.cobj;
                        ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("info-circle", edata.alias), 'sql').then(tpkey => {
                            ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                            ndkStep.cpSet(tpkey, edata); //记录连接
                            ndkStep.cpInfo(tpkey); //显示连接

                            //赋值脚本
                            var sql = ndkNoteSQL[edata.type].find(x => x.name == "db-params-info").sql;
                            var tpobj = ndkTab.tabKeys[tpkey];
                            tpobj.editor.setValue(sql);

                            //执行脚本
                            ndkExecute.editorSql(tpkey);
                        })
                    }
                }
                break;
            //打开链接
            case "open-link":
                {
                    var gourl = document.createElement("a");
                    gourl.href = dval;
                    gourl.target = "_blank";
                    gourl.click();
                }
                break;
            //SQL 笔记
            case "sql-notes":
                {
                    var tpkey = args.getAttribute('panel');
                    var cp = ndkStep.cpGet(tpkey);
                    ndkView.viewExecuteSql({ Item1: { "sql-notes": ndkNoteSQL[cp.cobj.type] } }, tpkey)
                }
                break
            //SQL 历史
            case "sql-historys":
                {
                    var tpkey = args.getAttribute('panel');
                    var cp = ndkStep.cpGet(tpkey);
                    ndkStorage.historysGet().then(historys => {
                        ndkView.viewExecuteSql({ Item1: { "sql-historys": historys } }, tpkey)
                    })
                }
                break
            // 查看单元格-图片
            case "view-cell-image":
                {
                    var html = `<div style="text-align:center"><img style="max-width:100%;" src="data:image;base64,${dval}"></div>`;
                    ndkFunction.alert(html, 'full');
                }
                break;
            // 查看单元格-文本
            case "view-cell-text":
                {
                    ndkFunction.alert(`<sl-textarea></sl-textarea>`, 'full');
                    try {
                        ndkVary.domAlert.querySelector('sl-textarea').value = window.atob(dval);
                    } catch (error) {
                        ndkFunction.output(error)
                    }
                }
                break;
        }
    },

    /**
     * 设置主题（SL组件）
     */
    themeSL: () => {
        ndkVary.domMenu.querySelectorAll(`sl-menu-item[data-cmd="theme"]`).forEach(item => {
            item.checked = item.getAttribute("data-val") == ndkVary.theme;
        })
        if (ndkVary.themeGet() == "dark") {
            document.documentElement.classList.add('sl-theme-dark');
        } else {
            document.documentElement.classList.remove('sl-theme-dark');
        }
    },

    /**
     * 设置主题（表格）
     */
    themeGrid: function () {

        var setTheme = (dom) => {
            var themeName = "balham", //alpine balham material
                themeLight = `ag-theme-${themeName}`,
                themeDark = `ag-theme-${themeName}-dark`;

            dom.classList.remove(themeLight);
            dom.classList.remove(themeDark);
            switch (ndkVary.themeGet()) {
                case "dark":
                    dom.classList.add(themeDark);
                    break;
                default:
                    dom.classList.add(themeLight);
            }
        }

        for (const key in ndkVary) {
            if (key.startsWith("domGrid")) {
                var dom = ndkVary[key];
                setTheme(dom);
            }
        }

        //选项卡2窗口 表格
        ndkVary.domTabGroup2.querySelectorAll('.nr-grid-execute-sql').forEach(dom => {
            setTheme(dom);
        })

        //选项卡3执行结果 表格
        for (var i in ndkTab.tabKeys) {
            var tpkey = ndkTab.tabKeys[i];
            if (tpkey.grids) {
                tpkey.grids.forEach(grid => {
                    var dom = grid.domGridExecuteSql;
                    setTheme(dom);
                })
            }
        }
    },

    /**
     * 设置主题（编辑器）
     */
    themeEditor: function () {
        if (window.monaco) {
            switch (ndkVary.themeGet()) {
                case "dark":
                    monaco.editor.setTheme("vs-dark");
                    break;
                default:
                    monaco.editor.setTheme("vs");
            }
        }
    },

    /**
     * 设置分离器大小
     * @param {*} domSpliter 
     * @param {*} isShow 
     */
    setSpliterSize: (domSpliter, isShow = 'no') => {
        //是水平
        var isHorizontal = domSpliter.classList.contains('nrc-spliter-horizontal'),
            domSpliterChild = domSpliter.children;

        //自动显示第二项
        if (isShow == 'auto') {
            if (isHorizontal && domSpliterChild[3].clientWidth / domSpliter.clientWidth < 0.15) {
                isShow = '50%';
            } else if (!isHorizontal && domSpliterChild[3].clientHeight / domSpliter.clientHeight < 0.15) {
                isShow = '30%';
            } else {
                return false;
            }
        }

        //显示结果
        if (isShow == 'yes') {
            isShow = '30%';
        } else if (isShow == 'no') {
            isShow = `calc(100% - var(--nrc-spliter-width))`;
        }

        if (isHorizontal) {
            domSpliterChild[0].style.width = isShow;
            domSpliterChild[2].style.left = isShow;
            domSpliterChild[3].style.width = `calc(100% - var(--nrc-spliter-width) - ${isShow})`;
        } else {
            domSpliterChild[0].style.height = isShow;
            domSpliterChild[2].style.top = isShow;
            domSpliterChild[3].style.height = `calc(100% - var(--nrc-spliter-width) - ${isShow})`;
        }
    },

    /**
     * 获取分离器大小
     * @param {*} domSpliter 
     */
    getSpliterSize: (domSpliter) => {
        //是水平
        var isHorizontal = domSpliter.classList.contains('nrc-spliter-horizontal');
        if (isHorizontal) {
            var cw = parseFloat(getComputedStyle(domSpliter.firstElementChild).width),
                sw = parseFloat(getComputedStyle(domSpliter).width);
            return (cw / sw * 100).toFixed(2) + '%';
        } else {
            var ch = parseFloat(getComputedStyle(domSpliter.firstElementChild).height),
                sh = parseFloat(getComputedStyle(domSpliter).height);
            return (ch / sh * 100).toFixed(2) + '%';
        }
    },

    /**
     * 配置安全
     * @param {*} configObj 
     * @param {*} type 
     * @returns 
     */
    safeConfig: (configObj, type) => new Promise(resolve => {

        //加密
        let conns = configObj[ndkStorage.keys.keyConns.key] || [];
        let ci = 0;
        conns.forEach(item => {
            ndkFunction[type](item.conn, ndkVary.key).then(conn => {
                ci++;
                item.conn = conn
            }).catch(() => {
                ci++;
            })
        });

        ndkVary.defer.safeConfig = setInterval(() => {
            if (conns.length == ci) {
                clearInterval(ndkVary.defer.safeConfig);

                resolve(configObj);
            }
        }, 100);
    }),

    //自适应大小 (动画结束后调用)
    size: function () {
        var vh = document.documentElement.clientHeight;

        //分离容器高度
        ndkVary.domSpliter1.style.height = `${vh - ndkVary.domSpliter1.getBoundingClientRect().top - 5}px`;

        ['Conns', 'Database', 'Table', 'Column'].forEach(vkey => {
            var dom = ndkVary[`domGrid${vkey}`];
            if (getComputedStyle(dom.parentElement).display != "none") {
                var gt = dom.getBoundingClientRect().top + 5;
                dom.style.height = `${vh - gt}px`;
            }
        });

        //选项卡2窗口 执行SQL
        ndkVary.domTabGroup2.querySelectorAll('.nr-spliter2').forEach(node => {
            var pnode = node.parentElement;
            if (getComputedStyle(pnode).display != "none") {
                var gt = node.getBoundingClientRect().top + 5;
                node.style.height = `${vh - gt}px`;

                //选项卡3执行结果
                var tpkey = ndkTab.tabKeys[pnode.name];

                if (tpkey && tpkey.grids) {
                    tpkey.grids.forEach(grid => {
                        if (getComputedStyle(grid.domTabPanel).display != "none") {
                            var tableh = `${vh - grid.domGridExecuteSql.getBoundingClientRect().top - 5}px`;
                            grid.domGridExecuteSql.style.height = tableh;

                            //显示表格
                            if (!grid.gridOps && grid.opsExecuteSql) {
                                var gridOps = new agGrid.Grid(grid.domGridExecuteSql, grid.opsExecuteSql).gridOptions;

                                //快捷搜索
                                grid.domFilterExecuteSql.addEventListener('input', function () {
                                    gridOps.api.setQuickFilter(this.value);
                                }, false);

                                grid.gridOps = gridOps;
                            }
                        }
                    })
                }
            }
        });

        //选项卡2窗口 生成代码
        ndkVary.domTabGroup2.querySelectorAll('.nr-spliter3').forEach(node => {
            var pnode = node.parentElement;
            if (getComputedStyle(pnode).display != "none") {
                var gt = node.getBoundingClientRect().top + 5;
                node.style.height = `${vh - gt}px`;

                var tpkey = ndkTab.tabKeys[pnode.name];

                tpkey.domEditor1.style.height = `${vh - gt - tpkey.domTool1.clientHeight}px`;
                tpkey.domEditor2.style.height = `${vh - gt - tpkey.domTool2.clientHeight}px`;
            }
        });
    }
}

export { ndkAction };