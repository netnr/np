//低版本跳转
try { eval("() => 1") } catch (e) { top.location = "https://ub.netnr.eu.org" }

/*
    upstream: From nginx upstream
    Source must support cross-domain
 */

(function (window) {
    var ups = function (hosts, callback, timeout) {
        //全局对象、默认请求超时、默认源过期
        var gk = "upstreamCache", dto = 3000, es = 1000 * 300;
        if (!(gk in window)) {
            try {
                window[gk] = JSON.parse(localStorage.getItem(gk)) || {};
            } catch (e) {
                window[gk] = {}
            }
        }

        var startTime = new Date().valueOf(),
            cacheKey = hosts.join(','),
            hostsCache = window[gk][cacheKey];

        if (hostsCache && startTime - hostsCache.date < es) {
            callback(hostsCache.ok[0], hostsCache.ok);
        } else {
            var ok = [], bad = 0, i = 0, len = hosts.length;
            for (; i < len;) {
                var host = hosts[i++];
                //自动补齐链接
                host = host.trim().toLowerCase().indexOf("//") >= 0 ? host : "//" + host;
                //发起fetch，添加成功的url（该url与hosts可能不一样），须支持跨域请求
                fetch(host).then(function (res) {
                    res.ok ? ok.push(res.url) : bad++;
                }).catch(function () { bad++ })
            }
            var si = setInterval(function () {
                var isc = false, now = new Date().valueOf();
                //当timeout为1，返回最快可用的host
                if (timeout == 1 && ok.length > 0) {
                    isc = true;
                }
                //所有请求结束 或 超时，返回结果
                var istimeout = now - startTime > ((timeout == 1 || !timeout) ? dto : timeout);
                if (ok.length + bad == len || istimeout) {
                    isc = true;
                }
                if (isc) {
                    clearInterval(si);
                    window[gk][cacheKey] = { date: now, ok: ok };
                    localStorage.setItem(gk, JSON.stringify(window[gk]));
                    callback(ok[0], ok);
                }
            }, 1)
        }
    }

    window.upstream = ups;

    return ups;
})(window);

/**导航 */
let bsnav = {
    mask: function (yesno) {
        if (yesno == true) {
            document.querySelector('.bsnav-mask').classList.add('active');
        }
        else if (yesno == false) {
            document.querySelector('.bsnav-mask').classList.remove('active');
        }
    },
    offcanvas: function (yesno) {
        bsnav.mask(yesno);

        if (yesno == true) {
            document.querySelector('.mobile-offcanvas').classList.add('show');
            document.body.classList.add('offcanvas-active');
        }
        else if (yesno == false) {
            document.querySelector('.mobile-offcanvas.show').classList.remove('show');
            document.body.classList.remove('offcanvas-active');
        }
    },
    init: function () {

        document.body.addEventListener('click', function (e) {
            var target = e.target;

            //close
            if (target.classList.contains("btn-close") && target.parentNode.classList.contains("bsnav-header")) {
                e.preventDefault();
                bsnav.offcanvas(false);
            }
            if (target.classList.contains("bsnav-mask")) {
                bsnav.offcanvas(false);
            }

            //open
            if (target.classList.contains("bsnav-menu-toggle") || target.parentNode.classList.contains("bsnav-menu-toggle")) {
                bsnav.offcanvas(!document.querySelector('.mobile-offcanvas').classList.contains('show'));
            }
        });
    }
}

bsnav.init();

let bs = {
    obj: {},
    alert: function (content) {
        if (bs.obj.alert) {
            bs.obj.alert.dispose();
        }

        var dom = document.createElement("div");
        dom.className = "modal";
        dom.innerHTML = `
          <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-sm">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">Message</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div class="modal-body">
                ${content}
              </div>
              <div class="modal-footer justify-content-center">
                <button type="button" class="btn btn-warning" data-bs-dismiss="modal">确定</button>
              </div>
            </div>
          </div>
        `;

        document.body.appendChild(dom);
        bs.obj.alert = new bootstrap.Modal(dom);
        bs.obj.alert.show();
    },
    msg: function (content) {
        if (!bs.obj.msgbox) {
            bs.obj.msgbox = document.createElement("div");
            bs.obj.msgbox.className = "toast-container position-fixed top-50 start-50 translate-middle p-3";
            bs.obj.msgbox.style.zIndex = 9;
            document.body.appendChild(bs.obj.msgbox);
        }

        var dom = document.createElement("div");
        dom.innerHTML = `<div class="toast" data-bs-autohide="true">
            <div class="toast-header">
                <img src="/favicon.ico" class="rounded me-2" alt="icon" style="height:18px">
                <strong class="me-auto pt-1">Message</strong>
                <small class="text-muted">${new Date().toLocaleTimeString()}</small>
                <button type="button" class="btn-close me-1" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                ${content}
            </div>
        </div>`;

        var toastdom = dom.children[0];
        bs.obj.msgbox.appendChild(toastdom);
        var toast = new bootstrap.Toast(toastdom);
        toastdom.addEventListener('hidden.bs.toast', function () {
            this.toast.dispose();
            this.remove();
        })
        toastdom.toast = toast;
        toast.show();
    }
}

/* ScriptService */
var ss = {
    apiServer: "https://www.netnr.eu.org",
    meConfig: function (config) {
        var ops = {
            value: "",
            theme: "vs",
            fontSize: 18,
            automaticLayout: true,
            scrollbar: {
                verticalScrollbarSize: 13,
                horizontalScrollbarSize: 13
            },
            minimap: {
                enabled: true
            }
        }
        if (config) {
            for (var i in config) {
                ops[i] = config[i];
            }
        }
        return ops;
    },
    keepSetValue: function (me, text) {
        var cpos = me.getPosition();
        me.executeEdits('', [{
            range: me.getModel().getFullModelRange(),
            text: text
        }]);
        me.setSelection(new monaco.Range(0, 0, 0, 0));
        me.setPosition(cpos);
    },
    init: function () {

        //icon
        ss.loadPath("/images/icon.svg", "20210619").then(res => {
            $('body').append('<div class="d-none">' + res + '</div>');
        })

        ss.lsInit();

        $(function () {
            //Monaco Editor 编辑器全屏切换
            $('.me-full-btn').click(function () {
                var mebox = $(this).parent();
                if (mebox.hasClass('me-full')) {
                    mebox.removeClass('me-full')
                    mebox.addClass('position-relative')
                } else {
                    mebox.addClass('me-full')
                    mebox.removeClass('position-relative')
                }
            })
        })
    },

    /**
     * 从路径加载资源（优先取缓存）
     * @param {any} path 路径
     * @param {any} version 缓存
     */
    loadPath: function (path, version) {
        return new Promise(function (resolve) {
            var pv = localStorage.getItem(path), data;
            try {
                if (pv != null) {
                    var json = JSON.parse(pv);
                    if (json.version == version) {
                        data = json.data;
                    }
                }
            } catch (e) { }
            if (data == null) {
                fetch(path + "?" + version).then(x => x.text()).then(res => {
                    localStorage.setItem(path, JSON.stringify({ data: res, version }));
                    resolve(res);
                })
            } else {
                resolve(data);
            }
        })
    },

    bmobInit: function () {
        //比目初始化
        window.Bmob && Bmob.initialize("59a522843b951532546934352166df80", "97fcbeae1457621def948aba1db01821");
    },

    agg: {
        lk: function () {
            agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { };
        },
        defaultColDef: {
            sortable: true,
            resizable: true,
            filter: "agSetColumnFilter",
            menuTabs: ['filterMenuTab']
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
            export: '导出',
            csvExport: '导出 CSV',
            excelExport: '导出 Excel',
            excelXmlExport: '导出 XML',

            // Enterprise Menu Aggregation and Status Bar
            sum: '求和',
            min: '最小',
            max: '最大',
            none: 'None',
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
            pivotChartAndPivotMode: 'Pivot Chart & Pivot Mode',
            pivotChart: 'Pivot Chart',
            chartRange: 'Chart Range',

            columnChart: 'Column',
            groupedColumn: 'Grouped',
            stackedColumn: 'Stacked',
            normalizedColumn: '100% Stacked',

            barChart: 'Bar',
            groupedBar: 'Grouped',
            stackedBar: 'Stacked',
            normalizedBar: '100% Stacked',

            pieChart: 'Pie',
            pie: 'Pie',
            doughnut: 'Doughnut',

            line: 'Line',

            xyChart: 'X Y (Scatter)',
            scatter: 'Scatter',
            bubble: 'Bubble',

            areaChart: 'Area',
            area: 'Area',
            stackedArea: 'Stacked',
            normalizedArea: '100% Stacked',

            histogramChart: 'Histogram',

            // Charts
            pivotChartTitle: 'Pivot Chart',
            rangeChartTitle: 'Range Chart',
            settings: 'Settings',
            data: 'Data',
            format: 'Format',
            categories: 'Categories',
            defaultCategory: '(None)',
            series: 'Series',
            xyValues: 'X Y Values',
            paired: 'Paired Mode',
            axis: 'Axis',
            navigator: 'Navigator',
            color: 'Color',
            thickness: 'Thickness',
            xType: 'X Type',
            automatic: 'Automatic',
            category: 'Category',
            number: 'Number',
            time: 'Time',
            xRotation: 'X Rotation',
            yRotation: 'Y Rotation',
            ticks: 'Ticks',
            width: 'Width',
            height: 'Height',
            length: 'Length',
            padding: 'Padding',
            spacing: 'Spacing',
            chart: 'Chart',
            title: 'Title',
            titlePlaceholder: 'Chart title - double click to edit',
            background: 'Background',
            font: 'Font',
            top: 'Top',
            right: 'Right',
            bottom: 'Bottom',
            left: 'Left',
            labels: 'Labels',
            size: 'Size',
            minSize: 'Minimum Size',
            maxSize: 'Maximum Size',
            legend: 'Legend',
            position: 'Position',
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
            columnGroup: 'Column',
            barGroup: 'Bar',
            pieGroup: 'Pie',
            lineGroup: 'Line',
            scatterGroup: 'X Y (Scatter)',
            areaGroup: 'Area',
            histogramGroup: 'Histogram',
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
    },

    /**
     * 代理请求
     * @param {any} obj 请求参数
     * @param {any} hi 指定代理
     */
    ajax: function (obj, hi) {
        var hosts = ["https://cors.eu.org/", "https://seep.eu.org/", "https://www.netnr.eu.org/api/v1/Proxy?url="];
        if (hi != null) {
            obj.url = hosts[hi] + encodeURIComponent(obj.url);
            $.ajax(obj)
        } else {
            upstream(hosts, function (fast) {
                obj.url = fast + encodeURIComponent(obj.url);
                $.ajax(obj);
            }, 1);
        }
    },

    /**
     * 代理数据处理
     * @param {any} data
     */
    datalocation: function (data) {
        return data || {};
        ss.loading(0);
    },

    /**
     * html 编码
     * @param {any} html
     */
    htmlEncode: function (html) {
        return document.createElement('a').appendChild(document.createTextNode(html)).parentNode.innerHTML;
    },

    /**
     * html 解码
     * @param {any} html
     */
    htmlDecode: function (html) {
        var a = document.createElement('a');
        a.innerHTML = html;
        return a.innerText;
    },

    /**
     * 加载
     * @param {any} close
     */
    loading: function (close) {
        if (close === 0 || close === false) {
            clearTimeout(window.loadingdefer);
            window.loadingdom.hide();
        } else {
            if (!window.loadingdom) {
                window.loadingdom = $('<div class="loading"><svg><use xlink:href="#loading"></use></svg></div>').appendTo(document.body);
            }
            window.loadingdom.hide();
            window.loadingdefer = setTimeout(function () {
                window.loadingdom.show();
            }, 1000);
        }
    },

    /* localStorage */
    lsKey: location.pathname,
    ls: {},
    lsInit: function () {
        var lsv = localStorage.getItem(this.lsKey);
        if (lsv && (lsv = JSON.parse(lsv))) {
            this.ls = lsv;
        }
    },
    lsArr: function (key) {
        return this.ls[key] = this.ls[key] || [];
    },
    lsObj: function (key) {
        return this.ls[key] = this.ls[key] || {};
    },
    lsStr: function (key) {
        return this.ls[key] = this.ls[key] || "";
    },
    lsSave: function () {
        localStorage.setItem(this.lsKey, JSON.stringify(this.ls));
    },

    /**
     * 下载
     * @param {any} content
     * @param {any} fileName
     */
    dowload: function (content, fileName) {
        var aTag = document.createElement('a');
        aTag.download = fileName;
        if (content.nodeType == 1) {
            aTag.href = content.toDataURL();
        } else {
            var blob = new Blob([content]);
            aTag.href = URL.createObjectURL(blob);
        }
        document.body.appendChild(aTag);
        aTag.click();
        aTag.remove();
    },

    /**
     * 大小可视化
     * @param {any} size
     */
    sizeOf: function (size) {
        var u = 'B', s = size;
        if (size >= 1024) {
            u = 'K';
            s = size / 1024.0
        }
        if (size >= 1024 * 1024) {
            u = 'M'
            s = size / 1024.0 / 1024;
        }
        if (size >= 1024 * 1024 * 1024) {
            u = 'G'
            s = size / 1024.0 / 1024 / 1024;
        }
        return s.toFixed(1) + u;
    },

    /**
     * 接收文件
     * @param {any} fn 回调
     * @param {any} fileNode 选择文件的节点
     */
    receiveFiles: function (fn, fileNode) {

        //拖拽
        $(document).on("dragleave dragenter dragover", function (e) {
            if (e && e.stopPropagation) { e.stopPropagation() } else { window.event.cancelBubble = true }
            if (e && e.preventDefault) { e.preventDefault() } else { window.event.returnValue = false }
        }).on("drop", function (e) {
            if (e && e.preventDefault) { e.preventDefault() } else { window.event.returnValue = false }
            e = e || window.event;
            var files = (e.dataTransfer || e.originalEvent.dataTransfer).files;
            if (files && files.length) {
                fn(files, 'drag');
            }
        });

        //浏览
        $(fileNode).change(function () {
            var files = this.files;
            if (files.length) {
                fn(files, 'change');
            }
        });

        //粘贴
        document.addEventListener('paste', function (event) {
            if (event.clipboardData || event.originalEvent) {
                var clipboardData = (event.clipboardData || event.originalEvent.clipboardData);
                if (clipboardData.items) {
                    var items = clipboardData.items, len = items.length, files = [];
                    for (var i = 0; i < len; i++) {
                        var blob = items[i].getAsFile();
                        blob && files.push(blob);
                    }
                    if (files.length) {
                        fn(files, 'paste');
                    }
                }
            }
        })
    }
}

ss.init();