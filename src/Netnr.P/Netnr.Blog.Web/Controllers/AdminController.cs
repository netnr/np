using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using AgGrid.InfiniteRowModel;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 后台管理
    /// </summary>
    [Authorize]
    [FilterConfigs.IsAdmin]
    public class AdminController : Controller
    {
        public ContextBase db;

        public AdminController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 后台管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        #region 文章管理

        /// <summary>
        /// 文章管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Write()
        {
            return View();
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        [HttpGet]
        public object WriteList(string grp)
        {
            var query = from a in db.UserWriting
                        join b in db.UserInfo on a.Uid equals b.UserId
                        select new
                        {
                            a.UwId,
                            a.UwTitle,
                            a.UwCreateTime,
                            a.UwUpdateTime,
                            a.UwReadNum,
                            a.UwReplyNum,
                            a.UwOpen,
                            a.UwStatus,
                            a.UwLaud,
                            a.UwMark,
                            a.UwCategory,

                            b.UserId,
                            b.Nickname,
                            b.UserName,
                            b.UserMail
                        };

            return query.GetInfiniteRowModelBlock(grp);
        }

        /// <summary>
        /// 保存一篇文章（管理）
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM WriteSave([FromForm] UserWriting mo)
        {
            var vm = new ResultVM();

            var uinfo = IdentityService.Get(HttpContext);

            var oldmo = db.UserWriting.FirstOrDefault(x => x.UwId == mo.UwId);

            if (oldmo != null)
            {
                oldmo.UwTitle = mo.UwTitle;
                oldmo.UwStatus = mo.UwStatus;
                oldmo.UwReplyNum = mo.UwReplyNum;
                oldmo.UwReadNum = mo.UwReadNum;
                oldmo.UwLaud = mo.UwLaud;
                oldmo.UwMark = mo.UwMark;
                oldmo.UwOpen = mo.UwOpen;

                db.UserWriting.Update(oldmo);

                int num = db.SaveChanges();

                vm.Set(num > 0);
            }

            return vm;
        }

        #endregion

        #region 回复管理

        /// <summary>
        /// 回复管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Reply()
        {
            return View();
        }

        /// <summary>
        /// 查询回复
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        [HttpGet]
        public object ReplyList(string grp)
        {
            var query = from a in db.UserReply
                        join b1 in db.UserInfo on a.Uid equals b1.UserId into bg
                        from b in bg.DefaultIfEmpty()
                        select new
                        {
                            a.UrId,
                            a.Uid,
                            a.UrAnonymousName,
                            a.UrAnonymousLink,
                            a.UrAnonymousMail,
                            a.UrTargetType,
                            a.UrTargetId,
                            a.UrContent,
                            a.UrContentMd,
                            a.UrCreateTime,
                            a.UrStatus,
                            a.UrTargetPid,
                            a.Spare1,
                            a.Spare2,
                            a.Spare3,

                            UserId = b == null ? 0 : b.UserId,
                            Nickname = b == null ? null : b.Nickname,
                            UserName = b == null ? null : b.UserName,
                            UserMail = b == null ? null : b.UserMail
                        };

            return query.GetInfiniteRowModelBlock(grp);
        }

        /// <summary>
        /// 保存一条回复（管理）
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM ReplySave([FromForm] UserReply mo)
        {
            var vm = new ResultVM();

            var uinfo = IdentityService.Get(HttpContext);

            var oldmo = db.UserReply.FirstOrDefault(x => x.UrId == mo.UrId);

            if (oldmo != null)
            {
                oldmo.UrAnonymousName = mo.UrAnonymousName;
                oldmo.UrAnonymousMail = mo.UrAnonymousMail;
                oldmo.UrAnonymousLink = mo.UrAnonymousLink;

                oldmo.UrContent = mo.UrContent;
                oldmo.UrContentMd = mo.UrContentMd;

                oldmo.UrStatus = mo.UrStatus;

                db.UserReply.Update(oldmo);

                int num = db.SaveChanges();

                vm.Set(num > 0);
            }

            return vm;
        }

        #endregion

        #region 日志管理

        /// <summary>
        /// 日志页面
        /// </summary>
        /// <returns></returns>l
        public IActionResult Log()
        {
            return View();
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="queryAllSql"></param>
        /// <param name="queryLimitSql"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 10), HttpGet]
        public LoggingResultVM QueryLog(string queryAllSql, string queryLimitSql)
        {
            return LoggingTo.Query(queryAllSql, queryLimitSql);
        }

        /// <summary>
        /// 日志图表
        /// </summary>
        /// <returns></returns>
        public IActionResult LogChart()
        {
            return View();
        }

        /// <summary>
        /// 查询日志流量
        /// </summary>
        /// <param name="days">类型</param>
        /// <param name="LogGroup">分组</param>
        /// <returns></returns>
        [ResponseCache(Duration = 10), HttpGet]
        public LoggingResultVM QueryLogStatsPVUV(int? days, string LogGroup)
        {
            var listWhere = new List<List<string>>();
            if (!string.IsNullOrWhiteSpace(LogGroup))
            {
                listWhere = new List<List<string>>
                {
                    new List<string> { "LogGroup", "=", LogGroup }
                };
            }

            var vm = LoggingTo.StatsPVUV(days ?? 0, listWhere);
            return vm;
        }

        /// <summary>
        /// 查询日志Top
        /// </summary>
        /// <param name="days">类型</param>
        /// <param name="field">属性字段</param>
        /// <param name="LogGroup">分组</param>
        /// <returns></returns>
        [ResponseCache(Duration = 10), HttpGet]
        public LoggingResultVM QueryLogStatsTop(int? days, string field, string LogGroup)
        {
            var listWhere = new List<List<string>>();
            if (!string.IsNullOrWhiteSpace(LogGroup))
            {
                listWhere = new List<List<string>>
                {
                    new List<string> { "LogGroup", "=", LogGroup }
                };
            }

            var vm = LoggingTo.StatsTop(days ?? 0, field, listWhere);
            return vm;
        }

        #endregion

        #region 键值

        /// <summary>
        /// 字典
        /// </summary>
        /// <returns></returns>
        public IActionResult KeyValues()
        {
            return View();
        }

        /// <summary>
        /// 键值 命令
        /// </summary>
        /// <param name="id"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM KeyVal([FromRoute] string id, string keys)
        {
            return ResultVM.Try(vm =>
            {
                var listKey = string.IsNullOrWhiteSpace(keys) ? new List<string>() : keys.Split(',').ToList();

                switch (id?.ToLower())
                {
                    case "grab":
                        {
                            listKey.ForEach(key =>
                            {
                                string api = $"https://baike.baidu.com/api/openapi/BaikeLemmaCardApi?scope=103&format=json&appid=379020&bk_key={key.ToUrlEncode()}&bk_length=600";
                                try
                                {
                                    var result = HttpTo.Get(api);
                                    if (result.Length > 100)
                                    {
                                        var mo = db.KeyValues.FirstOrDefault(x => x.KeyName == key);
                                        if (mo == null)
                                        {
                                            mo = new KeyValues
                                            {
                                                KeyId = Guid.NewGuid().ToString(),
                                                KeyName = key.ToLower(),
                                                KeyValue = result
                                            };
                                            db.KeyValues.Add(mo);
                                        }
                                        else
                                        {
                                            mo.KeyValue = result;
                                            db.KeyValues.Update(mo);
                                        }

                                        var num = db.SaveChanges();
                                        vm.Log.Add($"Done {key} {num}");
                                    }
                                    else
                                    {
                                        vm.Log.Add($"Fatal {key} {result}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                    vm.Log.Add($"{key} Error {ex.Message}");
                                }
                            });
                        }
                        break;
                    case "synonym":
                        {
                            string mainKey = listKey.First().ToLower();
                            listKey.RemoveAt(0);

                            var listkvs = new List<KeyValueSynonym>();
                            listKey.ForEach(key =>
                            {
                                var kvs = new KeyValueSynonym
                                {
                                    KsId = Guid.NewGuid().ToString(),
                                    KeyName = mainKey,
                                    KsName = key.ToLower()
                                };
                                listkvs.Add(kvs);
                            });

                            var mo = db.KeyValueSynonym.FirstOrDefault(x => x.KeyName == mainKey);
                            if (mo != null)
                            {
                                db.KeyValueSynonym.Remove(mo);
                            }
                            db.KeyValueSynonym.AddRange(listkvs);

                            int num = db.SaveChanges();
                            vm.Log.Add($"Done {mainKey} {num}");
                        }
                        break;
                    case "tag":
                        {
                            if (listKey.Count > 0)
                            {
                                var mt = db.Tags.Where(x => listKey.Contains(x.TagName)).ToList();
                                if (mt.Count == 0)
                                {
                                    var listMo = new List<Tags>();
                                    var tagHs = new HashSet<string>();
                                    foreach (var tag in listKey)
                                    {
                                        if (tagHs.Add(tag))
                                        {
                                            var mo = new Tags
                                            {
                                                TagName = tag,
                                                TagCode = tag,
                                                TagStatus = 1,
                                                TagHot = 0,
                                                TagIcon = tag + ".svg"
                                            };
                                            listMo.Add(mo);
                                        }
                                    }
                                    tagHs.Clear();

                                    //新增&刷新缓存
                                    db.Tags.AddRange(listMo);
                                    var num = db.SaveChanges();

                                    CommonService.TagsQuery(false);
                                    vm.Log.Add($"Done 已刷新缓存 {num}");
                                }
                                else
                                {
                                    vm.Log.Add($"Done 标签已存在 {mt.ToJson()}");
                                }
                            }
                            else
                            {
                                CommonService.TagsQuery(false);
                                vm.Log.Add($"Done 已刷新缓存");
                            }
                        }
                        break;
                }

                vm.Set(EnumTo.RTag.success);
                return vm;
            });
        }

        #endregion

        #region 表管理

        public IActionResult Table()
        {
            return View();
        }

        [HttpGet]
        public ResultVM TableQuery(string queryAllSql, string queryLimitSql)
        {
            return ResultVM.Try(vm =>
            {
                var conn = db.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                var cmd = conn.CreateCommand();
                cmd.CommandText = queryLimitSql;
                var reader = cmd.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                cmd.CommandText = queryAllSql;
                var count = cmd.ExecuteScalar();

                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }

                vm.Data = new { table, count };
                vm.Set(EnumTo.RTag.success);

                return vm;
            });
        }

        [HttpGet]
        public ResultVM TableMeta(string name, string tableName)
        {
            return ResultVM.Try(vm =>
            {
                var conn = db.Database.GetDbConnection();
                var dk = new DataKitTo(AppTo.TDB, conn);

                switch (name)
                {
                    case "table":
                        vm.Data = dk.GetTable();
                        vm.Log.Add(AppTo.TDB);
                        break;
                    case "column": vm.Data = dk.GetColumn(tableName); break;
                }

                vm.Set(EnumTo.RTag.success);

                return vm;
            });
        }

        #endregion

        #region 清理全局缓存

        public IActionResult ClearCache()
        {
            GlobalTo.Memorys.Clear();
            CacheTo.RemoveAll();
            return Ok("Done!");
        }

        #endregion

        #region 生成脚本服务


        /// <summary>
        /// 构建静态文件
        /// </summary>
        /// <returns></returns>
        public ResultVM Build()
        {
            var vm = BuildHtml<SSController>();
            return vm;
        }

        /// <summary>
        /// 根据控制器构建静态页面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ResultVM BuildHtml<T>() where T : Controller
        {
            var vm = new ResultVM();

            try
            {
                Console.WriteLine("BuildHtml Start ...");
                GlobalTo.Memorys.Clear();
                CacheTo.RemoveAll();

                AppContext.SetSwitch("Netnr.BuildHtml", true);

                //反射 action
                var type = typeof(T);
                var methods = type.GetMethods().Where(x => x.DeclaringType == type).ToList();
                var ctrlName = type.Name.Replace("Controller", "").ToLower();

                //访问前缀
                var urlPrefix = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{ctrlName}/";

                vm.Log.Add($"Build Count：{methods.Count}");

                var cbs = new ConcurrentBag<string>();
                //并行请求
                Parallel.ForEach(methods, mh =>
                {
                    Console.WriteLine(mh.Name);

                    cbs.Add(mh.Name);
                    string html = HttpTo.Get(urlPrefix + mh.Name);
                    var savePath = $"{AppTo.WebRootPath}/{mh.Name.ToLower()}.html";
                    FileTo.WriteText(html, savePath, false);
                });
                vm.Log.AddRange(cbs);
                Console.WriteLine("\nDone!\n");

                vm.Set(EnumTo.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }
            finally
            {
                AppContext.SetSwitch("Netnr.BuildHtml", false);
            }

            return vm;
        }

        #endregion
    }
}