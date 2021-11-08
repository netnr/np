var agg = {
    lk: function () {
        agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { };
    },
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
        format: '格式化',
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
};
agg.lk();

var ndk = {
    init: function () {
        setTimeout(() => {
            ndk.domLoading.classList.add("d-none");
            ndk.domMain.classList.remove("d-none");

            ndk.eventInit();
        }, 500);
    },

    theme: "light", //主题 可选 dark
    version: '0.1.0',

    //数据库类型
    typeDB: ["SQLite", "MySQL", "MariaDB", "Oracle", "SQLServer", "PostgreSQL"],
    iconDB: type => ["🖤", "💚", "🤎", "💗", "🧡", "💙"][ndk.typeDB.indexOf(type)], //对应图标
    typeEnv: ["Development", "Test", "Production"], //环境类型
    iconEnv: env => ["⚪", "🔵", "🔴"][ndk.typeEnv.indexOf(env)], //环境图标
    icons: {
        id: "🆔",
        connType: "💞",
        connAlias: "👻",
        connOrder: "🚩",
        connGroup: "👪",
        connEnv: "⚪",
        connConn: "🔗",
        connDatabase: "🔋",
        loading: "🔄",
        generate: "🎲",
        ctrl: "🔧",
        name: "📛",
        key: "🔑",
        edit: "✍",
        incr: "➚",
        comment: "📝",
    },

    //接口头部参数（如：{ Authorization: "token" }）
    apiHeaders: null,

    domLoading: document.querySelector('.nr-loading'), //载入
    domMain: document.querySelector('.nr-main'), //主体

    domMenu: document.querySelector(".nr-menu"), //菜单
    domCardConnDatabase: document.querySelector('.nr-card-conn-database'), //连接-数据库层
    domGridConnDatabase: document.querySelector('.nr-grid-conn-database'), //连接-数据库grid

    domStatus: document.querySelector(".nr-status"), //请求状态

    domGridTable: document.querySelector(".nr-grid-table"), //表grid

    domTabsTool: document.querySelector(".nr-tabs-tool"), //选项卡公用工具栏
    domTabsManager: document.querySelector(".nr-tabs-manager"), //选项卡管理
    domGridTabs: document.querySelector(".nr-grid-tabs"), //选项卡gird
    domTabsMenu: document.querySelector('.nr-tabsmenu'), //选项卡菜单组
    domTabs: document.querySelector('.nr-tabs'), //选项卡内容组

    domGridColumn: document.querySelector(".nr-grid-column"), //列grid

    domSqlExecuteSql: document.querySelector(".nr-sql-executesql"), //查询grid
    domGridExecuteSql: document.querySelector(".nr-grid-executesql"), //查询grid
    domBtnExecuteSql: document.querySelector(".nr-execute-sql"), //执行sql
    domChkesSql: document.querySelector("#chk-es-sql"),
    domChkesGrid: document.querySelector("#chk-es-grid"),

    domMsgModal: document.querySelector('.nr-msgModal'), //消息层

    vary: {}, //变量

    //模板 连接字符串
    templateConn: {
        SQLite: "Data Source=<文件物理路径，或网络路径，后台自动下载文件>",
        MySQL: "Server=<IP>;Port=3306;Uid=<USER>;Pwd=<PWD>;Database=<DBNAME>;",
        MariaDB: "Server=<IP>;Port=3306;Uid=<USER>;Pwd=<PWD>;Database=<DBNAME>;",
        Oracle: "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<IP>)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=<USER>;Password=<PWD>;",
        SQLServer: "Server=<IP>,1433;User Id=<USER>;Password=<PWD>;Database=<DBNAME>;TrustServerCertificate=True;",
        PostgreSQL: "Server=<IP>;Port=5432;User Id=<USER>;Password=<PWD>;Database=<DBNAME>;"
    },

    //示例连接
    demoConn: [
        { id: 10001, type: "SQLite", alias: "SQLite netnrf", group: "demo", order: 1, env: "Test", conn: "Data Source=https://s1.netnr.eu.org/2020/05/22/2037505934.db" },
        { id: 10002, type: "MySQL", alias: "Heroku JawsDB（ustf345c1n0wkaow）", group: "demo", order: 2, env: "Test", conn: "Server=c8u4r7fp8i8qaniw.chr7pe7iynqr.eu-west-1.rds.amazonaws.com;Port=3306;Uid=fyxnmvubyl01t2k9;Pwd=ai7a4eg3c31scfcm;Database=ustf345c1n0wkaow;" },
        { id: 10003, type: "MariaDB", alias: "Heroku JawsDB（gvx25hgtxzfr2lia）", group: "demo", order: 3, env: "Test", conn: "Server=eporqep6b4b8ql12.chr7pe7iynqr.eu-west-1.rds.amazonaws.com;Port=3306;Uid=hydfd5qr08d3akt9;Pwd=tk53sieop5ua97pv;Database=gvx25hgtxzfr2lia;" },
        { id: 10004, type: "SQLServer", alias: "SOMEE MSSQL（netnr-kit）", group: "demo", order: 5, env: "Test", conn: "Server=198.37.116.112,1433;User Id=netnr_SQLLogin_1;Password=o2y9vrbjac;Database=netnr-kit;TrustServerCertificate=True;" },
        { id: 10005, type: "PostgreSQL", alias: "Heroku PostgreSQL（d7mhfq80unm96q）", group: "demo", order: 6, env: "Test", conn: "Server=ec2-54-74-35-87.eu-west-1.compute.amazonaws.com;Port=5432;User Id=psphnovbbmsgtj;Password=7554b25380195aa5755a24c7f6e1f9f94f3de3dcef9c345c7e93ae8b07699ace;Database=d7mhfq80unm96q;;SslMode=Require;Trust Server Certificate=true;" }
    ],

    //模板 主机
    templateServer: [
        { host: "https://www.netnr.eu.org/api/v1", remark: "基于 Heroku 构建" },
        { host: location.origin, remark: "当前" }
    ],

    //接口
    apiServer: location.origin,
    apiGetDatabase: "/DK/GetDatabase",
    apiGetTable: "/DK/GetTable",
    apiGetColumn: "/DK/GetColumn",
    apiSetTableComment: "/DK/SetTableComment",
    apiSetColumnComment: "/DK/SetColumnComment",
    apiExecuteSql: "/DK/ExecuteSql",
    apiGetData: "/DK/GetData",
    apiGetDEI: "/DK/GetDEI",

    //连接信息
    apiConnDatabase: null,

    /**
     * 设置连接信息
     * @param {any} connDatabase
     */
    setConnDatabase: function (connDatabase) {

        var hideConn = false;
        if (ndk.gridOpsConnDatabase && ndk.gridOpsConnDatabase.columnApi) {
            var cdcs = ndk.gridOpsConnDatabase.columnApi.getColumnState();
            hideConn = cdcs.filter(x => x.colId == "conn" && x.hide == true).length > 0;
        }
        var viewItem = [
            ndk.iconDB(connDatabase.type) + connDatabase.type,
            ndk.icons.connAlias + connDatabase.alias,
            ndk.icons.connDatabase + connDatabase.databaseName,
            ndk.iconEnv(connDatabase.env) + connDatabase.env
        ];
        if (!hideConn) {
            viewItem.push(ndk.icons.connConn + connDatabase.conn)
        }
        var btncd = ndk.domCardConnDatabase.previousElementSibling;
        btncd.innerHTML = ndk.iconDB(connDatabase.type) + connDatabase.alias + ndk.iconEnv(connDatabase.env);
        btncd.title = viewItem.join(' ');
        ndk.apiConnDatabase = connDatabase;
        ndk.stepSave();
    },

    //事件初始化
    eventInit: function () {

        //菜单项事件
        document.body.addEventListener('click', function (e) {
            var cmd = e.target.getAttribute("data-cmd")
            ndk.actionRun(cmd);
        }, false);

        //连接-数据库模态框
        ndk.bsConnDatabase = new bootstrap.Offcanvas(ndk.domCardConnDatabase);
        //连接-数据库 阻止 ESC
        ndk.domGridConnDatabase.addEventListener('keydown', function (e) {
            e.stopPropagation();
        }, false)

        //ExecuteSql change
        ndk.domChkesGrid.addEventListener('click', function () {
            ndk.domGridExecuteSql.classList.remove('d-none')
            ndk.domSqlExecuteSql.classList.add('d-none')
            ndk.size()
        }, false)
        ndk.domChkesSql.addEventListener('click', function () {
            ndk.domGridExecuteSql.classList.add('d-none')
            ndk.domSqlExecuteSql.classList.remove('d-none')
            ndk.size()
        }, false)
        //执行（选中）SQL
        ndk.domBtnExecuteSql.addEventListener('click', function () {
            var sql = ndk.meSql.getModel().getValueInRange(ndk.meSql.getSelection()).trim();
            if (sql == "") {
                sql = ndk.meSql.getValue()
            }
            if (sql == "") {
                ndk.msg("执行 SQL 不能为空");
            } else {
                ndk.loadExecuteSql(ndk.apiConnDatabase, sql);
            }
        }, false)

        //自适应
        ndk.size();
        window.addEventListener('resize', () => ndk.size(), false)

        //恢复
        ndk.stepRead();

        //加载连接及数据库
        ndk.loadConnDatabase();
    },

    /**
     * 动作命令
     * @param {any} cmd 
     */
    actionRun: function (cmd) {
        switch (cmd) {
            //主题
            case "theme-light":
            case "theme-dark":
                {
                    ndk.theme = cmd.split('-').pop();
                    ndk.themeGrid(ndk.theme);
                    ndk.themeEditor(ndk.theme);
                    if (ndk.theme == "dark") {
                        document.body.classList.add("nr-theme-dark");
                    } else {
                        document.body.classList.remove("nr-theme-dark");
                    }
                    ndk.stepSave();
                }
                break;
            //查询
            case "executesql":
                {
                    ndk.tabOpen("executesql");
                }
                break;
            //重置表
            case "reset-grid-table":
                {
                    if (ndk.gridOpsTable && ndk.gridOpsTable.api) {
                        ndk.gridOpsTable.api.destroy();
                    }
                    ndk.stepSave();
                }
                break;
            //重置列
            case "reset-grid-column":
                {
                    if (ndk.gridOpsColumn && ndk.gridOpsColumn.api) {
                        ndk.gridOpsColumn.api.destroy();
                    }
                    ndk.stepSave();
                }
                break;
            //加载表
            case "load-table":
                ndk.loadTable(ndk.apiConnDatabase)
                break;
            //加载列
            case "load-column":
                {
                    var filterTableName = "";
                    var srow = ndk.gridOpsTable.api.getSelectedRows();
                    if (srow.length > 0) {
                        if (ndk.gridOpsTable.rowData.length != srow.length) {
                            filterTableName = srow.map(x => x.TableName).join(',');
                        }
                        ndk.loadColumn(ndk.apiConnDatabase, filterTableName);
                    } else {
                        ndk.msg("请选择表")
                    }
                }
                break;
            //缩放
            case "max-box1":
            case "max-box2":
                {
                    if (ndk.domMain.classList.contains(cmd)) {
                        ndk.domMain.classList.remove(cmd);
                    } else {
                        for (var i = ndk.domMain.classList.length - 1; i >= 0; i--) {
                            var cls = ndk.domMain.classList[i];
                            if (cls.startsWith("max-box")) {
                                ndk.domMain.classList.remove(cls)
                            }
                        }
                        ndk.domMain.classList.add(cmd);
                    }
                    ndk.size();
                    ndk.stepSave();
                }
                break;

            //折叠/展开 列
            case "expand-grid-column":
            case "collapse-grid-column":
                if (ndk.gridOpsColumn && ndk.gridOpsColumn.api) {
                    ndk.gridOpsColumn.api.forEachNode((node) => {
                        if (node.level == 0) {
                            node.setExpanded(cmd.startsWith('expand'));
                        }
                    });
                    ndk.stepSave();
                }
                break;
        }
    },

    //步骤项
    stepList: [
        "step-theme", //主题
        "step-conn-database", //连接-数据库
        "step-max-box", //最大化
        "step-load-table", //表载入
        "step-filter-table", //表过滤
        "step-selected-table", //表选中
        "step-view-tab", //选项卡显示
        {
            //表设计
            "step-view-tab-column": [
                "step-load-column", //列载入
                "step-filter-column", //列过滤
                "step-selected-column" //列选中
            ],
            //执行SQL
            "step-view-tab-executesql": [
                "step-editor-content" //编辑器内容
            ]
        }
    ],

    //步骤保存
    stepSave: function () {
        //非恢复读取中
        if (ndk.vary.steping != 1) {

            var wobj = {
                theme: ndk.theme,
                connDatabase: ndk.apiConnDatabase,
                maxGrid: '',
                loadTable: ndk.gridOpsTable && ndk.gridOpsTable.api ? true : false,
                selectedTable: [],
                loadColumn: ndk.gridOpsColumn && ndk.gridOpsColumn.api ? true : false,
                selectedColumn: [],
            }

            //缩放表格
            for (var i = 0; i < ndk.domMain.classList.length; i++) {
                var cls = ndk.domMain.classList[i];
                if (cls.startsWith('max-box')) {
                    wobj.maxGrid = cls;
                    break;
                }
            }

            if (wobj.loadTable) {
                wobj.selectedTable = ndk.gridOpsTable.api.getSelectedRows().map(x => x.TableName);
            }
            if (wobj.loadColumn) {
                wobj.selectedColumn = ndk.gridOpsColumn.api.getSelectedRows().map(x => x.TableName + ":" + x.ColumnName);
            }

            localforage.setItem("steps", wobj);
        }
    },

    //步骤读取
    stepRead: function () {
        if (ndk.vary.steping == 1) {
            ndk.msg("正在读取中...")
        } else {
            ndk.vary.stepStartDate = Date.now();
            ndk.vary.steping = 1;
            ndk.vary.stepIndex = 0;

            ndk.vary.stepDefer = setTimeout(() => {
                ndk.vary.stepEndDate = Date.now();
                ndk.vary.steping = -1;
                console.log("step timeout");
            }, 1000 * 60)

            localforage.getItem("steps").then(wobj => {
                ndk.stepRun(ndk.vary.stepIndex, ndk.stepList, wobj)
            })
        }
    },

    /**
     * 步骤运行
     * @param {any} index
     * @param {any} stepList
     * @param {any} wobj
     */
    stepRun: function (index, stepList, wobj) {
        if (index < stepList.length) {
            var step = stepList[index++];

            ndk.stepItem(step, wobj).then(() => {
                ndk.stepRun(index, stepList, wobj)
            }).catch(() => {
                ndk.vary.stepEndDate = Date.now();
                ndk.vary.steping = -1;
                clearTimeout(ndk.vary.stepDefer);
                console.log("step end");
            })
        } else {
            ndk.vary.stepEndDate = Date.now();
            ndk.vary.steping = 2;
            clearTimeout(ndk.vary.stepDefer);
            console.log(`step done ${ndk.vary.stepEndDate - ndk.vary.stepStartDate}`);
        }
    },

    /**
     * 步骤
     * @param {any} step
     * @param {any} wobj
     */
    stepItem: function (step, wobj) {
        return new Promise((resolve, reject) => {
            switch (step) {
                //主题
                case "step-theme":
                    ndk.actionRun(wobj.theme == "dark" ? "theme-dark" : "theme-light")
                    resolve();
                    break;
                //连接-数据库
                case "step-conn-database":
                    if (wobj.connDatabase) {
                        ndk.setConnDatabase(wobj.connDatabase);
                        resolve()
                    } else {
                        reject()
                    }
                    break;
                //表格全屏
                case "step-max-box":
                    if (wobj.maxGrid != "") {
                        ndk.actionRun(wobj.maxGrid)
                    }
                    resolve()
                    break;
                //加载表
                case "step-load-table":
                    if (wobj.loadTable) {
                        ndk.loadTable(wobj.connDatabase).then(() => {
                            resolve()
                        }).catch(err => reject(err))
                    } else {
                        reject()
                    }
                    break;
                //选择表
                case "step-selected-table":
                    if (wobj.selectedTable.length) {
                        ndk.gridOpsTable.api.forEachNode(node => {
                            if (wobj.selectedTable.includes(node.data.TableName)) {
                                node.setSelected(true)
                            }
                        })
                    }
                    resolve()
                    break;
                //加载列
                case "step-load-column":
                    if (wobj.loadColumn) {
                        ndk.loadColumn(wobj.connDatabase, wobj.selectedTable.join(",")).then(() => {
                            resolve()
                        })
                    } else {
                        reject()
                    }
                    break;
                //选择列
                case "step-selected-column":
                    if (wobj.selectedColumn.length) {
                        ndk.gridOpsColumn.api.forEachNode((node) => {
                            if (node.data && wobj.selectedColumn.includes(node.data.TableName + ":" + node.data.ColumnName)) {
                                node.setSelected(true)
                            }
                        });
                    }
                    resolve()
                    break;
                default:
                    break;
            }
        })
    },

    //新的连接ID
    random: (s, e) => Math.floor(Math.random() * (e - s + 1) + s),

    /**
     * 连接字符串 设置
     * @param {any} connObj
     */
    connSet: function (connObj) {
        return new Promise((resolve) => {
            ndk.connGet().then(conns => {
                if (ndk.type(connObj) != "Array") {
                    connObj = [connObj];
                }

                connObj.forEach(x => {
                    for (var i = conns.length - 1; i >= 0; i--) {
                        var cobj = conns[i];
                        if (cobj.id == x.id) {
                            conns.splice(i, 1);
                            break;
                        }
                    }
                });
                conns = conns.concat(connObj);

                localforage.setItem("conns", conns).then(() => {
                    resolve(conns);
                })
            })
        })
    },

    /**
     * 连接字符串 设置
     * @param {any} connObj
     */
    connDelete: function (id) {
        return new Promise((resolve) => {
            ndk.connGet().then(conns => {
                for (var i = 0; i < conns.length; i++) {
                    var cobj = conns[i];
                    if (cobj.id == id) {
                        conns.splice(i, 1);
                        break;
                    }
                }
                localforage.setItem("conns", conns).then(() => {
                    resolve(conns);
                })
            })
        })
    },

    /**
     * 连接字符串获取
     * @param {any} id 可选 id
     */
    connGet: function (id) {
        return new Promise((resolve) => {
            localforage.getItem("conns").then((conns) => {
                conns = conns || [];
                if (id != null) {
                    resolve(conns.filter(x => x.id == id).pop());
                } else {
                    resolve(conns)
                }
            })
        })
    },

    //加载连接及数据库
    loadConnDatabase: function () {
        return new Promise((resolve) => {
            ndk.connGet().then(conns => {
                if (conns.length == 0) {
                    conns = ndk.demoConn
                }
                conns.sort((a, b) => a.order - b.order);

                var gopscd = {
                    localeText: agg.localeText, //语言
                    //默认列属性配置
                    defaultColDef: {
                        width: 130, maxWidth: 2000, filter: true, sortable: true, editable: true, resizable: true,
                        menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
                    },
                    //列
                    columnDefs: [
                        { field: 'id', headerName: ndk.icons.id + "连接", width: 150, cellRenderer: 'agGroupCellRenderer', rowDrag: true, editable: false, sortable: false, },
                        {
                            field: 'type', headerName: ndk.icons.connType + "类型", enableRowGroup: true, width: 160,
                            cellRenderer: params => params.value ? ndk.iconDB(params.value) + params.value : "",
                            cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                                values: ndk.typeDB, formatValue: fv => ndk.iconDB(fv) + fv
                            }
                        },
                        { field: 'alias', headerName: ndk.icons.connAlias + "别名", width: 350 },
                        { field: 'group', headerName: ndk.icons.connGroup + "分组", width: 160, enableRowGroup: true },
                        { field: 'order', headerName: ndk.icons.connOrder + "排序" },
                        {
                            field: 'env', headerName: ndk.icons.connEnv + "环境", width: 160,
                            cellRenderer: params => params.value ? ndk.iconEnv(params.value) + params.value : "",
                            cellEditor: 'agRichSelectCellEditor', cellEditorParams: {
                                values: ndk.typeEnv, formatValue: fv => ndk.iconEnv(fv) + fv
                            }
                        },
                        { field: 'conn', headerName: ndk.icons.connConn + "连接字符串", width: 600, cellEditor: 'agLargeTextCellEditor', },
                        {
                            headerName: ndk.icons.ctrl + "操作", pinned: 'right', width: 110, filter: false, sortable: false, editable: false, menuTabs: false,
                            cellRenderer: class {
                                init(params) {
                                    this.eGui = document.createElement('div');

                                    //非分组
                                    if (params.data) {
                                        this.eGui.innerHTML = `
                                        <a href="javascript:void(0);" class="text-decoration-none nr-conn-cell-refresh" title="刷新库">♻</a> &nbsp;
                                        <a href="javascript:void(0);" class="text-decoration-none nr-conn-cell-add" title="新增">➕</a> &nbsp;
                                        <a href="javascript:void(0);" class="text-decoration-none nr-conn-cell-del" title="删除">❌</a>
                                       `;

                                        this.eGui.addEventListener('click', function (e) {
                                            var target = e.target;

                                            if (target.classList.contains("nr-conn-cell-add")) {
                                                //复制连接
                                                var newrow = { ...params.data };
                                                newrow.id = ndk.random(20000, 99999);
                                                newrow.alias += "+";

                                                ndk.gridOpsConnDatabase.api.applyTransaction({
                                                    add: [newrow],
                                                    addIndex: params.rowIndex + 1
                                                });

                                                ndk.connSet(newrow);
                                            } else if (target.classList.contains("nr-conn-cell-del")) {
                                                //删除连接
                                                if (confirm("确定删除？")) {
                                                    ndk.gridOpsConnDatabase.api.applyTransaction({
                                                        remove: [params.data]
                                                    });
                                                    ndk.connDelete(params.data.id)
                                                }
                                            } else if (target.classList.contains("nr-conn-cell-refresh")) {
                                                //刷新库
                                                var dgrid = ndk.gridOpsConnDatabase.api.getDetailGridInfo(`detail_${params.data.id}`)
                                                if (dgrid != null) {
                                                    ndk.setGridLoading(dgrid, 0)

                                                    ndk.loadDatabase(params.data).then(dbs => {
                                                        ndk.setGridLoading(dgrid, 1)

                                                        dbs.forEach(d => d.conn = params.data);
                                                        dgrid.api.setRowData(dbs)
                                                    }).catch(() => {
                                                        ndk.setGridLoading(dgrid, 2)
                                                    })
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
                    rowGroupPanelShow: 'always', //分组面板
                    masterDetail: true, // 子表
                    keepDetailRows: true, // 保持子表（不刷新）
                    getRowHeight: function (params) {
                        if (params.node && params.node.detail) {
                            var gridSizes = params.api.getSizesForCurrentTheme();
                            return gridSizes.rowHeight * 13 + gridSizes.headerHeight;
                        }
                    },
                    // 单元格变动
                    onCellValueChanged: function () {
                        //编辑连接信息
                        ndk.connSet(ndk.getAllRows(ndk.gridOpsConnDatabase, true));
                    },
                    getRowNodeId: data => data.id,
                    detailCellRendererParams: {
                        detailGridOptions: {
                            //默认列属性配置
                            defaultColDef: {
                                width: 140, filter: true, sortable: true, resizable: true,
                                menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
                            },
                            columnDefs: [
                                { field: 'DatabaseName', headerName: "🔋库名", width: 300, checkboxSelection: true },
                                { field: 'DatabaseClassify', headerName: "类别", enableRowGroup: true },
                                { field: 'DatabaseOwner', headerName: "所属者", enableRowGroup: true },
                                { field: 'DatabaseSpace', headerName: "表空间", enableRowGroup: true },
                                { field: 'DatabaseCharset', headerName: "字符集", enableRowGroup: true },
                                { field: 'DatabaseCollation', headerName: "排序规则", width: 180, enableRowGroup: true },
                                { field: 'DatabaseDataLength', headerName: "数据大小", cellRenderer: params => params.value > 0 ? ndk.formatByteSize(params.value) : "" },
                                { field: 'DatabaseLogLength', headerName: "日志大小", cellRenderer: params => params.value > 0 ? ndk.formatByteSize(params.value) : "" },
                                { field: 'DatabaseIndexLength', headerName: "索引大小", cellRenderer: params => params.value > 0 ? ndk.formatByteSize(params.value) : "" },
                                { field: 'DatabasePath', headerName: "库路径", width: 400 },
                                { field: 'DatabaseLogPath', headerName: "日志路径", width: 400 },
                                { field: 'DatabaseCreateTime', headerName: "创建时间", width: 200 }
                            ],
                            autoGroupColumnDef: {
                                minWidth: 200,
                            },
                            rowGroupPanelShow: 'always', //分组面板
                            animateRows: true, //动画
                            onRowSelected: function (event) {
                                if (event.node.isSelected()) {
                                    var sdata = event.node.data;

                                    ndk.gridOpsConnDatabase.api.forEachDetailGridInfo(detailGridInfo => {
                                        //清除其它连接选择
                                        if (detailGridInfo.id.split('_').pop() != sdata.conn.id) {
                                            detailGridInfo.api.forEachNode(node => {
                                                node.setSelected(false)
                                            })
                                        }

                                        detailGridInfo.api.setDatasource([])
                                    })

                                    //设置连接-数据库
                                    var connDatabase = sdata.conn;
                                    connDatabase.databaseName = sdata.DatabaseName;
                                    ndk.setConnDatabase(connDatabase);

                                    setTimeout(() => ndk.bsConnDatabase.hide(), 500)
                                    ndk.actionRun('reset-grid-table');
                                    ndk.actionRun('reset-grid-column');
                                    //加载表
                                    ndk.loadTable(connDatabase);
                                }
                            },
                            //双击数据库
                            onRowDoubleClicked: function (params) {
                                params.node.setSelected(true);
                            }
                        },
                        //子表数据
                        getDetailRowData: function (params) {

                            var dbkey = params.data.type == "Oracle" ? "user id=" : "database=", databaseName,
                                dbs = params.data.conn.split(';').filter(kv => kv.toLowerCase().startsWith(dbkey));
                            if (dbs.length) {
                                databaseName = dbs[0].split('=').pop();
                            }

                            ndk.loadDatabase(params.data).then(dbs => {
                                dbs.forEach(d => d.conn = params.data);
                                params.successCallback(dbs)

                                //自动过滤数据库
                                if (databaseName != null) {
                                    params.node.detailGridInfo.api.setFilterModel({
                                        DatabaseName: {
                                            type: 'set',
                                            values: [databaseName]
                                        }
                                    })
                                }
                            }).catch((e) => {
                                console.log(e);
                                params.successCallback()
                            })
                        },
                    },
                    //连接菜单项
                    getContextMenuItems: (event) => {

                        //新增连接
                        var adddbs = [];
                        ndk.typeDB.forEach(type => {
                            adddbs.push({
                                name: type,
                                icon: ndk.iconDB(type),
                                action: function () {
                                    var order = event.node ? event.node.rowIndex + 1 : ndk.getAllRows(ndk.gridOpsConnDatabase, true).length;

                                    var newrow = {
                                        id: ndk.random(20000, 99999),
                                        type: type,
                                        alias: 'alias',
                                        group: 'default',
                                        order: order + 1,
                                        env: ndk.typeEnv[0],
                                        conn: ndk.templateConn[type]
                                    };

                                    ndk.gridOpsConnDatabase.api.applyTransaction({
                                        add: [newrow],
                                        addIndex: order
                                    });

                                    ndk.connSet(newrow);
                                }
                            })
                        })

                        //示例连接
                        var demodbs = [];
                        ndk.demoConn.forEach(dc => {
                            demodbs.push({
                                name: dc.alias,
                                icon: ndk.iconDB(dc.type),
                                action: function () {
                                    var rows = ndk.getAllRows(ndk.gridOpsConnDatabase, true);
                                    if (rows.filter(x => x.id == dc.id).length) {
                                        var rowNode = ndk.gridOpsConnDatabase.api.getRowNode(dc.id);
                                        ndk.gridOpsConnDatabase.api.ensureIndexVisible(rowNode.rowIndex); //滚动到行显示
                                        ndk.gridOpsConnDatabase.api.flashCells({ rowNodes: [rowNode] }); //闪烁行
                                    } else {
                                        var newrow = dc;
                                        newrow.order = rows.length + 1;

                                        ndk.gridOpsConnDatabase.api.applyTransaction({
                                            add: [newrow],
                                            addIndex: rows.length
                                        });

                                        ndk.connSet(newrow);
                                    }
                                }
                            })
                        })

                        var result = [
                            {
                                name: '创建连接', icon: ndk.icons.connConn,
                                subMenu: adddbs
                            },
                            {
                                name: '示例连接',
                                subMenu: demodbs
                            },
                            {
                                name: '导出', icon: ndk.iconGrid('save'),
                                subMenu: [
                                    'csvExport',
                                    'excelExport',
                                    'separator',
                                    {
                                        name: "导出 JSON", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Markdown", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Markdown（Copy）", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Custom", icon: ndk.iconGrid('save'),
                                    }
                                ]
                            },
                            'separator',
                            {
                                name: '刷新', icon: ndk.icons.loading,
                                action: function () {
                                    ndk.loadConnDatabase()
                                }
                            },
                            'autoSizeAll',
                            'resetColumns',
                            'separator',
                            'copy',
                            'copyWithHeaders'
                        ];

                        return result;
                    },
                    enableBrowserTooltips: true, //title 提示
                    rowSelection: 'multiple', //多选
                    rowDragManaged: true, //拖拽
                    enableMultiRowDragging: true,//多行拖住
                    onRowDragEnd: function (event) {
                        //更新排序
                        var uprow = [], oi = 1;
                        event.api.forEachNode(node => {
                            var data = node.data;
                            data.order = oi++;
                            uprow.push(data)
                        })

                        event.api.applyTransaction({
                            update: uprow
                        });

                        ndk.connSet(uprow)
                    },
                    enableRangeSelection: true, //范围选择
                    pagination: false, //启用分页
                    paginationPageSize: 100, //显示行数
                    cacheBlockSize: 100, //请求行数
                    suppressMoveWhenRowDragging: true,
                    animateRows: true, //动画
                    rowData: conns, //数据源
                };

                if (ndk.gridOpsConnDatabase && ndk.gridOpsConnDatabase.api) {
                    ndk.gridOpsConnDatabase.api.destroy()
                } else {
                    ndk.size();
                }
                ndk.themeGrid(ndk.theme);
                ndk.gridOpsConnDatabase = new agGrid.Grid(ndk.domGridConnDatabase, gopscd).gridOptions;

                resolve()
            })
        })
    },

    /**
     * 通用请求
     * @param {any} url
     * @param {any} options
     */
    request: function (url, options) {
        return new Promise((resolve, reject) => {
            var ops = {
                method: "GET"
            }
            if (options != null) {
                for (var i in options) {
                    ops[i] = options[i]
                }
            }

            ndk.loading(true);
            fetch(url, ops).then(x => x.json()).then(res => {
                console.log(res);
                ndk.loading(false);
                if (res.code == 200) {
                    resolve(res)
                } else {
                    ndk.msg(res.msg);
                    reject(res)
                }
            }).catch(err => {
                ndk.msg(err);
                ndk.loading(false);
                reject(err)
            })
        })
    },

    /**
     * 参数拼接
     * @param {any} pars
     */
    parameterJoin: function (pars) {
        var arr = [];
        for (var i in pars) {
            arr.push(`${i}=${encodeURIComponent(pars[i])}`);
        }
        return arr.join('&');
    },

    /**
     * 加载状态
     * @param {any} show 
     */
    loading: function (show) {
        if (show) {
            ndk.domStatus.innerHTML = '';
            ndk.domStatus.classList.add('me-1')
            ndk.domStatus.classList.add('spinner-border')
            ndk.domStatus.classList.add('spinner-border-sm')
        } else {
            ndk.domStatus.innerHTML = '☁';
            ndk.domStatus.classList.remove('me-1')
            ndk.domStatus.classList.remove('spinner-border')
            ndk.domStatus.classList.remove('spinner-border-sm')
        }
    },

    /**
     * 获取表格所有数据
     * @param {any} gridOps
     * @param {any} isLeaf
     */
    getAllRows: function (gridOps, isLeaf) {
        let rowData = [];
        if (isLeaf) {
            gridOps.api.forEachLeafNode(node => rowData.push(node.data));
        } else {
            gridOps.api.forEachNode(node => rowData.push(node.data));
        }
        return rowData;
    },

    /**
     * 设置表格加载
     * @param {any} gridOps
     * @param {any} isHide 1隐藏 2隐藏无数据 0加载中
     */
    setGridLoading: function (gridOps, isHide) {
        if (gridOps && gridOps.api) {
            switch (isHide) {
                case 1:
                    gridOps.api.hideOverlay()
                    break;
                case 2:
                    gridOps.api.hideOverlay()
                    gridOps.api.showNoRowsOverlay()
                    break;
                default:
                    gridOps.api.showLoadingOverlay()
            }
        }
    },

    /**
     * 加载库
     * @param {any} cobj
     */
    loadDatabase: function (cobj) {
        return new Promise((resolve, reject) => {
            var pars = ndk.parameterJoin({ tdb: cobj.type, conn: cobj.conn });
            ndk.request(`${ndk.apiServer}${ndk.apiGetDatabase}?${pars}`).then(res => {
                var dbs = res.data;
                dbs.sort((a, b) => {
                    return -1 * a.DatabaseClassify.localeCompare(b.DatabaseClassify) || a.DatabaseName.localeCompare(b.DatabaseName);
                })
                resolve(dbs)
            }).catch(err => reject(err))
        })
    },

    /**
     * 加载表
     * @param {any} connDatabase
     */
    loadTable: function (connDatabase) {
        return new Promise((resolve, reject) => {
            if (connDatabase == null) {
                reject()
            } else {
                var pars = ndk.parameterJoin({ tdb: connDatabase.type, conn: connDatabase.conn, DatabaseName: connDatabase.databaseName });
                ndk.request(`${ndk.apiServer}${ndk.apiGetTable}?${pars}`).then(res => {
                    var tables = res.data;

                    var isSQLite = ndk.apiConnDatabase.type == "SQLite";
                    var opsTable = {
                        localeText: agg.localeText, //语言
                        //默认列属性配置
                        defaultColDef: {
                            width: 140, maxWidth: 2000, filter: true, sortable: true, resizable: true,
                            menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
                        },
                        //列
                        columnDefs: [
                            { headerName: ndk.icons.id, valueGetter: "node.rowIndex + 1", width: 100, checkboxSelection: true, headerCheckboxSelection: true, sortable: false, filter: false, menuTabs: false },
                            {
                                field: 'TableName', tooltipField: "TableName", headerName: ndk.icons.name + "表名", width: 220,
                                cellRenderer: params => {
                                    if (ndk.apiConnDatabase.type == "SQLServer") {
                                        return `${params.data.TableSchema}.${params.value}`
                                    }
                                    return params.value
                                }
                            },
                            { field: 'TableComment', tooltipField: "TableComment", headerName: ndk.icons.comment + "表注释", width: 290, hide: isSQLite, editable: !isSQLite, cellEditor: 'agLargeTextCellEditor', },
                            { field: 'TableRows', headerName: "行数" },
                            {
                                field: 'TableDataLength', headerName: "数据大小", width: 140,
                                cellRenderer: params => ndk.formatByteSize(params.value)
                            },
                            {
                                field: 'TableIndexLength', headerName: "索引大小", width: 140,
                                cellRenderer: params => ndk.formatByteSize(params.value)
                            },
                            { field: 'TableCollation', headerName: "字符集", width: 160, },
                            { field: 'TableCreateTime', headerName: "创建时间", width: 190, },
                            { field: 'TableModifyTime', headerName: "修改时间", width: 190, },
                            { field: 'TableSchema', headerName: "Schema" },
                            { field: 'TableType', headerName: "分类" },
                        ],
                        enableBrowserTooltips: true, //title 提示
                        rowSelection: 'multiple', //多选
                        onSelectionChanged: function () {
                            ndk.stepSave();
                        },
                        //双击表
                        onCellDoubleClicked: function (event) {
                            //非表注释打开列
                            if (event.column.colId != "TableComment") {
                                ndk.actionRun('load-column')
                            }
                        },
                        // 单元格变动
                        onCellValueChanged: function (params) {
                            //修改表注释
                            ndk.setTableComment(ndk.apiConnDatabase, params.data.TableName, params.value);
                        },
                        //表菜单项
                        getContextMenuItems: (event) => {
                            var edata = event.node ? event.node.data : null;
                            var result = [
                                {
                                    name: '表设计', disabled: event.node == null, icon: ndk.iconGrid('columns'),
                                    action: function () {
                                        if (!event.node.isSelected()) {
                                            event.node.setSelected(true);
                                        }
                                        ndk.actionRun('load-column')
                                    }
                                },
                                {
                                    name: '表数据', icon: ndk.iconGrid('grip'), disabled: event.node == null, tooltip: "查询表数据",
                                    action: function () {
                                        event.api.deselectAll();
                                        event.node.setSelected(true);

                                        //构建查询SQL并执行
                                        ndk.tabOpen('executesql').then(() => {
                                            Promise.all([ndk.editorSetSql("-- Please wait ..."), ndk.requestColumn(ndk.apiConnDatabase, edata.TableName)]).then(allres => {
                                                var columns = allres[1];
                                                var sql = ndk.buildSqlSelect(ndk.apiConnDatabase, edata, columns)
                                                ndk.editorSetSql(sql).then(() => {
                                                    ndk.editorFormatter(ndk.meSql); //格式化
                                                    ndk.domBtnExecuteSql.click()
                                                })
                                            })
                                        })
                                    }
                                },
                                {
                                    name: '生成 SQL', icon: ndk.icons.generate, disabled: event.node == null,
                                    subMenu: [
                                        {
                                            name: `SELECT`, icon: ndk.iconGrid('paste'), action: function () {

                                            }
                                        },
                                        {
                                            name: `INSERT`, icon: ndk.iconGrid('paste'), action: function () {

                                            }
                                        },
                                        {
                                            name: `UPDATE`, icon: ndk.iconGrid('paste'), action: function () {

                                            }
                                        },
                                        {
                                            name: `DELETE`, icon: ndk.iconGrid('paste'), action: function () {

                                            }
                                        },
                                        {
                                            name: `TRUNCATE`, icon: ndk.iconGrid('paste'), action: function () {

                                            }
                                        },
                                        {
                                            name: `DROP`, icon: ndk.iconGrid('paste'), action: function () {

                                            }
                                        },
                                        'separator',
                                        {
                                            name: `结构 DDL`, icon: ndk.iconGrid('paste'), action: function () {

                                            }
                                        },
                                        {
                                            name: `结构和数据`, icon: ndk.iconGrid('paste'), action: function () {

                                            }
                                        },
                                    ]
                                },
                                {
                                    name: '导出', icon: ndk.iconGrid('desc'),
                                    subMenu: [
                                        'csvExport',
                                        'excelExport',
                                        'separator',
                                        {
                                            name: "导出 JSON", icon: ndk.iconGrid('save'),
                                        },
                                        {
                                            name: "导出 Markdown", icon: ndk.iconGrid('save'),
                                        },
                                        {
                                            name: "导出 Markdown（Copy）", icon: ndk.iconGrid('save'),
                                        },
                                        {
                                            name: "导出 Custom", icon: ndk.iconGrid('save'),
                                        }
                                    ]
                                },
                                {
                                    name: '导入', icon: ndk.iconGrid('asc'),
                                },
                                'separator',
                                {
                                    name: '刷新', icon: ndk.icons.loading,
                                    action: function () {
                                        ndk.actionRun('load-table')
                                    }
                                },
                                {
                                    name: '全屏切换', icon: ndk.iconGrid('maximize'),
                                    action: function () {
                                        ndk.actionRun('max-box1')
                                    }
                                },
                                'autoSizeAll',
                                'resetColumns',
                                'separator',
                                'copy',
                                'copyWithHeaders'
                            ];

                            return result;
                        },
                        enableRangeSelection: true, //范围选择
                        pagination: false, //启用分页
                        paginationPageSize: 100, //显示行数
                        cacheBlockSize: 100, //请求行数
                        animateRows: true, //动画
                        rowData: tables, //数据源
                        onGridReady: function () {
                            ndk.size()
                        }
                    };

                    if (ndk.gridOpsTable && ndk.gridOpsTable.api) {
                        ndk.gridOpsTable.api.destroy()
                    } else {
                        ndk.size();
                    }
                    ndk.themeGrid(ndk.theme);
                    ndk.gridOpsTable = new agGrid.Grid(ndk.domGridTable, opsTable).gridOptions;

                    ndk.stepSave();
                    resolve(tables)
                }).catch(err => reject(err))
            }
        })
    },

    /**
     * 设置表注释
     * @param {any} connDatabase
     * @param {any} tableName
     * @param {any} tableComment
     */
    setTableComment: function (connDatabase, tableName, tableComment) {
        return new Promise((resolve, reject) => {
            if (connDatabase == null) {
                reject()
            } else {

                var fd = new FormData();
                fd.append('tdb', connDatabase.type);
                fd.append('conn', connDatabase.conn);
                fd.append('TableName', tableName);
                fd.append('TableComment', tableComment);
                fd.append('DatabaseName', connDatabase.databaseName);

                ndk.request(`${ndk.apiServer}${ndk.apiSetTableComment}`, {
                    method: "POST",
                    body: fd
                }).then(res => resolve(res)).catch(err => reject(err))
            }
        })
    },

    /**
     * 请求列
     * @param {any} connDatabase
     * @param {any} filterTableName
     */
    requestColumn: function (connDatabase, filterTableName) {
        return new Promise((resolve, reject) => {
            if (connDatabase == null) {
                reject()
            } else {

                var fd = new FormData();
                fd.append('tdb', connDatabase.type);
                fd.append('conn', connDatabase.conn);
                fd.append('filterTableName', filterTableName);
                fd.append('DatabaseName', connDatabase.databaseName);

                ndk.request(`${ndk.apiServer}${ndk.apiGetColumn}`, {
                    method: "POST",
                    body: fd
                }).then(res => {
                    var columns = res.data;
                    resolve(columns)
                }).catch(err => reject(err))
            }
        })
    },

    /**
     * 加载列
     * @param {any} connDatabase
     * @param {any} filterTableName
     */
    loadColumn: function (connDatabase, filterTableName) {
        return new Promise((resolve, reject) => {
            ndk.requestColumn(connDatabase, filterTableName).then(columns => {
                var isSQLite = ndk.apiConnDatabase.type == "SQLite";
                var opsColumn = {
                    localeText: agg.localeText, //语言
                    //默认列属性配置
                    defaultColDef: {
                        width: 110, maxWidth: 2000, filter: true, sortable: true, resizable: true,
                        menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
                    },
                    //列
                    columnDefs: [
                        { field: 'TableName', headerName: "表名", rowGroup: true, width: 200, hide: true, },
                        { field: 'TableComment', headerName: "表注释", width: 300, hide: true },
                        { field: 'ColumnName', headerName: "字段", width: 200, hide: true, },
                        { field: 'ColumnType', headerName: "类型及长度", width: 150, hide: true },
                        { field: 'DataType', headerName: "类型", width: 120, hide: true },
                        { field: 'DataLength', headerName: "长度", hide: true },
                        { field: 'DataScale', headerName: "精度", hide: true },
                        { field: 'ColumnOrder', headerName: "列序" },
                        {
                            field: 'PrimaryKey', headerName: "主键", hide: true,
                            cellRenderer: params => params.value > 0 ? ndk.icons.key + params.value : ""
                        },
                        { field: 'AutoIncr', headerName: "自增", cellRenderer: params => params.value == 1 ? ndk.icons.incr : "" },
                        {
                            field: 'IsNullable', headerName: "必填", hide: true,
                            cellRenderer: params => params.value == 1 ? "" : ndk.icons.edit
                        },
                        { field: 'ColumnDefault', headerName: "默认值", width: 120, },
                        { field: 'ColumnComment', tooltipField: "ColumnComment", headerName: ndk.icons.comment + "列注释", width: 330, hide: isSQLite, editable: !isSQLite, cellEditor: 'agLargeTextCellEditor', },
                    ],
                    enableBrowserTooltips: true, //title 提示
                    groupSelectsChildren: true, //选择组及子节点
                    autoGroupColumnDef: {
                        field: "ColumnName", headerName: "表列", width: 560,
                        headerCheckboxSelection: true, //全选组
                        headerCheckboxSelectionFilteredOnly: true,
                        filterValueGetter: (params) => params.data.TableName, //组过滤
                        cellRendererParams: {
                            checkbox: true,
                            suppressCount: true,
                            suppressEnterExpand: true,
                            suppressDoubleClickExpand: true,
                            innerRenderer: function (item) {
                                if (item.node.group) {
                                    var grow = ndk.gridOpsColumn.rowData.filter(x => x[item.node.field] == item.value);
                                    return `<b>${item.value} (${grow.length}) </b> <span class="badge bg-secondary mx-2" role="button">数据</span> <span title="${grow[0].TableComment || ""}">${grow[0].TableComment || ""}</span>`
                                } else {
                                    var colattr = [];
                                    if (item.data.PrimaryKey > 0) {
                                        colattr.push(ndk.icons.key + item.data.PrimaryKey);
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
                    },
                    rowSelection: 'multiple', //多选
                    onSelectionChanged: function () {
                        ndk.stepSave();
                    },
                    // 单元格变动
                    onCellValueChanged: function (params) {
                        //修改列注释
                        ndk.setColumnComment(ndk.apiConnDatabase, params.data.TableName, params.data.ColumnName, params.value);
                    },
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
                                tooltip:
                                    'Very long tooltip, did I mention that I am very long, well I am! Long!  Very Long!',
                            },
                            {
                                name: '导出', icon: ndk.iconGrid('save'),
                                subMenu: [
                                    'csvExport',
                                    'excelExport',
                                    'separator',
                                    {
                                        name: "导出 JSON", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Markdown", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Markdown（Copy）", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Custom", icon: ndk.iconGrid('save'),
                                    },
                                ]
                            },
                            'expandAll',
                            'contractAll',
                            'separator',
                            {
                                name: '刷新', icon: ndk.icons.loading,
                                action: function () {
                                    ndk.actionRun('load-column')
                                }
                            },
                            {
                                name: '全屏切换', icon: ndk.iconGrid('maximize'),
                                action: function () {
                                    ndk.actionRun('max-box2')
                                }
                            },
                            'autoSizeAll',
                            'resetColumns',
                            'separator',
                            'copy',
                            'copyWithHeaders'
                        ];

                        return result;
                    },
                    enableRangeSelection: true, //范围选择
                    pagination: false, //启用分页
                    paginationPageSize: 100, //显示行数
                    cacheBlockSize: 100, //请求行数
                    animateRows: true, //动画
                    rowData: columns, //数据源
                    onGridReady: function () {
                        ndk.size()
                    }
                };

                if (ndk.gridOpsColumn && ndk.gridOpsColumn.api) {
                    ndk.gridOpsColumn.api.destroy()
                }

                ndk.tabOpen('column');
                ndk.themeGrid(ndk.theme);
                ndk.gridOpsColumn = new agGrid.Grid(ndk.domGridColumn, opsColumn).gridOptions;

                //展开
                setTimeout(() => {
                    ndk.gridOpsColumn.api.forEachNode((node) => {
                        if (node.level == 0) {
                            node.setExpanded(true);
                        }
                    });
                }, 300)

                ndk.stepSave();
                resolve(columns)
            }).catch(err => reject(err))
        })
    },

    /**
     * 设置列注释
     * @param {any} connDatabase
     * @param {any} tableName
     * @param {any} columnName
     * @param {any} columnComment
     */
    setColumnComment: function (connDatabase, tableName, columnName, columnComment) {
        return new Promise((resolve, reject) => {
            if (connDatabase == null) {
                reject()
            } else {

                var fd = new FormData();
                fd.append('tdb', connDatabase.type);
                fd.append('conn', connDatabase.conn);
                fd.append('TableName', tableName);
                fd.append('ColumnName', columnName);
                fd.append('ColumnComment', columnComment);
                fd.append('DatabaseName', connDatabase.databaseName);

                ndk.request(`${ndk.apiServer}${ndk.apiSetColumnComment}`, {
                    method: "POST",
                    body: fd
                }).then(res => resolve(res)).catch(err => reject(err))
            }
        })
    },

    /**
     * 执行SQL
     * @param {any} connDatabase
     * @param {any} sql
     */
    loadExecuteSql: function (connDatabase, sql) {
        return new Promise((resolve, reject) => {
            var fd = new FormData();
            fd.append('tdb', connDatabase.type);
            fd.append('conn', connDatabase.conn);
            fd.append('sql', sql);
            fd.append('DatabaseName', connDatabase.databaseName);

            ndk.request(ndk.apiExecuteSql, {
                method: "POST",
                body: fd
            }).then(res => {
                var esdata = res.data, esrows = [], columnDefs = [
                    { headerName: ndk.icons.id, valueGetter: "node.rowIndex + 1", width: 100, checkboxSelection: true, headerCheckboxSelection: true, sortable: false, filter: false, menuTabs: false },
                ];

                //填充数据
                if (esdata.Item1) {
                    for (var tname in esdata.Item1) {
                        var dt = esdata.Item1[tname];
                        esrows = dt;
                        break;
                    }
                }

                //填充列头
                if (esrows.length > 0) {
                    var esrow = esrows[0];
                    for (var field in esrow) {
                        columnDefs.push({ field: field })
                    }
                }

                var opses = {
                    localeText: agg.localeText, //语言
                    //默认列属性配置
                    defaultColDef: {
                        width: 320, maxWidth: 2000, filter: true, sortable: true, resizable: true,
                        menuTabs: ['generalMenuTab', 'filterMenuTab', 'columnsMenuTab']
                    },
                    //列
                    columnDefs: columnDefs,
                    enableBrowserTooltips: true, //title 提示
                    rowSelection: 'multiple', //多选
                    //双击单元格
                    onCellDoubleClicked: function (event) {

                    },
                    //表菜单项
                    getContextMenuItems: (event) => {
                        var result = [
                            'copy',
                            'copyWithHeaders',
                            {
                                name: '导出', icon: ndk.iconGrid('save'),
                                subMenu: [
                                    'csvExport',
                                    'excelExport',
                                    'separator',
                                    {
                                        name: "导出 JSON", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Markdown", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Markdown（Copy）", icon: ndk.iconGrid('save'),
                                    },
                                    {
                                        name: "导出 Custom", icon: ndk.iconGrid('save'),
                                    }
                                ]
                            },
                            {
                                name: '生成 SQL', icon: ndk.icons.generate,
                                subMenu: [
                                    {
                                        name: `SELECT`, icon: ndk.iconGrid('paste'), action: function () {

                                        }
                                    },
                                    {
                                        name: `INSERT`, icon: ndk.iconGrid('paste'), action: function () {

                                        }
                                    },
                                    {
                                        name: `UPDATE`, icon: ndk.iconGrid('paste'), action: function () {

                                        }
                                    },
                                    {
                                        name: `DELETE`, icon: ndk.iconGrid('paste'), action: function () {

                                        }
                                    },
                                    {
                                        name: `TRUNCATE`, icon: ndk.iconGrid('paste'), action: function () {

                                        }
                                    },
                                    {
                                        name: `DROP`, icon: ndk.iconGrid('paste'), action: function () {

                                        }
                                    },
                                    'separator',
                                    {
                                        name: `结构 DDL`, icon: ndk.iconGrid('paste'), action: function () {

                                        }
                                    },
                                    {
                                        name: `结构和数据`, icon: ndk.iconGrid('paste'), action: function () {

                                        }
                                    },
                                ]
                            },
                            'separator',
                            {
                                name: '刷新', icon: ndk.icons.loading,
                                action: function () {
                                    ndk.actionRun('load-table')
                                }
                            },
                            {
                                name: '全屏切换', icon: ndk.iconGrid('maximize'),
                                action: function () {
                                    ndk.actionRun('max-box2')
                                }
                            },
                            'autoSizeAll',
                            'resetColumns',
                            'separator',
                            'chartRange'
                        ];

                        return result;
                    },
                    enableRangeSelection: true, //范围选择
                    pagination: false, //启用分页
                    paginationPageSize: 100, //显示行数
                    cacheBlockSize: 100, //请求行数
                    animateRows: true, //动画
                    rowData: esrows, //数据源
                    onGridReady: function (event) {
                        event.columnApi.autoSizeAllColumns(); //列宽自动大小
                        ndk.size()
                    }
                };

                ndk.domChkesGrid.click(); //切换到表格
                if (ndk.gridOpsExecuteSql && ndk.gridOpsExecuteSql.api) {
                    ndk.gridOpsExecuteSql.api.destroy()
                } else {
                    ndk.size();
                }
                ndk.themeGrid(ndk.theme);
                ndk.gridOpsExecuteSql = new agGrid.Grid(ndk.domGridExecuteSql, opses).gridOptions;

                resolve(esdata)
            }).catch(err => reject(err))
        })
    },

    /**
     * 编辑器SQL
     * @param {any} sql
     */
    editorSetSql: function (sql) {
        return new Promise((resolve) => {
            if (ndk.meSql) {
                if (sql != null) {
                    ndk.meSql.setValue(sql)
                }
                resolve()
            } else {
                ndk.editorReady().then(() => {
                    ndk.meSql = monaco.editor.create(ndk.domSqlExecuteSql, {
                        value: sql || "",
                        fontSize: 18,
                        automaticLayout: true,
                        scrollbar: {
                            verticalScrollbarSize: 13, horizontalScrollbarSize: 13
                        },
                        minimap: { enabled: true },
                        language: "sql"
                    });

                    ndk.meSql.addCommand(monaco.KeyCode.PauseBreak, function () {
                        ndk.domBtnExecuteSql.click()
                    })

                    resolve()
                })
            }
        })
    },

    /** monaco-editor */
    editorReady: function () {
        return new Promise(resolve => {
            if (window.monaco) {
                resolve()
            } else {
                require(['vs/editor/editor.main'], function () {

                    //格式化
                    monaco.languages.registerDocumentFormattingEditProvider('sql', {
                        provideDocumentFormattingEdits: function (model, _options, _token) {
                            return [{
                                text: ndk.formatterSQL(model.getValue(), ndk.apiConnDatabase.type),
                                range: model.getFullModelRange()
                            }];
                        }
                    });

                    ndk.themeEditor(ndk.theme)
                    resolve()
                })
            }
        })
    },

    /**
     * monaco-editor 格式化
     * @param {any} editor
     */
    editorFormatter: editor => editor.trigger('a', 'editor.action.formatDocument'),

    /**
     * 
     * @param {any} text
     * @param {any} type
     */
    formatterSQL: function (text, type) {
        var sqlang;
        switch (type) {
            case "SQLite": sqlang = 'sql'; break;
            case "MySQL":
            case "MariaDB":
            case "PostgreSQL":
                sqlang = type.toLowerCase();
                break;
            case "SQLServer": sqlang = 'tsql'; break;
            case "Oracle": sqlang = 'plsql'; break;
            default: sqlang = 'tsql'; break;
        }

        return sqlFormatter.format(text, {
            language: sqlang,
            uppercase: true
        });
    },

    /**
     * 构建SQL引用符号
     * @param {any} type
     * @param {any} key
     */
    buildSqlQuote: function (type, key) {
        switch (type) {
            case "SQLite":
            case "SQLServer":
                return `[${key}]`
            case "MySQL":
            case "MariaDB":
                return '`' + key + '`'
            case "Oracle":
            case "PostgreSQL":
                return `"${key}"`
            default:
                return key
        }
    },

    /**
     * 构建SQL完整表名
     * @param {any} connDatabase
     * @param {any} table
     */
    buildSqlFullTableName: function (connDatabase, table) {
        var ftn = [ndk.buildSqlQuote(connDatabase.type, connDatabase.databaseName)];
        if (table.TableSchema != null && table.TableSchema != "") {
            ftn.push(ndk.buildSqlQuote(connDatabase.type, table.TableSchema))
        }
        ftn.push(ndk.buildSqlQuote(connDatabase.type, table.TableName))

        return ftn.join('.')
    },

    /**
     * 构建SQL select
     * @param {any} connDatabase
     * @param {any} table
     * @param {any} columns
     */
    buildSqlSelect: function (connDatabase, table, columns) {
        var table = ndk.buildSqlFullTableName(connDatabase, table), cols = [];
        columns.forEach(column => cols.push(ndk.buildSqlQuote(connDatabase.type, column.ColumnName)));

        return `SELECT ${cols.join(',')} FROM ${table}`
    },

    /**
     * 获取 icon
     * @param {any} icon
     */
    iconGrid: icon => `<span class="ag-icon ag-icon-${icon}"></span>`,

    /**
     * 设置主题
     * @param {any} theme
     */
    themeGrid: function (theme) {
        ['ag-theme-balham', 'ag-theme-balham-dark'].forEach(item => {
            ndk.domGridTable.classList.remove(item);
            ndk.domGridColumn.classList.remove(item);
            ndk.domGridConnDatabase.classList.remove(item);
            ndk.domGridExecuteSql.classList.remove(item)
        })
        switch (theme) {
            case "dark":
                ndk.domGridTable.classList.add("ag-theme-balham-dark");
                ndk.domGridColumn.classList.add("ag-theme-balham-dark");
                ndk.domGridConnDatabase.classList.add("ag-theme-balham-dark");
                ndk.domGridExecuteSql.classList.add("ag-theme-balham-dark");
                break;
            default:
                ndk.domGridTable.classList.add("ag-theme-balham");
                ndk.domGridColumn.classList.add("ag-theme-balham");
                ndk.domGridConnDatabase.classList.add("ag-theme-balham");
                ndk.domGridExecuteSql.classList.add("ag-theme-balham");
        }
    },

    /**
     * 设置主题
     * @param {any} theme
     */
    themeEditor: function (theme) {
        if (window.monaco) {
            switch (theme) {
                case "dark":
                    monaco.editor.setTheme("vs-dark");
                    break;
                default:
                    monaco.editor.setTheme("vs");
            }
        }
    },

    /**
     * 打开标签
     * @param {any} name
     */
    tabOpen: function (name) {
        return new Promise(resolve => {
            ndk.domTabsTool.classList.add('d-none')
            var vtab = ndk.domTabsManager.children[0];
            vtab.innerHTML = "≡";
            for (var i = 0; i < ndk.domTabs.children.length; i++) {
                var tab = ndk.domTabs.children[i];
                if (tab.getAttribute("data-tab") == name) {
                    tab.classList.remove('d-none')
                    ndk.domTabsTool.classList.remove('d-none')
                    vtab.innerHTML = name;
                } else {
                    tab.classList.add('d-none')
                }
            }
            for (var i = 0; i < ndk.domTabsMenu.children.length; i++) {
                var tabmenu = ndk.domTabsMenu.children[i];
                if (tabmenu.getAttribute("data-tabmenu") == name) {
                    tabmenu.classList.remove('d-none')
                } else {
                    tabmenu.classList.add('d-none')
                }
            }
            if (name == "executesql") {
                ndk.domChkesSql.click()
                ndk.editorSetSql().then(() => {
                    resolve()
                })
            } else {
                ndk.size()
                resolve()
            }
        })
    },

    //自适应大小
    size: function () {
        var vh = document.documentElement.clientHeight;
        var gh1 = vh - ndk.domGridTable.getBoundingClientRect().top - 15;
        var gh2 = vh - ndk.domGridColumn.getBoundingClientRect().top - 15;
        var domes = ndk.domTabs.querySelector('[data-tab="executesql"]');
        var gh3 = vh - domes.getBoundingClientRect().top - 15;
        ndk.domGridTable.style.height = `${gh1}px`;
        ndk.domGridColumn.style.height = `${gh2}px`;
        domes.style.height = `${gh3}px`;
    },

    /**
     * 格式化字节大小
     * @param {any} size
     * @param {any} keep 默认保留2位
     * @param {any} rate 默认1024换算
     * @returns 
     */
    formatByteSize: function (size, keep, rate) {
        keep = keep || 2;
        rate = rate || 1024;

        if (Math.abs(size) < rate) {
            return size + ' B';
        }

        const units = rate == 1000 ? ['KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'] : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
        let u = -1;
        const r = 10 ** keep;

        do {
            size /= rate;
            ++u;
        } while (Math.round(Math.abs(size) * r) / r >= rate && u < units.length - 1);

        return size.toFixed(keep) + ' ' + units[u];
    },

    /**
     * 判断类型
     * @param {any} obj
     */
    type: function (obj) {
        var tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },

    /**
     * 分组
     * @param {any} array 
     * @param {any} f 
     * @returns 
     */
    groupBy: function (array, f) {
        var groups = {};
        array.forEach(o => {
            var key = f(o);
            groups[key] = groups[key] || [];
            groups[key].push(o);
        });
        return Object.keys(groups);
    },

    /**
     * 消息提示
     * @param msg 消息
     */
    msg: function (msg) {

        var mmc = ndk.domMsgModal,
            ms = 'modal-sm',
            txt = msg,
            tlen = 0;
        for (var i = 0; i < txt.length; i++) {
            if (/^[\u4e00-\u9fa5]$/.test(txt[i])) {
                tlen++;
            }
        }
        tlen += txt.length;

        if (tlen > 30) {
            ms = 'modal-sm'
        }
        if (tlen > 100) {
            ms = 'modal-lg'
        }
        if (tlen > 200) {
            ms = 'modal-xl'
        }

        var mdialog = mmc.children[0];
        mdialog.classList.remove("modal-sm");
        mdialog.classList.remove("modal-lg");
        mdialog.classList.remove("modal-xl");

        mdialog.classList.add(ms);
        mmc.querySelector('.modal-body').innerHTML = msg;

        var bmodal = new bootstrap.Modal(mmc);
        bmodal.show();

        return bmodal;
    },
}

ndk.init();