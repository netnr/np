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
Extend.cs | 常用方法拓展（依赖 `Newtonsoft.Json`，JSON、实体、编码、SQL 等转换）
FileTo.cs | 读写文件
HttpTo.cs | HTTP 请求（GET、POST 等，可设置 `HttpWebRequest` 对象）
ParsingTo.cs | 解析（正则相关）
PathTo.cs | 路径
RandomTo.cs | 生成随机码（验证码）
SnowflakeTo.cs | 雪花 ID
SystemStatusTo.cs | 系统状态信息
TreeTo.cs | Tree 常用方法（List 数据集生成 JSON tree，菜单多级导航）
UniqueTo.cs | 生成唯一的标识（GUID 转成 long）
ZipTo.cs | ZIP 压缩、解压缩