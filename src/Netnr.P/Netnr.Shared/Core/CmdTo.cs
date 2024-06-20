#if Full || Core

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;

namespace Netnr;

/// <summary>
/// 调用cmd
/// </summary>
public partial class CmdTo
{
    /// <summary>
    /// 是 Windows
    /// </summary>
    public static bool IsWindows { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// ProcessStartInfo
    /// </summary>
    /// <param name="Arguments">参数命令</param>
    /// <param name="FileName">执行程序，Windows默认cmd，Linux默认bash</param>
    /// <param name="outputEncoding">输出编码，默认 UTF8</param>
    /// <returns></returns>
    public static ProcessStartInfo BuildProcessStartInfo(string Arguments, string FileName = null, Encoding outputEncoding = null)
    {
        outputEncoding ??= Encoding.UTF8;

        var psi = new ProcessStartInfo
        {
            RedirectStandardOutput = true,  //由调用程序获取输出信息
            RedirectStandardError = true,   //重定向标准错误输出
            UseShellExecute = false,        //是否使用操作系统shell启动
            CreateNoWindow = true,          //不显示程序窗口

            StandardOutputEncoding = outputEncoding,
            StandardErrorEncoding = outputEncoding,
        };

        if (string.IsNullOrWhiteSpace(FileName))
        {
            psi.FileName = IsWindows ? "cmd.exe" : "bash";
            psi.Arguments = IsWindows ? $"/C \"{Arguments}\"" : $"-c \"{Arguments}\"";
        }
        else
        {
            psi.FileName = FileName;
            psi.Arguments = Arguments;
        }

        return psi;
    }

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="Arguments">参数命令</param>
    /// <param name="FileName">执行程序，Windows默认cmd，Linux默认bash</param>
    /// <param name="outputEncoding">输出编码，默认 UTF8</param>
    /// <returns></returns>
    public static Process BuildProcess(string Arguments, string FileName = null, Encoding outputEncoding = null)
    {
        var psi = BuildProcessStartInfo(Arguments, FileName, outputEncoding);
        var proc = new Process { StartInfo = psi };
        return proc;
    }

    /// <summary>
    /// 执行（简单，获取标准输出和错误）
    /// </summary>
    /// <param name="Arguments">参数命令</param>
    /// <param name="FileName">执行程序，Windows默认cmd，Linux默认bash</param>
    /// <param name="timeout">等待超时，默认不超时</param>
    /// <returns></returns>
    public static CliResult Execute(string Arguments, string FileName = null, TimeSpan? timeout = null)
    {
        var clir = new CliResult();

        var proc = BuildProcess(Arguments, FileName);
        clir.CrProcess = proc;

        proc.Start();

        clir.CrOutput = proc.StandardOutput.ReadToEnd();
        clir.CrError = proc.StandardError.ReadToEnd();

        if (timeout.HasValue)
        {
            proc.WaitForExit((int)timeout.Value.TotalMilliseconds);
        }
        else
        {
            proc.WaitForExit();
        }
        proc.Close();

        return clir;
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

#endif