using Xunit;

namespace Netnr.Test
{
    public class TestNetnrCore
    {
        [Fact]
        public void CacheTo_1_Set()
        {
            var key1 = "test1";
            Core.CacheTo.Set(key1, "val", 1, false);

            Assert.Equal("val", Core.CacheTo.Get(key1));

            Thread.Sleep(1000);

            Assert.Null(Core.CacheTo.Get(key1));
        }

        [Fact]
        public void CacheTo_2_Remove()
        {
            var key2 = "test2";
            Core.CacheTo.Set(key2, "val");

            Assert.Equal("val", Core.CacheTo.Get(key2));

            Core.CacheTo.Remove(key2);

            Assert.Null(Core.CacheTo.Get(key2));
        }

        [Fact]
        public void CacheTo_3_RemoveAll()
        {
            var key3 = "test3";
            Core.CacheTo.SetOption(key3, "val3", 10, false);

            Thread.Sleep(3000);
            Core.CacheTo.RemoveAll();

            Assert.Null(Core.CacheTo.Get(key3));
        }

        [Fact]
        public void CaclTo_1_AES()
        {
            var txt1 = "text1";
            var key1 = "key1";

            var val1 = Core.CalcTo.AESEncrypt(txt1, key1);
            var txt2 = Core.CalcTo.AESDecrypt(val1, key1);

            Assert.Equal(txt1, txt2);

            var key2 = "bad key";
            try
            {
                Core.CalcTo.AESDecrypt(val1, key2);
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

            var val1 = Core.CalcTo.AESEncrypt(txt1, Core.CalcTo.AESBuild("key 123", "iv 456"));
            var txt2 = Core.CalcTo.AESDecrypt(val1, Core.CalcTo.AESBuild("key 123", "iv 456"));

            Assert.Equal(txt1, txt2);
        }

        [Fact]
        public void CaclTo_1_DES()
        {
            var txt1 = "text1";
            var key1 = "key1";

            var val1 = Core.CalcTo.DESEncrypt(txt1, key1);
            var txt2 = Core.CalcTo.DESDecrypt(val1, key1);

            Assert.Equal(txt1, txt2);

            var key2 = "bad key";
            try
            {
                Core.CalcTo.DESDecrypt(val1, key2);
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

            var val1 = Core.CalcTo.DESEncrypt(txt1, Core.CalcTo.DESBuild("key 123", "iv 456"));
            var txt2 = Core.CalcTo.DESDecrypt(val1, Core.CalcTo.DESBuild("key 123", "iv 456"));

            Assert.Equal(txt1, txt2);
        }

        [Fact]
        public void CaclTo_1_HashBase64()
        {
            var txt1 = "text1";

            var val1 = Core.CalcTo.HashBase64(Core.CalcTo.HashType.MD5, txt1.ToStream());

            //https://approsto.com/md5-generator/
            Assert.Equal("zvfM2J2s8c7W9eyR11mVPw==", val1);
        }

        [Fact]
        public void CaclTo_2_HashString()
        {
            var txt1 = "text1";
            Assert.Equal("cef7ccd89dacf1ced6f5ec91d759953f", Core.CalcTo.MD5(txt1));
            Assert.Equal("38e9be6401e62f7d1555230d9df956fb85c2ba6e", Core.CalcTo.SHA_1(txt1));
            Assert.Equal("fe8df1a5a1980493ca9406ad3bb0e41297d979d90165a181fb39a5616a1c0789", Core.CalcTo.SHA_256(txt1));
            Assert.Equal("94cdfd2332d260a102e4b5541503f9ff82d1e7136a2f720fea018816dbb7c62eb761ef050a83fd44971d667483080fdf", Core.CalcTo.SHA_384(txt1));
            Assert.Equal("b5534aa935bcb3749cbd0846c636ef2dca0d4119225b731b03336d51fd765a81ff0d9c19ed31ccc6dc6b6fc2826ee0eac29e7115330566e2088e3c2ee9155b4e", Core.CalcTo.SHA_512(txt1));

            var key1 = "key1";
            Assert.Equal("9f04de3535fa2e64924083003b8b16c8a84ef0ee", Core.CalcTo.HMAC_SHA1(txt1, key1));
            Assert.Equal("cd1e288d14bfafa334ccb7f4394d9f524726a6c565aa548a9483155c89dd0e8c", Core.CalcTo.HMAC_SHA256(txt1, key1));
            Assert.Equal("eeea03bda39b6c6441b4baab8c644832fcd48ff269b682b53a2407249f54ce5ebedab1c325198c464357280349f0b101", Core.CalcTo.HMAC_SHA384(txt1, key1));
            Assert.Equal("fceeb5fa9d2107da9e35a9b068d0b4f38020b63ef56810d8e042c5e59d8deb9810232f8fdc61772b485318f9a0261a763ae6979442687ceba8aefe7d372f365a", Core.CalcTo.HMAC_SHA512(txt1, key1));
            Assert.Equal("cb3b4a1be8c7c2963c4a2475c0837466", Core.CalcTo.HMAC_MD5(txt1, key1));
        }

        [Fact]
        public void CaclTo_1_RSA()
        {
            Core.CalcTo.RSAKey(out string xmlPrivateKey, out string xmlPublicKey, 2048);
            Assert.NotEmpty(xmlPrivateKey);
            Assert.NotEmpty(xmlPublicKey);

            var content1 = "abc 123 !@#";
            var content2 = Core.CalcTo.encoding.GetBytes(content1);

            var resultEncrypt1 = Core.CalcTo.RSAEncrypt(xmlPublicKey, content1);
            var resultEncrypt2 = Core.CalcTo.RSAEncrypt(xmlPublicKey, content2);
            Assert.NotEmpty(resultEncrypt1);
            Assert.NotEmpty(resultEncrypt2);

            var resultDecrypt1 = Core.CalcTo.RSADecrypt(xmlPrivateKey, resultEncrypt1);
            var resultDecrypt2 = Core.CalcTo.RSADecrypt(xmlPrivateKey, resultEncrypt2);
            Assert.Equal(content1, resultDecrypt1);
            Assert.Equal(content1, resultDecrypt2);
        }

        [Fact]
        public void CmdTo_1()
        {
            if (Core.CmdTo.IsWindows)
            {
                var val1 = Core.CmdTo.Execute("ver");
                Assert.Empty(val1.CrError);
                Assert.Contains("Microsoft Windows", val1.CrOutput);
            }
            else
            {
                var val1 = Core.CmdTo.Execute("uname -a");
                Assert.Empty(val1.CrError);
                Assert.Contains("Linux", val1.CrOutput);
            }
        }

        [Fact]
        public void CmdTo_2()
        {
            var result = string.Empty;
            var psi = Core.CmdTo.PSInfo("-V", @"curl");
            Core.CmdTo.Execute(psi, (process, cr) =>
            {
                process.Start();
                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
            });

            Assert.StartsWith("curl", result);
        }

        [Fact]
        public void CmdTo_3_FFmpeg()
        {
            var result = string.Empty;
            Core.CmdTo.Execute(Core.CmdTo.PSInfo(@"-i", "ffmpeg"), (process, cr) =>
            {
                process.ErrorDataReceived += (sender, output) =>
                {
                    result += output.Data;
                };

                process.Start();//启动线程
                process.BeginErrorReadLine();//开始异步读取
                process.WaitForExit();//阻塞等待进程结束
                process.Close();//关闭进程
                process.Dispose();
            });

            Assert.StartsWith("ffmpeg", result);
        }

        [Fact]
        public void ConsoleTo()
        {
            Core.ConsoleTo.Log(DateTime.Now);
            var i = 0;
            try
            {
                var j = 0 / i;
            }
            catch (Exception ex)
            {
                Core.ConsoleTo.Log(ex);
            }

            var now = DateTime.Now;
            var filename = $"console_{now:yyyyMMdd}.log";

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", now.Year.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fullPath = Path.Combine(path, filename);

            Assert.True(File.Exists(fullPath));
        }

        [Fact]
        public void Extend_1_JSON_Table_List()
        {
            var vm = new SharedResultVM();
            var list = new List<SharedResultVM>()
            {
                new SharedResultVM(),
                new SharedResultVM()
            };

            var val1 = vm.ToJson();
            var val2 = list.ToJson();
            Assert.NotEmpty(val1);
            Assert.NotEmpty(val2);

            var jo1 = val1.ToJObject();
            var ja1 = val2.ToJArray();
            Assert.NotNull(jo1);
            Assert.NotNull(ja1);
            Assert.True(string.IsNullOrEmpty(jo1["null"].ToStringOrEmpty()));

            var jo2 = val1.ToModel<SharedResultVM>();
            var jo3 = val2.ToModels<SharedResultVM>();
            Assert.IsType<SharedResultVM>(jo2);
            Assert.Equal(2, jo3.Count);

            var dt1 = list.ToDataTable();
            Assert.Equal(2, dt1.Rows.Count);

            var list2 = dt1.ToModel<SharedResultVM>();
            Assert.Equal(2, list2.Count);
        }

        [Fact]
        public void Extend_2_Escape()
        {
            var passwd = "' or ''='";
            var sql = $"select 1 from user where user='admin' and passwd='{passwd.OfSql()}'";
            Assert.DoesNotContain("or ''=''", sql);
        }

        [Fact]
        public void Extend_3_Encode_Decode()
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
        public void Extend_4_Copy()
        {
            var vm1 = new SharedResultVM
            {
                Code = 2,
                Msg = "msg"
            };
            var vm2 = new SharedResultVM().ToRead(vm1);
            Assert.Equal(vm1.Code, vm2.Code);
            Assert.Equal(vm1.Msg, vm2.Msg);
        }

        [Fact]
        public void Extend_5_Date()
        {
            var datetime = new DateTime(2022, 5, 12, 3, 24, 35, DateTimeKind.Utc);
            Assert.Equal(1652325875, datetime.ToTimestamp());
            Assert.Equal(19125, datetime.ToUtcTotalDays());
        }

        [Fact]
        public void Extend_6_Add()
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
        public void FileTo()
        {
            var txt1 = "test1";
            var path = Path.GetTempFileName();

            Core.FileTo.WriteText(txt1, path);
            Core.FileTo.WriteText(txt1, path);

            Assert.StartsWith($"{txt1}\r\n{txt1}", Core.FileTo.ReadText(path));

            var txt2 = "test2";
            Core.FileTo.WriteText(txt2, path, false);
            Assert.StartsWith(txt2, Core.FileTo.ReadText(path));

            File.Delete(path);
        }

        [Fact]
        public void HttpTo_1_Get()
        {
            string url = "https://vv.video.qq.com/checktime?otype=json";
            var result = Core.HttpTo.Get(url);
            Assert.Contains("QZOutputJson", result);
        }

        [Fact]
        public void HttpTo_2_Post()
        {
            string url = "https://vv.video.qq.com/checktime?otype=json";
            var result = Core.HttpTo.Post(url, "");
            Assert.Contains("QZOutputJson", result);
        }

        [Fact]
        public void HttpTo_9_Download()
        {
            string url = "https://img01.sogoucdn.com/app/a/100540022/2021053117531272442865.png";
            var tmpPath = Path.GetTempFileName();
            Core.HttpTo.DownloadSave(Core.HttpTo.HWRequest(url), tmpPath);

            Assert.True(File.Exists(tmpPath));
        }

        [Fact]
        public void ParsingTo_1_Is()
        {
            var key1 = "netnr@netnr.com";
            Assert.True(Core.ParsingTo.IsMail(key1));
            Assert.False(Core.ParsingTo.IsMail(key1.Replace(".", "..")));
            Assert.False(Core.ParsingTo.IsMail(key1.Replace("@", "")));
        }

        [Fact]
        public void ParsingTo_2_Format()
        {
            Assert.Equal("1023 B", Core.ParsingTo.FormatByteSize(1023));
            Assert.Equal("1 KiB", Core.ParsingTo.FormatByteSize(1024));
            Assert.Equal("1 KiB", Core.ParsingTo.FormatByteSize(1025));
            Assert.Equal("1 MiB", Core.ParsingTo.FormatByteSize(1024 * 1024));
            Assert.Equal("1 MiB", Core.ParsingTo.FormatByteSize(1024 * 1024 + 1));
            Assert.Equal("1 MiB", Core.ParsingTo.FormatByteSize(1024 * 1024 - 1));
            Assert.Equal("1 GiB", Core.ParsingTo.FormatByteSize(1024 * 1024 * 1024));

            Assert.Equal("00:00:00:222", Core.ParsingTo.FormatMillisecondsSize(222));
            Assert.Equal("00:00:22:022", Core.ParsingTo.FormatMillisecondsSize(1000 * 22 + 22));
            Assert.Equal("00:01:22:022", Core.ParsingTo.FormatMillisecondsSize(1000 * 82 + 22));
            Assert.Equal("01:00:00:022", Core.ParsingTo.FormatMillisecondsSize(1000 * 3600 + 22));
        }

        [Fact]
        public void PathTo()
        {
            Assert.Equal("abc/123/xyz/", Core.PathTo.Combine("abc/ ", "/123", "", "xyz/"));

            Assert.NotEqual("abc/123/xyz/", Path.Combine("abc/ ", "/123", "", "xyz/"));
        }

        [Fact]
        public void RandomTo()
        {
            Assert.Matches("\\w{4}", Core.RandomTo.NumCode());
            Assert.Matches("\\d{99}", Core.RandomTo.NumCode(99));

            Assert.Matches("\\w{4}", Core.RandomTo.StrCode());
            Assert.Matches("\\w{10}", Core.RandomTo.StrCode(10));
            Assert.Matches("\\w{50}", Core.RandomTo.StrCode(50));
            Assert.Matches("\\w{999}", Core.RandomTo.StrCode(999));
        }

        [Fact]
        public void SnowflakeTo()
        {
            var st = new SharedTimingVM();
            var sf = new Core.SnowflakeTo();

            var hs = new HashSet<long>();
            for (int i = 0; i < 1_999_999; i++)
            {
                var val = sf.NewId();
                if (!hs.Add(val))
                {
                    throw new Exception("same");
                }
            }
            Debug.WriteLine($"Snowflake 1_999_999: {st.PartTimeFormat()}");
        }

        [Fact]
        public void SystemStatusTo()
        {
            var ss = new Core.SystemStatusTo();
            Assert.NotEmpty(ss.ToJson());
            Assert.NotEmpty(ss.ToView());

            //var isStop = false;
            //Core.SystemStatusTo.GetNetworkSpeed((brSpeed, bsSpeed, brDic, bsDic) =>
            //{
            //    Debug.WriteLine($"Received Speed: {Core.ParsingTo.FormatByteSize(brSpeed)}/s");
            //    Debug.WriteLine($"Sent Speed: {Core.ParsingTo.FormatByteSize(bsSpeed)}/s\r\n");

            //    isStop = true;
            //}, ref isStop);
        }

        [Fact]
        public void TreeTo()
        {
            var list = new List<SharedResultVM>();
            //id:Code Pid:Data
            var vm1 = new SharedResultVM() { Code = 1, Msg = "一级", Data = 0 };
            var vm2 = new SharedResultVM() { Code = 11, Msg = "二级", Data = 1 };
            var vm3 = new SharedResultVM() { Code = 12, Msg = "二级", Data = 1 };
            var vm4 = new SharedResultVM() { Code = 121, Msg = "三级", Data = 12 };
            list.Add(vm1);
            list.Add(vm2);
            list.Add(vm3);
            list.Add(vm4);

            var result = Core.TreeTo.ListToTree(list, "Data", "Code", new List<string> { "0" });
            Assert.NotEmpty(result);

            var nodes = Core.TreeTo.FindToTree(list, "Data", "Code", new List<string> { "12" });
            Assert.Single(nodes);
        }

        [Fact]
        public void UniqueTo()
        {
            var st = new SharedTimingVM();
            var hs = new HashSet<long>();
            for (int i = 0; i < 1_999_999; i++)
            {
                var val = Core.UniqueTo.LongId();
                if (!hs.Add(val))
                {
                    throw new Exception("same");
                }
            }
            Debug.WriteLine($"UniqueTo 1_999_999: {st.PartTimeFormat()}");
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
            Core.ZipTo.Create(dicFile, zipPath);
            Assert.True(File.Exists(zipPath));

            Core.ZipTo.Extract(zipPath);

            var dirPath = Path.Combine(Path.GetTempPath(), "00-zip-test");
            Assert.True(File.Exists(Path.Combine(dirPath, Path.GetFileName(pathName1))));

            File.Delete(zipPath);
            Core.ZipTo.Create(dirPath, zipPath);

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

            var zipBuffer = Core.ZipTo.TextToZip(text);
            var text1 = Core.ZipTo.ZipToText(zipBuffer);
            Assert.Equal(text, text1);

            Core.ZipTo.TextToZip(text, savePath: savePath);
            Assert.True(File.Exists(savePath));

            File.Delete(savePath);
        }
    }
}
