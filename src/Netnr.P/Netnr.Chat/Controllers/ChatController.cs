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
        public SharedResultVM GetOnlineAllUser()
        {
            var vm = new SharedResultVM();

            try
            {
                vm.Data = ccs.OnlineUser2;
                vm.Set(SharedEnum.RTag.success);
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
        public SharedResultVM PushMessageToUsers([FromBody] ChatMessageVM cm)
        {
            var vm = new SharedResultVM();

            try
            {
                vm = ccs.HandleMessageToUsers(cm, HttpContext, Clients);
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
        [HttpGet]
        public SharedResultVM UserMessageReceipt([FromBody] ChatMessageVM cm)
        {
            var vm = new SharedResultVM();

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
        public SharedResultVM PushMessageToGroup([FromBody] ChatMessageVM cm)
        {
            var vm = new SharedResultVM();

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
        /// 获取未读用户消息数量
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetUnreadUserMessageCount(string UserId)
        {
            var vm = new SharedResultVM();

            try
            {
                vm.Data = ccs.GetUnreadUserMessageCount(db, UserId);
                vm.Set(SharedEnum.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取未读组消息数量
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM GetUnreadGroupMessageCount(string UserId)
        {
            var vm = new SharedResultVM();

            try
            {
                vm.Data = ccs.GetUnreadGroupMessageCount(db, UserId);
                vm.Set(SharedEnum.RTag.success);
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
        public SharedResultVM GroupNew([FromBody] ChatGroupVM cg)
        {
            var vm = new SharedResultVM();

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