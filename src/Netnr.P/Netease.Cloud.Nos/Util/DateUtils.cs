using System;
using System.Globalization;

namespace Netease.Cloud.NOS.Util
{
    internal static class DateUtils
    {
        private const string Rfc822DateFormat = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";
        private const string Iso8601DateFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

        public static string FormatRfc822Date(DateTime datetime)
        {
            return datetime.ToUniversalTime().ToString(Rfc822DateFormat,
                               CultureInfo.InvariantCulture);
        }

        public static DateTime ParseRfc822Date(String s)
        {
            return DateTime.SpecifyKind(
                DateTime.ParseExact(s, Rfc822DateFormat, CultureInfo.InvariantCulture),
                DateTimeKind.Utc);
        }

        public static string FormatIso8601Date(DateTime datetime)
        {
            return datetime.ToUniversalTime().ToString(Iso8601DateFormat, CultureInfo.CurrentCulture);
        }
    }
}
