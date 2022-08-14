using System.Net;
using Xunit;

namespace Netnr.Test
{
    public class TestNetnrIPQuery
    {
        [Fact]
        public void Query()
        {
            var address = IPAddress.Parse("114.44.227.87");

            using var reader = new MaxMind.Db.Reader(@"D:\tmp\res\GeoLite2\GeoLite2-Country.mmdb");
            var data = reader.Find<Dictionary<string, object>>(address);
            Debug.WriteLine(data);
        }
    }
}
