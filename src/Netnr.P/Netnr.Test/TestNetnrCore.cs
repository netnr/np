using Xunit;

namespace Netnr.Test
{
    public class TestNetnrCore
    {
        [Fact]
        public void CacheTo_1()
        {
            CacheTo.Set("abc", 1);
            Debug.WriteLine(CacheTo.Get<int>("abc"));
            Debug.WriteLine(CacheTo.Get<int>("abcd"));
            CacheTo.Set("abc", DateTime.Now);
            Debug.WriteLine(CacheTo.Get<DateTime>("abc"));
            CacheTo.Set("abc", new ResultVM());
            Debug.WriteLine(CacheTo.Get<ResultVM>("abc"));
            Debug.WriteLine(CacheTo.Get<ResultVM>("abcd"));

            CacheTo.Set("k1", DateTime.Now, 1, false);
            Debug.WriteLine(CacheTo.Get<DateTime>("k1"));
            Thread.Sleep(1000);
            Debug.WriteLine(CacheTo.Get<DateTime?>("k1"));
        }

        [Fact]
        public void CaclTo_1_AES()
        {
            var txt1 = "text1";
            var key1 = "key1";

            var val1 = CalcTo.AESEncrypt(txt1, key1);
            var txt2 = CalcTo.AESDecrypt(val1, key1);

            Assert.Equal(txt1, txt2);

            var v1 = CalcTo.AESDecrypt("ObQj0pVbF5lUu8lKGIhMqw==", "");
            Debug.WriteLine(v1);

            var key2 = "bad key";
            try
            {
                CalcTo.AESDecrypt(val1, key2);
            }
            catch (Exception ex)
            {
                key2 = ex.Message;
            }
            Assert.NotEqual("bad key", key2);
        }

        [Fact]
        public void CaclTo_2_AES()
        {
            var txt1 = "text1";

            var val1 = CalcTo.AESEncrypt(txt1, CalcTo.AESBuild("key 123"));
            var txt2 = CalcTo.AESDecrypt(val1, CalcTo.AESBuild("key 123"));

            Assert.Equal(txt1, txt2);
        }

        [Fact]
        public void CaclTo_1_DES()
        {
            var txt1 = "text1";
            var key1 = "key1";

            var val1 = CalcTo.DESEncrypt(txt1, key1);
            var txt2 = CalcTo.DESDecrypt(val1, key1);

            Assert.Equal(txt1, txt2);

            var key2 = "bad key";
            try
            {
                CalcTo.DESDecrypt(val1, key2);
            }
            catch (Exception ex)
            {
                key2 = ex.Message;
            }
            Assert.NotEqual("bad key", key2);
        }

        [Fact]
        public void CaclTo_2_DES()
        {
            var txt1 = "text1";

            var val1 = CalcTo.DESEncrypt(txt1, CalcTo.DESBuild("key 123", "iv 456"));
            var txt2 = CalcTo.DESDecrypt(val1, CalcTo.DESBuild("key 123", "iv 456"));

            Assert.Equal(txt1, txt2);
        }

        [Fact]
        public void CaclTo_1_HashBase64()
        {
            var txt1 = "text1";

            var val1 = CalcTo.HashBase64(CalcTo.HashType.MD5, txt1.ToStream());

            //https://approsto.com/md5-generator/
            Assert.Equal("zvfM2J2s8c7W9eyR11mVPw==", val1);
        }

        [Fact]
        public void CaclTo_2_HashString()
        {
            var txt1 = "text1";
            Assert.Equal("cef7ccd89dacf1ced6f5ec91d759953f", CalcTo.MD5(txt1));
            Assert.Equal("38e9be6401e62f7d1555230d9df956fb85c2ba6e", CalcTo.SHA_1(txt1));
            Assert.Equal("fe8df1a5a1980493ca9406ad3bb0e41297d979d90165a181fb39a5616a1c0789", CalcTo.SHA_256(txt1));
            Assert.Equal("94cdfd2332d260a102e4b5541503f9ff82d1e7136a2f720fea018816dbb7c62eb761ef050a83fd44971d667483080fdf", CalcTo.SHA_384(txt1));
            Assert.Equal("b5534aa935bcb3749cbd0846c636ef2dca0d4119225b731b03336d51fd765a81ff0d9c19ed31ccc6dc6b6fc2826ee0eac29e7115330566e2088e3c2ee9155b4e", CalcTo.SHA_512(txt1));

            var key1 = "key1";
            Assert.Equal("9f04de3535fa2e64924083003b8b16c8a84ef0ee", CalcTo.HMAC_SHA1(txt1, key1));
            Assert.Equal("cd1e288d14bfafa334ccb7f4394d9f524726a6c565aa548a9483155c89dd0e8c", CalcTo.HMAC_SHA256(txt1, key1));
            Assert.Equal("eeea03bda39b6c6441b4baab8c644832fcd48ff269b682b53a2407249f54ce5ebedab1c325198c464357280349f0b101", CalcTo.HMAC_SHA384(txt1, key1));
            Assert.Equal("fceeb5fa9d2107da9e35a9b068d0b4f38020b63ef56810d8e042c5e59d8deb9810232f8fdc61772b485318f9a0261a763ae6979442687ceba8aefe7d372f365a", CalcTo.HMAC_SHA512(txt1, key1));
            Assert.Equal("cb3b4a1be8c7c2963c4a2475c0837466", CalcTo.HMAC_MD5(txt1, key1));
        }

        [Fact]
        public void CaclTo_1_RSA()
        {
            CalcTo.RSAKey(out string xmlPrivateKey, out string xmlPublicKey, 2048);
            Assert.NotEmpty(xmlPrivateKey);
            Assert.NotEmpty(xmlPublicKey);

            var content1 = "abc 123 !@#";
            var content2 = CalcTo.DefaultEncoding.GetBytes(content1);

            var resultEncrypt1 = CalcTo.RSAEncrypt(xmlPublicKey, content1);
            var resultEncrypt2 = CalcTo.RSAEncrypt(xmlPublicKey, content2);
            Assert.NotEmpty(resultEncrypt1);
            Assert.NotEmpty(resultEncrypt2);

            var resultDecrypt1 = CalcTo.RSADecrypt(xmlPrivateKey, resultEncrypt1);
            var resultDecrypt2 = CalcTo.RSADecrypt(xmlPrivateKey, resultEncrypt2);
            Assert.Equal(content1, resultDecrypt1);
            Assert.Equal(content1, resultDecrypt2);
        }

        [Fact]
        public void CmdTo_1()
        {
            if (CmdTo.IsWindows)
            {
                var val1 = CmdTo.Execute("ver");
                Assert.Empty(val1.CrError);
                Assert.Contains("Microsoft Windows", val1.CrOutput);
            }
            else
            {
                var val1 = CmdTo.Execute("uname -a");
                Assert.Empty(val1.CrError);
                Assert.Contains("Linux", val1.CrOutput);
            }
        }

        [Fact]
        public void CmdTo_2()
        {
            var proc = CmdTo.BuildProcess("-V", @"curl");
            proc.Start();

            var result = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            proc.Close();

            Assert.StartsWith("curl", result);
        }

        [Fact]
        public void CmdTo_3_FFmpeg()
        {
            var result = string.Empty;

            var proc = CmdTo.BuildProcess(@"-i", "ffmpeg");
            proc.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            //错误信息输出
            proc.ErrorDataReceived += (sender, output) =>
            {
                if (output.Data != null)
                {
                    result += output.Data;
                }
            };

            proc.Start();//启动线程

            proc.BeginErrorReadLine();//开始异步读取错误信息
            proc.WaitForExit();//阻塞等待进程结束

            proc.Close();//关闭进程
            proc.Dispose();

            Assert.StartsWith("ffmpeg", result);
        }

        [Fact]
        public void ConsoleTo_1()
        {
            var i = 0;
            try
            {
                var j = 0 / i;
            }
            catch (Exception ex)
            {
                ConsoleTo.LogError(ex);
            }

            var now = DateTime.Now;
            var filename = $"console_{now:yyyyMMdd}.log";

            var path = Path.Combine(AppContext.BaseDirectory, "logs", now.Year.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fullPath = Path.Combine(path, filename);

            Assert.True(File.Exists(fullPath));
        }

        [Fact]
        public void Extensions_0_Xml()
        {
            var v1 =
@"
<xml>
  <ToUserName><![CDATA[toUser]]></ToUserName>
  <FromUserName><![CDATA[FromUser]]></FromUserName>
  <CreateTime>123456789</CreateTime>
  <MsgType><![CDATA[event]]></MsgType>
  <Event><![CDATA[subscribe]]></Event>
</xml>
";
            var doc = v1.DeXml();
            Assert.True(doc.GetValue("/xml/ToUserName") == "toUser");
            Assert.True(doc.SelectSingleNode("/xml/FromUserName").InnerText == "FromUser");
        }

        [Fact]
        public void Extensions_1_Table_List()
        {
            var list = new List<ResultVM>()
            {
                new(),
                new()
            };

            var dt1 = list.ToDataTable();
            Assert.Equal(2, dt1.Rows.Count);

            var list2 = dt1.ToModel<ResultVM>();
            Assert.Equal(2, list2.Count);
        }

        [Fact]
        public void Extensions_2_Escape()
        {
            var passwd = "' or ''='";
            var sql = $"select 1 from user where user='admin' and passwd='{passwd.OfSql()}'";
            Assert.DoesNotContain("or ''=''", sql);
        }

        [Fact]
        public void Extensions_3_Encode_Decode()
        {
            var key1 = "一二三";
            var val1 = key1.ToUrlEncode();
            Assert.Equal("%E4%B8%80%E4%BA%8C%E4%B8%89", val1);
            Assert.Equal(key1, val1.ToUrlDecode());

            var key2 = "<div>1</div>";
            var val2 = key2.ToHtmlEncode();
            Assert.Equal("&lt;div&gt;1&lt;/div&gt;", val2);
            Assert.Equal(key2, val2.ToHtmlDecode());

            Assert.Equal(key1, key1.ToByte().ToText());
            Assert.Equal(key1, key1.ToBase64Encode().ToBase64Decode());
        }

        [Fact]
        public void Extensions_4_Copy()
        {
            var vm1 = new ResultVM
            {
                Code = 2,
                Msg = "msg"
            };
            var vm2 = new ResultVM().ToDeepCopy(vm1);
            Assert.Equal(vm1.Code, vm2.Code);
            Assert.Equal(vm1.Msg, vm2.Msg);
        }

        [Fact]
        public void Extensions_5_Date()
        {
            var datetime = new DateTime(2022, 5, 12, 3, 24, 35, DateTimeKind.Utc);
            Assert.Equal(1652325875, datetime.ToTimestamp());
            Assert.Equal(19125, datetime.ToUtcTotalDays());
        }

        [Fact]
        public void Extensions_6_Add()
        {
            var v1 = new string[3] { "a", "b", "c" };
            var v2 = v1.Add("d");
            foreach (var item in v2)
            {
                Debug.WriteLine(item);
            }
            Assert.True(v2.Count() == 4);
        }

        [Fact]
        public void FileTo_1()
        {
            var txt1 = "test1";
            var path = Path.GetTempFileName();

            FileTo.WriteText(txt1, path);
            FileTo.WriteText(txt1, path);

            Assert.StartsWith($"{txt1}\r\n{txt1}", File.ReadAllText(path));

            var txt2 = "test2";
            FileTo.WriteText(txt2, path, false);
            Assert.StartsWith(txt2, File.ReadAllText(path));

            File.Delete(path);
        }

        [Fact]
        public async Task HttpTo_1_Get()
        {
            string url = "https://vv.video.qq.com/checktime?otype=json";

            var client = HttpTo.BuildClient();
            var result = await client.GetStringAsync(url);

            Assert.Contains("QZOutputJson", result);
        }

        [Fact]
        public async Task HttpTo_2_Post()
        {
            string url = "https://vv.video.qq.com/checktime?otype=json";

            var client = HttpTo.BuildClient();
            var resp = await client.PostAsync(url, null);
            var result = await resp.Content.ReadAsStringAsync();

            Assert.Contains("QZOutputJson", result);
        }

        [Fact]
        public async Task LockTo_1()
        {
            var st = Stopwatch.StartNew();

            var num = 0;
            var arr = Enumerable.Range(1, 999);
            arr.AsParallel().ForAll(i =>
            {
                num++;
            });
            // != 999
            Debug.WriteLine(num);
            Debug.WriteLine(st.Elapsed);
            st.Restart();

            num = 0;
            arr.AsParallel().ForAll(i =>
            {
                LockTo.Run("key", 1000, () =>
                {
                    num++;
                });
            });
            // == 999
            Debug.WriteLine(num);
            Assert.True(num == arr.Count());
            Debug.WriteLine(st.Elapsed);
            st.Restart();

            num = 0;
            await Parallel.ForEachAsync(arr, async (i, ctoken) =>
            {
                await LockTo.RunAsync("key", 1000, () =>
                {
                    num++;
                    return Task.CompletedTask;
                });
            });
            // == 999
            Debug.WriteLine(num);
            Assert.True(num == arr.Count());
            Debug.WriteLine(st.Elapsed);
        }

        [Fact]
        public void LngLatTo_1()
        {
            double lng = 106.533115;
            double lat = 29.349887;

            var gcj = LngLatTo.Wgs84ToGcj02(lng, lat);
            Assert.True(gcj.Length == 2);
            Debug.WriteLine(string.Join(',', gcj));

            var bd = LngLatTo.Gcj02ToBd09(gcj[0], gcj[1]);
            Assert.True(bd.Length == 2);
            Debug.WriteLine(string.Join(',', bd));
        }

        [Fact]
        public async Task MailTo_1()
        {
            try
            {
                var model = new MailTo.SendModel()
                {
                    Host = "smtp.exmail.qq.com",
                    //Port = 465,
                    FromMail = "netnr@netnr.com",
                    FromPassword = "密码",
                    FromName = "Netnr",
                    Subject = "验证码",
                    Body = $"你的验证码为：<h2 style='text-align:center'>{RandomTo.NewNumber()}</h2>",
                    ToMail = ["netnr@tempmail.cn", "netnr@bccto.cc"],
                    CcMail = ["netnr@teml.net", "netnr@nqmo.com"]
                };

                await MailTo.Send(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [Fact]
        public async Task MonitorTo_1_HTTP()
        {
            var url = "https://www.baidu.com";
            var result = await MonitorTo.HTTP(url, HttpMethod.Get);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public void MonitorTo_2_SSL()
        {
            var uri = new Uri("https://expired.badssl.com/");
            var result = MonitorTo.SSL(uri);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public async Task MonitorTo_3_DNS()
        {
            var host = "www.baidu.com";
            var result = await MonitorTo.DNS(host);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public void MonitorTo_4_TCPort()
        {
            var host = "www.baidu.com";
            var result = MonitorTo.TCPort(host, 443);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public void MonitorTo_5_PING()
        {
            var host = "www.baidu.com";
            var result = MonitorTo.PING(host);
            Debug.WriteLine(result.ToJson(true));
            Assert.True(result.Code == 200);
        }

        [Fact]
        public void ParsingTo_1_Is()
        {
            var key1 = "netnr@netnr.com";
            Assert.True(ParsingTo.IsMail(key1));
            Assert.False(ParsingTo.IsMail(key1.Replace(".", "..")));
            Assert.False(ParsingTo.IsMail(key1.Replace("@", "")));
        }

        [Fact]
        public void ParsingTo_2_Format()
        {
            Assert.Equal("1023B", ParsingTo.FormatByte(1023));
            Assert.Equal("1KiB", ParsingTo.FormatByte(1024));
            Assert.Equal("1KiB", ParsingTo.FormatByte(1025));
            Assert.Equal("1MiB", ParsingTo.FormatByte(1024 * 1024));
            Assert.Equal("1MiB", ParsingTo.FormatByte(1024 * 1024 + 1));
            Assert.Equal("1MiB", ParsingTo.FormatByte(1024 * 1024 - 1));
            Assert.Equal("1GiB", ParsingTo.FormatByte(1024 * 1024 * 1024));

            Assert.Equal("00:00:00.222", ParsingTo.FormatMilliseconds(222));
            Assert.Equal("00:00:22.022", ParsingTo.FormatMilliseconds(1000 * 22 + 22));
            Assert.Equal("00:01:22.022", ParsingTo.FormatMilliseconds(1000 * 82 + 22));
            Assert.Equal("01:00:00.022", ParsingTo.FormatMilliseconds(1000 * 3600 + 22));

            var now = DateTime.Now;
            Debug.WriteLine(ParsingTo.FormatTimeAgo(now));
            now = now.AddSeconds(-9);
            Debug.WriteLine(ParsingTo.FormatTimeAgo(now));
            now = now.AddMinutes(-9);
            Debug.WriteLine(ParsingTo.FormatTimeAgo(now));
            now = now.AddHours(-9);
            Debug.WriteLine(ParsingTo.FormatTimeAgo(now));
            now = now.AddDays(-9);
            Debug.WriteLine(ParsingTo.FormatTimeAgo(now));
            now = now.AddMonths(-1);
            Debug.WriteLine(ParsingTo.FormatTimeAgo(now));
            now = now.AddYears(-9);
            Debug.WriteLine(ParsingTo.FormatTimeAgo(now));
        }

        [Fact]
        public void ParsingTo_3_Safe()
        {
            var key1 = "Abc_123";
            Assert.False(ParsingTo.IsDanger(key1));
            Assert.True(ParsingTo.ReplaceDanger(key1) == key1);

            var key2 = $"你好{key1}@-@,'\"";
            Assert.True(ParsingTo.IsDanger("你好"));
            Assert.True(ParsingTo.IsDanger(key2));
            Assert.True(ParsingTo.ReplaceDanger(key2) == key1);
        }

        [Fact]
        public void PathTo_1()
        {
            Assert.Equal("abc/123/xyz/", ParsingTo.Combine("abc/ ", "/123", "", "xyz/"));

            Assert.NotEqual("abc/123/xyz/", Path.Combine("abc/ ", "/123", "", "xyz/"));
        }

        [Fact]
        public void RandomTo_1()
        {
            Assert.Matches("\\w{4}", RandomTo.NewNumber());
            Assert.Matches("\\d{99}", RandomTo.NewNumber(99));

            Assert.Matches("\\w{4}", RandomTo.NewString());
            Assert.Matches("\\w{10}", RandomTo.NewString(10));
            Assert.Matches("\\w{50}", RandomTo.NewString(50));
            Assert.Matches("\\w{999}", RandomTo.NewString(999));
        }

        [Fact]
        public async Task SystemStatusTo_1()
        {
            var ss = new SystemStatusTo();
            await ss.RefreshAll();
            Assert.NotEmpty(ss.ToView());
            Debug.WriteLine(ss.ToJson(true));
        }

        [Fact]
        public async Task SystemStatusTo_2()
        {
            var list1 = await SystemStatusTo.GetAddressList();
            Debug.WriteLine(list1.ToJson(true));

            var endPoint = await SystemStatusTo.GetAddressInterNetwork();
            Debug.WriteLine(endPoint.ToJson(true));
        }

        [Fact]
        public void TextMiningTo_1()
        {
            var sw = Stopwatch.StartNew();
            var filePath = @"D:\tmp\txt\《西游记》.txt";

            var tm = new TextMiningTo();
            tm.FromFile(filePath);

            Debug.WriteLine(sw.Elapsed);
            Debug.WriteLine(tm.TopItems().ToJson());
        }

        [Fact]
        public void ZipTo_1()
        {
            var pathName1 = Path.GetTempFileName();
            var pathName2 = Path.GetTempFileName();

            var dicFile = new Dictionary<string, string>
            {
                { pathName1, "" },
                { pathName2, "" }
            };

            var zipPath = Path.Combine(Path.GetTempPath(), "00-zip-test.zip");
            ZipTo.Create(dicFile, zipPath);
            Assert.True(File.Exists(zipPath));

            ZipTo.Extract(zipPath);

            var dirPath = Path.Combine(Path.GetTempPath(), "00-zip-test");
            Assert.True(File.Exists(Path.Combine(dirPath, Path.GetFileName(pathName1))));

            File.Delete(zipPath);
            ZipTo.Create(dirPath, zipPath);

            File.Delete(pathName1);
            File.Delete(pathName2);
            Directory.Delete(dirPath, true);
        }

        [Fact]
        public void ZipTo_2()
        {
            var text = "这是test内容，2022-05-27";
            for (int i = 0; i < 999; i++)
            {
                text += text;
            }

            var savePath = Path.Combine(Path.GetTempPath(), "text2zip.zip");

            var zipBuffer = ZipTo.TextToZip(text);
            var text1 = ZipTo.ZipToText(zipBuffer);
            Assert.Equal(text, text1);

            ZipTo.TextToZip(text, savePath: savePath);
            Assert.True(File.Exists(savePath));

            File.Delete(savePath);
        }
    }
}
