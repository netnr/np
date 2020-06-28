using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netnr.Logging;
using Netnr.Blog.Web.Filters;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 后台管理
    /// </summary>
    [Authorize]
    [FilterConfigs.IsAdmin]
    public class AdminController : Controller
    {
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
        /// 查询文章列表
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public QueryDataOutputVM QueryWriteList(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            using (var db = new Data.ContextBase())
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

                if (!string.IsNullOrWhiteSpace(ivm.Pe1))
                {
                    query = query.Where(x => x.UwTitle.Contains(ivm.Pe1));
                }

                Application.CommonService.QueryJoin(query, ivm, ref ovm);
            }

            return ovm;
        }

        /// <summary>
        /// 保存一篇文章（管理）
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        public ActionResultVM WriteAdminSave(Domain.UserWriting mo)
        {
            var vm = new ActionResultVM();

            var uinfo = new Application.UserAuthService(HttpContext).Get();

            using (var db = new Data.ContextBase())
            {
                var oldmo = db.UserWriting.FirstOrDefault(x => x.UwId == mo.UwId);

                if (oldmo != null)
                {
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
        /// 查询回复列表
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public QueryDataOutputVM QueryReplyList(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            using (var db = new Data.ContextBase())
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

                                b.UserId,
                                b.Nickname,
                                b.UserName,
                                b.UserMail
                            };

                if (!string.IsNullOrWhiteSpace(ivm.Pe1))
                {
                    query = query.Where(x => x.UrContent.Contains(ivm.Pe1));
                }

                Application.CommonService.QueryJoin(query, ivm, ref ovm);
            }

            return ovm;
        }

        /// <summary>
        /// 保存一条回复（管理）
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        public ActionResultVM ReplyAdminSave(Domain.UserReply mo)
        {
            var vm = new ActionResultVM();

            var uinfo = new Application.UserAuthService(HttpContext).Get();

            using (var db = new Data.ContextBase())
            {
                var oldmo = db.UserReply.FirstOrDefault(x => x.UrId == mo.UrId);

                if (oldmo != null)
                {
                    oldmo.UrAnonymousName = mo.UrAnonymousName;
                    oldmo.UrAnonymousMail = mo.UrAnonymousMail;
                    oldmo.UrAnonymousLink = mo.UrAnonymousLink;

                    oldmo.UrStatus = mo.UrStatus;

                    db.UserReply.Update(oldmo);

                    int num = db.SaveChanges();

                    vm.Set(num > 0);
                }
            }

            return vm;
        }

        #endregion

        #region 日志

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
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public string QueryLog(int page, int rows)
        {
            var now = DateTime.Now;
            var vm = LoggingTo.Query(now.AddYears(-5), now, page, rows);

            return new
            {
                data = vm.Data,
                total = vm.Total,
                lost = vm.Lost
            }.ToJson();
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
        /// <param name="type">类型</param>
        /// <param name="LogGroup">分组</param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public LoggingResultVM QueryLogStatsPVUV(int? type, string LogGroup)
        {
            Dictionary<string, string> dicWhere = null;
            if (!string.IsNullOrWhiteSpace(LogGroup))
            {
                dicWhere = new Dictionary<string, string>
                {
                    { "LogGroup", LogGroup }
                };
            }

            var vm = LoggingTo.StatsPVUV(type ?? 0, dicWhere);
            return vm;
        }

        /// <summary>
        /// 查询日志Top
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="field">属性字段</param>
        /// <param name="LogGroup">分组</param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public LoggingResultVM QueryLogReportTop(int? type, string field, string LogGroup)
        {
            Dictionary<string, string> dicWhere = null;
            if (!string.IsNullOrWhiteSpace(LogGroup))
            {
                dicWhere = new Dictionary<string, string>
                {
                    { "LogGroup", LogGroup }
                };
            }

            var vm = LoggingTo.StatsTop(type ?? 0, field, dicWhere);
            return vm;
        }

        #endregion

        #region 百科字典

        /// <summary>
        /// 字典
        /// </summary>
        /// <returns></returns>
        public IActionResult KeyValues()
        {
            string cmd = RouteData.Values["id"]?.ToString();
            if (cmd != null)
            {
                string result = string.Empty;
                var rt = new List<object>
                {
                    0,
                    "fail"
                };

                try
                {
                    switch (cmd)
                    {
                        case "grab":
                            {
                                string key = Request.Form["Key"].ToString();
                                string api = $"https://baike.baidu.com/api/openapi/BaikeLemmaCardApi?scope=103&format=json&appid=379020&bk_key={key.ToEncode()}&bk_length=600";
                                string apirt = Core.HttpTo.Get(api);
                                if (apirt.Length > 100)
                                {
                                    using var db = new Data.ContextBase();
                                    var kvMo = db.KeyValues.Where(x => x.KeyName == key).FirstOrDefault();
                                    if (kvMo == null)
                                    {
                                        kvMo = new Domain.KeyValues
                                        {
                                            KeyId = Guid.NewGuid().ToString(),
                                            KeyName = key.ToLower(),
                                            KeyValue = apirt
                                        };
                                        db.KeyValues.Add(kvMo);
                                    }
                                    else
                                    {
                                        kvMo.KeyValue = apirt;
                                        db.KeyValues.Update(kvMo);
                                    }

                                    rt[0] = db.SaveChanges();
                                    rt[1] = kvMo;
                                }
                                else
                                {
                                    rt[0] = 0;
                                    rt[1] = apirt;
                                }
                            }
                            break;
                        case "synonym":
                            {
                                var keys = Request.Form["keys"].ToString().Split(',').ToList();

                                string mainKey = keys.First().ToLower();
                                keys.RemoveAt(0);

                                var listkvs = new List<Domain.KeyValueSynonym>();
                                foreach (var key in keys)
                                {
                                    var kvs = new Domain.KeyValueSynonym
                                    {
                                        KsId = Guid.NewGuid().ToString(),
                                        KeyName = mainKey,
                                        KsName = key.ToLower()
                                    };
                                    listkvs.Add(kvs);
                                }

                                using var db = new Data.ContextBase();
                                var mo = db.KeyValueSynonym.Where(x => x.KeyName == mainKey).FirstOrDefault();
                                if (mo != null)
                                {
                                    db.KeyValueSynonym.Remove(mo);
                                }
                                db.KeyValueSynonym.AddRange(listkvs);
                                int oldrow = db.SaveChanges();
                                rt[0] = 1;
                                rt[1] = " 受影响 " + oldrow + " 行";
                            }
                            break;
                        case "addtag":
                            {
                                var tags = Request.Form["tags"].ToString().Split(',').ToList();

                                if (tags.Count > 0)
                                {
                                    using var db = new Data.ContextBase();
                                    var mt = db.Tags.Where(x => tags.Contains(x.TagName)).ToList();
                                    if (mt.Count == 0)
                                    {
                                        var listMo = new List<Domain.Tags>();
                                        var tagHs = new HashSet<string>();
                                        foreach (var tag in tags)
                                        {
                                            if (tagHs.Add(tag))
                                            {
                                                var mo = new Domain.Tags
                                                {
                                                    TagName = tag.ToLower(),
                                                    TagStatus = 1,
                                                    TagHot = 0,
                                                    TagIcon = tag.ToLower() + ".svg"
                                                };
                                                listMo.Add(mo);
                                            }
                                        }
                                        tagHs.Clear();

                                        //新增&刷新缓存
                                        db.Tags.AddRange(listMo);
                                        rt[0] = db.SaveChanges();

                                        Application.CommonService.TagsQuery(false);

                                        rt[1] = "操作成功";
                                    }
                                    else
                                    {
                                        rt[0] = 0;
                                        rt[1] = "标签已存在：" + mt.ToJson();
                                    }
                                }
                                else
                                {
                                    rt[0] = 0;
                                    rt[1] = "新增标签不能为空";
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    rt[1] = ex.Message;
                    rt.Add(ex.StackTrace);
                }

                result = rt.ToJson();
                return Content(result);
            }
            return View();
        }

        #endregion
    }
}