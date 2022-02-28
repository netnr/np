# upstream
JavaScript请求服务负载均衡，仿nginx upstream

### 说明
- 根据配置的源发起`fetch` 返回`ok=true`表示可用
- 源须支持跨域请求
-  **3** 秒超时， **30** 秒请求缓存，缓存全局对象`upstreamCache`

### 作用
- 无缝切换可用的源
- 优先选择最快的源

### 使用
- <https://www.netnr.com/run/code/4890439129232505252>

```
var hosts = ["https://api.github.com", "https://www.staticfile.org"];

//所有源请求完或默认超时
upstream(hosts, function (fast, ok, isCache) {
    //fast:最快的源
    //ok:可用的源 数组
    console.log(fast, ok, isCache)
});

//最快的一个源或默认超时
upstream(hosts, function (fast, ok) {
    console.log(fast, ok)
}, 1);

//所有源请求完或自定义超时
upstream(hosts, function (fast, ok) {
    console.log(fast, ok)
}, 280);


//主机头跳转或404等，须请求具体路径的情况
upstream([
    "https://cdnjs.cloudflare.com/ajax/libs/jquery/1.12.4/jquery.min.js",
    "https://cdn.staticfile.org/jquery/1.12.4/jquery.min.js"
], function (fast, ok) {
    console.log(fast, ok);
    //根据ok变量提取可用的源
});
```