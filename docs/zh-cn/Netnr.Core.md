# Netnr.Core
公共类库

### 安装（NuGet）
```
Install-Package Netnr.Core
```

### [更新日志](Netnr.Core.ChangeLog.md)

### 类
- CacheTo.cs　　缓存
- CalcTo.cs　　算法、加密、解密（MD5、DES、SHA1、HMAC_SHA1）
- CmdTo.cs　　执行命令，支持Windows、Linux
- ConsoleTo.cs　　输出日志、错误信息
- Extend.cs　　常用方法拓展（依赖 `Newtonsoft.Json`，JSON、实体、编码、SQL等转换）
- FileTo.cs　　读写文件
- HttpTo.cs　　HTTP请求（GET、POST等，可设置 `HttpWebRequest` 对象）
- LamdaTo.cs　　动态生成 Lamda 表达式
- ParsingTo.cs　　解析（正则相关）
- PathTo.cs　　路径
- QueryableTo.cs　　查询支持处理
- RandomTo.cs　　生成随机码（验证码）
- RsaTo.cs　　RSA加密解密及RSA签名和验证
- TreeTo.cs　　Tree常用方法（List数据集生成JSON tree，菜单多级导航）
- UniqueTo.cs　　生成唯一的标识（GUID转成long）