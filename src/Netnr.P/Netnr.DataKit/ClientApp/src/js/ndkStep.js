import { ndkVary } from './ndkVary';
import { ndkI18n } from './ndkI18n';
import { ndkStorage } from './ndkStorage';
import { ndkFunction } from './ndkFunction';
import { ndkView } from './ndkView';
import { ndkTab } from './ndkTab';
import { ndkRequest } from './ndkRequest';
import { ndkAction } from './ndkAction';

// 记录
var ndkStep = {
    //连接记录
    "cp-1": {
        cobj: {},
        databaseName: null
    },
    /**
     * 所有key
     */
    cpKeys: ['1'],
    /**
     * 获取
     * @param {*} key 
     * @returns 
     */
    cpGet: key => ndkStep[`cp-${key}`],
    /**
     * 设置
     * @param {*} key 
     * @param {*} cobj 
     * @param {*} databaseName
     * @returns 
     */
    cpSet: (key, cobj, databaseName) => {
        if (!ndkStep.cpKeys.includes(key)) {
            ndkStep.cpKeys.push(key)
        }
        ndkStep[`cp-${key}`] = { cobj, databaseName }
        ndkStep.stepSave();
    },
    /**
     * 删除
     * @param {*} key 
     */
    cpRemove: key => {
        var ki = ndkStep.cpKeys.indexOf(key);
        if (ki >= 0) {
            ndkStep.cpKeys.splice(ki, 1);
        }
        delete ndkStep[`cp-${key}`];
        ndkStep.stepSave();
    },
    /**
     * 显示
     * @param {any} key
     * @param {any} cobj
     * @param {any} databaseName
     */
    cpInfo: (key) => {
        if (key == "reset") {
            ndkVary.domTabconns.style.display = "none";
            ndkVary.domTabdatabase.style.display = "none";
        } else {
            //显示的连接-数据库
            var cp = ndkStep.cpGet(key);
            if (cp != null) {
                var cobj = cp.cobj;
                //选项卡连接
                if (key != 1) {
                    //右侧选项卡连接

                    ndkVary.domTabconns.style.display = "block";
                    ndkVary.domTabdatabase.style.display = "block";

                    //设置当前选项卡
                    ndkVary.domTabconns.setAttribute("data-key", key);
                    ndkVary.domTabdatabase.setAttribute("data-cobjid", cobj.id);

                    //颜色包裹
                    var colorWrap = function (htm) {
                        var color = ndkVary.colorEnv(cp.cobj.env);
                        if (color) {
                            color = `style="color:${color};"`;
                        } else {
                            color = "";
                        }
                        return `<span ${color}>${htm}</span>`;
                    }

                    // 显示连接
                    var showConn = colorWrap(ndkVary.iconSvg(cobj.type, cobj.alias + ndkVary.iconEnv(cobj.env), { className: "align-middle me-1", library: "nr-icon" }));
                    // 显示数据库
                    var showDatabaseName = colorWrap(cp.databaseName == null ? "(none)" : ndkVary.iconSvg("server", cp.databaseName, { className: "align-middle me-1" }));

                    //连接
                    if (!ndkVary.domGridTabconns) {
                        ndkVary.domTabconns.innerHTML = `
                        <sl-button title="${cobj.conn}" slot="trigger" size="small" caret>${showConn}</sl-button>
                        <sl-menu class="px-2">
                            <div>
                                <sl-input class="nr-filter-tabconns mb-2" size="small" placeholder="${ndkI18n.lg.search}">
                                    <sl-icon name="search" slot="prefix"></sl-icon>
                                </sl-input>
                            </div>
                            <div class="nr-grid-tabconns" style="width:24em;max-width:100%;height:55vh"></div>
                        </sl-menu>
                        `;

                        ndkVary.domGridTabconns = ndkVary.domTabconns.querySelector(".nr-grid-tabconns");
                        ndkVary.domFilterTabconns = ndkVary.domTabconns.querySelector(".nr-filter-tabconns");
                        //过滤
                        ndkVary.domFilterTabconns.addEventListener("input", function () {
                            ndkVary.gridOpsTabconns.api.setQuickFilter(this.value);
                        });

                        //显示连接（检测是否重新加载表格、回填选中）
                        ndkVary.domTabconns.addEventListener("sl-show", function (event) {
                            var tabkey = ndkVary.domTabconns.getAttribute("data-key");
                            var tabcp = ndkStep.cpGet(tabkey);

                            //检测是否重新加载表格
                            if (ndkVary.gridOpsTabconns == null || ndkVary.envConnsChanged == true) {
                                ndkVary.envConnsChanged = false;

                                var opsTabconns = agg.optionDef({
                                    rowData: agg.getAllRows(ndkVary.gridOpsConns),//数据源
                                    getRowId: event => event.data.id, //指定行标识列
                                    rowGroupPanelShow: 'never',
                                    rowSelection: 'single', //单选
                                    suppressRowClickSelection: false, //点击选择行
                                    enableRangeSelection: false, //关闭选择范围
                                    suppressContextMenu: true, //禁用右键菜单
                                    headerHeight: 0,
                                    columnDefs: [
                                        {
                                            field: 'alias', headerName: ndkI18n.lg.connAlias, tooltipField: 'conn', flex: 1, checkboxSelection: true,
                                            cellRenderer: (params) => {
                                                if (!params.node.group) {
                                                    if (params.data.type) {
                                                        return ndkVary.iconSvg(params.data.type, params.value, { library: "nr-icon" });
                                                    }
                                                    return params.value
                                                }
                                            },
                                            cellStyle: params => {
                                                var denv = params.node.data && params.node.data.env;
                                                switch (denv) {
                                                    case "Test":
                                                    case "Production":
                                                        return { 'color': ndkVary.colorEnv(denv) };
                                                }
                                            }
                                        }
                                    ],
                                    // 改变连接
                                    onSelectionChanged: function () {
                                        var tabkey = ndkVary.domTabconns.getAttribute("data-key");
                                        var tabcp = ndkStep.cpGet(tabkey);
                                        var srows = ndkVary.gridOpsTabconns.api.getSelectedRows();
                                        //连接变化
                                        if (srows.length && tabcp.cobj.id != srows[0].id) {
                                            ndkStep.cpSet(tabkey, srows[0]);
                                            //更新标签名
                                            ndkTab.tabKeys[tabkey].domTab.innerHTML = ndkVary.iconSvg("plug-fill", srows[0].alias);
                                            ndkTab.tabNavFix();

                                            ndkStep.cpInfo(tabkey);
                                        }
                                    }
                                });

                                ndkView.createGrid("tabconns", opsTabconns);
                            }

                            //清空过滤
                            ndkVary.domFilterTabconns.value = "";
                            ndkVary.gridOpsTabconns.api.setQuickFilter("");

                            ndkVary.gridOpsTabconns.api.clearRangeSelection(); //清除范围选择                            
                            //选中当前
                            ndkVary.gridOpsTabconns.api.forEachNode(function (node) {
                                if (node.data.id == tabcp.cobj.id) {
                                    node.setSelected(true, true);
                                    ndkVary.gridOpsTabconns.api.ensureIndexVisible(node.rowIndex); //滚动到行显示
                                }
                            })
                        })
                    } else {
                        var btn = ndkVary.domTabconns.firstElementChild;
                        btn.title = cobj.conn;
                        btn.innerHTML = showConn;
                    }

                    //数据库
                    if (!ndkVary.domGridTabdatabase) {
                        ndkVary.domTabdatabase.innerHTML = `
                        <sl-button title="${cp.databaseName || ""}" slot="trigger" size="small" caret>${showDatabaseName}</sl-button>
                        <sl-menu class="px-2">
                            <div>
                                <sl-input class="nr-filter-tabdatabase mb-2" size="small" placeholder="${ndkI18n.lg.search}">
                                    <sl-icon name="search" slot="prefix"></sl-icon>
                                </sl-input>
                            </div>
                            <div class="nr-grid-tabdatabase" style="width:24em;max-width:100%;height:55vh"></div>
                        </sl-menu>
                        `;

                        ndkVary.domGridTabdatabase = ndkVary.domTabdatabase.querySelector(".nr-grid-tabdatabase");
                        ndkVary.domFilterTabdatabase = ndkVary.domTabdatabase.querySelector(".nr-filter-tabdatabase");
                        //过滤
                        ndkVary.domFilterTabdatabase.addEventListener("input", function () {
                            ndkVary.gridOpsTabdatabase.api.setQuickFilter(this.value);
                        });

                        //显示数据库（检测是否重新加载表格、回填选中）
                        ndkVary.domTabdatabase.addEventListener("sl-show", function (event) {

                            var tabkey = ndkVary.domTabconns.getAttribute("data-key");
                            var tabcp = ndkStep.cpGet(tabkey);
                            var gridcobjid = ndkVary.domTabdatabase.getAttribute("data-grid-cobjid");

                            new Promise((resolve, reject) => {
                                //检测是否重新加载表格
                                if (ndkVary.gridOpsTabdatabase == null || gridcobjid != tabcp.cobj.id) {
                                    ndkVary.domTabdatabase.setAttribute("data-grid-cobjid", tabcp.cobj.id);

                                    ndkRequest.reqDatabaseName(tabcp.cobj).then(rowData => {
                                        //载入选项卡数据库
                                        var opsTabdatabase = agg.optionDef({
                                            rowData: rowData,//数据源
                                            getRowId: event => event.data.DatabaseName, //指定行标识列
                                            rowGroupPanelShow: 'never',
                                            rowSelection: 'single', //单选
                                            suppressRowClickSelection: false, //点击选择行
                                            enableRangeSelection: false, //关闭选择范围
                                            suppressContextMenu: true, //禁用右键菜单
                                            headerHeight: 0,
                                            columnDefs: [
                                                {
                                                    field: 'DatabaseName', headerName: ndkI18n.lg.dbName, tooltipField: 'DatabaseName', flex: 1, checkboxSelection: true,
                                                    cellRenderer: (params) => {
                                                        return ndkVary.iconSvg("server", params.value);
                                                    }
                                                },
                                            ],
                                            // 改变数据库
                                            onSelectionChanged: function () {
                                                var tabkey = ndkVary.domTabconns.getAttribute("data-key");
                                                var tabcp = ndkStep.cpGet(tabkey);
                                                var srows = ndkVary.gridOpsTabdatabase.api.getSelectedRows();
                                                var databaseName = srows.length ? srows[0].DatabaseName : null;

                                                //数据库变化
                                                if (tabcp.databaseName != databaseName) {
                                                    ndkStep.cpSet(tabkey, tabcp.cobj, databaseName);
                                                    //更新标签名
                                                    if (databaseName) {
                                                        ndkTab.tabKeys[tabkey].domTab.innerHTML = ndkVary.iconSvg("server", srows[0].DatabaseName);
                                                    } else {
                                                        ndkTab.tabKeys[tabkey].domTab.innerHTML = ndkVary.iconSvg("plug-fill", tabcp.cobj.alias);
                                                    }
                                                    ndkTab.tabNavFix();

                                                    ndkStep.cpInfo(tabkey);
                                                }
                                            }
                                        });

                                        ndkView.createGrid("tabdatabase", opsTabdatabase);

                                        resolve();
                                    }).catch(err => {
                                        reject(err);
                                    });
                                } else {
                                    resolve();
                                }
                            }).then(() => {
                                //清空过滤
                                ndkVary.domFilterTabdatabase.value = "";
                                ndkVary.gridOpsTabdatabase.api.setQuickFilter("");

                                ndkVary.gridOpsTabdatabase.api.clearRangeSelection(); //清除范围选择
                                //选中当前
                                if (tabcp.databaseName) {
                                    ndkVary.gridOpsTabdatabase.api.forEachNode(function (node) {
                                        if (node.data.DatabaseName == tabcp.databaseName) {
                                            node.setSelected(true, true);
                                        }
                                    })
                                } else {
                                    ndkVary.gridOpsTabdatabase.api.deselectAll();
                                }
                            })
                        })
                    } else {
                        var btn = ndkVary.domTabdatabase.firstElementChild;
                        btn.title = cp.databaseName || "";
                        btn.innerHTML = showDatabaseName;
                    }
                }
            }
        }
    },

    //获取当前连接-数据库
    cpCurrent: function () {
        switch (ndkVary.domTabGroup1.activeTab.panel) {
            case "tp1-conns":
                {
                    var srows = ndkVary.gridOpsConns.api.getSelectedRows(),
                        crows = ndkVary.gridOpsConns.api.getCellRanges(), edata;
                    if (srows.length) {
                        edata = srows[0];
                    } else if (crows.length) {
                        //范围选择行
                        edata = ndkVary.gridOpsConns.api.getDisplayedRowAtIndex(crows[0].startRow.rowIndex).data
                    }

                    if (edata) {
                        return { cobj: edata };
                    } else {
                        ndkFunction.output(ndkI18n.lg.selectConn);
                    }
                }
                break;
            case "tp1-database":
                {
                    var srows = ndkVary.gridOpsDatabase.api.getSelectedRows(),
                        crows = ndkVary.gridOpsDatabase.api.getCellRanges(), edata;
                    if (srows.length) {
                        edata = srows.sort((a, b) => a.DatabaseName.localeCompare(b.DatabaseName))[0];
                    } else if (crows.length) {
                        //范围选择行
                        edata = ndkVary.gridOpsDatabase.api.getDisplayedRowAtIndex(crows[0].startRow.rowIndex).data
                    }

                    var cp = ndkStep.cpGet(1);
                    if (edata) {
                        return { cobj: cp.cobj, databaseName: edata.DatabaseName };
                    } else {
                        return cp;
                    }
                }
            case "tp1-table":
            case "tp1-column":
                {
                    var cp = ndkStep.cpGet(1);
                    return cp;
                }
        }
    },

    stepVarIng: 0,//状态（1：恢复中）
    stepVarStart: Date.now(),//恢复开始时间
    stepVarEnd: Date.now(),//恢复结束时间
    stepVarIndex: 0, //恢复索引
    stepVarDefer: null, //恢复延迟任务
    /**
     * 步骤状态
     * @param {any} ing
     */
    stepStatus: ing => {
        switch (ing) {
            //开始
            case 1:
                ndkStep.stepVarStart = Date.now();
                ndkStep.stepVarIng = ing;
                ndkStep.stepVarIndex = 0;
                break;
            //完成
            case 2:
                ndkStep.stepVarEnd = Date.now();
                ndkStep.stepVarIng = ing;
                console.debug(`step done ${ndkStep.stepVarEnd - ndkStep.stepVarStart}`);
                clearTimeout(ndkStep.stepVarDefer);
                break;
            //超时
            default:
                ndkStep.stepVarEnd = Date.now();
                ndkStep.stepVarIng = -1;
                console.debug("step timeout");
                break;
        }
    },
    /**
     * 保存步骤
     */
    stepSave: () => {
        //非恢复中
        if (ndkStep.stepVarIng != 1) {

            var sobj = {
                theme: ndkVary.theme, //主题
                language: ndkI18n.language, //语言
                spliter1Size: ndkAction.getSpliterSize(ndkVary.domSpliter1), //分离器1大小
                apiServer: ndkVary.apiServer, //api服务器
                parameterConfig: {}, //参数配置
                connCache: {}, //连接缓存
                tabGroup1Show: "tp-conns", //左侧标签组显示
                filterConns: ndkVary.domFilterConns.value, //连接过滤
                viewDatabase: ndkVary.gridOpsDatabase != null, //显示数据库
                filterDatabase: ndkVary.domFilterDatabase.value, //数据库过滤
                viewTable: ndkVary.gridOpsTable != null, //显示表
                filterTable: ndkVary.domFilterTable.value, //表过滤
                viewColumn: ndkVary.gridOpsColumn != null, //显示列
                filterColumn: ndkVary.domFilterColumn.value, //列过滤
                viewColumnTableName: [], //列信息表名
            };

            //存储参数值
            for (const key in ndkVary.parameterConfig) {
                sobj.parameterConfig[key] = ndkVary.parameterConfig[key].value;
            }

            //显示选项卡
            ndkVary.domTabGroup1.querySelectorAll('sl-tab-panel').forEach(node => {
                if (node.style.display == "block") {
                    sobj.tabGroup1Show = node.name
                }
            });

            //连接缓存
            //ndkStep.cpKeys.forEach(k => sobj.connCache[k] = ndkStep.cpGet(k));
            sobj.connCache['1'] = ndkStep.cpGet(1) || {};

            //列信息表名
            if (sobj.viewColumn) {
                sobj.viewColumnTableName = ndkFunction.groupBy(ndkVary.gridOpsColumn.rowData, x => x.TableName);
            }

            ndkStorage.stepsSet(sobj)
        }
    },
    /**
     * 步骤恢复开始
     */
    stepStart: () => new Promise(resolve => {
        if (ndkStep.stepVarIng == 1) {
            ndkFunction.output(ndkI18n.lg.inProgress);
        } else {
            ndkStep.stepStatus(1);

            ndkStep.stepVarDefer = setTimeout(() => {
                ndkStep.stepStatus(-1);
                resolve();
            }, 1000 * 30);

            ndkStorage.stepsGet().then(sobj => ndkStep.stepItemRun([
                "step-theme",
                "step-spliter1-size",
                "step-api-server",
                "step-parameter-config",
                "step-conn-cache",
                "step-view-conns",
                "step-tab-group1-show",
            ], sobj).then(() => ndkStep.stepItemRun([
                "step-view-database",
                "step-view-table",
                "step-view-column"
            ], sobj).then(() => {
                ndkStep.stepStatus(2);
                resolve()
            })));
        }
    }),
    /**
     * 步骤项运行
     * @param {any} items
     * @param {any} sobj
     */
    stepItemRun: (items, sobj) => new Promise(resolve => {
        var ics = [];
        items.forEach(item => ics.push(ndkStep.stepItemCmd(item, sobj)));
        Promise.all(ics).then(() => resolve()).catch(() => resolve())
    }),
    /**
     * 步骤项目命令（无需写入）
     * @param {*} stepItem
     * @param {*} sobj 
     * @returns 
     */
    stepItemCmd: (stepItem, sobj) => new Promise(resolve => {
        sobj = sobj || {}
        switch (stepItem) {
            //主题
            case "step-theme":
                ndkAction.actionRun('theme', sobj.theme);
                resolve();
                break;
            //分离器1大小
            case "step-spliter1-size":
                if (sobj.spliter1Size && sobj.spliter1Size != "") {
                    ndkAction.setSpliterSize(ndkVary.domSpliter1, sobj.spliter1Size);
                }
                resolve();
                break;
            //接口服务
            case "step-api-server":
                if (sobj.apiServer && sobj.apiServer != "") {
                    ndkVary.apiServer = sobj.apiServer;
                }
                resolve();
                break;
            //参数配置
            case "step-parameter-config":
                if (sobj.parameterConfig) {
                    for (const key in sobj.parameterConfig) {
                        ndkVary.parameterConfig[key].value = sobj.parameterConfig[key];
                    }
                }
                resolve();
                break;
            //连接-缓存
            case "step-conn-cache":
                for (var k in sobj.connCache) {
                    var cp = sobj.connCache[k];
                    ndkStep.cpSet(k, cp.cobj, cp.databaseName)
                }
                resolve();
                break;
            //载入连接
            case "step-view-conns":
                ndkRequest.reqConns().then(conns => {
                    ndkView.viewConns(conns).then(() => {
                        ndkStep.stepItemCmd('step-tab-group1-show', sobj)
                        var fv = (sobj.filterConns || "").trim();
                        if (fv != "") {
                            ndkVary.domFilterConns.value = fv;
                            ndkVary.gridOpsConns.api.setQuickFilter(fv);
                        }
                        resolve();
                    })
                })
                break;
            //载入库
            case "step-view-database":
                if (sobj.viewDatabase) {
                    var cp = ndkStep.cpGet(1);
                    ndkRequest.reqDatabaseName(cp.cobj).then(databases => {
                        ndkView.viewDatabase(databases).then(() => {
                            ndkStep.stepItemCmd('step-tab-group1-show', sobj)

                            var fv = (sobj.filterDatabase || "").trim();
                            if (fv != "") {
                                ndkVary.domFilterDatabase.value = fv;
                                ndkVary.gridOpsDatabase.api.setQuickFilter(fv);
                            }
                            resolve();
                        })
                    }).catch(resolve)
                } else {
                    resolve();
                }
                break;
            //载入表
            case "step-view-table":
                var cp = ndkStep.cpGet(1);
                if (sobj.viewTable && cp.databaseName != null) {
                    ndkRequest.reqTable(cp.cobj, cp.databaseName).then(tables => {
                        ndkView.viewTable(tables, cp.cobj).then(() => {
                            ndkStep.stepItemCmd('step-tab-group1-show', sobj)

                            var fv = (sobj.filterTable || "").trim();
                            if (fv != "") {
                                ndkVary.domFilterTable.value = fv;
                                ndkVary.gridOpsTable.api.setQuickFilter(fv);
                            }
                            resolve();
                        })
                    }).catch(resolve)
                } else {
                    resolve();
                }
                break;
            //载入列
            case "step-view-column":
                var cp = ndkStep.cpGet(1);
                if (sobj.viewColumn && sobj.viewColumnTableName.length && cp.databaseName != null) {
                    ndkRequest.reqColumn(cp.cobj, cp.databaseName, sobj.viewColumnTableName.join(',')).then(columns => {
                        ndkView.viewColumn(columns, cp.cobj).then(() => {
                            ndkStep.stepItemCmd('step-tab-group1-show', sobj)

                            var fv = (sobj.filterColumn || "").trim();
                            if (fv != "") {
                                ndkVary.domFilterColumn.value = fv;
                                ndkVary.gridOpsColumn.api.setQuickFilter(fv);
                            }
                            resolve();
                        })
                    }).catch(resolve)
                } else {
                    resolve();
                }
                break;
            //显示选项卡1连接
            case "step-tab-group1-show":
                if (sobj.tabGroup1Show && sobj.tabGroup1Show != "") {
                    ndkVary.domTabGroup1.show(sobj.tabGroup1Show);
                }
                resolve();
                break;
            default:
                break;
        }
    })
}

export { ndkStep }