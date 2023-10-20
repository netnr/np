# Netnr.Core
公共类库

### 安装（NuGet）
```
Install-Package Netnr.Core
```

### 类
类 | 描述
--- | ---
CacheTo.cs | 缓存
CalcTo.cs | 加密、解密（AES、DES、RSA、MD5、SHA1、HMAC_SHA1）
CmdTo.cs | 执行命令，支持 Windows、Linux
ConsoleTo.cs | 输出日志、错误信息
Extensions.cs | 常用方法拓展
FileTo.cs | 读写文件
HttpTo.cs | HTTP 请求（GET、POST 等，可设置 `HttpWebRequest` 对象）
LockTo.cs | 锁
MailTo.cs | 邮件
MonitorTo.cs | 监测
ParsingTo.cs | 解析（正则、拼接）
PredicateTo.cs | 谓词构建（And、Or 等）
RandomTo.cs | 生成随机码（验证码）
Snowflake53To.cs | 雪花 ID （兼容 JS）
SnowflakeTo.cs | 雪花 ID
SystemStatusTo.cs | 系统状态信息
TextMiningTo.cs | 文本挖掘
UniqueTo.cs | 生成唯一的标识（GUID 转成 long）
ZipTo.cs | ZIP 压缩、解压缩