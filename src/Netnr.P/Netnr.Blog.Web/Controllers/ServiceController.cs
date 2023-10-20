namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 服务
    /// </summary>
    [Route("[Controller]/[action]")]
    public class ServiceController : Controller
    {
        public ContextBase db;

        public ServiceController(ContextBase cb)
        {
            db = cb;
        }

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
        public async Task Weixin(string signature, string timestamp, string nonce, string echostr, string encrypt_type, string msg_signature)
        {
            string result = null;

            //微信公众号验证地址
            if (Request.Method == "GET")
            {
                await Console.Out.WriteLineAsync(msg_signature);

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
                ConsoleTo.WriteCard("Receive WeixinMP Message", postData);

                //处理消息并回复
                result = WeixinMPService.MessageReply(postData);
            }

            //输出
            var buffer = result.ToByte();
            await Response.Body.WriteAsync(buffer);
            await Response.Body.FlushAsync();
        }

        /// <summary>
        /// 导出样本数据（开发环境）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public async Task<ResultVM> ExportSampleData()
        {
            var vm = new ResultVM();

            try
            {
                if (BaseTo.IsDev)
                {
                    var export_before = "static/sample_before.zip";
                    var export_demo = "static/sample.zip";

                    //备份
                    if ((await DatabaseExport(export_before)).Code == 200)
                    {
                        //清理仅保留示例数据

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

                        db.GiftRecord.RemoveRange(db.GiftRecord.ToList());
                        db.GiftRecordDetail.RemoveRange(db.GiftRecordDetail.ToList());

                        db.Draw.RemoveRange(db.Draw.Where(x => x.DrId != "d4969500168496794720" && x.DrId != "m4976065893797151245").ToList());

                        db.DocSet.RemoveRange(db.DocSet.Where(x => x.DsCode != "4840050256984581805" && x.DsCode != "5036967707833574483").ToList());
                        db.DocSetDetail.RemoveRange(db.DocSetDetail.Where(x => x.DsCode != "4840050256984581805" && x.DsCode != "5036967707833574483").ToList());

                        var num = db.SaveChanges();

                        //导出示例数据
                        vm = await DatabaseExport(export_demo, true);

                        //导入恢复
                        if ((await DatabaseImport(export_before, true)).Code == 200)
                        {
                            var fullPath = Path.Combine(AppTo.ContentRootPath, export_before);
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                else
                {
                    vm.Set(RCodeTypes.refuse);
                    vm.Msg = "仅限开发环境使用";
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 数据库导出（管理员）
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <param name="onlyData">仅数据，默认带表结构</param>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public async Task<ResultVM> DatabaseExport(string zipName = "db/backup.zip", bool onlyData = false)
        {
            var vm = new ResultVM();

            try
            {
                var edb = new DataKitTransfer.ExportDatabase
                {
                    PackagePath = Path.Combine(AppTo.ContentRootPath, zipName),
                    ReadConnectionInfo = new DbKitConnectionOption()
                    {
                        ConnectionType = AppTo.DBT,
                        Connection = db.Database.GetDbConnection()
                    },
                    ExportType = onlyData ? "onlyData" : "all"
                };

                if (System.IO.File.Exists(edb.PackagePath))
                {
                    System.IO.File.Delete(edb.PackagePath);
                }
                vm = await DataKitTo.ExportDataTable(await edb.AsExportDataTable());
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 数据库导入（管理员）
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <param name="clearTable">清空表，默认 false</param>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public async Task<ResultVM> DatabaseImport(string zipName = "db/backup.zip", bool clearTable = false)
        {
            var vm = new ResultVM();

            try
            {
                var idb = new DataKitTransfer.ImportDatabase
                {
                    WriteConnectionInfo = new DbKitConnectionOption
                    {
                        ConnectionType = AppTo.DBT,
                        Connection = db.Database.GetDbConnection()
                    },
                    PackagePath = Path.Combine(AppTo.ContentRootPath, zipName),
                    WriteDeleteData = clearTable
                };

                vm = await DataKitTo.ImportDatabase(idb);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 数据库备份到 Git（管理员）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public async Task<ResultVM> DatabaseBackupToGit()
        {
            var vm = new ResultVM();

            try
            {
                if (AppTo.GetValue<bool?>("DisableDatabaseWrite") == true)
                {
                    vm.Set(RCodeTypes.refuse);
                    return vm;
                }

                var now = $"{DateTime.Now:yyyyMMdd_HHmmss}";

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

                await Task.Delay(1000);

                //备份数据
                var zipPath = $"db/backup_{now}.zip";
                if ((await DatabaseExport(zipPath)).Code == 200)
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
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
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
                //处理Guff查询记录数
                var ctype = ConnectionTypes.GuffRecord.ToString();
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
                    vm.Set(RCodeTypes.failure);
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
            vm.Set(RCodeTypes.success);

            return vm;
        }
    }
}