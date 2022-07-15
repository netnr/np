# Netnr.WeChat
微信 SDK，一个轻量级的，极致简约的微信公众平台 SDK

### 安装 (NuGet)
```
Install-Package Netnr.WeChat
```
### 说明
Fork：<https://github.com/night-king/weixinSDK>

### 调整
- 不使用`Dynamic`对象，所有方法返回`string`类型的`JSON`字符串
- 请求从`HttpClient`改为`HttpWebRequest`
- 取值，引入`Newtonsoft.Json`解析，拓展方法为`ToJson()`
- 类结构与微信`API`地址完全一致，举例说明：
    - `/merchant/create ` 接口对应的方法 `Merchant.Create()`
    - `/merchant/category/getsub` 接口对应的方法 `Merchant.Category.GetSub()`
    - `/cgi-bin/message/template/send` 接口对应的方法 `Cgi_Bin.Message.Template.Send()`
    - 根据接口`/`分割，最后是方法名，前面的是类、子类，在使用时，看微信`API`地址就知道怎么调用方法