using System.Runtime.InteropServices;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// 清理内存
    /// </summary>
    public class ClearMemoryService
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

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
                }
                else
                {
                    listProcesses.Add(Process.GetCurrentProcess());
                }

                DXService.Log($"开始清理 {listProcesses.Count} 个进程");
                for (int i = 0; i < listProcesses.Count; i++)
                {
                    var item = listProcesses[i];
                    try
                    {
                        DXService.Log($"清理 {item.Id,6} {item.ProcessName} , 进度: {i + 1}/{listProcesses.Count}");
                        _ = EmptyWorkingSet(item.Handle);
                    }
                    catch (Exception ex)
                    {
                        DXService.Log(ex.Message);
                    }
                }

                DXService.Log("Done!");
            }
            else
            {
                DXService.Log("Windows only");
            }
        }
    }
}
