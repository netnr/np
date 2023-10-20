using System.Runtime.InteropServices;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// 清理内存
    /// </summary>
    public partial class CleanUpService
    {
        [LibraryImport("psapi.dll")]
        private static partial int EmptyWorkingSet(IntPtr hwProc);

        /// <summary>
        /// 清理内存
        /// </summary>
        /// <param name="isAll">清理全部</param>
        public static void CleanUp(bool isAll = true)
        {
            if (CmdTo.IsWindows)
            {
                var listProcesses = new List<Process>();
                if (isAll)
                {
                    listProcesses.AddRange(Process.GetProcesses());
                    listProcesses = listProcesses.Where(x => x.Id > 100).ToList();
                }
                else
                {
                    listProcesses.Add(Process.GetCurrentProcess());
                }

                for (int i = 0; i < listProcesses.Count; i++)
                {
                    var item = listProcesses[i];
                    var msg = $"{i + 1,4} / {listProcesses.Count} cleanup {item.Id,6} / {item.ProcessName}";
                    try
                    {
                        _ = EmptyWorkingSet(item.Handle);
                        ConsoleTo.LogColor(msg);
                    }
                    catch (Exception ex)
                    {
                        ConsoleTo.LogColor($"{msg} {ex.Message}", ConsoleColor.Red);
                    }
                }

                ConsoleTo.LogColor("Done!");
            }
            else
            {
                ConsoleTo.LogColor("Windows only");
            }
        }
    }
}
