namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 主体
    /// </summary>
    public class HomeController : Controller
    {
        public ContextBase db;

        public HomeController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="k">搜索</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string k, int page = 1)
        {
            var ckey = $"Writing-Page-{page}";
            PageVM vm = CacheTo.Get<PageVM>(ckey);
            if (vm == null || !string.IsNullOrWhiteSpace(k))
            {
                vm = await CommonService.UserWritingQuery(k, page);
                vm.Route = Request.Path;

                //仅缓存不搜索页面
                if (string.IsNullOrWhiteSpace(k))
                {
                    CacheTo.Set(ckey, vm, 30, false);
                }
            }

            return View("~/Views/Home/_PartialWritingList.cshtml", vm);
        }

        /// <summary>
        /// 标签分类
        /// </summary>
        /// <param name="id">标签</param>
        /// <param name="k">搜索</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> Type([FromRoute] string id, string k, int page = 1)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RedirectResult("/");
            }
            else
            {
                var vm = await CommonService.UserWritingQuery(k, page, id);
                vm.Route = Request.Path;

                return View("~/Views/Home/_PartialWritingList.cshtml", vm);
            }
        }

        /// <summary>
        /// 标签
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Tags()
        {
            var tags = (await CommonService.TagsQuery()).Select(x => new
            {
                x.TagName,
                x.TagIcon
            }).ToJson();

            ViewData["tags"] = tags;

            return View();
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Write()
        {
            return View();
        }

        /// <summary>
        /// 标签
        /// </summary>
        /// <returns></returns>
        public async Task<ResultVM> TagSelect()
        {
            var vm = new ResultVM
            {
                Data = await CommonService.TagsQuery()
            };
            vm.Set(RCodeTypes.success);

            return vm;
        }

        /// <summary>
        /// 新增文章
        /// </summary>
        /// <param name="mo">文章信息</param>
        /// <param name="TagIds">标签，多个逗号分割</param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<ResultVM> WriteSave([FromForm] UserWriting mo, [FromForm] string TagIds)
        {
            var vm = new ResultVM();

            try
            {
                vm = IdentityService.CompleteInfoValid(HttpContext);
                if (vm.Code == 200)
                {
                    var uinfo = IdentityService.Get(HttpContext);

                    var lisTagId = new List<int>();
                    TagIds.Split(',').ForEach(x => lisTagId.Add(Convert.ToInt32(x)));
                    var lisTagName = (await CommonService.TagsQuery()).Where(x => lisTagId.Contains(x.TagId)).ToList();

                    mo.Uid = uinfo?.UserId;
                    mo.UwCreateTime = DateTime.Now;
                    mo.UwUpdateTime = mo.UwCreateTime;
                    mo.UwLastUid = mo.Uid;
                    mo.UwLastDate = mo.UwCreateTime;
                    mo.UwReplyNum = 0;
                    mo.UwReadNum = 0;
                    mo.UwOpen = 1;
                    mo.UwLaud = 0;
                    mo.UwMark = 0;
                    mo.UwStatus = 1;

                    await db.UserWriting.AddAsync(mo);
                    await db.SaveChangesAsync();

                    var listwt = new List<UserWritingTags>();
                    foreach (var tag in lisTagId)
                    {
                        var wtmo = new UserWritingTags
                        {
                            UwId = mo.UwId,
                            TagId = tag,
                            TagName = lisTagName.FirstOrDefault(x => x.TagId == tag).TagName
                        };

                        listwt.Add(wtmo);
                    }
                    await db.UserWritingTags.AddRangeAsync(listwt);
                    await db.SaveChangesAsync();

                    //标签热点+1
                    var listTagId = listwt.Select(x => x.TagId.Value);
                    var num = await db.Tags.Where(x => listTagId.Contains(x.TagId))
                        .ExecuteUpdateAsync(x => x.SetProperty(p => p.TagHot, p => p.TagHot + 1));

                    //推送通知
                    _ = PushService.PushWeChat("网站消息（文章）", $"文章ID：{mo.UwId}\r\n{mo.UwTitle}");

                    vm.Data = mo.UwId;
                    vm.Set(num > 0);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 一篇
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 5)]
        public async Task<IActionResult> List([FromRoute] int id, int page = 1)
        {
            if (id > 0)
            {
                //查询一条
                var uwo = await CommonService.UserWritingOneQuery(id);
                if (uwo == null)
                {
                    return Redirect("/");
                }

                var pag = new PaginationVM
                {
                    PageNumber = Math.Max(page, 1),
                    PageSize = 20
                };

                var vm = new PageVM()
                {
                    //查询回复
                    Rows = await CommonService.ReplyOneQuery(ReplyTypes.UserWriting, id.ToString(), pag),
                    Pag = pag,
                    Temp = uwo,
                    Route = $"/home/list/{id}"
                };

                //点赞、收藏
                if (User.Identity.IsAuthenticated)
                {
                    var uinfo = IdentityService.Get(HttpContext);
                    var listuc = await db.UserConnection.Where(x => x.Uid == uinfo.UserId && x.UconnTargetType == ConnectionTypes.UserWriting.ToString() && x.UconnTargetId == id.ToString()).ToListAsync();

                    ViewData["uca1"] = listuc.Any(x => x.UconnAction == 1) ? "yes" : "";
                    ViewData["uca2"] = listuc.Any(x => x.UconnAction == 2) ? "yes" : "";
                }

                return View(vm);
            }
            else
            {
                return Redirect("/");
            }
        }

        /// <summary>
        /// 回复
        /// </summary>
        /// <param name="mo">回复信息</param>
        /// <param name="um">消息通知</param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public ResultVM ReplySave([FromForm] UserReply mo, [FromForm] UserMessage um)
        {
            var vm = new ResultVM();
            vm = IdentityService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                if (!mo.Uid.HasValue || string.IsNullOrWhiteSpace(mo.UrContent) || string.IsNullOrWhiteSpace(mo.UrTargetId))
                {
                    vm.Set(RCodeTypes.failure);
                }
                else
                {
                    mo.Uid = IdentityService.Get(HttpContext)?.UserId ?? 0;
                    var now = DateTime.Now;

                    //回复消息
                    um.UmId = Guid.NewGuid().ToLongString();
                    um.UmTriggerUid = mo.Uid;
                    um.UmType = MessageTypes.UserWriting.ToString();
                    um.UmTargetId = mo.UrTargetId;
                    um.UmAction = 2;
                    um.UmStatus = 1;
                    um.UmContent = mo.UrContent;
                    um.UmCreateTime = now;

                    //回复内容
                    mo.UrCreateTime = now;
                    mo.UrStatus = 1;
                    mo.UrTargetPid = 0;
                    mo.UrTargetType = ReplyTypes.UserWriting.ToString();

                    mo.UrAnonymousLink = ParsingTo.JsSafeJoin(mo.UrAnonymousLink);

                    db.UserReply.Add(mo);

                    //回填文章最新回复记录
                    var mow = db.UserWriting.FirstOrDefault(x => x.UwId.ToString() == mo.UrTargetId);
                    if (mow != null)
                    {
                        mow.UwReplyNum += 1;
                        mow.UwLastUid = mo.Uid;
                        mow.UwLastDate = now;

                        um.UmTargetIndex = mow.UwReplyNum;

                        db.UserWriting.Update(mow);
                    }

                    if (um.Uid != um.UmTriggerUid)
                    {
                        db.UserMessage.Add(um);
                    }

                    int num = db.SaveChanges();

                    //推送通知
                    _ = PushService.PushWeChat("网站消息（留言）", $"类别：{mo.UrTargetType}\r\n{mo.UrContentMd}");

                    vm.Set(num > 0);
                }
            }

            return vm;
        }

        /// <summary>
        /// 点赞收藏
        /// </summary>
        /// <param name="id"></param>
        /// <param name="a">动作</param>
        /// <returns></returns>
        [Authorize, HttpGet]
        public async Task<ResultVM> ConnSave([FromRoute] int id, int a)
        {
            var vm = new ResultVM();

            try
            {
                var uinfo = IdentityService.Get(HttpContext);
                var num = 0;

                var modelConn = await db.UserConnection.FirstOrDefaultAsync(x => x.Uid == uinfo.UserId && x.UconnTargetId == id.ToString() && x.UconnAction == a);
                if (modelConn == null)
                {
                    modelConn = new UserConnection()
                    {
                        UconnId = Guid.NewGuid().ToLongString(),
                        UconnAction = a,
                        UconnCreateTime = DateTime.Now,
                        UconnTargetId = id.ToString(),
                        UconnTargetType = ConnectionTypes.UserWriting.ToString(),
                        Uid = uinfo.UserId
                    };
                    await db.UserConnection.AddAsync(modelConn);
                    if (a == 1)
                    {
                        num += await db.UserWriting.Where(x => x.UwId == id)
                            .ExecuteUpdateAsync(x => x.SetProperty(p => p.UwLaud, p => p.UwLaud + 1));
                    }
                    if (a == 2)
                    {
                        num += await db.UserWriting.Where(x => x.UwId == id)
                            .ExecuteUpdateAsync(x => x.SetProperty(p => p.UwMark, p => p.UwMark + 1));
                    }
                    vm.Data = "1";
                }
                else
                {
                    db.UserConnection.Remove(modelConn);
                    if (a == 1)
                    {
                        num += await db.UserWriting.Where(x => x.UwId == id)
                            .ExecuteUpdateAsync(x => x.SetProperty(p => p.UwLaud, p => p.UwLaud - 1));
                    }
                    if (a == 2)
                    {
                        num += await db.UserWriting.Where(x => x.UwId == id)
                            .ExecuteUpdateAsync(x => x.SetProperty(p => p.UwMark, p => p.UwMark - 1));
                    }
                    vm.Data = "0";
                }

                num += await db.SaveChangesAsync();
                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 阅读追加
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        public async Task<IActionResult> ReadPlus([FromRoute] int id)
        {
            await db.UserWriting.Where(x => x.UwId == id)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.UwReadNum, p => p.UwReadNum + 1));

            return Ok();
        }

        /// <summary>
        /// 完善信息
        /// </summary>
        /// <returns></returns>
        public IActionResult CompleteInfo()
        {
            return View();
        }
    }
}
