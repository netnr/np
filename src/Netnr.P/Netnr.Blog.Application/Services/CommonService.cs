using Microsoft.EntityFrameworkCore;

namespace Netnr.Blog.Application.Services
{
    /// <summary>
    /// 公共常用
    /// </summary>
    public class CommonService
    {
        /// <summary>
        /// 页量
        /// </summary>
        public static int PageSize { get; set; } = 12;

        /// <summary>
        /// 静态资源链接
        /// </summary>
        /// <param name="typePath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string StaticResourceLink(string typePath, params string[] args)
        {
            var paths = new List<string>();
            new List<string> { "Server", "PhysicalRootPath", typePath }.ForEach(item =>
            {
                paths.Add(AppTo.GetValue($"StaticResource:{item}"));
            });
            paths.AddRange(args);
            return PathTo.Combine(paths.ToArray());
        }

        /// <summary>
        /// 完整物理路径
        /// </summary>
        /// <param name="typePath">分类配置，默认为空</param>
        /// <param name="args">追加路径</param>
        /// <returns></returns>
        public static string StaticResourcePath(string typePath = null, params string[] args)
        {
            return PathTo.Combine(AppTo.WebRootPath, UrlRelativePath(typePath, args));
        }

        /// <summary>
        /// URL 相对路径
        /// </summary>
        /// <param name="typePath">分类配置，默认为空</param>
        /// <param name="args">追加路径</param>
        /// <returns></returns>
        public static string UrlRelativePath(string typePath = null, params string[] args)
        {
            var listPath = new List<string>
            {
                AppTo.GetValue("StaticResource:PhysicalRootPath")
            };

            if (!string.IsNullOrWhiteSpace(typePath))
            {
                listPath.Add(AppTo.GetValue($"StaticResource:{typePath}"));
            }

            listPath.AddRange(args);
            return PathTo.Combine(listPath.ToArray());
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <param name="cacheFirst">默认取缓存</param>
        /// <returns></returns>
        public static async Task<List<Tags>> TagsQuery(bool cacheFirst = true)
        {
            var ckey = $"Tags";

            var result = CacheTo.Get<List<Tags>>(ckey);
            if (cacheFirst == false || result == null)
            {
                using var db = ContextBaseFactory.CreateDbContext();
                result = await db.Tags.Where(x => x.TagStatus == 1).OrderByDescending(x => x.TagHot).ToListAsync();
                CacheTo.Set(ckey, result, 300, false);
            }
            return result;
        }

        /// <summary>
        /// 获取文章标签统计
        /// </summary>
        /// <param name="cacheFirst"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, int>> UserWritingByTagCountQuery(bool cacheFirst = true)
        {
            var ckey = $"WritingTags-Group";

            var result = CacheTo.Get<Dictionary<string, int>>(ckey);
            if (cacheFirst == false || result == null)
            {
                using var db = ContextBaseFactory.CreateDbContext();
                var query = from a in db.UserWritingTags
                            group a by a.TagName into m
                            orderby m.Count() descending
                            select new
                            {
                                m.Key,
                                total = m.Count()
                            };
                var qs = await query.Take(28).OrderByDescending(x => x.total).ToListAsync();

                var dic = new Dictionary<string, int>();
                foreach (var item in qs)
                {
                    dic.Add(item.Key, item.total);
                }

                result = dic;
                CacheTo.Set(ckey, result, 300, false);
            }
            return result;
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="KeyWords"></param>
        /// <param name="page"></param>
        /// <param name="TagName"></param>
        /// <returns></returns>
        public static async Task<PageVM> UserWritingQuery(string KeyWords, int page, string TagName = "")
        {
            var vm = new PageVM();

            KeyWords ??= "";

            var pag = new PaginationVM
            {
                PageNumber = Math.Max(page, 1),
                PageSize = PageSize
            };

            var dicQs = new Dictionary<string, string> { { "k", KeyWords } };

            using var db = ContextBaseFactory.CreateDbContext();

            var query = from a in db.UserWriting select a;
            if (!string.IsNullOrWhiteSpace(TagName))
            {
                query = from a in db.UserWritingTags.Where(x => x.TagName == TagName)
                        join b in db.UserWriting on a.UwId equals b.UwId
                        select b;
                query = query.Distinct();
            }
            query = query.Where(x => x.UwOpen == 1 && x.UwStatus == 1);

            if (!string.IsNullOrWhiteSpace(KeyWords))
            {
                var kws = new List<string>();
                kws.AddRange(KeyWords.Split(' '));
                kws.Add(KeyWords);
                kws = kws.Distinct().ToList();

                var inner = PredicateTo.False<UserWriting>();
                switch (AppTo.TDB)
                {
                    case EnumTo.TypeDB.SQLite:
                        kws.ForEach(k => inner = inner.Or(x => EF.Functions.Like(x.UwTitle, $"%{k}%") || EF.Functions.Like(x.UwContentMd, $"%{k}%")));
                        break;
                    case EnumTo.TypeDB.PostgreSQL:
                        kws.ForEach(k => inner = inner.Or(x => EF.Functions.ILike(x.UwTitle, $"%{k}%") || EF.Functions.ILike(x.UwContentMd, $"%{k}%")));
                        break;
                    default:
                        kws.ForEach(k => inner = inner.Or(x => x.UwTitle.Contains(k) || x.UwContentMd.Contains(k)));
                        break;
                }
                query = query.Where(inner);
            }

            pag.Total = await query.CountAsync();

            query = query.OrderByDescending(x => x.UwId).Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize);

            var list = await query.ToListAsync();

            //文章ID
            var listUwId = list.Select(x => x.UwId).ToList();

            //文章的所有的标签
            var queryTags = from a in db.UserWritingTags
                            join b in db.Tags on a.TagName equals b.TagName
                            where listUwId.Contains(a.UwId) || b.TagName == TagName
                            orderby a.UwtId ascending
                            select new
                            {
                                a.UwId,
                                a.TagName,
                                b.TagIcon
                            };
            var listUwTags = await queryTags.ToListAsync();

            //文章人员ID
            var listUwUid = list.Select(x => x.UwLastUid).Concat(list.Select(x => x.Uid)).Distinct();

            //文章人员ID对应的信息
            var listUwUserInfo = await db.UserInfo.Where(x => listUwUid.Contains(x.UserId)).Select(x => new { x.UserId, x.Nickname }).ToListAsync();

            //把信息赋值到文章表的备用字段上
            foreach (var item in list)
            {
                //标签
                item.Spare1 = listUwTags.Where(x => x.UwId == item.UwId).Select(x => new { x.TagName, x.TagIcon }).ToJson();

                //写主昵称
                item.Spare3 = listUwUserInfo.FirstOrDefault(x => x.UserId == item.Uid)?.Nickname;

                //有回复
                if (item.UwLastUid > 0)
                {
                    //回复用户昵称
                    item.Spare2 = listUwUserInfo.FirstOrDefault(x => x.UserId == item.UwLastUid)?.Nickname;
                }
            }

            vm.Rows = list;
            vm.Pag = pag;
            vm.QueryString = dicQs;

            //标签信息
            if (!string.IsNullOrWhiteSpace(TagName))
            {
                try
                {
                    var kvs = await KeyValuesQuery(new List<string> { TagName });
                    var jt = kvs.FirstOrDefault()?.KeyValue.DeJson();
                    if (jt != null)
                    {
                        vm.Temp = new
                        {
                            tag = new[] { new { TagName, listUwTags.FirstOrDefault(x => x.TagName == TagName)?.TagIcon } },
                            baike_abstract = jt.Value.GetValue("abstract"),
                            baike_url = jt.Value.GetValue("url")
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return vm;
        }

        /// <summary>
        /// 获取关联的文章列表
        /// </summary>
        /// <param name="OwnerId">所属用户关联</param>
        /// <param name="connectionType">关联分类</param>
        /// <param name="action">动作</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static async Task<PageVM> UserConnWritingQuery(int OwnerId, ConnectionType connectionType, int action, int page)
        {
            var pag = new PaginationVM
            {
                PageNumber = Math.Max(page, 1),
                PageSize = PageSize
            };

            using var db = ContextBaseFactory.CreateDbContext();
            IQueryable<UserWriting> query = null;

            switch (connectionType)
            {
                case ConnectionType.UserWriting:
                    {
                        query = from a in db.UserConnection
                                join b in db.UserWriting on a.UconnTargetId equals b.UwId.ToString()
                                where a.Uid == OwnerId && a.UconnTargetType == connectionType.ToString() && a.UconnAction == action
                                orderby a.UconnCreateTime descending
                                select b;
                    }
                    break;
            }

            if (query == null)
            {
                return null;
            }

            pag.Total = await query.CountAsync();

            query = query.Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize);

            var list = await query.ToListAsync();

            //文章ID
            var listUwId = list.Select(x => x.UwId).ToList();

            //文章的所有的标签
            var listUwTags = await (from a in db.Tags
                                    join b in db.UserWritingTags on a.TagName equals b.TagName
                                    where listUwId.Contains(b.UwId)
                                    select new
                                    {
                                        b.UwId,
                                        b.TagName,
                                        a.TagIcon
                                    }).ToListAsync();

            //文章人员ID
            var listUwUid = list.Select(x => x.UwLastUid).Concat(list.Select(x => x.Uid)).Distinct();

            //文章人员ID对应的信息
            var listUwUserInfo = await db.UserInfo.Where(x => listUwUid.Contains(x.UserId)).Select(x => new { x.UserId, x.Nickname }).ToListAsync();

            //把信息赋值到文章表的备用字段上
            foreach (var item in list)
            {
                //标签
                item.Spare1 = listUwTags.Where(x => x.UwId == item.UwId).Select(x => new { x.TagName, x.TagIcon }).ToJson();

                //写主昵称
                item.Spare3 = listUwUserInfo.FirstOrDefault(x => x.UserId == item.Uid)?.Nickname;

                //有回复
                if (item.UwLastUid > 0)
                {
                    //回复用户昵称
                    item.Spare2 = listUwUserInfo.FirstOrDefault(x => x.UserId == item.UwLastUid)?.Nickname;
                }
            }

            var vm = new PageVM()
            {
                Rows = list,
                Pag = pag
            };

            return vm;
        }

        /// <summary>
        /// 获取一篇文章详情
        /// </summary>
        /// <param name="UwId"></param>
        /// <returns></returns>
        public static async Task<UserWriting> UserWritingOneQuery(int UwId)
        {
            using var db = ContextBaseFactory.CreateDbContext();
            var one = db.UserWriting.Find(UwId);
            if (one == null)
            {
                return null;
            }

            //标签
            var onetags = await (from a in db.UserWritingTags
                                 join b in db.Tags on a.TagName equals b.TagName
                                 where a.UwId == one.UwId
                                 select new
                                 {
                                     a.TagName,
                                     b.TagIcon
                                 }).ToListAsync();
            one.Spare1 = onetags.ToJson();

            //昵称
            var usermo = await db.UserInfo.FirstOrDefaultAsync(x => x.UserId == one.Uid);
            one.Spare2 = usermo?.Nickname;

            return one;
        }

        /// <summary>
        /// 获取一个目标ID的回复
        /// </summary>
        /// <param name="replyType">回复分类</param>
        /// <param name="UrTargetId">回复目标ID</param>
        /// <param name="pag">分页信息</param>
        /// <returns></returns>
        public static async Task<List<UserReply>> ReplyOneQuery(ReplyType replyType, string UrTargetId, PaginationVM pag)
        {
            using var db = ContextBaseFactory.CreateDbContext();
            var query = from a in db.UserReply
                        join b in db.UserInfo on a.Uid equals b.UserId into bg
                        from b1 in bg.DefaultIfEmpty()
                        where a.UrTargetType == replyType.ToString() && a.UrTargetId == UrTargetId
                        orderby a.UrCreateTime ascending
                        select new UserReply
                        {
                            Uid = a.Uid,
                            UrId = a.UrId,
                            UrStatus = a.UrStatus,
                            UrCreateTime = a.UrCreateTime,
                            UrContent = a.UrContent,
                            UrAnonymousLink = a.UrAnonymousLink,
                            UrAnonymousMail = a.UrAnonymousMail,
                            UrAnonymousName = a.UrAnonymousName,

                            UrTargetId = a.UrTargetId,

                            Spare1 = b1 == null ? null : b1.Nickname,
                            Spare2 = b1 == null ? null : b1.UserPhoto
                        };

            pag.Total = await query.CountAsync();

            query = query.Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize);

            var list = await query.ToListAsync();

            return list;
        }

        /// <summary>
        /// 键值对
        /// </summary>
        /// <param name="ListKeyName"></param>
        /// <returns></returns>
        public static async Task<List<KeyValues>> KeyValuesQuery(List<string> ListKeyName)
        {
            using var db = ContextBaseFactory.CreateDbContext();
            var listKv = await db.KeyValues.Where(x => ListKeyName.Contains(x.KeyName)).ToListAsync();
            if (listKv.Count != ListKeyName.Count)
            {
                var hasK = listKv.Select(x => x.KeyName).ToList();
                var noK = new List<string>();
                foreach (var item in ListKeyName)
                {
                    if (!hasK.Contains(item))
                    {
                        noK.Add(item);
                    }
                }
                if (noK.Count > 0)
                {
                    var listKvs = await db.KeyValueSynonym.Where(x => noK.Contains(x.KsName)).ToListAsync();
                    if (listKvs.Count > 0)
                    {
                        var appendKey = listKvs.Select(x => x.KeyName).ToList();
                        var appendKv = await db.KeyValues.Where(x => appendKey.Contains(x.KeyName)).ToListAsync();
                        foreach (var item in appendKv)
                        {
                            var mc = listKvs.Where(x => x.KeyName == item.KeyName).FirstOrDefault();
                            if (mc != null)
                            {
                                item.KeyName = mc.KsName;
                            }
                        }
                        listKv.AddRange(appendKv);
                    }
                }
            }

            return listKv;
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="messageType">消息分类</param>
        /// <param name="action">消息动作</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static async Task<PageVM> MessageQuery(int UserId, MessageType? messageType, int? action, int page = 1)
        {
            using var db = ContextBaseFactory.CreateDbContext();
            var query = from a in db.UserMessage
                        join b in db.UserInfo on a.UmTriggerUid equals b.UserId into bg
                        from b1 in bg.DefaultIfEmpty()
                        orderby a.UmCreateTime descending
                        where a.Uid == UserId
                        select new
                        {
                            a,
                            Nickname = b1 == null ? null : b1.Nickname,
                            UserPhoto = b1 == null ? null : b1.UserPhoto
                        };
            if (messageType.HasValue)
            {
                query = query.Where(x => x.a.UmType == messageType.ToString());
            }
            if (action.HasValue)
            {
                query = query.Where(x => x.a.UmAction == action);
            }

            var pag = new PaginationVM
            {
                PageNumber = Math.Max(page, 1),
                PageSize = PageSize,
                Total = await query.CountAsync()
            };

            var list = await query.Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize).ToListAsync();
            if (list.Count > 0)
            {
                //分类：根据ID查询对应的标题
                var listUwId = list.Where(x => x.a.UmType == MessageType.UserWriting.ToString()).Select(x => Convert.ToInt32(x.a.UmTargetId)).ToList();
                var listUw = await db.UserWriting.Where(x => listUwId.Contains(x.UwId)).Select(x => new { x.UwId, x.UwTitle }).ToListAsync();

                foreach (var item in list)
                {
                    item.a.Spare1 = item.Nickname;
                    item.a.Spare2 = item.UserPhoto;
                    item.a.Spare3 = listUw.FirstOrDefault(x => x.UwId.ToString() == item.a.UmTargetId)?.UwTitle;
                }
            }

            var data = list.Select(x => x.a).ToList();

            PageVM pageSet = new()
            {
                Rows = data,
                Pag = pag
            };

            return pageSet;
        }

        /// <summary>
        /// 有新消息数量查询
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        public static int NewMessageQuery(int UserId)
        {
            if (AppTo.GetValue<bool>("ReadOnly"))
            {
                return 0;
            }

            var ckey = "NewMessage";
            var gkey = UserId.ToString();

            var num = CacheTo.Get<int?>(ckey, gkey);
            if (num == null)
            {
                CacheTo.Set(ckey, 0, 30, false, gkey);
                Task.Run(() =>
                {
                    try
                    {
                        using var db = ContextBaseFactory.CreateDbContext();
                        var num2 = db.UserMessage.Where(x => x.Uid == UserId && x.UmStatus == 1).Count();
                        CacheTo.Set(ckey, num2, 30, false, gkey);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            }

            return num ?? 0;
        }

        /// <summary>
        /// Gist查询，按列权重排序
        /// </summary>
        /// <param name="q">搜索</param>
        /// <param name="lang">语言</param>
        /// <param name="OwnerId">所属用户</param>
        /// <param name="UserId">登录用户</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public static async Task<PageVM> GistQuery(string q, string lang, int OwnerId = 0, int UserId = 0, int page = 1)
        {
            using var db = ContextBaseFactory.CreateDbContext();

            var query1 = from a in db.Gist
                         join b in db.UserInfo on a.Uid equals b.UserId
                         where a.GistStatus == 1
                         orderby a.GistCreateTime descending
                         select new
                         {
                             a,
                             b.Nickname
                         };

            if (!string.IsNullOrWhiteSpace(lang))
            {
                query1 = query1.Where(x => x.a.GistLanguage == lang);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query1 = AppTo.TDB switch
                {
                    EnumTo.TypeDB.SQLite => query1.Where(x => EF.Functions.Like(x.a.GistFilename, $"%{q}%") || EF.Functions.Like(x.a.GistRemark, $"%{q}%") || EF.Functions.Like(x.a.GistContent, $"%{q}%")),
                    EnumTo.TypeDB.PostgreSQL => query1.Where(x => EF.Functions.ILike(x.a.GistFilename, $"%{q}%") || EF.Functions.ILike(x.a.GistRemark, $"%{q}%") || EF.Functions.ILike(x.a.GistContent, $"%{q}%")),
                    _ => query1.Where(x => x.a.GistFilename.Contains(q) || x.a.GistRemark.Contains(q) || x.a.GistContent.Contains(q)),
                };
            }

            //所属用户
            if (OwnerId != 0)
            {
                query1 = query1.Where(x => x.a.Uid == OwnerId);
            }

            //未登录
            if (UserId == 0)
            {
                query1 = query1.Where(x => x.a.GistOpen == 1);
            }
            else
            {
                //已登录：公开&登录用户的所有
                query1 = query1.Where(x => x.a.GistOpen == 1 || x.a.Uid == UserId);
            }

            IQueryable<Gist> query = null;

            //搜索
            if (!string.IsNullOrWhiteSpace(q))
            {
                var query2 = query1.Select(x => new
                {
                    SearchOrder = (x.a.GistFilename.Contains(q) ? 4 : 0) + (x.a.GistRemark.Contains(q) ? 2 : 0) + (x.a.GistContent.Contains(q) ? 1 : 0),
                    x.Nickname,
                    x.a
                }).OrderByDescending(x => x.SearchOrder);

                switch (AppTo.TDB)
                {
                    case EnumTo.TypeDB.SQLite:
                        query2 = query1.Select(x => new
                        {
                            SearchOrder = (EF.Functions.Like(x.a.GistFilename, $"%{q}%") ? 4 : 0) + (EF.Functions.Like(x.a.GistRemark, $"%{q}%") ? 2 : 0) + (EF.Functions.Like(x.a.GistContent, $"%{q}%") ? 1 : 0),
                            x.Nickname,
                            x.a
                        }).OrderByDescending(x => x.SearchOrder);
                        break;
                    case EnumTo.TypeDB.PostgreSQL:
                        query2 = query1.Select(x => new
                        {
                            SearchOrder = (EF.Functions.Like(x.a.GistFilename, $"%{q}%") ? 4 : 0) + (EF.Functions.Like(x.a.GistRemark, $"%{q}%") ? 2 : 0) + (EF.Functions.Like(x.a.GistContent, $"%{q}%") ? 1 : 0),
                            x.Nickname,
                            x.a
                        }).OrderByDescending(x => x.SearchOrder);
                        break;
                }

                query = query2.Select(x => new Gist
                {
                    GistCode = x.a.GistCode,
                    GistContentPreview = x.a.GistContentPreview,
                    GistCreateTime = x.a.GistCreateTime,
                    GistFilename = x.a.GistFilename,
                    GistId = x.a.GistId,
                    GistLanguage = x.a.GistLanguage,
                    GistRemark = x.a.GistRemark,
                    GistRow = x.a.GistRow,
                    GistTags = x.a.GistTags,
                    GistTheme = x.a.GistTheme,
                    Uid = x.a.Uid,

                    Spare3 = x.Nickname
                });
            }
            else
            {
                query = query1.Select(x => new Gist
                {
                    GistCode = x.a.GistCode,
                    GistContentPreview = x.a.GistContentPreview,
                    GistCreateTime = x.a.GistCreateTime,
                    GistFilename = x.a.GistFilename,
                    GistId = x.a.GistId,
                    GistLanguage = x.a.GistLanguage,
                    GistRemark = x.a.GistRemark,
                    GistRow = x.a.GistRow,
                    GistTags = x.a.GistTags,
                    GistTheme = x.a.GistTheme,
                    Uid = x.a.Uid,

                    Spare3 = x.Nickname
                });
            }

            var pag = new PaginationVM
            {
                PageNumber = Math.Max(page, 1),
                PageSize = 10
            };

            var dicQs = new Dictionary<string, string> { { "q", q } };

            pag.Total = await query.CountAsync();
            var list = await query.Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize).ToListAsync();

            PageVM pageSet = new()
            {
                Rows = list,
                Pag = pag,
                QueryString = dicQs
            };

            //显示词云 Spare1=1
            if (string.IsNullOrWhiteSpace(q) && pag.Total > 0)
            {
                var result = await db.Gist
                    .Select(x => new { x.GistCode, x.GistFilename, x.GistRemark, x.GistCreateTime, x.GistOpen, x.GistStatus, x.Spare1 })
                    .Where(x => x.GistOpen == 1 && x.GistStatus == 1 && x.Spare1 == "1")
                    .OrderByDescending(x => x.GistCreateTime)
                    .ToListAsync();

                pageSet.Temp = result.ToJson();
            }

            return pageSet;
        }

        /// <summary>
        /// Doc查询
        /// </summary>
        /// <param name="q">搜索</param>
        /// <param name="OwnerId">所属用户</param>
        /// <param name="UserId">登录用户</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public static async Task<PageVM> DocQuery(string q, int OwnerId, int UserId, int page = 1)
        {
            using var db = ContextBaseFactory.CreateDbContext();
            var query = from a in db.DocSet
                        join b in db.UserInfo on a.Uid equals b.UserId
                        where a.DsStatus == 1
                        orderby a.DsCreateTime descending
                        select new DocSet
                        {
                            DsCode = a.DsCode,
                            DsCreateTime = a.DsCreateTime,
                            DsName = a.DsName,
                            DsOpen = a.DsOpen,
                            DsRemark = a.DsRemark,
                            Uid = a.Uid,
                            Spare1 = a.Spare1,

                            Spare3 = b.Nickname
                        };

            //所属用户
            if (OwnerId != 0)
            {
                query = query.Where(x => x.Uid == OwnerId);
            }

            //未登录
            if (UserId == 0)
            {
                query = query.Where(x => x.DsOpen == 1);
            }
            else
            {
                //已登录：公开&登录用户的所有
                query = query.Where(x => x.DsOpen == 1 || x.Uid == UserId);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = AppTo.TDB switch
                {
                    EnumTo.TypeDB.SQLite => query.Where(x => EF.Functions.Like(x.DsName, $"%{q}%") || EF.Functions.Like(x.DsRemark, $"%{q}%")),
                    EnumTo.TypeDB.PostgreSQL => query.Where(x => EF.Functions.ILike(x.DsName, $"%{q}%") || EF.Functions.ILike(x.DsRemark, $"%{q}%")),
                    _ => query.Where(x => x.DsName.Contains(q) || x.DsRemark.Contains(q)),
                };
            }

            var pag = new PaginationVM
            {
                PageNumber = Math.Max(page, 1),
                PageSize = PageSize
            };

            var dicQs = new Dictionary<string, string> { { "q", q } };

            pag.Total = await query.CountAsync();
            var list = await query.Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize).ToListAsync();

            PageVM pageSet = new()
            {
                Rows = list,
                Pag = pag,
                QueryString = dicQs
            };

            return pageSet;
        }

        /// <summary>
        /// Run查询
        /// </summary>
        /// <param name="q">搜索</param>
        /// <param name="OwnerId">所属用户</param>
        /// <param name="UserId">登录用户</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public static async Task<PageVM> RunQuery(string q, int OwnerId = 0, int UserId = 0, int page = 1)
        {
            using var db = ContextBaseFactory.CreateDbContext();
            var query = from a in db.Run
                        join b in db.UserInfo on a.Uid equals b.UserId
                        where a.RunStatus == 1
                        orderby a.RunCreateTime descending
                        select new Run
                        {
                            RunCode = a.RunCode,
                            RunCreateTime = a.RunCreateTime,
                            RunId = a.RunId,
                            RunRemark = a.RunRemark,
                            RunTags = a.RunTags,
                            RunTheme = a.RunTheme,
                            Uid = a.Uid,
                            RunOpen = a.RunOpen,

                            Spare3 = b.Nickname,
                        };

            //所属用户
            if (OwnerId != 0)
            {
                query = query.Where(x => x.Uid == OwnerId);
            }

            //未登录
            if (UserId == 0)
            {
                query = query.Where(x => x.RunOpen == 1);
            }
            else
            {
                //已登录：公开&登录用户的所有
                query = query.Where(x => x.RunOpen == 1 || x.Uid == UserId);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = AppTo.TDB switch
                {
                    EnumTo.TypeDB.SQLite => query.Where(x => EF.Functions.Like(x.RunRemark, $"%{q}%")),
                    EnumTo.TypeDB.PostgreSQL => query.Where(x => EF.Functions.ILike(x.RunRemark, $"%{q}%")),
                    _ => query.Where(x => x.RunRemark.Contains(q)),
                };
            }

            var pag = new PaginationVM
            {
                PageNumber = Math.Max(page, 1),
                PageSize = PageSize
            };

            var dicQs = new Dictionary<string, string> { { "q", q } };

            pag.Total = await query.CountAsync();
            var list = await query.Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize).ToListAsync();

            PageVM pageSet = new()
            {
                Rows = list,
                Pag = pag,
                QueryString = dicQs
            };

            return pageSet;
        }

        /// <summary>
        /// Draw查询
        /// </summary>
        /// <param name="q">搜索</param>
        /// <param name="OwnerId">所属用户</param>
        /// <param name="UserId">登录用户</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public static async Task<PageVM> DrawQuery(string q, int OwnerId = 0, int UserId = 0, int page = 1)
        {
            using var db = ContextBaseFactory.CreateDbContext();
            var query = from a in db.Draw
                        join b in db.UserInfo on a.Uid equals b.UserId
                        where a.DrStatus == 1
                        orderby a.DrCreateTime descending
                        select new Draw
                        {
                            DrId = a.DrId,
                            Uid = a.Uid,
                            DrType = a.DrType,
                            DrName = a.DrName,
                            DrRemark = a.DrRemark,
                            DrCategory = a.DrCategory,
                            DrOrder = a.DrOrder,
                            DrCreateTime = a.DrCreateTime,
                            DrStatus = a.DrStatus,
                            DrOpen = a.DrOpen,
                            Spare1 = a.Spare1,

                            Spare3 = b.Nickname
                        };

            //所属用户
            if (OwnerId != 0)
            {
                query = query.Where(x => x.Uid == OwnerId);
            }

            //未登录
            if (UserId == 0)
            {
                query = query.Where(x => x.DrOpen == 1);
            }
            else
            {
                //已登录：公开&登录用户的所有
                query = query.Where(x => x.DrOpen == 1 || x.Uid == UserId);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = AppTo.TDB switch
                {
                    EnumTo.TypeDB.SQLite => query.Where(x => EF.Functions.Like(x.DrName, $"%{q}%") || EF.Functions.Like(x.DrRemark, $"%{q}%")),
                    EnumTo.TypeDB.PostgreSQL => query.Where(x => EF.Functions.ILike(x.DrName, $"%{q}%") || EF.Functions.ILike(x.DrRemark, $"%{q}%")),
                    _ => query.Where(x => x.DrName.Contains(q) || x.DrRemark.Contains(q)),
                };
            }

            var pag = new PaginationVM
            {
                PageNumber = Math.Max(page, 1),
                PageSize = PageSize
            };

            var dicQs = new Dictionary<string, string> { { "q", q } };

            pag.Total = await query.CountAsync();
            var list = await query.Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize).ToListAsync();

            PageVM pageSet = new()
            {
                Rows = list,
                Pag = pag,
                QueryString = dicQs
            };

            return pageSet;
        }

        /// <summary>
        /// Guff查询
        /// </summary>
        /// <param name="category">类别，可选，支持 text、image、audio、video、me（我的）、melaud（我点赞的）、mereply（我回复的）</param>
        /// <param name="q">搜索</param>
        /// <param name="nv">分类名/分类值</param>
        /// <param name="tag">标签</param>
        /// <param name="obj">对象</param>
        /// <param name="OwnerId">所属用户</param>
        /// <param name="UserId">登录用户</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public static async Task<PageVM> GuffQuery(string category, string q, string nv, string tag, string obj, int OwnerId, int UserId, int page = 1)
        {
            var ctype = ConnectionType.GuffRecord.ToString();

            using var db = ContextBaseFactory.CreateDbContext();

            IQueryable<GuffRecord> query = null;

            switch (category?.ToLower())
            {
                case "melaud":
                    {
                        query = from c in db.UserConnection
                                join a in db.GuffRecord on c.UconnTargetId equals a.GrId
                                join b in db.UserInfo on a.Uid equals b.UserId
                                where c.Uid == UserId && c.UconnTargetType == ctype && c.UconnAction == 1 && a.GrStatus == 1
                                orderby c.UconnCreateTime descending
                                select new GuffRecord
                                {
                                    GrId = a.GrId,
                                    GrTypeName = a.GrTypeName,
                                    GrTypeValue = a.GrTypeValue,
                                    GrObject = a.GrObject,
                                    GrContent = a.GrContent,
                                    GrContentMd = a.GrContentMd,
                                    GrImage = a.GrImage,
                                    GrAudio = a.GrAudio,
                                    GrVideo = a.GrVideo,
                                    GrFile = a.GrFile,
                                    GrRemark = a.GrRemark,
                                    GrTag = a.GrTag,
                                    GrCreateTime = a.GrCreateTime,
                                    GrUpdateTime = a.GrUpdateTime,
                                    GrOpen = a.GrOpen,
                                    GrReadNum = a.GrReadNum,
                                    GrReplyNum = a.GrReplyNum,
                                    GrLaud = a.GrLaud,
                                    GrMark = a.GrMark,

                                    Uid = a.Uid,

                                    //已点赞
                                    Spare1 = "laud",
                                    //是我的
                                    Spare2 = a.Uid == UserId ? "owner" : "",
                                    //昵称
                                    Spare3 = b.Nickname
                                };
                    }
                    break;
                case "mereply":
                    {
                        query = from c in db.UserReply
                                join a in db.GuffRecord on c.UrTargetId equals a.GrId
                                join b in db.UserInfo on a.Uid equals b.UserId
                                where c.Uid == UserId && c.UrTargetType == ctype && a.GrStatus == 1
                                orderby c.UrCreateTime descending
                                select new GuffRecord
                                {
                                    GrId = a.GrId,
                                    GrTypeName = a.GrTypeName,
                                    GrTypeValue = a.GrTypeValue,
                                    GrObject = a.GrObject,
                                    GrContent = a.GrContent,
                                    GrContentMd = a.GrContentMd,
                                    GrImage = a.GrImage,
                                    GrAudio = a.GrAudio,
                                    GrVideo = a.GrVideo,
                                    GrFile = a.GrFile,
                                    GrRemark = a.GrRemark,
                                    GrTag = a.GrTag,
                                    GrCreateTime = a.GrCreateTime,
                                    GrUpdateTime = a.GrUpdateTime,
                                    GrOpen = a.GrOpen,
                                    GrReadNum = a.GrReadNum,
                                    GrReplyNum = a.GrReplyNum,
                                    GrLaud = a.GrLaud,
                                    GrMark = a.GrMark,

                                    Uid = a.Uid,

                                    Spare2 = a.Uid == UserId ? "owner" : "",
                                    Spare3 = b.Nickname
                                };
                    }
                    break;
                case "me":
                case "top":
                case "text":
                case "image":
                case "audio":
                case "video":
                default:
                    {
                        query = from a in db.GuffRecord
                                join b in db.UserInfo on a.Uid equals b.UserId
                                where a.GrStatus == 1
                                select new GuffRecord
                                {
                                    GrId = a.GrId,
                                    GrTypeName = a.GrTypeName,
                                    GrTypeValue = a.GrTypeValue,
                                    GrObject = a.GrObject,
                                    GrContent = a.GrContent,
                                    GrContentMd = a.GrContentMd,
                                    GrImage = a.GrImage,
                                    GrAudio = a.GrAudio,
                                    GrVideo = a.GrVideo,
                                    GrFile = a.GrFile,
                                    GrRemark = a.GrRemark,
                                    GrTag = a.GrTag,
                                    GrCreateTime = a.GrCreateTime,
                                    GrUpdateTime = a.GrUpdateTime,
                                    GrOpen = a.GrOpen,
                                    GrReadNum = a.GrReadNum,
                                    GrReplyNum = a.GrReplyNum,
                                    GrLaud = a.GrLaud,
                                    GrMark = a.GrMark,

                                    Uid = a.Uid,

                                    Spare2 = a.Uid == UserId ? "owner" : "",
                                    Spare3 = b.Nickname
                                };
                    }
                    break;
            }

            query = (category?.ToLower()) switch
            {
                "top" => query.OrderByDescending(x => x.GrLaud),
                "text" => query.OrderByDescending(x => x.GrCreateTime).Where(x => !string.IsNullOrEmpty(x.GrContent) && string.IsNullOrEmpty(x.GrImage) && string.IsNullOrEmpty(x.GrAudio) && string.IsNullOrEmpty(x.GrVideo)),
                "image" => query.OrderByDescending(x => x.GrCreateTime).Where(x => !string.IsNullOrEmpty(x.GrImage)),
                "audio" => query.OrderByDescending(x => x.GrCreateTime).Where(x => !string.IsNullOrEmpty(x.GrAudio)),
                "video" => query.OrderByDescending(x => x.GrCreateTime).Where(x => !string.IsNullOrEmpty(x.GrVideo)),
                _ => query.OrderByDescending(x => x.GrCreateTime),
            };

            //所属用户
            if (OwnerId != 0)
            {
                query = query.Where(x => x.Uid == OwnerId);
            }

            //未登录
            if (UserId == 0)
            {
                query = query.Where(x => x.GrOpen == 1);
            }
            else
            {
                //已登录：公开&登录用户的所有
                query = query.Where(x => x.GrOpen == 1 || x.Uid == UserId);
            }

            //分类名/分类值
            if (!string.IsNullOrWhiteSpace(nv))
            {
                if (!nv.Contains('/'))
                {
                    nv += "/";
                }

                var nvs = nv.Split('/').ToList();
                var n = nvs.FirstOrDefault();
                var v = nvs.LastOrDefault();

                //分类名
                if (!string.IsNullOrWhiteSpace(n))
                {
                    query = query.Where(x => x.GrTypeName == n);
                }

                //分类值
                if (!string.IsNullOrWhiteSpace(v))
                {
                    query = query.Where(x => x.GrTypeValue == v);
                }
            }

            //标签
            if (!string.IsNullOrWhiteSpace(tag))
            {
                query = query.Where(x => x.GrTag.Contains(tag));
            }

            //对象
            if (!string.IsNullOrWhiteSpace(obj))
            {
                query = query.Where(x => x.GrObject.Contains(obj));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = AppTo.TDB switch
                {
                    EnumTo.TypeDB.SQLite => query.Where(x => EF.Functions.Like(x.GrContent, $"%{q}%") || EF.Functions.Like(x.GrRemark, $"%{q}%") || EF.Functions.Like(x.GrTag, $"%{q}%")),
                    EnumTo.TypeDB.PostgreSQL => query.Where(x => EF.Functions.ILike(x.GrContent, $"%{q}%") || EF.Functions.ILike(x.GrRemark, $"%{q}%") || EF.Functions.ILike(x.GrTag, $"%{q}%")),
                    _ => query.Where(x => x.GrContent.Contains(q) || x.GrRemark.Contains(q) || x.GrTag.Contains(q)),
                };
            }

            var pag = new PaginationVM
            {
                PageNumber = Math.Max(page, 1),
                PageSize = PageSize
            };

            var dicQs = new Dictionary<string, string> { { "q", q } };

            pag.Total = await query.CountAsync();
            var list = await query.Skip((pag.PageNumber - 1) * pag.PageSize).Take(pag.PageSize).ToListAsync();

            var listid = list.Select(x => x.GrId).ToList();

            //点赞查询
            if (category != "melaud")
            {
                var listtid = await db.UserConnection.Where(x => listid.Contains(x.UconnTargetId) && x.Uid == UserId && x.UconnTargetType == ctype && x.UconnAction == 1).Select(x => x.UconnTargetId).ToListAsync();
                foreach (var item in list)
                {
                    if (listtid.Contains(item.GrId))
                    {
                        item.Spare1 = "laud";
                    }
                }
            }

            //查询记录
            if (!AppTo.GetValue<bool>("ReadOnly"))
            {
                var ormo = new OperationRecord()
                {
                    OrId = UniqueTo.LongId().ToString(),
                    OrType = ctype,
                    OrAction = "query",
                    OrSource = string.Join(",", listid),
                    OrCreateTime = DateTime.Now,
                    OrMark = "default"
                };
                await db.OperationRecord.AddAsync(ormo);
                await db.SaveChangesAsync();
            }

            PageVM pageSet = new()
            {
                Rows = list,
                Pag = pag,
                QueryString = dicQs
            };

            return pageSet;
        }
    }
}