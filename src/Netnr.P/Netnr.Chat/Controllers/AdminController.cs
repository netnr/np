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
        public ResultVM GetOnlineAllUser(string key)
        {
            return ResultVM.Try(vm =>
            {
                var ak = GlobalTo.GetValue<string>("TokenManagement:AdminKey");
                if (!string.IsNullOrWhiteSpace(ak) && key == ak)
                {
                    vm.Data = chs.OnlineUser1;
                    vm.Set(EnumTo.RTag.success);
                }
                else
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }

                return vm;
            });
        }

    }
}
