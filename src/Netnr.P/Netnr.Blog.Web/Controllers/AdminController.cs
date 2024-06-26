﻿namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 后台管理
    /// </summary>
    [Authorize]
    [FilterConfigs.IsAdmin]
    public class AdminController(ContextBase cb) : WebController
    {
        public ContextBase db = cb;

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
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> WriteList()
        {
            var vm = new ResultVM();

            try
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
                vm.Data = await query.ToListAsync();
                vm.Set(RCodeTypes.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 保存一篇文章（管理）
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> WriteSave([FromForm] UserWriting mo)
        {
            var vm = new ResultVM();

            var num = await db.UserWriting.Where(x => x.UwId == mo.UwId)
                .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.UwTitle, mo.UwTitle)
                .SetProperty(p => p.UwStatus, mo.UwStatus)
                .SetProperty(p => p.UwReplyNum, mo.UwReplyNum)
                .SetProperty(p => p.UwReadNum, mo.UwReadNum)
                .SetProperty(p => p.UwLaud, mo.UwLaud)
                .SetProperty(p => p.UwMark, mo.UwMark)
                .SetProperty(p => p.UwOpen, mo.UwOpen));
            vm.Set(num > 0);

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
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> ReplyList()
        {
            var vm = new ResultVM();

            try
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
                vm.Data = await query.ToListAsync();
                vm.Set(RCodeTypes.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 保存一条回复（管理）
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> ReplySave([FromForm] UserReply mo)
        {
            var vm = new ResultVM();

            //xss
            mo.UrContent = CommonService.XSS(mo.UrContent);

            var num = await db.UserReply.Where(x => x.UrId == mo.UrId).ExecuteUpdateAsync(x => x
                .SetProperty(p => p.UrAnonymousName, mo.UrAnonymousName)
                .SetProperty(p => p.UrAnonymousMail, mo.UrAnonymousMail)
                .SetProperty(p => p.UrAnonymousLink, mo.UrAnonymousLink)
                .SetProperty(p => p.UrContent, mo.UrContent)
                .SetProperty(p => p.UrContentMd, mo.UrContentMd)
                .SetProperty(p => p.UrStatus, mo.UrStatus));
            vm.Set(num > 0);

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
        [HttpGet]
        public async Task<LoggingResultVM> QueryLog(string queryAllSql, string queryLimitSql)
        {
            return await LoggingTo.Query(queryAllSql, queryLimitSql);
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
        [HttpGet]
        public async Task<LoggingResultVM> QueryLogStatsPVUV(int? days, string LogGroup)
        {
            var listWhere = new List<List<string>>();
            if (!string.IsNullOrWhiteSpace(LogGroup))
            {
                listWhere =
                [
                    new List<string> { "LogGroup", "=", LogGroup }
                ];
            }

            var vm = await LoggingTo.StatsPVUV(days ?? 0, listWhere);
            return vm;
        }

        /// <summary>
        /// 查询日志Top
        /// </summary>
        /// <param name="days">类型</param>
        /// <param name="field">属性字段</param>
        /// <param name="LogGroup">分组</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<LoggingResultVM> QueryLogStatsTop(int? days, string field, string LogGroup)
        {
            var listWhere = new List<List<string>>();
            if (!string.IsNullOrWhiteSpace(LogGroup))
            {
                listWhere =
                [
                    new List<string> { "LogGroup", "=", LogGroup }
                ];
            }

            var vm = await LoggingTo.StatsTop(days ?? 0, field, listWhere);
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
        public async Task<ResultVM> KeyVal([FromRoute] string id, string keys)
        {
            var vm = new ResultVM();
            try
            {
                var listKey = string.IsNullOrWhiteSpace(keys) ? Array.Empty<string>() : keys.Split(',');

                switch (id?.ToLower())
                {
                    case "grab":
                        {
                            foreach (var key in listKey)
                            {
                                string api = $"https://baike.baidu.com/api/openapi/BaikeLemmaCardApi?scope=103&format=json&appid=379020&bk_key={key.ToUrlEncode()}&bk_length=600";
                                try
                                {
                                    var hc = new HttpClient();
                                    var resp = await hc.GetAsync(api);
                                    var result = await resp.Content.ReadAsStringAsync();
                                    if (result.Length > 100)
                                    {
                                        var mo = await db.KeyValues.FirstOrDefaultAsync(x => x.KeyName == key);
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

                                        var num = await db.SaveChangesAsync();
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
                            }
                        }
                        break;
                    case "synonym":
                        {
                            string mainKey = listKey.First().ToLower();

                            var listkvs = new List<KeyValueSynonym>();
                            for (int i = 1; i < listKey.Length; i++)
                            {
                                var kvs = new KeyValueSynonym
                                {
                                    KsId = Guid.NewGuid().ToString(),
                                    KeyName = mainKey,
                                    KsName = listKey[i].ToLower()
                                };
                                listkvs.Add(kvs);
                            }

                            var mo = await db.KeyValueSynonym.FirstOrDefaultAsync(x => x.KeyName == mainKey);
                            if (mo != null)
                            {
                                db.KeyValueSynonym.Remove(mo);
                            }
                            await db.KeyValueSynonym.AddRangeAsync(listkvs);

                            int num = await db.SaveChangesAsync();
                            vm.Log.Add($"Done {mainKey} {num}");
                        }
                        break;
                    case "tag":
                        {
                            if (listKey.Length > 0)
                            {
                                var mt = await db.Tags.Where(x => listKey.Contains(x.TagName)).ToListAsync();
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
                                    await db.Tags.AddRangeAsync(listMo);
                                    var num = await db.SaveChangesAsync();

                                    await CommonService.TagsQuery(false);
                                    vm.Log.Add($"Done 已刷新缓存 {num}");
                                }
                                else
                                {
                                    vm.Log.Add($"Done 标签已存在 {mt.ToJson()}");
                                }
                            }
                            else
                            {
                                await CommonService.TagsQuery(false);
                                vm.Log.Add($"Done 已刷新缓存");
                            }
                        }
                        break;
                }

                vm.Set(RCodeTypes.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        #endregion

        #region 缓存

        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <returns></returns>
        public IActionResult CacheRemoveAll()
        {
            CacheTo.RemoveAll();
            return Ok("Done!");
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns></returns>
        public IActionResult CacheAll()
        {
            return Json(CacheTo.CacheDictionary);
        }

        #endregion
    }
}