# Netnr.UAParser
提取浏览器名称、浏览器版本号、系统名称、系统版本号、是否为爬虫

### 安装 (NuGet)
```
Install-Package Netnr.UAParser
```

### 使用 (Usage)
```csharp
//（首次预编译耗时约 5 秒）

var uap = new UAParsers(userAgent);

var clientModel = uap.GetClient();
var deviceModel = uap.GetDevice();
var osModel = uap.GetOS();
var botModel = uap.GetBot();
```

### 附
正则：<https://github.com/matomo-org/device-detector>  
去除详细型号检测，包精简，轻依赖，预编译正则，速度快。