using Xunit;

namespace Netnr.Test
{
    /// <summary>
    /// HttpClient
    /// </summary>
    public class TestHttpClient
    {
        [Fact]
        public async void HttpClient_1_GET()
        {
            //https://stackoverflow.com/questions/12553277
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var hc = new HttpClient(handler);

            var resp = await hc.GetAsync("https://httpbin.org/get");

            Debug.WriteLine(resp.IsSuccessStatusCode);
            if (resp.IsSuccessStatusCode)
            {
                var read = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine(read);
            }
        }


        [Fact]
        public async void HttpClient_2_GBK()
        {
            ReadyTo.EncodingReg();

            var hc = new HttpClient();
            hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");

            var url = $"https://whois.west.cn/www.baidu.com";
            var stream = await hc.GetStreamAsync(url);
            var html = await new StreamReader(stream, Encoding.GetEncoding("gbk")).ReadToEndAsync();

            Debug.WriteLine(html);
        }

        [Fact]
        public async void HttpClient_3_POST()
        {
            var hc = new HttpClient();

            var content = new StringContent("a=1&b=x");
            var resp = await hc.PostAsync("https://httpbin.org/post", content);

            Debug.WriteLine(resp.IsSuccessStatusCode);
            if (resp.IsSuccessStatusCode)
            {
                var read = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine(read);
            }
        }

        [Fact]
        public async void HttpClient_4_POST()
        {
            var hc = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"a","1" },
                {"b","x" },
            });
            var resp = await hc.PostAsync("https://httpbin.org/post", content);

            Debug.WriteLine(resp.IsSuccessStatusCode);
            if (resp.IsSuccessStatusCode)
            {
                var read = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine(read);
            }
        }

        [Fact]
        public async void HttpClient_5_POST()
        {
            var hc = new HttpClient();
            var content = new MultipartFormDataContent
            {
                { new StreamContent(File.OpenRead("/abc/note.txt")), "file", "note.txt" },
                { new StringContent("value"), "key" }
            };
            var resp = await hc.PostAsync("https://httpbin.org/post", content);

            Debug.WriteLine(resp.IsSuccessStatusCode);
            if (resp.IsSuccessStatusCode)
            {
                var read = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine(read);
            }
        }
    }
}
