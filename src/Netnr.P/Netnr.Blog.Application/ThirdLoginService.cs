using Netnr.Login;

namespace Netnr.Blog.Application
{
    /// <summary>
    /// 第三方登录
    /// </summary>
    public class ThirdLoginService
    {
        /// <summary>
        /// 三方登录
        /// </summary>
        public class ThirdLoginVM
        {
            /// <summary>
            /// 路径名，标识
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// 显示名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 图标
            /// </summary>
            public string Icon
            {
                get
                {
                    return CommonService.StaticResourceLink("LoginPath", $"{Key}.svg");
                }
            }

            /// <summary>
            /// 是否绑定
            /// </summary>
            public bool Bind { get; set; }
        }

        /// <summary>
        /// 登录链接
        /// </summary>
        /// <param name="loginType">登录类型</param>
        /// <param name="authType">登录防伪追加标识，区分登录、注册</param>
        /// <returns></returns>
        public static string LoginLink(string loginType, string authType = "")
        {
            string url = string.Empty;

            if (Enum.TryParse(loginType, true, out LoginBase.LoginType vtype))
            {
                switch (vtype)
                {
                    case LoginBase.LoginType.QQ:
                        {
                            var reqe = new QQ_Authorization_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = QQ.AuthorizationHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.WeiBo:
                        {
                            var reqe = new Weibo_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = Weibo.AuthorizeHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.WeChat:
                        {
                            var reqe = new WeChat_Authorization_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = WeChat.AuthorizationHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.GitHub:
                        {
                            var reqe = new GitHub_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = GitHub.AuthorizeHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.Gitee:
                        {
                            var reqe = new Gitee_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = Gitee.AuthorizeHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.TaoBao:
                        {
                            var reqe = new TaoBao_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = TaoBao.AuthorizeHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.MicroSoft:
                        {
                            var reqe = new MicroSoft_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = MicroSoft.AuthorizeHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.DingTalk:
                        {
                            var reqe = new DingTalk_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = DingTalk.AuthorizeHref_ScanCode(reqe);
                        }
                        break;
                    case LoginBase.LoginType.Google:
                        {
                            var reqe = new Google_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = Google.AuthorizeHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.AliPay:
                        {
                            var reqe = new AliPay_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = AliPay.AuthorizeHref(reqe);
                        }
                        break;
                    case LoginBase.LoginType.StackOverflow:
                        {
                            var reqe = new StackOverflow_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            url = StackOverflow.AuthorizeHref(reqe);
                        }
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                url = "/account/login";
            }

            return url;
        }

        /// <summary>
        /// 获取快捷登录项
        /// </summary>
        /// <param name="umo">用户绑定状态</param>
        /// <returns></returns>
        public static List<ThirdLoginVM> GetQuickLogin(Domain.UserInfo umo = null)
        {
            if (umo == null)
            {
                umo = new Domain.UserInfo();
            }

            return new List<ThirdLoginVM>
            {
                new ThirdLoginVM
                {
                    Key = "qq",
                    Name = "QQ",
                    Bind = !string.IsNullOrWhiteSpace(umo.OpenId1)
                },
                new ThirdLoginVM
                {
                    Key = "weibo",
                    Name = "微博",
                    Bind = !string.IsNullOrWhiteSpace(umo.OpenId2)
                },
                new ThirdLoginVM
                {
                    Key = "github",
                    Name = "GitHub",
                    Bind = !string.IsNullOrWhiteSpace(umo.OpenId3)
                },
                new ThirdLoginVM
                {
                    Key = "taobao",
                    Name = "淘宝",
                    Bind = !string.IsNullOrWhiteSpace(umo.OpenId4)
                },
                new ThirdLoginVM
                {
                    Key = "microsoft",
                    Name = "Microsoft",
                    Bind = !string.IsNullOrWhiteSpace(umo.OpenId5)
                },
                new ThirdLoginVM
                {
                    Key = "dingtalk",
                    Name = "钉钉",
                    Bind = !string.IsNullOrWhiteSpace(umo.OpenId6)
                }
            };
        }
    }
}
