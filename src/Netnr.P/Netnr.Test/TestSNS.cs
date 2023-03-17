using Xunit;

namespace Netnr.Test
{
    public class TestSNS
    {
        [Fact]
        public void Tmp()
        {
            Debug.WriteLine("tmp");

            var sw = Stopwatch.StartNew();
            var filePath = @"D:\tmp\txt\《西游记》.txt";
            
            var tm = new TextMiningTo();
            tm.FromFile(filePath);

            Debug.WriteLine(sw.Elapsed);
            Debug.WriteLine(tm.TopItems().ToJson());
        }
    }
}