using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Netnr.Chat.Application.ViewModel;
using ccs = Netnr.Chat.Application.ChatContextService;

namespace Netnr.Chat.Controllers
{
    /// <summary>
    /// 接口
    /// </summary>
    [Route("[controller]/[action]")]
    [Authorize]
    public class ChatController : Controller
    {
        readonly IHubClients Clients;
        readonly IGroupManager Groups;

        private readonly Data.ContextBase db;

        public ChatController(IHubContext<Application.ChatHubService> hub, Data.ContextBase _db)
        {
            Clients = hub.Clients;
            Groups = hub.Groups;

            db = _db;
        }

        /// <summary>
        /// 获取所有在线用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResultVM GetOnlineAllUser()
        {
            var vm = new ActionResultVM();

            try
            {
                vm.Data = ccs.OnlineUser2;
                vm.Set(ARTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 推送消息到用户
        /// </summary>
        /// <param name="cm">推送消息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResultVM PushMessageToUsers([FromForm]ChatMessageVM cm)
        {
            var vm = new ActionResultVM();

            try
            {
                vm = ccs.HandleMessageToUser(cm, HttpContext, Clients);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 用户消息回执
        /// </summary>
        /// <param name="cm">推送消息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResultVM UserMessageReceipt([FromForm]ChatMessageVM cm)
        {
            var vm = new ActionResultVM();

            try
            {
                vm = ccs.HandleUserMessageReceipt(cm, HttpContext, db);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 推送消息到组
        /// </summary>
        /// <param name="cm">推送消息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResultVM PushMessageToGroups([FromForm]ChatMessageVM cm)
        {
            var vm = new ActionResultVM();

            try
            {
                vm = ccs.HandleMessageToGroup(cm, HttpContext, Clients);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 新建组
        /// </summary>
        /// <param name="cg">组信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResultVM GroupNew([FromForm]ChatGroupVM cg)
        {
            var vm = new ActionResultVM();

            try
            {
                vm = ccs.HandelGroupNew(cg, Groups, db);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

    }
}