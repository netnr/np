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
        /// <param name="path">物理目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="e">编码</param>
        /// <param name="isAppend">默认追加，false覆盖</param>
        public static void WriteText(string content, string path, string fileName, Encoding e, bool isAppend = true)
        {
            //检测目录
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fullpath = Path.Combine(path, fileName);

            //打开方式
            var fm = (!File.Exists(fullpath) || !isAppend) ? FileMode.Create : FileMode.Append;

            using var fs = new FileStream(fullpath, fm);
            //流写入
            using var sw = new StreamWriter(fs, e);
            sw.WriteLine(content);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="isAppend"></param>
        public static void WriteText(string content, string path, string fileName, bool isAppend = true)
        {
            WriteText(content, path, fileName, Encoding.UTF8, isAppend);
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="path">物理目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="e">编码 默认UTF8</param>
        /// <returns></returns>
        public static string ReadText(string path, string fileName, Encoding e = null)
        {
            var result = string.Empty;

            var fullpath = Path.Combine(path, fileName);

            if (File.Exists(fullpath))
            {
                if (e == null)
                {
                    e = Encoding.UTF8;
                }

                result = File.ReadAllText(fullpath, e);
            }

            return result;
        }
    }
}