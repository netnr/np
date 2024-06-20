using Xunit;

namespace Netnr.Test
{
    /// <summary>
    /// HttpClient
    /// </summary>
    public class TestHttpClient
    {
        [Fact]
        public async Task HttpClient_1_GET()
        {
            //https://stackoverflow.com/questions/12553277
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All,

                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            
            var hc = new HttpClient(handler);
            hc.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip");

            var resp = await hc.GetAsync("https://zme.ink");

            Debug.WriteLine(resp.IsSuccessStatusCode);
            if (resp.IsSuccessStatusCode)
            {
                var read = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine(read);
            }
        }

        [Fact]
        public async Task HttpClient_2_GBK()
        {
            BaseTo.ReadyEncoding();

            var hc = new HttpClient();
            hc.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");

            var url = $"https://whois.west.cn/www.baidu.com";
            var stream = await hc.GetStreamAsync(url);
            var html = await new StreamReader(stream, Encoding.GetEncoding("gbk")).ReadToEndAsync();

            Debug.WriteLine(html);
        }

        [Fact]
        public async Task HttpClient_3_POST()
        {
            var hc = new HttpClient();

            var content = new StringContent(new { keyword = "search" }.ToJson(), Encoding.UTF8, "application/json");
            var resp = await hc.PostAsync("https://httpbin.org/post", content);

            Debug.WriteLine(resp.IsSuccessStatusCode);
            if (resp.IsSuccessStatusCode)
            {
                var read = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine(read);
            }
        }

        [Fact]
        public async Task HttpClient_4_POST()
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
        public async Task HttpClient_5_POST()
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

        [Fact]
        public async Task HttpClient_6_Download()
        {
            var url = "https://github.com/netnr/np/releases/download/dist/ndx-1.0.2-win-x64.zip";
            var savePath = @"D:\tmp\res\try\download.bin";

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            await client.DownloadAsync(url, savePath, (rlen, tlen) =>
            {
                Debug.WriteLine($"{rlen}/{tlen} {rlen * 1.0 / tlen * 100:F2}%");
            });
        }

        [Fact]
        public async Task HttpClient_7_EnsureSuccess()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://zme.ink/README"); // 这一步可能会抛出异常

            // 响应状态码不在 200-299，且抛出异常
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(responseBody);
        }

        [Fact]
        public async Task HttpClient_8_IsSuccess()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://zme.ink/README"); // 这一步可能会抛出异常

            // 响应状态码在 200-299，且不抛出异常
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseBody);
            }
            else
            {
                Debug.WriteLine($"请求失败: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }

        [Fact]
        public async Task HttpClient_9()
        {
            var url = "https://static.centbrowser.cn/win_stable/5.0.1002.354/centbrowser_5.0.1002.354_x64_portable.exe";
            var savePath = @"D:\tmp\res\try\download.bin";

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            await client.DownloadAsync(url, savePath, (rlen, tlen) =>
            {
                Debug.WriteLine($"{rlen}/{tlen} {rlen * 1.0 / tlen * 100:F2}%");
            });
        }

    }
}
