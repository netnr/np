using chs = Netnr.Chat.Application.ChatHubService;

namespace Netnr.Chat.Controllers
{
    [Route("[controller]/[action]")]
    public class AdminController : Controller
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取所有在线用户
        /// </summary>
        /// <param name="key">密码</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetOnlineAllUser(string key)
        {
            return SharedResultVM.Try(vm =>
            {
                var ak = SharedFast.GlobalTo.GetValue<string>("TokenManagement:AdminKey");
                if (!string.IsNullOrWhiteSpace(ak) && key == ak)
                {
                    vm.Data = chs.OnlineUser1;
                    vm.Set(SharedEnum.RTag.success);
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
