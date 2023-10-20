using HashidsNet;
using Xunit;

namespace Netnr.Test
{
    public class TestUnique
    {
        [Fact]
        public void Ulid_1()
        {
            var a1 = Ulid.NewUlid();
            Debug.WriteLine(a1);

            var a2 = Ulid.NewUlid();
            Debug.WriteLine(a2);

            var a3 = BitConverter.ToUInt64(Ulid.NewUlid().ToByteArray(), 0);
            Debug.WriteLine(a3);

            var a4 = BitConverter.ToUInt64(Ulid.NewUlid().ToByteArray(), 0);
            Debug.WriteLine(a4);
        }

        [Fact]
        public void HashIds_1()
        {
            var hashids = new Hashids("this is my salt");

            var h1 = hashids.Encode(12345);
            Debug.WriteLine(h1);

            var h2 = hashids.EncodeLong(Snowflake53To.Id());
            Debug.WriteLine(h2);
        }

        [Fact]
        public void GuidToLong_1()
        {
            var sw = Stopwatch.StartNew();
            var hs = new HashSet<long>();
            for (int i = 0; i < 1_999_999; i++)
            {
                var val = Guid.NewGuid().ToLong();
                if (!hs.Add(val))
                {
                    throw new Exception("same");
                }
            }
            Debug.WriteLine($"Guid ToLong 1_999_999: {sw.Elapsed}");
        }

        [Fact]
        public void SnowflakeTo_1()
        {
            var sw = Stopwatch.StartNew();

            var hs = new HashSet<long>();
            for (int i = 0; i < 2_999_999; i++)
            {
                var val = SnowflakeTo.Id();
                if (!hs.Add(val))
                {
                    throw new Exception("same");
                }
            }
            Debug.WriteLine($"Snowflake 2_999_999: {sw.Elapsed}");
        }

        [Fact]
        public void Snowflake53To_2()
        {
            var now = DateTime.Now;
            var d1 = Snowflake53To.GetId(now);

            var sw = Stopwatch.StartNew();

            var hs = new HashSet<long>();
            for (int i = 0; i < 2_999_999; i++)
            {
                var val = Snowflake53To.Id();

                if (!hs.Add(val))
                {
                    throw new Exception($"same，count：{hs.Count}，time：{sw.Elapsed}");
                }
            }
            sw.Stop();

            var d2 = Snowflake53To.GetId(DateTime.UtcNow);

            Debug.WriteLine($"d1 {d1}\r\nh1 {hs.First()}\r\nhn {hs.Last()}\r\nd2 {d2}");
            Debug.WriteLine($"Snowflake53 2_999_999: {sw.Elapsed}");
        }

        [Fact]
        public void Snowflake53To_3()
        {
            var now = DateTime.Now;
            var d1 = Snowflake53To.GetId(now);
            Debug.WriteLine(d1);

            var d2 = Snowflake53To.GetId(DateTime.Parse("2023-08-16 20:04:02.113"));
            Debug.WriteLine(d2);
            var t1 = Snowflake53To.Parse(d2);
            Debug.WriteLine(t1);
        }

        [Fact]
        public void Snowflake53To_4()
        {
            var hsId = new HashSet<long>();
            """
                2023-08-17 14:31:08.750
                2023-08-17 14:31:09.503
                2023-09-13 10:40:14.850
                """.Split('\n').ForEach(item =>
            {
                if (!string.IsNullOrWhiteSpace(item) && DateTime.TryParse(item, out DateTime time))
                {
                    var id = Snowflake53To.GetId(time);
                    while (!hsId.Add(id))
                    {
                        id++;
                    }
                    Debug.WriteLine(id);
                }
            });
        }

        [Fact]
        public void Snowflake53To_5()
        {
            var st = Stopwatch.StartNew();
            long count = 0;
            while (true)
            {
                Snowflake53To.Id();
                count++;
                if (st.ElapsedMilliseconds > 1000)
                {
                    Debug.WriteLine($"{Snowflake53To.Id()} 已生成 {count} 次");
                    st.Restart();

                    Thread.Sleep(10);
                }
            }
        }
    }

}