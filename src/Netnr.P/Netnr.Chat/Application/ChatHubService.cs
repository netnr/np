using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Netnr.Chat.Application.ViewModel;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.Chat.Application
{
    public class ChatHubService : Hub
    {
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var uc = GetUserConnInfo(Context, true);
            //未授权，拒绝连接
            if (uc == null)
            {
                Context.Abort();
            }
            else
            {
                await base.OnConnectedAsync();
            }
        }

        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            GetUserConnInfo(Context, false);
            return base.OnDisconnectedAsync(exception);
        }


        #region 缓存

        /// <summary>
        /// 系统用户，用户ID 对应 用户信息
        /// </summary>
        public static Dictionary<string, ChatUserConnVM> OnlineUser9 = new()
        {
            {
                "9",
                new ChatUserConnVM
                {
                    Conns = new Dictionary<string, ChatConnectionVM>()
                    {
                        {
                            "9",
                            new ChatConnectionVM()
                            {
                                ConnId = "9",
                                UserSign = "system"
                            }
                        }
                    },
                    UserId = "9",
                    UserName = "9"
                }
            }
        };

        /// <summary>
        /// 在线用户，用户ID 对应 用户信息
        /// </summary>
        public static Dictionary<string, ChatUserConnVM> OnlineUser1 = new();

        #endregion

        #region 方法

        /// <summary>
        /// 获取授权用户信息（无连接信息）
        /// </summary>
        /// <param name="hc">上下文</param>
        /// <param name="token">授权码</param>
        /// <returns></returns>
        public static ChatUserConnVM GetUserAuthInfo(HttpContext hc, string token = null)
        {
            ChatUserConnVM vm = null;

            //cookie 授权
            if (GlobalTo.GetValue<bool>("TokenManagement:EnableCookie") && hc.User.Identity.IsAuthenticated)
            {
                vm.GetType().GetProperties().ToList().ForEach(pi =>
                {
                    try
                    {
                        var val = Convert.ChangeType(hc.User.FindFirstValue(pi.Name), pi.PropertyType);
                        pi.SetValue(vm, val);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            }
            else
            {
                try
                {
                    //验证 token
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        token = hc.Request.Query["access_token"].ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var key = GlobalTo.GetValue("TokenManagement:Secret");

                        var model = CalcTo.AESDecrypt(token, key).ToEntity<ChatUserConnVM>();
                        if (DateTime.Now.ToTimestamp() < model.ExpireDate)
                        {
                            vm = model;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return vm;
        }

        /// <summary>
        /// 获取用户连接信息
        /// </summary>
        /// <param name="cc">连接对象</param>
        /// <param name="isOnline">上线、下线</param>
        /// <returns></returns>
        public static ChatUserConnVM GetUserConnInfo(HubCallerContext cc, bool isOnline)
        {
            var hc = cc.GetHttpContext();
            var uc = GetUserAuthInfo(hc);

            if (uc != null)
            {
                //补上连接信息
                if (OnlineUser1.ContainsKey(uc.UserId))
                {
                    uc.Conns = OnlineUser1[uc.UserId].Conns;
                }

                //上线
                if (isOnline)
                {
                    //追加连接
                    if (!uc.Conns.ContainsKey(cc.ConnectionId))
                    {
                        uc.Conns.Add(cc.ConnectionId, new ChatConnectionVM
                        {
                            ConnId = cc.ConnectionId,
                            UserDevice = uc.UserDevice,
                            UserSign = uc.UserSign
                        });
                    }

                    //初次
                    if (!OnlineUser1.ContainsKey(uc.UserId))
                    {
                        OnlineUser1.Add(uc.UserId, uc);
                    }
                    else
                    {
                        OnlineUser1[uc.UserId] = uc;
                    }
                }
                else
                {
                    //下线
                    if (uc.Conns.ContainsKey(cc.ConnectionId))
                    {
                        uc.Conns.Remove(cc.ConnectionId);
                    }

                    if (uc.Conns.Count == 0)
                    {
                        OnlineUser1.Remove(uc.UserId);
                    }
                }
            }

            return uc;
        }

        /// <summary>
        /// 新消息ID
        /// </summary>
        /// <returns></returns>
        public static string NewMessageId()
        {
            return "m" + UniqueTo.LongId();
        }

        /// <summary>
        /// 推送消息转存储消息
        /// </summary>
        /// <param name="cm"></param>
        public static List<Domain.NChatMessageToUser> WriteMessageForUser(ChatMessageVM cm)
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
                    CmuPushUserDevice = cm.CmFromDevice,
                    CmuPushUserSign = cm.CmFromSign
                });
            });

            return listCm;
        }

        /// <summary>
        /// 推送消息转存储消息（反转）
        /// </summary>
        /// <param name="listCmo"></param>
        /// <returns></returns>
        public static List<ChatMessageVM> WriteMessageForUserReverse(List<Domain.NChatMessageToUser> listCmo)
        {
            var cms = new List<ChatMessageVM>();

            foreach (var cmo in listCmo)
            {
                cms.Add(new ChatMessageVM()
                {
                    CmId = cmo.CmuId,
                    CmFromId = cmo.CmuPushUserId,
                    CmFromDevice = cmo.CmuPushUserDevice,
                    CmFromSign = cmo.CmuPushUserSign,
                    CmToIds = new List<string> { cmo.CmuPullUserId },
                    CmContent = cmo.CmuContent,
                    CmWhich = cmo.CmuPushWhich,
                    CmType = cmo.CmuPushType,
                    CmTime = cmo.CmuCreateTime
                });
            }

            return cms;
        }

        /// <summary>
        /// 推送消息转存储消息
        /// </summary>
        /// <param name="cm"></param>
        public static List<Domain.NChatMessageToGroup> WriteMessageForGroup(ChatMessageVM cm)
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
                    CmgPushUserDevice = cm.CmFromDevice,
                    CmgPushUserSign = cm.CmFromSign
                });
            });

            return listCm;
        }

        /// <summary>
        /// 推送消息转存储消息（反转）
        /// </summary>
        /// <param name="listCmo"></param>
        public static List<ChatMessageVM> WriteMessageForGroupReverse(List<Domain.NChatMessageToGroup> listCmo)
        {
            var cms = new List<ChatMessageVM>();

            foreach (var cmo in listCmo)
            {
                cms.Add(new ChatMessageVM()
                {
                    CmId = cmo.CmgId,
                    CmFromId = cmo.CmgPushUserId,
                    CmFromDevice = cmo.CmgPushUserDevice,
                    CmFromSign = cmo.CmgPushUserSign,
                    CmToIds = new List<string> { cmo.CmgPullGroupId },
                    CmContent = cmo.CmgContent,
                    CmWhich = cmo.CmgPushWhich,
                    CmType = cmo.CmgPushType,
                    CmTime = cmo.CmgCreateTime
                });
            }

            return cms;
        }

        /// <summary>
        /// 根据用户ID找到用户信息
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="onlyOnline">仅在线，默认 在线和离线</param>
        /// <returns></returns>
        public static List<ChatUserConnVM> FindUsers(List<string> UserId, bool onlyOnline = false)
        {
            var users = new List<ChatUserConnVM>();

            var offid = new List<string>();

            UserId.ForEach(id =>
            {
                if (OnlineUser1.ContainsKey(id))
                {
                    users.Add(OnlineUser1[id]);
                }
                else if (OnlineUser9.ContainsKey(id))
                {
                    users.Add(OnlineUser9[id]);
                }
                else
                {
                    offid.Add(id);
                }
            });

            //离线用户
            if (!onlyOnline && offid.Count > 0)
            {
                using var db = Data.ContextBaseFactory.CreateDbContext();

                var offu = db.NChatUser.Where(x => offid.Contains(x.CuUserId)).Select(x => new ChatUserConnVM()
                {
                    UserId = x.CuUserId,
                    UserName = x.CuUserName,
                    UserPhoto = x.CuUserPhoto
                }).ToList();

                users.AddRange(offu);
            }

            return users;
        }

        #endregion
    }
}