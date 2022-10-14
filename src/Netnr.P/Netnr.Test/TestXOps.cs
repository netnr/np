using Xunit;

namespace Netnr.Test
{
    public class TestXOps
    {
        [Fact]
        public async void HTTP()
        {
            var url = "https://www.baidu.com";
            var result = await MonitorTo.HTTP(url, HttpMethod.Get);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public void SSL()
        {
            var host = "www.baidu.com";
            var result = MonitorTo.SSL(host);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public async void DNS()
        {
            var host = "www.baidu.com";
            var result = await MonitorTo.DNS(host);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public void TCPort()
        {
            var host = "www.baidu.com";
            var result = MonitorTo.TCPort(host, 443);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public void PING()
        {
            var host = "www.baidu.com";
            var result = MonitorTo.PING(host);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }
    }
}