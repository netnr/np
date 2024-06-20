using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [Authorize]
    [Route("[controller]/[action]")]
    public class SettingController(ContextBase cb) : Controller
    {
        public ContextBase db = cb;

        #region 系统按钮

        /// <summary>
        /// 系统按钮页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SysButton()
        {
            return View();
        }

        /// <summary>
        /// 查询系统按钮
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<QueryDataOutputVM> QuerySysButton(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var list = await db.SysButton.OrderBy(x => x.SbBtnOrder).ToListAsync();
            var tree = TreeTo.ListToTree(list, "SbPid", "SbId", [Guid.Empty.ToString()]);
            ovm.Data = tree.DeJson();

            //列
            if (ivm.ColumnsExists != 1)
            {
                ovm.Columns = await db.SysTableConfig.Where(x => x.TableName == ivm.TableName).OrderBy(x => x.ColOrder).ToListAsync();
            }

            return ovm;
        }

        /// <summary>
        /// 保存系统按钮
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="savetype">保存类型</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> SaveSysButton(SysButton mo, string savetype)
        {
            var vm = new ResultVM();

            if (string.IsNullOrWhiteSpace(mo.SbPid))
            {
                mo.SbPid = Guid.Empty.ToString();
            }
            mo.SbBtnHide ??= -1;

            if (savetype == "add")
            {
                mo.SbId = Guid.NewGuid().ToString();
                await db.SysButton.AddAsync(mo);
            }
            else
            {
                db.SysButton.Update(mo);
            }

            int num = await db.SaveChangesAsync();
            vm.Set(num > 0);

            //清理缓存
            CacheTo.Remove(CommonService.GlobalCacheKey.SysButton);

            return vm;
        }

        /// <summary>
        /// 删除系统按钮
        /// </summary>
        /// <param name="id">按钮ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> DelSysButton(string id)
        {
            var vm = new ResultVM();

            var num = await db.SysButton.Where(x => x.SbBtnId == id).ExecuteDeleteAsync();
            vm.Set(num > 0);

            return vm;
        }

        #endregion

        #region 系统菜单

        /// <summary>
        /// 系统菜单页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SysMenu()
        {
            return View();
        }

        /// <summary>
        /// 查询系统菜单
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<QueryDataOutputVM> QuerySysMenu(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var list = await db.SysMenu.OrderBy(x => x.SmOrder).ToListAsync();
            var tree = TreeTo.ListToTree(list, "SmPid", "SmId", [Guid.Empty.ToString()]);
            ovm.Data = tree.DeJson();

            //列
            if (ivm.ColumnsExists != 1)
            {
                ovm.Columns = await db.SysTableConfig.Where(x => x.TableName == ivm.TableName).OrderBy(x => x.ColOrder).ToListAsync();
            }

            return ovm;
        }

        /// <summary>
        /// 保存系统菜单
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="savetype"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM SaveSysMenu(SysMenu mo, string savetype)
        {
            var vm = new ResultVM();

            if (string.IsNullOrWhiteSpace(mo.SmPid))
            {
                mo.SmPid = Guid.Empty.ToString();
            }

            if (savetype == "add")
            {
                mo.SmId = Guid.NewGuid().ToString();
                db.SysMenu.Add(mo);
            }
            else
            {
                db.SysMenu.Update(mo);
            }

            int num = db.SaveChanges();
            vm.Set(num > 0);

            //清理缓存
            CacheTo.Remove(CommonService.GlobalCacheKey.SysMenu);

            return vm;
        }

        /// <summary>
        /// 删除系统菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM DelSysMenu(string id)
        {
            var vm = new ResultVM();

            var ids = new List<string> { id };
            do
            {
                var listMo = db.SysMenu.Where(x => ids.Contains(x.SmId)).ToList();
                if (listMo.Count > 0)
                {
                    db.SysMenu.RemoveRange(listMo);

                    var childMo = db.SysMenu.Where(x => ids.Contains(x.SmPid)).ToList();
                    ids = childMo.Select(x => x.SmId).ToList();
                }
                else
                {
                    ids.Clear();
                }
            } while (ids.Count > 0);

            int num = db.SaveChanges();
            vm.Set(num > 0);

            return vm;
        }

        #endregion

        #region 系统角色

        /// <summary>
        /// 系统角色页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SysRole()
        {
            return View();
        }

        /// <summary>
        /// 查询系统角色
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<QueryDataOutputVM> QuerySysRole(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.SysRole, ivm, db);
            return ovm;
        }

        /// <summary>
        /// 保存系统角色
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="savetype"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> SaveSysRole(SysRole mo, string savetype)
        {
            var vm = new ResultVM();

            if (savetype == "add")
            {
                mo.SrId = Guid.Empty.ToString();
                mo.SrCreateTime = DateTime.Now;
                await db.SysRole.AddAsync(mo);
            }
            else
            {
                db.SysRole.Update(mo);
            }

            int num = await db.SaveChangesAsync();
            if (num > 0)
            {
                //清理缓存
                CacheTo.Remove(CommonService.GlobalCacheKey.SysRole);
            }

            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 复制角色权限
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="copyid">复制的角色ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> CopySysRoleAuth(SysRole mo, string copyid)
        {
            var vm = new ResultVM();

            var list = await db.SysRole.Where(x => x.SrId == mo.SrId || x.SrId == copyid).ToListAsync();
            var copymo = list.Find(x => x.SrId == copyid);
            foreach (var item in list)
            {
                item.SrMenus = copymo.SrMenus;
                item.SrButtons = copymo.SrButtons;
            }
            db.SysRole.UpdateRange(list);

            int num = await db.SaveChangesAsync();
            if (num > 0)
            {
                //清理缓存
                CacheTo.Remove(CommonService.GlobalCacheKey.SysRole);
            }

            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 删除系统角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> DelSysRole(string id)
        {
            var vm = new ResultVM();

            if (await db.SysUser.AnyAsync(x => x.SrId == id))
            {
                vm.Set(RCodeTypes.exist);
            }
            else
            {
                var num = await db.SysRole.Where(x => x.SrId == id).ExecuteDeleteAsync();
                if (num > 0)
                {
                    //清理缓存
                    CacheTo.Remove(CommonService.GlobalCacheKey.SysRole);
                }

                vm.Set(num > 0);
            }

            return vm;
        }

        #endregion

        #region 系统用户

        /// <summary>
        /// 系统用户页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SysUser()
        {
            return View();
        }

        /// <summary>
        /// 查询系统用户
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<QueryDataOutputVM> QuerySysUser(QueryDataInputVM ivm)
        {
            var query = from a in db.SysUser
                        join b in db.SysRole on a.SrId equals b.SrId
                        select new
                        {
                            a.SuId,
                            a.SuNickname,
                            a.SrId,
                            a.SuSign,
                            a.SuStatus,
                            a.SuGroup,
                            a.SuName,
                            a.SuPwd,
                            a.SuCreateTime,
                            OldUserPwd = a.SuPwd,
                            b.SrName
                        };
            var ovm = await CommonService.QueryJoin(query, ivm, db);

            return ovm;
        }

        /// <summary>
        /// 保存系统用户
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="savetype"></param>
        /// <param name="OldUserPwd">原密码，有变化代表为改密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> SaveSysUser(SysUser mo, string savetype, string OldUserPwd)
        {
            var vm = new ResultVM();

            if (savetype == "add")
            {
                if (await db.SysUser.Where(x => x.SuName == mo.SuName).AnyAsync())
                {
                    vm.Set(RCodeTypes.exist);
                }
                else
                {
                    mo.SuId = Guid.NewGuid().ToString();
                    mo.SuCreateTime = DateTime.Now;
                    mo.SuPwd = CalcTo.MD5(mo.SuPwd);
                    await db.SysUser.AddAsync(mo);
                }
            }
            else
            {
                if (await db.SysUser.Where(x => x.SuName == mo.SuName && x.SuId != mo.SuId).AnyAsync())
                {
                    vm.Set(RCodeTypes.exist);
                }
                else
                {
                    if (mo.SuPwd != OldUserPwd)
                    {
                        mo.SuPwd = CalcTo.MD5(mo.SuPwd);
                    }
                    db.SysUser.Update(mo);
                }
            }

            int num = await db.SaveChangesAsync();
            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 删除系统用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> DelSysUser(string id)
        {
            var vm = new ResultVM();

            var num = await db.SysUser.Where(x => x.SuId == id).ExecuteDeleteAsync();
            vm.Set(num > 0);

            return vm;
        }

        #endregion

        #region 系统日志

        /// <summary>
        /// 系统日志页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SysLog()
        {
            return View();
        }

        /// <summary>
        /// 查询系统日志
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<QueryDataOutputVM> QuerySysLog(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.SysLog, ivm, db);
            return ovm;
        }

        #endregion

        #region 数据字典

        /// <summary>
        /// 系统数据字典
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SysDictionary()
        {
            return View();
        }

        /// <summary>
        /// 查询系统数据字典
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<QueryDataOutputVM> QuerySysDictionary(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.SysDictionary, ivm, db);
            return ovm;
        }

        /// <summary>
        /// 保存数据字典
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="savetype"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> SaveSysDictionary(SysDictionary mo, string savetype)
        {
            var vm = new ResultVM();

            if (savetype == "add")
            {
                mo.SdId = Guid.NewGuid().ToString();
                mo.SdPid = Guid.Empty.ToString();
                await db.SysDictionary.AddAsync(mo);
            }
            else
            {
                db.SysDictionary.Update(mo);
            }

            int num = await db.SaveChangesAsync();
            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 逻辑删除数据字典
        /// </summary>
        /// <param name="id">字典ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> DelSysDictionary(string id)
        {
            var vm = new ResultVM();

            var num = await db.SysDictionary.Where(x => x.SdId == id).ExecuteUpdateAsync(x => x.SetProperty(p => p.SdStatus, -1));
            vm.Set(num > 0);

            return vm;
        }

        #endregion

        #region 表配置

        /// <summary>
        /// 系统表配置页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SysTableConfig()
        {
            return View();
        }

        /// <summary>
        /// 查询表配置
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<QueryDataOutputVM> QuerySysTableConfig(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.SysTableConfig, ivm, db);
            return ovm;
        }

        /// <summary>
        /// 保存表配置
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="ColRelation">关系符</param>
        /// <param name="savetype"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> SaveSysTableConfig(SysTableConfig mo, List<string> ColRelation, string savetype)
        {
            var vm = new ResultVM();

            mo.ColRelation = string.Join(',', ColRelation);

            if (savetype == "add")
            {
                mo.Id = Guid.NewGuid().ToString();
                await db.SysTableConfig.AddAsync(mo);
            }
            else
            {
                db.SysTableConfig.Update(mo);
            }

            int num = await db.SaveChangesAsync();
            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 删除表配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> DelSysTableConfig(string id)
        {
            var vm = new ResultVM();

            var num = await db.SysTableConfig.Where(x => x.Id == id).ExecuteDeleteAsync();
            vm.Set(num > 0);

            return vm;
        }

        #endregion

        #region 样式配置

        /// <summary>
        /// 样式配置页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SysStyle()
        {
            return View();
        }

        #endregion
    }
}