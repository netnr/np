namespace Netnr.Blog.Web.Services
{
    /// <summary>
    /// 作业
    /// </summary>
    public partial class WorkService
    {
        /// <summary>
        /// 处理操作记录
        /// </summary>
        /// <returns></returns>
        public static async Task<ResultVM> ExecuteOperationRecord()
        {
            var vm = new ResultVM();

            try
            {
                //处理Guff查询记录数
                var ctype = ConnectionTypes.GuffRecord.ToString();

                var db = ContextBaseFactory.CreateDbContext();
                var listOr = await db.OperationRecord.Where(x => x.OrType == ctype && x.OrMark == "default").ToListAsync();
                if (listOr.Count > 0)
                {
                    var listAllId = string.Join(",", listOr.Select(x => x.OrSource).ToList()).Split(',').ToList();
                    var listid = listAllId.Distinct();

                    var listmo = await db.GuffRecord.Where(x => listid.Contains(x.GrId)).ToListAsync();
                    foreach (var item in listmo)
                    {
                        item.GrReadNum += listAllId.GroupBy(x => x).FirstOrDefault(x => x.Key == item.GrId).Count();
                    }
                    db.GuffRecord.UpdateRange(listmo);

                    db.OperationRecord.RemoveRange(listOr);

                    int num = await db.SaveChangesAsync();

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
        /// 备份到 Git
        /// </summary>
        /// <returns></returns>
        public static async Task<ResultVM> ExecuteBackupToGit()
        {
            var vm = new ResultVM();

            try
            {
                if (AppTo.GetValue<bool?>("ProgramParameters:DisableDatabaseWrite") == true)
                {
                    vm.Set(RCodeTypes.refuse);
                    return vm;
                }

                var now = $"{DateTime.Now:yyyyMMdd_HHmmss}";

                var db = ContextBaseFactory.CreateDbContext();
                var database = db.Database.GetDbConnection().Database;

                var createScript = db.Database.GenerateCreateScript();

                //备份创建脚本
                var b1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(createScript));
                var p1 = $"{database}/backup_{now}.sql";

                vm.Log.Add(await GiteePost(b1, p1, now));
                vm.Log.Add(await GitHubPut(b1, p1, now));

                await Task.Delay(1000);

                //备份数据
                var vmBackup = await ExecuteBackupData();
                if (vmBackup.Code == 200)
                {
                    var zipFile = vmBackup.Data.ToString();
                    var b2 = Convert.ToBase64String(File.ReadAllBytes(zipFile));
                    var p2 = $"{database}/backup_{now}.zip";

                    vm.Log.Add(await GiteePost(b2, p2, now));
                    vm.Log.Add(await GitHubPut(b2, p2, now));

                    File.Delete(zipFile);
                }

                var vmj = vm.ToJson(true);
                ConsoleTo.LogCard(nameof(ExecuteBackupToGit), vmj);
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
        private static async Task<string> GitHubPut(string content, string path, string message = "m", string token = null, string or = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    token = AppTo.GetValue("ApiKey:GitHub:GistToken");
                }
                if (string.IsNullOrWhiteSpace(or))
                {
                    or = AppTo.GetValue("ProgramParameters:AdminBackupToGit");
                }

                var url = $"https://api.github.com/repos/{or}/contents/{path}";

                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
                client.DefaultRequestHeaders.Accept.TryParseAdd("application/vnd.github.v3+json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", token);

                var requestContent = new StringContent(new { message, content }.ToJson(), Encoding.UTF8, "application/json");
                var resp = await client.PutAsync(url, requestContent);

                if (resp.IsSuccessStatusCode)
                {
                    var read = await resp.Content.ReadAsStringAsync();
                    return read;
                }
                else
                {
                    return $"{(int)resp.StatusCode} {resp.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return ex.ToJson();
            }
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
        private static async Task<string> GiteePost(string content, string path, string message = "m", string token = null, string or = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    token = AppTo.GetValue("ApiKey:Gitee:GistToken");
                }
                if (string.IsNullOrWhiteSpace(or))
                {
                    or = AppTo.GetValue("ProgramParameters:AdminBackupToGit");
                }

                var listor = or.Split('/');
                var owner = listor.First();
                var repo = listor.Last();
                var url = $"https://gitee.com/api/v5/repos/{owner}/{repo}/contents/{path}";

                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");

                var requestContent = new StringContent(new { access_token = token, message, content }.ToJson(), Encoding.UTF8, "application/json");
                var resp = await client.PostAsync(url, requestContent);

                if (resp.IsSuccessStatusCode)
                {
                    var read = await resp.Content.ReadAsStringAsync();
                    return read;
                }
                else
                {
                    return $"{(int)resp.StatusCode} {resp.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return ex.ToJson();
            }
        }

        /// <summary>
        /// 数据备份
        /// </summary>
        /// <returns></returns>
        public static async Task<ResultVM> ExecuteBackupData()
        {
            var vm = new ResultVM();

            try
            {
                var db = ContextBaseFactory.CreateDbContext();

                var edb = new DataKitTransfer.ExportDatabase
                {
                    ReadConnectionInfo = new DbKitConnectionOption
                    {
                        ConnectionType = AppTo.DBT,
                        Connection = db.Database.GetDbConnection()
                    },
                    ExportType = "dataOnly",
                    PackagePath = Path.Combine(AppTo.ContentRootPath, $"static/{DateTime.Now:yyyyMMddHHmm}.zip"),

                };

                if (File.Exists(edb.PackagePath))
                {
                    File.Delete(edb.PackagePath);
                }
                vm = await DataKitTo.ExportDataTable(await edb.AsExportDataTable());
                vm.Data = edb.PackagePath;
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 斗鱼房间在线状态
        /// </summary>
        /// <returns></returns>
        public static async Task<ResultVM> ExecuteDouyuRoomOnlineStatus()
        {
            var vm = new ResultVM();

            var now = DateTime.Now;
            if ((now - BaseTo.StartTime).TotalMinutes > 5)
            {
                var client = HttpTo.BuildClient();
                var roomIds = AppTo.GetValue("ProgramParameters:CrontabDouyuRooms").Split(',');
                foreach (var roomId in roomIds)
                {
                    if (vm.Log.Count > 0)
                    {
                        vm.Log.Add("-----");
                    }
                    vm.Log.Add($"roomId: {roomId}");

                    try
                    {
                        client.DefaultRequestHeaders.Referrer = new Uri("https://douyu.com/{id}");

                        var url = $"http://open.douyucdn.cn/api/RoomApi/room/{roomId}";
                        var result = await client.GetStringAsync(url);
                        var json = result.DeJson();

                        if (json.GetValue<int>("error") == 0)
                        {
                            json = json.GetProperty("data");

                            //简短信息
                            var dictShort = new Dictionary<string, string>();
                            foreach (var property in json.EnumerateObject())
                            {
                                if (property.Name != "gift")
                                {
                                    dictShort.Add(property.Name, json.GetValue(property.Name));
                                }
                            }
                            if (roomIds.Length > 1)
                            {
                                vm.Log.Add(dictShort.ToJson());
                            }
                            else
                            {
                                vm.Data = dictShort;
                            }

                            var ckey = $"douyu-room-{roomId}";
                            var cval = CacheTo.Get<DateTime?>(ckey);

                            var start_time = json.GetValue("start_time");
                            var owner_name = json.GetValue("owner_name");
                            var room_name = json.GetValue("room_name");

                            //上线
                            var online = json.GetValue<int>("online");
                            if (online > 0)
                            {
                                if (cval.HasValue)
                                {
                                    CacheTo.Set(ckey, cval);
                                }
                                else
                                {
                                    await PushService.PushWeChat($"[Live] {owner_name}", $"标题: {room_name}\r\n时间: {start_time}\r\nhttps://api.netnr.eu.org/douyu/{roomId}");
                                    CacheTo.Set(ckey, now);
                                }
                            }
                            else
                            {
                                if (cval.HasValue)
                                {
                                    await PushService.PushWeChat($"[End] {owner_name}", $"标题: {room_name}\r\n时间: {start_time}\r\nhttps://api.netnr.eu.org/douyu/{roomId}");
                                }
                                CacheTo.Remove(ckey);
                            }
                        }
                        else
                        {
                            vm.Log.Add(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        vm.Log.Add($"Exception: {ex.Message}");
                    }

                    await Task.Delay(1000 * 3);
                }
            }

            vm.Set(RCodeTypes.success);
            return vm;
        }

    }
}
