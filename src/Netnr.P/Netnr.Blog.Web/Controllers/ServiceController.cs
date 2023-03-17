using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 服务
    /// </summary>
    [Route("[Controller]/[action]")]
    public class ServiceController : Controller
    {
        /// <summary>
        /// 微信公众号
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echostr"></param>
        /// <param name="encrypt_type"></param>
        /// <param name="msg_signature"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async void Weixin(string signature, string timestamp, string nonce, string echostr, string encrypt_type, string msg_signature)
        {
            string result = null;

            //微信公众号验证地址
            if (Request.Method == "GET")
            {
                var token = AppTo.GetValue("ApiKey:WeixinMP:Token");

                var args = new[] { token, timestamp, nonce }.OrderBy(x => x).ToArray();
                var hash = CalcTo.SHA_1(string.Join("", args)).ToLower();

                result = hash == signature ? echostr : "Verification failed";
            }
            else if (Request.Method == "POST")
            {
                //接收
                using var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);
                var postData = ms.ToArray().ToText();

                //解密
                if (encrypt_type == "aes")
                {
                    var token = AppTo.GetValue("ApiKey:WeixinMP:Token");
                    var encodingAESKey = AppTo.GetValue("ApiKey:WeixinMP:EncodingAESKey");
                    var appId = AppTo.GetValue("ApiKey:WeixinMP:AppID");
                }
                ConsoleTo.Title("Receive WeixinMP Message", postData);

                //处理消息并回复
                result = WeixinMPService.MessageReply(postData);
            }

            //输出
            var buffer = result.ToByte();
            await Response.Body.WriteAsync(buffer);
            await Response.Body.FlushAsync();
        }

        /// <summary>
        /// 数据库导出（管理员）
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <param name="onlyData">仅数据，默认带表结构</param>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public ResultVM DatabaseExport(string zipName = "db/backup.zip", bool onlyData = false) => ResultVM.Try(vm =>
        {
            var edb = new DataKitTransferVM.ExportDatabase
            {
                PackagePath = Path.Combine(AppTo.ContentRootPath, zipName),
                ReadConnectionInfo = new DataKitTransferVM.ConnectionInfo()
                {
                    ConnectionString = DbContextTo.GetConn().Replace("Filename=", "Data Source="),
                    ConnectionType = AppTo.TDB
                },
                Type = onlyData ? "onlyData" : "all"
            };

            if (System.IO.File.Exists(edb.PackagePath))
            {
                System.IO.File.Delete(edb.PackagePath);
            }
            vm = DataKitTo.ExportDatabase(edb);

            return vm;
        });

        /// <summary>
        /// 数据库导出（管理员，示例数据，开发环境）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public ResultVM DatabaseExportDemo() => ResultVM.Try(vm =>
        {
            if (GlobalTo.IsDev)
            {
                var export_before = "db/backup_demo_before.zip";
                var export_demo = "db/backup_demo.zip";

                //备份
                if (DatabaseExport(export_before).Code == 200)
                {
                    //清理仅保留示例数据

                    using var db = ContextBaseFactory.CreateDbContext();

                    db.UserInfo.RemoveRange(db.UserInfo.ToList());
                    db.UserInfo.Add(new UserInfo()
                    {
                        UserId = 1,
                        UserName = "netnr",
                        UserPwd = "e10adc3949ba59abbe56e057f20f883e",//123456
                        UserCreateTime = DateTime.Now
                    });

                    db.UserConnection.RemoveRange(db.UserConnection.ToList());
                    db.UserMessage.RemoveRange(db.UserMessage.ToList());
                    db.UserReply.RemoveRange(db.UserReply.Where(x => x.UrTargetId != "117").ToList());
                    db.UserWriting.RemoveRange(db.UserWriting.Where(x => x.UwId != 117).ToList());
                    db.UserWritingTags.RemoveRange(db.UserWritingTags.Where(x => x.UwId != 117).ToList());

                    db.Tags.RemoveRange(db.Tags.Where(x => x.TagId != 58 && x.TagId != 96).ToList());

                    db.Run.RemoveRange(db.Run.OrderBy(x => x.RunCreateTime).Skip(1).ToList());

                    db.OperationRecord.RemoveRange(db.OperationRecord.ToList());

                    db.Notepad.RemoveRange(db.Notepad.ToList());

                    db.KeyValues.RemoveRange(db.KeyValues.Where(x => x.KeyName != "https" && x.KeyName != "browser").ToList());
                    db.KeyValueSynonym.RemoveRange(db.KeyValueSynonym.ToList());

                    db.GuffRecord.RemoveRange(db.GuffRecord.ToList());

                    db.Gist.RemoveRange(db.Gist.Where(x => x.GistCode != "5373307231488995367").ToList());
                    db.GistSync.RemoveRange(db.GistSync.Where(x => x.GistCode != "5373307231488995367").ToList());

                    db.GiftRecord.RemoveRange(db.GiftRecord.ToList());
                    db.GiftRecordDetail.RemoveRange(db.GiftRecordDetail.ToList());

                    db.Draw.RemoveRange(db.Draw.Where(x => x.DrId != "d4969500168496794720" && x.DrId != "m4976065893797151245").ToList());

                    db.DocSet.RemoveRange(db.DocSet.Where(x => x.DsCode != "4840050256984581805" && x.DsCode != "5036967707833574483").ToList());
                    db.DocSetDetail.RemoveRange(db.DocSetDetail.Where(x => x.DsCode != "4840050256984581805" && x.DsCode != "5036967707833574483").ToList());

                    var num = db.SaveChanges();

                    //导出示例数据
                    vm = DatabaseExport(export_demo, true);

                    //导入恢复
                    if (DatabaseImport(export_before, true).Code == 200)
                    {
                        var fullPath = Path.Combine(AppTo.ContentRootPath, export_before);
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            else
            {
                vm.Set(EnumTo.RTag.refuse);
                vm.Msg = "仅限开发环境使用";
            }

            return vm;
        });

        /// <summary>
        /// 数据库备份到 Git（管理员）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public ResultVM DatabaseBackupToGit()
        {
            return ResultVM.Try(vm =>
            {
                if (AppTo.GetValue<bool>("ReadOnly"))
                {
                    vm.Set(EnumTo.RTag.refuse);
                    return vm;
                }

                var now = $"{DateTime.Now:yyyyMMdd_HHmmss}";

                var db = ContextBaseFactory.CreateDbContext();
                var database = db.Database.GetDbConnection().Database;

                var createScript = db.Database.GenerateCreateScript();

                //备份创建脚本
                var b1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(createScript));
                var p1 = $"{database}/backup_{now}.sql";
                try
                {
                    vm.Log.Add(PutGitee(b1, p1, now));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    vm.Log.Add(ex.Message);
                }
                try
                {
                    vm.Log.Add(PutGitHub(b1, p1, now));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    vm.Log.Add(ex.Message);
                }

                Thread.Sleep(1000 * 1);

                //备份数据
                var zipPath = $"db/backup_{now}.zip";
                if (DatabaseExport(zipPath).Code == 200)
                {
                    var ppath = Path.Combine(AppTo.ContentRootPath, zipPath);
                    var b2 = Convert.ToBase64String(System.IO.File.ReadAllBytes(ppath));
                    var p2 = $"{database}/backup_{now}.zip";
                    try
                    {
                        vm.Log.Add(PutGitee(b2, p2, now));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        vm.Log.Add(ex.Message);
                    }
                    try
                    {
                        vm.Log.Add(PutGitHub(b2, p2, now));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        vm.Log.Add(ex.Message);
                    }

                    System.IO.File.Delete(ppath);
                }

                var vmj = vm.ToJson(true);
                Console.WriteLine(vmj);
                ConsoleTo.Log(vmj);

                return vm;
            });
        }

        /// <summary>
        /// 推送到GitHub
        /// </summary>
        /// <param name="content">内容 base64</param>
        /// <param name="path">路径</param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="or"></param>
        /// <returns></returns>
        private static string PutGitHub(string content, string path, string message = "m", string token = null, string or = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                token = AppTo.GetValue("ApiKey:GitHub:GistToken");
            }
            if (string.IsNullOrWhiteSpace(or))
            {
                or = AppTo.GetValue("Common:AdminBackupToGit");
            }

            var put = $"https://api.github.com/repos/{or}/contents/{path}";

            var hwr = HttpTo.HWRequest(put, "PUT", Encoding.UTF8.GetBytes(new { message, content }.ToJson()));

            hwr.Headers.Set("Accept", "application/vnd.github.v3+json");
            hwr.Headers.Set("Authorization", $"token {token}");
            hwr.Headers.Set("Content-Type", "application/json");

            var result = HttpTo.Url(hwr);

            return result;
        }

        /// <summary>
        /// 推送到Gitee
        /// </summary>
        /// <param name="content">内容 base64</param>
        /// <param name="path">路径</param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="or"></param>
        /// <returns></returns>
        private static string PutGitee(string content, string path, string message = "m", string token = null, string or = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                token = AppTo.GetValue("ApiKey:Gitee:GistToken");
            }
            if (string.IsNullOrWhiteSpace(or))
            {
                or = AppTo.GetValue("Common:AdminBackupToGit");
            }

            var listor = or.Split('/');
            var owner = listor.First();
            var repo = listor.Last();
            var uri = $"https://gitee.com/api/v5/repos/{owner}/{repo}/contents/{path}";

            var hwr = HttpTo.HWRequest(uri, "POST", Encoding.UTF8.GetBytes(new { access_token = token, message, content }.ToJson()));
            hwr.Headers.Set("Content-Type", "application/json");

            var result = HttpTo.Url(hwr);

            return result;
        }

        /// <summary>
        /// 数据库导入（管理员）
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <param name="clearTable">清空表，默认 false</param>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public ResultVM DatabaseImport(string zipName = "db/backup.zip", bool clearTable = false)
        {
            return ResultVM.Try(vm =>
            {
                var idb = new DataKitTransferVM.ImportDatabase
                {
                    WriteConnectionInfo = new DataKitTransferVM.ConnectionInfo
                    {
                        ConnectionType = AppTo.TDB,
                        ConnectionString = DbContextTo.GetConn().Replace("Filename=", "Data Source=")
                    },
                    PackagePath = Path.Combine(AppTo.ContentRootPath, zipName),
                    WriteDeleteData = clearTable
                };

                vm = DataKitTo.ImportDatabase(idb);

                return vm;
            });
        }

        /// <summary>
        /// Gist 代码片段，同步到 GitHub、Gitee（管理员）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public ResultVM GistSync()
        {
            var vm = new ResultVM();

            try
            {
                using var db = ContextBaseFactory.CreateDbContext();

                //同步用户ID
                int UserId = AppTo.GetValue<int>("Common:AdminId");

                //日志
                var listLog = new List<object>() { "Gist代码片段同步" };

                var listGist = db.Gist.Where(x => x.Uid == UserId).OrderBy(x => x.GistCreateTime).ToList();

                var codes = listGist.Select(x => x.GistCode).ToList();

                var listGs = db.GistSync.Where(x => x.Uid == UserId).ToList();

                //执行命令记录
                var dicSync = new Dictionary<string, string>();

                foreach (var gist in listGist)
                {
                    var gs = listGs.FirstOrDefault(x => x.GistCode == gist.GistCode);
                    //新增
                    if (gs == null)
                    {
                        dicSync.Add(gist.GistCode, "add");
                    }
                    else if (gs?.GsGitHubTime != gist.GistUpdateTime || gs?.GsGiteeTime != gist.GistUpdateTime)
                    {
                        dicSync.Add(gist.GistCode, "update");
                    }
                }

                //删除
                var delCode = listGs.Select(x => x.GistCode).Except(listGist.Select(x => x.GistCode)).ToList();

                var token_gh = AppTo.GetValue("ApiKey:GitHub:GistToken");
                var token_ge = AppTo.GetValue("ApiKey:Gitee:GistToken");

                listLog.Add("同步新增、修改：" + dicSync.Count + " 条");
                listLog.Add(dicSync);

                //同步新增、修改
                if (dicSync.Count > 0)
                {
                    foreach (var key in dicSync.Keys)
                    {
                        var st = dicSync[key];
                        var gist = listGist.FirstOrDefault(x => x.GistCode == key);
                        var gs = listGs.FirstOrDefault(x => x.GistCode == key);

                        #region 发送主体
                        var jo = new JsonObject
                        {
                            ["access_token"] = token_ge,//only gitee 

                            ["description"] = gist.GistRemark,
                            ["public"] = gist.GistOpen == 1,
                            ["files"] = new JsonObject
                            {
                                [gist.GistFilename] = new JsonObject
                                {
                                    ["content"] = gist.GistContent
                                }
                            }
                        };

                        byte[] sendData = Encoding.UTF8.GetBytes(jo.ToJson());
                        #endregion

                        switch (st)
                        {
                            case "add":
                                {
                                    var gsmo = new GistSync()
                                    {
                                        GistCode = key,
                                        Uid = UserId,
                                        GistFilename = gist.GistFilename
                                    };

                                    //GitHub
                                    {
                                        var hwr = HttpTo.HWRequest("https://api.github.com/gists", "POST", sendData);
                                        hwr.Headers.Add(HttpRequestHeader.Authorization, "token " + token_gh);
                                        hwr.ContentType = "application/json";

                                        var rt = HttpTo.Url(hwr);

                                        gsmo.GsGitHubId = rt.DeJson().GetValue("id");
                                        gsmo.GsGitHubTime = gist.GistUpdateTime;
                                    }

                                    //Gitee
                                    {
                                        var hwr = HttpTo.HWRequest("https://gitee.com/api/v5/gists", "POST", sendData);
                                        hwr.ContentType = "application/json";

                                        var rt = HttpTo.Url(hwr);

                                        gsmo.GsGiteeId = rt.DeJson().GetValue("id");
                                        gsmo.GsGiteeTime = gist.GistUpdateTime;
                                    }

                                    _ = db.GistSync.Add(gsmo);
                                    _ = db.SaveChanges();

                                    listLog.Add("新增一条成功");
                                    listLog.Add(gsmo);
                                }
                                break;
                            case "update":
                                {
                                    if (gs.GistFilename != gist.GistFilename)
                                    {
                                        jo["files"][gs.GistFilename] = null;
                                        gs.GistFilename = gist.GistFilename;
                                    }

                                    //GitHub
                                    {
                                        var hwr = HttpTo.HWRequest("https://api.github.com/gists/" + gs.GsGitHubId, "PATCH", sendData);
                                        hwr.Headers.Add(HttpRequestHeader.Authorization, "token " + token_gh);
                                        hwr.ContentType = "application/json";

                                        _ = HttpTo.Url(hwr);

                                        gs.GsGitHubTime = gist.GistUpdateTime;
                                    }

                                    //Gitee
                                    {
                                        var hwr = HttpTo.HWRequest("https://gitee.com/api/v5/gists/" + gs.GsGiteeId, "PATCH", sendData);
                                        hwr.ContentType = "application/json";

                                        _ = HttpTo.Url(hwr);

                                        gs.GsGiteeTime = gist.GistUpdateTime;
                                    }

                                    _ = db.GistSync.Update(gs);
                                    _ = db.SaveChanges();

                                    listLog.Add("更新一条成功");
                                    listLog.Add(gs);
                                }
                                break;
                        }

                        Thread.Sleep(1000 * 2);
                    }
                }

                listLog.Add("同步删除：" + delCode.Count + " 条");
                listLog.Add(delCode);

                //同步删除
                if (delCode.Count > 0)
                {
                    foreach (var code in delCode)
                    {
                        var gs = listGs.FirstOrDefault(x => x.GistCode == code);

                        var dc = "00".ToCharArray();

                        #region GitHub
                        var hwr_gh = HttpTo.HWRequest("https://api.github.com/gists/" + gs.GsGitHubId, "DELETE");
                        hwr_gh.Headers.Add(HttpRequestHeader.Authorization, "token " + token_gh);
                        var resp_gh = (HttpWebResponse)hwr_gh.GetResponse();
                        if (resp_gh.StatusCode == HttpStatusCode.NoContent)
                        {
                            dc[0] = '1';
                        }
                        #endregion

                        #region Gitee
                        var hwr_ge = HttpTo.HWRequest("https://gitee.com/api/v5/gists/" + gs.GsGiteeId + "?access_token=" + token_ge, "DELETE");
                        var resp_ge = (HttpWebResponse)hwr_ge.GetResponse();
                        if (resp_ge.StatusCode == HttpStatusCode.NoContent)
                        {
                            dc[1] = '1';
                        }
                        #endregion

                        if (string.Join("", dc) == "11")
                        {
                            _ = db.GistSync.Remove(gs);
                            _ = db.SaveChanges();

                            listLog.Add("删除一条成功");
                            listLog.Add(gs);
                        }
                        else
                        {
                            listLog.Add("删除一条异常");
                            listLog.Add(dc);
                        }

                        Thread.Sleep(1000 * 2);
                    }
                }

                listLog.Add("完成同步");

                vm.Set(EnumTo.RTag.success);
                vm.Data = listLog;

            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            var vmj = vm.ToJson(true);
            Console.WriteLine(vmj);
            ConsoleTo.Log(vmj);

            return vm;
        }

        /// <summary>
        /// 处理操作记录（管理员）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public ResultVM HandleOperationRecord()
        {
            var vm = new ResultVM();

            try
            {
                using var db = ContextBaseFactory.CreateDbContext();

                //处理Guff查询记录数
                var ctype = ConnectionType.GuffRecord.ToString();
                var listOr = db.OperationRecord.Where(x => x.OrType == ctype && x.OrMark == "default").ToList();
                if (listOr.Count > 0)
                {
                    var listAllId = string.Join(",", listOr.Select(x => x.OrSource).ToList()).Split(',').ToList();
                    var listid = listAllId.Distinct();

                    var listmo = db.GuffRecord.Where(x => listid.Contains(x.GrId)).ToList();
                    foreach (var item in listmo)
                    {
                        item.GrReadNum += listAllId.GroupBy(x => x).FirstOrDefault(x => x.Key == item.GrId).Count();
                    }
                    db.GuffRecord.UpdateRange(listmo);

                    db.OperationRecord.RemoveRange(listOr);

                    int num = db.SaveChanges();

                    vm.Set(num > 0);
                    vm.Data = "处理操作记录，受影响行数：" + num;
                }
                else
                {
                    vm.Set(EnumTo.RTag.lack);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            var vmj = vm.ToJson(true);
            Console.WriteLine(vmj);
            ConsoleTo.Log(vmj);

            return vm;
        }

        /// <summary>
        /// 监控（管理员）
        /// </summary>
        /// <param name="type">类型，http tcp ssl</param>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public ResultVM Monitor(string type)
        {
            return ResultVM.Try(vm =>
            {
                if (AppTo.GetValue<bool>("Monitor:enable"))
                {
                    var items = AppTo.GetValue($"Monitor:{type}").Split(',');
                    foreach (var item in items)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            Thread.Sleep(3000);

                            try
                            {
                                switch (type)
                                {
                                    case "http":
                                        {
                                            var ckey = $"Monitor:{type}:{item}";
                                            var lm = item.Split(' ');

                                            var client = new HttpClient();
                                            var request = new HttpRequestMessage
                                            {
                                                RequestUri = new Uri(lm.First().ToString()),
                                                Method = lm.Last().ToLower() == "post" ? HttpMethod.Post : HttpMethod.Get
                                            };

                                            var num = CacheTo.Get<int>(ckey);
                                            try
                                            {
                                                if (client.Send(request).StatusCode == HttpStatusCode.OK)
                                                {
                                                    if (num >= 3)
                                                    {
                                                        _ = PushService.PushAsync("HTTP 监控通知（正常）", item);
                                                    }
                                                    num = 0;
                                                }
                                                else
                                                {
                                                    throw new Exception("error");
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                num++;
                                                if (num == 3)
                                                {
                                                    _ = PushService.PushAsync("HTTP 监控通知（异常）", item);
                                                }
                                            }
                                            CacheTo.Set(ckey, num);
                                        }
                                        break;
                                    case "tcp":
                                        {
                                            var ckey = $"Monitor:{type}:{item}";

                                            var hp = item.Split(' ');
                                            var client = new TcpClient();

                                            var num = CacheTo.Get<int>(ckey);
                                            try
                                            {
                                                if (client.ConnectAsync(Dns.GetHostAddresses(hp.First()).First(), Convert.ToInt32(hp.Last())).Wait(5000))
                                                {
                                                    if (num >= 3)
                                                    {
                                                        _ = PushService.PushAsync("TCP 监控通知（正常）", item);
                                                    }
                                                    num = 0;
                                                }
                                                else
                                                {
                                                    throw new Exception("error");
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                num++;
                                                if (num == 3)
                                                {
                                                    _ = PushService.PushAsync("TCP 监控通知（异常）", item);
                                                }
                                            }
                                            CacheTo.Set(ckey, num);
                                        }
                                        break;
                                    case "ssl":
                                        {
                                            var hnp = item.Split(':');
                                            var hostname = hnp[0];
                                            var port = hnp.Length > 1 ? Convert.ToInt32(hnp[1]) : 443;

                                            var client = new TcpClient(hostname, port);
                                            var sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) =>
                                            {
                                                var outMsg = new List<string>();
                                                if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                                                {
                                                    outMsg.Add($"{sslPolicyErrors} 证书不可用");
                                                }
                                                else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
                                                {
                                                    outMsg.Add($"{sslPolicyErrors} 证书名称不匹配");
                                                }
                                                else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                                                {
                                                    outMsg.Add($"{sslPolicyErrors}");
                                                }

                                                var now = DateTime.Now;
                                                var co = ((X509Certificate2)certificate);

                                                var lastDay = (int)((co.NotAfter - now).TotalDays);
                                                var sslAlertDay = AppTo.GetValue<int>("Monitor:sslAlertDay"); //报警天数
                                                if (lastDay <= sslAlertDay)
                                                {
                                                    outMsg.Add($"有效期: {lastDay} 天\r\n{co.NotBefore:yyyy-MM-dd HH:mm} 至 {co.NotAfter:yyyy-MM-dd HH:mm}");
                                                }

                                                if (outMsg.Count > 0)
                                                {
                                                    outMsg.Insert(0, item);
                                                    _ = PushService.PushAsync("SSL 监控通知", string.Join("\r\n", outMsg));
                                                }

                                                return true;
                                            }), null);

                                            try
                                            {
                                                sslStream.AuthenticateAsClient(hostname);
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex);
                                                client.Close();
                                            }
                                        }
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                ConsoleTo.Log(ex);
                            }
                        }
                    }
                }
                else
                {
                    vm.Set(EnumTo.RTag.refuse);
                    vm.Msg = "未启用";
                }

                if (vm.Code != 0)
                {
                    var vmj = vm.ToJson(true);
                    Console.WriteLine(vmj);
                    ConsoleTo.Log(vmj);
                }

                return vm;
            });
        }

        /// <summary>
        /// WebHook
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> WebHook()
        {
            var vm = new ResultVM();

            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            var postData = ms.ToArray().ToText();
            Console.WriteLine(postData);

            vm.Data = postData.DeJson();
            vm.Set(EnumTo.RTag.success);

            return vm;
        }

        /// <summary>
        /// 静态资源使用分析（已处理到 2021-07）
        /// </summary>
        /// <param name="rootPath">静态资源根目录</param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM StaticResourceUsageAnalysis(string rootPath = @"D:\ROOM\static")
        {
            return ResultVM.Try(vm =>
            {
                vm.LogEvent(le =>
                {
                    Console.WriteLine(le.NewItems[0]);
                });

                var db = ContextBaseFactory.CreateDbContext();
                var uws = db.UserWriting.ToList();
                var urs = db.UserReply.ToList();
                var runs = db.Run.ToList();

                var filesPath = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories).ToList();
                foreach (var path in filesPath)
                {
                    if (!path.Contains(".git") && !path.Contains($@"{rootPath}\static\"))
                    {
                        var filename = Path.GetFileName(path);

                        if (!uws.Any(x => x.UwContent.Contains(filename)) && !urs.Any(x => x.UrContent != null && x.UrContent.Contains(filename)) && !runs.Any(x => x.RunContent1.Contains(filename) || x.RunContent2.Contains(filename)))
                        {
                            vm.Log.Add(path);
                        }
                    }
                }

                return vm;
            });
        }
    }
}