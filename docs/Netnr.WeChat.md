# Netnr.WeChat
WeChat SDK, a lightweight, extremely simple WeChat public platform SDK

### Install from NuGet
```
Install-Package Netnr.WeChat
```
### Description
Fork: <https://github.com/night-king/weixinSDK>

### Adjustment
- Without using `Dynamic` object, all methods return `JSON` string of type `string`
- Request changed from `HttpClient` to `HttpWebRequest`
- Take the value, introduce the analysis of `Newtonsoft.Json`, the extension method is `ToJson()`
- The class structure is exactly the same as the WeChat API address, for example:
     - Method corresponding to `/merchant/create` interface `Merchant.Create()`
     - Method corresponding to `/merchant/category/getsub` interface `Merchant.Category.GetSub()`
     - Method corresponding to `/cgi-bin/message/template/send` interface `Cgi_Bin.Message.Template.Send()`
     - Separate according to the interface`/`, and finally the method name, the front is the class and subclass, when using it, you can know how to call the method by looking at the WeChat API` address