using System.Net;
using Xunit;

namespace Netnr.Test
{
    public class TestNetnrIPQuery
    {
        [Fact]
        public void Query()
        {
            var address = IPAddress.Parse("202.97.96.2");

            using (var reader = new MaxMind.Db.Reader(@"D:\tmp\res\dbip\dbip-asn.mmdb"))
            {
                var data = reader.Find<Dictionary<string, object>>(address);
                Debug.WriteLine(data);
            }

            using (var reader = new MaxMind.Db.Reader(@"D:\tmp\res\dbip\dbip-city.mmdb"))
            {
                var data = reader.Find<Dictionary<string, object>>(address);
                Debug.WriteLine(data);
            }
        }
    }
}
