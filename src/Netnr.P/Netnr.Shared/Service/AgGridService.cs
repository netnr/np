#if Full || Service

namespace Netnr;

/// <summary>
/// AG Grid
/// </summary>
public partial class AgGridService
{
    #region args

    /// <summary>
    /// 数据库类型
    /// </summary>
    public DBTypes Tdb { get; set; }

    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 别名列映射，别名:真实列名
    /// </summary>
    public Dictionary<string, string> AliasMapColumn { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// 预置查询列，列字段逗号分割
    /// </summary>
    public string PresetColumn { get; set; }

    /// <summary>
    /// 预置条件，不带 Where 的条件项
    /// </summary>
    public string PresetWhere { get; set; }

    /// <summary>
    /// 查询条件
    /// rowGroupCols 分组列，数组
    /// valueCols 分组统计项，数组
    /// groupKeys 分组父级路径，数组
    /// sortModel 排序列，数组
    /// startRow 分页开始行号
    /// endRow 分页结束行号
    /// </summary>
    public JsonElement.ObjectEnumerator ParamsJson { get; set; }

    #endregion

    /// <summary>
    /// 构建SQL，返回页查询、总数查询
    /// </summary>
    /// <returns></returns>
    public ValueTuple<string, string> BuildSql()
    {
        var selectSql = CreateSelectSql();
        var fromSql = $"FROM {TableName}";
        var whereSql = CreateWhereSql();
        var limitSql = CreateLimitSql();
        var orderBySql = CreateOrderBySql();
        var groupBySql = CreateGroupBySql();

        var sql = $"{selectSql.Trim()} {fromSql.Trim()} {whereSql.Trim()} {groupBySql.Trim()} {orderBySql.Trim()} {limitSql.Trim()}".Trim();
        var sqlCount = $"SELECT COUNT(*) FROM ({selectSql.Trim()} {fromSql.Trim()} {whereSql.Trim()} {groupBySql.Trim()}) TTT".Trim();

        return new(sql, sqlCount);
    }

    /// <summary>
    /// 查询列
    /// </summary>
    /// <returns></returns>
    public string CreateSelectSql()
    {
        var listCols = new List<string>();

        var rowGroupCols = ParamsJson.FirstOrDefault(x => x.Name == "rowGroupCols").Value;
        var valueCols = ParamsJson.FirstOrDefault(x => x.Name == "valueCols").Value;
        var groupKeys = ParamsJson.FirstOrDefault(x => x.Name == "groupKeys").Value;

        if (rowGroupCols.ValueKind == JsonValueKind.Array && groupKeys.ValueKind == JsonValueKind.Array)
        {
            var groupCols = rowGroupCols.EnumerateArray();
            if (IsDoingGrouping(groupCols, groupKeys.EnumerateArray()))
            {
                var colsToSelect = groupCols.Select(x =>
                {
                    var field = x.GetValue("field");
                    if (AliasMapColumn.ContainsKey(field))
                    {
                        field = AliasMapColumn[field];
                    }
                    else
                    {
                        field = ParsingTo.ReplaceDanger(field);
                    }
                    return field;
                });

                //valueCols.forEach(function(valueCol) {
                //    colsToSelect.push(valueCol.aggFunc + '(' + valueCol.field + ') as ' + valueCol.field);
                //});

                listCols.AddRange(colsToSelect);
            }
        }

        if (!string.IsNullOrWhiteSpace(PresetColumn))
        {
            listCols.AddRange(PresetColumn.Split(','));
        }
        else if (listCols.Count == 0)
        {
            listCols.Add("*");
        }

        return $"SELECT {string.Join(", ", listCols)}";
    }

    /// <summary>
    /// 构建过滤
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public string CreateFilterSql(JsonProperty item)
    {
        var result = string.Empty;

        var field = item.Name;
        if (AliasMapColumn.ContainsKey(field))
        {
            field = AliasMapColumn[field];
        }
        else
        {
            field = ParsingTo.ReplaceDanger(field);
        }
        field = DbKitExtensions.SqlQuote(field, Tdb);

        var filterType = item.Value.GetProperty("filterType").ToString();
        var type = item.Value.GetProperty("type").ToString();
        if (type == "blank")
        {
            result = filterType == "number" ? $"{field} IS NULL" : $"({field} IS NULL OR {field} = '')";
        }
        else if (type == "notBlank")
        {
            result = filterType == "number" ? $"{field} IS NOT NULL" : $"{field} IS NOT NULL OR {field} != ''";
        }
        else
        {
            switch (filterType)
            {
                case "text":
                    {
                        var filter = item.Value.GetProperty("filter").ToString().OfSql();
                        switch (type)
                        {
                            case "equals":
                                result = $"{field} = '{filter}'";
                                break;
                            case "notEqual":
                                result = $"{field} != '{filter}'";
                                break;
                            case "contains":
                                result = $"{field} LIKE '%{filter}%'";
                                break;
                            case "notContains":
                                result = $"{field} NOT LIKE '%{filter}%'";
                                break;
                            case "startsWith":
                                result = $"{field} LIKE '{filter}%'";
                                break;
                            case "endsWith":
                                result = $"{field} LIKE '%{filter}'";
                                break;
                        }
                    }
                    break;
                case "number":
                    {
                        var filter = Convert.ToDecimal(item.Value.GetProperty("filter").ToString());
                        switch (type)
                        {
                            case "equals":
                                result = $"{field} = {filter}";
                                break;
                            case "notEqual":
                                result = $"{field} != {filter}";
                                break;
                            case "greaterThan":
                                result = $"{field} > {filter}";
                                break;
                            case "greaterThanOrEqual":
                                result = $"{field} >= {filter}";
                                break;
                            case "lessThan":
                                result = $"{field} < {filter}";
                                break;
                            case "lessThanOrEqual":
                                result = $"{field} <= {filter}";
                                break;
                            case "inRange":
                                var filterTo = Convert.ToDecimal(item.Value.GetProperty("filterTo").ToString());
                                result = $"{field} >= {filter} AND {field} <= {filterTo}";
                                break;
                        }
                    }
                    break;
                case "date":
                    {
                        var dateFrom = DateTime.Parse(item.Value.GetProperty("dateFrom").ToString()).ToString("yyyyMMdd");

                        switch (Tdb)
                        {
                            case DBTypes.SQLite:
                                field = $"STRFTIME('%Y%m%d', {field})";
                                break;
                            case DBTypes.MySQL:
                            case DBTypes.MariaDB:
                                field = $"DATE_FORMAT({field}, '%Y%m%d')";
                                break;
                            case DBTypes.Oracle:
                                field = $"TO_CHAR({field}, 'yyyymmdd')";
                                break;
                            case DBTypes.SQLServer:
                                field = $"CONVERT(VARCHAR(99), {field}, 112)";
                                break;
                            case DBTypes.PostgreSQL:
                                field = $"TO_CHAR({field}, 'YYYYMMDD')";
                                break;
                        }

                        switch (type)
                        {
                            case "equals":
                                result = $"{field} = '{dateFrom}'";
                                break;
                            case "notEqual":
                                result = $"{field} != '{dateFrom}'";
                                break;
                            case "greaterThan":
                                result = $"{field} > '{dateFrom}'";
                                break;
                            case "greaterThanOrEqual":
                                result = $"{field} >= '{dateFrom}'";
                                break;
                            case "lessThan":
                                result = $"{field} < '{dateFrom}'";
                                break;
                            case "lessThanOrEqual":
                                result = $"{field} <= '{dateFrom}'";
                                break;
                            case "inRange":
                                var dateTo = DateTime.Parse(item.Value.GetProperty("dateTo").ToString()).ToString("yyyyMMdd");
                                result = $"{field} >= '{dateFrom}' AND {field} <= '{dateTo}'";
                                break;
                        }
                    }
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// 过滤
    /// </summary>
    /// <returns></returns>
    public string CreateWhereSql()
    {
        var result = string.Empty;
        if (!string.IsNullOrWhiteSpace(PresetWhere))
        {
            result = $"WHERE {PresetWhere}";
        }

        var whereParts = new List<string>();

        var rowGroupCols = ParamsJson.FirstOrDefault(x => x.Name == "rowGroupCols").Value;
        var groupKeys = ParamsJson.FirstOrDefault(x => x.Name == "groupKeys").Value;

        var filterModel = ParamsJson.FirstOrDefault(x => x.Name == "filterModel").Value;

        if (groupKeys.ValueKind == JsonValueKind.Array)
        {
            //    groupKeys.forEach(function(key, index) {
            //        var colName = rowGroupCols[index].field;
            //        whereParts.push(colName + ' = "' + key + '"')
            //            });
        }

        if (filterModel.ValueKind == JsonValueKind.Object)
        {
            var filterParts = filterModel.EnumerateObject().Select(item =>
            {
                var iobj = item.Value.EnumerateObject();
                var operator_ = iobj.FirstOrDefault(x => x.Name == "operator").Value;
                if (operator_.ValueKind == JsonValueKind.String)
                {
                    var arr = Enumerable.Range(1, 2).ToList().Select(i =>
                    {
                        var condition = item.Value.GetProperty($"condition{i}");

                        var jp = new Dictionary<string, JsonElement> { { item.Name, condition.ToJson().DeJson() } }
                        .ToJson().DeJson().EnumerateObject().FirstOrDefault();

                        return CreateFilterSql(jp);
                    });

                    return $"({string.Join($" {operator_} ", arr)})";
                }
                else
                {
                    return CreateFilterSql(item);
                }
            });
            whereParts.AddRange(filterParts);
        }

        if (whereParts.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(result))
            {
                result = $"WHERE {string.Join(" AND ", whereParts)}";
            }
            else
            {
                result += $" AND {string.Join(" AND ", whereParts)}";
            }
        }

        return result;
    }

    /// <summary>
    /// 分组
    /// </summary>
    /// <returns></returns>
    public string CreateGroupBySql()
    {
        var result = string.Empty;

        var rowGroupCols = ParamsJson.FirstOrDefault(x => x.Name == "rowGroupCols").Value;
        var groupKeys = ParamsJson.FirstOrDefault(x => x.Name == "groupKeys").Value;

        if (rowGroupCols.ValueKind == JsonValueKind.Array && groupKeys.ValueKind == JsonValueKind.Array)
        {
            var groupCols = rowGroupCols.EnumerateArray();
            if (IsDoingGrouping(groupCols, groupKeys.EnumerateArray()))
            {
                var colsToGroupBy = groupCols.Select(x =>
                {
                    var field = x.GetValue("field");
                    if (AliasMapColumn.ContainsKey(field))
                    {
                        field = AliasMapColumn[field];
                    }
                    else
                    {
                        field = ParsingTo.ReplaceDanger(field);
                    }

                    return field;
                });

                return $"GROUP BY {string.Join(", ", colsToGroupBy)}";
            }
        }

        return result;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <returns></returns>
    public string CreateOrderBySql()
    {
        var result = string.Empty;
        var sortParts = new List<string>();

        var rowGroupCols = ParamsJson.FirstOrDefault(x => x.Name == "rowGroupCols").Value;
        var groupKeys = ParamsJson.FirstOrDefault(x => x.Name == "groupKeys").Value;
        var sortModel = ParamsJson.FirstOrDefault(x => x.Name == "sortModel").Value;

        if (sortModel.ValueKind == JsonValueKind.Array)
        {
            _ = sortModel.EnumerateArray().Select(item =>
            {
                var field = item.GetProperty("colId").ToString();

                if (AliasMapColumn.ContainsKey(field))
                {
                    field = AliasMapColumn[field];
                }
                else
                {
                    field = ParsingTo.ReplaceDanger(field);
                }
                field = DbKitExtensions.SqlQuote(field, Tdb);

                var sort = item.GetProperty("sort").ToString().ToUpper();
                sort = sort == "DESC" ? sort : "ASC";

                sortParts.Add($"{field} {sort}");

                return field;

            }).ToList();
        }

        if (sortParts.Count > 0)
        {
            result = $"ORDER BY {string.Join(", ", sortParts)}";
        }
        else
        {
            if (Tdb == DBTypes.SQLServer)
            {
                result = "ORDER BY (SELECT NULL)";
            }
        }

        return result;
    }

    public static bool IsDoingGrouping(JsonElement.ArrayEnumerator rowGroupCols, JsonElement.ArrayEnumerator groupKeys)
    {
        return rowGroupCols.Count() > groupKeys.Count();
    }

    /// <summary>
    /// 分页
    /// </summary>
    /// <returns></returns>
    public string CreateLimitSql()
    {
        var startRow = Convert.ToInt32(ParamsJson.First(x => x.Name == "startRow").Value.ToString());
        var endRow = Convert.ToInt32(ParamsJson.First(x => x.Name == "endRow").Value.ToString());
        var pageSize = endRow - startRow;

        var result = string.Empty;
        switch (Tdb)
        {
            case DBTypes.SQLite:
            case DBTypes.PostgreSQL:
                result = $"LIMIT {pageSize} OFFSET {startRow}";
                break;
            case DBTypes.MySQL:
            case DBTypes.MariaDB:
                result = $"LIMIT {startRow}, {pageSize}";
                break;
            case DBTypes.Oracle:
                break;
            case DBTypes.SQLServer:
                result = $"OFFSET {startRow} ROWS FETCH NEXT {pageSize} ROWS ONLY";
                break;
            default:
                throw new Exception("database type not set");
        }
        return result;
    }
}

#endif