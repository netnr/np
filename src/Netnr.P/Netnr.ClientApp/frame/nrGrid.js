import { nrcBase } from "./nrcBase";
import { nrcRely } from "./nrcRely";

// ag-grid
let nrGrid = {
    flagRowHeight: 42, //表格行高
    flagPageSize: 30, //表格分页大小

    tsLoaded: null,
    /**
     * 资源依赖，默认远程，可重写为本地
     */
    rely: async () => {
        await nrcRely.remote('agGrid');
        nrGrid.err();
    },

    err: () => {
        let okey = "__consoleError";
        if (!window[okey]) {
            window[okey] = console.error;

            console.error = function () {
                let arg0 = arguments[0];
                if (!(arguments.length == 1 && typeof arg0 == "string" && arg0.startsWith('*'))) {
                    window[okey].apply(this, arguments);
                }
            }
        }
    },

    /**
     * 构建 grid dom
     * @param {any} dom
     */
    buildDom: (dom) => {
        dom = dom || document.createElement("div");
        dom.classList.add('nrg-grid');

        let themeClass = "ag-theme-alpine";
        if (nrcBase.isDark()) {
            themeClass += '-dark';
        }
        dom.classList.add(themeClass);

        return dom;
    },

    /**
     * 创建显示 grid
     * @param {any} domGrid 
     * @param {any} gridOptions 
     */
    createGrid: async (domGrid, gridOptions) => {
        await nrGrid.rely();

        domGrid.innerHTML = "";
        let gridApi = agGrid.createGrid(domGrid, gridOptions);
        domGrid["gridApi"] = gridApi;
        return gridApi;
    },

    /**
     * 字典列
     * @param {any} column 
     * @param {any} valueText 
     * @returns 
     */
    newColumnSet: (column, valueText) => Object.assign({
        cellRenderer: params => {
            if (params.data) {
                let item = valueText.find(x => x.value == params.value);
                if (item == null) {
                    item = valueText.find(x => x.default == true);
                }
                if (item) {
                    return item.html != null ? item.html : nrcBase.xssOf(item.text);
                }
                return params.value;
            }
        },
        filter: 'agSetColumnFilter', filterParams: {
            buttons: ['apply', 'reset'], values: valueText.map(x => x.value),
            cellRenderer: (params) => {
                let item = valueText.find(x => x.value == params.value);
                if (item == null) {
                    item = valueText.find(x => x.default == true);
                }
                if (item) {
                    return item.html != null ? item.html : nrcBase.xssOf(`${item.text} (${item.value})`);
                }
                return params.value;
            }
        }, cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
            values: valueText.map(x => x.value),
            cellHeight: nrGrid.flagRowHeight * 0.9,
            valueListMaxHeight: nrGrid.flagRowHeight * 6,
            formatValue: value => {
                let item = valueText.find(x => x.value == value);
                if (item == null) {
                    item = valueText.find(x => x.default == true);
                }
                if (item) {
                    return item.html != null ? item.html : item.text;
                }
            },
        }
    }, column),

    /**
     * 日期列
     * @param {any} column
     */
    newColumnDate: (column) => Object.assign({
        filter: 'agDateColumnFilter',
        filterParams: { buttons: ['apply', 'reset'], comparator: nrGrid.agDateColumnFilterComparator },
        width: 200,
        valueGetter: params => {
            if (params.data) {
                let val = params.data[column.field];
                if (nrcBase.type(val) == "Date") {
                    return nrcBase.formatDateTime("datetime", val);
                } else if (!nrcBase.isNullOrWhiteSpace(val)) {
                    return val.split('.')[0];
                }
            }
        },
        valueFormatter: nrGrid.formatterDateTime
    }, column),

    /**
     * 日期列
     * @param {any} column
     */
    newColumnDateOnly: (column) => Object.assign({
        filter: 'agDateColumnFilter',
        filterParams: { buttons: ['apply', 'reset'], comparator: nrGrid.agDateColumnFilterComparator },
        width: 150,
        valueGetter: params => {
            if (params.data && params.data[column.field]) {
                return params.data[column.field].split(' ')[0];
            }
        },
        valueFormatter: nrGrid.formatterDate
    }, column),

    /**
     * 仅比较日期
     * @param {*} filterDate 
     * @param {*} cellValue 
     * @returns 
     */
    agDateColumnFilterComparator: function (filterDate, cellValue) {
        if (cellValue == null || cellValue == "") return -1;

        //仅比较日期
        let cellDate = new Date(cellValue);
        cellDate = new Date(Number(cellDate.getFullYear()), Number(cellDate.getMonth()) - 1, Number(cellDate.getDate()));
        filterDate = new Date(Number(filterDate.getFullYear()), Number(filterDate.getMonth()) - 1, Number(filterDate.getDate()));

        if (filterDate.getTime() == cellDate.getTime()) {
            return 0;
        }
        if (cellDate < filterDate) {
            return -1;
        }
        if (cellDate > filterDate) {
            return 1;
        }
    },

    /**
     * 文本域列
     * @param {any} column
     * @param {any} maxLength
     */
    newColumnTextarea: (column, maxLength) => Object.assign({
        cellEditor: 'agLargeTextCellEditor', cellEditorParams: { maxLength: maxLength || 99999999 },
        cellEditorPopup: true,
        filterParams: { buttons: ['apply', 'reset'] }
    }, column),

    /**
     * 数字列
     * @param {any} column
     */
    newColumnNumber: (column) => Object.assign({
        filter: 'agNumberColumnFilter', filterParams: { buttons: ['apply', 'reset'] }
    }, column),

    /**
     * 行号
     * @param {any} ops
     * @returns
     */
    newColumnLineNumber: ops => {
        let newCol = Object.assign({
            field: "#line_number", headerName: "🆔", valueGetter: "node.rowIndex + 1", width: 100, maxWidth: 180,
            checkboxSelection: true, headerCheckboxSelection: true,
            sortable: false, filter: false, menuTabs: []
        }, ops);

        //仅全选过滤的数据行
        if (newCol.checkboxSelection && newCol.headerCheckboxSelection && newCol.headerCheckboxSelectionFilteredOnly == null) {
            newCol.headerCheckboxSelectionFilteredOnly = true;
        }

        return newCol;
    },

    /**
     * 默认列属性
     * @param {any} colDef
     */
    defaultColDef: colDef => Object.assign({
        //默认属性
        width: 180, minWidth: 100, maxWidth: 4000, sortable: true, resizable: true, enableCellChangeFlash: true,
        //默认文本过滤
        filter: 'agTextColumnFilter', filterParams: { buttons: ['apply', 'reset'] },
        //默认菜单项
        menuTabs: ['filterMenuTab', 'generalMenuTab', 'columnsMenuTab']
    }, colDef),

    /**
     * 默认分组属性~
     * @param {any} ops
     */
    autoGroupColumnDef: ops => Object.assign({ width: 300, maxWidth: 4000 }, ops),

    /**
     * 获取 icon
     * @param {any} icon
     */
    iconGrid: icon => `<span class="ag-icon ag-icon-${icon}"></span>`,

    /**
     * 公共请求，可重写
     * @param {*} url 
     */
    fetch: async (url) => await nrcBase.fetch(url),

    /**
     * grid 配置
     */
    gridOptions: (options) => Object.assign({
        localeText: nrGrid.localeText, //语言
        defaultColDef: nrGrid.defaultColDef(), //默认列属性
        autoGroupColumnDef: nrGrid.autoGroupColumnDef(), //默认分组
        suppressMoveWhenRowDragging: true, //抑制拖拽实时移动
        suppressRowClickSelection: false, //抑制点击行选中
        suppressFieldDotNotation: true, //抑制字段 key 点 . 解析
        suppressGroupRowsSticky: true, //抑制分组黏在顶部
        // suppressClipboardPaste: true, //抑制 Ctrl+V 粘贴更新
        enableBrowserTooltips: true, //使用浏览器提示
        enableRangeSelection: true, //范围选择
        // enableCellChangeFlash: true, //单元格更新闪烁，新版在 defaultColDef 配置
        rowGroupPanelShow: 'always', //启用列拖拽分组 'never', 'always', 'onlyWhenGrouping'
        rowSelection: 'multiple', //多选 multiple、单选 single
        //getRowId: event => event.data.data_id, //主键列
        columnDefs: [], //列配置
        autoSizePadding: 40, //自动调整列宽追加值（标题动态图标、排序标记等）
        headerHeight: nrGrid.flagRowHeight, //表头高度
        rowHeight: nrGrid.flagRowHeight, //行高度
        pagination: true, //分页
        paginationAutoPageSize: true, //自动分页
        paginationPageSize: nrGrid.flagPageSize, //单页数量
        popupParent: document.body, //右键菜单容器
        animateRows: true, //动画
        isRowSelectable: rowNode => rowNode.group !== true, //非分组显示复选框
        onRowDataUpdated: event => event.api.refreshCells(), //数据更新后刷新（更新行号）
        onSortChanged: event => event.api.refreshCells(), //排序后刷新（更新行号）        
        onFilterChanged: event => event.api.refreshCells(), //过滤后刷新（更新行号）
        onRowGroupOpened: event => event.api.refreshCells(), //组展开后刷新（更新行号）
        //右键菜单
        getContextMenuItems: nrGrid.getContextMenuItems,
    }, options),

    /**
     * grid 无限滚动模式（多用）
     * @param {any} options
     * @param {any} buildUrl 构建请求 URL，接收 params 参数
     * @param {any} loadedCall 加载完成渲染前(result, params)
     * @returns 
     */
    gridOptionsInfinite: async (options, buildUrl, loadedCall) => {
        let gridOps = nrGrid.gridOptions({
            cacheBlockSize: nrGrid.flagPageSize, //加载数量
            rowGroupPanelShow: "never",
            rowModelType: 'infinite', //模式
            //数据源
            datasource: {
                getRows: async params => {
                    let url = buildUrl.call(params, params);
                    let result = await nrGrid.fetch(url);

                    if (loadedCall) {
                        await loadedCall(result, params);
                    }

                    if (result.code == 200) {
                        params.successCallback(result.data.RowsThisBlock, result.data.LastRow)
                    } else {
                        params.failCallback();
                    }
                }
            },
        });

        Object.assign(gridOps, options);
        return gridOps;
    },

    /**
     * grid 服务端模式（少用）
     * @param {any} options
     * @param {any} buildUrl 构建请求 URL，接收 params 参数
     * @param {any} loadedCall 加载完成渲染前(result, params)
     * @returns 
     */
    gridOptionsServer: async (options, buildUrl, loadedCall) => {
        let gridOps = nrGrid.gridOptions({
            cacheBlockSize: nrGrid.flagPageSize, //加载数量
            rowGroupPanelShow: "never",
            rowModelType: 'serverSide', //模式
            //数据源
            serverSideDatasource: {
                getRows: async params => {
                    let url = buildUrl.call(params, params);
                    let result = await nrGrid.fetch(url);

                    if (loadedCall) {
                        await loadedCall(result, params);
                    }

                    if (result.code == 200) {
                        params.success({ rowData: result.data.RowsThisBlock, rowCount: result.data.LastRow });
                    } else {
                        params.fail();
                    }
                }
            },
        });

        Object.assign(gridOps, options);
        return gridOps;
    },

    /**
     * grid 客户端模式配置
     * @param {any} options
     */
    gridOptionsClient: (options) => {
        let gridOps = Object.assign(nrGrid.gridOptions(), {
            pagination: false
        });
        gridOps = Object.assign(gridOps, options);
        return gridOps;
    },

    /**
     * 获取参数 JSON
     * @param {*} gridApi 
     * @returns 
     */
    getParamsJson: (gridApi) => {
        let paramsJson = {
            startRow: 0,
            endRow: gridApi.paginationGetRowCount() || 9999,
            filterModel: gridApi.getFilterModel(),
            sortModel: []
        };

        gridApi.getColumns().filter(x => x.sort != null).forEach(col => {
            paramsJson.sortModel[col.sortIndex || 0] = { sort: col.sort, colId: col.colId };
        });

        return paramsJson;
    },

    /**
     * 右键菜单
     * @param {*} params 
     * @param {*} type 默认通用，可选 import excel
     */
    getContextMenuItems: (params, type) => {
        let agRootWrapper = params.api.gos.eGridDiv.querySelector('.ag-root-wrapper');
        let isFullscreen = agRootWrapper.classList.contains(`nrg-fullscreen`);
        let itemCopyText = {
            name: "复制文本", icon: nrGrid.iconGrid('copy'), action: async function () {
                let cranges = params.api.getCellRanges()[0];
                let rows = [];
                let rowNodes = [];
                for (let rowIndex = cranges.startRow.rowIndex; rowIndex <= cranges.endRow.rowIndex; rowIndex++) {
                    let rowNode = params.api.getDisplayedRowAtIndex(rowIndex);
                    let cols = [];
                    cranges.columns.forEach(column => {
                        let content = rowNode.data[column.colId];
                        Object.assign(params, { column, data: rowNode.data, value: content });
                        if (typeof column.colDef.valueFormatter == "function") {
                            content = column.colDef.valueFormatter(params)
                        } else if (typeof column.colDef.cellRenderer == "function") {
                            content = column.colDef.cellRenderer(params)
                        }
                        cols.push(content);
                    });
                    rowNodes.push(rowNode);
                    rows.push(cols.join('\t'));
                }
                await nrcBase.clipboard(rows.join('\r\n')); //复制
                params.api.flashCells({ rowNodes, columns: cranges.columns.map(x => x.colId) }); //闪烁

            }
        }
        let itemFullScreen = {
            name: isFullscreen ? "取消全屏" : "全屏显示", icon: nrGrid.iconGrid(isFullscreen ? 'minimize' : 'maximize'), action: function () {
                agRootWrapper.classList.toggle(`nrg-fullscreen`);
            }
        }
        let itemAdd = {
            name: "新增一行", action: async function () {
                params.api.applyTransaction({ add: [{}] });
            }
        }
        let itemDelete = {
            name: "删除选中的行", icon: nrGrid.iconGrid('cross'), action: async function () {
                let srows = params.api.getSelectedRows();
                if (srows.length) {
                    params.api.applyTransaction({ remove: srows });
                }
            }
        }
        let itemExport = {
            name: nrGrid.localeText.export, icon: nrGrid.iconGrid("down"),
            subMenu: [,
                {
                    name: `${nrGrid.localeText.excelExport} (格式化)`, icon: nrGrid.iconGrid('excel'), action: function () {
                        nrGrid.exportDataAsExcel(params.api);
                    }
                },
                'excelExport',
                'csvExport',
            ]
        };

        let result = [];

        switch (type) {
            case "excel":
                result = [
                    'copy',
                    'copyWithHeaders',
                    'separator',
                    itemFullScreen,
                    'autoSizeAll',
                    'resetColumns',
                    itemExport,
                    'separator',
                    'chartRange'
                ];
                break;
            case "import":
                result = [
                    'copy',
                    'copyWithHeaders',
                    'separator',
                    itemAdd,
                    itemDelete,
                    'separator',
                    itemFullScreen,
                    'autoSizeAll',
                    'resetColumns',
                    itemExport,
                ];
                break;
            default:
                result = [
                    'copy',
                    'copyWithHeaders',
                    itemCopyText,
                    'separator',
                    itemFullScreen,
                    'autoSizeAll',
                    'resetColumns',
                    itemExport,
                ];
                break;
        }

        return result;
    },

    /**
     * 获取所有行
     * @param {any} gridApi 
     * @param {any} isLeaf 
     * @returns 
     */
    getAllRows: function (gridApi, isLeaf = true) {
        let rowData = [];
        if (gridApi) {
            if (isLeaf) {
                gridApi.forEachLeafNode(node => rowData.push(node.data));
            } else {
                gridApi.forEachNode(node => rowData.push(node.data));
            }
        }
        return rowData;
    },

    /**
     * 获取选中或范围的行
     * @param {any} gridApi 
     * @returns 
     */
    getSelectedOrRangeRow: function (gridApi) {
        let srows = gridApi.getSelectedRows(), crows = gridApi.getCellRanges();
        if (srows.length > 0) {
            return srows[0]
        } else if (crows.length > 0) {
            return gridApi.getDisplayedRowAtIndex(crows[0].startRow.rowIndex).data
        }
    },

    /**
     * 获取容器
     * @param {any} event 
     * @returns 
     */
    getContainer: event => event.api.gos.eGridDiv,

    /**
     * 设置加载状态
     * @param {any} gridApi 
     * @param {any} isHide 
     */
    setGridLoading: function (gridApi, isHide) {
        if (gridApi) {
            switch (isHide) {
                case 1:
                    gridApi.hideOverlay();
                    break;
                case 2:
                    gridApi.hideOverlay();
                    gridApi.showNoRowsOverlay();
                    break;
                default:
                    gridApi.showLoadingOverlay();
            }
        }
    },

    /**
     * 格式化时间
     * @param {any} params 
     * @returns 
     */
    formatterDateTime: (params) => {
        if (params.value != null && params.value != "") {
            return nrcBase.formatDateTime('datetime', params.value);
        }
    },

    /**
     * 格式化日期
     * @param {any} params 
     * @returns 
     */
    formatterDate: (params) => {
        if (params.value != null && params.value != "") {
            return nrcBase.formatDateTime('date', params.value);
        }
    },

    /**
     * 格式化触发
     * @param {*} params 
     * @returns 
     */
    formatterTrigger: (params) => {
        let colDef = params.column.getColDef();
        let fn;
        if (typeof colDef.valueFormatter == "function") {
            fn = colDef.valueFormatter
        } else if (colDef.cellRenderer) {
            if (typeof colDef.cellRenderer == "function") {
                fn = colDef.cellRenderer;
            } else if (colDef.cellRendererParams && typeof colDef.cellRendererParams.innerRenderer == "function") {
                fn = colDef.cellRendererParams.innerRenderer;
            }
        }

        if (typeof fn == "function") {
            let newParams = {
                ...params,
                data: params.node.data,
                node: params.node,
                colDef: params.column.getColDef()
            };
            return fn(newParams);
        }

        return params.value;
    },

    /**
     * 表格数据转换 [{Entity:{},Entity2:{}}] => [{}]
     * @param {*} result 
     */
    fromResultToRows: (result) => {
        if (result.code == 200) {
            let rows = [];
            result.data.RowsThisBlock.forEach(item => {
                let row = {};
                Object.keys(item).forEach(key => {
                    Object.assign(row, item[key]);
                })
                rows.push(row);
            })
            result.data.RowsThisBlock = rows;
        }
    },

    /**
     * 数组转为 AgGrid treeData 模式 getDataPath 方法所需的路径数组
     * @param {*} rows 
     * @param {*} pidField 
     * @param {*} idField 
     * @param {*} startPid 
     * @param {*} dataField 
     * @returns 
     */
    fromRowsToTreeData: (rows, pidField, idField, startPid = '0', dataField = "$dataPath") => {
        let rowsMap = {}; // 用于存储每个节点的上级路径

        // 构建节点映射表
        rows.forEach(row => rowsMap[row[idField]] = row);

        // 遍历数组，为每个节点新增 rowPath 字段
        rows.forEach(row => {
            let rowPath = []; // 用于存储当前节点的路径

            // 逐级向上查找上级节点，直到根节点
            let currentRow = row;
            while (currentRow && currentRow[pidField] != startPid) {
                const parentRow = rowsMap[currentRow[pidField]];
                if (parentRow) {
                    rowPath.unshift(parentRow[idField]);
                    currentRow = parentRow;
                } else {
                    break;
                }
            }

            rowPath.push(row[idField]); // 添加当前节点
            row[dataField] = rowPath;
        });

        return rows;
    },

    /**
     * 导出 Excel 格式化
     * @param {*} gridApi 
     */
    exportDataAsExcel: (gridApi) => {
        gridApi.exportDataAsExcel({
            processCellCallback: (params) => nrGrid.formatterTrigger(params)
        });
    },

    /**
     * 语言包
     */
    localeText: {
        // Set Filter
        selectAll: '（全部）',
        selectAllSearchResults: '（全部搜索结果）',
        searchOoo: '搜索...',
        blanks: '（空）',
        noMatches: '未找到',

        // Number Filter & Text Filter
        filterOoo: '搜索...',
        equals: '等于',
        notEqual: '不等于',
        blank: '空',
        notBlank: '非空',
        empty: '选择一项',

        // Number Filter
        lessThan: '小于',
        greaterThan: '大于',
        lessThanOrEqual: '小于等于',
        greaterThanOrEqual: '大于等于',
        inRange: '范围',
        inRangeStart: '开始值',
        inRangeEnd: '结束值',

        // Text Filter
        contains: '包含',
        notContains: '不包含',
        startsWith: '开始包含',
        endsWith: '结尾包含',

        // Date Filter
        dateFormatOoo: 'yyyy-mm-dd',

        // Filter Conditions
        andCondition: '和',
        orCondition: '或',

        // Filter Buttons
        applyFilter: '确定',
        resetFilter: '重置',
        clearFilter: '清除',
        cancelFilter: '取消',

        // Filter Titles
        textFilter: '文本搜索',
        numberFilter: '数字搜索',
        dateFilter: '日期搜索',
        setFilter: '项搜索',

        // Side Bar
        columns: '列',
        filters: '搜索',

        // columns tool panel
        pivotMode: '枢轴模式',
        groups: '行组',
        rowGroupColumnsEmptyMessage: '拖拽列到此处进行分组',
        values: '值',
        valueColumnsEmptyMessage: '拖拽到此处合计',
        pivots: '列标签',
        pivotColumnsEmptyMessage: '拖拽到此处设置列标签',

        // Header of the Default Group Column
        group: '分组',
        // Row Drag
        rowDragRows: '行',
        rowDragRows: '多行',

        // Other
        loadingOoo: '加载中...',
        loadingError: '错误',
        noRowsToShow: '（空）',
        enabled: '启用',

        // Menu
        pinColumn: '固定列',
        pinLeft: '左固定',
        pinRight: '右固定',
        noPin: '取消固定',
        valueAggregation: '合计',
        noAggregation: '无',
        autosizeThiscolumn: '当前列大小自适应',
        autosizeAllColumns: '所有列大小自适应',
        groupBy: '分组',
        ungroupBy: '不分组',
        ungroupAll: '取消所有分组',
        addToValues: '添加值 ${variable}',
        removeFromValues: '移除值 ${variable}',
        addToLabels: '添加到标签 ${variable}',
        removeFromLabels: '移除标签 ${variable}',
        resetColumns: '重置列',
        expandAll: '展开全部',
        collapseAll: '折叠全部',
        copy: '复制',
        ctrlC: 'Ctrl+C',
        ctrlX: 'Ctrl+X',
        copyWithHeaders: '复制（带标题）',
        copyWithGroupHeaders: '复制（带分组）',
        cut: '剪切',
        paste: '粘贴',
        ctrlV: 'Ctrl+V',
        export: '导出',
        csvExport: '导出 CSV',
        excelExport: '导出 Excel',

        // Enterprise Menu Aggregation and Status Bar
        sum: '求和',
        first: '首',
        last: '尾',
        min: '最小',
        max: '最大',
        none: '无',
        count: '总数',
        avg: '平均',
        filteredRows: '过滤行',
        selectedRows: '选中',
        totalRows: '总行',
        totalAndFilteredRows: '搜索',
        more: '更多',
        to: '-',
        of: '，共',
        page: '当前',
        pageLastRowUnknown: '?',
        nextPage: '下一页',
        lastPage: '尾页',
        firstPage: '首页',
        previousPage: '上一页',
        // Pivoting
        pivotColumnGroupTotals: '总',

        // Enterprise Menu (Charts)
        pivotChartAndPivotMode: '图表枢轴 & 枢轴模式',
        pivotChart: '图表枢轴',
        chartRange: '范围图表',

        columnChart: '柱状图',
        groupedColumn: '分组',
        stackedColumn: '堆叠柱形图',
        normalizedColumn: '100% 堆叠柱形图',

        barChart: '条形图',
        groupedBar: '分组',
        stackedBar: '堆叠柱形图',
        normalizedBar: '100% 堆叠柱形图',

        pieChart: '饼形图',
        pie: '饼图',
        doughnut: '环形图',

        line: '线图',

        xyChart: '散点图及气泡图',
        scatter: '散点图',
        bubble: '气泡图',

        areaChart: '面积图',
        area: '面积',
        stackedArea: '叠堆',
        normalizedArea: '100% 叠堆',

        histogramChart: '直方图',
        histogramFrequency: "Frequency",

        combinationChart: 'Combination',
        columnLineCombo: 'Column & Line',
        AreaColumnCombo: 'Area & Column',

        // Charts
        pivotChartTitle: '图表枢轴',
        rangeChartTitle: '范围图表',
        settings: '设置',
        data: '数据',
        format: '格式',
        categories: '类别',
        defaultCategory: '(无)',
        series: '系数',
        xyValues: 'X Y 值',
        paired: '配对模式',
        axis: '轴',
        navigator: '导航',
        color: '颜色',
        thickness: '坐标宽度',
        xType: 'X Type',
        automatic: 'Automatic',
        category: '类别',
        number: '数字',
        time: '时间',
        autoRotate: '自动旋转',
        xRotation: 'X 旋转',
        yRotation: 'Y 旋转',
        ticks: 'Ticks',
        width: '宽',
        height: '高',
        length: '长',
        padding: '填充',
        spacing: '间距',
        chart: '图表',
        title: '标题',
        titlePlaceholder: '图表标题 - 双击编辑',
        background: '背景',
        font: '字体',
        top: '上',
        right: '右',
        bottom: '下',
        left: '左',
        labels: '标签',
        size: '大小',
        minSize: '最小',
        maxSize: '最大',
        legend: '指标项',
        position: '位置',
        markerSize: 'Marker Size',
        markerStroke: 'Marker Stroke',
        markerPadding: 'Marker Padding',
        itemSpacing: 'Item Spacing',
        itemPaddingX: 'Item Padding X',
        itemPaddingY: 'Item Padding Y',
        layoutHorizontalSpacing: 'Horizontal Spacing',
        layoutVerticalSpacing: 'Vertical Spacing',
        strokeWidth: '线条宽度',
        lineDash: '划线',
        offset: 'Offset',
        offsets: 'Offsets',
        tooltips: '显示提示',
        callout: 'Callout',
        markers: '标点',
        shadow: '阴影',
        blur: '发散',
        xOffset: 'X 偏移',
        yOffset: 'Y 偏移',
        lineWidth: '线条粗细',
        normal: '正常',
        bold: '加粗',
        italic: '斜体',
        boldItalic: '加粗斜体',
        predefined: 'Predefined',
        fillOpacity: '填充透明度',
        strokeOpacity: '线条透明度',
        histogramBinCount: 'Bin count',
        columnGroup: '柱状',
        barGroup: '条形',
        pieGroup: '饼状',
        lineGroup: '线',
        scatterGroup: '散点及气泡',
        areaGroup: '面积',
        histogramGroup: '直方',
        combinationGroup: 'Combination',
        groupedColumnTooltip: 'Grouped',
        stackedColumnTooltip: 'Stacked',
        normalizedColumnTooltip: '100% Stacked',
        groupedBarTooltip: 'Grouped',
        stackedBarTooltip: 'Stacked',
        normalizedBarTooltip: '100% Stacked',
        pieTooltip: 'Pie',
        doughnutTooltip: 'Doughnut',
        lineTooltip: 'Line',
        groupedAreaTooltip: 'Area',
        stackedAreaTooltip: 'Stacked',
        normalizedAreaTooltip: '100% Stacked',
        scatterTooltip: 'Scatter',
        bubbleTooltip: 'Bubble',
        histogramTooltip: 'Histogram',
        columnLineComboTooltip: 'Column & Line',
        areaColumnComboTooltip: 'Area & Column',
        customComboTooltip: 'Custom Combination',
        noDataToChart: 'No data available to be charted.',
        pivotChartRequiresPivotMode: 'Pivot Chart requires Pivot Mode enabled.',
        chartSettingsToolbarTooltip: 'Menu',
        chartLinkToolbarTooltip: 'Linked to Grid',
        chartUnlinkToolbarTooltip: 'Unlinked from Grid',
        chartDownloadToolbarTooltip: 'Download Chart',
        seriesChartType: 'Series Chart Type',
        seriesType: 'Series Type',
        secondaryAxis: 'Secondary Axis',

        // Data types
        true: 'True',
        false: 'False',
        invalidDate: '无效日期',
        invalidNumber: '无效数字',
        january: '1月',
        february: '2月',
        march: '3月',
        april: '4月',
        may: '5月',
        june: '6月',
        july: '7月',
        august: '8月',
        september: '9月',
        october: '10月',
        november: '11月',
        december: '12月',
    }
}

Object.assign(window, { nrGrid });
export { nrGrid }