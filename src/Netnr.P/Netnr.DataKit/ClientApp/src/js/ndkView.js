import { agg } from './agg';
import { ndkI18n } from './ndkI18n';
import { ndkFunction } from './ndkFunction';
import { ndkStorage } from './ndkStorage';
import { ndkVary } from './ndkVary';
import { ndkStep } from './ndkStep';
import { ndkTab } from './ndkTab';
import { ndkGenerateSQL } from './ndkGenerateSQL';
import { ndkEditor } from './ndkEditor';
import { ndkRequest } from './ndkRequest';
import { ndkExecute } from './ndkExecute';
import { ndkAction } from './ndkAction';

// 显示
var ndkView = {

    /**
     * 创建
     * @param {*} vkey 接收对象key
     * @param {*} gridOps 表格配置
     */
    createGrid: (vkey, gridOps) => {
        ndkView.removeGrid(vkey);

        vkey = ndkFunction.hump(vkey);
        var vgrid = `gridOps${vkey}`, vdom = `domGrid${vkey}`;
        ndkAction.themeGrid(ndkVary.theme);
        ndkAction.size();

        ndkVary[vgrid] = new agGrid.Grid(ndkVary[vdom], gridOps).gridOptions;
    },
    /**
     * 移除
     * @param {any} vkey
     */
    removeGrid: vkey => {
        vkey = ndkFunction.hump(vkey);
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
                var dom = agg.getContainer(params);
                var ihtm = `${cp.cobj.type} ${ndkVary.emoji.link}${cp.cobj.alias}`;
                if (cp.databaseName && !dom.classList.contains("nr-grid-database")) {
                    ihtm = `${ihtm} ${ndkVary.emoji.bucket}${cp.databaseName}`;
                }

                this.eGui = document.createElement('div');
                this.eGui.style.cssText = `display:flex;align-items:center;${color}`;
                this.eGui.innerHTML = `<small title="${cp.cobj.conn}" style="white-space: nowrap;">${ihtm}</small></sl-tooltip>`;
            }
            getGui() {
                return this.eGui
            }
        },
    },

    /**
     * 显示连接
     * @param {*} conns 
     */
    viewConns: (conns) => new Promise(resolve => {
        var opsConns = agg.optionDef({
            rowData: conns,//数据源
            getRowId: event => event.data.id, //指定行标识列
            defaultColDef: agg.defaultColDef({ editable: true }),
            suppressClickEdit: true, //禁点击编辑
            columnDefs: [
                {
                    field: 'alias', headerName: ndkI18n.lg.connAlias, tooltipField: 'conn', width: 350,
                    checkboxSelection: true, headerCheckboxSelection: true, headerCheckboxSelectionFilteredOnly: true, //仅全选过滤的数据行
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
                },
                { field: 'group', headerName: ndkI18n.lg.connGroup, width: 160, enableRowGroup: true },
                {
                    field: 'env', headerName: ndkI18n.lg.connEnv, width: 160, enableRowGroup: true,
                    cellRenderer: params => params.value ? ndkVary.iconEnv(params.value) + params.value : "",
                    cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                        values: ndkVary.typeEnv, formatValue: fv => ndkVary.iconEnv(fv) + fv
                    }
                },
                {
                    field: 'type', headerName: ndkI18n.lg.connType, enableRowGroup: true, width: 160,
                    cellRenderer: params => params.value ? ndkVary.iconSvg(params.value.toLowerCase(), params.value, { library: "nr-icon" }) : "",
                    cellEditor: 'agRichSelectCellEditor', cellEditorParams: { values: ndkVary.typeDB }
                },
                {
                    field: 'order', headerName: ndkI18n.lg.connOrder, rowDrag: true,
                    filterParams: agg.filterParamsDef("Number")
                },
                { field: 'id', headerName: ndkVary.emoji.id, width: 150, editable: false },
                { field: 'conn', headerName: ndkI18n.lg.connConnection, width: 600, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                {
                    headerName: ndkI18n.lg.connControl, pinned: 'right', width: 100, hide: true, filter: false, sortable: false, editable: false, menuTabs: false,
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
                                        newrow.id = ndkFunction.random(20000, 99999);
                                        newrow.alias += "+";

                                        ndkVary.gridOpsConns.api.applyTransaction({
                                            add: [newrow],
                                            addIndex: params.rowIndex + 1
                                        });

                                        ndkStorage.connsSet(newrow);
                                    } else if (target.classList.contains("nr-conn-cell-del")) {
                                        //删除连接
                                        if (confirm("确定删除？")) {
                                            ndkVary.gridOpsConns.api.applyTransaction({
                                                remove: [params.data]
                                            });
                                            ndkStorage.connsDelete(params.data.id);
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

                ndkStorage.connsSet(uprow);
            },
            // 单元格变动
            onCellValueChanged: function (event) {
                //类型变动
                if (event.column.colId == "type") {
                    event.api.refreshCells({ rowNodes: [event.data], force: true })
                }
                //编辑连接信息
                ndkStorage.connsSet(agg.getAllRows(ndkVary.gridOpsConns, true));
            },
            // 双击行连接
            onRowDoubleClicked: function (event) {
                //切换连接
                if (ndkStep.cpGet(1).cobj.id != event.data.id) {
                    ["database", "table", "column"].forEach(vkey => ndkView.removeGrid(vkey))
                }
                ndkStep.cpSet(1, event.data); //记录连接
                ndkRequest.reqDatabaseName(event.data).then(databases => {
                    ndkView.viewDatabase(databases).then(() => {
                        ndkVary.domTabGroup1.show('tp1-database')
                    })
                })
            },
            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.ctrlKey && event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridConns.classList.toggle('nrc-fullscreen');
                    ndkAction.size();
                }
            },
            //连接右键菜单
            getContextMenuItems: (event) => {
                var enode = event.node, edata = enode && enode.data,
                    srows = ndkVary.gridOpsConns.api.getSelectedRows();

                //新增连接
                var adddbs = [];
                ndkVary.typeDB.forEach(type => {
                    adddbs.push({
                        name: type,
                        icon: ndkVary.iconSvg(type),
                        action: function () {
                            var order = enode ? enode.rowIndex + 1 : agg.getAllRows(ndkVary.gridOpsConns, true).length;

                            var newrow = {
                                id: ndkFunction.random(20000, 99999),
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

                            ndkStorage.connsSet(newrow);
                        }
                    });
                });

                //示例连接
                var demodbs = [];
                ndkVary.resConnDemo.forEach(dc => {
                    demodbs.push({
                        name: dc.alias,
                        icon: ndkVary.iconSvg(dc.type),
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

                                ndkStorage.connsSet(newrow);
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
                        name: `${ndkI18n.lg.confirmDelete}（${srows.length}）`, icon: ndkVary.emoji.ok,
                        action: function () {
                            ndkVary.gridOpsConns.api.applyTransaction({ remove: srows });
                            ndkStorage.connsDelete(srows.map(x => x.id));
                        }
                    });
                    srows.forEach(row => {
                        deletedbs.push({
                            name: ndkVary.iconSvg(row.type, row.alias, { library: "nr-icon" }), icon: ndkVary.iconSvg("trash"),
                            action: function () {
                                ndkVary.gridOpsConns.api.applyTransaction({ remove: [row] });
                                ndkStorage.connsDelete(row.id);
                            }
                        })
                    });
                }

                var result = [
                    {
                        name: ndkI18n.lg.newQuery, icon: ndkVary.emoji.comment, disabled: edata == null,
                        action: function () {
                            //打开选项卡
                            ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("plug-fill", edata.alias), 'sql').then(tpkey => {
                                ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                ndkStep.cpSet(tpkey, edata); //记录连接
                                ndkStep.cpInfo(tpkey); //显示连接
                            })
                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.openConn, icon: ndkVary.iconSvg("plug-fill"), disabled: edata == null,
                        action: function () {
                            //切换连接
                            if (ndkStep.cpGet(1).cobj.id != event.node.data.id) {
                                ["database", "table", "column"].forEach(vkey => ndkView.removeGrid(vkey))
                            }
                            ndkStep.cpSet(1, event.node.data); //记录连接
                            ndkRequest.reqDatabaseName(event.node.data).then(databases => {
                                ndkView.viewDatabase(databases).then(() => {
                                    ndkVary.domTabGroup1.show('tp1-database')
                                })
                            })
                        }
                    },
                    { name: ndkI18n.lg.createConn, icon: ndkVary.iconSvg("plug-fill"), subMenu: adddbs },
                    { name: ndkI18n.lg.demoConn, subMenu: demodbs },
                    {
                        name: ndkI18n.lg.editConn, icon: ndkVary.iconSvg("pencil-square"), disabled: edata == null,
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
                            newrow.id = ndkFunction.random();
                            newrow.alias += "+";

                            ndkVary.gridOpsConns.api.applyTransaction({
                                add: [newrow],
                                addIndex: event.node.rowIndex + 1
                            });

                            ndkStorage.connsSet(newrow);
                        }
                    },
                    {
                        name: ndkI18n.lg.deleteConn, icon: ndkVary.iconSvg("trash"), disabled: deletedbs.length == 0,
                        subMenu: deletedbs
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.refresh, icon: ndkVary.iconSvg("arrow-repeat"),
                        action: function () {
                            ndkRequest.reqConns().then(conns => {
                                ndkView.viewConns(conns)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Ctrl+Alt+M',
                        action: function () {
                            ndkVary.domGridConns.classList.toggle('nrc-fullscreen');
                            ndkAction.size();
                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.save, icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "JSON", icon: ndkVary.iconGrid('save'), action: function () {
                                    ndkVary.pasteContent = JSON.stringify(agg.getAllRows(ndkVary.gridOpsConns));
                                    ndkFunction.clipboard(ndkVary.pasteContent).then(() => {
                                        ndkFunction.output(ndkI18n.lg.copiedToClipboard);
                                    })
                                }
                            }
                        ]
                    },
                    'copy',
                    'copyWithHeaders'
                ];

                return result;
            },
        });

        ndkView.createGrid("conns", opsConns);

        resolve()
    }),

    /**
     * 显示干
     * @param {*} trunks
     * @param {*} cobj 
     * @returns 
     */
    viewTrunk: (trunks, cobj) => new Promise(resolve => {
        trunks = [
            { Level: 0, path: ["Database-1"] },
            { Level: 1, path: ["Database-1", "Schema-1"] },
            { Level: 2, path: ["Database-1", "Schema-1", "Type-1"] },
            { Level: 3, path: ["Database-1", "Schema-1", "Type-1", "Item-1"] },
            { Level: 3, path: ["Database-1", "Schema-1", "Type-1", "Item-2"] },
        ];

        var opsTrunk = agg.optionDef({
            rowData: trunks,//数据源
            getRowId: event => event.data.TableName, //指定行标识列
            rowGroupPanelShow: 'never',
            treeData: true,
            getDataPath: function (data) {
                console.log(arguments)
                return data.path;
            },
            defaultColDef: {
                flex: 1,
            },
            autoGroupColumnDef: {
                headerName: 'Trunk',
                minWidth: 300,
                cellRendererParams: {
                    suppressCount: true,
                },
            },

            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.ctrlKey && event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridTable.classList.toggle('nrc-fullscreen');
                    ndkAction.size();
                }
            },
            //表菜单项
            getContextMenuItems: (event) => {
                var edata = event.node ? event.node.data : null;
                var result = [];

                return result;
            }
        });

        ndkView.createGrid("trunk", opsTrunk);
        ndkVary.domGridTrunk.style.height = "90vh";

        resolve();
    }),

    /**
     * 显示库
     * @param {*} databases 
     */
    viewDatabase: (databases) => new Promise(resolve => {
        var opsDatabase = agg.optionDef({
            rowData: databases,//数据源
            getRowId: event => event.data.DatabaseName, //指定行标识列
            columnDefs: [
                {
                    headerName: ndkI18n.lg.databaseInfo,
                    children: [
                        agg.numberCol(), //行号
                        {
                            field: 'DatabaseName', headerName: ndkI18n.lg.dbName, tooltipField: 'DatabaseName', width: 350,
                            cellRenderer: (params) => {
                                if (!params.node.group) {
                                    return ndkVary.iconSvg("server", params.value);
                                }
                            }
                        },
                        { field: 'DatabaseOwner', headerName: ndkI18n.lg.dbOwner, enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseSpace', headerName: ndkI18n.lg.dbSpace, enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseCharset', headerName: ndkI18n.lg.dbCharSet, enableRowGroup: true, columnGroupShow: 'open' },
                        { field: 'DatabaseCollation', headerName: ndkI18n.lg.dbCollate, width: 260, enableRowGroup: true, columnGroupShow: 'open' },
                        {
                            field: 'DatabaseDataLength', headerName: ndkI18n.lg.dbDataSize, columnGroupShow: 'open', aggFunc: 'sum',
                            cellRenderer: params => params.value > 0 ? ndkFunction.formatByteSize(params.value) : "",
                            filterParams: agg.filterParamsDef("Number")
                        },
                        {
                            field: 'DatabaseLogLength', headerName: ndkI18n.lg.dbLogSize, columnGroupShow: 'open', aggFunc: 'sum',
                            cellRenderer: params => params.value > 0 ? ndkFunction.formatByteSize(params.value) : "",
                            filterParams: agg.filterParamsDef("Number")
                        },
                        {
                            field: 'DatabaseIndexLength', headerName: ndkI18n.lg.dbIndexSize, columnGroupShow: 'open', aggFunc: 'sum',
                            cellRenderer: params => params.value > 0 ? ndkFunction.formatByteSize(params.value) : "",
                            filterParams: agg.filterParamsDef("Number")
                        },
                        { field: 'DatabasePath', headerName: ndkI18n.lg.dbDataPath, width: 400, columnGroupShow: 'open' },
                        { field: 'DatabaseLogPath', headerName: ndkI18n.lg.dbLogPath, width: 400, columnGroupShow: 'open' },
                        {
                            field: 'DatabaseCreateTime', headerName: ndkI18n.lg.createTime, width: 210, columnGroupShow: 'open',
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
                connStatusBarComponent: ndkView.gridComponents.connStatusBarComponent
            },
            //双击库打开表
            onRowDoubleClicked: function (event) {
                if (!event.node.group) {
                    var cp = ndkStep.cpGet(1);
                    ndkStep.cpSet(1, cp.cobj, event.node.data.DatabaseName); //记录连接
                    ndkRequest.reqTable(cp.cobj, event.node.data.DatabaseName).then(tables => {
                        ndkView.viewTable(tables, cp.cobj).then(() => {
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
                    ndkRequest.reqDatabaseInfo(cp.cobj, rows.map(row => row.DatabaseName).join(',')).then(res => {
                        ndkVary.gridOpsDatabase.api.applyTransactionAsync({
                            update: res
                        })
                    });
                }
            },
            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.ctrlKey && event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridDatabase.classList.toggle('nrc-fullscreen');
                    ndkAction.size();
                }
            },
            //菜单项
            getContextMenuItems: (event) => {
                var edata = event.node ? event.node.data : null;

                var result = [
                    {
                        name: ndkI18n.lg.newQuery, icon: ndkVary.emoji.comment, disabled: edata == null,
                        action: function () {
                            //构建选项卡
                            ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("server", edata.DatabaseName), 'sql').then(tpkey => {
                                ndkVary.domTabGroup2.show(tpkey);//显示选项卡
                                var cp = ndkStep.cpGet(1);
                                ndkStep.cpSet(tpkey, cp.cobj, edata.DatabaseName); //记录连接
                                ndkStep.cpInfo(tpkey); //显示连接
                            })
                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.openDatabase, icon: ndkVary.iconSvg("server"), disabled: edata == null,
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            ndkStep.cpSet(1, cp.cobj, edata.DatabaseName); //记录连接
                            ndkRequest.reqTable(cp.cobj, edata.DatabaseName).then(tables => {
                                ndkView.viewTable(tables, cp.cobj).then(() => {
                                    ndkVary.domTabGroup1.show('tp1-table')
                                })
                            })
                        }
                    },
                    {
                        name: ndkI18n.lg.exportData, icon: ndkVary.iconGrid('save'), disabled: true,
                    },
                    {
                        name: ndkI18n.lg.importData, icon: ndkVary.iconGrid('save'), disabled: true,
                    },
                    {
                        name: ndkI18n.lg.executeSqlFile, icon: ndkVary.iconSvg("file-earmark-code"), disabled: true,
                        action: function () {

                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.refresh, icon: ndkVary.iconSvg("arrow-repeat"),
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            ndkRequest.reqDatabaseName(cp.cobj, true).then(databases => {
                                ndkView.viewDatabase(databases)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Ctrl+Alt+M',
                        action: function () {
                            ndkVary.domGridDatabase.classList.toggle('nrc-fullscreen');
                            ndkAction.size();
                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.save, icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "JSON", icon: ndkVary.iconGrid('save'), action: function () {
                                    ndkVary.pasteContent = JSON.stringify(agg.getAllRows(ndkVary.gridOpsDatabase));
                                    ndkFunction.clipboard(ndkVary.pasteContent).then(() => {
                                        ndkFunction.output(ndkI18n.lg.copiedToClipboard);
                                    })
                                }
                            }
                        ]
                    },
                    'copy',
                    'copyWithHeaders',
                    'separator',
                    'chartRange'
                ];

                return result;
            }
        });

        ndkView.setDisplayItem(opsDatabase.columnDefs[0].children); // 设置显示列
        ndkView.createGrid("database", opsDatabase); //创建表格

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
     * 显示表
     * @param {*} tables
     * @param {*} cobj 
     * @returns 
     */
    viewTable: (tables, cobj) => new Promise(resolve => {
        var isSQLite = cobj.type == "SQLite", isJoinSchema = ["SQLServer", "PostgreSQL"].includes(cobj.type);

        var opsTable = agg.optionDef({
            rowData: tables,//数据源
            getRowId: event => `${event.data.SchemaName}.${event.data.TableName}`, //指定行标识列
            rowGroupPanelShow: 'never',
            columnDefs: [
                agg.numberCol(), //行号
                {
                    field: 'TableName', tooltipField: "TableName", headerName: ndkI18n.lg.tableName, width: 350,
                    cellRenderer: params => {
                        var val = params.value;
                        if (isJoinSchema && params.data) {
                            val = `${params.data.SchemaName}.${params.value}`;
                        }
                        return val == null ? "" : ndkVary.iconSvg("table", val);
                    }
                },
                { field: 'TableComment', tooltipField: "TableComment", headerName: ndkI18n.lg.tableComment, width: 290, hide: isSQLite, editable: !isSQLite, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                { field: 'TableRows', headerName: ndkI18n.lg.tableRows, filterParams: agg.filterParamsDef("Number"), aggFunc: 'sum', },
                {
                    field: 'TableDataLength', headerName: ndkI18n.lg.tableDataSize, width: 160, aggFunc: 'sum',
                    cellRenderer: params => ndkFunction.formatByteSize(params.value),
                    filterParams: agg.filterParamsDef("Number")
                },
                {
                    field: 'TableIndexLength', headerName: ndkI18n.lg.tableIndexSize, width: 160, aggFunc: 'sum',
                    cellRenderer: params => ndkFunction.formatByteSize(params.value),
                    filterParams: agg.filterParamsDef("Number")
                },
                { field: 'TableCollation', headerName: ndkI18n.lg.tableCollate, width: 260, },
                {
                    field: 'TableCreateTime', headerName: ndkI18n.lg.createTime, width: 220,
                    filterParams: agg.filterParamsDef("Date")
                },
                {
                    field: 'TableModifyTime', headerName: ndkI18n.lg.updateTime, width: 220,
                    filterParams: agg.filterParamsDef("Date")
                },
                { field: 'TableEngine', headerName: "Engine" },
                { field: 'SchemaName', headerName: ndkI18n.lg.schemaName, width: 200 },
                { field: 'TableOwner', headerName: ndkI18n.lg.tableOwner },
                { field: 'TableSpace', headerName: ndkI18n.lg.tableSpace },
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
                connStatusBarComponent: ndkView.gridComponents.connStatusBarComponent
            },
            //双击表打开列
            onCellDoubleClicked: function (event) {
                //非表注释打开列
                if (event.column.colId != "TableComment") {
                    var edata = event.node.data;
                    var cp = ndkStep.cpGet(1);
                    ndkStep.cpSet(1, cp.cobj, cp.databaseName); //记录连接
                    var filterSNTN = edata.SchemaName ? `${edata.SchemaName}.${edata.TableName}` : edata.TableName;
                    ndkRequest.reqColumn(cp.cobj, cp.databaseName, filterSNTN).then(columns => {
                        ndkView.viewColumn(columns, cp.cobj).then(() => {
                            ndkVary.domTabGroup1.show('tp1-column')
                        })
                    })
                }
            },
            onCellValueChanged: function (event) {
                //修改表注释
                if (event.column.colId == "TableComment") {
                    var cp = ndkStep.cpGet(1), edata = event.data;
                    ndkRequest.setTableComment(cp.cobj, edata.TableName, event.value, edata.SchemaName, cp.databaseName).then(() => {
                        ndkStorage.ccBrushTable(); //更新缓存
                    }).catch(() => {
                        //更新失败，恢复原值
                        var edata = { ...event.data };
                        edata.TableComment = event.oldValue;
                        event.api.applyTransaction({
                            update: [edata]
                        });
                    })
                }
            },
            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.ctrlKey && event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridTable.classList.toggle('nrc-fullscreen');
                    ndkAction.size();
                }
            },
            //表菜单项
            getContextMenuItems: (event) => {
                var edata = event.node ? event.node.data : null;
                var isSQLite = ndkStep.cpGet(1).cobj.type == "SQLite";
                var result = [
                    {
                        name: ndkI18n.lg.tableDesign, disabled: edata == null, icon: ndkVary.iconSvg("layout-three-columns"),
                        action: function () {
                            var srows = ndkVary.gridOpsTable.api.getSelectedRows();
                            //选中的表行或右键行
                            if (srows.length == 0) {
                                srows = [edata];
                            }

                            var filterSNTN = "";
                            if (ndkVary.gridOpsTable.rowData.length != srows.length) {
                                filterSNTN = srows.map(x => x.SchemaName == null ? x.TableName : `${x.SchemaName}.${x.TableName}`).join(',')
                            }

                            var cp = ndkStep.cpGet(1);
                            ndkStep.cpSet(1, cp.cobj, cp.databaseName); //记录连接
                            ndkRequest.reqColumn(cp.cobj, cp.databaseName, filterSNTN).then(columns => {
                                ndkView.viewColumn(columns, cp.cobj).then(() => {
                                    ndkVary.domTabGroup1.show('tp1-column')
                                    ndkStep.stepSave();
                                })
                            })
                        }
                    },
                    {
                        name: ndkI18n.lg.tableData, icon: ndkVary.iconSvg("grid-3x3-gap-fill"), disabled: edata == null,
                        action: function () {
                            var srows = ndkVary.gridOpsTable.api.getSelectedRows();
                            //选中的表行或右键行
                            if (srows.length == 0) {
                                srows = [edata];
                            }

                            //打开选项卡
                            ndkTab.tabBuild(ndkFunction.random(), ndkVary.iconSvg("table", srows[0].TableName), 'sql').then(tpkey => {
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
                                    sqls.push(ndkGenerateSQL.buildSelectSql(tpcp, tableRow));
                                });
                                if (sqls.length == 1) {
                                    sqls = ndkEditor.formatterSQL(sqls[0], cp.cobj.type); //格式化
                                } else {
                                    sqls = sqls.join(';\r\n');
                                }
                                tpobj.editor.setValue(sqls);

                                //执行SQL
                                if (srows.length < 6) {
                                    ndkExecute.editorSql(tpkey)
                                }
                            })
                        }
                    },
                    {
                        name: ndkI18n.lg.tableGenerateSQL, icon: ndkVary.iconSvg("file-earmark-code"),
                        subMenu: [
                            {
                                name: `SELECT`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                    ndkGenerateSQL.buildNewTabSql(edata, 'Select');
                                }
                            },
                            {
                                name: `INSERT`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                    ndkGenerateSQL.buildNewTabSql(edata, 'Insert');
                                }
                            },
                            {
                                name: `UPDATE`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                    ndkGenerateSQL.buildNewTabSql(edata, 'Update');
                                }
                            },
                            {
                                name: `DELETE`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                    ndkGenerateSQL.buildNewTabSql(edata, 'Delete');
                                }
                            },
                            {
                                name: `TRUNCATE`, icon: ndkVary.iconSvg("file-earmark-code"), disabled: isSQLite, action: function () {
                                    ndkGenerateSQL.buildNewTabSql(edata, 'Truncate');
                                }
                            },
                            {
                                name: `DROP`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                    ndkGenerateSQL.buildNewTabSql(edata, 'Drop');
                                }
                            },
                            'separator',
                            {
                                name: `DDL`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                    ndkGenerateSQL.buildNewTabSql(edata, 'DDL');
                                }
                            },
                            {
                                name: `结构和数据`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                }
                            },
                        ]
                    },
                    {
                        name: ndkI18n.lg.exportData, icon: ndkVary.iconGrid('save'), disabled: true,
                    },
                    {
                        name: ndkI18n.lg.importData, icon: ndkVary.iconGrid('save'), disabled: true,
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.refresh, icon: ndkVary.iconSvg("arrow-repeat"),
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            ndkRequest.reqTable(cp.cobj, cp.databaseName, true).then(tables => {
                                ndkView.viewTable(tables, cp.cobj)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Ctrl+Alt+M',
                        action: function () {
                            ndkVary.domGridTable.classList.toggle('nrc-fullscreen');
                            ndkAction.size();
                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.save, icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "JSON", icon: ndkVary.iconGrid('save'), action: function () {
                                    ndkVary.pasteContent = JSON.stringify(agg.getAllRows(ndkVary.gridOpsTable));
                                    ndkFunction.clipboard(ndkVary.pasteContent).then(() => {
                                        ndkFunction.output(ndkI18n.lg.copiedToClipboard);
                                    })
                                }
                            },
                            {
                                name: "Markdown", icon: ndkVary.iconGrid('save'), action: function () {
                                    var columns = ndkVary.gridOpsTable.columnApi.getAllGridColumns();
                                    var headers = [];
                                    for (let index = 1; index < columns.length; index++) {
                                        var column = columns[index];
                                        if (column.visible) {
                                            headers.push(column.colDef)
                                        }
                                    }

                                    ndkVary.pasteContent = ndkFunction.rowsToMarkdown(agg.getAllRows(ndkVary.gridOpsTable), headers, { htmlCull: true });
                                    ndkFunction.clipboard(ndkVary.pasteContent).then(() => {
                                        ndkFunction.output(ndkI18n.lg.copiedToClipboard);
                                    })
                                }
                            }
                        ]
                    },
                    'copy',
                    'copyWithHeaders',
                    'separator',
                    'chartRange'
                ];

                return result;
            }
        });

        ndkView.setDisplayItem(opsTable.columnDefs); // 设置显示列
        ndkView.createGrid("table", opsTable); //创建表格

        resolve();
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
            getRowId: event => `${event.data.SchemaName}.${event.data.TableName}.${event.data.ColumnName}`, //指定行标识列
            groupSelectsChildren: true, //选择子项
            groupDefaultExpanded: 2, //组展开
            //列
            columnDefs: [
                agg.numberCol({ hide: true }), //行号
                { field: 'SchemaName', headerName: ndkI18n.lg.schemaName, rowGroup: true, enableRowGroup: true, width: 200, hide: true },
                { field: 'TableName', headerName: ndkI18n.lg.tableName, rowGroup: true, enableRowGroup: true, width: 350, hide: true, },
                { field: 'TableComment', headerName: ndkI18n.lg.tableComment, width: 300, hide: true },
                { field: 'ColumnName', headerName: ndkI18n.lg.columnName, width: 200, hide: true, },
                { field: 'ColumnComment', tooltipField: "ColumnComment", headerName: ndkI18n.lg.columnComment, width: 330, hide: isSQLite, editable: !isSQLite, cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: 999 } },
                { field: 'ColumnType', headerName: ndkI18n.lg.columnTypeLength, width: 180, hide: true },
                { field: 'DataType', headerName: ndkI18n.lg.columnType, width: 130, hide: true },
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
                    cellRenderer: params => params.value > 0 ? ndkVary.emoji.key + params.value : ""
                },
                { field: 'AutoIncr', headerName: ndkI18n.lg.columnAutoIncrement, cellRenderer: params => params.value == 1 ? ndkVary.emoji.incr : "" },
                {
                    field: 'IsNullable', headerName: ndkI18n.lg.columnIsNullable, hide: true,
                    cellRenderer: params => params.value == 0 ? ndkVary.emoji.star : ""
                },
                { field: 'ColumnDefault', headerName: ndkI18n.lg.columnDefault, width: 120, },
            ],
            autoGroupColumnDef: agg.autoGroupColumnDef({
                field: "ColumnName", headerName: ndkI18n.lg.group, width: 560,
                headerCheckboxSelection: true, headerCheckboxSelectionFilteredOnly: true, //仅全选过滤的数据行
                filterValueGetter: params => {
                    if (params.data.SchemaName != null) {
                        return `${params.data.SchemaName}.${params.data.TableName}`
                    } else {
                        return params.data.TableName
                    }
                }, // 过滤值构建
                cellRendererParams: {
                    checkbox: true,
                    suppressCount: true,
                    suppressEnterExpand: true,
                    suppressDoubleClickExpand: true,
                    innerRenderer: function (item) {
                        if (item.node.group) {
                            switch (item.node.field) {
                                case "SchemaName":
                                    var grow = ndkFunction.groupBy(ndkVary.gridOpsColumn.rowData.filter(x => x[item.node.field] == item.value), x => x.TableName);
                                    return `<b>${item.value} (${grow.length})</b>`;
                                case "TableName":
                                    var grow = item.node.parent.field == "SchemaName" ?
                                        ndkVary.gridOpsColumn.rowData.filter(x => x[item.node.parent.field] == item.node.parent.key && x[item.node.field] == item.value) :
                                        ndkVary.gridOpsColumn.rowData.filter(x => x[item.node.field] == item.value);
                                    return `<b>${item.value} (${grow.length})</b> <span title="${grow[0].TableComment || ""}">${grow[0].TableComment || ""}</span>`;
                                default:
                                    return item.value;
                            }
                        } else {
                            var colattr = [];
                            if (item.data.PrimaryKey > 0) {
                                colattr.push(ndkVary.emoji.key + item.data.PrimaryKey);
                            }
                            colattr.push(item.data.ColumnType);
                            if (item.data.IsNullable == 1) {
                                colattr.push("null");
                            } else {
                                colattr.push("not null");
                            }
                            return ndkVary.iconSvg("bookshelf", `${item.value} <span class="opacity-75">( ${colattr.join(', ')} )</span>`);
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
                connStatusBarComponent: ndkView.gridComponents.connStatusBarComponent
            },
            onCellValueChanged: function (event) {
                //修改列注释
                if (event.column.colId == "ColumnComment") {
                    var cp = ndkStep.cpGet(1), edata = event.data;
                    ndkRequest.setColumnComment(cobj, edata.TableName, edata.ColumnName, event.value, edata.SchemaName, cp.databaseName).then(() => {
                        //更新本地缓存
                    }).catch(() => {
                        //更新失败，恢复原值
                        var edata = { ...event.data };
                        edata.columnComment = event.oldValue;
                        event.api.applyTransaction({
                            update: [edata]
                        });
                    })
                }
            },
            // 单元格按键
            onCellKeyDown: function (event) {
                //全屏切换
                if (event.event.ctrlKey && event.event.altKey && event.event.code == "KeyM") {
                    ndkVary.domGridColumn.classList.toggle('nrc-fullscreen');
                    ndkAction.size();
                }
            },
            //列右键菜单项
            getContextMenuItems: (params) => {
                var result = [
                    'expandAll',
                    'contractAll',
                    'separator',
                    {
                        name: ndkI18n.lg.refresh, icon: ndkVary.iconSvg("arrow-repeat"),
                        action: function () {
                            var cp = ndkStep.cpGet(1);
                            var filterSNTN = ndkFunction.groupBy(ndkVary.gridOpsColumn.rowData, x => x.SchemaName == null ? x.TableName : `${x.SchemaName}.${x.TableName}`).join(',');
                            ndkRequest.reqColumn(cp.cobj, cp.databaseName, filterSNTN, true).then(columns => {
                                ndkView.viewColumn(columns, cp.cobj)
                            })
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    {
                        name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Ctrl+Alt+M',
                        action: function () {
                            ndkVary.domGridColumn.classList.toggle('nrc-fullscreen');
                            ndkAction.size();
                        }
                    },
                    'separator',
                    {
                        name: ndkI18n.lg.save, icon: ndkVary.iconGrid('save'),
                        subMenu: [
                            'csvExport',
                            'excelExport',
                            'separator',
                            {
                                name: "JSON", icon: ndkVary.iconGrid('save'), action: function () {
                                    ndkVary.pasteContent = JSON.stringify(agg.getAllRows(ndkVary.gridOpsColumn));
                                    ndkFunction.clipboard(ndkVary.pasteContent).then(() => {
                                        ndkFunction.output(ndkI18n.lg.copiedToClipboard);
                                    })
                                }
                            },
                            {
                                name: "Markdown", icon: ndkVary.iconGrid('save'), action: function () {
                                    var columns = ndkVary.gridOpsColumn.columnApi.getAllGridColumns();
                                    var headers = [], tableNameGroupActive = false;
                                    for (let index = 1; index < columns.length; index++) {
                                        var column = columns[index];
                                        if (column.visible) {
                                            headers.push(column.colDef)
                                        }

                                        //是表名分组
                                        if (column.colId == "TableName" && column.rowGroupActive) {
                                            tableNameGroupActive = true;
                                        }
                                    }

                                    if (tableNameGroupActive) {
                                        var mds = [];
                                        var arows = agg.getAllRows(ndkVary.gridOpsColumn);
                                        ndkFunction.groupBy(arows, x => x.TableName).forEach(tableName => {
                                            var frows = arows.filter(x => x.TableName == tableName);
                                            var tableComment = ndkFunction.markdownEncode(frows[0].TableComment);

                                            var md = ndkFunction.rowsToMarkdown(frows, headers);
                                            mds.push(`### ${tableName}\n${tableComment}\n\n` + md);
                                        });
                                        ndkVary.pasteContent = mds.join('\n\n');
                                    } else {
                                        ndkVary.pasteContent = ndkFunction.rowsToMarkdown(agg.getAllRows(ndkVary.gridOpsColumn), headers);
                                    }
                                    ndkFunction.clipboard(ndkVary.pasteContent).then(() => {
                                        ndkFunction.output(ndkI18n.lg.copiedToClipboard);
                                    })
                                }
                            }
                        ]
                    },
                    'copy',
                    'copyWithHeaders'
                ];

                return result;
            },
        });

        ndkView.setDisplayItem(opsColumn.columnDefs); // 设置显示列
        ndkView.createGrid("column", opsColumn); // 创建表格

        resolve();
    }),

    /**
     * 设置显示项
     * @param {*} columnDefs 
     * @param {*} ctype 
     */
    setDisplayItem: (columnDefs, ctype) => {
        ctype = ctype || ndkStep.cpGet(1).cobj.type;
        var remoteItem = [];
        switch (ctype) {
            case "SQLite":
                remoteItem = [
                    'DatabaseOwner', 'DatabaseSpace', 'DatabaseCollation', 'DatabaseLogLength', 'DatabaseIndexLength', 'DatabaseLogPath',
                    'TableDataLength', 'TableIndexLength', 'TableCollation', 'TableCreateTime', 'TableModifyTime', 'TableEngine', 'SchemaName', 'TableOwner', 'TableSpace', 'TableType',
                    'TableComment', 'ColumnComment'
                ]
                break;
            case "MySQL":
            case "MariaDB":
                remoteItem = [
                    'DatabaseOwner', 'DatabaseSpace', 'DatabaseLogLength', 'DatabaseLogPath',
                    'TableModifyTime', 'TableOwner', 'TableSpace'
                ]
                break;
            case "Oracle":
                remoteItem = [
                    'DatabaseLogLength', 'DatabaseLogPath',
                    'TableCollation', 'TableEngine', 'SchemaName'
                ]
                break;
            case "SQLServer":
                remoteItem = [
                    'DatabaseSpace', 'DatabaseCharset', 'DatabaseIndexLength',
                    'TableCollation', 'TableEngine', 'TableSpace'
                ]
                break;
            case "PostgreSQL":
                remoteItem = [
                    'DatabaseLogLength', 'DatabaseIndexLength', 'DatabaseLogPath', 'DatabaseCreateTime',
                    'TableCollation', 'TableCreateTime', 'TableModifyTime', 'TableEngine', 'TableSpace'
                ]
                break;
        }
        //倒序循环删除
        for (var i = columnDefs.length - 1; i >= 0; i--) {
            var item = columnDefs[i];
            if (remoteItem.includes(item.field)) {
                columnDefs.splice(i, 1);
            }
        }
    },

    /**
     * 显示执行SQL
     * @param {any} esdata
     * @param {any} tpkey
     */
    viewExecuteSql: (esdata, tpkey) => new Promise((resolve) => {
        var tpobj = ndkTab.tabKeys[tpkey];
        //选项卡已关闭
        if (tpobj == null) {
            resolve();
        }

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

                //删除完时，隐藏执行结果
                if (tpobj.domTabGroup3.children.length == 0) {
                    ndkAction.setSpliterSize(tpobj.domTabPanel.children[0], 'no');
                }

                //阻止冒泡
                event.stopPropagation();
            });
            //选项卡3执行结果-显示
            tpobj.domTabGroup3.addEventListener('sl-tab-show', function (event) {
                ndkAction.size();
            }, false);
        }

        new Promise(resolve2 => {
            for (const itemn in esdata) {
                //跳过表结构
                if (itemn == "Item2") {
                    continue;
                }
                var ti = 0, dtSchema;
                for (const iname in esdata[itemn]) {
                    ti++;

                    if (esdata["Item2"] && iname in esdata["Item2"]) {
                        dtSchema = esdata["Item2"][iname];
                    }

                    var esrows = esdata[itemn][iname], columnDefs = [];
                    var tabname = "Result", tabpanel = `tp3-${itemn}-${ti}`;
                    if (itemn == "Item1") {
                        if (["sql-notes", "sql-historys"].includes(iname)) {
                            tabname = ndkFunction.hump(ndkI18n.lg[`storageKey_${iname.split("-").pop()}`]);
                        } else if (dtSchema) {
                            tabname = dtSchema[0].BaseTableName || tabname;
                        }

                        if (ti > 1) {
                            tabname += " - " + ti;
                        }
                        tabname = ndkVary.iconSvg("grid-3x3-gap-fill", tabname)
                    } else {
                        tabname = ndkVary.iconSvg("info-circle", "Info")
                    }

                    //结构
                    var tabbox = document.createElement("div");
                    tabbox.innerHTML = `
<sl-tab slot="nav" panel="${tabpanel}" closable>${tabname}</sl-tab>
<sl-tab-panel name="${tabpanel}">
    <sl-input class="nr-filter-execute-sql mb-2" placeholder="${ndkI18n.lg.search}" size="small">
        <sl-icon name="search" slot="prefix"></sl-icon>
    </sl-input>
    <div class="nr-grid-execute-sql" data-key="${tpkey}"></div>
</sl-tab-panel>
                `;
                    let domTab = tabbox.querySelector('sl-tab'),
                        domTabPanel = tabbox.querySelector('sl-tab-panel'),
                        domFilterExecuteSql = tabbox.querySelector('.nr-filter-execute-sql'),
                        domGridExecuteSql = tabbox.querySelector('.nr-grid-execute-sql');

                    //添加到选项卡
                    var slpanels = tpobj.domTabGroup3.querySelectorAll("sl-tab-panel");
                    tpobj.domTabGroup3.insertBefore(domTab, slpanels[0]);
                    tpobj.domTabGroup3.appendChild(domTabPanel);

                    tabbox.remove();

                    //完成事件
                    var gridReady = function (event) {
                        event.api.sizeColumnsToFit(); //列宽100%自适应
                        ndkAction.size();
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
                                cellRenderer: params => {
                                    if (params.value == null && !params.node.group) {
                                        return ndkFunction.formatNull();
                                    } else {
                                        return params.value;
                                    }
                                }, headerComponentParams: { template: null },
                            }
                            if (dtSchema) {
                                var colSchema = dtSchema.find(x => x.ColumnName == field);
                                if (colSchema) {
                                    //没有类型列则取C#类型
                                    var colType = colSchema.DataTypeName;
                                    if (colType == null) {
                                        colType = colSchema.DataType.split(',')[0].split('.')[1].toLowerCase();
                                    }

                                    if (colSchema.IsKey) {
                                        htip.push(ndkVary.emoji.key);
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
                                    var sdt = ndkGenerateSQL.sqlDataType(tdb, column);
                                    switch (sdt.category) {
                                        case "string":
                                            colDef.width = Math.min(colSchema.ColumnSize * 10, 500);

                                            if (colSchema.DataType.startsWith("System.Byte")) {
                                                switch (colSchema.DataTypeName) {
                                                    //图片
                                                    case "image":
                                                        colDef.cellRenderer = params => {
                                                            if (params.value == null) {
                                                                return ndkFunction.formatNull();
                                                            } else {
                                                                return `<span role="button" data-cmd="view-cell-image" data-val="${params.value}">${ndkVary.emoji.see}image</span>`;
                                                            }
                                                        };
                                                        break;
                                                    default:
                                                        colDef.cellRenderer = params => {
                                                            if (params.value == null) {
                                                                return ndkFunction.formatNull();
                                                            } else {
                                                                return `<span role="button" data-cmd="view-cell-text" data-val="${params.value}">${ndkVary.emoji.see}text</span>`;
                                                            }
                                                        };
                                                        break;
                                                }
                                            } else {
                                                colDef.cellRenderer = params => {
                                                    var slen = ndkVary.parameterConfig.gridDataShowLength.value;
                                                    if (params.value == null) {
                                                        return ndkFunction.formatNull();
                                                    } else if (params.value.length > slen) {
                                                        return `${ndkVary.emoji.cut} ${ndkFunction.htmlEncode(params.value.substring(0, slen))} ...`;
                                                    } else {
                                                        return ndkFunction.htmlEncode(params.value);
                                                    }
                                                };
                                            }
                                            htag.push(ndkVary.iconSvg("type"))
                                            break;
                                        case "number":
                                            htag.push(ndkVary.iconSvg("123"))
                                            colDef.filterParams = agg.filterParamsDef("Number");
                                            break;
                                        case "date":
                                            htag.push(ndkVary.iconSvg("clock"))
                                            colDef.width = 280;
                                            colDef.filterParams = agg.filterParamsDef("Date");
                                            break;
                                    }
                                    if (colSchema.IsKey) {
                                        htag.push(ndkVary.iconSvg("key", "", { className: "nrc-text-orange" }))
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
                        if (columnDefs.length > 3) {
                            gridReady = function () {
                                ndkAction.size();
                            }
                        }
                    } else if (iname in (esdata.Item2 || {})) {
                        //填充列头（空表）
                        for (const coln in esdata.Item2[iname]) {
                            var col = esdata.Item2[iname][coln];
                            columnDefs.push({ headerName: col.ColumnName, field: col.ColumnName, headerTooltip: field });
                        }

                        gridReady = function (event) {
                            event.columnApi.autoSizeAllColumns();
                            ndkAction.size();
                        }
                    }

                    let opsExecuteSql = agg.optionDef({
                        rowData: esrows,
                        columnDefs: columnDefs,
                        statusBar: {
                            statusPanels: [
                                { statusPanel: 'agSelectedRowCountComponent' },
                                { statusPanel: 'agTotalAndFilteredRowCountComponent' },
                            ],
                        },
                        // 单元格按键
                        onCellKeyDown: function (event) {
                            //全屏切换
                            if (event.event.ctrlKey && event.event.altKey && event.event.code == "KeyM") {
                                domGridExecuteSql.classList.toggle('nrc-fullscreen');
                                ndkAction.size();
                            }
                        },
                        //表菜单项
                        getContextMenuItems: (event) => {
                            let result = [
                                {
                                    name: "NULL", disabled: true,
                                },
                                {
                                    name: ndkI18n.lg.delete, icon: ndkVary.iconGrid("cross"), disabled: true,
                                },
                                'separator',
                                {
                                    name: ndkI18n.lg.save, icon: ndkVary.iconGrid('save'),
                                    subMenu: [
                                        'csvExport',
                                        'excelExport',
                                        'separator',
                                        {
                                            name: "JSON", icon: ndkVary.iconGrid('save'), action: function () {
                                                ndkVary.pasteContent = JSON.stringify(agg.getAllRows(opsExecuteSql));
                                                ndkFunction.clipboard(ndkVary.pasteContent).then(() => {
                                                    ndkFunction.output(ndkI18n.lg.copiedToClipboard);
                                                })
                                            }
                                        },
                                        {
                                            name: "Markdown", icon: ndkVary.iconGrid('save'),
                                        }
                                    ]
                                },
                                'copy',
                                'copyWithHeaders',
                                {
                                    name: ndkI18n.lg.dataGenerateSQL, icon: ndkVary.iconSvg("file-earmark-code"),
                                    subMenu: [
                                        {
                                            name: `INSERT`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                                ndkGenerateSQL.dataNewSql(event, "Insert");
                                            }
                                        },
                                        {
                                            name: `UPDATE`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                                ndkGenerateSQL.dataNewSql(event, "Update");
                                            }
                                        },
                                        {
                                            name: `DELETE`, icon: ndkVary.iconSvg("file-earmark-code"), action: function () {
                                                ndkGenerateSQL.dataNewSql(event, "Delete");
                                            }
                                        }
                                    ]
                                },
                                'separator',
                                'autoSizeAll',
                                'resetColumns',
                                {
                                    name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Ctrl+Alt+M',
                                    action: function () {
                                        domGridExecuteSql.classList.toggle('nrc-fullscreen');
                                        ndkAction.size();
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
                                name: ndkI18n.lg.fullScreenSwitch, icon: ndkVary.iconGrid("maximize"), shortcut: 'Ctrl+Alt+M',
                                action: function () {
                                    domGridExecuteSql.classList.toggle('nrc-fullscreen');
                                    ndkAction.size();
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

            //显示执行结果（自动）
            ndkAction.setSpliterSize(tpobj.domTabPanel.children[0], 'auto');

            ndkAction.themeGrid(ndkVary.theme);
            //切换面板时再呈现表
            ndkAction.size();

            resolve(tpobj);
        })
    })

}

export { ndkView }