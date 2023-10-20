using System.Net;
using Xunit;

namespace Netnr.Test
{
    public class TestNetnrIPQuery
    {
        [Fact]
        public void Query()
        {
            BaseTo.ReadyEncoding();

            var ipq = new IPQuery(@"D:\tmp\res\cz\qqwry.dat", @"D:\tmp\res\cz\ipv6wry.db");

            new string[] {
                "42.201.45.10", "36.132.150.90", "61.186.154.83",
                "39.160.170.17", "125.86.94.191", "114.44.227.87",
                "152.69.198.201", "110.165.32.0", "183.182.31.255",
                "122.100.240.17", "63.221.138.13", "104.21.14.7",
                "183.230.11.188", "42.193.14.202", "202.99.231.255",
                "203.107.45.167",  "1.10.10.255", "76.76.21.164",
                "240e:330:18a1:f300:f878:a33f:bfca:f5b0",
                "2603:c023:8:57e:d6fb:bebc:8c17:4b74",
                "2408:8764::1:97", "fec0:0:2:1::1",
            }.ForEach(ip =>
            {
                var result = ipq.Search(ip);
                if (result != null)
                {
                    Debug.WriteLine($"{ip} \t Addr: {result.Addr} ,ISP: {result.ISP}");
                }
            });
        }

        [Fact]
        public void MaxMind()
        {
            var address = IPAddress.Parse("114.44.227.87");

            using var reader = new MaxMind.Db.Reader(@"D:\tmp\res\GeoLite2\GeoLite2-Country.mmdb");
            var data = reader.Find<Dictionary<string, object>>(address);
            Debug.WriteLine(data);
        }
    }
}
