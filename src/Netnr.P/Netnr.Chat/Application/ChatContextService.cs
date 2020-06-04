using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Netnr.Chat.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Netnr.Chat.Application
{
    /// <summary>
    /// Chat上下文
    /// </summary>
    public class ChatContextService
    {
        #region 缓存

        /// <summary>
        /// 系统用户，用户ID 对应 用户信息
        /// </summary>
        public static Dictionary<string, ChatUserVM> OnlineUser1 = new Dictionary<string, ChatUserVM>()
        {
            {
                "0",
                new ChatUserVM
                {
                    Conns = new Dictionary<string, ChatConnectionVM>()
                    {
                        {
                            "0",
                            new ChatConnectionVM()
                            {
                                ConnId = "0",
                                ConnSign = "system"
                            }
                        }
                    },
                    UserId = "0",
                    UserName = "0"
                }
            }
        };

        /// <summary>
        /// 在线用户，用户ID 对应 用户信息
        /// </summary>
        public static Dictionary<string, ChatUserVM> OnlineUser2 = new Dictionary<string, ChatUserVM>();

        /// <summary>
        /// 组，组ID 对应 组信息
        /// </summary>
        public static Dictionary<string, ChatGroupVM> OnlineGroup1 = new Dictionary<string, ChatGroupVM>();

        /// <summary>
        /// 用户消息记录，接收用户ID 对应 发送消息
        /// </summary>
        public static Dictionary<string, List<Domain.NChatMessageToUser>> UserMessage1 = new Dictionary<string, List<Domain.NChatMessageToUser>>();

        /// <summary>
        /// 组消息记录，接收组ID 对应 发送消息
        /// </summary>
        public static Dictionary<string, List<Domain.NChatMessageToGroup>> GroupMessage1 = new Dictionary<string, List<Domain.NChatMessageToGroup>>();

        #endregion

        #region 处理事件

        /// <summary>
        /// 处理用户消息发送
        /// </summary>
        /// <param name="cm">发送消息</param>
        /// <param name="hc">上下文</param>
        /// <param name="Clients">连接客户端对象</param>
        public static ActionResultVM HandleMessageToUsers(ChatMessageVM cm, HttpContext hc, IHubClients Clients)
        {
            var vm = new ActionResultVM();
            var pmax = GlobalTo.GetValue<int>("NetnrChat:BatchPushUserMax");
            var rme = GlobalTo.GetValue("NetnrChat:ReceiveMessage");

            if (cm == null)
            {
                vm.Set(ARTag.lack);
                vm.Msg = "消息主体不能为空";
            }
            else if (string.IsNullOrWhiteSpace(cm.CmFromId))
            {
                vm.Set(ARTag.lack);
                vm.Msg = "发送用户ID不能为空";
            }
            else if (cm.CmContent == null)
            {
                vm.Set(ARTag.lack);
                vm.Msg = "发送内容不能为空";
            }
            else if (!Enum.TryParse(cm.CmType, true, out MessageType mt))
            {
                vm.Set(ARTag.lack);
                vm.Msg = "消息类型有误";
            }
            else if (cm.CmToIds == null || cm.CmToIds.Count == 0)
            {
                vm.Set(ARTag.lack);
                vm.Msg = "接收用户ID不能为空";
            }
            else if (cm.CmToIds.Count > pmax)
            {
                vm.Set(ARTag.refuse);
                vm.Msg = $"接收用户限制为最多{pmax}";
            }
            else
            {
                //接收用户
                var users = FindUsers(cm.CmToIds);

                if (users.Count > 0)
                {
                    cm.CmId = NewMessageId();
                    cm.CmType = mt.ToString();
                    cm.CmTime = DateTime.Now;
                    cm.CmWhich = "User";

                    //用户连接信息
                    var cu = GetChatUserInfo(hc);
                    cm.CmFromConn = cu?.Conns.Values.FirstOrDefault();

                    //开始推送
                    foreach (var user in users)
                    {
                        var isonline = user.Conns.Count > 0;

                        //在线
                        if (isonline)
                        {
                            Clients.User(user.UserId).SendAsync(rme, cm);
                        }

                        //写入消息
                        var cs = isonline ? 2 : 1;
                        var wcm = SaveAsUserMessage(cm);
                        wcm.ForEach(x => x.CmuStatus = cs);
                        AddMessage(wcm);
                    }

                    //发送成功，返回消息ID
                    vm.Data = cm.CmId;
                    vm.Set(ARTag.success);
                }
                else
                {
                    vm.Set(ARTag.invalid);
                    vm.Msg = "接收用户ID无效";
                }
            }

            return vm;
        }

        /// <summary>
        /// 处理用户消息回执
        /// </summary>
        /// <param name="cm">发送消息</param>
        /// <param name="hc">上下文</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        public static ActionResultVM HandleUserMessageReceipt(ChatMessageVM cm, HttpContext hc, Data.ContextBase db)
        {
            var vm = new ActionResultVM();

            if (cm == null)
            {
                vm.Set(ARTag.lack);
                vm.Msg = "消息主体不能为空";
            }
            else if (string.IsNullOrWhiteSpace(cm.CmId))
            {
                vm.Set(ARTag.lack);
                vm.Msg = "消息ID不能为空";
            }
            else
            {
                vm.Set(ARTag.invalid);
                vm.Msg = "消息ID无效";

                //用户连接信息
                var cu = GetChatUserInfo(hc);

                //缓存消息
                if (UserMessage1.ContainsKey(cu.UserId))
                {
                    var mb = UserMessage1[cu.UserId].FirstOrDefault(x => x.CmuId == cm.CmId);
                    if (mb != null)
                    {
                        mb.CmuStatus = cm.CmStatus;

                        vm.Set(ARTag.success);
                    }
                }
                else
                {
                    var mb = db.NChatMessageToUser.Find(cm.CmId);
                    if (mb != null)
                    {
                        mb.CmuStatus = cm.CmStatus;
                        db.SaveChanges();

                        vm.Set(ARTag.success);
                    }
                }
            }

            return vm;
        }

        /// <summary>
        /// 处理组消息发送
        /// </summary>
        /// <param name="cm">发送消息</param>
        /// <param name="hc">上下文</param>
        /// <param name="Clients">连接客户端对象</param>
        public static ActionResultVM HandleMessageToGroup(ChatMessageVM cm, HttpContext hc, IHubClients Clients)
        {
            var vm = new ActionResultVM();
            var pmax = GlobalTo.GetValue<int>("NetnrChat:BatchPushGroupMax");
            var rme = GlobalTo.GetValue("NetnrChat:ReceiveMessage");

            if (cm == null)
            {
                vm.Set(ARTag.lack);
                vm.Msg = "消息主体不能为空";
            }
            else if (string.IsNullOrWhiteSpace(cm.CmFromId))
            {
                vm.Set(ARTag.lack);
                vm.Msg = "发送用户ID不能为空";
            }
            else if (cm.CmContent == null)
            {
                vm.Set(ARTag.lack);
                vm.Msg = "发送内容不能为空";
            }
            else if (!Enum.TryParse(cm.CmType, true, out MessageType mt))
            {
                vm.Set(ARTag.lack);
                vm.Msg = "消息类型有误";
            }
            else if (cm.CmToIds == null || cm.CmToIds.Count == 0)
            {
                vm.Set(ARTag.lack);
                vm.Msg = "接收组ID不能为空";
            }
            else if (cm.CmToIds.Count > pmax)
            {
                vm.Set(ARTag.refuse);
                vm.Msg = $"接收组限制为最多{pmax}";
            }
            else
            {
                //接收用户
                var groups = FindGroup(cm.CmToIds);

                if (groups.Count > 0)
                {
                    cm.CmId = NewMessageId();
                    cm.CmType = mt.ToString();
                    cm.CmTime = DateTime.Now;
                    cm.CmWhich = "Group";

                    //用户连接信息
                    var cu = GetChatUserInfo(hc);
                    cm.CmFromConn = cu?.Conns.Values.FirstOrDefault();

                    //发送消息
                    var groupids = groups.Select(x => x.GroupId).ToList();
                    Clients.Groups(groupids).SendAsync(rme, cm);

                    //写入消息
                    var wcm = SaveAsGroupMessage(cm);
                    AddMessage(wcm);

                    //发送成功，返回消息ID
                    vm.Data = cm.CmId;
                    vm.Set(ARTag.success);
                }
                else
                {
                    vm.Set(ARTag.invalid);
                    vm.Msg = "接收组ID无效";
                }
            }

            return vm;
        }

        /// <summary>
        /// 处理新建组
        /// </summary>
        /// <param name="cg">组信息</param>
        /// <param name="Groups"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ActionResultVM HandelGroupNew(ChatGroupVM cg, IGroupManager Groups, Data.ContextBase db)
        {
            var vm = new ActionResultVM();

            if (cg == null)
            {
                vm.Set(ARTag.lack);
                vm.Msg = "组信息不能为空";
            }
            else if (string.IsNullOrWhiteSpace(cg.GroupName))
            {
                vm.Set(ARTag.lack);
                vm.Msg = "组名称不能为空";
            }
            else if (cg.GroupUser.Count == 0)
            {
                vm.Set(ARTag.fail);
                vm.Msg = "组用户至少有一个（默认第一个用户为管理员）";
            }

            //管理员
            var users = FindUsers(cg.GroupUser.Keys.ToList());
            if (users?.Count > 0)
            {
                var now = DateTime.Now;

                cg.GroupId = Core.UniqueTo.LongId().ToString();

                //组成员
                var listgm = new List<Domain.NChatGroupMember>();

                //用户加入组
                foreach (var user in users)
                {
                    foreach (var conn in user.Conns.Keys)
                    {
                        Groups.AddToGroupAsync(conn, cg.GroupId);
                    }

                    //添加组成员
                    listgm.Add(new Domain.NChatGroupMember()
                    {
                        CgmId = Core.UniqueTo.LongId().ToString(),
                        CgId = cg.GroupId,
                        CuUserId = user.UserId,
                        CgmCreateTime = now,
                        CgmStatus = 1
                    });
                }

                //维护组
                OnlineGroup1.Add(cg.GroupId, cg);

                //写入表
                db.NChatGroup.Add(new Domain.NChatGroup()
                {
                    CgId = cg.GroupId,
                    CgName = cg.GroupName,
                    CgOwnerId = users.FirstOrDefault().UserId,
                    CgCreateTime = DateTime.Now,
                    CcId = string.IsNullOrWhiteSpace(cg.GroupClassify) ? "1" : cg.GroupClassify,
                    CgStatus = 1
                });
                db.NChatGroupMember.AddRange(listgm);
                db.SaveChangesAsync();

                //成功
                vm.Data = cg.GroupId;
                vm.Set(ARTag.success);
            }
            else
            {
                vm.Set(ARTag.invalid);
                vm.Msg = "组用户无效";
            }

            return vm;
        }

        ///// <summary>
        ///// 处理加入组
        ///// </summary>
        ///// <param name="groupVM">组信息</param>
        ///// <returns></returns>
        //public static ActionResultVM HandelJoinGroup(ChatGroupVM groupVM)
        //{
        //    var group = ccs.FindGroup(groupVM.GroupId);
        //    if (group != null)
        //    {
        //        //加入用户
        //        var users = ccs.FindUsers(groupVM.GroupUser.Keys.ToList());

        //        if (users.Count > 0)
        //        {
        //            //用户加入组
        //            foreach (var user in users)
        //            {
        //                foreach (var conn in user.Conns.Keys)
        //                {
        //                    await Groups.AddToGroupAsync(conn, group.GroupConnId);
        //                }

        //                //维护组
        //                ccs.OnlineGroup1[groupVM.GroupId].GroupUser.Add(user.UserId, user);
        //            }

        //            //推通知
        //            await PushMessageToGroup(new ChatMessageVM()
        //            {
        //                CmFromId = ccs.OnlineUser1.Values.First().UserId,
        //                CmType = MessageType.EventJoinGroup.ToString(),
        //                CmContent = users,
        //                CmToIds = new List<string> { groupVM.GroupId }
        //            });
        //        }
        //    }
        //}

        ///// <summary>
        ///// 处理离开组
        ///// </summary>
        ///// <param name="groupVM">组信息</param>
        ///// <returns></returns>
        //public static ActionResultVM HandelLeaveGroup(ChatGroupVM groupVM)
        //{
        //    var group = ccs.FindGroup(groupVM.GroupId);
        //    if (group != null)
        //    {
        //        //离开用户
        //        var users = ccs.FindUsers(groupVM.GroupUser.Keys.ToList());

        //        if (users.Count > 0)
        //        {
        //            //删除用户
        //            foreach (var user in users)
        //            {
        //                foreach (var conn in user.Conns.Keys)
        //                {
        //                    await Groups.RemoveFromGroupAsync(conn, group.GroupConnId);
        //                }

        //                //维护组
        //                ccs.OnlineGroup1[groupVM.GroupId].GroupUser.Remove(user.UserId);
        //            }

        //            //推通知
        //            await PushMessageToGroup(new ChatMessageVM()
        //            {
        //                CmFromId = ccs.OnlineUser1.Values.First().UserId,
        //                CmType = MessageType.EventLeaveGroup.ToString(),
        //                CmContent = users,
        //                CmToIds = new List<string> { groupVM.GroupId }
        //            });
        //        }
        //    }
        //}

        ///// <summary>
        ///// 处理删除组
        ///// </summary>
        ///// <param name="groupVM">组信息</param>
        ///// <returns></returns>
        //public static ActionResultVM HandelDelGroup(ChatGroupVM groupVM)
        //{
        //    var group = ccs.FindGroup(groupVM.GroupId);
        //    if (group != null)
        //    {
        //        //组所有用户
        //        var users = ccs.FindUsers(group.GroupUser.Keys.ToList());

        //        if (users.Count > 0)
        //        {
        //            //删除组
        //            foreach (var user in users)
        //            {
        //                foreach (var conn in user.Conns.Keys)
        //                {
        //                    await Groups.RemoveFromGroupAsync(conn, group.GroupConnId);
        //                }
        //            }

        //            //维护组
        //            ccs.OnlineGroup1.Remove(group.GroupId);

        //            //推通知
        //            await PushMessageToGroup(new ChatMessageVM()
        //            {
        //                CmFromId = ccs.OnlineUser1.Values.First().UserId,
        //                CmType = MessageType.EventDelGroup.ToString(),
        //                CmToIds = new List<string> { groupVM.GroupId }
        //            });
        //        }
        //    }
        //}

        #endregion

        #region 方法

        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <param name="hc">上下文</param>
        /// <returns></returns>
        public static ChatUserVM GetChatUserInfo(HttpContext hc)
        {
            if (hc.User.Identity.IsAuthenticated)
            {
                var ud = hc.User.FindFirstValue(ClaimTypes.UserData)?.ToJObject();

                var cu = new ChatUserVM()
                {
                    UserId = hc.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    UserName = hc.User.FindFirstValue(ClaimTypes.Name),
                    UserPhoto = ud["UserPhoto"]?.ToString()
                };

                return cu;
            }

            return null;
        }

        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <param name="cc">连接对象</param>
        /// <returns></returns>
        public static ChatUserVM GetChatUserInfo(HubCallerContext cc)
        {
            var hc = cc.GetHttpContext();
            var cu = GetChatUserInfo(hc);

            if (cu != null)
            {
                var ud = hc.User.FindFirstValue(ClaimTypes.UserData)?.ToJObject();
                cu.Conns = new Dictionary<string, ChatConnectionVM>()
                {
                    {
                        cc.ConnectionId,
                        new ChatConnectionVM
                        {
                            ConnId = cc.ConnectionId,
                            UserAgent = hc.Request.Headers["User-Agent"],
                            UserDevice = ud["Device"]?.ToString(),
                            ConnSign = ud["Sign"]?.ToString()
                        }
                    }
                };
            }

            return cu;
        }

        /// <summary>
        /// 用户上线
        /// </summary>
        public static void UserOnline(ChatUserVM ou)
        {
            var user = FindOnlineUser(ou.UserId);
            if (user == null)
            {
                OnlineUser2.Add(ou.UserId, ou);
            }
            else
            {
                var conn = ou.Conns.FirstOrDefault();
                OnlineUser2[ou.UserId].Conns.Add(conn.Key, conn.Value);
            }
        }

        /// <summary>
        /// 用户下线
        /// </summary>
        public static void UserOffline(ChatUserVM ou)
        {
            var user = FindOnlineUser(ou?.UserId);
            if (user != null)
            {
                var cu = OnlineUser2[ou.UserId];

                //多端登录
                if (cu.Conns.Count > 1)
                {
                    cu.Conns.Remove(ou.Conns.FirstOrDefault().Key);
                }
                else
                {
                    OnlineUser2.Remove(ou.UserId);
                }
            }
        }

        /// <summary>
        /// 新消息ID
        /// </summary>
        /// <returns></returns>
        public static string NewMessageId()
        {
            return "m" + Core.UniqueTo.LongId();
        }

        /// <summary>
        /// 推送消息转存储消息
        /// </summary>
        /// <param name="cm"></param>
        public static List<Domain.NChatMessageToUser> SaveAsUserMessage(ChatMessageVM cm)
        {
            var listCm = new List<Domain.NChatMessageToUser>();

            cm.CmToIds.ForEach(id =>
            {
                listCm.Add(new Domain.NChatMessageToUser()
                {
                    CmuId = cm.CmId,
                    CmuPushUserId = cm.CmFromId,
                    CmuPullUserId = id,
                    CmuContent = cm.CmContent.ToJson(),
                    CmuPushWhich = cm.CmWhich,
                    CmuPushType = cm.CmType,
                    CmuCreateTime = cm.CmTime,
                    CmuPushUserDevice = cm.CmFromConn?.UserDevice,
                    CmuPushUserSign = cm.CmFromConn?.ConnSign
                });
            });

            return listCm;
        }

        /// <summary>
        /// 推送消息转存储消息
        /// </summary>
        /// <param name="cm"></param>
        public static List<Domain.NChatMessageToGroup> SaveAsGroupMessage(ChatMessageVM cm)
        {
            var listCm = new List<Domain.NChatMessageToGroup>();

            cm.CmToIds.ForEach(id =>
            {
                listCm.Add(new Domain.NChatMessageToGroup()
                {
                    CmgId = cm.CmId,
                    CmgPushUserId = cm.CmFromId,
                    CmgPullGroupId = id,
                    CmgContent = cm.CmContent.ToJson(),
                    CmgPushWhich = cm.CmWhich,
                    CmgPushType = cm.CmType,
                    CmgCreateTime = cm.CmTime,
                    CmgPushUserDevice = cm.CmFromConn?.UserDevice,
                    CmgPushUserSign = cm.CmFromConn?.ConnSign
                });
            });

            return listCm;
        }

        /// <summary>
        /// 新增用户消息
        /// </summary>
        /// <param name="cms"></param>
        public static void AddMessage(List<Domain.NChatMessageToUser> cms)
        {
            cms.ForEach(cm =>
            {
                if (UserMessage1.ContainsKey(cm.CmuPullUserId))
                {
                    UserMessage1[cm.CmuPullUserId].Add(cm);
                }
                else
                {
                    UserMessage1.Add(cm.CmuPullUserId, new List<Domain.NChatMessageToUser> { cm });
                }
            });
        }

        /// <summary>
        /// 新增组消息
        /// </summary>
        /// <param name="cms"></param>
        public static void AddMessage(List<Domain.NChatMessageToGroup> cms)
        {
            cms.ForEach(cm =>
            {
                if (GroupMessage1.ContainsKey(cm.CmgPullGroupId))
                {
                    GroupMessage1[cm.CmgPullGroupId].Add(cm);
                }
                else
                {
                    GroupMessage1.Add(cm.CmgPullGroupId, new List<Domain.NChatMessageToGroup> { cm });
                }
            });
        }

        /// <summary>
        /// 根据用户ID找到用户信息
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        public static List<ChatUserVM> FindUsers(List<string> UserId)
        {
            var users = new List<ChatUserVM>();

            var offid = new List<string>();

            Parallel.ForEach(UserId, id =>
            {
                if (OnlineUser2.ContainsKey(id))
                {
                    users.Add(OnlineUser2[id]);
                }
                else if (OnlineUser1.ContainsKey(id))
                {
                    users.Add(OnlineUser1[id]);
                }
                else
                {
                    offid.Add(id);
                }
            });

            //离线用户
            if (offid.Count > 0)
            {
                using var db = new Data.ContextBase(Data.ContextBase.DCOB().Options);

                var offu = db.NChatUser.Where(x => offid.Contains(x.CuUserId)).Select(x => new ChatUserVM()
                {
                    UserId = x.CuUserId,
                    UserName = x.CuUserName,
                    UserPhoto = x.CuUserPhoto
                }).ToList();

                users.AddRange(offu);
            }

            return users;
        }

        /// <summary>
        /// 根据用户ID找到在线用户信息
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        public static ChatUserVM FindOnlineUser(string UserId)
        {
            if (!string.IsNullOrEmpty(UserId) && OnlineUser2.ContainsKey(UserId))
            {
                return OnlineUser2[UserId];
            }
            return null;
        }

        /// <summary>
        /// 根据组ID找到组信息
        /// </summary>
        /// <param name="GroupId">组ID</param>
        /// <returns></returns>
        public static List<ChatGroupVM> FindGroup(List<string> GroupId)
        {
            var groups = new List<ChatGroupVM>();

            GroupId.ForEach(x =>
            {
                if (OnlineGroup1.ContainsKey(x))
                {
                    groups.Add(OnlineGroup1[x]);
                }
            });

            return groups;
        }

        #endregion
    }
}
