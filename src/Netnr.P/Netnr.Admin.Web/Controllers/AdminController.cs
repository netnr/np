namespace Netnr.Admin.Web.Controllers
{
    /// <summary>
    /// 基础
    /// </summary>
    [FilterConfigs.IsCORS]
    [Route("[controller]/[action]")]
    public class AdminController : MainController
    {
        public ContextBase db;

        public AdminController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="paramsJson"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM MenuGet(string paramsJson)
        {
            LogContent("菜单列表");

            var vm = GetQuery(db.BaseMenu, paramsJson);
            return vm;
        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> MenuPost(BaseMenu model)
        {
            var vm = new ResultVM();

            try
            {
                model.MenuId = SnowflakeTo.Id();
                model.CreateTime = DateTime.Now;

                await db.BaseMenu.AddAsync(model);
                var num = await db.SaveChangesAsync();
                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResultVM> MenuPut(BaseMenu model)
        {
            var vm = new ResultVM();

            try
            {
                db.BaseMenu.Update(model);
                var num = await db.SaveChangesAsync();
                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResultVM> MenuDelete(long menuId)
        {
            var vm = new ResultVM();

            try
            {
                BaseMenu model = new() { MenuId = menuId };
                db.BaseMenu.Attach(model);

                db.BaseMenu.Remove(model);
                var num = await db.SaveChangesAsync();
                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="paramsJson"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultVM LogGet(string paramsJson)
        {
            LogContent("日志列表");

            var vm = GetQuery(db.BaseLog, paramsJson);
            return vm;
        }
    }
}
