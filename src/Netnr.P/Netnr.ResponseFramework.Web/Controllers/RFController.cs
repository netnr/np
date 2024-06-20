namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 示例，请删除
    /// </summary>
    public class RFController(ContextBase cb) : Controller
    {
        public ContextBase db = cb;

        #region 表配置示例

        /// <summary>
        /// 表配置示例页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Tce()
        {
            return View();
        }

        /// <summary>
        /// 查询表配置示例
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public async Task<QueryDataOutputVM> QueryTempExample(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.TempExample, ivm, db);
            return ovm;
        }

        #endregion

        #region DataGrid示例页面

        /// <summary>
        /// DataGrid示例页面
        /// </summary>
        /// <returns></returns>
        public IActionResult DataGrid()
        {
            return View();
        }

        /// <summary>
        /// 查询表配置
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public async Task<QueryDataOutputVM> QuerySysTableConfig(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.SysTableConfig, ivm, db);
            return ovm;
        }

        #endregion

        #region TreeGrid示例页面

        /// <summary>
        /// TreeGrid示例页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult TreeGrid(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var query = from a in db.SysMenu
                            where a.SmPid == id
                            orderby a.SmOrder
                            select new
                            {
                                a.SmId,
                                a.SmPid,
                                a.SmName,
                                a.SmUrl,
                                a.SmOrder,
                                a.SmIcon,
                                a.SmStatus,
                                a.SmGroup,
                                //查询是否有子集
                                state = (from b in db.SysMenu where b.SmPid == a.SmId select b.SmId).Any() ? "closed" : "open"
                            };
                var list = query.ToList();
                return Content(list.ToJson());
            }

            return View();
        }

        /// <summary>
        /// 查询系统菜单
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public async Task<QueryDataOutputVM> QuerySysMenu(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.SysMenu, ivm, db);
            return ovm;
        }

        #endregion

        #region Grid表格联动

        /// <summary>
        /// Grid表格联动
        /// </summary>
        /// <returns></returns>
        public IActionResult GridChange()
        {
            return View();
        }

        /// <summary>
        /// Grid多表格-主表
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public async Task<QueryDataOutputVM> QueryGridChange1(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.SysRole, ivm, db);
            return ovm;
        }

        /// <summary>
        /// Grid多表格-子表
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public async Task<QueryDataOutputVM> QueryGridChange2(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.SysUser, ivm, db);
            return ovm;
        }

        #endregion

        #region 静态表单示例页面

        /// <summary>
        /// 静态表单示例页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Form()
        {
            return View();
        }

        #endregion

        #region 生成多表单

        /// <summary>
        /// 生成多表单
        /// </summary>
        /// <returns></returns>
        public IActionResult BuildForms()
        {
            return View();
        }

        #endregion

        #region 单据

        /// <summary>
        /// 单据
        /// </summary>
        /// <returns></returns>
        public IActionResult Invoice()
        {
            return View();
        }

        /// <summary>
        /// 查询单据主表
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public async Task<QueryDataOutputVM> QueryInvoiceMain(QueryDataInputVM ivm)
        {
            var ovm = await CommonService.QueryJoin(db.TempInvoiceMain, ivm, db);
            return ovm;
        }

        /// <summary>
        /// 查询单据明细表
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public async Task<QueryDataOutputVM> QueryInvoiceDetail(QueryDataInputVM ivm)
        {
            var query = from a in db.TempInvoiceDetail select a;
            if (string.IsNullOrWhiteSpace(ivm.Pe1))
            {
                query = query.Where(x => false);
            }
            else
            {
                query = query.Where(x => x.TimId == ivm.Pe1);
            }
            var ovm = await CommonService.QueryJoin(query, ivm, db);

            return ovm;
        }

        /// <summary>
        /// 保存单据
        /// </summary>
        /// <param name="moMain"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ResultVM SaveInvoiceForm(TempInvoiceMain moMain, string rows)
        {
            var vm = new ResultVM();

            //明细反序列化为对象
            var listDetail = rows.DeJson<TempInvoiceDetail[]>();

            //新增，补齐主表信息
            var isadd = string.IsNullOrWhiteSpace(moMain.TimId);
            if (isadd)
            {
                moMain.TimId = Guid.NewGuid().ToString();
                moMain.TimCreateTime = DateTime.Now;

                moMain.TimOwnerId = Guid.Empty.ToString();
                moMain.TimOwnerName = "系统登录人员";
            }


            if (isadd)
            {
                db.TempInvoiceMain.Add(moMain);
            }
            else
            {
                db.TempInvoiceMain.Update(moMain);

                //更新时，删除原有明细
                var currDetail = db.TempInvoiceDetail.Where(x => x.TimId == moMain.TimId).ToList();
                if (currDetail.Count > 0)
                {
                    db.TempInvoiceDetail.RemoveRange(currDetail);
                }
            }

            //添加明细
            if (listDetail.Length > 0)
            {
                //初始值
                foreach (var item in listDetail)
                {
                    item.TidId = Guid.NewGuid().ToString();
                    item.TimId = moMain.TimId;
                }

                db.TempInvoiceDetail.AddRange(listDetail);
            }

            int num = db.SaveChanges();

            vm.Set(num > 0);

            if (isadd)
            {
                vm.Data = moMain.TimId;
            }

            return vm;
        }

        #endregion

        #region 上传接口示例

        /// <summary>
        /// 公共上传示例
        /// </summary>
        /// <returns></returns>
        public IActionResult Upload()
        {
            return View();
        }

        #endregion

        #region 富文本

        /// <summary>
        /// 嵌入富文本
        /// </summary>
        /// <returns></returns>
        public IActionResult RichText()
        {
            return View();
        }

        #endregion
    }
}