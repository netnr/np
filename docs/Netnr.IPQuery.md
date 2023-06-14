# Netnr.IPQuery
查询 IP 地址，支持 IPv4 和 IPv6 ，主要国内省市查询，国外仅有国家名称

### 使用 (Usage)
```csharp
// 注册 GBK 编码
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

//实例，指定 IPv4 和 IPv6 文件数据库路径
var ipq = new IPQuery(@"D:\tmp\qqwry.dat", @"D:\tmp\ipv6wry.db");

var result1 = ipq.Search("61.186.154.83");
Console.WriteLine($"{result1.Addr} {result1.ISP}");

var result2 = ipq.Search("fec0:0:2:1::1");
Console.WriteLine($"{result2.Addr} {result2.ISP}");
```

### 来源
IPv4 来源 https://cz88.net (从公众号下载 exe 程序，安装后拷贝 qqwry.dat 文件)  
IPv6 来源 https://ip.zxinc.org (下载 https://ip.zxinc.org/ip.7z 解压拷贝 ipv6wry.db 文件)

### 更多
来源参考 https://github.com/zu1k/nali  
在线接口 https://ip.zxinc.org/api.php?ip=116.179.37.70&type=json