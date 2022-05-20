var agQuery = {
    ofSql: sql => {
        if (typeof sql === 'string') {
            return sql.replaceAll("'", "''");
        }
        return sql
    },
    typeDB: "SQLServer",
    table: "(TABLE)",
    columns: [],

    buildSql: (request) => {

        const selectSql = agQuery.createSelectSql(request);
        const fromSql = ` from ${agQuery.table} `;
        const whereSql = agQuery.createWhereSql(request);
        const limitSql = agQuery.createLimitSql(request);

        const orderBySql = agQuery.createOrderBySql(request);
        const groupBySql = agQuery.createGroupBySql(request);

        const queryAllSql = selectSql + fromSql + whereSql + groupBySql;
        const queryLimitSql = queryAllSql + orderBySql + limitSql;

        console.log(queryLimitSql);

        return {
            queryAllSql,
            queryLimitSql
        };
    },

    createSelectSql: (request) => {
        const rowGroupCols = request.rowGroupCols;
        const valueCols = request.valueCols;
        const groupKeys = request.groupKeys;

        if (agQuery.isDoingGrouping(rowGroupCols, groupKeys)) {
            const colsToSelect = [];

            const rowGroupCol = rowGroupCols[groupKeys.length];
            colsToSelect.push(rowGroupCol.field);

            valueCols.forEach(function (valueCol) {
                colsToSelect.push(valueCol.aggFunc + '(' + valueCol.field + ') as ' + valueCol.field);
            });

            return ' select ' + colsToSelect.join(', ');
        }

        return ' select *';
    },

    createFilterSql: (key, item) => {
        switch (item.filterType) {
            case 'text':
                return agQuery.createTextFilterSql(key, item);
            case 'number':
                return agQuery.createNumberFilterSql(key, item);
            case 'date':
                return agQuery.createDateFilterSql(key, item);
            case 'set':
                return agQuery.createSetFilterSql(key, item);
            default:
                console.log('unkonwn filter type: ' + item.filterType);
        }
    },

    createNumberFilterSql: (key, item) => {
        switch (item.type) {
            case 'equals':
                return key + ' = ' + item.filter;
            case 'notEqual':
                return key + ' != ' + item.filter;
            case 'greaterThan':
                return key + ' > ' + item.filter;
            case 'greaterThanOrEqual':
                return key + ' >= ' + item.filter;
            case 'lessThan':
                return key + ' < ' + item.filter;
            case 'lessThanOrEqual':
                return key + ' <= ' + item.filter;
            case 'inRange':
                return '(' + key + ' >= ' + item.filter + ' and ' + key + ' <= ' + item.filterTo + ')';
            default:
                console.log('unknown number filter type: ' + item.type);
                return 'true';
        }
    },

    createTextFilterSql: (key, item) => {
        var filter = agQuery.ofSql(item.filter);
        switch (item.type) {
            case 'equals': return `${key} = '${filter}'`;
            case 'notEqual': return `${key} != '${filter}'`;
            case 'contains': return `${key} like '%${filter}%'`;
            case 'notContains': return `${key} not like '%${filter}%'`;
            case 'startsWith': return `${key} like '${filter}%'`;
            case 'endsWith': return `${key} like '%${filter}'`;
            default:
                console.log('unknown text filter type: ' + item.type);
                return 'true';
        }
    },

    createDateFilterSql: (key, item) => {
        switch (item.type) {
            case 'equals':
                return `${key} = '${item.dateFrom}'`;
            case 'notEqual':
                return `${key} != '${item.dateFrom}'`;
            case 'greaterThan':
                return `${key} >= '${item.dateFrom}'`;
            case 'lessThan':
                return `${key} <= '${item.dateFrom}'`;
            case 'inRange':
                return `(${key} >= '${item.dateFrom}' and ${key} <= '${item.dateTo}')`;
            case "blank":
                return `${key} is null`;
            case "notBlank":
                return `${key} is not null`;
            default:
                console.log('unknown text filter type: ' + item.type);
                return 'true';
        }
    },

    createSetFilterSql: (key, item) => {
        switch (item.filterType) {
            case 'set':
                if (item.values.length) {
                    return `${key} in ('${item.values.join("','")}')`;
                } else {
                    return `${key} is null`;
                }
            default:
                console.log('unknown text filter type: ' + item.type);
                return 'true';
        }
    },

    createWhereSql: (request) => {
        const rowGroupCols = request.rowGroupCols;
        const groupKeys = request.groupKeys;
        const filterModel = request.filterModel;

        const whereParts = [];

        if (groupKeys.length > 0) {
            groupKeys.forEach(function (key, index) {
                const colName = rowGroupCols[index].field;
                whereParts.push(`${colName} = '${agQuery.ofSql(key)}'`);
            });
        }

        if (filterModel) {
            const keySet = Object.keys(filterModel);
            keySet.forEach(function (key) {
                const item = filterModel[key];
                console.log(item);
                whereParts.push(agQuery.createFilterSql(key, item));
            });
        }

        if (whereParts.length > 0) {
            return ' where ' + whereParts.join(' and ');
        } else {
            return '';
        }
    },

    createGroupBySql: (request) => {
        const rowGroupCols = request.rowGroupCols;
        const groupKeys = request.groupKeys;

        if (agQuery.isDoingGrouping(rowGroupCols, groupKeys)) {
            const colsToGroupBy = [];

            const rowGroupCol = rowGroupCols[groupKeys.length];
            colsToGroupBy.push(rowGroupCol.field);

            return ' group by ' + colsToGroupBy.join(', ');
        } else {
            return '';
        }
    },

    createOrderBySql: (request) => {
        const rowGroupCols = request.rowGroupCols;
        const groupKeys = request.groupKeys;
        const sortModel = request.sortModel;

        const grouping = agQuery.isDoingGrouping(rowGroupCols, groupKeys);

        const sortParts = [];
        if (sortModel) {

            const groupColIds =
                rowGroupCols.map(groupCol => groupCol.id)
                    .slice(0, groupKeys.length + 1);

            sortModel.forEach(function (item) {
                if (grouping && groupColIds.indexOf(item.colId) < 0) {
                    // ignore
                } else {
                    sortParts.push(item.colId + ' ' + item.sort);
                }
            });
        }

        if (sortParts.length > 0) {
            return ' order by ' + sortParts.join(', ');
        } else {
            switch (agQuery.typeDB) {
                case "SQLServer":
                    return ' ORDER BY(SELECT NULL) ';
                default:
                    return '';
            }
        }
    },

    isDoingGrouping: (rowGroupCols, groupKeys) => {
        // we are not doing grouping if at the lowest level. we are at the lowest level
        // if we are grouping by more columns than we have keys for (that means the user
        // has not expanded a lowest level group, OR we are not grouping at all).
        return rowGroupCols.length > groupKeys.length;
    },

    createLimitSql: (request) => {
        const startRow = request.startRow;
        const endRow = request.endRow;
        const pageSize = endRow - startRow;

        switch (agQuery.typeDB) {
            case "SQLite":
            case "PostgreSQL":
                return ` LIMIT ${pageSize} OFFSET ${startRow}`
            case "MySQL":
            case "MariaDB":
                return ` LIMIT ${startRow},${pageSize}`
            case "SQLServer":
                return ` OFFSET ${startRow} ROWS FETCH NEXT ${pageSize} ROWS ONLY`
            default:
                return '';
        }
    }
};

var page = {
    domGrid: document.querySelector(".nr-grid"),
    domSeTable: document.querySelector(".nr-se-table"),

    init: () => {
        page.tableMeta("table");

        page.domSeTable.addEventListener("sl-change", function () {
            agQuery.table = this.value;
            if (agQuery.table != "") {
                page.tableMeta("column");
            }
        }, false)

        window.addEventListener('resize', function () {
            page.autoSize();
        });
    },
    tableMeta: (name) => {
        fetch(`/Admin/TableMeta?name=${name}&tableName=${agQuery.table}`).then(res => res.json()).then(res => {
            if (res.code == 200) {
                switch (name) {
                    case "table":
                        {
                            agQuery.typeDB = res.log[0];
                            res.data.forEach(item => {
                                var sntn = item.SchemaName ? `${item.SchemaName}.${item.TableName}` : item.TableName;

                                var domItem = document.createElement("sl-menu-item");
                                domItem.value = sntn;
                                domItem.innerText = sntn;
                                page.domSeTable.appendChild(domItem);
                            })
                        }
                        break;
                    case "column":
                        {
                            agQuery.columns = [
                                {
                                    headerName: "🆔", valueGetter: "node.rowIndex + 1", width: 120, maxWidth: 150,
                                    sortable: false, filter: false, menuTabs: false
                                }
                            ];
                            res.data.forEach(item => {
                                agQuery.columns.push({
                                    field: item.ColumnName, maxWidth: 900, enableRowGroup: true, headerTooltip: [item.ColumnType, item.ColumnComment].join(' '),
                                });
                            })

                            page.load();
                        }
                        break;
                }
            }
        })
    },
    load: () => {

        var gridOptions = {
            localeText: ag.localeText, //语言
            defaultColDef: {
                filter: 'agTextColumnFilter', floatingFilter: true,
                sortable: true, resizable: true, width: 320
            },
            rowGroupPanelShow: 'always',
            //列
            columnDefs: agQuery.columns,
            pagination: true, //启用分页
            paginationPageSize: 500, //显示行数
            cacheBlockSize: 30, //请求行数
            enableRangeSelection: true, //范围选择
            animateRows: true, //动画
            serverSideStoreType: 'partial',
            rowModelType: 'serverSide', //服务器模式
            //数据源
            serverSideDatasource: {
                getRows: params => {
                    var pr = params.request; //请求参数
                    console.log(pr)

                    var pars = agQuery.buildSql(pr);
                    pars.queryAllSql = `select count(1) from (${pars.queryAllSql}) T`;

                    var querys = [];
                    for (var i in pars) {
                        querys.push(i + "=" + encodeURIComponent(pars[i]));
                    }

                    fetch(`/Admin/TableQuery?${querys.join('&')}`).then(x => x.json()).then(res => {
                        if (res.code == 200) {
                            params.success({ rowData: res.data.table, rowCount: res.data.count });
                        }

                    }).catch(err => {
                        console.log(err);
                        params.fail();
                    })
                }
            },
            onGridReady: () => {
                //自适应
                page.autoSize();
            },
            getContextMenuItems: (params) => {
                var result = [
                    { name: "Reload", icon: "🔄", action: page.load },
                    'autoSizeAll',
                    'resetColumns',
                    'separator',
                    'copy',
                    'copyWithHeaders',
                    'separator',
                    'export',
                    'chartRange'
                ];

                return result;
            },
        };
        if (page.gridOps) {
            page.gridOps.api.destroy();
        }
        page.gridOps = new agGrid.Grid(page.domGrid, gridOptions).gridOptions;
    },
    autoSize: () => {
        var ch = document.documentElement.clientHeight;

        if (page.gridOps) {
            var vh = ch - page.domGrid.getBoundingClientRect().top - 15;
            page.domGrid.style.height = vh + "px";
        }
    },
}

page.init();