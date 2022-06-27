using Netnr.Blog.Domain;
using Microsoft.AspNetCore.Authorization;
using Netnr.Core;
using Netnr.Blog.Data;

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
        public IActionResult Index(string k, int page = 1)
        {
            var ckey = $"Writing-{page}";
            if (!string.IsNullOrWhiteSpace(k) || CacheTo.Get(ckey) is not SharedPageVM vm)
            {
                vm = Application.CommonService.UserWritingQuery(k, page);
                vm.Route = Request.Path;

                if (string.IsNullOrWhiteSpace(k))
                {
                    CacheTo.Set(ckey, vm, 30, false);
                }
            }

            return View("_PartialWritingList", vm);
        }

        /// <summary>
        /// 标签分类
        /// </summary>
        /// <param name="id">标签</param>
        /// <param name="k">搜索</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        [ResponseCache(Duration = 5)]
        public IActionResult Type([FromRoute] string id, string k, int page = 1)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RedirectResult("/");
            }
            else
            {
                var vm = Application.CommonService.UserWritingQuery(k, page, id);
                vm.Route = Request.Path;

                return View("_PartialWritingList", vm);
            }
        }

        /// <summary>
        /// 标签
        /// </summary>
        /// <returns></returns>
        public IActionResult Tags()
        {
            var tags = Application.CommonService.TagsQuery().Select(x => new
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
        public List<Tags> TagSelect()
        {
            var list = Application.CommonService.TagsQuery();
            return list;
        }

        /// <summary>
        /// 新增文章
        /// </summary>
        /// <param name="mo">文章信息</param>
        /// <param name="TagIds">标签，多个逗号分割</param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public SharedResultVM WriteSave([FromForm] UserWriting mo, [FromForm] string TagIds)
        {
            return SharedResultVM.Try(vm =>
            {
                vm = Apps.LoginService.CompleteInfoValid(HttpContext);
                if (vm.Code == 200)
                {
                    var uinfo = Apps.LoginService.Get(HttpContext);

                    var lisTagId = new List<int>();
                    TagIds.Split(',').ToList().ForEach(x => lisTagId.Add(Convert.ToInt32(x)));
                    var lisTagName = Application.CommonService.TagsQuery().Where(x => lisTagId.Contains(x.TagId)).ToList();

                    mo.Uid = uinfo.UserId;
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

                    db.UserWriting.Add(mo);
                    db.SaveChanges();

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
                    db.UserWritingTags.AddRange(listwt);

                    //标签热点+1
                    var listTagId = listwt.Select(x => x.TagId.Value);
                    var listTags = db.Tags.Where(x => listTagId.Contains(x.TagId)).ToList();
                    listTags.ForEach(x => x.TagHot += 1);
                    db.Tags.UpdateRange(listTags);

                    int num = db.SaveChanges();

                    //推送通知
                    Application.PushService.PushAsync("网站消息（文章）", $"文章ID：{mo.UwId}\r\n{mo.UwTitle}");

                    vm.Data = mo.UwId;
                    vm.Set(num > 0);
                }

                return vm;
            });
        }

        /// <summary>
        /// 一篇
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 5)]
        public IActionResult List([FromRoute] int id, int page = 1)
        {
            if (id > 0)
            {
                var uwo = Application.CommonService.UserWritingOneQuery(id);
                if (uwo == null)
                {
                    return Redirect("/");
                }

                var pag = new SharedPaginationVM
                {
                    PageNumber = Math.Max(page, 1),
                    PageSize = 20
                };

                var vm = new SharedPageVM()
                {
                    Rows = Application.CommonService.ReplyOneQuery(Application.EnumService.ReplyType.UserWriting, id.ToString(), pag),
                    Pag = pag,
                    Temp = uwo,
                    Route = $"/home/list/{id}"
                };


                if (User.Identity.IsAuthenticated)
                {
                    var uinfo = Apps.LoginService.Get(HttpContext);
                    var listuc = db.UserConnection.Where(x => x.Uid == uinfo.UserId && x.UconnTargetType == Application.EnumService.ConnectionType.UserWriting.ToString() && x.UconnTargetId == id.ToString()).ToList();

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
        public SharedResultVM ReplySave([FromForm] UserReply mo, [FromForm] UserMessage um)
        {
            var vm = new SharedResultVM();
            vm = Apps.LoginService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                if (!mo.Uid.HasValue || string.IsNullOrWhiteSpace(mo.UrContent) || string.IsNullOrWhiteSpace(mo.UrTargetId))
                {
                    vm.Set(SharedEnum.RTag.lack);
                }
                else
                {
                    var uinfo = Apps.LoginService.Get(HttpContext);
                    mo.Uid = uinfo.UserId;

                    var now = DateTime.Now;

                    //回复消息
                    um.UmId = UniqueTo.LongId().ToString();
                    um.UmTriggerUid = mo.Uid;
                    um.UmType = Application.EnumService.MessageType.UserWriting.ToString();
                    um.UmTargetId = mo.UrTargetId;
                    um.UmAction = 2;
                    um.UmStatus = 1;
                    um.UmContent = mo.UrContent;
                    um.UmCreateTime = now;

                    //回复内容
                    mo.UrCreateTime = now;
                    mo.UrStatus = 1;
                    mo.UrTargetPid = 0;
                    mo.UrTargetType = Application.EnumService.ReplyType.UserWriting.ToString();

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
                    Application.PushService.PushAsync("网站消息（留言）", $"类别：{mo.UrTargetType}\r\n{mo.UrContentMd}");

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
        public SharedResultVM ConnSave([FromRoute] int id, int a)
        {
            var vm = new SharedResultVM();

            var uinfo = Apps.LoginService.Get(HttpContext);

            var uw = db.UserWriting.Find(id);

            var uc = db.UserConnection.FirstOrDefault(x => x.Uid == uinfo.UserId && x.UconnTargetId == id.ToString() && x.UconnAction == a);
            if (uc == null)
            {
                uc = new UserConnection()
                {
                    UconnId = UniqueTo.LongId().ToString(),
                    UconnAction = a,
                    UconnCreateTime = DateTime.Now,
                    UconnTargetId = id.ToString(),
                    UconnTargetType = Application.EnumService.ConnectionType.UserWriting.ToString(),
                    Uid = uinfo.UserId
                };
                db.UserConnection.Add(uc);
                if (a == 1)
                {
                    uw.UwLaud += 1;
                }
                if (a == 2)
                {
                    uw.UwMark += 1;
                }
                db.UserWriting.Update(uw);

                vm.Data = "1";
            }
            else
            {
                db.UserConnection.Remove(uc);
                if (a == 1)
                {
                    uw.UwLaud -= 1;
                }
                if (a == 2)
                {
                    uw.UwMark -= 1;
                }
                db.UserWriting.Update(uw);

                vm.Data = "0";
            }

            int num = db.SaveChanges();

            vm.Set(num > 0);
            return vm;
        }

        /// <summary>
        /// 阅读追加
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        public IActionResult ReadPlus([FromRoute] int id)
        {
            var mo = db.UserWriting.Find(id);
            if (mo != null)
            {
                mo.UwReadNum += 1;
                db.UserWriting.Update(mo);
                db.SaveChanges();
            }

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

        /// <summary>
        /// Swagger自定义样式
        /// </summary>
        /// <returns></returns>
        public IActionResult SwaggerCustomStyle()
        {
            var txt = @".opblock-options{display:none}.download-contents{width:auto !important}";

            return new ContentResult()
            {
                Content = txt,
                StatusCode = 200,
                ContentType = "text/css"
            };
        }
    }
}
