using Microsoft.AspNetCore.Authorization;
using Netnr.Blog.Data;
using AgGrid.InfiniteRowModel;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 记事本
    /// </summary>
    [Authorize]
    public class NoteController : Controller
    {
        public ContextBase db;

        public NoteController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 记事本
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM NoteList(string grp)
        {
            return SharedResultVM.Try(vm =>
            {
                var uinfo = Apps.LoginService.Get(HttpContext);

                var query = from a in db.Notepad
                            join b in db.UserInfo on a.Uid equals b.UserId
                            orderby a.NoteCreateTime descending
                            where a.Uid == uinfo.UserId
                            select new Domain.Notepad
                            {
                                NoteId = a.NoteId,
                                NoteTitle = a.NoteTitle,
                                NoteCreateTime = a.NoteCreateTime,
                                NoteUpdateTime = a.NoteUpdateTime,
                                Uid = a.Uid,
                            };

                vm.Data = query.GetInfiniteRowModelBlock(grp);
                vm.Set(SharedEnum.RTag.success);

                return vm;
            });
        }

        /// <summary>
        /// 保存一条记事本
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM SaveNote([FromForm] Domain.Notepad mo)
        {
            var vm = Apps.LoginService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                if (string.IsNullOrWhiteSpace(mo.NoteTitle) || string.IsNullOrWhiteSpace(mo.NoteContent))
                {
                    vm.Set(SharedEnum.RTag.lack);
                }
                else
                {
                    var uinfo = Apps.LoginService.Get(HttpContext);

                    var now = DateTime.Now;
                    if (mo.NoteId == 0)
                    {
                        mo.NoteCreateTime = now;
                        mo.NoteUpdateTime = now;
                        mo.Uid = uinfo.UserId;

                        db.Notepad.Add(mo);

                        int num = db.SaveChanges();
                        vm.Set(num > 0);
                        vm.Data = mo.NoteId;
                    }
                    else
                    {
                        var currmo = db.Notepad.Find(mo.NoteId);
                        if (currmo.Uid == uinfo.UserId)
                        {
                            currmo.NoteTitle = mo.NoteTitle;
                            currmo.NoteContent = mo.NoteContent;
                            currmo.NoteUpdateTime = now;

                            db.Notepad.Update(currmo);

                            int num = db.SaveChanges();

                            vm.Set(num > 0);
                        }
                        else
                        {
                            vm.Set(SharedEnum.RTag.unauthorized);
                        }
                    }
                }

            }

            return vm;
        }

        /// <summary>
        /// 查询一条记事本详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM QueryNoteOne(int id)
        {
            return SharedResultVM.Try(vm =>
            {
                var uinfo = Apps.LoginService.Get(HttpContext);

                var mo = db.Notepad.Find(id);
                if (mo == null)
                {
                    vm.Set(SharedEnum.RTag.invalid);
                }
                else if (mo.Uid == uinfo.UserId)
                {
                    vm.Set(SharedEnum.RTag.success);
                    vm.Data = mo;
                }
                else
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }

                return vm;
            });
        }

        /// <summary>
        /// 删除记事本
        /// </summary>
        /// <param name="ids">多个逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DelNote(string ids)
        {
            return SharedResultVM.Try(vm =>
            {
                var uinfo = Apps.LoginService.Get(HttpContext);
                var listKeyId = ids.Split(',').Select(x => Convert.ToInt32(x)).ToList();

                var listMo = db.Notepad.Where(x => listKeyId.Contains(x.NoteId) && x.Uid == uinfo.UserId).ToList();
                if (listMo.Count > 0)
                {
                    db.Notepad.RemoveRange(listMo);
                    int num = db.SaveChanges();

                    vm.Set(num > 0);
                }
                else
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }

                return vm;
            });
        }
    }
}