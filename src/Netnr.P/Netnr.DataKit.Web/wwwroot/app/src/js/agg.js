var agg = {
    lk: () => agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { },

    /**
     * 默认列属性
     * @param {any} ops
     */
    defaultColDef: ops => Object.assign({
        width: 150, maxWidth: 2000, filter: true, sortable: true, resizable: true,
        menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
    }, ops),

    /**
     * 默认分组属性
     * @param {any} ops
     */
    autoGroupColumnDef: ops => Object.assign({
        width: 300, maxWidth: 2000
    }, ops),

    /**
     * 默认配置
     * @param {any} ops
     */
    optionDef: ops => Object.assign({
        localeText: agg.localeText, //语言
        defaultColDef: agg.defaultColDef(), //列配置
        autoGroupColumnDef: agg.autoGroupColumnDef(), //分组
        rowGroupPanelShow: 'always', //启用列拖拽分组 'never', 'always', 'onlyWhenGrouping'
        enableBrowserTooltips: true, //提示
        rowSelection: 'multiple', //多选
        suppressRowClickSelection: true, //单击行不选择
        enableRangeSelection: true, //范围选择
        headerHeight: 42, //表头高度
        pagination: false, //不分页
        paginationPageSize: 100,
        cacheBlockSize: 100,
        suppressMoveWhenRowDragging: true, //拖拽不实时移动
        animateRows: true, //动画
    }, ops),

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
        copyWithHeaders: '复制带标题',
        paste: '粘贴',
        ctrlV: 'Ctrl+V',
        export: '内置导出',
        csvExport: '导出 CSV',
        excelExport: '导出 Excel',
        excelXmlExport: '导出 XML',

        // Enterprise Menu Aggregation and Status Bar
        sum: '求和',
        min: '最小',
        max: '最大',
        none: '无',
        count: '总数',
        avg: '平均',
        filteredRows: '已过滤',
        selectedRows: '选中行',
        totalRows: '总行',
        totalAndFilteredRows: '行',
        more: '更多',
        to: '-',
        of: '，共',
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
        thickness: 'Thickness',
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
        legend: 'Legend',
        position: '位置',
        markerSize: 'Marker Size',
        markerStroke: 'Marker Stroke',
        markerPadding: 'Marker Padding',
        itemSpacing: 'Item Spacing',
        itemPaddingX: 'Item Padding X',
        itemPaddingY: 'Item Padding Y',
        layoutHorizontalSpacing: 'Horizontal Spacing',
        layoutVerticalSpacing: 'Vertical Spacing',
        strokeWidth: 'Stroke Width',
        offset: 'Offset',
        offsets: 'Offsets',
        tooltips: 'Tooltips',
        callout: 'Callout',
        markers: 'Markers',
        shadow: 'Shadow',
        blur: 'Blur',
        xOffset: 'X Offset',
        yOffset: 'Y Offset',
        lineWidth: 'Line Width',
        normal: 'Normal',
        bold: 'Bold',
        italic: 'Italic',
        boldItalic: 'Bold Italic',
        predefined: 'Predefined',
        fillOpacity: 'Fill Opacity',
        strokeOpacity: 'Line Opacity',
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

window.addEventListener("DOMContentLoaded", function () {
    agg.lk();
}, false);

export { agg }