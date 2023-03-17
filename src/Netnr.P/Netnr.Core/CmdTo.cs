using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// 调用cmd
/// </summary>
public class CmdTo
{
    /// <summary>
    /// ProcessStartInfo
    /// </summary>
    /// <param name="Arguments">参数命令</param>
    /// <param name="FileName">执行程序，Windows默认cmd，Linux默认bash</param>
    /// <returns></returns>
    public static ProcessStartInfo PSInfo(string Arguments, string FileName = null)
    {
        var psi = new ProcessStartInfo
        {
            RedirectStandardOutput = true,  //由调用程序获取输出信息
            RedirectStandardError = true,   //重定向标准错误输出
            UseShellExecute = false,        //是否使用操作系统shell启动
            CreateNoWindow = true          //不显示程序窗口
        };

        if (string.IsNullOrWhiteSpace(FileName))
        {
            psi.FileName = GlobalTo.IsWindows ? "cmd.exe" : "bash";
            psi.Arguments = GlobalTo.IsWindows ? $"/C \"{Arguments}\"" : $"-c \"{Arguments}\"";
        }
        else
        {
            psi.FileName = FileName;
            psi.Arguments = Arguments;
        }

        return psi;
    }

    /// <summary>
    /// 执行（简单，获取标准输出和错误）
    /// </summary>
    /// <param name="Arguments">参数命令</param>
    /// <param name="FileName">执行程序，Windows默认cmd，Linux默认bash</param>
    /// <returns></returns>
    public static CliResult Execute(string Arguments, string FileName = null)
    {
        return Execute(PSInfo(Arguments, FileName), (process, cr) =>
        {
            process.Start();

            cr.CrOutput = process.StandardOutput.ReadToEnd();
            cr.CrError = process.StandardError.ReadToEnd();

            process.WaitForExit();
            process.Close();
        });
    }

    /// <summary>
    /// 执行（自定义）
    /// </summary>
    /// <param name="Arguments">参数命令</param>
    /// <param name="FileName">执行程序，Windows默认cmd，Linux默认bash</param>
    /// <param name="apc"></param>
    /// <returns></returns>
    public static CliResult Execute(string Arguments, string FileName, Action<Process, CliResult> apc)
    {
        return Execute(PSInfo(Arguments, FileName), apc);
    }

    private static CliResult Execute(Func<CliResult> action)
    {
        return action();
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="psi"></param>
    /// <param name="apc"></param>
    /// <returns></returns>
    public static CliResult Execute(ProcessStartInfo psi, Action<Process, CliResult> apc)
    {
        return Execute(() =>
        {
            var cr = new CliResult();

            using var process = new Process { StartInfo = psi };
            cr.CrProcess = process;

            //回调
            apc?.Invoke(process, cr);

            return cr;
        });
    }

    /// <summary>
    /// 根据端口号杀进程
    /// </summary>
    /// <param name="ports"></param>
    public static void KillProcess(int[] ports)
    {
        var listPortInfo = GetPortInfo();
        var listPid = listPortInfo.Where(x => ports.Contains(x.Port)).Select(x => x.ProcessId).Distinct().ToList();
        listPid.ForEach(pid =>
        {
            try
            {
                var proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        });
    }

    /// <summary>
    /// 获取端口信息
    /// </summary>
    /// <param name="isAll">全部，默认仅出监听 LISTEN</param>
    /// <returns></returns>
    public static List<PortInfo> GetPortInfo(bool isAll = false)
    {
        var result = new List<PortInfo>();

        if (GlobalTo.IsWindows)
        {
            var er = Execute("-ano", "netstat");
            var rows = Regex.Split(er.CrOutput, "\r\n");
            foreach (string row in rows)
            {
                var tokens = Regex.Split(row, "\\s+");
                if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                {
                    var state = tokens[4];
                    if (!isAll && state != "LISTENING")
                    {
                        continue;
                    }

                    var proto = tokens[1].ToLower();
                    var ipAndPort = tokens[2];
                    var ipv6AndPort = ipAndPort.Split("]:");

                    var pi = new PortInfo
                    {
                        Protocol = proto,
                        State = state,
                        ProcessId = Convert.ToInt32(tokens[proto == "udp" ? 4 : 5])
                    };
                    if (pi.State == "LISTENING")
                    {
                        pi.State = "LISTEN";
                    }

                    //ipv6
                    if (ipv6AndPort.Length == 2)
                    {
                        pi.IP = $"{ipv6AndPort[0].TrimStart('[')}";
                        pi.Port = Convert.ToInt32(ipv6AndPort[1]);
                        pi.Protocol += "6";
                    }
                    else
                    {
                        var ipv4AndPort = ipAndPort.Split(':');
                        pi.IP = ipv4AndPort[0];
                        pi.Port = Convert.ToInt32(ipv4AndPort[1]);
                    }

                    try
                    {
                        var procInfo = Process.GetProcessById(pi.ProcessId);
                        pi.ProcessName = procInfo.ProcessName;
                    }
                    catch (Exception) { }

                    result.Add(pi);
                }
            }
        }
        else
        {
            var er = Execute("-tunlp", "netstat");
            var rows = Regex.Split(er.CrOutput, "\n");
            foreach (string row in rows)
            {
                var tokens = Regex.Split(row, "\\s+");
                var proto = tokens[0];

                if ((proto.StartsWith("tcp") || proto.StartsWith("udp")) && tokens.Length > 7 && tokens[5] == "LISTEN")
                {
                    var state = tokens[5];
                    if (!isAll && state != "LISTEN")
                    {
                        continue;
                    }

                    var ipAndPort = tokens[3];
                    var ipsIndex = ipAndPort.LastIndexOf(':');
                    var pipn = tokens[6].Split('/').ToList();
                    var pi = new PortInfo
                    {
                        Port = Convert.ToInt32(ipAndPort.Substring(ipsIndex + 1)),
                        IP = ipAndPort.Substring(0, ipsIndex),
                        Protocol = proto,
                        ProcessId = Convert.ToInt32(pipn[0])
                    };
                    pipn.RemoveAt(0);
                    pi.ProcessName = string.Join("/", pipn);

                    result.Add(pi);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 端口信息
    /// </summary>
    public class PortInfo
    {
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// tcp udp tcp6 udp6
        /// </summary>
        public string Protocol { get; set; }
        /// <summary>
        /// 状态：LISTEN
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 进程ID
        /// </summary>
        public int ProcessId { get; set; }
    }

    /// <summary>
    /// 输出
    /// </summary>
    public class CliResult
    {
        /// <summary>
        /// 进程
        /// </summary>
        public Process CrProcess { get; set; }

        /// <summary>
        /// 标准输出
        /// </summary>
        public string CrOutput { get; set; }
        /// <summary>
        /// 错误输出
        /// </summary>
        public string CrError { get; set; }
    }
}