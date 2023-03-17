using Xunit;

namespace Netnr.Test
{
    /// <summary>
    /// HttpClient
    /// </summary>
    public class TestFileWatch
    {
        [Fact]
        public void Changed()
        {
            var watch = new FileSystemWatcher
            {
                Path = @"D:\tmp\txt",
                Filter = "sns.txt",
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
            };
            watch.Changed += (s, e) =>
            {
                Debug.WriteLine(e.FullPath);
            };
            watch.EnableRaisingEvents = true;
        }

        [Fact]
        public void LoopRead()
        {
            using var fs = new FileStream(@"D:\tmp\txt\sns.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            sr.BaseStream.Seek(fs.Length, SeekOrigin.Begin);

            while (true)
            {
                //小于等于全文
                if (fs.Length < fs.Position)
                {
                    fs.Position = fs.Length;
                }

                // 实时读取文件内容
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }

                Thread.Sleep(500);
            }
        }
    }
}
