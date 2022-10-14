// ag-grid
var ag = {
    lk: () => agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { },

    /**
     * 字典列
     * @param {*} column 
     * @param {*} obj 
     * @returns 
     */
    agSetColumn: (column, obj) => Object.assign(column, {
        cellRenderer: params => {
            if (params.value in obj) {
                return obj[params.value];
            }
            return params.value;
        },
        filter: 'agSetColumnFilter', filterParams: {
            buttons: ['apply', 'reset'], values: Object.keys(obj),
            cellRenderer: (params) => params.value in obj ? `${obj[params.value]} (${params.value})` : params.value
        }
    }),

    /**
     * 日期列
     * @param {any} column
     * @param {any} obj
     */
    agDateColumn: (column) => Object.assign(column, {
        filter: 'agDateColumnFilter', filterParams: { buttons: ['apply', 'reset'] }
    }),

    /**
     * 默认列属性
     * @param {any} ops
     */
    defaultColDef: ops => Object.assign({
        width: 150, minWidth: 100, maxWidth: 4000, filter: true, sortable: true, resizable: true,
        filter: 'agMultiColumnFilter', menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
    }, ops),

    /**
     * 默认分组属性
     * @param {any} ops
     */
    autoGroupColumnDef: ops => Object.assign({
        width: 300, maxWidth: 4000
    }, ops),

    /**
     * 默认配置
     * @param {any} ops
     */
    optionDef: ops => Object.assign({
        localeText: ag.localeText, //语言
        defaultColDef: ag.defaultColDef(), //列配置        
        autoGroupColumnDef: ag.autoGroupColumnDef(), //分组
        suppressFieldDotNotation: true, //字段 key 允许包含 .
        rowGroupPanelShow: 'always', //启用列拖拽分组 'never', 'always', 'onlyWhenGrouping'
        enableBrowserTooltips: true, //提示
        rowSelection: 'multiple', //多选
        suppressRowClickSelection: true, //单击行不选择
        enableRangeSelection: true, //范围选择
        autoSizePadding: 40, //自动调整列宽追加值（标题动态图标、排序标记等）
        headerHeight: 32, //表头高度
        pagination: false, //不分页
        paginationPageSize: 100,
        cacheBlockSize: 100,
        suppressMoveWhenRowDragging: true, //拖拽不实时移动
        animateRows: true, //动画
        isRowSelectable: rowNode => rowNode.group !== true, //非分组显示复选框        
        onSortChanged: event => event.api.refreshCells(), //排序后刷新（更新行号）        
        onFilterChanged: event => event.api.refreshCells(), //过滤后刷新（更新行号）
        onRowGroupOpened: event => event.api.refreshCells(), //组展开后刷新（更新行号）
    }, ops),

    /**
     * 行号
     * @param {any} ops
     * @returns
     */
    numberCol: ops => Object.assign({
        headerName: "🆔", valueGetter: "node.rowIndex + 1", width: 120, maxWidth: 150,
        checkboxSelection: true, headerCheckboxSelection: true,
        headerCheckboxSelectionFilteredOnly: true, //仅全选过滤的数据行
        sortable: false, filter: false, menuTabs: false
    }, ops),

    /**
     * 过滤器
     * @param {*} type 
     * @param {*} ops 
     * @returns 
     */
    filterParamsDef: (type, ops) => {
        switch (type) {
            case "Number":
                return { filters: [{ filter: `ag${type}ColumnFilter` }, { filter: 'agSetColumnFilter', }] }
            case "Date":
                return {
                    filters: [
                        {
                            filter: 'agDateColumnFilter',
                            filterParams: {
                                comparator: function (filterDate, cellValue) {
                                    if (cellValue == null || cellValue == "") return -1;

                                    //仅比较日期
                                    var cellDate = new Date(cellValue);
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
                                }
                            },
                        },
                        {
                            filter: 'agSetColumnFilter',
                            filterParams: { comparator: (a, b) => a = b },
                        },
                    ],
                };
        }
        return ops;
    },

    /**
     * 获取所有行
     * @param {*} gridOps 
     * @param {*} isLeaf 
     * @returns 
     */
    getAllRows: function (gridOps, isLeaf = true) {
        let rowData = [];
        if (isLeaf) {
            gridOps.api.forEachLeafNode(node => rowData.push(node.data));
        } else {
            gridOps.api.forEachNode(node => rowData.push(node.data));
        }
        return rowData;
    },

    /**
     * 获取选中或范围的行
     * @param {*} gridOps 
     * @returns 
     */
    getSelectedOrRangeRow: function (gridOps) {
        var srows = gridOps.api.getSelectedRows(), crows = gridOps.api.getCellRanges();
        if (srows.length > 0) {
            return srows[0]
        } else if (crows.length > 0) {
            return gridOps.api.getDisplayedRowAtIndex(crows[0].startRow.rowIndex).data
        }
    },

    /**
     * 获取容器
     * @param {*} event 
     * @returns 
     */
    getContainer: event => event.api.gridOptionsWrapper.environment.eGridDiv,

    /**
     * 设置加载状态
     * @param {*} gridOps 
     * @param {*} isHide 
     */
    setGridLoading: function (gridOps, isHide) {
        if (gridOps && gridOps.api) {
            switch (isHide) {
                case 1:
                    gridOps.api.hideOverlay();
                    break;
                case 2:
                    gridOps.api.hideOverlay();
                    gridOps.api.showNoRowsOverlay();
                    break;
                default:
                    gridOps.api.showLoadingOverlay();
            }
        }
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

        // Other
        loadingOoo: '加载中...',
        noRowsToShow: '（空）',
        enabled: '启用',

        // Menu
        pinColumn: '固定列',
        pinLeft: '左固定',
        pinRight: '右固定',
        noPin: '取消固定',
        valueAggregation: '合计',
        autosizeThiscolumn: '当前列大小自适应',
        autosizeAllColumns: '所有列大小自适应',
        groupBy: '分组',
        ungroupBy: '不分组',
        addToValues: '添加值 ${variable}',
        removeFromValues: '移除值 ${variable}',
        addToLabels: '添加到标签 ${variable}',
        removeFromLabels: '移除标签 ${variable}',
        resetColumns: '重置列',
        expandAll: '展开全部',
        collapseAll: '折叠全部',
        copy: '复制',
        ctrlC: 'Ctrl+C',
        copyWithHeaders: '复制（带标题）',
        copyWithHeaderGroups: '复制（带分组）',
        paste: '粘贴',
        ctrlV: 'Ctrl+V',
        export: '内置保存',
        csvExport: 'CSV 导出',
        excelExport: 'Excel 导出',

        // Enterprise Menu Aggregation and Status Bar
        sum: '求和',
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
        of: '，总共',
        page: '当前',
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
        noDataToChart: 'No data available to be charted.',
        pivotChartRequiresPivotMode: 'Pivot Chart requires Pivot Mode enabled.',
        chartSettingsToolbarTooltip: 'Menu',
        chartLinkToolbarTooltip: 'Linked to Grid',
        chartUnlinkToolbarTooltip: 'Unlinked from Grid',
        chartDownloadToolbarTooltip: 'Download Chart',
    }
}

export { ag }