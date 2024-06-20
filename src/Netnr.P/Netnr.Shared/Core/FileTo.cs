#if Full || Core

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Netnr;

/// <summary>
/// 文件
/// </summary>
public partial class FileTo
{
    /// <summary>
    /// 流写入
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="fileFullPath">文件完整物理路径</param>
    /// <param name="e">编码</param>
    /// <param name="isAppend">默认追加，false覆盖</param>
    public static void WriteText(string content, string fileFullPath, Encoding e, bool isAppend = true)
    {
        var dn = Path.GetDirectoryName(fileFullPath);
        //检测目录
        if (!Directory.Exists(dn))
        {
            Directory.CreateDirectory(dn);
        }

        //打开方式
        var fm = (!File.Exists(fileFullPath) || !isAppend) ? FileMode.Create : FileMode.Append;

        using var fs = new FileStream(fileFullPath, fm);
        //流写入
        using var sw = new StreamWriter(fs, e);
        sw.WriteLine(content);
        sw.Flush();
        sw.Close();
    }

    /// <summary>
    /// 写入
    /// </summary>
    /// <param name="content"></param>
    /// <param name="fileFullPath">文件完整物理路径</param>
    /// <param name="isAppend"></param>
    public static void WriteText(string content, string fileFullPath, bool isAppend = true)
    {
        WriteText(content, fileFullPath, Encoding.UTF8, isAppend);
    }

    /// <summary>
    /// 遍历目录及文件
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="handler"></param>
    public static void EachDirectory(string rootPath, Action<IEnumerable<DirectoryInfo>, IEnumerable<FileInfo>> handler)
    {
        var dir = new DirectoryInfo(rootPath);
        if (dir.Exists)
        {
            var subDir = dir.EnumerateDirectories();

            handler.Invoke(subDir, dir.EnumerateFiles()); //处理

            foreach (DirectoryInfo dd in subDir)
            {
                EachDirectory(dd.FullName, handler);
            }
        }
    }

    /// <summary>
    /// 拷贝目录
    /// </summary>
    /// <param name="fromSource">源目录</param>
    /// <param name="toTarget">新目录</param>
    /// <param name="ignoreFolder">忽略文件夹</param>
    public static void CopyDirectory(DirectoryInfo fromSource, DirectoryInfo toTarget, IList<string> ignoreFolder = null)
    {
        if (fromSource.FullName.Equals(toTarget.FullName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (ignoreFolder != null && ignoreFolder.Any(toTarget.Name.EndsWith))
        {
            return;
        }

        if (Directory.Exists(toTarget.FullName) == false)
        {
            Directory.CreateDirectory(toTarget.FullName);
        }

        fromSource.EnumerateFiles().ForEach(fi =>
        {
            fi.CopyTo(Path.Combine(toTarget.ToString(), fi.Name), true);
        });

        fromSource.EnumerateDirectories().ForEach(diSourceSubDir =>
        {
            if (ignoreFolder?.Any(x => x == diSourceSubDir.Name) != true)
            {
                DirectoryInfo nextTargetSubDir = toTarget.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir, ignoreFolder);
            }
        });
    }

    /// <summary>
    /// 拷贝目录
    /// </summary>
    /// <param name="fromSourceDirectory">源目录</param>
    /// <param name="toTargetDirectory">新目录</param>
    /// <param name="ignoreFolder">忽略文件夹</param>
    public static void CopyDirectory(string fromSourceDirectory, string toTargetDirectory, IList<string> ignoreFolder = null)
    {
        DirectoryInfo fromSource = new(fromSourceDirectory);
        DirectoryInfo toTarget = new(toTargetDirectory);

        CopyDirectory(fromSource, toTarget, ignoreFolder);
    }

    /// <summary>
    /// 清空文件夹下全部，保留文件夹
    /// </summary>
    /// <param name="directory"></param>
    public static void ClearDirectory(string directory)
    {
        DirectoryInfo di = new(directory);
        if (di.Exists)
        {
            di.EnumerateFiles().ForEach(fi => fi.Delete());
            di.EnumerateDirectories().ForEach(di => di.Delete(true));
        }
    }

    /// <summary>
    /// 是二进制（非文本）
    /// </summary>
    /// <param name="filePath">路径</param>
    /// <param name="consecutiveNul">连续Nul数量，默认1</param>
    /// <returns></returns>
    public static bool IsBinary(string filePath, int consecutiveNul = 1)
    {
        const int charsToCheck = 8000;
        const char nulChar = '\0';

        int nulCount = 0;

        using var streamReader = new StreamReader(filePath);
        for (var i = 0; i < charsToCheck; i++)
        {
            if (streamReader.EndOfStream)
                return false;

            if ((char)streamReader.Read() == nulChar)
            {
                if (++nulCount >= consecutiveNul)
                    return true;
            }
            else
            {
                nulCount = 0;
            }
        }

        return false;
    }

}

#endif