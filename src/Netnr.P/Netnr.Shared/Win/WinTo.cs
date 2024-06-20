#if Full || Win

using Microsoft.Win32;
using System.Runtime.Versioning;

namespace Netnr;

[SupportedOSPlatform("windows")]
public partial class WinTo
{
    #region .NET Framework https://github.com/jmalarcon/DotNetVersions

    /// <summary>
    /// 版本信息
    /// </summary>
    public class DOTNETFrameworkModel
    {
        public string Version { get; set; }
        public string ServicePack { get; set; }
    }

    /// <summary>
    /// Writes the version
    /// </summary>
    /// <param name="result"></param>
    /// <param name="version"></param>
    /// <param name="spLevel"></param>
    static void WriteVersion(ref List<DOTNETFrameworkModel> result, string version, string spLevel = "")
    {
        version = version.Trim();
        if (!string.IsNullOrEmpty(version))
        {
            var model = new DOTNETFrameworkModel
            {
                Version = version
            };
            if (!string.IsNullOrWhiteSpace(spLevel))
            {
                model.ServicePack = $"Service Pack {spLevel}";
            }

            result.Add(model);
        }
    }

    /// <summary>
    /// 1-4.5
    /// </summary>
    /// <returns></returns>
    public static List<DOTNETFrameworkModel> Get45Less()
    {
        var result = new List<DOTNETFrameworkModel>();

        // Opens the registry key for the .NET Framework entry.
        using var ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\");
        foreach (string versionKeyName in ndpKey.GetSubKeyNames())
        {
            // Skip .NET Framework 4.5 version information.
            if (versionKeyName == "v4")
            {
                continue;
            }

            if (versionKeyName.StartsWith('v'))
            {
                RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                // Get the .NET Framework version value.
                string name = (string)versionKey.GetValue("Version", "");
                // Get the service pack (SP) number.
                string sp = versionKey.GetValue("SP", "").ToString();
                // Get the installation flag, or an empty string if there is none.
                string install = versionKey.GetValue("Install", "").ToString();
                if (string.IsNullOrEmpty(install)) // No install info; it must be in a child subkey.
                    WriteVersion(ref result, name);
                else
                {
                    if (!(string.IsNullOrEmpty(sp)) && install == "1")
                    {
                        WriteVersion(ref result, name, sp);
                    }
                }
                if (!string.IsNullOrEmpty(name))
                {
                    continue;
                }
                foreach (string subKeyName in versionKey.GetSubKeyNames())
                {
                    RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                    name = (string)subKey.GetValue("Version", "");
                    if (!string.IsNullOrEmpty(name))
                        sp = subKey.GetValue("SP", "").ToString();

                    install = subKey.GetValue("Install", "").ToString();
                    if (string.IsNullOrEmpty(install)) //No install info; it must be later.
                        WriteVersion(ref result, name);
                    else
                    {
                        if (!(string.IsNullOrEmpty(sp)) && install == "1")
                        {
                            WriteVersion(ref result, name, sp);
                        }
                        else if (install == "1")
                        {
                            WriteVersion(ref result, name);
                        }
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 4.5+
    /// </summary>
    /// <returns></returns>
    public static List<DOTNETFrameworkModel> Get45Plus()
    {
        var result = new List<DOTNETFrameworkModel>();

        const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

        using var ndpKey = Registry.LocalMachine.OpenSubKey(subkey);
        if (ndpKey != null)
        {
            //First check if there's an specific version indicated
            if (ndpKey.GetValue("Version") != null)
            {
                WriteVersion(ref result, ndpKey.GetValue("Version").ToString());
            }
            else
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    WriteVersion(ref result, CheckFor45PlusVersion((int)ndpKey.GetValue("Release")));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Checking the version using >= enables forward compatibility.
    /// </summary>
    /// <param name="releaseKey"></param>
    /// <returns></returns>
    static string CheckFor45PlusVersion(int releaseKey)
    {
        if (releaseKey >= 533320)
            return "4.8.1";
        if (releaseKey >= 528040)
            return "4.8";
        if (releaseKey >= 461808)
            return "4.7.2";
        if (releaseKey >= 461308)
            return "4.7.1";
        if (releaseKey >= 460798)
            return "4.7";
        if (releaseKey >= 394802)
            return "4.6.2";
        if (releaseKey >= 394254)
            return "4.6.1";
        if (releaseKey >= 393295)
            return "4.6";
        if (releaseKey >= 379893)
            return "4.5.2";
        if (releaseKey >= 378675)
            return "4.5.1";
        if (releaseKey >= 378389)
            return "4.5";
        // This code should never execute. A non-null release key should mean
        // that 4.5 or later is installed.
        return "";
    }

    #endregion
}

#endif