let nrcAgGridQuery = {
    ofSql: sql => {
        if (typeof sql === 'string') {
            return sql.replaceAll("'", "''");
        }
        return sql
    },
    DBTypes: "SQLite",

    buildSql: (request) => {

        const selectSql = nrcAgGridQuery.createSelectSql(request);
        const fromSql = ' from (TABLE) ';
        const whereSql = nrcAgGridQuery.createWhereSql(request);
        const limitSql = nrcAgGridQuery.createLimitSql(request);

        const orderBySql = nrcAgGridQuery.createOrderBySql(request);
        const groupBySql = nrcAgGridQuery.createGroupBySql(request);

        const queryAllSql = selectSql + fromSql + whereSql + groupBySql;
        const queryLimitSql = queryAllSql + orderBySql + limitSql;

        return { queryAllSql, queryLimitSql };
    },

    createSelectSql: (request) => {
        const rowGroupCols = request.rowGroupCols;
        const valueCols = request.valueCols;
        const groupKeys = request.groupKeys;

        if (nrcAgGridQuery.isDoingGrouping(rowGroupCols, groupKeys)) {
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
                return nrcAgGridQuery.createTextFilterSql(key, item);
            case 'number':
                return nrcAgGridQuery.createNumberFilterSql(key, item);
            case 'date':
                return nrcAgGridQuery.createDateFilterSql(key, item);
            case 'set':
                return nrcAgGridQuery.createSetFilterSql(key, item);
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
        let filter = nrcAgGridQuery.ofSql(item.filter);
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
                whereParts.push(`${colName} = '${nrcAgGridQuery.ofSql(key)}'`);
            });
        }

        if (filterModel) {
            const keySet = Object.keys(filterModel);
            keySet.forEach(function (key) {
                const item = filterModel[key];
                whereParts.push(nrcAgGridQuery.createFilterSql(key, item));
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

        if (nrcAgGridQuery.isDoingGrouping(rowGroupCols, groupKeys)) {
            const colsToGroupBy = [];

            const rowGroupCol = rowGroupCols[groupKeys.length];
            colsToGroupBy.push(rowGroupCol.field);

            return ' group by ' + colsToGroupBy.join(', ');
        } else {
            // select all columns
            return '';
        }
    },

    createOrderBySql: (request) => {
        const rowGroupCols = request.rowGroupCols;
        const groupKeys = request.groupKeys;
        const sortModel = request.sortModel;

        const grouping = nrcAgGridQuery.isDoingGrouping(rowGroupCols, groupKeys);

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
            return '';
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

        switch (nrcAgGridQuery.DBTypes) {
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

Object.assign(window, { nrcAgGridQuery });
export { nrcAgGridQuery }