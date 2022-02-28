using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Netnr.ResponseFramework.Data;
using Netnr.ResponseFramework.Domain;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [Authorize]
    [Route("[controller]/[action]")]
    public class SettingController : Controller
    {
        public ContextBase db;

        public SettingController(ContextBase cb)
        {
            db = cb;
        }

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
        public QueryDataOutputVM QuerySysButton(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var list = db.SysButton.OrderBy(x => x.SbBtnOrder).ToList();
            var tree = Core.TreeTo.ListToTree(list, "SbPid", "SbId", new List<string> { Guid.Empty.ToString() });
            ovm.Data = tree.ToJArray();

            //列
            if (ivm.ColumnsExists != 1)
            {
                ovm.Columns = db.SysTableConfig.Where(x => x.TableName == ivm.TableName).OrderBy(x => x.ColOrder).ToList();
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
        public SharedResultVM SaveSysButton(SysButton mo, string savetype)
        {
            var vm = new SharedResultVM();

            if (string.IsNullOrWhiteSpace(mo.SbPid))
            {
                mo.SbPid = Guid.Empty.ToString();
            }
            if (mo.SbBtnHide == null)
            {
                mo.SbBtnHide = -1;
            }

            if (savetype == "add")
            {
                mo.SbId = Guid.NewGuid().ToString();
                db.SysButton.Add(mo);
            }
            else
            {
                db.SysButton.Update(mo);
            }

            int num = db.SaveChanges();
            vm.Set(num > 0);

            //清理缓存
            Core.CacheTo.Remove(Application.CommonService.GlobalCacheKey.SysButton);

            return vm;
        }

        /// <summary>
        /// 删除系统按钮
        /// </summary>
        /// <param name="id">按钮ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DelSysButton(string id)
        {
            var vm = new SharedResultVM();

            var mo = db.SysButton.Find(id);
            db.SysButton.Remove(mo);

            int num = db.SaveChanges();
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
        public QueryDataOutputVM QuerySysMenu(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var list = db.SysMenu.OrderBy(x => x.SmOrder).ToList();
            var tree = Core.TreeTo.ListToTree(list, "SmPid", "SmId", new List<string> { Guid.Empty.ToString() });
            ovm.Data = tree.ToJArray();

            //列
            if (ivm.ColumnsExists != 1)
            {
                ovm.Columns = db.SysTableConfig.Where(x => x.TableName == ivm.TableName).OrderBy(x => x.ColOrder).ToList();
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
        public SharedResultVM SaveSysMenu(SysMenu mo, string savetype)
        {
            var vm = new SharedResultVM();

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
            Core.CacheTo.Remove(Application.CommonService.GlobalCacheKey.SysMenu);

            return vm;
        }

        /// <summary>
        /// 删除系统菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DelSysMenu(string id)
        {
            var vm = new SharedResultVM();

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
        public QueryDataOutputVM QuerySysRole(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var query = db.SysRole;
            Application.CommonService.QueryJoin(query, ivm, db, ref ovm);

            return ovm;
        }

        /// <summary>
        /// 保存系统角色
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="savetype"></param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM SaveSysRole(SysRole mo, string savetype)
        {
            var vm = new SharedResultVM();

            if (savetype == "add")
            {
                mo.SrId = Guid.Empty.ToString();
                mo.SrCreateTime = DateTime.Now;
                db.SysRole.Add(mo);
            }
            else
            {
                db.SysRole.Update(mo);
            }

            int num = db.SaveChanges();
            if (num > 0)
            {
                //清理缓存
                Core.CacheTo.Remove(Application.CommonService.GlobalCacheKey.SysRole);
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
        public SharedResultVM CopySysRoleAuth(SysRole mo, string copyid)
        {
            var vm = new SharedResultVM();

            var list = db.SysRole.Where(x => x.SrId == mo.SrId || x.SrId == copyid).ToList();
            var copymo = list.Find(x => x.SrId == copyid);
            foreach (var item in list)
            {
                item.SrMenus = copymo.SrMenus;
                item.SrButtons = copymo.SrButtons;
            }
            db.SysRole.UpdateRange(list);

            int num = db.SaveChanges();
            if (num > 0)
            {
                //清理缓存
                Core.CacheTo.Remove(Application.CommonService.GlobalCacheKey.SysRole);
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
        public SharedResultVM DelSysRole(string id)
        {
            var vm = new SharedResultVM();

            if (db.SysUser.Where(x => x.SrId == id).Any())
            {
                vm.Set(SharedEnum.RTag.exist);
            }
            else
            {
                var mo = db.SysRole.Find(id);
                db.SysRole.Remove(mo);

                int num = db.SaveChanges();
                if (num > 0)
                {
                    //清理缓存
                    Core.CacheTo.Remove(Application.CommonService.GlobalCacheKey.SysRole);
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
        public QueryDataOutputVM QuerySysUser(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

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
            Application.CommonService.QueryJoin(query, ivm, db, ref ovm);

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
        public SharedResultVM SaveSysUser(SysUser mo, string savetype, string OldUserPwd)
        {
            var vm = new SharedResultVM();

            if (savetype == "add")
            {
                if (db.SysUser.Where(x => x.SuName == mo.SuName).Any())
                {
                    vm.Set(SharedEnum.RTag.exist);
                }
                else
                {
                    mo.SuId = Guid.NewGuid().ToString();
                    mo.SuCreateTime = DateTime.Now;
                    mo.SuPwd = Core.CalcTo.MD5(mo.SuPwd);
                    db.SysUser.Add(mo);
                }
            }
            else
            {
                if (db.SysUser.Where(x => x.SuName == mo.SuName && x.SuId != mo.SuId).Any())
                {
                    vm.Set(SharedEnum.RTag.exist);
                }
                else
                {
                    if (mo.SuPwd != OldUserPwd)
                    {
                        mo.SuPwd = Core.CalcTo.MD5(mo.SuPwd);
                    }
                    db.SysUser.Update(mo);
                }
            }

            int num = db.SaveChanges();
            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 删除系统用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DelSysUser(string id)
        {
            var vm = new SharedResultVM();

            var mo = db.SysUser.Find(id);
            db.SysUser.Remove(mo);

            int num = db.SaveChanges();
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
        public QueryDataOutputVM QuerySysLog(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var query = db.SysLog;
            Application.CommonService.QueryJoin(query, ivm, db, ref ovm);

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
        public QueryDataOutputVM QuerySysDictionary(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var query = db.SysDictionary;
            Application.CommonService.QueryJoin(query, ivm, db, ref ovm);

            return ovm;
        }

        /// <summary>
        /// 保存数据字典
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="savetype"></param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM SaveSysDictionary(SysDictionary mo, string savetype)
        {
            var vm = new SharedResultVM();

            if (savetype == "add")
            {
                mo.SdId = Guid.NewGuid().ToString();
                mo.SdPid = Guid.Empty.ToString();
                db.SysDictionary.Add(mo);
            }
            else
            {
                db.SysDictionary.Update(mo);
            }

            int num = db.SaveChanges();
            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 逻辑删除数据字典
        /// </summary>
        /// <param name="id">字典ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DelSysDictionary(string id)
        {
            var vm = new SharedResultVM();

            var mo = db.SysDictionary.Find(id);
            mo.SdStatus = -1;
            db.SysDictionary.Update(mo);

            int num = db.SaveChanges();
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
        public QueryDataOutputVM QuerySysTableConfig(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var query = db.SysTableConfig;
            Application.CommonService.QueryJoin(query, ivm, db, ref ovm);

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
        public SharedResultVM SaveSysTableConfig(SysTableConfig mo, List<string> ColRelation, string savetype)
        {
            var vm = new SharedResultVM();

            mo.ColRelation = string.Join(',', ColRelation);

            if (savetype == "add")
            {
                mo.Id = Guid.NewGuid().ToString();
                db.SysTableConfig.Add(mo);
            }
            else
            {
                db.SysTableConfig.Update(mo);
            }

            int num = db.SaveChanges();
            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 删除表配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DelSysTableConfig(string id)
        {
            var vm = new SharedResultVM();

            var mo = db.SysTableConfig.Find(id);
            db.SysTableConfig.Remove(mo);

            int num = db.SaveChanges();
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