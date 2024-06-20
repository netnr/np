using AngleSharp.Css.Values;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 服务
    /// </summary>
    [Route("[Controller]/[action]")]
    public class ServiceController(ContextBase cb) : Controller
    {
        public ContextBase db = cb;

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
                using var reader = new StreamReader(Request.Body);
                var requestBody = await reader.ReadToEndAsync();

                //using var ms = new MemoryStream();
                //await Request.Body.CopyToAsync(ms);
                //var postData = ms.ToArray().ToText();

                //解密
                if (encrypt_type == "aes")
                {
                    var token = AppTo.GetValue("ApiKey:WeixinMP:Token");
                    var encodingAESKey = AppTo.GetValue("ApiKey:WeixinMP:EncodingAESKey");
                    var appId = AppTo.GetValue("ApiKey:WeixinMP:AppID");
                }
                ConsoleTo.WriteCard(nameof(Weixin), requestBody);

                //处理消息并回复
                result = WeixinMPService.MessageReply(requestBody);
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
                        if ((await DatabaseImport(export_before, true, true)).Code == 200)
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
        public async Task<ResultVM> DatabaseExport(string zipName = "static/backup.zip", bool onlyData = false)
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
        /// <param name="deleteData">删除表数据，默认 false</param>
        /// <param name="realTimePrint">实时打印日志</param>
        /// <returns></returns>
        [HttpGet]
        [FilterConfigs.IsAdmin]
        public async Task<ResultVM> DatabaseImport(string zipName = "static/backup.zip", bool deleteData = false, bool realTimePrint = false)
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
                    WriteDeleteData = deleteData
                };

                if (realTimePrint)
                {
                    vm = await DataKitTo.ImportDatabase(idb, le =>
                    {
                        Console.WriteLine(le.NewItems[0]);
                    });
                }
                else
                {
                    vm = await DataKitTo.ImportDatabase(idb);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

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

            using var reader = new StreamReader(Request.Body);
            var requestBody = await reader.ReadToEndAsync();
            Console.WriteLine(requestBody);

            vm.Data = requestBody.DeJson();
            vm.Set(RCodeTypes.success);

            return vm;
        }
    }
}