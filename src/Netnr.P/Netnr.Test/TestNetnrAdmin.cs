using Xunit;

namespace Netnr.Test
{
    public class TestNetnrAdmin
    {
        [Fact]
        public async void Fast()
        {
            var host = "https://localhost:9931";
            var tableName = "test_table";

            var hc = new HttpClient();

            Debug.WriteLine($"删除表 {tableName}");
            var resp1 = await hc.GetAsync($"{host}/Fast/RemoveTable?tableName={tableName}");
            RespWrite(resp1);

            Debug.WriteLine($"创建表 {tableName}");
            var resp2 = await hc.GetAsync($"{host}/Fast/CreateTable?tableName={tableName}&tableComment=Test");
            RespWrite(resp2);

            Debug.WriteLine("Fast Done!");
        }

        private static async void RespWrite(HttpResponseMessage resp)
        {
            Assert.True(resp.StatusCode == System.Net.HttpStatusCode.OK);
            var result = await resp.Content.ReadAsStringAsync();
            Debug.WriteLine(result.DeJson().ToJson(true));
        }
    }
}