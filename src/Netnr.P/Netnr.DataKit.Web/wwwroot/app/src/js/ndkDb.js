import { agg } from './agg';
import { ndkI18n } from './ndkI18n';
import { ndkFn } from './ndkFn';
import { ndkLs } from './ndkLs';
import { ndkVary } from './ndkVary';
import { ndkStep } from './ndkStep';
import { ndkTab } from './ndkTab';
import { ndkBuild } from './ndkBuild';
import { ndkEditor } from './ndkEditor';

var ndkDb = {

    /**
     * 参数拼接
     * @param {*} pars 
     * @returns 
     */
    parameterJoin: (pars) => {
        var arr = [];
        for (var i in pars) {
            arr.push(`${i}=${encodeURIComponent(pars[i])}`);
        }
        return arr.join('&');
    },

    /**
     * 请求
     * @param {*} url 
     * @param {*} options 
     * @returns 
     */
    request: (url, options) => new Promise((resolve, reject) => {
        ndkFn.requestStatus(true);
        fetch(url, Object.assign({ method: "GET" }, options)).then(x => x.json()).then(res => {
            console.debug(res);
            ndkFn.requestStatus(false);
            if (res.code == 200) {
                resolve(res);
            } else {
                ndkFn.msg(res.msg);
                reject(res);
            }
        }).catch(err => {
            ndkFn.confirm(`${err}<sl-divider></sl-divider><code>${url}</code>`);
            ndkFn.requestStatus(false);
            reject(err);
        });
    }),

    /**
     * 创建
     * @param {*} vkey 接收对象key
     * @param {*} gridOps 表格配置
     */
    createGrid: (vkey, gridOps) => {
        ndkDb.removeGrid(vkey);

        vkey = ndkFn.fu(vkey);
        var vgrid = `gridOps${vkey}`, vdom = `domGrid${vkey}`;
        ndkFn.themeGrid(ndkVary.theme);
        ndkFn.size();
        ndkVary[vgrid] = new agGrid.Grid(ndkVary[vdom], gridOps).gridOptions;
    },
    /**
     * 移除
     * @param {any} vkey
     */
    removeGrid: vkey => {
        vkey = ndkFn.fu(vkey);
        var vgrid = `gridOps${vkey}`;
        if (ndkVary[vgrid] && ndkVary[vgrid].api) {
            ndkVary[`domFilter${vkey}`].value = "";
            ndkVary[vgrid].api.destroy();
            ndkVary[vgrid] = null;
        }
    },

    // 表格组件
    gridComponents: {
        // 连接状态
        connStatusBarComponent: class {
            init(params) {
                this.params = params;

                var cp = ndkStep.cpGet(1);
                var color = ndkVary.colorEnv(cp.cobj.env);
                if (color) {
                    color = `color:${color};`
                } else {
                    color = "";
                }
                var ihtm = ndkVary.iconSvg(cp.cobj.type, "nr-svg-typedb") + cp.cobj.alias;
                if (cp.databaseName) {
                    ihtm += " &nbsp; " + ndkVary.iconSvg("database", "nr-svg-typedb") + cp.databaseName;
                }

                this.eGui = document.createElement('div');
                this.eGui.innerHTML = `<sl-tooltip content="${cp.cobj.conn}"><small style="white-space: nowrap;${color}">${ihtm}</small></sl-tooltip>`;
            }
            getGui() {
                return this.eGui
            }
        }
    },

    /**
     * 请求连接
     */
    reqConns: () => new Promise((resolve) => {
        ndkLs.connsGet().then(conns => {
            if (conns.length == 0) {
                conns = ndkVary.resConnDemo;
            }
            conns.sort((a, b) => a.order - b.order);

            resolve(conns);
        });
    }),

    /**
     * 显示连接
     * @param {*} conns 
     */
    viewConns: (conns) => new Promise(resolve => {
        var opsConns = agg.optionDef({
            rowData: conns,//数据源
            getRowNodeId: data => data.id, //指定行标识列
            defaultColDef: agg.defaultColDef({ editable: true }),
            suppressClickEdit: true, //禁点击编辑
            columnDefs: [
                {
                    field: 'alias', headerName: ndkVary.icons.connConn + ndkI18n.lg.connAlias, tooltipField: 'conn', width: 350,
                    checkboxSelection: true, headerCheckboxSelection: true, headerCheckboxSelectionFilteredOnly: true, //仅全选过滤的数据行
                    cellRenderer: (params) => {
                        if (!params.node.group) {
                            if (params.data.type) {
                                return ndkVary.iconSvg(params.data.type, "nr-svg-typedb") + params.value;
                            }
                            return params.value
                        }
                    },
                    cellStyle: params => {
                        switch (params.node.data?.env) {
                            case "Test":
                            case "Production":
                                return { 'color': ndkVary.colorEnv(params.node.data?.env) };
                        }
                    }
                },
                {
                    field: 'env', headerName: ndkVary.icons.connEnv + ndkI18n.lg.connEnv, width: 160, enableRowGroup: true,
                    cellRenderer: params => params.value ? ndkVary.iconEnv(params.value) + params.value : "",
                    cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                        values: ndkVary.typeEnv, formatValue: fv => ndkVary.iconEnv(fv) + fv
                    }
                },
                { field: 'group', headerName: ndkVary.icons.connGroup + ndkI18n.lg.connGroup, width: 160, enableRowGroup: true },
                {
                    field: 'type', headerName: ndkVary.icons.connType + ndkI18n.lg.connType, enableRowGroup: true, width: 160,
                    cellRenderer: params => params.value ? ndkVary.iconSvg(params.value, "nr-svg-typedb") + params.value : "",
                    cellEditor: 'agRichSelectCellEditor', cellEditorParams: { values: ndkVary.typeDB }
                },
                {
                    field: 'order', headerName: ndkVary.icons.connOrder + ndkI18n.lg.connOrder, rowDrag: true,
                    filterParams: agg.filterParamsDef("Number")
                },
                { field: 'id', headerName: ndkVary.icons.id, width: 150, editable: false },
                { field: 'conn', headerName: ndkVary.icons.connConn + ndkI18n.lg.connConnection, width: 600, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                {
                    headerName: ndkVary.icons.ctrl + ndkI18n.lg.connControl, pinned: 'right', width: 100, hide: true, filter: false, sortable: false, editable: false, menuTabs: false,
                    cellRenderer: class {
                        init(params) {
                            this.eGui = document.createElement('div');

                            //非分组
                            if (params.data) {
                                this.eGui.innerHTML = `
                                <a href="javascript:void(0);" class="text-decoration-none nr-conn-cell-add" title="新增">➕</a> &nbsp;
                                <a href="javascript:void(0);" class="text-decoration-none nr-conn-cell-del" title="删除">❌</a>
                               `;

                                this.eGui.addEventListener('click', function (e) {
                                    var target = e.target;

                                    if (target.classList.contains("nr-conn-cell-add")) {
                                        //复制连接
                                        var newrow = { ...params.data };
                                        newrow.id = ndkFn.random(20000, 99999);
                                        newrow.alias += "+";

                                        ndkVary.gridOpsConns.api.applyTransaction({
                                            add: [newrow],
                                            addIndex: params.rowIndex + 1
                                        });

                                        ndkLs.connsSet(newrow);
                                    } else if (target.classList.contains("nr-conn-cell-del")) {
                                        //删除连接
                                        if (confirm("确定删除？")) {
                                            ndkVary.gridOpsConns.api.applyTransaction({
                                                remove: [params.data]
                                            });
                                            ndkLs.connsDelete(params.data.id);
                                        }
                                    }
                                }, false);
                            }
                        }
                        getGui() {
                            return this.eGui;
                        }
                    }
                },
            ],
            rowDragManaged: true, //拖拽
            rowDragMultiRow: true, //多行拖拽
            onRowDragEnd: function (event) {
                //更新排序
                var uprow = [], oi = 1;
                event.api.forEachNode(node => {
                    var data = node.data;
                    data.order = oi++;
                    uprow.push(data);
                });

                event.api.applyTransaction({
                    update: uprow
                });

                ndkLs.connsSet(uprow);
            },
            // 单元格变动
            onCellValueChanged: function (event) {
                //类型变动
                if (event.column.colId == "type") {
                    event.api.refreshCells({ rowNodes: [event.data], force: true })
                }
                //编辑连接信息
                ndkLs.connsSet(agg.getAllRows(ndkVary.gridOpsConns, true));
            },
            // 双击行连接
            onRowDoubleClicked: function (event) {
                //切换连接
                if (ndkStep.cpGet(1).cobj.id != event.data.id) {
                    ["database", "table", "column"].forEach(vkey => ndkDb.removeGrid(vkey))
                }
                ndkStep.cpSet(1, event.data); //记录连接
                ndkDb.reqDatabaseName(event.data).then(databases => {
                    ndkDb.viewDatabase(databases).then(() => {
                        ndkVary.domTabGroup1.show('tp1-database')
                    })
                })
            },
            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridConns.classList.toggle('nrc-fullscreen');
                    ndkFn.size();
                }
            },
            //连接右键菜单
            getContextMenuItems: (event) => {
                var enode = event.node, edata = enode?.data,
                    srows = ndkVary.gridOpsConns.api.getSelectedRows();

                //新增连接
                var adddbs = [];
                ndkVary.typeDB.forEach(type => {
                    adddbs.push({
                        name: type,
                        icon: ndkVary.iconSvg(type, "nr-svg-typedb-menu"),
                        action: function () {
                            var order = enode ? enode.rowIndex + 1 : agg.getAllRows(ndkVary.gridOpsConns, true).length;

                            var newrow = {
                                id: ndkFn.random(20000, 99999),
                                type: type,
                                alias: 'alias',
                                group: 'default',
                                order: order + 1,
                                env: ndkVary.typeEnv[0],
                                conn: ndkVary.resConnTemplate[type]
                            };

                            ndkVary.gridOpsConns.api.applyTransaction({
                                add: [newrow],
                                addIndex: order
                            });

                            ndkLs.connsSet(newrow);
                        }
                    });
                });

                //示例连接
                var demodbs = [];
                ndkVary.resConnDemo.forEach(dc => {
                    demodbs.push({
                        name: dc.alias,
                        icon: ndkVary.iconSvg(dc.type, "nr-svg-typedb-menu"),
                        action: function () {
                            var rows = agg.getAllRows(ndkVary.gridOpsConns, true);
                            if (rows.filter(x => x.id == dc.id).length) {
                                var rowNode = ndkVary.gridOpsConns.api.getRowNode(dc.id);
                                ndkVary.gridOpsConns.api.ensureIndexVisible(rowNode.rowIndex); //滚动到行显示
                                ndkVary.gridOpsConns.api.flashCells({ rowNodes: [rowNode] }); //闪烁行
                            } else {
                                var newrow = dc;
                                newrow.order = rows.length + 1;

                                ndkVary.gridOpsConns.api.applyTransaction({
                                    add: [newrow],
                                    addIndex: rows.length
                                });

                                ndkLs.connsSet(newrow);
                            }
                        }
                    });
                });

                //删除选中项或右键项
                var deletedbs = [];
                if (srows.length == 0 && edata) {
                    srows.push(edata);
                }
                if (srows.length) {
                    deletedbs.push({
                        name: `${ndkI18n.lg.confirmDelete}（${srows.length}）`, icon: ndkVary.icons.ok,
                        action: function () {
                            ndkVary.gridOpsConns.api.applyTransaction({ remove: srows });
                            ndkLs.connsDelete(srows.map(x => x.id));
                        }
                    });
                    srows.forEach(row => {
                        deletedbs.push({
                            name: ndkVary.iconSvg(row.type, "nr-svg-typedb-menu") + " " + row.alias, icon: ndkVary.icons.remove,
                            action: function () {
                                ndkVary.gridOpsConns.api.applyTransaction({ remove: [row] });
                                ndkLs.connsDelete(row.id);
                            }
                        })
                    });
                }

                var result = [
                    {
                        name: ndkI18n.lg.newQuery, icon: ndkVary.icons.comment, disabled: edata == null,
                        action: function () {
                            //打开选项卡
                            ndkTab.tabBuild(ndkFn.random(), ndkVary.icons.connConn + edata.alias, 'sql').then(tpkey => {
                                ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                ndkStep.cpSet(tpkey, edata); //记录连接
                                ndkStep.cpInfo(tpkey); //显示连接
                            })
                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.openConn, icon: ndkVary.icons.connDatabase, disabled: edata == null,
                        action: function () {
                            //切换连接
                            if (ndkStep.cpGet(1).cobj.id != event.node.data.id) {
                                ["database", "table", "column"].forEach(vkey => ndkDb.removeGrid(vkey))
                            }
                            ndkStep.cpSet(1, event.node.data); //记录连接
                            ndkDb.reqDatabaseName(event.node.data).then(databases => {
                                ndkDb.viewDatabase(databases).then(() => {
                                    ndkVary.domTabGroup1.show('tp1-database')
                                })
                            })
                        }
                    },
                    { name: ndkI18n.lg.createConn, icon: ndkVary.icons.connConn, subMenu: adddbs },
                    { name: ndkI18n.lg.demoConn, subMenu: demodbs },
                    {
                        name: ndkI18n.lg.editConn, icon: ndkVary.icons.edit, disabled: edata == null,
                        action: function () {
                            event.api.startEditingCell({
                                rowIndex: event.node.rowIndex,
                                colKey: event.column.colId,
                            });
                        }
                    },
                    {
                        name: ndkI18n.lg.copyConn, icon: ndkVary.iconGrid('copy'), disabled: edata == null,
                        action: function () {
                            var newrow = { ...event.node.data };
                            newrow.id = ndkFn.random();
                            newrow.alias += "+";

                            ndkVary.gridOpsConns.api.applyTransaction({
                                add: [newrow],
                                addIndex: event.node.rowIndex + 1
                            });

                            ndkLs.connsSet(newrow);
                        }
                    },
                    {
                        name: ndkI18n.lg.deleteConn, icon: ndkVary.icons.remove, disabled: deletedbs.length == 0,
                        subMenu: deletedbs
                    },
                    {
                        name: ndkI18n.lg.export, icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "JSON", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Markdown", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Custom", icon: ndkVary.iconGrid('save'),
                            }
                        ]
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.refresh, icon: ndkVary.icons.loading,
                        action: function () {
                            ndkDb.reqConns().then(conns => {
                                ndkDb.viewConns(conns)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Alt+M',
                        action: function () {
                            ndkVary.domGridConns.classList.toggle('nrc-fullscreen');
                            ndkFn.size();
                        }
                    },
                    'separator',
                    'copy',
                    'copyWithHeaders'
                ];

                return result;
            },
        });

        ndkDb.createGrid("conns", opsConns);

        resolve()
    }),

    /**
     * 请求库名
     * @param {*} cobj
     * @param {*} forcedReload
     * @returns 
     */
    reqDatabaseName: (cobj, forcedReload = false) => new Promise((resolve, reject) => {
        new Promise(fr => {
            if (forcedReload) {
                fr()
            } else {
                ndkLs.cc([cobj.id]).then(res => {
                    if (res != null) {
                        fr(res.data)
                    } else {
                        fr()
                    }
                }).catch(() => fr())
            }
        }).then(data => {
            if (data) {
                resolve(data)
            } else {
                var pars = ndkDb.parameterJoin({ tdb: cobj.type, conn: cobj.conn });
                ndkDb.request(`${ndkVary.apiServer}${ndkVary.apiGetDatabaseName}?${pars}`).then(res => {
                    var dbrows = [];
                    res.data.forEach(name => {
                        dbrows.push({ DatabaseName: name })
                    })
                    ndkLs.cc([cobj.id], dbrows) //缓存

                    resolve(dbrows);
                }).catch(err => reject(err));
            }
        })
    }),

    /**
     * 请求库信息
     * @param {*} cobj
     * @param {*} filterDatabaseName
     * @param {*} forcedReload
     * @returns 
     */
    reqDatabaseInfo: (cobj, filterDatabaseName, forcedReload = false) => new Promise((resolve, reject) => {
        var fdn = filterDatabaseName == "" ? [] : filterDatabaseName.split(',');
        new Promise(fr => {
            if (forcedReload) {
                fr()
            } else {
                ndkLs.cc([cobj.id]).then(res => {
                    if (res != null) {
                        var hasdi = true;
                        res.data.forEach(row => {
                            if (fdn.length == 0) {
                                if (!Object.hasOwnProperty.call(row, "DatabaseClassify")) {
                                    hasdi = false;
                                }
                            } else {
                                if (fdn.includes(row.DatabaseName) && !Object.hasOwnProperty.call(row, "DatabaseClassify")) {
                                    hasdi = false;
                                }
                            }
                        })
                        if (hasdi) {
                            fr(res.data)
                        } else {
                            fr()
                        }
                    } else {
                        fr()
                    }
                }).catch(() => fr())
            }
        }).then(data => {
            if (data) {
                resolve(data)
            } else {
                var pars = ndkDb.parameterJoin({ tdb: cobj.type, conn: cobj.conn, filterDatabaseName: filterDatabaseName });
                ndkDb.request(`${ndkVary.apiServer}${ndkVary.apiGetDatabaseInfo}?${pars}`).then(res => {
                    ndkLs.cc([cobj.id]).then(cdb => {
                        cdb.data.forEach(row => {
                            var newrow = res.data.filter(x => x.DatabaseName == row.DatabaseName);
                            if (newrow.length > 0) {
                                Object.assign(row, newrow[0]);
                            }
                        })
                        ndkLs.cc([cobj.id], cdb.data) //缓存

                        resolve(cdb.data);
                    })
                }).catch(err => reject(err));
            }
        })
    }),

    /**
     * 显示库
     * @param {*} databases 
     */
    viewDatabase: (databases) => new Promise(resolve => {
        var opsDatabase = agg.optionDef({
            rowData: databases,//数据源
            getRowNodeId: data => data.DatabaseName, //指定行标识列
            columnDefs: [
                {
                    headerName: ndkI18n.lg.databaseInfo,
                    children: [
                        agg.numberCol(), //行号
                        { field: 'DatabaseName', headerName: ndkVary.icons.connDatabase + ndkI18n.lg.dbName, tooltipField: 'DatabaseName', width: 350 },
                        { field: 'DatabaseClassify', headerName: ndkI18n.lg.dbCatagory, enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseOwner', headerName: ndkI18n.lg.dbOwner, enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseSpace', headerName: ndkI18n.lg.dbSpace, enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseCharset', headerName: ndkI18n.lg.dbCharSet, enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseCollation', headerName: ndkI18n.lg.dbCollate, width: 180, enableRowGroup: true, columnGroupShow: 'open' },
                        {
                            field: 'DatabaseDataLength', headerName: ndkI18n.lg.dbDataSize, columnGroupShow: 'open', aggFunc: 'sum',
                            cellRenderer: params => params.value > 0 ? ndkFn.formatByteSize(params.value) : "",
                            filterParams: agg.filterParamsDef("Number")
                        },
                        {
                            field: 'DatabaseLogLength', headerName: ndkI18n.lg.dbLogSize, columnGroupShow: 'open', aggFunc: 'sum',
                            cellRenderer: params => params.value > 0 ? ndkFn.formatByteSize(params.value) : "",
                            filterParams: agg.filterParamsDef("Number")
                        },
                        {
                            field: 'DatabaseIndexLength', headerName: ndkI18n.lg.dbIndexSize, columnGroupShow: 'open', aggFunc: 'sum',
                            cellRenderer: params => params.value > 0 ? ndkFn.formatByteSize(params.value) : "",
                            filterParams: agg.filterParamsDef("Number")
                        },
                        { field: 'DatabasePath', headerName: ndkI18n.lg.dbDataPath, width: 400, columnGroupShow: 'open' },
                        { field: 'DatabaseLogPath', headerName: ndkI18n.lg.dbLogPath, width: 400, columnGroupShow: 'open' },
                        {
                            field: 'DatabaseCreateTime', headerName: ndkI18n.lg.createTime, width: 200, columnGroupShow: 'open',
                            filterParams: agg.filterParamsDef("Date")
                        },
                    ]
                }
            ],
            statusBar: {
                statusPanels: [
                    { statusPanel: 'connStatusBarComponent', align: 'left', },
                    { statusPanel: 'agSelectedRowCountComponent' },
                    { statusPanel: 'agTotalAndFilteredRowCountComponent' },
                ],
            },
            components: {
                connStatusBarComponent: ndkDb.gridComponents.connStatusBarComponent
            },
            //双击库打开表
            onRowDoubleClicked: function (event) {
                if (!event.node.group) {
                    var cp = ndkStep.cpGet(1);
                    ndkStep.cpSet(1, cp.cobj, event.node.data.DatabaseName); //记录连接
                    ndkDb.reqTable(cp.cobj, event.node.data.DatabaseName).then(tables => {
                        ndkDb.viewTable(tables, cp.cobj).then(() => {
                            ndkVary.domTabGroup1.show('tp1-table')
                        })
                    })
                }
            },
            //库信息展开
            onColumnGroupOpened: function (event) {
                if (event.columnGroup.expanded) {
                    var rows = event.api.getSelectedRows();
                    if (rows.length == 0) {
                        var firstdi = event.api.getFirstDisplayedRow();
                        var lastdi = event.api.getLastDisplayedRow();
                        for (var i = firstdi; i <= lastdi; i++) {
                            var row = event.api.getDisplayedRowAtIndex(i);
                            if (!row.group) {
                                rows.push(row.data);
                            }
                        }
                    } else if (agg.getAllRows(ndkVary.gridOpsDatabase).length == rows.length) {
                        rows = [];
                    }

                    var cp = ndkStep.cpGet(1);
                    ndkDb.reqDatabaseInfo(cp.cobj, rows.map(row => row.DatabaseName).join(',')).then(res => {
                        ndkVary.gridOpsDatabase.api.applyTransactionAsync({
                            update: res
                        })
                    });
                }
            },
            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridDatabase.classList.toggle('nrc-fullscreen');
                    ndkFn.size();
                }
            },
            //连接菜单项
            getContextMenuItems: (event) => {
                var edata = event.node ? event.node.data : null;

                var result = [
                    {
                        name: ndkI18n.lg.newQuery, icon: ndkVary.icons.comment, disabled: edata == null,
                        action: function () {
                            //构建选项卡
                            ndkTab.tabBuild(ndkFn.random(), ndkVary.icons.connDatabase + edata.DatabaseName, 'sql').then(tpkey => {
                                ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                var cp = ndkStep.cpGet(1);
                                ndkStep.cpSet(tpkey, cp.cobj, edata.DatabaseName); //记录连接
                                ndkStep.cpInfo(tpkey); //显示连接
                            })
                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.openDatabase, icon: ndkVary.icons.connTable, disabled: edata == null,
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            ndkStep.cpSet(1, cp.cobj, edata.DatabaseName); //记录连接
                            ndkDb.reqTable(cp.cobj, edata.DatabaseName).then(tables => {
                                ndkDb.viewTable(tables, cp.cobj).then(() => {
                                    ndkVary.domTabGroup1.show('tp1-table')
                                })
                            })
                        }
                    },
                    {
                        name: ndkI18n.lg.export, icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "JSON", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Markdown", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Custom", icon: ndkVary.iconGrid('save'),
                            }
                        ]
                    },
                    {
                        name: ndkI18n.lg.import, icon: ndkVary.iconGrid('asc'),
                        action: function () {

                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.refresh, icon: ndkVary.icons.loading,
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            ndkDb.reqDatabaseName(cp.cobj, true).then(databases => {
                                ndkDb.viewDatabase(databases)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Alt+M',
                        action: function () {
                            ndkVary.domGridDatabase.classList.toggle('nrc-fullscreen');
                            ndkFn.size();
                        }
                    },
                    'separator',
                    'copy',
                    'copyWithHeaders',
                    'separator',
                    'chartRange'
                ];

                return result;
            }
        });

        ndkDb.createGrid("database", opsDatabase);

        //如果库多，默认过滤显示
        if (databases.length > ndkVary.parameterConfig.autoFilterDatabaseNumber.value) {
            var cd = ndkStep.cpGet(1);
            var dbkey = cd.cobj.type == "Oracle" ? "user id=" : "database=", databaseName, dbs = cd.cobj.conn.split(';').filter(kv => kv.toLowerCase().startsWith(dbkey));
            if (dbs.length) {
                databaseName = dbs[0].split('=').pop();

                //过滤库名（如果调整过滤配置与过滤值不合则会报错）
                try {
                    ndkVary.gridOpsDatabase.api.setFilterModel({
                        DatabaseName: {
                            filterType: "multi",
                            filterModels: [
                                null,
                                {
                                    values: [databaseName],
                                    filterType: "set"
                                }
                            ]
                        }
                    });
                } catch (error) {
                    console.debug(error);
                }
            }
        }

        resolve();
    }),

    /**
     * 请求表
     * @param {*} cobj 
     * @param {*} databaseName
     * @param {*} forcedReload
     * @returns 
     */
    reqTable: (cobj, databaseName, forcedReload = false) => new Promise((resolve, reject) => {
        new Promise(fr => {
            if (forcedReload) {
                fr()
            } else {
                ndkLs.cc([cobj.id, databaseName]).then(res => {
                    if (res != null) {
                        fr(res.data)
                    } else {
                        fr()
                    }
                }).catch(() => fr())
            }
        }).then(data => {
            if (data) {
                resolve(data)
            } else {
                var pars = ndkDb.parameterJoin({ tdb: cobj.type, conn: cobj.conn, databaseName: databaseName });
                ndkDb.request(`${ndkVary.apiServer}${ndkVary.apiGetTable}?${pars}`).then(res => {
                    ndkLs.cc([cobj.id, databaseName], res.data) //缓存
                    resolve(res.data);
                }).catch(err => reject(err));
            }
        })
    }),

    /**
     * 显示表
     * @param {*} tables
     * @param {*} cobj 
     * @returns 
     */
    viewTable: (tables, cobj) => new Promise(resolve => {
        var isSQLite = cobj.type == "SQLite", isSQLServer = cobj.type == "SQLServer";

        var opsTable = agg.optionDef({
            rowData: tables,//数据源
            getRowNodeId: data => data.TableName, //指定行标识列
            rowGroupPanelShow: 'never',
            columnDefs: [
                agg.numberCol(), //行号
                {
                    field: 'TableName', tooltipField: "TableName", headerName: ndkVary.icons.connTable + ndkI18n.lg.tableName, width: 350,
                    cellRenderer: params => {
                        if (isSQLServer && params.data) {
                            return `${params.data.TableSchema}.${params.value}`;
                        }
                        return params.value;
                    }
                },
                { field: 'TableComment', tooltipField: "TableComment", headerName: ndkVary.icons.comment + ndkI18n.lg.tableComment, width: 290, hide: isSQLite, editable: !isSQLite, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                { field: 'TableRows', headerName: ndkI18n.lg.tableRows, filterParams: agg.filterParamsDef("Number"), aggFunc: 'sum', },
                {
                    field: 'TableDataLength', headerName: ndkI18n.lg.tableDataSize, width: 160, aggFunc: 'sum',
                    cellRenderer: params => ndkFn.formatByteSize(params.value),
                    filterParams: agg.filterParamsDef("Number")
                },
                {
                    field: 'TableIndexLength', headerName: ndkI18n.lg.tableIndexSize, width: 160, aggFunc: 'sum',
                    cellRenderer: params => ndkFn.formatByteSize(params.value),
                    filterParams: agg.filterParamsDef("Number")
                },
                { field: 'TableCollation', headerName: ndkI18n.lg.tableCollate, width: 180, },
                {
                    field: 'TableCreateTime', headerName: ndkI18n.lg.createTime, width: 220,
                    filterParams: agg.filterParamsDef("Date")
                },
                {
                    field: 'TableModifyTime', headerName: ndkI18n.lg.updateTime, width: 220,
                    filterParams: agg.filterParamsDef("Date")
                },
                { field: 'TableSchema', headerName: ndkI18n.lg.tableSchema },
                { field: 'TableType', headerName: ndkI18n.lg.tableCatagory },
            ],
            groupIncludeFooter: true, //显示分组小计
            groupIncludeTotalFooter: true, //显示分组总计
            statusBar: {
                statusPanels: [
                    { statusPanel: 'connStatusBarComponent', align: 'left', },
                    { statusPanel: 'agSelectedRowCountComponent' },
                    { statusPanel: 'agTotalAndFilteredRowCountComponent' },
                ],
            },
            components: {
                connStatusBarComponent: ndkDb.gridComponents.connStatusBarComponent
            },
            //双击表打开列
            onCellDoubleClicked: function (event) {
                //非表注释打开列
                if (event.column.colId != "TableComment") {
                    var cp = ndkStep.cpGet(1);
                    ndkStep.cpSet(1, cp.cobj, cp.databaseName); //记录连接
                    ndkDb.reqColumn(cp.cobj, cp.databaseName, event.node.data.TableName).then(columns => {
                        ndkDb.viewColumn(columns, cp.cobj).then(() => {
                            ndkVary.domTabGroup1.show('tp1-column')
                        })
                    })
                }
            },
            onCellValueChanged: function (event) {
                //修改表注释
                if (event.column.colId == "TableComment") {
                    var cp = ndkStep.cpGet(1);
                    ndkDb.setTableComment(cp.cobj, cp.databaseName, event.data.TableName, event.value).then(() => {
                        //更新本地数据
                    })
                }
            },
            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridTable.classList.toggle('nrc-fullscreen');
                    ndkFn.size();
                }
            },
            //表菜单项
            getContextMenuItems: (event) => {
                var edata = event.node ? event.node.data : null;
                var isSQLite = ndkStep.cpGet(1).cobj.type == "SQLite";
                var result = [
                    {
                        name: ndkI18n.lg.tableDesign, disabled: event.node?.data == null, icon: ndkVary.iconGrid('columns'),
                        action: function () {
                            var srows = ndkVary.gridOpsTable.api.getSelectedRows();
                            //选中的表行或右键行
                            if (srows.length == 0) {
                                srows = [edata];
                            }

                            var filterTable = "";
                            if (ndkVary.gridOpsTable.rowData.length != srows.length) {
                                filterTable = srows.map(x => x.TableName).join(',')
                            }

                            var cp = ndkStep.cpGet(1);
                            ndkStep.cpSet(1, cp.cobj, cp.databaseName); //记录连接
                            ndkDb.reqColumn(cp.cobj, cp.databaseName, filterTable).then(columns => {
                                ndkDb.viewColumn(columns, cp.cobj).then(() => {
                                    ndkVary.domTabGroup1.show('tp1-column')
                                    ndkStep.stepSave();
                                })
                            })
                        }
                    },
                    {
                        name: ndkI18n.lg.tableData, icon: ndkVary.iconGrid('grip'), disabled: event.node?.data == null,
                        action: function () {
                            var srows = ndkVary.gridOpsTable.api.getSelectedRows();
                            //选中的表行或右键行
                            if (srows.length == 0) {
                                srows = [edata];
                            }

                            //打开选项卡
                            ndkTab.tabBuild(ndkFn.random(), ndkVary.icons.connTable + srows[0].TableName, 'sql').then(tpkey => {
                                ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                var tpobj = ndkTab.tabKeys[tpkey];

                                tpobj.editor.setValue(`-- ${ndkI18n.lg.generatingScript}`);

                                var cp = ndkStep.cpGet(1);
                                ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName); //记录连接
                                ndkStep.cpInfo(tpkey); //显示连接

                                var tpcp = ndkStep.cpGet(tpkey);
                                //构建SQL
                                var sqls = [];
                                srows.forEach(tableRow => {
                                    sqls.push(ndkBuild.buildSelectSql(tpcp, tableRow));
                                });
                                if (sqls.length == 1) {
                                    sqls = ndkEditor.formatterSQL(sqls[0], cp.cobj.type); //格式化
                                } else {
                                    sqls = sqls.join(';\r\n');
                                }
                                tpobj.editor.setValue(sqls);

                                //执行SQL
                                if (srows.length < 6) {
                                    ndkTab.tabEditorExecuteSql(tpkey)
                                }
                            })
                        }
                    },
                    {
                        name: ndkI18n.lg.tableGenerateSQL, icon: ndkVary.icons.generate,
                        subMenu: [
                            {
                                name: `SELECT`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    ndkBuild.buildNewTabSql(edata, 'Select');
                                }
                            },
                            {
                                name: `INSERT`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    ndkBuild.buildNewTabSql(edata, 'Insert');
                                }
                            },
                            {
                                name: `UPDATE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    ndkBuild.buildNewTabSql(edata, 'Update');
                                }
                            },
                            {
                                name: `DELETE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    ndkBuild.buildNewTabSql(edata, 'Delete');
                                }
                            },
                            {
                                name: `TRUNCATE`, icon: ndkVary.iconGrid('paste'), disabled: isSQLite, action: function () {
                                    ndkBuild.buildNewTabSql(edata, 'Truncate');
                                }
                            },
                            {
                                name: `DROP`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    ndkBuild.buildNewTabSql(edata, 'Drop');
                                }
                            },
                            'separator',
                            {
                                name: `DDL`, icon: ndkVary.iconGrid('paste'), action: function () {
                                }
                            },
                            {
                                name: `结构和数据`, icon: ndkVary.iconGrid('paste'), action: function () {
                                }
                            },
                        ]
                    },
                    {
                        name: ndkI18n.lg.export, icon: ndkVary.iconGrid('desc'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "JSON", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Markdown", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Custom", icon: ndkVary.iconGrid('save'),
                            }
                        ]
                    },
                    {
                        name: ndkI18n.lg.import, icon: ndkVary.iconGrid('asc'),
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.refresh, icon: ndkVary.icons.loading,
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            ndkDb.reqTable(cp.cobj, cp.databaseName, true).then(tables => {
                                ndkDb.viewTable(tables, cp.cobj)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Alt+M',
                        action: function () {
                            ndkVary.domGridTable.classList.toggle('nrc-fullscreen');
                            ndkFn.size();
                        }
                    },
                    'separator',
                    'copy',
                    'copyWithHeaders',
                    'separator',
                    'chartRange'
                ];

                return result;
            }
        });

        ndkDb.createGrid("table", opsTable);

        resolve();
    }),

    /**
     * 设置表注释
     * @param {any} cobj
     * @param {any} databaseName
     * @param {any} tableName
     * @param {any} tableComment
     */
    setTableComment: (cobj, databaseName, tableName, tableComment) => new Promise((resolve, reject) => {
        var fd = new FormData();
        fd.append('tdb', cobj.type);
        fd.append('conn', cobj.conn);
        fd.append('tableName', tableName);
        fd.append('tableComment', tableComment);
        fd.append('databaseName', databaseName);

        ndkDb.request(`${ndkVary.apiServer}${ndkVary.apiSetTableComment}`, {
            method: "POST",
            body: fd
        }).then(res => resolve(res)).catch(err => reject(err));
    }),

    /**
     * 请求列
     * @param {*} cobj 
     * @param {*} databaseName
     * @param {*} filterTableName
     * @param {*} forcedReload
     * @returns 
     */
    reqColumn: (cobj, databaseName, filterTableName, forcedReload = false) => new Promise((resolve, reject) => {
        new Promise(fr => {
            if (forcedReload) {
                fr()
            } else {
                ndkLs.cc([cobj.id, databaseName, '*']).then(cout => {
                    if (cout != null) {
                        if (filterTableName != null && filterTableName != "") {
                            var tns = cout.data.map(x => x.TableName),
                                ftns = filterTableName.split(',');
                            //本地缓存不够
                            if (ftns.filter(x => !tns.includes(x)).length) {
                                fr();
                            } else {
                                var fdata = cout.data.filter(x => ftns.includes(x.TableName));
                                fr(fdata)
                            }
                        } else {
                            fr(cout.data)
                        }
                    } else {
                        fr()
                    }
                }).catch(() => fr())
            }
        }).then(data => {
            if (data) {
                resolve(data)
            } else {
                var fd = new FormData();
                fd.append('tdb', cobj.type);
                fd.append('conn', cobj.conn);
                fd.append('filterTableName', filterTableName);
                fd.append('databaseName', databaseName);

                ndkDb.request(`${ndkVary.apiServer}${ndkVary.apiGetColumn}`, {
                    method: "POST",
                    body: fd
                }).then(res => {
                    //缓存
                    ndkLs.cc([cobj.id, databaseName, '*']).then(cout => {
                        if (cout != null) {
                            //移除旧表再拼接再排序
                            var tns = res.data.map(x => x.TableName);
                            var fdata = cout.data.filter(x => !tns.includes(x.TableName));
                            fdata = fdata.concat(res.data).sort((a, b) => a.TableName.localeCompare(b.TableName))
                            ndkLs.cc([cobj.id, databaseName, '*'], fdata)
                        } else {
                            ndkLs.cc([cobj.id, databaseName, '*'], res.data)
                        }
                    })
                    resolve(res.data);
                }).catch(err => reject(err));
            }
        })
    }),

    /**
     * 显示列
     * @param {*} columns
     * @param {*} cobj 
     * @returns 
     */
    viewColumn: (columns, cobj) => new Promise(resolve => {
        var isSQLite = cobj.type == "SQLite";

        var opsColumn = agg.optionDef({
            rowData: columns,//数据源
            getRowNodeId: data => `${data.TableName}:${data.ColumnName}`, //指定行标识列
            groupSelectsChildren: true, //选择子项
            //列
            columnDefs: [
                agg.numberCol({ hide: true }), //行号
                { field: 'TableName', headerName: ndkVary.icons.connTable + ndkI18n.lg.tableName, rowGroup: true, enableRowGroup: true, width: 350, hide: true, },
                { field: 'TableComment', headerName: ndkVary.icons.connTable + ndkI18n.lg.tableComment, width: 300, hide: true },
                { field: 'ColumnName', headerName: ndkVary.icons.connColumn + ndkI18n.lg.columnName, width: 200, hide: true, },
                { field: 'ColumnComment', tooltipField: "ColumnComment", headerName: ndkVary.icons.comment + ndkI18n.lg.columnComment, width: 330, hide: isSQLite, editable: !isSQLite, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                { field: 'ColumnType', headerName: ndkI18n.lg.columnTypeLength, width: 150, hide: true },
                { field: 'DataType', headerName: ndkI18n.lg.columnType, width: 120, hide: true },
                {
                    field: 'DataLength', headerName: ndkI18n.lg.columnLength, hide: true,
                    filterParams: agg.filterParamsDef("Number")
                },
                {
                    field: 'DataScale', headerName: ndkI18n.lg.columnScale, hide: true,
                    filterParams: agg.filterParamsDef("Number")
                },
                {
                    field: 'ColumnOrder', headerName: ndkI18n.lg.columnOrder,
                    filterParams: agg.filterParamsDef("Number")
                },
                {
                    field: 'PrimaryKey', headerName: ndkI18n.lg.columnPrimary, hide: true,
                    cellRenderer: params => params.value > 0 ? ndkVary.icons.key + params.value : ""
                },
                { field: 'AutoIncr', headerName: ndkI18n.lg.columnAutoIncrement, cellRenderer: params => params.value == 1 ? ndkVary.icons.incr : "" },
                {
                    field: 'IsNullable', headerName: ndkI18n.lg.columnIsNullable, hide: true,
                    cellRenderer: params => params.value == 1 ? "" : ndkVary.icons.edit
                },
                { field: 'ColumnDefault', headerName: ndkI18n.lg.columnDefault, width: 120, },
            ],
            autoGroupColumnDef: agg.autoGroupColumnDef({
                field: "ColumnName", headerName: ndkI18n.lg.group, width: 560,
                headerCheckboxSelection: true, headerCheckboxSelectionFilteredOnly: true, //仅全选过滤的数据行
                filterValueGetter: params => params.data.TableName,
                cellRendererParams: {
                    checkbox: true,
                    suppressCount: true,
                    suppressEnterExpand: true,
                    suppressDoubleClickExpand: true,
                    innerRenderer: function (item) {
                        if (item.node.group) {
                            var grow = ndkVary.gridOpsColumn.rowData.filter(x => x[item.node.field] == item.value);
                            return `<b>${item.value} (${grow.length}) </b> <span class="badge bg-secondary mx-2" role="button">数据</span> <span title="${grow[0].TableComment || ""}">${grow[0].TableComment || ""}</span>`;
                        } else {
                            var colattr = [];
                            if (item.data.PrimaryKey > 0) {
                                colattr.push(ndkVary.icons.key + item.data.PrimaryKey);
                            }
                            colattr.push(item.data.ColumnType);
                            if (item.data.IsNullable == 1) {
                                colattr.push("null");
                            } else {
                                colattr.push("not null");
                            }
                            return `${item.value} <span class="opacity-75">( ${colattr.join(', ')} )</span>`;
                        }
                    }
                }
            }),
            statusBar: {
                statusPanels: [
                    { statusPanel: 'connStatusBarComponent', align: 'left', },
                    { statusPanel: 'agSelectedRowCountComponent' },
                    { statusPanel: 'agTotalAndFilteredRowCountComponent' },
                ],
            },
            components: {
                connStatusBarComponent: ndkDb.gridComponents.connStatusBarComponent
            },
            onCellValueChanged: function (event) {
                //修改列注释
                if (event.column.colId == "ColumnComment") {
                    var cp = ndkStep.cpGet(1);
                    ndkDb.setColumnComment(cp.cobj, cp.databaseName, event.data.TableName, event.data.ColumnName, event.value).then(() => {
                        //更新本地数据
                    })
                }
            },
            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridColumn.classList.toggle('nrc-fullscreen');
                    ndkFn.size();
                }
            },
            //列右键菜单项
            getContextMenuItems: (params) => {
                var result = [
                    'expandAll',
                    'contractAll',
                    {
                        name: ndkI18n.lg.export, icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "JSON", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Markdown", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "Custom", icon: ndkVary.iconGrid('save'),
                            },
                        ]
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.refresh, icon: ndkVary.icons.loading,
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            ndkDb.reqColumn(cp.cobj, cp.databaseName, cp.tableName, true).then(columns => {
                                ndkDb.viewColumn(columns, cp.cobj)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Alt+M',
                        action: function () {
                            ndkVary.domGridColumn.classList.toggle('nrc-fullscreen');
                            ndkFn.size();
                        }
                    },
                    'separator',
                    'copy',
                    'copyWithHeaders'
                ];

                return result;
            },
        });

        ndkDb.createGrid("column", opsColumn);

        //展开分组
        ndkVary.gridOpsColumn.api.forEachNode((node) => {
            if (node.level == 0) {
                node.setExpanded(true);
            }
        });

        resolve();
    }),

    /**
     * 设置列注释
     * @param {any} cobj
     * @param {any} databaseName
     * @param {any} tableName
     * @param {any} columnName
     * @param {any} columnComment
     */
    setColumnComment: (cobj, databaseName, tableName, columnName, columnComment) => new Promise((resolve, reject) => {
        var fd = new FormData();
        fd.append('tdb', cobj.type);
        fd.append('conn', cobj.conn);
        fd.append('tableName', tableName);
        fd.append('columnName', columnName);
        fd.append('columnComment', columnComment);
        fd.append('databaseName', databaseName);

        ndkDb.request(`${ndkVary.apiServer}${ndkVary.apiSetColumnComment}`, {
            method: "POST",
            body: fd
        }).then(res => resolve(res)).catch(err => reject(err));
    }),

    /**
     * 请求执行SQL
     * @param {*} cobj
     * @param {*} forcedReload
     * @returns 
     */
    reqExecuteSql: (cobj, databaseName, sql) => new Promise((resolve, reject) => {
        var fd = new FormData();
        fd.append('tdb', cobj.type);
        fd.append('conn', cobj.conn);
        fd.append('sql', sql);
        if (databaseName) {
            fd.append('databaseName', databaseName);
        }

        ndkDb.request(`${ndkVary.apiServer}${ndkVary.apiExecuteSql}`, {
            method: "POST",
            body: fd
        }).then(res => {
            resolve(res.data);
        }).catch(err => reject(err));
    }),

    /**
     * 显示执行SQL
     * @param {any} esdata
     * @param {any} tpkey
     */
    viewExecuteSql: (esdata, tpkey) => new Promise(resolve => {
        var tpobj = ndkTab.tabKeys[tpkey];

        //清空表
        if (tpobj.grids) {
            tpobj.grids.forEach(item => item.gridOps && item.gridOps.api.destroy());
        }
        tpobj.grids = [];
        tpobj.esdata = esdata;
        tpobj.domTabGroup3.innerHTML = '';
        if (!tpobj.domTabGroup3.getAttribute("data-bind")) {
            tpobj.domTabGroup3.setAttribute("data-bind", true);

            //选项卡3执行结果-关闭
            tpobj.domTabGroup3.addEventListener('sl-close', async event => {
                var sltab = event.target;
                var panel = tpobj.domTabGroup3.querySelector(`sl-tab-panel[name="${sltab.panel}"]`);

                if (sltab.active) {
                    var otab = sltab.previousElementSibling || sltab.nextElementSibling;
                    if (otab.nodeName != "sl-tab-panel".toUpperCase()) {
                        tpobj.domTabGroup3.show(otab.panel);
                    }
                }

                sltab.remove();
                panel.remove();
                ndkTab.tabNavFix();

                //删除key
                for (var i = 0; i < tpobj.grids.length; i++) {
                    var grid = tpobj.grids[i];
                    if (grid.tpkey == sltab.panel) {
                        tpobj.grids.splice(i, 1);
                        break;
                    }
                }

                //阻止冒泡
                event.stopPropagation();
            });
            //选项卡3执行结果-显示
            tpobj.domTabGroup3.addEventListener('sl-tab-show', function (event) {
                ndkFn.size()
            }, false);
        }

        new Promise(resolve2 => {
            for (const itemn in esdata) {
                //跳过表结构
                if (itemn == "Item2") {
                    continue;
                }
                var ti = 0, tableSchema;
                for (const iname in esdata[itemn]) {
                    ti++;

                    if (esdata["Item2"] && iname in esdata["Item2"]) {
                        tableSchema = esdata["Item2"][iname];
                    }

                    var esrows = esdata[itemn][iname], columnDefs = [];
                    var tabname = "Result", tabpanel = `tp3-${itemn}-${ti}`;
                    if (itemn == "Item1") {
                        if (tableSchema) {
                            tabname = tableSchema[0].BaseTableName || tabname;
                        }
                        if (ti > 1) {
                            tabname += " - " + ti;
                        }
                        tabname = ndkVary.icons.data + tabname;
                    } else {
                        tabname = ndkVary.icons.info + "Info"
                    }

                    //结构
                    var tabbox = document.createElement("div");
                    tabbox.innerHTML = `
<sl-tab slot="nav" panel="${tabpanel}" closable>${tabname}</sl-tab>
<sl-tab-panel name="${tabpanel}">
    <sl-input class="nr-filter-execute-sql mb-2" placeholder="${ndkI18n.lg.search}" size="small"></sl-input>
    <div class="nr-grid-execute-sql" data-key="${tpkey}"></div>
</sl-tab-panel>
                `;
                    let domTab = tabbox.querySelector('sl-tab'),
                        domTabPanel = tabbox.querySelector('sl-tab-panel'),
                        domFilterExecuteSql = tabbox.querySelector('.nr-filter-execute-sql'),
                        domGridExecuteSql = tabbox.querySelector('.nr-grid-execute-sql');

                    var slpanels = tpobj.domTabGroup3.querySelectorAll("sl-tab-panel");
                    tpobj.domTabGroup3.insertBefore(domTab, slpanels[0]);
                    tpobj.domTabGroup3.appendChild(domTabPanel);

                    tabbox.remove();

                    //完成事件
                    var gridReady = function (event) {
                        event.api.sizeColumnsToFit(); //列宽100%自适应
                        ndkFn.size();
                    }

                    //填充列头（有数据）
                    if (esrows.length > 0) {
                        if (itemn == "Item1") {
                            columnDefs.push(agg.numberCol()); //行号
                        }

                        var esrow = esrows[0];
                        for (var field in esrow) {
                            var htip = [], htag = [], colDef = {
                                field: field, headerName: field, headerTooltip: field, width: 200, enableRowGroup: true,
                                headerComponentParams: { template: null }
                            }
                            if (tableSchema) {
                                var colSchema = tableSchema.find(x => x.ColumnName == field);
                                if (colSchema) {
                                    //没有类型列则取C#类型
                                    var colType = colSchema.DataTypeName;
                                    if (colType == null) {
                                        colType = colSchema.DataType.split(',')[0].split('.')[1].toLowerCase();
                                    }

                                    if (colSchema.IsKey) {
                                        htip.push(ndkVary.icons.key);
                                    }
                                    htip.push(colSchema.ColumnName);
                                    htip.push(colType);
                                    htip.push(colSchema.ColumnSize);
                                    htip.push(colSchema.AllowDBNull ? "null" : "not null");

                                    var tdb = ndkStep.cpGet(tpobj.tpkey).cobj.type;
                                    var column = {
                                        DataType: colSchema.DataTypeName == null ? colType.replace(/\d+/g, '') : colSchema.DataTypeName,
                                        DataLength: colSchema.ColumnSize,
                                        PrimaryKey: colSchema.IsKey ? 1 : 0
                                    }
                                    var sdt = ndkBuild.sqlDataType(tdb, column);
                                    switch (sdt.category) {
                                        case "string":
                                            colDef.width = Math.min(colSchema.ColumnSize * 10, 500);
                                            //字符串显示截断
                                            colDef.cellRenderer = params => {
                                                var slen = ndkVary.parameterConfig.gridDataShowLength.value;
                                                if (params.value != null && params.value.length > slen) {
                                                    return ndkVary.icons.cut + " " + params.value.substring(0, slen) + " ...";
                                                }
                                                return params.value;
                                            };
                                            htag.push(ndkVary.iconSvg("abc", "nr-svg-headertype"))
                                            break;
                                        case "number":
                                            htag.push(ndkVary.iconSvg("123", "nr-svg-headertype"))
                                            colDef.filterParams = agg.filterParamsDef("Number");
                                            break;
                                        case "date":
                                            htag.push(ndkVary.iconSvg("clock", "nr-svg-headertype"))
                                            colDef.width = 280;
                                            colDef.filterParams = agg.filterParamsDef("Date");
                                            break;
                                    }
                                    if (colSchema.IsKey) {
                                        htag.push(ndkVary.iconSvg("key", "nr-svg-headertype nrc-text-orange"))
                                    }

                                    colDef.width = Math.max(colDef.width, 200);
                                    colDef.width = Math.min(colDef.width, 500);
                                }
                            } else {
                                htip.push(field);
                            }

                            colDef.headerTooltip = htip.join(" ");
                            colDef.headerComponentParams.template = `
                            <div class="ag-cell-label-container" role="presentation">
                                <span ref="eMenu" class="ag-header-icon ag-header-cell-menu-button" aria-hidden="true"></span>
                                <div ref="eLabel" class="ag-header-cell-label" role="presentation" unselectable="on">
                                    <sup class="ag-header-type-icon">${htag.join('')}</sup>
                                    <span ref="eText" class="ag-header-cell-text" unselectable="on"></span>
                                    <span ref="eFilter" class="ag-header-icon ag-header-label-icon ag-filter-icon" aria-hidden="true"></span>
                                    <span ref="eSortOrder" class="ag-header-icon ag-header-label-icon ag-sort-order" aria-hidden="true"></span>
                                    <span ref="eSortAsc" class="ag-header-icon ag-header-label-icon ag-sort-ascending-icon" aria-hidden="true"></span>
                                    <span ref="eSortDesc" class="ag-header-icon ag-header-label-icon ag-sort-descending-icon" aria-hidden="true"></span>
                                    <span ref="eSortNone" class="ag-header-icon ag-header-label-icon ag-sort-none-icon" aria-hidden="true"></span>
                                </div>
                            </div>`;
                            columnDefs.push(colDef);
                        }

                        //超过3列，列宽不自适应
                        if (columnDefs.length > 2) {
                            gridReady = function () {
                                ndkFn.size();
                            }
                        }
                    } else if (iname in (esdata.Item2 || {})) {
                        //填充列头（空表）
                        for (const coln in esdata.Item2[iname]) {
                            var col = esdata.Item2[iname][coln];
                            columnDefs.push({ headerName: col.ColumnName, field: col.ColumnName, headerTooltip: field });
                        }

                        gridReady = function (event) {
                            console.log("gridReady", event);
                            event.columnApi.autoSizeAllColumns();
                            ndkFn.size();
                        }
                    }

                    var opsExecuteSql = agg.optionDef({
                        rowData: esrows,
                        columnDefs: columnDefs,
                        //表菜单项
                        getContextMenuItems: (event) => {
                            var result = [
                                'copy',
                                'copyWithHeaders',
                                {
                                    name: ndkI18n.lg.export, icon: ndkVary.iconGrid('save'),
                                    subMenu: [
                                        'csvExport',
                                        'excelExport',
                                        'separator',
                                        {
                                            name: "JSON", icon: ndkVary.iconGrid('save'),
                                        },
                                        {
                                            name: "Markdown", icon: ndkVary.iconGrid('save'),
                                        },
                                        {
                                            name: "Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                                        },
                                        {
                                            name: "Custom", icon: ndkVary.iconGrid('save'),
                                        }
                                    ]
                                },
                                {
                                    name: ndkI18n.lg.dataGenerateSQL, icon: ndkVary.icons.generate,
                                    subMenu: [
                                        {
                                            name: `INSERT`, icon: ndkVary.iconGrid('paste'), action: function () {
                                                ndkBuild.dataNewSql(event, "Insert");
                                            }
                                        },
                                        {
                                            name: `UPDATE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                                ndkBuild.dataNewSql(event, "Update");
                                            }
                                        },
                                        {
                                            name: `DELETE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                                ndkBuild.dataNewSql(event, "Delete");
                                            }
                                        }
                                    ]
                                },
                                'separator',
                                'autoSizeAll',
                                'resetColumns',
                                {
                                    name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Alt+M',
                                    action: function () {
                                        domGridExecuteSql.classList.toggle('nrc-fullscreen');
                                        ndkFn.size();
                                    }
                                },
                                'separator',
                                'chartRange'
                            ];

                            return result;
                        },
                        onGridReady: gridReady
                    });
                    //Info
                    if (itemn != "Item1") {
                        opsExecuteSql.columnDefs.forEach(colDef => {
                            delete colDef.width;
                            colDef.flex = 1;
                        });
                        delete opsExecuteSql.onGridReady;
                        opsExecuteSql.rowGroupPanelShow = "never";
                        opsExecuteSql.headerHeight = 0;
                        opsExecuteSql.getContextMenuItems = () => [
                            'copy',
                            'csvExport',
                            'excelExport',
                            'separator',
                            'autoSizeAll',
                            'resetColumns',
                            {
                                name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Alt+M',
                                action: function () {
                                    domGridExecuteSql.classList.toggle('nrc-fullscreen');
                                    ndkFn.size();
                                }
                            }
                        ];
                    }

                    //存储
                    tpobj.grids.push({
                        tpkey: tabpanel,
                        domTab,
                        domTabPanel,
                        domFilterExecuteSql,
                        domGridExecuteSql,
                        opsExecuteSql
                    });
                }
            }

            resolve2();
        }).then(() => {
            //默认呈现第一个结果
            ndkTab.tabNavFix();
            //无结果优先显示信息
            if (tpobj.grids.length == 2 && tpobj.grids[0].opsExecuteSql.columnDefs.length == 0) {
                tpobj.domTabGroup3.show(tpobj.grids[1].tpkey);
            } else {
                tpobj.domTabGroup3.show(tpobj.grids[0].tpkey);
            }

            ndkFn.themeGrid(ndkVary.theme);
            //切换面板时再呈现表
            ndkFn.size();

            resolve(tpobj);
        })
    })

}

export { ndkDb }