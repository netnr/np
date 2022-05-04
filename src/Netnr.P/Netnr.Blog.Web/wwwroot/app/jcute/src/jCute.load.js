/**
 * 载入js脚本，并回调
 * @param {string} src js脚本路径
 * @param {function} success 载入成功回调方法
 */
jCute.getScript = function (src, success) {
    var ele = document.createElement("SCRIPT");
    ele.src = src;
    ele.type = "text/javascript";
    document.getElementsByTagName("HEAD")[0].appendChild(ele);
    //加载完成回调
    if (success != undefined) {
        ele.onload = ele.onreadystatechange = function () {
            if (!this.readyState || this.readyState == "loaded" || this.readyState == "complete") { success(); }
        }        
    }
};

/**
 * 载入css样式
 * @param {string} href css样式路径
 */
jCute.getStyle = function (href) {
    var ele = document.createElement("LINK");
    ele.href = href;
    ele.rel = "stylesheet";
    document.getElementsByTagName("HEAD")[0].appendChild(ele);
};

