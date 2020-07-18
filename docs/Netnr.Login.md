# Netnr.Login
Third-party OAuth authorized login, QQ, WeChat, Weibo, GitHub, Gitee, Taobao (Tmall), Microsoft, DingTalk, Google, Alipay, StackOverflow

> Demo：<https://www.netnr.com/account/login>

### Install from NuGet
```
Install-Package Netnr.Login
```

### [CHANGELOG](Netnr.Login.ChangeLog.md)

### third-party login
<table>
    <tr><th>Tripartite</th><th>documents</th></tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/qq.svg" height="30" title="QQ"></td>
        <td><a target="_blank" href="https://wiki.connect.qq.com/准备工作_oauth2-0">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/wechat.svg" height="30" title="WeChat"></td>
        <td><a target="_blank" href="https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419316505&token=&lang=zh_CN">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/weibo.svg" height="30" title="Weibo"></td>
        <td><a target="_blank" href="https://open.weibo.com/wiki/授权机制说明">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/github.svg" height="30" title="GitHub"></td>
        <td><a target="_blank" href="https://developer.github.com/apps/building-oauth-apps/authorizing-oauth-apps">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/gitee.svg" height="30" title="Gitee"></td>
        <td><a target="_blank" href="https://gitee.com/api/v5/oauth_doc">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/taobao.svg" height="30" title="Taobao/Tmail"></td>
        <td><a target="_blank" href="https://open.taobao.com/doc.htm?spm=a219a.7386797.0.0.4e00669acnkQy6&source=search&docId=105590&docType=1">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/microsoft.svg" height="30" title="Microsoft"></td>
        <td><a target="_blank" href="https://docs.microsoft.com/zh-cn/graph/auth/">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/dingtalk.svg" height="30" title="DingTalk"></td>
        <td><a target="_blank" href="https://ding-doc.dingtalk.com/doc#/serverapi2/kymkv6">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/google.svg" height="30" title="谷歌/Google"></td>
        <td><a target="_blank" href="https://developers.google.com/identity/protocols/OpenIDConnect">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/alipay.svg" height="30" title="AliPay"></td>
        <td><a target="_blank" href="https://docs.open.alipay.com/263/105809">documents</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.zme.ink/static/login/stackoverflow.svg" height="30" title="Stack Overflow"></td>
        <td><a target="_blank" href="https://api.stackexchange.com">documents</a></td>
    </tr>
</table>


> Reminder: Generally, all third-party logins have a **state** parameter, which is used to prevent CSRF attacks (anti-counterfeiting). You can use this parameter to add the prefix of login and registration.

### Usage
```csharp
/*
 * This is the test code, just to call each interface and get the unique identification
 * In actual applications, nicknames, mailboxes, avatars, etc. must also be processed
 */

using System;

namespace Netnr.Login.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var lc = new LoginClient(LoginBase.LoginType.StackOverflow);

            //Copy the authorization link and open it in the browser, get the code after authorization, and assign it manually. Manual assignment needs to be decoded
            var url = lc.Auth();

            var ar = new LoginBase.AuthorizeResult();
            ar.code = "";
            //Break here, assign the code obtained above and continue
            ar.code = ar.code.ToDecode();

            lc.AuthCallback(ar);
        }

        public class LoginClient
        {
            private LoginBase.LoginType? loginType;

            public LoginClient(LoginBase.LoginType _loginType)
            {
                loginType = _loginType;

                // Configuration
                QQConfig.APPID = "XXX";
                QQConfig.APPKey = "XXX";
                //Callback address, consistent with the address filled in in the application
                QQConfig.Redirect_Uri = "https://rf2.netnr.com/account/authcallback/qq";

                WeChatConfig.AppId = "";
                WeChatConfig.AppSecret = "";
                WeChatConfig.Redirect_Uri = "";

                WeiboConfig.AppKey = "";
                WeiboConfig.AppSecret = "";
                WeiboConfig.Redirect_Uri = "";

                GitHubConfig.ClientID = "";
                GitHubConfig.ClientSecret = "";
                GitHubConfig.Redirect_Uri = "";
                //The application name of the application is very important
                GitHubConfig.ApplicationName = "netnrf";

                TaoBaoConfig.AppKey = "";
                TaoBaoConfig.AppSecret = "";
                TaoBaoConfig.Redirect_Uri = "";

                MicroSoftConfig.ClientID = "";
                MicroSoftConfig.ClientSecret = "";
                MicroSoftConfig.Redirect_Uri = "";

                DingTalkConfig.appId = "";
                DingTalkConfig.appSecret = "";
                DingTalkConfig.Redirect_Uri = "";

                GiteeConfig.ClientID = "";
                GiteeConfig.ClientSecret = "";
                GiteeConfig.Redirect_Uri = "";

                GoogleConfig.ClientID = "";
                GoogleConfig.ClientSecret = "";
                GoogleConfig.Redirect_Uri = "";

                AliPayConfig.AppId = "";
                AliPayConfig.AppPrivateKey = "";
                AliPayConfig.Redirect_Uri = "";

                StackOverflowConfig.ClientId = "";
                StackOverflowConfig.ClientSecret = "";
                StackOverflowConfig.Key = "";
                StackOverflowConfig.Redirect_Uri = "";
            }

            /// <summary>
            /// Generate request link
            /// </summary>
            /// <param name="authType">Additional information in anti-counterfeiting parameters (can be used to distinguish between login, registration, binding, and unbinding)</param>
            /// <returns></returns>
            public string Auth(string authType = "")
            {
                var url = string.Empty;

                switch (loginType)
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
                    case LoginBase.LoginType.DingTalk:
                        {
                            var reqe = new DingTalk_Authorize_RequestEntity();
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                reqe.state = authType + reqe.state;
                            }
                            //扫描模式
                            url = DingTalk.AuthorizeHref_ScanCode(reqe);

                            //密码模式
                            //url = DingTalk.AuthorizeHref_Password(reqe);
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

                return url;
            }

            /// <summary>
            /// Callback method
            /// </summary>
            /// <param name="authorizeResult">Receive authorization code, anti-counterfeiting parameters</param>
            public void AuthCallback(LoginBase.AuthorizeResult authorizeResult)
            {
                if (string.IsNullOrWhiteSpace(authorizeResult.code))
                {
                    //打开链接没登录授权
                }
                else
                {
                    //Uniquely identifies
                    string OpenId = string.Empty;

                    switch (loginType)
                    {
                        case LoginBase.LoginType.QQ:
                            {
                                //获取 access_token
                                var tokenEntity = QQ.AccessToken(new QQ_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                //获取 OpendId
                                var openidEntity = QQ.OpenId(new QQ_OpenId_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token
                                });

                                //获取 UserInfo
                                _ = QQ.OpenId_Get_User_Info(new QQ_OpenAPI_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token,
                                    openid = openidEntity.openid
                                });

                                //身份唯一标识
                                OpenId = openidEntity.openid;
                            }
                            break;
                        case LoginBase.LoginType.WeiBo:
                            {
                                //获取 access_token
                                var tokenEntity = Weibo.AccessToken(new Weibo_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                //获取 access_token 的授权信息
                                var tokenInfoEntity = Weibo.GetTokenInfo(new Weibo_GetTokenInfo_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token
                                });

                                //获取 users/show
                                _ = Weibo.UserShow(new Weibo_UserShow_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token,
                                    uid = Convert.ToInt64(tokenInfoEntity.uid)
                                });

                                OpenId = tokenEntity.access_token;
                            }
                            break;
                        case LoginBase.LoginType.WeChat:
                            {
                                //获取 access_token
                                var tokenEntity = WeChat.AccessToken(new WeChat_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                //获取 user
                                _ = WeChat.Get_User_Info(new WeChat_OpenAPI_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token,
                                    openid = tokenEntity.openid
                                });

                                //身份唯一标识
                                OpenId = tokenEntity.openid;
                            }
                            break;
                        case LoginBase.LoginType.GitHub:
                            {
                                //获取 access_token
                                var tokenEntity = GitHub.AccessToken(new GitHub_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                //获取 user
                                var userEntity = GitHub.User(new GitHub_User_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token
                                });

                                OpenId = userEntity.id.ToString();
                            }
                            break;
                        case LoginBase.LoginType.TaoBao:
                            {
                                //获取 access_token
                                var tokenEntity = TaoBao.AccessToken(new TaoBao_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                OpenId = tokenEntity.open_uid;
                            }
                            break;
                        case LoginBase.LoginType.MicroSoft:
                            {
                                //获取 access_token
                                var tokenEntity = MicroSoft.AccessToken(new MicroSoft_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                //获取 user
                                var userEntity = MicroSoft.User(new MicroSoft_User_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token
                                });

                                OpenId = userEntity.id.ToString();
                            }
                            break;
                        case LoginBase.LoginType.DingTalk:
                            {
                                //获取 user
                                var userEntity = DingTalk.User(new DingTalk_User_RequestEntity(), authorizeResult.code);

                                OpenId = userEntity?.openid;
                            }
                            break;
                        case LoginBase.LoginType.Gitee:
                            {
                                //获取 access_token
                                var tokenEntity = Gitee.AccessToken(new Gitee_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                //获取 user
                                var userEntity = Gitee.User(new Gitee_User_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token
                                });

                                OpenId = userEntity.id.ToString();
                            }
                            break;
                        case LoginBase.LoginType.Google:
                            {
                                //获取 access_token
                                var tokenEntity = Google.AccessToken(new Google_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                //获取 user
                                var userEntity = Google.User(new Google_User_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token
                                });

                                OpenId = userEntity.sub;
                            }
                            break;
                        case LoginBase.LoginType.AliPay:
                            {
                                //获取 access_token
                                var tokenEntity = AliPay.AccessToken(new AliPay_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.auth_code
                                });

                                //实际上这一步已经获取到 OpenId，登录验证可以了，获取个人信息还需调用下面的接口
                                //tokenEntity.user_id

                                //获取 user
                                var userEntity = AliPay.User(new AliPay_User_RequestEntity()
                                {
                                    auth_token = tokenEntity.access_token
                                });

                                OpenId = userEntity.user_id;
                            }
                            break;
                        case LoginBase.LoginType.StackOverflow:
                            {
                                //获取 access_token
                                var tokenEntity = StackOverflow.AccessToken(new StackOverflow_AccessToken_RequestEntity()
                                {
                                    code = authorizeResult.code
                                });

                                //获取 user
                                var userEntity = StackOverflow.User(new StackOverflow_User_RequestEntity()
                                {
                                    access_token = tokenEntity.access_token
                                });

                                OpenId = userEntity.user_id;
                            }
                            break;
                    }

                    //Get the openid
                    if (string.IsNullOrWhiteSpace(OpenId))
                    {
                        //TO DO
                    }
                }

            }
        }
    }
}
```

### Frame
- .NETStandard 2.1
- .NETFramework 4.0