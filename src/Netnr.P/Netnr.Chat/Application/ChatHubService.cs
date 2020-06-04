using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using ccs = Netnr.Chat.Application.ChatContextService;

namespace Netnr.Chat.Application
{
    [Authorize]
    public class ChatHubService : Hub
    {
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var cu = ccs.GetChatUserInfo(Context);
            ccs.UserOnline(cu);

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 断开（非调试模式）
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var cu = ccs.GetChatUserInfo(Context);
            ccs.UserOffline(cu);

            return base.OnDisconnectedAsync(exception);
        }
    }
}