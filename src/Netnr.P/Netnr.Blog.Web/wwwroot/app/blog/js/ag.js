var ag = {
    agSetColumn: (column, obj) => {
        return Object.assign(column, {
            cellRenderer: params => {
                if (params.value in obj) {
                    return obj[params.value];
                }
                return params.value;
            },
            filter: 'agSetColumnFilter', filterParams: { values: Object.keys(obj) }
        });
    },

    lk: function () {
        agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { };
    },

    /**
     * 默认列属性
     * @param {any} ops
     */
    defaultColDef: ops => Object.assign({
        width: 150, maxWidth: 4000, filter: true, sortable: true, resizable: true,
        filter: 'agMultiColumnFilter', menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
    }, ops),

    /**
     * 默认分组属性
     * @param {any} ops
     */
    autoGroupColumnDef: ops => Object.assign({ width: 300, maxWidth: 4000 }, ops),

    /**
     * 默认配置
     * @param {any} ops
     */
    optionDef: ops => Object.assign({
        localeText: ag.localeText, //语言
        defaultColDef: ag.defaultColDef(), //列配置
        autoGroupColumnDef: ag.autoGroupColumnDef(), //分组
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
        startsWith: '头包含',
        endsWith: '尾包含',

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
        ungroupBy: '取消分组',
        resetColumns: '重置列',
        expandAll: '展开全部',
        collapseAll: '折叠全部',
        copy: '复制',
        ctrlC: 'Ctrl+C',
        copyWithHeaders: '复制（带标题）',
        paste: '粘贴',
        ctrlV: 'Ctrl+V',
        export: '内置保存',
        csvExport: '保存为 CSV',
        excelExport: '保存为 Excel',
        excelXmlExport: '保存为 XML',

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

        // Enterprise Menu (Charts)
        pivotChartAndPivotMode: '图表枢轴 & 枢轴模式',
        pivotChart: '图表枢轴',
        chartRange: '范围图表',

        columnChart: '柱状图',
        groupedColumn: '分组',
        stackedColumn: 'Stacked',
        normalizedColumn: '100% Stacked',

        barChart: '条形图',
        groupedBar: '分组',
        stackedBar: 'Stacked',
        normalizedBar: '100% Stacked',

        pieChart: '饼形图',
        pie: 'Pie',
        doughnut: 'Doughnut',

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

        // ARIA
        ariaHidden: 'hidden',
        ariaVisible: 'visible',
        ariaChecked: 'checked',
        ariaUnchecked: 'unchecked',
        ariaIndeterminate: 'indeterminate',
        ariaColumnSelectAll: 'Toggle Select All Columns',
        ariaInputEditor: 'Input Editor',
        ariaDateFilterInput: 'Date Filter Input',
        ariaFilterInput: 'Filter Input',
        ariaFilterColumnsInput: 'Filter Columns Input',
        ariaFilterValue: 'Filter Value',
        ariaFilterFromValue: 'Filter from value',
        ariaFilterToValue: 'Filter to value',
        ariaFilteringOperator: 'Filtering Operator',
        ariaColumnToggleVisibility: 'column toggle visibility',
        ariaColumnGroupToggleVisibility: 'column group toggle visibility',
        ariaRowSelect: 'Press SPACE to select this row',
        ariaRowDeselect: 'Press SPACE to deselect this row',
        ariaRowToggleSelection: 'Press Space to toggle row selection',
        ariaRowSelectAll: 'Press Space to toggle all rows selection',
        ariaSearch: 'Search',
        ariaSearchFilterValues: 'Search filter values'
    }
}

export { ag }