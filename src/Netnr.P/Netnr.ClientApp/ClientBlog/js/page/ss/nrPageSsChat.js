import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/chat",
    ckeyAccount: "/ss/chat/account",

    init: async () => {
        Object.assign(nrVary.domChatView.style, {
            minHeight: '15em',
            height: `calc(100vh - 440px)`
        });

        await nrStorage.init();

        await nrcRely.remote('netnrmdAce.js');
        await nrcRely.remote('netnrmd');
        await nrcBase.importScript('/file/identicon/identicon.min.js');
        nrApp.tsMd = netnrmd.init(nrVary.domChatWrite, {
            theme: nrcBase.isDark() ? "dark" : "light",
            height: 240,
            viewmodel: 1,

            // 渲染前回调
            viewbefore: function () {
                this.objToolbarIcons.forEach(item => {
                    if (!['emoji', 'code', 'fullscreen', 'splitscreen'].includes(item.cmd)) {
                        item.class = (item.class || "") + ' d-none';
                    }
                });

                this.objToolbarIcons.unshift({
                    title: '分隔符', cmd: 'split', svg: '<path d="M9.5 13a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z"/>'
                })
                this.objToolbarIcons.unshift({
                    title: '发送', cmd: 'send', key: 'Ctrl+Enter', svg: '<path d="M15.854.146a.5.5 0 0 1 .11.54l-5.819 14.547a.75.75 0 0 1-1.329.124l-3.178-4.995L.643 7.184a.75.75 0 0 1 .124-1.33L15.314.037a.5.5 0 0 1 .54.11ZM6.636 10.07l2.761 4.338L14.13 2.576 6.636 10.07Zm6.787-8.201L1.591 6.602l4.339 2.76 7.494-7.493Z"/>',
                    action: () => {
                        nrPage.sendUnlimitedGroupMessage(nrApp.tsMd.getmd());
                    }
                })
            },
        });

        nrPage.bindEvent();

        nrVary.domWait.nextElementSibling.classList.remove('invisible');
        nrVary.domWait.remove();

        //读取账号
        nrPage.chatAccount = await nrStorage.getItem(nrPage.ckeyAccount);
        if (nrPage.chatAccount) {
            nrVary.domTxtAccount.value = nrPage.chatAccount;

            //自动登录
            nrVary.domBtnLogin.click();
        }
    },

    bindEvent: () => {
        nrVary.domBtnLogin.addEventListener('click', async function () {
            if (nrPage.chatOnline) {
                //注销
                nrApp.setLoading(nrVary.domBtnLogin);
                nrPage.chatClient.logout();
            } else {
                //加入
                if (nrVary.domTxtAccount.value.trim() != "") {
                    nrPage.chatAccount = nrVary.domTxtAccount.value.trim();

                    nrApp.setLoading(nrVary.domBtnLogin);
                    await nrPage.chatLogin();
                } else {
                    nrApp.alert('请输入账号');
                }
            }
        });
        nrVary.domTxtAccount.addEventListener('keydown', async function (event) {
            if (nrPage.chatOnline == false && event.code == "Enter") {
                nrVary.domBtnLogin.click();
            }
        })

        //body click
        document.body.addEventListener('click', async function (event) {
            let target = event.target;
            let action = target.dataset.action;
            switch (action) {
                case 'mute':
                    target.classList.toggle('active');
                    nrPage.chatConfig.mute = !nrPage.chatConfig.mute;
                    break;
                case 'about':
                    nrApp.alert(netnrmd.render([
                        '#### 基于 MIMC 的无限大群',
                        'https://github.com/Xiaomi-mimc/mimc-webjs-sdk  ',
                        'https://admin.mimc.chat.xiaomi.net/docs/0403-webjs.html'
                    ].join('\r\n')), target.innerHTML);
                    break;
            }

        });
    },

    chatClient: null,
    chatAccount: null,
    chatOnline: false,
    chatGroup: {
        ownerId: 'netnr',
        name: "NetnrGroup",
        topicId: "11464322971074560",
    },
    chatUnlimitedGroup: {
        topicId: "51664086823862272",
        topicName: "Netnrs",
    },
    chatKeys: {
        appId: "2882303761517686414",
        appKey: "5341768622414",
        appSecret: "u5BI0V0QYuodKEabigumzA=="
    },
    chatConfig: {
        mute: false, //静音
        messageLast: null,
    },
    postJson: (url, data) => {
        let xhr = new XMLHttpRequest();
        xhr.open('POST', url, false);
        xhr.setRequestHeader('content-type', 'application/json');
        xhr.send(JSON.stringify(data));

        return JSON.parse(xhr.response);
    },
    callback: {
        fetchMIMCToken: () => {
            let token = Object.assign({
                appAccount: nrPage.chatAccount
            }, nrPage.chatKeys);

            let result = nrPage.postJson('https://mimc.chat.xiaomi.net/api/account/token', token);
            return result;
        },
        //在线状态变化回调
        statusChange: (bindResult, errType, errReason, errDesc) => {
            console.debug('statusChange', bindResult, errType, errReason, errDesc)
            nrPage.chatOnline = bindResult;
            nrApp.setLoading(nrVary.domBtnLogin, true);
            //上线
            if (bindResult) {
                nrPage.addMessage(0, `您（<b>${nrcBase.htmlEncode(nrPage.chatAccount)}</b>）<span class="text-success">已上线</span>`);

                nrVary.domTxtAccount.readOnly = true;
                nrVary.domBtnLogin.innerHTML = "注销";

                nrStorage.setItem(nrPage.ckeyAccount, nrVary.domTxtAccount.value.trim());

                //进入群聊
                nrPage.joinUnlimitedGroup();
            } else {
                nrApp.alert(`登录失败！<hr/>${errType}<hr/>${errReason}<hr/>${errDesc}`);
            }
        },
        //解散无限大群回调
        ucDismiss: (topicId) => {
            console.debug('ucDismiss', topicId);
            nrPage.addMessage(0, '解散无限大群');
        },
        //加入无限大群回调
        ucJoinResp: (topicId, code, msg) => {
            console.debug('ucJoinResp', topicId, code, msg)
            if (code == 0) {
                nrPage.addMessage(0, `您已进入群聊`);
            } else {
                nrApp.alert(`加入群聊失败<hr/>${msg}`);
            }
        },
        //接收无限大群消息回调
        ucMessage: (groupMsg) => {
            nrPage.addMessage(2, groupMsg);
        },
        /**
         * @param[packetId] string: 成功发送到服务器消息的packetId，即sendMessage(,)的返回值
         * @param[sequence] string: 服务器生成，单用户空间内递增唯一，可用于排序（升序）/去重
         * @param[timeStamp] string: 消息到达服务器时间（ms）
         * @param[errMsg] string: 服务器返回的错误信息
         **/
        serverAck: (packetId, sequence, timeStamp, errMsg) => {
            console.debug('serverAck', packetId, sequence, timeStamp, errMsg)
            //自己消息
            nrPage.addMessage(1, timeStamp);
            nrApp.tsMd.setmd('');
        },
        //连接断开回调
        disconnect: () => {
            nrPage.addMessage(0, `您（<b>${nrcBase.htmlEncode(nrPage.chatAccount)}</b>）<span class="text-danger">已下线</span>`);

            nrPage.chatOnline = false;
            nrApp.setLoading(nrVary.domBtnLogin, true);
            nrVary.domTxtAccount.readOnly = false;
            nrVary.domBtnLogin.innerHTML = "登录";
        },
    },

    //登录
    chatLogin: async () => {
        await nrcBase.importScript('/file/mimc/mimc-min_1_0_3.js');

        let user = nrPage.chatClient = new MIMCUser(nrPage.chatKeys.appId, nrPage.chatAccount);

        user.registerFetchToken(nrPage.callback.fetchMIMCToken);         //获取token回调
        user.registerStatusChange(nrPage.callback.statusChange);         //登录结果回调
        user.registerServerAckHandler(nrPage.callback.serverAck);        //发送消息后，服务器接收到消息ack的回调
        // user.registerP2PMsgHandler(receiveP2PMsg);       //接收单聊消息回调
        // user.registerGroupMsgHandler(receiveP2TMsg);     //接收群聊消息回调
        // user.registerPullNotificationHandler(pullNotificationHandler); //拉取离线消息回调
        user.registerDisconnHandler(nrPage.callback.disconnect);         //连接断开回调
        user.registerUCDismissHandler(nrPage.callback.ucDismiss);        //解散无限大群回调
        user.registerUCJoinRespHandler(nrPage.callback.ucJoinResp);      //加入无限大群回调
        user.registerUCMsgHandler(nrPage.callback.ucMessage);            //接收无限大群消息回调
        // user.registerUCQuitRespHandler(ucQuitResp);      //退出无限大群回调

        user.login();
    },

    //创建无限大群
    createUnlimitedGroup: () => {
        return nrPage.chatClient.createUnlimitedGroup("Netnrs", console.debug);
        // console.debug(`群id：${topicId}`)
        // console.debug(`群名称：${topicName}`)
        // console.debug(`是否创建成功：${isSuccess}`)
        // console.debug(`错误信息：${errMsg}`)
    },

    /**
     * 发送无限大群消息
     * @param {*} payload 消息主体
     */
    sendUnlimitedGroupMessage: (payload) => {
        if (!nrPage.chatOnline) {
            nrApp.alert('请选登录')
        } else if (payload.trim() == "") {
            nrApp.toast('请输入内容');
        } else {
            nrPage.chatConfig.messageLast = payload;
            return nrPage.chatClient.sendUnlimitedGroupMessage(nrPage.chatUnlimitedGroup.topicId, encodeURIComponent(payload));
        }
    },

    /**
     * 加入无限大群
     */
    joinUnlimitedGroup: () => {
        return nrPage.chatClient.joinUnlimitedGroup(nrPage.chatUnlimitedGroup.topicId);
    },

    /**
     * 添加消息
     * @param {*} type 类型，0：系统消息；1：自己消息；2：接收消息
     * @param {*} message 
     * @param {*} color 
     */
    addMessage: (type, message, color = 'secondary') => {
        let domMessage = document.createElement("div");
        domMessage.classList.add('mb-4');

        switch (type) {
            case 0:
                domMessage.innerHTML = `<span class="badge bg-${color}">系统消息</span><em class="opacity-50 mx-3">${nrcBase.now()}</em><span class="mt-2">${message}</span>`;
                break;
            case 1:
                domMessage.innerHTML = `
                <div class="row">
                    <div class="col text-end">
                        <div><em class="small opacity-50">${nrcBase.formatDateTime('datetime', message * 1)}</em></div>
                        <div class="float-end text-start mt-2 px-3 py-2 border border-success-subtle rounded nrg-clear-bottom" style="max-width:75%">${netnrmd.render(nrPage.chatConfig.messageLast)}</div>
                    </div>
                    <div class="col-auto">${nrcBase.getIconHtml('person-fill')}</div>
                </div>`;
                break;
            case 2:
                {
                    // console.debug(message.getPacketId()); // 客户端生成的消息ID
                    let fromAccount = message.getFromAccount(); // 消息发送者在APP帐号系统的帐号ID
                    // console.debug(message.getFromResource()); // 消息发送者resource
                    // console.debug(message.getTopicId()); // 群ID
                    let timeStamp = message.getTimeStamp() * 1; // 消息发送时间戳
                    let payload = decodeURIComponent(message.getPayload()); // payload为用户自定义消息，UTF-8 string类型

                    if (!nrPage.chatConfig.mute) {
                        nrcBase.voice(`有新消息`);
                    }
                    domMessage.innerHTML = `
                    <div class="row">
                        <div class="col-auto"><span class="border rounded d-inline-block mt-1">${iisvg({ value: fromAccount, size: 42 }).outerHTML}</span></div>
                        <div class="col">
                            <div>
                                <span class="me-3">${nrcBase.htmlEncode(fromAccount)}</span>
                                <em class="small opacity-50">${nrcBase.formatDateTime('datetime', timeStamp)}</em>
                            </div>
                            <div class="float-start mt-2 nrg-clear-bottom" style="max-width:75%">${netnrmd.render(payload)}</div>
                        </div>
                        <div class="col-1"></div>
                    </div>`;
                }
                break;
        }

        let isToBottom = nrVary.domChatView.scrollTop + nrVary.domChatView.clientHeight == nrVary.domChatView.scrollHeight;
        nrVary.domChatView.appendChild(domMessage);
        //底部时，保持
        if (isToBottom) {
            nrVary.domChatView.scrollTo(0, nrVary.domChatView.scrollHeight);
        }
    }
}

export { nrPage };