import { agg } from './agg';
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
            console.warn(res);
            ndkFn.requestStatus(false);
            if (res.code == 200) {
                resolve(res);
            } else {
                ndkFn.msg(res.msg);
                reject(res);
            }
        }).catch(err => {
            ndkFn.confirm(err);
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
            columnDefs: [
                { field: 'id', headerName: ndkVary.icons.id, width: 150, editable: false, checkboxSelection: true, headerCheckboxSelection: true, },
                { field: 'alias', headerName: ndkVary.icons.connConn + "连接名", width: 350, rowDrag: true, },
                {
                    field: 'type', headerName: ndkVary.icons.connType + "类型", enableRowGroup: true, width: 160,
                    cellRenderer: params => params.value ? ndkVary.iconDB(params.value) + params.value : "",
                    cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                        values: ndkVary.typeDB, formatValue: fv => ndkVary.iconDB(fv) + fv
                    }
                },
                { field: 'group', headerName: ndkVary.icons.connGroup + "分组", width: 160, enableRowGroup: true },
                {
                    field: 'env', headerName: ndkVary.icons.connEnv + "环境", width: 160, enableRowGroup: true,
                    cellRenderer: params => params.value ? ndkVary.iconEnv(params.value) + params.value : "",
                    cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                        values: ndkVary.typeEnv, formatValue: fv => ndkVary.iconEnv(fv) + fv
                    }
                },
                { field: 'order', headerName: ndkVary.icons.connOrder + "排序" },
                { field: 'conn', headerName: ndkVary.icons.connConn + "连接字符串", width: 600, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                {
                    headerName: ndkVary.icons.ctrl + "操作", pinned: 'right', width: 100, hide: true, filter: false, sortable: false, editable: false, menuTabs: false,
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
            autoGroupColumnDef: agg.autoGroupColumnDef(),
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
            onCellValueChanged: function () {
                //编辑连接信息
                ndkLs.connsSet(agg.getAllRows(ndkVary.gridOpsConns, true));
            },
            onCellDoubleClicked: function (event) {
                //双击ID打开
                if (event.colDef.field == "id") {
                    //切换连接
                    if (ndkStep.cpGet(1).cobj.id != event.node.data.id) {
                        ["database", "table", "column"].forEach(vkey => ndkDb.removeGrid(vkey))
                        ndkStep.cpInfo(1);
                    }
                    ndkStep.cpSet(1, event.node.data); //记录连接
                    ndkDb.reqDatabaseName(event.node.data).then(databases => {
                        ndkDb.viewDatabase(databases).then(() => {
                            ndkStep.cpInfo(1); //显示连接
                            ndkVary.domTabGroup1.show('tp1-database')
                        })
                    })
                }
            },
            //连接右键菜单
            getContextMenuItems: (event) => {
                var enode = event.node, edata = enode?.data;

                //新增连接
                var adddbs = [];
                ndkVary.typeDB.forEach(type => {
                    adddbs.push({
                        name: type,
                        icon: ndkVary.iconDB(type),
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
                        icon: ndkVary.iconDB(dc.type),
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

                var result = [
                    {
                        name: '新建查询', icon: ndkVary.icons.comment, disabled: edata == null,
                        action: function () {
                            //打开选项卡
                            ndkTab.tabOpen(ndkFn.random(), ndkVary.icons.connConn + edata.alias, 'sql').then(tpkey => {
                                ndkStep.cpSet(tpkey, edata); //记录连接
                                ndkStep.cpInfo(tpkey); //显示连接
                            })
                        }
                    },
                    'separator',
                    {
                        name: '打开连接', icon: ndkVary.icons.connDatabase, disabled: edata == null,
                        action: function () {
                            //切换连接
                            if (ndkStep.cpGet(1).cobj.id != event.node.data.id) {
                                ["database", "table", "column"].forEach(vkey => ndkDb.removeGrid(vkey))
                            }
                            ndkStep.cpSet(1, event.node.data); //记录连接
                            ndkDb.reqDatabaseName(event.node.data).then(databases => {
                                ndkDb.viewDatabase(databases).then(() => {
                                    ndkStep.cpInfo(1); //显示连接
                                    ndkVary.domTabGroup1.show('tp1-database')
                                })
                            })
                        }
                    },
                    {
                        name: '创建连接', icon: ndkVary.icons.connConn,
                        subMenu: adddbs
                    },
                    {
                        name: '示例连接',
                        subMenu: demodbs
                    },
                    {
                        name: '编辑连接', icon: ndkVary.icons.edit, disabled: edata == null,
                        action: function () {
                            event.api.startEditingCell({
                                rowIndex: event.node.rowIndex,
                                colKey: event.column.colId,
                            });
                        }
                    },
                    {
                        name: '复制连接', icon: ndkVary.iconGrid('copy'), disabled: edata == null,
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
                        name: '删除连接', icon: ndkVary.icons.remove, disabled: edata == null,
                        subMenu: [
                            {
                                name: "确定", icon: ndkVary.icons.ok,
                                action: function () {
                                    ndkVary.gridOpsConns.api.applyTransaction({
                                        remove: [edata]
                                    });
                                    ndkLs.connsDelete(edata.id);
                                }
                            }
                        ]
                    },
                    {
                        name: '导出', icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "导出 JSON", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Markdown", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Custom", icon: ndkVary.iconGrid('save'),
                            }
                        ]
                    },
                    'separator',
                    {
                        name: '刷新', icon: ndkVary.icons.loading,
                        action: function () {
                            ndkDb.reqConns().then(conns => {
                                ndkDb.viewConns(conns)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: '全屏切换', icon: ndkVary.iconGrid("maximize"),
                        action: function () {
                            if (ndkVary.domGridConns.classList.contains("ag-fullscreen")) {
                                ndkVary.domGridConns.classList.remove("ag-fullscreen")
                            } else {
                                ndkVary.domGridConns.classList.add("ag-fullscreen")
                            }

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
                    headerName: '数据库信息',
                    children: [
                        { headerName: ndkVary.icons.id, valueGetter: "node.rowIndex + 1", width: 120, checkboxSelection: true, headerCheckboxSelection: true, sortable: false, filter: false, menuTabs: false },
                        { field: 'DatabaseName', headerName: ndkVary.icons.connDatabase + "库名", width: 350 },
                        { field: 'DatabaseClassify', headerName: "类别", enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseOwner', headerName: "所属者", enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseSpace', headerName: "表空间", enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseCharset', headerName: "字符集", enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseCollation', headerName: "排序规则", width: 180, enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseDataLength', headerName: "数据大小", columnGroupShow: 'open', cellRenderer: params => params.value > 0 ? ndkFn.formatByteSize(params.value) : "" },
                        { field: 'DatabaseLogLength', headerName: "日志大小", columnGroupShow: 'open', cellRenderer: params => params.value > 0 ? ndkFn.formatByteSize(params.value) : "" },
                        { field: 'DatabaseIndexLength', headerName: "索引大小", columnGroupShow: 'open', cellRenderer: params => params.value > 0 ? ndkFn.formatByteSize(params.value) : "" },
                        { field: 'DatabasePath', headerName: "库路径", width: 400, columnGroupShow: 'open' },
                        { field: 'DatabaseLogPath', headerName: "日志路径", width: 400, columnGroupShow: 'open' },
                        { field: 'DatabaseCreateTime', headerName: "创建时间", width: 200, columnGroupShow: 'open' },
                    ]
                }
            ],
            autoGroupColumnDef: agg.autoGroupColumnDef(),
            //双击库打开表
            onRowDoubleClicked: function (event) {
                if (!event.node.group) {
                    var cp = ndkStep.cpGet(1);
                    ndkStep.cpSet(1, cp.cobj, event.node.data.DatabaseName); //记录连接
                    ndkDb.reqTable(cp.cobj, event.node.data.DatabaseName).then(tables => {
                        ndkDb.viewTable(tables, cp.cobj).then(() => {
                            ndkStep.cpInfo(1); //显示连接
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
                            rows.push(row.data);
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
            //连接菜单项
            getContextMenuItems: (event) => {
                var edata = event.node ? event.node.data : null;

                var result = [
                    {
                        name: '新建查询', icon: ndkVary.icons.comment, disabled: edata == null,
                        action: function () {
                            //打开选项卡
                            ndkTab.tabOpen(ndkFn.random(), ndkVary.icons.connDatabase + edata.DatabaseName, 'sql').then(tpkey => {
                                var cp = ndkStep.cpGet(1);
                                ndkStep.cpSet(tpkey, cp.cobj, edata.DatabaseName); //记录连接
                                ndkStep.cpInfo(tpkey); //显示连接
                            })
                        }
                    },
                    'separator',
                    {
                        name: '打开数据库', icon: ndkVary.icons.connTable, disabled: edata == null,
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            ndkStep.cpSet(1, cp.cobj, edata.DatabaseName); //记录连接
                            ndkDb.reqTable(cp.cobj, edata.DatabaseName).then(tables => {
                                ndkDb.viewTable(tables, cp.cobj).then(() => {
                                    ndkStep.cpInfo(1); //显示连接
                                    ndkVary.domTabGroup1.show('tp1-table')
                                })
                            })
                        }
                    },
                    {
                        name: '导出', icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "导出 JSON", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Markdown", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Custom", icon: ndkVary.iconGrid('save'),
                            }
                        ]
                    },
                    {
                        name: '导入', icon: ndkVary.iconGrid('asc'),
                        action: function () {

                        }
                    },
                    'separator',
                    {
                        name: '刷新', icon: ndkVary.icons.loading,
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
                        name: '全屏切换', icon: ndkVary.iconGrid("maximize"),
                        action: function () {
                            if (ndkVary.domGridDatabase.classList.contains("ag-fullscreen")) {
                                ndkVary.domGridDatabase.classList.remove("ag-fullscreen")
                            } else {
                                ndkVary.domGridDatabase.classList.add("ag-fullscreen")
                            }

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
        if (databases.length > ndkVary.config.autoFilterDatabaseNumber) {
            var cd = ndkStep.cpGet(1);
            var dbkey = cd.cobj.type == "Oracle" ? "user id=" : "database=", databaseName, dbs = cd.cobj.conn.split(';').filter(kv => kv.toLowerCase().startsWith(dbkey));
            if (dbs.length) {
                databaseName = dbs[0].split('=').pop();

                //过滤库名
                ndkVary.gridOpsDatabase.api.setFilterModel({
                    DatabaseName: {
                        type: 'set',
                        values: [databaseName]
                    }
                });
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
                { headerName: ndkVary.icons.id, valueGetter: "node.rowIndex + 1", width: 120, checkboxSelection: true, headerCheckboxSelection: true, sortable: false, filter: false, menuTabs: false },
                {
                    field: 'TableName', tooltipField: "TableName", headerName: ndkVary.icons.connTable + "表名", width: 350,
                    cellRenderer: params => {
                        if (isSQLServer) {
                            return `${params.data.TableSchema}.${params.value}`;
                        }
                        return params.value;
                    }
                },
                { field: 'TableComment', tooltipField: "TableComment", headerName: ndkVary.icons.comment + "表注释", width: 290, hide: isSQLite, editable: !isSQLite, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                { field: 'TableRows', headerName: "行数" },
                {
                    field: 'TableDataLength', headerName: "数据大小", width: 160,
                    cellRenderer: params => ndkFn.formatByteSize(params.value)
                },
                {
                    field: 'TableIndexLength', headerName: "索引大小", width: 160,
                    cellRenderer: params => ndkFn.formatByteSize(params.value)
                },
                { field: 'TableCollation', headerName: "字符集", width: 180, },
                { field: 'TableCreateTime', headerName: "创建时间", width: 220, },
                { field: 'TableModifyTime', headerName: "修改时间", width: 220, },
                { field: 'TableSchema', headerName: "Schema" },
                { field: 'TableType', headerName: "分类" },
            ],
            //双击表打开列
            onCellDoubleClicked: function (event) {
                //非表注释打开列
                if (event.column.colId != "TableComment") {
                    var cp = ndkStep.cpGet(1);
                    ndkStep.cpSet(1, cp.cobj, cp.databaseName, event.node.data.TableName); //记录连接
                    ndkDb.reqColumn(cp.cobj, cp.databaseName, event.node.data.TableName).then(columns => {
                        ndkDb.viewColumn(columns, cp.cobj).then(() => {
                            ndkStep.cpInfo(1); //显示连接
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
            //表菜单项
            getContextMenuItems: (event) => {
                var edata = event.node ? event.node.data : null;
                var result = [
                    {
                        name: '表设计', disabled: event.node?.data == null, icon: ndkVary.iconGrid('columns'),
                        action: function () {
                            if (!event.node.isSelected()) {
                                event.node.setSelected(true);
                            }

                            var srows = ndkVary.gridOpsTable.api.getSelectedRows();
                            var arows = agg.getAllRows(ndkVary.gridOpsTable);
                            var filterTable = "";
                            if (arows.length != srows.length) {
                                filterTable = srows.map(x => x.TableName).join(',')
                            }

                            var cp = ndkStep.cpGet(1);
                            ndkStep.cpSet(1, cp.cobj, cp.databaseName, filterTable); //记录连接
                            ndkDb.reqColumn(cp.cobj, cp.databaseName, filterTable).then(columns => {
                                ndkDb.viewColumn(columns, cp.cobj).then(() => {
                                    ndkStep.cpInfo(1); //显示连接
                                    ndkVary.domTabGroup1.show('tp1-column')
                                })
                            })
                        }
                    },
                    {
                        name: '表数据', icon: ndkVary.iconGrid('grip'), disabled: event.node?.data == null, tooltip: "查询表数据",
                        action: function () {
                            event.api.deselectAll();
                            event.node.setSelected(true);

                            //打开选项卡
                            ndkTab.tabOpen(ndkFn.random(), ndkVary.icons.connTable + edata.TableName, 'sql').then(tpkey => {
                                var tpobj = ndkTab.tabKeys[tpkey];

                                tpobj.editor.setValue("-- 正在生成脚本");

                                var cp = ndkStep.cpGet(1);
                                ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName, edata.TableName); //记录连接
                                ndkStep.cpInfo(tpkey); //显示连接

                                var tpcp = ndkStep.cpGet(tpkey);
                                //构建SQL
                                var sql = ndkBuild.buildSelectSql(tpcp, edata);
                                sql = ndkEditor.formatterSQL(sql, cp.cobj.type); //格式化
                                tpobj.editor.setValue(sql);

                                //执行SQL
                                ndkTab.tabEditorExecuteSql(tpkey)
                            })
                        }
                    },
                    {
                        name: '生成 SQL', icon: ndkVary.icons.generate, disabled: event.node?.data == null,
                        subMenu: [
                            {
                                name: `SELECT`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    //打开选项卡
                                    ndkTab.tabOpen(ndkFn.random(), ndkVary.icons.connTable + edata.TableName, 'sql').then(tpkey => {
                                        var tpobj = ndkTab.tabKeys[tpkey];

                                        tpobj.editor.setValue("-- 正在生成脚本");

                                        var cp = ndkStep.cpGet(1);
                                        ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName, edata.TableName); //记录连接
                                        ndkStep.cpInfo(tpkey); //显示连接
                                        //请求列
                                        ndkDb.reqColumn(cp.cobj, cp.databaseName, edata.TableName, true).then(columns => {
                                            var tpcp = ndkStep.cpGet(tpkey);
                                            //构建SQL
                                            var sql = ndkBuild.buildSelectSql(tpcp, edata, columns);
                                            sql = ndkEditor.formatterSQL(sql, cp.cobj.type); //格式化
                                            tpobj.editor.setValue(sql);
                                        })
                                    })
                                }
                            },
                            {
                                name: `INSERT`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    //打开选项卡
                                    ndkTab.tabOpen(ndkFn.random(), ndkVary.icons.connTable + edata.TableName, 'sql').then(tpkey => {
                                        var tpobj = ndkTab.tabKeys[tpkey];

                                        tpobj.editor.setValue("-- 正在生成脚本");

                                        var cp = ndkStep.cpGet(1);
                                        ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName, edata.TableName); //记录连接
                                        ndkStep.cpInfo(tpkey); //显示连接
                                        //请求列
                                        ndkDb.reqColumn(cp.cobj, cp.databaseName, edata.TableName, true).then(columns => {
                                            var tpcp = ndkStep.cpGet(tpkey);
                                            //构建SQL
                                            var sql = ndkBuild.buildInsertSql(tpcp, edata, columns);
                                            sql = ndkEditor.formatterSQL(sql, cp.cobj.type); //格式化
                                            tpobj.editor.setValue(sql);
                                        })
                                    })
                                }
                            },
                            {
                                name: `UPDATE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    //打开选项卡
                                    ndkTab.tabOpen(ndkFn.random(), ndkVary.icons.connTable + edata.TableName, 'sql').then(tpkey => {
                                        var tpobj = ndkTab.tabKeys[tpkey];

                                        tpobj.editor.setValue("-- 正在生成脚本");

                                        var cp = ndkStep.cpGet(1);
                                        ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName, edata.TableName); //记录连接
                                        ndkStep.cpInfo(tpkey); //显示连接
                                        //请求列
                                        ndkDb.reqColumn(cp.cobj, cp.databaseName, edata.TableName, true).then(columns => {
                                            var tpcp = ndkStep.cpGet(tpkey);
                                            //构建SQL
                                            var sql = ndkBuild.buildUpdateSql(tpcp, edata, columns);
                                            sql = ndkEditor.formatterSQL(sql, cp.cobj.type); //格式化
                                            tpobj.editor.setValue(sql);
                                        })
                                    })
                                }
                            },
                            {
                                name: `DELETE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                    //打开选项卡
                                    ndkTab.tabOpen(ndkFn.random(), ndkVary.icons.connTable + edata.TableName, 'sql').then(tpkey => {
                                        var tpobj = ndkTab.tabKeys[tpkey];

                                        tpobj.editor.setValue("-- 正在生成脚本");

                                        var cp = ndkStep.cpGet(1);
                                        ndkStep.cpSet(tpkey, cp.cobj, cp.databaseName, edata.TableName); //记录连接
                                        ndkStep.cpInfo(tpkey); //显示连接
                                        //请求列
                                        ndkDb.reqColumn(cp.cobj, cp.databaseName, edata.TableName, true).then(columns => {
                                            var tpcp = ndkStep.cpGet(tpkey);
                                            //构建SQL
                                            var sql = ndkBuild.buildDeleteSql(tpcp, edata, columns);
                                            sql = ndkEditor.formatterSQL(sql, cp.cobj.type); //格式化
                                            tpobj.editor.setValue(sql);
                                        })
                                    })
                                }
                            },
                            {
                                name: `TRUNCATE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                }
                            },
                            {
                                name: `DROP`, icon: ndkVary.iconGrid('paste'), action: function () {
                                }
                            },
                            'separator',
                            {
                                name: `结构 DDL`, icon: ndkVary.iconGrid('paste'), action: function () {
                                }
                            },
                            {
                                name: `结构和数据`, icon: ndkVary.iconGrid('paste'), action: function () {
                                }
                            },
                        ]
                    },
                    {
                        name: '导出', icon: ndkVary.iconGrid('desc'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "导出 JSON", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Markdown", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Custom", icon: ndkVary.iconGrid('save'),
                            }
                        ]
                    },
                    {
                        name: '导入', icon: ndkVary.iconGrid('asc'),
                    },
                    'separator',
                    {
                        name: '刷新', icon: ndkVary.icons.loading,
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
                        name: '全屏切换', icon: ndkVary.iconGrid("maximize"),
                        action: function () {
                            if (ndkVary.domGridTable.classList.contains("ag-fullscreen")) {
                                ndkVary.domGridTable.classList.remove("ag-fullscreen")
                            } else {
                                ndkVary.domGridTable.classList.add("ag-fullscreen")
                            }

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
            getRowNodeId: data => data.ColumnName, //指定行标识列
            groupSelectsChildren: true, //选择子项
            //列
            columnDefs: [
                { headerName: ndkVary.icons.id, valueGetter: "node.rowIndex + 1", width: 120, checkboxSelection: true, headerCheckboxSelection: true, sortable: false, filter: false, menuTabs: false, hide: true },
                { field: 'TableName', headerName: ndkVary.icons.connTable + "表名", rowGroup: true, enableRowGroup: true, width: 350, hide: true, },
                { field: 'TableComment', headerName: ndkVary.icons.connTable + "表注释", width: 300, hide: true },
                { field: 'ColumnName', headerName: ndkVary.icons.connColumn + "列名", width: 200, hide: true, },
                { field: 'ColumnComment', tooltipField: "ColumnComment", headerName: ndkVary.icons.comment + "列注释", width: 330, hide: isSQLite, editable: !isSQLite, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                { field: 'ColumnType', headerName: "类型及长度", width: 150, hide: true },
                { field: 'DataType', headerName: "类型", width: 120, hide: true },
                { field: 'DataLength', headerName: "长度", hide: true },
                { field: 'DataScale', headerName: "精度", hide: true },
                { field: 'ColumnOrder', headerName: "列序" },
                {
                    field: 'PrimaryKey', headerName: "主键", hide: true,
                    cellRenderer: params => params.value > 0 ? ndkVary.icons.key + params.value : ""
                },
                { field: 'AutoIncr', headerName: "自增", cellRenderer: params => params.value == 1 ? ndkVary.icons.incr : "" },
                {
                    field: 'IsNullable', headerName: "必填", hide: true,
                    cellRenderer: params => params.value == 1 ? "" : ndkVary.icons.edit
                },
                { field: 'ColumnDefault', headerName: "默认值", width: 120, },
            ],
            autoGroupColumnDef: agg.autoGroupColumnDef({
                field: "ColumnName", headerName: "分组", width: 560,
                headerCheckboxSelection: true,
                headerCheckboxSelectionFilteredOnly: true,
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
            onCellValueChanged: function (event) {
                //修改列注释
                if (event.column.colId == "ColumnComment") {
                    var cp = ndkStep.cpGet(1);
                    ndkDb.setColumnComment(cp.cobj, cp.databaseName, event.data.TableName, event.data.ColumnName, event.value).then(() => {
                        //更新本地数据
                    })
                }
            },
            //列右键菜单项
            getContextMenuItems: (params) => {
                var result = [
                    {
                        // custom item
                        name: 'Alert ' + params.value,
                        action: function () {
                            window.alert('Alerting at ' + params.value);
                        },
                        cssClasses: ['redFont', 'bold'],
                    },
                    {
                        // custom item
                        name: 'Always Disabled',
                        disabled: true,
                        tooltip: 'Very long tooltip, did I mention that I am very long, well I am! Long!  Very Long!',
                    },
                    {
                        name: '导出', icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "导出 JSON", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Markdown", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                            },
                            {
                                name: "导出 Custom", icon: ndkVary.iconGrid('save'),
                            },
                        ]
                    },
                    'expandAll',
                    'contractAll',
                    'separator',
                    {
                        name: '刷新', icon: ndkVary.icons.loading,
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
                        name: '全屏切换', icon: ndkVary.iconGrid("maximize"),
                        action: function () {
                            if (ndkVary.domGridColumn.classList.contains("ag-fullscreen")) {
                                ndkVary.domGridColumn.classList.remove("ag-fullscreen")
                            } else {
                                ndkVary.domGridColumn.classList.add("ag-fullscreen")
                            }

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
        setTimeout(() => {
            ndkVary.gridOpsColumn.api.forEachNode((node) => {
                if (node.level == 0) {
                    node.setExpanded(true);
                }
            });
        }, 300);

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
        tpobj.domTabGroup3.innerHTML = '';
        if (!tpobj.domTabGroup3.getAttribute("data-bind")) {
            tpobj.domTabGroup3.setAttribute("data-bind", true);

            //选项卡3-关闭
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
            //选项卡3-显示
            tpobj.domTabGroup3.addEventListener('sl-tab-show', function (event) {
                setTimeout(() => {
                    ndkFn.size()
                }, 1)
            }, false);
        }

        new Promise(resolve2 => {
            for (const itemn in esdata) {
                //跳过表结构
                if (itemn == "Item2") {
                    continue;
                }
                var ti = 0;
                for (const iname in esdata[itemn]) {
                    var esrows = esdata[itemn][iname], columnDefs = [];

                    var tabname = itemn == "Item1" ? ndkVary.icons.data + "Result" : ndkVary.icons.info + "Info", tabpanel = `tp3-${itemn}-${ti}`;
                    if (ti != 0) {
                        tabname += " - " + ti;
                    }

                    //结构
                    var tabbox = document.createElement("div");
                    tabbox.innerHTML = `
<sl-tab slot="nav" panel="${tabpanel}" closable>${tabname}</sl-tab>
<sl-tab-panel name="${tabpanel}">
    <sl-input class="nr-filter-execute-sql mb-2" placeholder="搜索" size="small"></sl-input>
    <div class="nr-grid-execute-sql"></div>
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

                    //填充列头
                    if (esrows.length > 0) {
                        if (itemn == "Item1") {
                            columnDefs.push({ headerName: ndkVary.icons.id, valueGetter: "node.rowIndex + 1", width: 120, checkboxSelection: true, headerCheckboxSelection: true, sortable: false, filter: false, menuTabs: false });
                        }

                        var esrow = esrows[0];
                        for (var field in esrow) {
                            columnDefs.push({ field: field, headerName: field, headerTooltip: field, enableRowGroup: true });
                        }
                    } else if (iname in (esdata.Item2 || {})) {
                        for (const coln in esdata.Item2[iname]) {
                            var col = esdata.Item2[iname][coln];
                            columnDefs.push({ headerName: col.ColumnName, field: col.ColumnName, headerTooltip: field });
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
                                    name: '导出', icon: ndkVary.iconGrid('save'),
                                    subMenu: [
                                        'csvExport',
                                        'excelExport',
                                        'separator',
                                        {
                                            name: "导出 JSON", icon: ndkVary.iconGrid('save'),
                                        },
                                        {
                                            name: "导出 Markdown", icon: ndkVary.iconGrid('save'),
                                        },
                                        {
                                            name: "导出 Markdown（Copy）", icon: ndkVary.iconGrid('save'),
                                        },
                                        {
                                            name: "导出 Custom", icon: ndkVary.iconGrid('save'),
                                        }
                                    ]
                                },
                                {
                                    name: '生成 SQL', icon: ndkVary.icons.generate,
                                    subMenu: [
                                        {
                                            name: `INSERT`, icon: ndkVary.iconGrid('paste'), action: function () {
                                            }
                                        },
                                        {
                                            name: `UPDATE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                            }
                                        },
                                        {
                                            name: `DELETE`, icon: ndkVary.iconGrid('paste'), action: function () {
                                            }
                                        }
                                    ]
                                },
                                'separator',
                                'autoSizeAll',
                                'resetColumns',
                                {
                                    name: '全屏切换', icon: ndkVary.iconGrid("maximize"),
                                    action: function () {
                                        if (domGridExecuteSql.classList.contains("ag-fullscreen")) {
                                            domGridExecuteSql.classList.remove("ag-fullscreen")
                                        } else {
                                            domGridExecuteSql.classList.add("ag-fullscreen")
                                        }

                                        ndkFn.size();
                                    }
                                },
                                'separator',
                                'chartRange'
                            ];

                            return result;
                        },
                        onGridReady: function (event) {
                            event.columnApi.autoSizeAllColumns(); //列宽自动大小
                            ndkFn.size();
                        }
                    });
                    if (itemn != "Item1") {
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
                                name: '全屏切换', icon: ndkVary.iconGrid("maximize"),
                                action: function () {
                                    if (domGridExecuteSql.classList.contains("ag-fullscreen")) {
                                        domGridExecuteSql.classList.remove("ag-fullscreen")
                                    } else {
                                        domGridExecuteSql.classList.add("ag-fullscreen")
                                    }

                                    ndkFn.size();
                                }
                            }
                        ]
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

                    ti++;
                }
            }

            //默认呈现第一个结果
            setTimeout(() => {
                ndkTab.tabNavFix();
                //无结果优先显示信息
                if (tpobj.grids.length == 2 && tpobj.grids[0].opsExecuteSql.columnDefs.length == 0) {
                    tpobj.domTabGroup3.show(tpobj.grids[1].tpkey);
                } else {
                    tpobj.domTabGroup3.show(tpobj.grids[0].tpkey);
                }
                resolve2();
            }, 10)
        }).then(() => {
            ndkFn.themeGrid(ndkVary.theme);
            //切换面板时再呈现表
            ndkFn.size();

            resolve(esdata);
        })
    })

}

export { ndkDb }