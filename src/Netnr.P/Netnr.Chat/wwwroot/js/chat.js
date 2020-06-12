/*          
 *  Netnr.Chat
 *  2020-04-30
 */

var nc = {
    //配置
    config: {
        //连接地址
        host: location.origin,
        //接收消息事件名称
        receiveMessage: "ReceiveMessage",
        //token缓存key
        lsKeyToken: "nc_token"
    },

    //缓存
    dc: {
        //连接对象
        conn: null,
        //授权信息
        tokenObj: null
    },

    init: function () {

        //读取token缓存
        nc.dc.tokenObj = nc.ls(nc.config.lsKeyToken);

        //构建
        nc.dc.conn = new signalR.HubConnectionBuilder().withUrl(nc.config.host + "/chathub", {
            accessTokenFactory: () => { return nc.token() }
        }).build();

        //开始
        nc.dc.conn.start().then(function () {
            //连接成功
            nc.eventConnSuccess();

            //接收消息
            nc.dc.conn.on(nc.config.receiveMessage, nc.eventReceiveMessage);
        }).catch(function (err) {
            console.log(err);
            alert("连接服务器失败");
        });

    },

    /**
     * 读取或设置缓存
     * @param {any} key 缓存键
     * @param {any} value 缓存值
     */
    ls: function (key, value) {
        if (arguments.length == 2) {
            localStorage.setItem(key, JSON.stringify(value));
        } else {
            var lsv = localStorage.getItem(key), lsvo;
            lsv != null && lsv != "" && (lsvo = JSON.parse(lsv));
            return lsvo;
        }
    },

    /**
     * 获取授权 Token
     */
    token: function () {
        return new Promise(function (resolve, reject) {

            //未记录或失效
            if (nc.dc.tokenObj == null || (new Date().valueOf() - nc.dc.tokenObj.startDate) / 1000 > nc.dc.tokenObj.expireDate) {

                var un = prompt("请输入账号", new Date().valueOf());
                var pd = prompt("请输入密码", new Date().valueOf());

                fetch(nc.config.host + "/account/token", {
                    method: "POST",
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        UserName: un,
                        Password: pd,
                        Device: "1",
                        Sign: "1.0.0"
                    })
                }).then(x => x.json()).then(res => {
                    if (res.code == 200) {
                        res.data.startDate = new Date().valueOf();
                        nc.dc.tokenObj = res.data;

                        nc.ls(nc.config.lsKeyToken, nc.dc.tokenObj)
                        resolve(nc.dc.tokenObj.token);
                    } else {
                        alert(res.msg);
                        reject(res.msg);
                    }
                })
            } else {
                resolve(nc.dc.tokenObj.token);
            }
        });
    },

    /**
     * 推送消息到用户
     * @param {any} cm 消息对象
     */
    pushMessageToUsers: function (cm) {
        cm.CmFromId = nc.dc.tokenObj.userId;

        return fetch(nc.config.host + "/Chat/PushMessageToUsers", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                Authorization: "Bearer " + nc.dc.tokenObj.token
            },
            body: JSON.stringify(cm)
        }).then(x => x.json());

        //return nc.dc.conn.invoke("PushMessageToUsers", cm);
    },

    /**
     * 推送消息到组
     * @param {any} cm 消息对象
     */
    pushMessageToGroups: function (cm) {
        cm.CmFromId = nc.dc.tokenObj.userId;

        return fetch(nc.config.host + "/Chat/PushMessageToGroups", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                Authorization: "Bearer " + nc.dc.tokenObj.token
            },
            body: JSON.stringify(cm)
        }).then(x => x.json());

        //return nc.dc.conn.invoke("PushMessageToGroups", cm);
    },

    /**
     * 事件：连接成功
     */
    eventConnSuccess: function () {
        document.getElementById("userId").innerHTML = "userId：" + nc.dc.tokenObj.userId;
        document.getElementById("userName").innerHTML = "userName：" + nc.dc.tokenObj.userName;

        document.getElementById("sendButton").disabled = false;
        document.getElementById("btnLogout").disabled = false;
    },

    /**
     * 事件：接收消息
     * @param {any} cm 消息对象
     */
    eventReceiveMessage: function (cm) {
        console.log(cm);

        var nn = document.createElement('li');
        nn.innerHTML = '<pre>' + JSON.stringify(cm, null, 4) + '</pre>';
        document.getElementById("messagesList").appendChild(nn);
    },

};


document.getElementById("sendButton").disabled = true;
document.getElementById("btnLogout").disabled = true;

nc.init();

document.getElementById("sendButton").addEventListener("click", function () {
    var toUser = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    nc.pushMessageToUsers({
        CmContent: message,
        CmType: "Text",
        CmToIds: [toUser]
    }).then(res => {
        if (res.code != 200) {
            alert(res.msg);
            console.log(res);
        }
    }).catch(function (err) {
        return console.error(err.toString());
    });
});

document.getElementById("btnLogout").addEventListener("click", function () {
    if (nc.dc.conn) {
        nc.dc.conn.stop();
        localStorage.removeItem(nc.config.lsKeyToken);
        alert('已注销')
        location.reload();
    }
});