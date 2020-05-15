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

        //#region 群聊
        ///// <summary>
        ///// 创建聊天室
        ///// </summary>
        ///// <param name="roomName"></param>
        //public void CreateRoom(string roomName)
        //{
        //    var room = cc.Rooms.Find(x => x.RoomName == roomName);
        //    if (room == null)
        //    {
        //        var rom = new ChatRoom
        //        {
        //            RoomName = roomName,
        //            RoomId = Guid.NewGuid().ToString().ToUpper()
        //        };
        //        cc.Rooms.Add(rom);//加入房间列表
        //        UpdateAllRoomList();//更新房间列表
        //        Clients.Client(Context.ConnectionId).showGroupMsg("success");
        //    }
        //    else
        //    {
        //        Clients.Client(Context.ConnectionId).showGroupMsg("error");
        //    }
        //}

        ///// <summary>
        /////加入聊天室
        ///// </summary>
        //public void JoinRoom(string roomId, string current_Id)
        //{
        //    // 查询聊天室，
        //    var room = cc.Rooms.Find(x => x.RoomId == roomId.Trim());
        //    var u = userInfoList.Find(x => x.UserID == current_Id);
        //    if (room != null)
        //    {
        //        //检测该用户是否存在在该房间
        //        var isExistUser = room.Users.Find(x => x.UserConnectionId == Context.ConnectionId);
        //        if (isExistUser == null)
        //        {
        //            var user = cc.Users.Find(x => x.UserConnectionId == Context.ConnectionId);
        //            user.Rooms.Add(room);//用户信息中加入房间信息
        //            room.Users.Add(user);//房间信息中加入用户信息
        //            Groups.Add(Context.ConnectionId, room.RoomName);//添加到组中
        //            Clients.Group(room.RoomName, new string[0]).showSysGroupMsg(u.UserName);
        //        }
        //    }
        //    else
        //    {
        //        Clients.Client(Context.ConnectionId).showMessage("该群组不存在");
        //    }
        //}

        ///// <summary>
        ///// 给指定房间内的所有用户发消息
        ///// </summary>
        ///// <param name="room">房间名</param>
        ///// <param name="message">消息</param>
        //public void SendMessageByRoom(string roomId, string current_Id, string message)
        //{
        //    var room = cc.Rooms.FirstOrDefault(x => x.RoomId == roomId);
        //    var user = userInfoList.Find(x => x.UserID == current_Id);
        //    if (room != null && user != null)
        //    {
        //        Clients.Group(room.RoomName, new string[0]).showGroupByRoomMsg(user.UserName, room.RoomId, message);
        //        AddChatHistory(ChatType.GroChat, user.UserName, message, user.UserID, "", room.RoomId);
        //    }
        //}

        ///// <summary>
        ///// 退出房间
        ///// </summary>
        //public void RemoveRoom(string roomId)
        //{
        //    var room = cc.Rooms.Find(x => x.RoomId == roomId);
        //    if (room != null)
        //    {
        //        var user = cc.Users.Find(x => x.UserConnectionId == Context.ConnectionId);
        //        room.Users.Remove(user);//从房间里移除该用户
        //        if (room.Users.Count <= 0)
        //        {
        //            cc.Rooms.Remove(room);//如果房间里没人了，删除该房间
        //        }
        //        Groups.Remove(Context.ConnectionId, room.RoomName);
        //        UpdateAllRoomList();//更新房间列表
        //        Clients.Client(Context.ConnectionId).removeRoom();
        //    }
        //    else
        //    {
        //        Clients.Client(Context.ConnectionId).showMessage("该房间不存在");
        //    }
        //}

        //#endregion

        //#region  method
        ///// <summary>
        ///// 获取所有在线用户集合
        ///// </summary>
        //public void GetAllOnlineUser()
        //{
        //    var uList = JsonHelper.ToJsonString(userInfoList);
        //    Clients.All.showUserList(uList);
        //}


        ///// <summary>
        ///// 获取历史记录
        ///// </summary>
        ///// <param name="chatType">消息类型0公共聊天，1好友，2群</param>
        ///// <param name="toId">接收者id</param>
        ///// <param name="frmId">发送方id</param>
        ///// <param name="roomId">房间id</param>
        //public void GetChatHistory(int chatType = (int)ChatEnumType.Public, string toId = "", string frmId = "", string roomId = "")
        //{
        //    var list = chatHistoryList;
        //    var type = (ChatEnumType)chatType;
        //    switch (type)
        //    {
        //        case ChatEnumType.Public:
        //            list = chatHistoryList.Where(x => x.ChatType == type).ToList();
        //            break;
        //        case ChatEnumType.Private:
        //            //自己发送给对方的，和对方发给自己的数据集合
        //            list = chatHistoryList.Where(x => x.ChatType == type && ((x.toId == toId && x.frmId == frmId) || (x.toId == frmId && x.frmId == toId))).ToList();
        //            break;
        //        case ChatEnumType.Group:
        //            list = chatHistoryList.Where(x => x.ChatType == type && x.RoomId == roomId).ToList();
        //            break;
        //        default:
        //            list = new List<ChatHistory>();
        //            break;
        //    }
        //    var data = JsonHelper.ToJsonString(list);
        //    var user = userInfoList.FirstOrDefault(x => x.UserID == frmId);
        //    var conid = Context.ConnectionId;
        //    if (user != null)
        //    {
        //        conid = user.ConnectionId;
        //    }
        //    Clients.Client(conid).initChatHistoryData(data, chatType);
        //}
        ///// <summary>
        ///// 添加历史记录数据
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="message"></param>
        ///// <param name="chatType">0公共聊天，1私聊，2群聊</param>
        //public void AddChatHistory(ChatEnumType chatType = 0, string userName = "", string message = "", string frmId = "", string toId = "", string roomId = "")
        //{
        //    ChatHistory history = new ChatHistory()
        //    {
        //        Hid = Guid.NewGuid().ToString().ToUpper(),
        //        ChatType = chatType,
        //        Message = message,
        //        UserName = userName,
        //        frmId = frmId,
        //        toId = toId,
        //        RoomId = roomId
        //    };
        //    chatHistoryList.Add(history);
        //}

        //#endregion
    }
}