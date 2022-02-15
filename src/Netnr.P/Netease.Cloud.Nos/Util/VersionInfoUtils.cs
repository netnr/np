using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Util
{
    public static class VersionInfoUtils
    {
        private const string USERAGENT_PREFIX = "nos-sdk-dotnet/";

        private static String _userAgent = null;

        public static string getUserAgent()
        {
            if (_userAgent == null)
                initializeUserAgent();
            return _userAgent;
        }

        internal static string DetermineOsVersion()
        {
            try
            {
                var os = Environment.OSVersion;
                return "windows" + os.Version.Major + "." + os.Version.Minor;
            }
            catch (InvalidOperationException)
            {
                return "Unknown OSVersion";
            }
        }

        internal static string DetermineSystemArchitecture()
        {
            return (IntPtr.Size == 8) ? "x86_64" : "x86";
        }

        private static void initializeUserAgent()
        {
            _userAgent = USERAGENT_PREFIX +
                typeof(ClientConfiguration).Assembly.GetName().Version + "(" +
                DetermineOsVersion() + "/" +
                Environment.OSVersion.Version + "/" +
                DetermineSystemArchitecture() + ":" +
                Environment.Version + ")";

        }
    }
}
