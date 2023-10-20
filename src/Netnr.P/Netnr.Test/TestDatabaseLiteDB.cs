using LiteDB;
using Xunit;

namespace Netnr.Test
{
    /// <summary>
    /// https://github.com/mbdavid/LiteDB
    /// </summary>
    public class TestDatabaseLiteDB
    {
        /*
         * 
         */

        [Fact]
        public void LiteDB_1()
        {
            // Open database (or create if doesn't exist)
            using var db = new LiteDatabase("D:/tmp/res/lite.db");
            // Get customer collection
            var col = db.GetCollection<Customer>("customers");

            // Create unique index in Name field
            col.EnsureIndex(x => x.Name, true);

            // Create your new customer instance
            var customer = new Customer
            {
                Name = "John Doe",
                Phones = new string[] { "8000-0000", "9000-0000" },
                Age = 39,
                IsActive = true
            };

            // Insert new customer document (Id will be auto-incremented)
            var v1 = col.Insert(customer);

            // Update a document inside a collection
            customer.Name = "Joana Doe";

            var v2 = col.Update(customer);

            // Use LINQ to query documents (with no index)
            var results = col.Find(x => x.Age > 20);
        }

        [Fact]
        public void LiteDB_2()
        {
            // Open database (or create if doesn't exist)
            using var db = new LiteDatabase("D:/tmp/res/lite.db");
            // Get customer collection
            var col = db.GetCollection<Customer>("customers");

            // Create unique index in Name field
            col.EnsureIndex(x => x.Name, true);

            var r1 = col.Count();
            Debug.WriteLine(r1);

            var list = new List<Customer>();
            for (int i = 0; i < 999_999; i++)
            {
                var customer = new Customer
                {
                    Name = Ulid.NewUlid().ToString(),
                    Phones = new string[] { Ulid.NewUlid().ToString(), Ulid.NewUlid().ToString() },
                    Birthday = DateTime.Now,
                    Age = Snowflake53To.Id(),
                    Remark = Ulid.NewUlid().ToString(),
                };
                list.Add(customer);
            }

            var st = Stopwatch.StartNew();
            var v1 = col.InsertBulk(list, list.Count);
            Debug.WriteLine($"{v1} {st.Elapsed}");

            // Use LINQ to query documents (with no index)
            var results = col.Find(x => x.Age > 20);
            Debug.WriteLine(results.Count());
        }
    }

    // Create your POCO class
    public class Customer
    {
        public string Name { get; set; }
        public long Age { get; set; }
        public DateTime Birthday { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
        public string Remark { get; set; }
    }
}