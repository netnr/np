using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO.Compression;

namespace Netnr;

/// <summary>
/// 压缩
/// </summary>
public class ZipTo
{
    /// <summary>
    /// 编码
    /// </summary>
    public static Encoding encoding = Encoding.UTF8;

    /// <summary>
    /// 创建打包
    /// </summary>
    /// <param name="pathName">文件完整路径 : 包内文件名（可选）</param>
    /// <param name="zipPath">zip 完整路径，默认添加的第一个文件同目录下</param>
    public static string Create(Dictionary<string, string> pathName, string zipPath = null)
    {
        if (string.IsNullOrWhiteSpace(zipPath))
        {
            var dn = Path.GetDirectoryName(pathName.Keys.First());
            zipPath = Path.Combine(dn, Path.GetFileName(dn) + ".zip");
        }
        using ZipArchive zip = ZipFile.Open(zipPath, File.Exists(zipPath) ? ZipArchiveMode.Update : ZipArchiveMode.Create);

        foreach (var path in pathName.Keys)
        {
            var name = pathName[path];
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Path.GetFileName(path);
            }

            zip.CreateEntryFromFile(path, name);
        }

        return zipPath;
    }

    /// <summary>
    /// 创建打包
    /// </summary>
    /// <param name="fullPath">需打包的文件夹完整路径</param>
    /// <param name="zipPath">zip 完整路径，默认文件夹同目录</param>
    public static string Create(string fullPath, string zipPath = null)
    {
        if (string.IsNullOrWhiteSpace(zipPath))
        {
            zipPath = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath) + ".zip");
        }

        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }
        ZipFile.CreateFromDirectory(fullPath, zipPath, CompressionLevel.Optimal, false);

        return zipPath;
    }

    /// <summary>
    /// 解压提取
    /// </summary>
    /// <param name="zipPath">zip 完整路径</param>
    /// <param name="dirName">文件完整路径-包内文件名（可选）</param>
    public static string Extract(string zipPath, string dirName = null)
    {
        if (string.IsNullOrWhiteSpace(dirName))
        {
            dirName = zipPath.Replace(Path.GetExtension(zipPath), "");
        }

        ZipFile.ExtractToDirectory(zipPath, dirName);

        return dirName;
    }

    /// <summary>
    /// 文本压缩为 ZIP
    /// </summary>
    /// <param name="text"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static byte[] TextToZip(string text, string filename = "content.txt")
    {
        using var compressedFileStream = new MemoryStream();
        using var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false);

        var zipEntry = zipArchive.CreateEntry(filename);
        using var originalFileStream = new MemoryStream(encoding.GetBytes(text));
        using var zipEntryStream = zipEntry.Open();
        originalFileStream.CopyTo(zipEntryStream);

        return compressedFileStream.ToArray();
    }

    /// <summary>
    /// 文本压缩为 ZIP
    /// </summary>
    /// <param name="text"></param>
    /// <param name="savePath"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static void TextToZip(string text, string savePath, string filename = "content.txt")
    {
        var saveDir = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }
        File.WriteAllBytes(savePath, TextToZip(text, filename));
    }

    /// <summary>
    /// 解压文本
    /// </summary>
    /// <param name="zippedBuffer"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string ZipToText(byte[] zippedBuffer, string filename = "content.txt")
    {
        using var zippedStream = new MemoryStream(zippedBuffer);
        using var archive = new ZipArchive(zippedStream);

        var entry = archive.Entries.FirstOrDefault(x => x.Name.EndsWith(filename)) ?? archive.Entries.FirstOrDefault();
        using var unzippedEntryStream = entry.Open();
        using var ms = new MemoryStream();
        unzippedEntryStream.CopyTo(ms);
        var unzippedArray = ms.ToArray();

        var text = encoding.GetString(unzippedArray);
        return text;
    }
}
