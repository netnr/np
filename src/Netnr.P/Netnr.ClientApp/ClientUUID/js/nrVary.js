// 方法
let nrVary = {
    flagName: "netnr",
    flagResp: "uuid",
    flagLibs: "libs",
    flagSuffix: ".md",
    flagToken: null,

    flagLocalUsed: false,//本地模式
    flagLocalPath: "/libs/index.json",
    flagLocalJson: null,

    flagProxyServer: 'https://netnr.zme.ink/api/v1/Proxy/?url=', //代理服务
    flagProxyUsed: true, //使用代理

    flagTitle: document.title, //站点名称
}

export { nrVary };