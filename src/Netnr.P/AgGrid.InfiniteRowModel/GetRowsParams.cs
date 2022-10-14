using System.Collections.Generic;
using System.Linq;

namespace AgGrid.InfiniteRowModel
{
    public class GetRowsParams
    {
        public int StartRow { get; set; }
        public int EndRow { get; set; }
        public IEnumerable<SortModel> SortModel { get; set; } = Enumerable.Empty<SortModel>();
        public IDictionary<string, FilterModel> FilterModel { get; set; } = new Dictionary<string, FilterModel>();
    }

    public class SortModel
    {
        public string Sort { get; set; }
        public string ColId { get; set; }
    }

    public static class SortModelSortDirection
    {
        public static HashSet<string> All { get; } = new() { Ascending, Descending };

        public const string Ascending = "asc";
        public const string Descending = "desc";
    }

    public class FilterModel
    {
        public string FilterType { get; set; }
        public string Type { get; set; }

        public object Filter { get; set; }
        public double FilterTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public IEnumerable<string> Values { get; set; }

        public string Operator { get; set; }
        public FilterModel Condition1 { get; set; }
        public FilterModel Condition2 { get; set; }
    }

    public static class FilterModelFilterType
    {
        public static HashSet<string> All { get; } = new() { Text, Number, Date, Boolean, Set };

        public const string Text = "text";
        public const string Number = "number";
        public const string Date = "date";
        public const string Boolean = "boolean";
        public const string Set = "set";
    }

    public static class FilterModelType
    {
        public static HashSet<string> All { get; } = new()
        {
            Equals, NotEqual, Contains, NotContains,
            StartsWith, EndsWith, LessThan, LessThanOrEqual,
            GreaterThan, GreaterThanOrEqual, InRange,
            Null, NotNull, Blank, NotBlank
        };

        new public const string Equals = "equals";
        public const string NotEqual = "notEqual";

        public const string Contains = "contains";
        public const string NotContains = "notContains";

        public const string StartsWith = "startsWith";
        public const string EndsWith = "endsWith";

        public const string LessThan = "lessThan";
        public const string LessThanOrEqual = "lessThanOrEqual";

        public const string GreaterThan = "greaterThan";
        public const string GreaterThanOrEqual = "greaterThanOrEqual";

        public const string InRange = "inRange";

        public const string Null = "null";
        public const string NotNull = "notNull";

        public const string Blank = "blank";
        public const string NotBlank = "notBlank";
    }

    public static class FilterModelOperator
    {
        public static HashSet<string> All { get; } = new() { And, Or };

        public const string And = "AND";
        public const string Or = "OR";
    }
}
