using System.Management;
using System.Runtime.Versioning;
using Xunit;

namespace Netnr.Test
{
    public class TestWMI
    {
        [SupportedOSPlatform("windows")]
        [Fact]
        public void WMI_1()
        {
            var options = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate
            };

            var server = "192.168.100.100";
            var scope = new ManagementScope($"\\\\{server}\\root\\cimv2", options);

            try
            {
                scope.Connect();

                var query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                var searcher = new ManagementObjectSearcher(scope, query);

                var queryCollection = searcher.Get();
                foreach (var m in queryCollection)
                {
                    Debug.WriteLine("Computer Name     : {0}", m["csname"]);
                    Debug.WriteLine("Windows Directory : {0}", m["WindowsDirectory"]);
                    Debug.WriteLine("Operating System  : {0}", m["Caption"]);
                    Debug.WriteLine("Version           : {0}", m["Version"]);
                    Debug.WriteLine("Manufacturer      : {0}", m["Manufacturer"]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [SupportedOSPlatform("windows")]
        [Fact]
        public void WMI_2()
        {
            var connection = new ConnectionOptions
            {
                Username = "Administrator",
                Password = "123"
            };
            var server = "192.168.100.100";
            var scope = new ManagementScope(string.Format("\\\\{0}\\root\\cimv2", server), connection);

            string command = "notepad.exe";
            var process = new ManagementClass(scope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
            var inParams = process.GetMethodParameters("Create");
            inParams["CommandLine"] = command;

            var outParams = process.InvokeMethod("Create", inParams, null);
            uint returnValue = (uint)outParams["ReturnValue"];
            if (returnValue == 0)
            {
                // 命令执行成功
                string processId = outParams["ProcessId"].ToString();
                Debug.WriteLine(processId);
            }
            else
            {
                // 命令执行失败
                string errorMessage = outParams["Description"].ToString();
                Debug.WriteLine(errorMessage);
            }
        }
    }
}