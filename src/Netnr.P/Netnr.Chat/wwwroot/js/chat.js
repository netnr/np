/*          
 *  Netnr.Chat
 *  2020-04-30
 */

var nc = {
    //连接服务
    hubHost: location.origin,
    //接收消息
    receiveMessage: "ReceiveMessage",

    //连接对象
    connObj: null,
    //授权Token
    tokenObj: null,

    //用户设备
    fromDevice: navigator.userAgent,
    //用户标记
    fromSign: "1.0.0",

    /** 初始化 */
    init: function () {
        try {
            nc.tokenObj = nc.ls("token");
        } catch (e) {
            console.log("读取缓存错误");
        }
    },

    /**
     * 构建请求链接
     * @param {any} path
     * @param {any} notToken
     */
    buildUrl: function (path, notToken) {
        var token = nc.tokenObj?.AccessToken;
        var url = nc.hubHost + path;
        if (token != null && !notToken) {
            url += "?access_token=" + encodeURIComponent(token);
        }
        return url;
    },

    /** 构建连接 */
    buildConn: function () {
        //构建
        nc.connObj = new signalR.HubConnectionBuilder().withUrl(nc.buildUrl("/chathub", true), {
            accessTokenFactory: () => {
                //带上授权 Token
                return nc.tokenObj?.AccessToken
            }
        }).build();

        //开始
        nc.connObj.start().then(function () {
            //连接成功
            nc.eventConnSuccess();

            //接收消息
            nc.connObj.on(nc.receiveMessage, nc.eventReceiveMessage);
        }).catch(function (err) {
            console.log("连接失败", err);
        });
    },

    /**
     * 用户是否授权
     */
    isAuth: function () {
        fetch(nc.buildUrl("/Account/UserAuthInfo")).then(x => x.json()).then(res => {
            if (res.code == 200) {
                console.log("已授权，初始化连接", res);
                nc.buildConn();
            } else {
                console.log("未授权，先授权，再构建连接", res);
                nc.buildToken().then(res => {
                    nc.buildConn();
                })
            }
        })
    },

    /**
     * 构建授权 Token
     */
    buildToken: function () {
        return new Promise(function (resolve, reject) {

            var un = prompt("请输入账号", new Date().valueOf());
            var pd = prompt("请输入密码", new Date().valueOf());

            fetch(nc.buildUrl("/account/token", true), {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    UserName: un,
                    Password: pd,
                    Device: nc.fromDevice,
                    Sign: nc.fromSign
                })
            }).then(x => x.json()).then(res => {
                if (res.code == 200) {
                    console.log("授权成功", res);
                    nc.ls("token", nc.tokenObj = res.data);
                    resolve(res);
                } else {
                    console.log("授权失败", res);
                    reject(res);
                }
            })
        });
    },

    /**
     * 推送消息到用户（好友才能推送）
     * @param {any} CmContent 发送内容
     * @param {any} CmType 消息类型 MessageType 枚举
     * @param {any} CmToIds 接收用户ID（一个或数组）
     */
    pushMessageToUsers: function (CmContent, CmType, CmToIds) {
        var fd = new FormData();
        fd.append("CmFromId", nc.tokenObj.UserId);
        fd.append("CmFromDevice", nc.fromDevice);
        fd.append("CmFromSign", nc.fromSign);

        fd.append("CmContent", CmContent);
        fd.append("CmType", CmType);

        if (typeof CmToIds != "object") {
            CmToIds = [CmToIds];
        }
        CmToIds.forEach(id => {
            fd.append("CmToIds", id);
        })

        return fetch(nc.buildUrl("/Chat/PushMessageToUsers"), {
            method: "POST",
            body: fd
        }).then(x => x.json());
    },

    /**
     * 推送消息到组
     * @param {any} CmContent 发送内容
     * @param {any} CmType 消息类型 MessageType 枚举
     * @param {any} CmToIds 接收群组ID（一个或数组）
     */
    pushMessageToGroups: function (CmContent, CmType, CmToIds) {
        var fd = new FormData();
        fd.append("CmFromId", nc.tokenObj.UserId);
        fd.append("CmFromDevice", nc.fromDevice);
        fd.append("CmFromSign", nc.fromSign);

        fd.append("CmContent", CmContent);
        fd.append("CmType", CmType);

        if (typeof CmToIds != "object") {
            CmToIds = [CmToIds];
        }
        CmToIds.forEach(id => {
            fd.append("CmToIds", id);
        })

        return fetch(nc.buildUrl("/Chat/PushMessageToGroups"), {
            method: "POST",
            body: fd
        }).then(x => x.json());
    },

    /**
     * 事件：连接成功
     */
    eventConnSuccess: function () {
        console.log('连接成功');
        document.querySelector('.nr-user').value = JSON.stringify(nc.tokenObj, null, 2) + "\n\n" + JSON.stringify(nc.connObj, null, 2);
    },

    /**
     * 事件：接收消息
     * @param {any} cm 消息对象
     */
    eventReceiveMessage: function (cm) {
        console.log("接收消息", cm);
        document.querySelector('.nr-message-list').value += JSON.stringify(cm, null, 2) + "\n\n";
    },

    /**
     * 读取或设置缓存
     * @param {any} key 缓存键
     * @param {any} value 缓存值
     */
    ls: function (key, value) {
        if (arguments.length == 2) {
            localStorage.setItem(key, typeof value == "object" ? JSON.stringify(value) : value);
        } else {
            var lsv = localStorage.getItem(key), lsvo;
            try {
                lsvo = JSON.parse(lsv)
            } catch (e) { lsvo = lsv; }
            return lsvo;
        }
    }

};

nc.init();
nc.isAuth();

//发送消息
document.querySelector('.nr-btn-send').addEventListener("click", function () {
    var userid = document.querySelector('.nr-send-userid').value;
    var message = document.querySelector('.nr-send-message').value;

    nc.pushMessageToUsers(message, "text", userid).then(res => {
        if (res.code != 200) {
            console.log(res);
        }
    }).catch(function (err) {
        return console.error(err.toString());
    });
});

//注销
document.getElementById("btnLogout").addEventListener("click", function () {
    if (nc.connObj) {
        nc.connObj.stop();
        localStorage.clear();
        console.log('已注销')
    }
});