# Netnr.UAParser
提取浏览器名称、浏览器版本号、系统名称、系统版本号、是否为爬虫

### 安装 (NuGet)
```
Install-Package Netnr.UAParser
```

### 使用 (Usage)
```
var uap = new UAParser.Parsers(userAgent);

var clientEntity = uap.GetClient();
var deviceEntity = uap.GetDevice();
var osEntity = uap.GetOS();
var botEntity = uap.GetBot();
```  

### 附
正则：<https://github.com/matomo-org/device-detector>  
对比：<https://github.com/totpero/DeviceDetector.NET>  
去除详细型号检测，包精简，轻依赖，预加载正则，相对缓存热数据  
不命中缓存时，一个耗时 300ms 左右，而 DeviceDetector.NET 需要 3s 左右