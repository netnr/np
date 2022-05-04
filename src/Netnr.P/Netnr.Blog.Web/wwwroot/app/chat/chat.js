(function (window) {

    if (typeof WebSocket != "function") {
        document.body.innerHTML = "<h1 style='text-align:center;line-height:200px;color:red;'>您的浏览器不支持哦！！！</h1>";
        return false;
    }

    var c = function () {

    }

    c.config = {
        uset: {
            online: false,
            volume: "1",
            lock: "0",
            wide: "0",
            fastsendkey: 'ctrlenter',
            photo: "favicon.ico"
        },
        group: {
            ownerId: 'netnr',
            name: "NetnrGroup",
            topicId: "11464322971074560",
        },
        appAccount: new Date().valueOf().toString().slice(-4),
        token: function () {
            return {
                appId: "2882303761517686414",
                appKey: "5341768622414",
                appSecret: "u5BI0V0QYuodKEabigumzA=="
            }
        }
    };

    c.httpRequest = function (url, data) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, false);
        xhr.setRequestHeader('content-type', 'application/json');
        xhr.send(JSON.stringify(data));

        return JSON.parse(xhr.response);
    }

    c.fetchMIMCToken = function () {
        var token = c.config.token();
        token.appAccount = c.config.appAccount;
        return c.httpRequest('https://mimc.chat.xiaomi.net/api/account/token', token);
    }

    //状态回调
    c.statusChange = function (bindResult, errType, errReason, errDesc) {
        if (bindResult) {
            //online
            sendTypeMsg(c.config.appAccount, -1);
        } else {
            c.writeToScreens("login failed.errReason=" + errReason + ",errDesc=" + errDesc + ",errType=" + errType);
        }
    }

    c.timeFormat = function (date, format) {
        function t(n) { return n < 10 ? "0" + n : n }
        var d = date,
            y = d.getFullYear(),
            m = t(d.getMonth() + 1),
            day = t(d.getDate()),
            h = t(d.getHours()),
            min = t(d.getMinutes()),
            s = t(d.getSeconds()),
            f = d.getMilliseconds();
        return !format ? (y + "-" + m + "-" + day + " " + h + ":" + min + ":" + s) :
            format.replace("yyyy", y).replace("MM", m).replace("dd", day).replace("HH", h).replace("mm", min).replace("ss", s).replace("ff", f)
    };

    //个人消息回调
    c.receiveP2PMsg = function (message) {
        var time = c.timeFormat(new Date(parseInt(message.getTimeStamp())));
        var msg = message.getPayload(), mpobj = $.parseJSON(msg), align = 1;
        if (mpobj.uid == c.config.appAccount) {
            align = 2;
        } else {
            c.audio('pull');
        }
        c.writeToScreens(mpobj.message, c.config.uset.photo, mpobj.nickname, time, align);
    }

    //群消息回调
    c.receiveP2TMsg = function (message) {
        var time = c.timeFormat(new Date(parseInt(message.getTimeStamp())));
        var msg = message.getPayload(), mpobj = $.parseJSON(msg), align = 1;
        switch (mpobj.type) {
            //group vary
            case -2:
                {
                    c.queryGroupInfo(function (data) {
                        c.writeToGroupMember(data);
                    })
                }
                break;
            //login
            case -1:
                {
                    c.audio('online');
                    c.writeToScreens("<a href='javascript:void(0);' onclick='c.toTA(this)'>" + decodeURIComponent(mpobj.message) + "</a> 上线");
                }
                break;
            //msg
            case 1:
                {
                    c.audio('pull');
                    c.writeToScreens(mpobj.message, c.config.uset.photo, message.getFromAccount(), time, align);
                }
                break;
        }
    }

    //发送消息
    c.sendMsg = function (toUser, message) {
        try {
            var packetId = c.user.sendMessage(toUser, message);
        } catch (err) {
            console.log("sendMessage fail, err=" + err);
        }
        c.writeToScreens(c.config.appAccount + " to " + toUser + ":" + message);
    }

    //推送消息
    c.pushMsg = function (toUser, message) {
        var pushData = c.config.token();
        pushData.fromAccount = c.config.appAccount;
        pushData.fromResource = "resWeb";
        pushData.toAccount = toUser;
        pushData.msg = message;
        var result = c.httpRequest('https://mimc.chat.xiaomi.net/api/push/p2p/', pushData);
        //c.writeToScreens(c.config.appAccount + " to " + toUser + ":" + message);
        if (200 !== result.code) {
            c.writeToScreens("result code:" + result.code + ",message=" + result.message);
        }
        var packetId = result.data.packetId;
    }

    c.serverAck = function (packetId, sequence, timeStamp, errMsg) {
        //c.writeToScreens("receive msg ack:" + packetId + ",sequence=" + sequence + ",ts=" + timeStamp);
        return;
    }

    //离线回调
    c.disconnect = function () {
        c.config.uset.online = false;
    }

    //离线
    c.logout = function () {
        c.user.logout();
    }

    //建群
    c.createGroup = function (topicName, groupMember) {
        var token = c.config.token();
        token.appAccount = c.config.appAccount;
        var data = { topicName: topicName, accounts: groupMember };
        var url = 'https://mimc.chat.xiaomi.net/api/topic/' + token.appId;

        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, false);
        xhr.setRequestHeader('content-type', 'application/json');
        xhr.setRequestHeader('appKey', token.appKey);
        xhr.setRequestHeader('appSecret', token.appSecret);
        xhr.setRequestHeader('appAccount', token.appAccount);
        xhr.send(JSON.stringify(data));

        var result = JSON.parse(xhr.response);
        if (200 !== result.code) {
            c.writeToScreens("create group failed,msg=" + result.message);
        }
    }

    //用户所属群信息
    c.queryBelongGroupInfo = function (fn) {
        var token = c.config.token();
        token.appAccount = c.config.appAccount;
        var url = 'https://mimc.chat.xiaomi.net/api/topic/' + token.appId + '/account';
        var xhr = new XMLHttpRequest();
        xhr.open('GET', url, false);
        xhr.setRequestHeader('content-type', 'application/json');
        xhr.setRequestHeader('appKey', token.appKey);
        xhr.setRequestHeader('appSecret', token.appSecret);
        xhr.setRequestHeader('appAccount', token.appAccount);
        xhr.send();

        var result = JSON.parse(xhr.response);

        if (typeof fn == "function") {
            if (fn(result) == false) {
                return false;
            }
        }
    }

    //群信息
    c.queryGroupInfo = function (fn) {
        var token = c.config.token();
        token.appAccount = c.config.appAccount;
        var url = 'https://mimc.chat.xiaomi.net/api/topic/' + token.appId + '/' + c.config.group.topicId;
        var xhr = new XMLHttpRequest();
        xhr.open('GET', url, false);
        xhr.setRequestHeader('content-type', 'application/json');
        xhr.setRequestHeader('appKey', token.appKey);
        xhr.setRequestHeader('appSecret', token.appSecret);
        xhr.setRequestHeader('appAccount', token.appAccount);
        xhr.send();

        var result = JSON.parse(xhr.response);

        if (typeof fn == "function") {
            if (fn(result) == false) {
                return false;
            }
        }
    }

    //发送群消息
    c.sendGroupMsg = function (topicId, groupMsg) {
        try {
            var packetId = c.user.sendGroupMessage(topicId, groupMsg);
        } catch (err) {
            console.log("sendGroupMessage fail, err=" + err);
        }
    }

    //添加群成员
    c.addGroupMember = function (accounts, fn) {
        var token = c.config.token();
        var data = { accounts: accounts };
        var url = 'https://mimc.chat.xiaomi.net/api/topic/' + token.appId + '/' + c.config.group.topicId + '/accounts';
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, false);
        xhr.setRequestHeader('content-type', 'application/json');
        xhr.setRequestHeader('appKey', token.appKey);
        xhr.setRequestHeader('appSecret', token.appSecret);
        xhr.setRequestHeader('appAccount', c.config.group.ownerId);
        xhr.send(JSON.stringify(data));

        var result = JSON.parse(xhr.response);

        if (typeof fn == "function") {
            fn(result)
        }
    };

    //删除群成员
    c.delGroupMember = function (accounts, fn) {
        var token = c.config.token();
        var url = 'https://mimc.chat.xiaomi.net/api/topic/' + token.appId + '/' + c.config.group.topicId + '/accounts?accounts=' + accounts;
        var xhr = new XMLHttpRequest();
        xhr.open('DELETE', url, false);
        xhr.setRequestHeader('content-type', 'application/json');
        xhr.setRequestHeader('appKey', token.appKey);
        xhr.setRequestHeader('appSecret', token.appSecret);
        xhr.setRequestHeader('appAccount', c.config.group.ownerId);
        xhr.send();

        var result = JSON.parse(xhr.response);

        if (typeof fn == "function") {
            fn(result)
        }
    }

    //登录
    c.login = function (account) {
        if (account) {
            c.config.appAccount = account;
        }
        c.user = new MIMCUser(c.config.token().appId, c.config.appAccount);
        c.user.registerP2PMsgHandler(c.receiveP2PMsg);
        c.user.registerGroupMsgHandler(c.receiveP2TMsg);
        c.user.registerFetchToken(c.fetchMIMCToken);
        c.user.registerStatusChange(c.statusChange);
        c.user.registerServerAckHandler(c.serverAck);
        c.user.registerDisconnHandler(c.disconnect);
        c.user.login();
        c.config.uset.online = true;
    }

    c.ce = function (nodeName) {
        return document.createElement(nodeName);
    }

    window.c = c;

})(window);

//消息音
c.audio = function (type) {
    if (c.config.uset.volume != "0") {
        clearTimeout(c.audiodefer);
        c.audiodefer = setTimeout(function () {
            var play = c.ce('audio');
            play.src = "audio/" + type + ".mp3";
            document.body.appendChild(play);
            play.play();
            play.onended = function () { document.body.removeChild(play); }
        }, 1000 * 1)
    }
}

//消息容器
c.msgbox = $('#msgbox');
//群成员列表容器
c.users = $('#users');
//群成员数量
c.userscount = $('#groupmembercount');

//消息内容
c.writeToScreens = function (message, photo, user, time, align) {
    if (arguments.length == 1) {
        photo = c.config.uset.photo;
        user = "系统消息";
        time = c.timeFormat(new Date());
    } else {
        message = netnrmd.render(message);
    }
    user = decodeURIComponent(user);
    message = '<div class="mb-msg netnrmd-body">' + message + '</div>';
    user = '<a href="javascript:void(0);" onclick="c.toTA(this)">' + user + '</a>';
    //photo = '<img class="mb-photo" src="' + photo + '" alt="头像">';
    photo = '<i class="mb-photo fa fa-user fa-3x" ></i>';
    time = '<small>' + time + '</small>';
    var md = '<div class="mb-title">' + photo + user + time + '</div>';
    if (align == 2) {
        md = $('<div class="mb-item mb-right">' + md + message + '</div>');
    } else {
        md = $('<div class="mb-item">' + md + message + '</div>');
    }
    c.msgbox.append(md);
    c.config.uset.lock == "0" && (c.msgbox[0].scrollTop = 999999);
}

//群成员列表
c.writeToGroupMember = function (data) {
    if (data.code == 200) {
        var topicinfo = data.data.topicInfo, selfitem, htm = [];
        c.userscount.html(data.data.members.length);
        htm.push('<a href="#" class="list-group-item" title="群主" data-uuid="' + topicinfo.ownerUuid + '" data-account="' + topicinfo.ownerAccount + '"><i class="fa fa-user-circle-o text-primary"></i>' + topicinfo.ownerAccount + '</a>');
        $(data.data.members).each(function () {
            if (this.uuid != topicinfo.ownerUuid) {
                if (this.account == c.config.appAccount) {
                    selfitem = this;
                } else {
                    htm.push('<a href="#" class="list-group-item" data-uuid="' + this.uuid + '" data-account="' + decodeURIComponent(this.account) + '"><i class="fa fa-user-o"></i>' + decodeURIComponent(this.account) + '</a>');
                }
            }
        });
        if (selfitem) {
            var mu = '<a href="#" class="list-group-item" data-uuid="' + selfitem.uuid + '" data-account="' + decodeURIComponent(selfitem.account) + '"><i class="fa fa-user"></i>' + decodeURIComponent(selfitem.account) + '</a>';
            htm.splice(1, 0, mu);
        }
        c.users.html(htm.join(''));
    } else {
        console.log('query group member error')
    }
};

//写入用户信息
c.setInfo = function () {
    localStorage.setItem('chat/config', JSON.stringify({
        uid: c.config.appAccount,
        uset: c.config.uset
    }))

    $('#uid').html(decodeURIComponent(c.config.appAccount));

    $('#funcmsgrecord').find('a').each(function () {
        var icon = $(this).find('i');
        switch (this.hash) {
            case "#msgrecord-lock":
                if (c.config.uset.lock != "0") {
                    icon.removeClass('fa-square-o').addClass('fa-check-square-o');
                } else {
                    icon.removeClass('fa-check-square-o').addClass('fa-square-o');
                }
                break;
            case "#msgrecord-volume":
                if (c.config.uset.volume != "0") {
                    icon.removeClass('fa-square-o').addClass('fa-check-square-o');
                } else {
                    icon.removeClass('fa-check-square-o').addClass('fa-square-o');
                }
                break;
        }
    });

    if (c.config.uset.wide != "0") {
        $('#wrap').addClass('container-fluid').removeClass('container');
    } else {
        $('#wrap').addClass('container').removeClass('container-fluid');
    }
}

//进群&群列表
c.init_querylist = function () {
    c.queryBelongGroupInfo(function (data) {
        if (data.code == 200) {
            var ingroup = false;
            $(data.data).each(function () {
                if (this.topicId == c.config.group.topicId) {
                    ingroup = true;
                    return false;
                }
            });
            if (ingroup) {
                c.queryGroupInfo(function (data) {
                    c.writeToGroupMember(data);
                })
            } else {
                c.addGroupMember(c.config.appAccount, function (data) {
                    if (data.code == 200) {
                        setTimeout(function () {
                            sendTypeMsg('vary', -2);
                            c.queryGroupInfo(function (data) {
                                c.writeToGroupMember(data);
                            })
                        }, 1000);
                    } else {
                        console.log(data.message);
                    }
                });
            }
        } else {
            console.log(data.message);
        }
    })
}

c.init = function () {
    var getls = localStorage.getItem("chat/config");
    if (getls) {
        getls = JSON.parse(getls);
        c.config.appAccount = getls.uid;
        Object.assign(c.config.uset, getls.uset)

        c.login();
        c.setInfo();
        c.init_querylist();
    } else {
        $('#myModalAccount').modal();
        setTimeout(function () {
            $('#username')[0].focus();
            $('#username')[0].select();
        }, 300);
        $('#username').keydown(function (e) {
            e = e || window.event;
            if (e.keyCode == 13) {
                $('#btnOk')[0].click();
            }
        }).val(decodeURIComponent(c.config.appAccount));
        $('#btnOk').click(function () {
            var val = $('#username').val().trim();
            if (val == "" || val.indexOf('>') >= 0 && val.indexOf('<') >= 0 && val.indexOf(',') >= 0) {
                $('#mamsg').html('皮，用户名不能为空哦');
            } else {
                $('#mamsg').html('');
                $('#myModalAccount').modal('hide');
                c.config.appAccount = encodeURIComponent(val);
                c.login();
                c.setInfo();
                c.init_querylist();
            }
        });
    }
}

//NetnrMD编辑器 功能扩展

netnrmd.extend = {
    //上传
    upload: {
        //按钮
        button: { title: '上传', cmd: 'upload', svg: '<path fill-rule="evenodd" d="M7.646 5.146a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 6.707V10.5a.5.5 0 0 1-1 0V6.707L6.354 7.854a.5.5 0 1 1-.708-.708l2-2z"/><path d="M4.406 3.342A5.53 5.53 0 0 1 8 2c2.69 0 4.923 2 5.166 4.579C14.758 6.804 16 8.137 16 9.773 16 11.569 14.502 13 12.687 13H3.781C1.708 13 0 11.366 0 9.318c0-1.763 1.266-3.223 2.942-3.593.143-.863.698-1.723 1.464-2.383zm.653.757c-.757.653-1.153 1.44-1.153 2.056v.448l-.445.049C2.064 6.805 1 7.952 1 9.318 1 10.785 2.23 12 3.781 12h8.906C13.98 12 15 10.988 15 9.773c0-1.216-1.02-2.228-2.313-2.228h-.5v-.5C12.188 4.825 10.328 3 8 3a4.53 4.53 0 0 0-2.941 1.1z"/>' },
        //动作
        action: function (that) {
            if (!that.uploadpopup) {
                //构建弹出内容
                var htm = [];
                htm.push('<div style="height:100px;margin:15px;border:3px dashed #ddd">');
                htm.push('<input type="file" style="width:100%;height:100%;" />');
                htm.push('</div>');

                //保存创建的上传弹出
                that.uploadpopup = netnrmd.popup("上传", htm.join(''));
                var ptitle = that.uploadpopup.querySelector('.np-header').querySelector('span');

                //选择文件上传，该上传接口仅为演示使用，仅支持图片格式的附件
                that.uploadpopup.querySelector('input').addEventListener('change', function () {
                    var file = this.files[0];
                    if (file) {
                        if (file.size > 1024 * 1024 * 10) {
                            alert('文件过大 （MAX 10 MB）')
                            this.value = "";
                            return;
                        }

                        var fd = new FormData();
                        fd.append('file', file);

                        //发起上传
                        var xhr = new XMLHttpRequest();
                        xhr.upload.onprogress = function (event) {
                            if (event.lengthComputable) {
                                //上传百分比
                                var per = ((event.loaded / event.total) * 100).toFixed(2);
                                if (per < 100) {
                                    ptitle.innerHTML = netnrmd.extend.upload.button.title + " （" + per + "%）";
                                } else {
                                    ptitle.innerHTML = netnrmd.extend.upload.button.title;
                                }
                            }
                        };

                        xhr.open("POST", "https://www.netnr.eu.org/api/v1/Upload", true);
                        xhr.send(fd);
                        xhr.onreadystatechange = function () {
                            if (xhr.readyState == 4) {
                                if (xhr.status == 200) {
                                    console.log(xhr.responseText)
                                    var res = JSON.parse(xhr.responseText);
                                    if (res.code == 200) {
                                        let url = "https://www.netnr.eu.org" + res.data.path;
                                        //上传成功，插入链接
                                        netnrmd.insertAfterText(that.obj.me, '[' + file.name + '](' + url + ')');
                                        that.uploadpopup.style.display = "none";
                                    } else {
                                        alert('上传失败');
                                    }
                                } else {
                                    alert('上传失败');
                                }
                            }
                        }
                    }
                }, false)
            }
            that.uploadpopup.style.display = "";
            that.uploadpopup.querySelector('input').value = '';
        }
    }
}

require(['vs/editor/editor.main'], function () {

    //初始化
    window.nmd = new netnrmd('#editor', {
        height: 150,

        //渲染前回调
        viewbefore: function () {
            this.items.forEach(item => {
                if (!["emoji", "fullscreen", "splitscreen"].includes(item.cmd)) {
                    item.class = item.class == null ? "hidden" : item.class + " hidden";
                }
            })
            this.items.push(netnrmd.extend.upload.button);
        },

        //命令回调
        cmdcallback: function (cmd) {
            switch (cmd) {
                case "upload":
                    netnrmd.extend[cmd].action(this)
                    break;
            }
        }
    });

    //快捷键
    nmd.obj.me.addCommand(monaco.KeyCode.Enter | monaco.KeyMod.CtrlCmd, function () {
        $('#btnPush')[0].click();
    })

});

c.init();

//@TA
c.toTA = function (that) {
    netnrmd.insertAfterText(nmd.obj.me, '@' + $(that).text() + " ");
}

function sendTypeMsg(msg, type) {
    if (!c.config.uset.online) {
        c.login();
    }
    var mp = {};
    mp.uid = c.config.appAccount;
    mp.type = type == undefined ? 1 : type;
    mp.message = msg;
    c.sendGroupMsg(c.config.group.topicId, JSON.stringify(mp))

    //本地显示非系统消息
    if (mp.type > 0) {
        c.writeToScreens(mp.message, c.config.uset.photo, mp.uid, c.timeFormat(new Date()), 2);
    }
}
//推送消息
$('#btnPush').click(function () {
    var msg = nmd.getmd().trim();
    if (msg != "") {
        sendTypeMsg(msg);
        nmd.setmd('');
    }
});


//菜单功能
$('#funcmenu').click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "A") {
        switch (target.hash) {
            case "#menu-wide":
                {
                    if (c.config.uset.wide != "0") {
                        c.config.uset.wide = "0"
                    } else {
                        c.config.uset.wide = "1"
                    }
                    c.setInfo();
                }
                break;
            case "#menu-logoff":
                {
                    localStorage.removeItem('chat/config');
                    location.reload(false);
                }
                break;
        }
    }
});

//消息记录功能
$('#funcmsgrecord').click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "A") {
        switch (target.hash) {
            case "#msgrecord-clear":
                c.msgbox.html('');
                break;
            case "#msgrecord-lock":
                {
                    if (c.config.uset.lock != "0") {
                        c.config.uset.lock = "0"
                    } else {
                        c.config.uset.lock = "1"
                    }
                    c.setInfo();
                }
                break;
            case "#msgrecord-volume":
                {
                    if (c.config.uset.volume != "0") {
                        c.config.uset.volume = "0"
                    } else {
                        c.config.uset.volume = "1"
                    }
                    c.setInfo();
                }
                break;
            default:
        }
    }
});

//清理成员
c.clearMember = function (force) {
    if (Number(c.userscount.html()) > 50 || force) {
        var accs = [];
        c.users.find('a').each(function () {
            var uid = $(this).attr('data-account');
            if (uid != c.config.group.ownerId && encodeURIComponent(uid) != c.config.appAccount) {
                accs.push(encodeURIComponent(encodeURIComponent(uid)));
            }
        });
        if (accs.length) {
            c.delGroupMember(accs.join(','), function (data) {
                sendTypeMsg('vary', -2);
                c.queryGroupInfo(function (data) {
                    c.writeToGroupMember(data);
                })
            });
        }
    }
}

//功能
c.funclist = function (type) {
    switch (type) {
        case "#msg-font":
            {

            }
            break;
        case "#app-file":
            {
                $('#grouptypenav').children().removeClass('active').eq(2).addClass('active');
                $('#grouptypebox').children().removeClass('active in').eq(2).addClass('active in');
            }
            break;
    }
}
//发送功能
$('#sendtool').click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "I") {
        c.funclist($(target).parent()[0].hash);
    }
});
//群应用功能
$('#chatapply').click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement, ca;
    $(this).children().each(function () {
        if (this.contains(target)) {
            ca = this;
            return false;
        }
    });
    if (ca) {
        c.funclist(ca.hash)
    }
});
//点击人员列表
$('#users').click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "A") {
        c.toTA(target);
    }
})

//注销
$('#btnLogout').click(function () {
    c.logout();
});

$(window).on('load resize', function () {
    var ch = $(window).height();
    c.msgbox.css('height', ch - 240);
    var user = $('#users');
    user.css('height', ch - user.offset().top - 18);
});