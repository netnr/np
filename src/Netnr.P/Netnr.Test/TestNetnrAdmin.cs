using Xunit;

namespace Netnr.Test
{
    public class TestNetnrAdmin
    {
        [Fact]
        public async void FastDataFake()
        {
            var host = "https://localhost:9931";

            var hc = new HttpClient();

            for (int i = 0; i < 3; i++)
            {
                var tableName = $"fake_{RandomTo.NewNumber()}";

                Debug.WriteLine($"{i + 1} 创建表 {tableName}");
                var dic2 = new Dictionary<string, string>()
                {
                    { "tableName", tableName },
                    { "tableComment", $"{tableName} 备注" }
                };
                var resp2 = await hc.PostAsync($"{host}/api/Fast/FastTablePost", new FormUrlEncodedContent(dic2));
                await RespWrite(resp2);

                var listCol3 = new List<string> { $"col_string_{RandomTo.NewNumber()}:String:字符串", $"col_number_{RandomTo.NewNumber()}:Integer:分组1", $"col_number_{RandomTo.NewNumber()}:Integer:序号2", $"col_number_{RandomTo.NewNumber()}:Integer:整型", $"col_decimal_{RandomTo.NewNumber()}:Decimal:小数", $"col_date_{RandomTo.NewNumber()}:Date:时间", $"col_text_{RandomTo.NewNumber()}:Text:文本" };
                await Parallel.ForEachAsync(listCol3, async (item, i) =>
                {
                    var cols = item.Split(':');
                    var fieldTitle = cols[2] + cols[0].Split('_').Last();
                    Debug.WriteLine($"创建列 {cols[0]}");
                    var dic3 = new Dictionary<string, string>()
                    {
                        { "tableName", tableName },
                        { "fieldAlias", cols[0] },
                        { "fieldType", cols[1]},
                        { "fieldTitle", fieldTitle }
                    };
                    var resp3 = await hc.PostAsync($"{host}/api/Fast/FastTableColumnPost_Lock", new FormUrlEncodedContent(dic3), i);
                    await RespWrite(resp3);
                });

                Debug.WriteLine($"表操作 {tableName} 新增");
                var now = DateTime.Now;
                var listAdd4 = new List<object>();
                var rowCount = RandomTo.Instance.Next(40, 99);
                for (int r = 0; r < rowCount; r++)
                {
                    var body = new Dictionary<string, object>();
                    listCol3.ForEach(item =>
                    {
                        var cols = item.Split(':');
                        var col = cols[0];
                        var newTime = now.AddDays(RandomTo.Instance.Next(999));
                        switch (cols[1])
                        {
                            case "Integer":
                                if (cols[2].StartsWith("分组"))
                                {
                                    body.Add(col, now.AddDays(RandomTo.Instance.Next(5)).ToTimestamp());
                                }
                                else if (cols[2].StartsWith("序号"))
                                {
                                    body.Add(col, r + 1);
                                }
                                else
                                {
                                    body.Add(col, newTime.ToTimestamp(true));
                                }
                                break;
                            case "Decimal":
                                body.Add(col, new decimal(RandomTo.Instance.NextDouble()));
                                break;
                            case "Date":
                                body.Add(col, newTime);
                                break;
                            default:
                                body.Add(col, $"{SnowflakeTo.Id()}-{Guid.NewGuid()}-<b>B<b><em>EM</em>");
                                break;
                        }
                    });

                    listAdd4.Add(new
                    {
                        method = "POST",
                        path = tableName,
                        body
                    });
                }
                var dic4 = new Dictionary<string, string> { { "rowsJson", listAdd4.ToJson() } };
                var resp4 = await hc.PostAsync($"{host}/api/Fast/FastTableDataBatch", new FormUrlEncodedContent(dic4));
                await RespWrite(resp4);
            }

            Debug.WriteLine("Fast Done!");
        }

        private static async Task<JsonElement> RespWrite(HttpResponseMessage resp)
        {
            Assert.True(resp.StatusCode == System.Net.HttpStatusCode.OK);
            var result = await resp.Content.ReadAsStringAsync();
            var resObj = result.DeJson();
            Debug.WriteLine(resObj.ToJson(true));
            Assert.True(resObj.GetValue<int>("code") == 200);
            return resObj;
        }
    }
}