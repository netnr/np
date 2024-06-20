namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 记事本
    /// </summary>
    [Authorize]
    public class NoteController(ContextBase cb) : WebController
    {
        public ContextBase db = cb;

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
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> NoteList()
        {
            var vm = new ResultVM();

            try
            {
                var uinfo = IdentityService.Get(HttpContext);

                var query = from a in db.Notepad
                            join b in db.UserInfo on a.Uid equals b.UserId
                            orderby a.NoteCreateTime descending
                            where a.Uid == uinfo.UserId
                            select new Notepad
                            {
                                NoteId = a.NoteId,
                                NoteTitle = a.NoteTitle,
                                NoteCreateTime = a.NoteCreateTime,
                                NoteUpdateTime = a.NoteUpdateTime,
                                Uid = a.Uid,
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
        /// 保存一条记事本
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> SaveNote([FromForm] Notepad mo)
        {
            var vm = IdentityService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                if (string.IsNullOrWhiteSpace(mo.NoteTitle) || string.IsNullOrWhiteSpace(mo.NoteContent))
                {
                    vm.Set(RCodeTypes.failure);
                }
                else
                {
                    var uinfo = IdentityService.Get(HttpContext);

                    var now = DateTime.Now;
                    if (mo.NoteId == 0)
                    {
                        mo.NoteCreateTime = now;
                        mo.NoteUpdateTime = now;
                        mo.Uid = uinfo.UserId;

                        db.Notepad.Add(mo);

                        int num = await db.SaveChangesAsync();
                        vm.Set(num > 0);
                        vm.Data = mo.NoteId;
                    }
                    else
                    {
                        var num = await db.Notepad.Where(x => x.NoteId == mo.NoteId && x.Uid == uinfo.UserId)
                            .ExecuteUpdateAsync(x => x.SetProperty(p => p.NoteTitle, mo.NoteTitle)
                           .SetProperty(p => p.NoteContent, mo.NoteContent)
                           .SetProperty(p => p.NoteUpdateTime, now));

                        vm.Set(num > 0);
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
        public async Task<ResultVM> QueryNoteOne(int id)
        {
            var vm = new ResultVM();

            var uinfo = IdentityService.Get(HttpContext);

            var mo = await db.Notepad.FirstOrDefaultAsync(x => x.NoteId == id && x.Uid == uinfo.UserId);
            if (mo == null)
            {
                vm.Set(RCodeTypes.failure);
            }
            else
            {
                vm.Set(RCodeTypes.success);
                vm.Data = mo;
            }

            return vm;
        }

        /// <summary>
        /// 删除记事本
        /// </summary>
        /// <param name="ids">多个逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> DelNote(string ids)
        {
            var vm = new ResultVM();

            try
            {
                var uinfo = IdentityService.Get(HttpContext);
                var listKeyId = ids.Split(',').Select(x => Convert.ToInt32(x)).ToList();

                var num = await db.Notepad.Where(x => listKeyId.Contains(x.NoteId) && x.Uid == uinfo.UserId).ExecuteDeleteAsync();
                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}