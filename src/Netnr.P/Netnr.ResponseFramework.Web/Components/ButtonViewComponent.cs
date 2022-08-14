namespace Netnr.ResponseFramework.Web.ViewComponents
{
    /// <summary>
    /// 按钮视图组件
    /// </summary>
    public class ButtonViewComponent : ViewComponent
    {
        /// <summary>
        /// 按钮组件
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            string current_url = "/" + RouteData.Values["controller"]?.ToString() + "/" + RouteData.Values["action"]?.ToString();

            //按钮列表
            var listBtn = new List<SysButton>();

            //根据路由反查页面对应的菜单
            var moMenu = CommonService.QuerySysMenuList(x => x.SmUrl?.ToLower() == current_url.ToLower()).FirstOrDefault();
            if (moMenu != null)
            {
                //登录用户的角色信息
                var luri = IdentityService.Role(HttpContext);
                if (luri != null && !string.IsNullOrWhiteSpace(luri.SrButtons))
                {
                    //角色配置的按钮
                    var joRole = luri.SrButtons.DeJson();
                    //根据菜单ID取对应的按钮
                    string btns = joRole.GetValue(moMenu.SmId);
                    if (!string.IsNullOrWhiteSpace(btns))
                    {
                        var btnids = btns.Split(',').ToList();

                        //根据按钮ID取按钮
                        listBtn = CommonService.QuerySysButtonList(x => btnids.Contains(x.SbId));
                    }
                }
            }

            return View(listBtn);
        }
    }
}
