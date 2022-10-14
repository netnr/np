using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace AgGrid.InfiniteRowModel
{
    internal static class InfiniteScroll
    {
        public static GetRowsParams DeserializeGetRowsParams(string json) =>
            JsonSerializer.Deserialize<GetRowsParams>(json, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        public static IQueryable<T> ToQueryableRows<T>(IQueryable<T> queryable, GetRowsParams getRowsParams, InfiniteRowModelOptions options, out IQueryable<T> queryableCount)
        {
            options ??= new InfiniteRowModelOptions();

            ValidateColIds<T>(getRowsParams);

            queryableCount = queryable.Filter(getRowsParams, options);

            return queryableCount.Sort(getRowsParams).Skip(getRowsParams.StartRow).Take(TakeCount(getRowsParams) + 1);
        }

        public static InfiniteRowModelResult<T> ToRowModelResult<T>(GetRowsParams getRowsParams, List<T> rows, IQueryable<T> queryableCount = null)
        {
            var reachedEnd = rows.Count <= TakeCount(getRowsParams);

            //always returns the total count
            var lastRow = getRowsParams.StartRow + rows.Count;
            if (reachedEnd == false && queryableCount != null)
            {
                lastRow = queryableCount.Count();
            }

            return new InfiniteRowModelResult<T>
            {
                RowsThisBlock = rows.Take(TakeCount(getRowsParams)).ToList(),
                LastRow = lastRow
            };
        }

        private static int TakeCount(GetRowsParams getRowsParams)
            => getRowsParams.EndRow - getRowsParams.StartRow;

        private static void ValidateColIds<T>(GetRowsParams getRowsParams)
        {
            var propertyNames = GetPropertyNames<T>();
            var invalidColIds = GetColIds(getRowsParams).Where(c => !propertyNames.Contains(c.ToPascalCase()));

            if (invalidColIds.Any())
            {
                throw new ArgumentException($"Invalid colIds: {string.Join(", ", invalidColIds)}.");
            }
        }

        private static HashSet<string> GetPropertyNames<T>()
            => typeof(T).GetProperties().Select(p => p.Name).ToHashSet();

        private static IEnumerable<string> GetColIds(GetRowsParams getRowsParams)
            => getRowsParams.FilterModel.Select(f => f.Key);

        private static IQueryable<T> Filter<T>(this IQueryable<T> queryable, GetRowsParams getRowsParams, InfiniteRowModelOptions options)
        {
            var props = typeof(T).GetProperties();

            foreach (var kvp in getRowsParams.FilterModel)
            {
                var colId = kvp.Key;
                var filterModel = kvp.Value;

                // datetime is nullable
                var colType = props.First(x => x.Name == colId).PropertyType;
                var nullableDateTime = colType == typeof(DateTime?);

                if (string.IsNullOrEmpty(filterModel.Operator))
                {
                    var predicate = GetPredicate(colId, filterModel, 0, options, nullableDateTime);
                    var args = GetWhereArgs(colId, filterModel, options, colType);

                    queryable = queryable.Where(predicate, args);
                }
                else
                {
                    ValidateOperator(filterModel);

                    var predicateLeftSide = GetPredicate(colId, filterModel.Condition1, 0, options, nullableDateTime);
                    var argsLeftSide = GetWhereArgs(colId, filterModel.Condition1, options, colType);

                    var rightSideArgsIndex = argsLeftSide.Length;

                    var predicateRightSide = GetPredicate(colId, filterModel.Condition2, rightSideArgsIndex, options, nullableDateTime);
                    var argsRightSide = GetWhereArgs(colId, filterModel.Condition2, options, colType);

                    var predicate = $"({predicateLeftSide}) {filterModel.Operator} ({predicateRightSide})";
                    var args = argsLeftSide.Concat(argsRightSide).ToArray();

                    queryable = queryable.Where(predicate, args);
                }
            }

            return queryable;
        }

        private static void ValidateOperator(FilterModel filterModel)
        {
            if (!FilterModelOperator.All.Contains(filterModel.Operator))
            {
                throw new ArgumentException($"Unsupported {nameof(FilterModel.Operator)} value ({filterModel.Operator}). Supported values: {string.Join(", ", FilterModelOperator.All)}.");
            }
        }

        private static object[] GetWhereArgs(string colId, FilterModel filterModel, InfiniteRowModelOptions options, Type colType = null)
        {
            return filterModel switch
            {
                { Type: FilterModelType.Null or FilterModelType.NotNull } => Array.Empty<object>(),

                { FilterType: FilterModelFilterType.Text } when options.CaseInsensitive => new object[] { GetString(filterModel.Filter).ToLower() },
                { FilterType: FilterModelFilterType.Text } => new object[] { GetString(filterModel.Filter) },

                { FilterType: FilterModelFilterType.Number, Type: FilterModelType.InRange } => new object[] { GetNumber(filterModel.Filter), filterModel.FilterTo },
                { FilterType: FilterModelFilterType.Number } => new object[] { GetNumber(filterModel.Filter) },

                { FilterType: FilterModelFilterType.Date, Type: FilterModelType.InRange } => new object[] { GetDate(filterModel.DateFrom), GetDate(filterModel.DateTo) },
                { FilterType: FilterModelFilterType.Date } => new object[] { GetDate(filterModel.DateFrom) },

                { FilterType: FilterModelFilterType.Boolean } => new object[] { GetBoolean(filterModel.Filter) },

                { FilterType: FilterModelFilterType.Set } when options.CaseInsensitive => new object[] { filterModel.Values.Select(v => v?.ToLower()).ToList() },
                { FilterType: FilterModelFilterType.Set } when colType == typeof(int?) => new object[] { filterModel.Values.Select(x => int.TryParse(x, out int xv) ? new int?(xv) : null).ToList() },
                { FilterType: FilterModelFilterType.Set } when colType == typeof(int) => new object[] { filterModel.Values.Select(x => int.Parse(x)).ToList() },
                { FilterType: FilterModelFilterType.Set } => new object[] { filterModel.Values },

                _ => throw new ArgumentException($"Unable to determine predicate arguments for {colId}. Most likely {nameof(FilterModel.FilterType)} value ({filterModel.FilterType}) is unsupported. Supported values: {string.Join(", ", FilterModelFilterType.All)}.")
            };
        }

        private static string GetString(object element)
            => (element as JsonElement?)?.GetString() ?? (string)element;

        private static double GetNumber(object element)
            => (element as JsonElement?)?.GetDouble() ?? Convert.ToDouble(element);

        private static DateTime? GetDate(string dateString)
        {
            if (!string.IsNullOrEmpty(dateString))
            {
                return DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            return null;
        }

        private static bool GetBoolean(object element)
            => (element as JsonElement?)?.GetBoolean() ?? (bool)element;

        private static string GetPredicate(string colId, FilterModel filterModel, int index, InfiniteRowModelOptions options, bool nullableDateTime = true)
        {
            var propertyName = colId.ToPascalCase();

            return filterModel switch
            {
                { Type: FilterModelType.Equals, FilterType: FilterModelFilterType.Text } when options.CaseInsensitive => $"{propertyName}.ToLower() == @{index}",
                { Type: FilterModelType.Equals, FilterType: FilterModelFilterType.Date } => nullableDateTime ? $"{propertyName}.Value.Date == @{index}" : $"{propertyName}.Date == @{index}",
                { Type: FilterModelType.Equals } => $"{propertyName} == @{index}",

                { Type: FilterModelType.NotEqual, FilterType: FilterModelFilterType.Text } when options.CaseInsensitive => $"{propertyName}.ToLower() != @{index}",
                { Type: FilterModelType.NotEqual, FilterType: FilterModelFilterType.Date } => nullableDateTime ? $"{propertyName}.Value.Date != @{index}" : $"{propertyName}.Date != @{index}",
                { Type: FilterModelType.NotEqual } => $"{propertyName} != @{index}",

                { Type: FilterModelType.Contains } when options.CaseInsensitive => $"{propertyName}.ToLower().Contains(@{index})",
                { Type: FilterModelType.Contains } => $"{propertyName}.Contains(@{index})",

                { Type: FilterModelType.NotContains } when options.CaseInsensitive => $"!{propertyName}.ToLower().Contains(@{index})",
                { Type: FilterModelType.NotContains } => $"!{propertyName}.Contains(@{index})",

                { Type: FilterModelType.StartsWith } when options.CaseInsensitive => $"{propertyName}.ToLower().StartsWith(@{index})",
                { Type: FilterModelType.StartsWith } => $"{propertyName}.StartsWith(@{index})",

                { Type: FilterModelType.EndsWith } when options.CaseInsensitive => $"{propertyName}.ToLower().EndsWith(@{index})",
                { Type: FilterModelType.EndsWith } => $"{propertyName}.EndsWith(@{index})",

                { Type: FilterModelType.LessThan, FilterType: FilterModelFilterType.Date } => nullableDateTime ? $"{propertyName}.Value.Date < @{index}" : $"{propertyName}.Date < @{index}",
                { Type: FilterModelType.LessThan } => $"{propertyName} < @{index}",
                { Type: FilterModelType.LessThanOrEqual, FilterType: FilterModelFilterType.Date } => nullableDateTime ? $"{propertyName}.Value.Date <= @{index}" : $"{propertyName}.Date <= @{index}",
                { Type: FilterModelType.LessThanOrEqual } => $"{propertyName} <= @{index}",

                { Type: FilterModelType.GreaterThan, FilterType: FilterModelFilterType.Date } => nullableDateTime ? $"{propertyName}.Value.Date > @{index}" : $"{propertyName}.Date > @{index}",
                { Type: FilterModelType.GreaterThan } => $"{propertyName} > @{index}",
                { Type: FilterModelType.GreaterThanOrEqual, FilterType: FilterModelFilterType.Date } => nullableDateTime ? $"{propertyName}.Value.Date >= @{index}" : $"{propertyName}.Date >= @{index}",
                { Type: FilterModelType.GreaterThanOrEqual } => $"{propertyName} >= @{index}",

                { Type: FilterModelType.InRange } when options.InRangeExclusive => $"{propertyName} > @{index} AND {propertyName} < @{index + 1}",

                { Type: FilterModelType.InRange, FilterType: FilterModelFilterType.Date } => nullableDateTime ? $"{propertyName}.Value.Date >= @{index} AND {propertyName}.Value.Date <= @{index + 1}" : $"{propertyName}.Date >= @{index} AND {propertyName}.Date <= @{index + 1}",
                { Type: FilterModelType.InRange } => $"{propertyName} >= @{index} AND {propertyName} <= @{index + 1}",

                { Type: FilterModelType.Null } => $"{propertyName} == null",
                { Type: FilterModelType.NotNull } => $"{propertyName} != null",

                { Type: FilterModelType.Blank } => $"{propertyName} == null",
                { Type: FilterModelType.NotBlank } => $"{propertyName} != null",

                { FilterType: FilterModelFilterType.Set } when options.CaseInsensitive => $"@{index}.Contains({propertyName}.ToLower())",
                { FilterType: FilterModelFilterType.Set } => $"@{index}.Contains({propertyName})",

                _ => throw new ArgumentException($"Unable to determine predicate for {colId}. Most likely {nameof(FilterModel.Type)} value ({filterModel.Type}) is unsupported. Supported values: {string.Join(", ", FilterModelType.All)}.")
            };
        }

        private static IQueryable<T> Sort<T>(this IQueryable<T> queryable, GetRowsParams getRowsParams)
        {
            ValidateSortDirections(getRowsParams);

            var orderingParts = getRowsParams.SortModel.Select(s => $"{s.ColId.ToPascalCase()} {s.Sort}");

            var ordering = string.Join(", ", orderingParts);

            if (ordering.Length == 0)
            {
                return queryable;
            }

            return queryable.OrderBy(ordering);
        }

        private static void ValidateSortDirections(GetRowsParams getRowsParams)
        {
            foreach (var sort in getRowsParams.SortModel)
            {
                if (!SortModelSortDirection.All.Contains(sort.Sort))
                {
                    throw new ArgumentException($"Unsupported {nameof(SortModel.Sort)} value ({sort.Sort}). Supported values: {string.Join(", ", SortModelSortDirection.All)}.");
                }
            }
        }
    }
}
