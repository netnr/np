using Xunit;
using System.Linq.Dynamic.Core;

namespace Netnr.Test
{
    public class TestIQueryable
    {
        [Fact]
        public void IQueryableWhere_1()
        {
            var list = new List<ValueTextVM>
            {
                new ValueTextVM{ Value="1",Text="3"},
                new ValueTextVM{ Value="2",Text="5"},
                new ValueTextVM{ Value="3",Text="7"},
                new ValueTextVM{ Value="4",Text="9"},
            };
            var v1 = list.AsQueryable().Where("Value == @0 || Value != @1", "2", "4").ToList();
            var v2 = list.AsQueryable().Where("Convert.ToInt32(Value) > @0", 2).ToList();
            var v3 = list.AsQueryable().Where("Convert.ToInt32(Value)*1.0/Convert.ToInt32(Text)*100 > @0", 35).ToList();

            Debug.WriteLine($"{v1.Count} {v2.Count} {v3.Count}");
        }

        [Fact]
        public void IQueryableWhere_2()
        {
            var now = DateTime.Now;
            var list = new List<TryModel>();
            var mo1 = new TryModel
            {
                Date1 = now,
                Date2 = now.AddDays(5),
                Text1 = Guid.NewGuid().ToString("N"),
                Int1 = Snowflake53To.Id()
            };
            var mo2 = new TryModel().ToDeepCopy(mo1);
            var mo3 = new TryModel().ToDeepCopy(mo1);
            mo3.Date2 = now.AddDays(3);
            list.Add(mo1);
            list.Add(mo2);
            list.Add(mo3);

            var v1 = list.AsQueryable().Where("(Date2 - Date1).TotalDays >= @0", 5).ToList();
            var v2 = list.AsQueryable().Where("(Date2 - DateTime.Now).TotalDays >= 4").ToList();

            Debug.WriteLine($"{v1.Count} {v2.Count}");
        }

        [Fact]
        public void IQueryableWhere_3()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("name","MySQL"),
                new KeyValuePair<string,string>("version","8.0.33"),
                new KeyValuePair<string,string>("version_2","109"),
                new KeyValuePair<string,string>("version_3","9"),
                new KeyValuePair<string,string>("account_expired: CQENT_PJTX","6"),
                new KeyValuePair<string,string>("account_expired: CQENT","2"),
            };

            var v0 = list.AsQueryable().Where("Key.StartsWith(\"account_expired:\") && Convert.ToDouble(Value) < 7").ToList();
            var v1 = list.AsQueryable().Where("Key == @0 || Key.StartsWith(@1)", "version", "version_").ToList();
            var v2 = list.AsQueryable().Where("Key.StartsWith(\"version_\") && Convert.ToInt32(Value) > 8").ToList();
            var v3 = list.AsQueryable().Where("!Key.Contains(\"version_\")").ToList();

            Debug.WriteLine($"{v0.Count} {v1.Count} {v2.Count} {v3.Count}");
        }

        public class TryModel
        {
            public DateTime Date1 { get; set; }
            public DateTime Date2 { get; set; }
            public string Text1 { get; set; }
            public long Int1 { get; set; }
        }
    }
}