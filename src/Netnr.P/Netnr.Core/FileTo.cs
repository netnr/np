using System.IO;
using System.Text;

namespace Netnr.Core
{
    /// <summary>
    /// 文件读写
    /// </summary>
    public class FileTo
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
        /// 读取
        /// </summary>
        /// <param name="fileFullPath">文件完整物理路径</param>
        /// <param name="e">编码 默认UTF8</param>
        /// <returns></returns>
        public static string ReadText(string fileFullPath, Encoding e = null)
        {
            var result = string.Empty;

            if (File.Exists(fileFullPath))
            {
                if (e == null)
                {
                    e = Encoding.UTF8;
                }

                result = File.ReadAllText(fileFullPath, e);
            }

            return result;
        }
    }
}