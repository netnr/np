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
        <td><img src="https://s1.netnr.eu.org/static/login/qq.svg" height="30" title="QQ"></td>
        <td><a target="_blank" href="https://wiki.connect.qq.com/准备工作_oauth2-0">documents</a></td>
        <td><a target="_blank" href="https://connect.qq.com/manage.html">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/wechat.svg" height="30" title="WeChat"></td>
        <td><a target="_blank" href="https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419316505&token=&lang=zh_CN">documents</a></td>
        <td><a target="_blank" href="https://open.weixin.qq.com">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/weibo.svg" height="30" title="Weibo"></td>
        <td><a target="_blank" href="https://open.weibo.com/wiki/授权机制说明">documents</a></td>
        <td><a target="_blank" href="https://open.weibo.com/apps">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/github.svg" height="30" title="GitHub"></td>
        <td><a target="_blank" href="https://docs.github.com/en/free-pro-team@latest/developers/apps/authorizing-oauth-apps">documents</a></td>
        <td><a target="_blank" href="https://github.com/settings/developers">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/gitee.svg" height="30" title="Gitee"></td>
        <td><a target="_blank" href="https://gitee.com/api/v5/oauth_doc">documents</a></td>
        <td><a target="_blank" href="https://gitee.com/oauth/applications">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/taobao.svg" height="30" title="Taobao/Tmail"></td>
        <td><a target="_blank" href="https://open.taobao.com/doc.htm?spm=a219a.7386797.0.0.4e00669acnkQy6&source=search&docId=105590&docType=1">documents</a></td>
        <td><a target="_blank" href="https://console.open.taobao.com/">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/microsoft.svg" height="30" title="Microsoft"></td>
        <td><a target="_blank" href="https://docs.microsoft.com/zh-cn/graph/auth/">documents</a></td>
        <td><a target="_blank" href="https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/dingtalk.svg" height="30" title="DingTalk"></td>
        <td><a target="_blank" href="https://ding-doc.dingtalk.com/doc#/serverapi2/kymkv6">documents</a></td>
        <td><a target="_blank" href="https://open-dev.dingtalk.com/#/loginMan">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/google.svg" height="30" title="谷歌/Google"></td>
        <td><a target="_blank" href="https://developers.google.com/identity/protocols/OpenIDConnect">documents</a></td>
        <td><a target="_blank" href="https://console.developers.google.com/apis/credentials">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/alipay.svg" height="30" title="AliPay"></td>
        <td><a target="_blank" href="https://docs.open.alipay.com/263/105809">documents</a></td>
        <td><a target="_blank" href="https://openhome.alipay.com/platform/developerIndex.htm">application</a></td>
    </tr>
    <tr>
        <td><img src="https://s1.netnr.eu.org/static/login/stackoverflow.svg" height="30" title="Stack Overflow"></td>
        <td><a target="_blank" href="https://api.stackexchange.com">documents</a></td>
        <td><a target="_blank" href="https://stackapps.com/apps/oauth/register">application</a></td>
    </tr>
</table>


> Reminder: Generally, all third-party logins have a **state** parameter, which is used to prevent CSRF attacks (anti-counterfeiting). You can use this parameter to add the prefix of login and registration.

### Usage
reference: `Netnr.Test/Controllers/LoginController.cs`