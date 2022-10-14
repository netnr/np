using System.Text.RegularExpressions;

namespace AgGrid.InfiniteRowModel
{
    internal static class StringExtensions
    {
        public static string ToPascalCase(this string name)
            => Regex.Replace(name, "^([a-z])", m => m.Groups[1].Value.ToUpper());
    }
}
