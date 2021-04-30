using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Netnr.SharedFast;
using System.IO;

namespace Netnr.Blog.Application
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class TaskService
    {
        /// <summary>
        /// 任务组件
        /// </summary>
        public class TaskComponent
        {
            /// <summary>
            /// 任务注册
            /// </summary>
            public class Reg : Registry
            {
                /// <summary>
                /// 构造
                /// </summary>
                public Reg()
                {
                    Schedule<BackupDataBaseJob>().ToRunEvery(1).Days().At(5, 5);

                    Schedule<GistSyncJob>().ToRunEvery(2).Hours();

                    Schedule<HandleOperationRecordJob>().ToRunEvery(30).Minutes();
                }
            }

            /// <summary>
            /// 数据库备份任务
            /// </summary>
            public class BackupDataBaseJob : IJob
            {
                void IJob.Execute()
                {
                    Core.ConsoleTo.Log(BackupDataBase().ToJson());
                }
            }

            /// <summary>
            /// Gist同步任务
            /// </summary>
            public class GistSyncJob : IJob
            {
                void IJob.Execute()
                {
                    Core.ConsoleTo.Log(GistSync().ToJson());
                }
            }

            /// <summary>
            /// 处理操作记录
            /// </summary>
            public class HandleOperationRecordJob : IJob
            {
                void IJob.Execute()
                {
                    Core.ConsoleTo.Log(HandleOperationRecord().ToJson());
                }
            }
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        public static SharedResultVM BackupDataBase()
        {
            var vm = new SharedResultVM();

            try
            {
                var listMsg = new List<object>() { "备份数据库" };


                var kp = $"Work:BackupDataBase:{GlobalTo.TDB}:";

                if (GlobalTo.GetValue<bool>(kp + "enable") == true)
                {
                    var cmd = GlobalTo.GetValue(kp + "cmd");
                    var sql = GlobalTo.GetValue(kp + "sql");

                    if (!string.IsNullOrWhiteSpace(cmd))
                    {
                        var er = Core.CmdTo.Execute(cmd).CrOutput;
                        vm.Log.Add($"执行CMD：{cmd}");
                        vm.Log.Add($"结果输出：{er}");
                    }

                    if (!string.IsNullOrWhiteSpace(sql))
                    {
                        using var conn = Data.ContextBaseFactory.CreateDbContext().Database.GetDbConnection();
                        conn.Open();
                        var connCmd = conn.CreateCommand();
                        connCmd.CommandText = sql;
                        int en = connCmd.ExecuteNonQuery();

                        vm.Log.Add($"执行SQL：{sql}");
                        vm.Log.Add($"受影响行数：{en}");
                    }

                    vm.Set(SharedEnum.RTag.success);
                }
                else
                {
                    vm.Set(SharedEnum.RTag.lack);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 导出数据库
        /// </summary>
        /// <returns></returns>
        public static SharedResultVM ExportDataBase()
        {
            var vm = new SharedResultVM();

            try
            {
                using var db = Data.ContextBaseFactory.CreateDbContext();
                var dicDbSet = Data.ContextBase.GetDicDbSet(db);

                var rows = 0;
                var dicOut = new Dictionary<string, object> { };
                foreach (var key in dicDbSet.Keys)
                {
                    var dbset = dicDbSet[key] as IQueryable<object>;
                    var list = dbset.ToList();
                    rows += list.Count;

                    dicOut.Add(key, list);
                }

                vm.Data = dicOut;

                vm.Log.Add($"时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                vm.Log.Add($"驱动：{db.Database.ProviderName}");

                var conn = db.Database.GetDbConnection();
                vm.Log.Add($"数据库：{conn.Database}");
                conn.Open();
                vm.Log.Add($"版本号：{conn.ServerVersion}");
                conn.Close();

                vm.Log.Add($"导出数据表 {dicOut.Keys.Count} 个，共 {rows} 行");

                vm.Set(SharedEnum.RTag.success);

                var fullPath = Path.Combine(Path.GetTempPath(), "data.json");
                Core.FileTo.WriteText(vm.ToJson(), fullPath, false);

                vm.Data = null;
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 导出示例数据
        /// </summary>
        /// <returns></returns>
        public static SharedResultVM ExportSampleData()
        {
            var vm = new SharedResultVM();

            try
            {
                using var db = Data.ContextBaseFactory.CreateDbContext();
                var dicOut = new Dictionary<string, object> { };

                dicOut["UserInfo"] = (from a in db.UserInfo
                                      where a.UserId == 1
                                      select new
                                      {
                                          UserId = 0,
                                          UserName = "netnr",
                                          UserPwd = "e10adc3949ba59abbe56e057f20f883e", //123456

                                          a.UserCreateTime,
                                          a.UserLoginTime,
                                          a.LoginLimit,
                                          a.UserSign,
                                          a.Nickname,
                                          a.UserPhoto,
                                          a.UserSex,
                                          a.UserMail,
                                          a.UserSay
                                      }).ToList();

                dicOut["Tags"] = (from a in db.Tags
                                  where a.TagId == 58 || a.TagId == 96
                                  select new
                                  {
                                      TagId = 0,
                                      a.TagName,
                                      a.TagIcon,
                                      a.TagStatus,
                                      a.TagHot
                                  }).ToList();

                dicOut["UserWriting"] = (from a in db.UserWriting
                                         where a.UwId == 117
                                         select new
                                         {
                                             UwId = 0,
                                             a.Uid,
                                             a.UwCategory,
                                             a.UwTitle,
                                             a.UwContent,
                                             a.UwContentMd,
                                             a.UwCreateTime,
                                             a.UwUpdateTime,
                                             a.UwLastUid,
                                             a.UwLastDate,
                                             a.UwReplyNum,
                                             a.UwReadNum,
                                             a.UwOpen,
                                             a.UwLaud,
                                             a.UwMark,
                                             a.UwStatus
                                         }).ToList();

                dicOut["UserWritingTags"] = (from a in db.UserWritingTags
                                             where a.UwId == 117
                                             select new
                                             {
                                                 UwtId = 0,
                                                 UwId = 1,
                                                 TagId = 1,
                                                 a.TagName
                                             }).ToList();

                dicOut["UserReply"] = (from a in db.UserReply
                                       where a.UrTargetId == "117"
                                       orderby a.UrCreateTime
                                       select new
                                       {
                                           UrId = 0,
                                           a.Uid,
                                           a.UrAnonymousName,
                                           a.UrAnonymousLink,
                                           a.UrAnonymousMail,
                                           a.UrTargetType,
                                           UrTargetId = 1,
                                           a.UrContent,
                                           a.UrContentMd,
                                           a.UrCreateTime,
                                           a.UrStatus,
                                           a.UrTargetPid
                                       }).Take(3).ToList();

                dicOut["Run"] = db.Run.OrderBy(x => x.RunCreateTime).Take(1).ToList();

                dicOut["KeyValues"] = db.KeyValues.Where(x => x.KeyName == "https" || x.KeyName == "browser").ToList();

                dicOut["Gist"] = db.Gist.Where(x => x.GistCode == "5373307231488995367").ToList();

                dicOut["GistSync"] = db.GistSync.Where(x => x.GistCode == "5373307231488995367").ToList();

                dicOut["Draw"] = db.Draw.Where(x => x.DrId == "d4969500168496794720" || x.DrId == "m4976065893797151245").ToList();

                dicOut["DocSet"] = db.DocSet.Where(x => x.DsCode == "4840050256984581805" || x.DsCode == "5036967707833574483").ToList();

                dicOut["DocSetDetail"] = db.DocSetDetail.Where(x => x.DsCode == "4840050256984581805" || x.DsCode == "5036967707833574483").ToList();

                vm.Data = dicOut;

                vm.Log.Add($"时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                vm.Log.Add($"驱动：{db.Database.ProviderName}");

                var conn = db.Database.GetDbConnection();
                vm.Log.Add($"数据库：{conn.Database}");
                conn.Open();
                vm.Log.Add($"版本号：{conn.ServerVersion}");
                conn.Close();

                vm.Set(SharedEnum.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// Gist代码片段，同步到GitHub、Gitee
        /// </summary>
        /// <returns></returns>
        public static SharedResultVM GistSync()
        {
            var vm = new SharedResultVM();

            try
            {
                if (GlobalTo.GetValue<bool>("Work:GistSync:enable"))
                {
                    using var db = Data.ContextBaseFactory.CreateDbContext();

                    //同步用户ID
                    int UserId = GlobalTo.GetValue<int>("Work:GistSync:UserId");

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

                    var token_gh = GlobalTo.GetValue("ApiKey:GitHub:GistToken");
                    var token_ge = GlobalTo.GetValue("ApiKey:Gitee:GistToken");

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

                            //发送主体
                            #region MyRegion
                            var jo = new JObject
                            {
                                ["access_token"] = token_ge,//only gitee 

                                ["description"] = gist.GistRemark,
                                ["public"] = gist.GistOpen == 1
                            };

                            var jc = new JObject
                            {
                                ["content"] = gist.GistContent
                            };

                            var jf = new JObject
                            {
                                [gist.GistFilename] = jc
                            };

                            jo["files"] = jf;
                            #endregion

                            switch (st)
                            {
                                case "add":
                                    {
                                        var gsmo = new Domain.GistSync()
                                        {
                                            GistCode = key,
                                            Uid = UserId,
                                            GistFilename = gist.GistFilename
                                        };

                                        //GitHub
                                        {
                                            var hwr = Core.HttpTo.HWRequest("https://api.github.com/gists", "POST", jo.ToJson());
                                            hwr.Headers.Add(HttpRequestHeader.Authorization, "token " + token_gh);
                                            hwr.ContentType = "application/json";
                                            hwr.UserAgent = "Netnr Agent";

                                            var rt = Core.HttpTo.Url(hwr);

                                            gsmo.GsGitHubId = rt.ToJObject()["id"].ToString();
                                            gsmo.GsGitHubTime = gist.GistUpdateTime;
                                        }

                                        //Gitee
                                        {
                                            var hwr = Core.HttpTo.HWRequest("https://gitee.com/api/v5/gists", "POST", jo.ToJson());
                                            hwr.ContentType = "application/json";

                                            var rt = Core.HttpTo.Url(hwr);

                                            gsmo.GsGiteeId = rt.ToJObject()["id"].ToString();
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
                                            var hwr = Core.HttpTo.HWRequest("https://api.github.com/gists/" + gs.GsGitHubId, "PATCH", jo.ToJson());
                                            hwr.Headers.Add(HttpRequestHeader.Authorization, "token " + token_gh);
                                            hwr.ContentType = "application/json";
                                            hwr.UserAgent = "Netnr Agent";

                                            _ = Core.HttpTo.Url(hwr);

                                            gs.GsGitHubTime = gist.GistUpdateTime;
                                        }

                                        //Gitee
                                        {
                                            var hwr = Core.HttpTo.HWRequest("https://gitee.com/api/v5/gists/" + gs.GsGiteeId, "PATCH", jo.ToJson());
                                            hwr.ContentType = "application/json";

                                            _ = Core.HttpTo.Url(hwr);

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
                            var hwr_gh = Core.HttpTo.HWRequest("https://api.github.com/gists/" + gs.GsGitHubId, "DELETE");
                            hwr_gh.Headers.Add(HttpRequestHeader.Authorization, "token " + token_gh);
                            hwr_gh.UserAgent = "Netnr Agent";
                            var resp_gh = (HttpWebResponse)hwr_gh.GetResponse();
                            if (resp_gh.StatusCode == HttpStatusCode.NoContent)
                            {
                                dc[0] = '1';
                            }
                            #endregion

                            #region Gitee
                            var hwr_ge = Core.HttpTo.HWRequest("https://gitee.com/api/v5/gists/" + gs.GsGiteeId + "?access_token=" + token_ge, "DELETE");
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

                    vm.Set(SharedEnum.RTag.success);
                    vm.Data = listLog;
                }
                else
                {
                    vm.Set(SharedEnum.RTag.lack);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                Console.WriteLine(ex);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                Core.ConsoleTo.Log(ex);
            }

            return vm;
        }

        /// <summary>
        /// 处理操作记录
        /// </summary>
        /// <returns></returns>
        public static SharedResultVM HandleOperationRecord()
        {
            var vm = new SharedResultVM();

            try
            {
                if (GlobalTo.GetValue<bool>("Work:HOR:enable"))
                {
                    using var db = Data.ContextBaseFactory.CreateDbContext();

                    //处理Guff查询记录数
                    var ctype = EnumService.ConnectionType.GuffRecord.ToString();
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
                        vm.Set(SharedEnum.RTag.lack);
                    }
                }
                else
                {
                    vm.Set(SharedEnum.RTag.lack);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

    }
}
