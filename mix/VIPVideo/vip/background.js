chrome.contextMenus.create({
    title: "VIP视频解析",
    contexts: ["page"],
    documentUrlPatterns: ["*://*.mgtv.com/*", "*://*.letv.com/*", "*://*.youku.com/*", "*://*.iqiyi.com/*", "*://*.v.qq.com/*", "*://*.tudou.com/*", "*://*.wasu.cn/*", "*://*.ku6.com/*", "*://*.56.com/*", "*://*.tv.sohu.com/*", "*://*.film.sohu.com/*", "*://*.1905.com/*", "*://*.pptv.com/*", "*://*.baofeng.com/*", "*://*.bilibili.com/*", "*://*.fun.tv/*", "*://*.6.cn/*"],
    targetUrlPatterns: ["http://*/*"],
    onclick: function (b) {
        burl = "https://v.netnr.eu.org/?url=" + encodeURIComponent(b.pageUrl);
        chrome.tabs.query({
            active: true,
            currentWindow: true
        }, function (c) {
            chrome.tabs.update(c[0].id, {
                url: burl + "&title=" + encodeURI(c[0].title)
            })
        })
    }
});
chrome.browserAction.onClicked.addListener(function (b) {
    chrome.tabs.create({
        url: 'https://v.netnr.eu.org'
    })
});