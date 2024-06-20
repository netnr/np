import { ndkEditor } from "./ndkEditor";
import { ndkStep } from "./ndkStep";
import { ndkTab } from "./ndkTab";
import { ndkVary } from './ndkVary';
import { ndkView } from './ndkView';
import { ndkStorage } from "./ndkStorage";
import { ndkNoteSQL } from "./ndkNoteSQL";
import { ndkI18n } from "./ndkI18n";
import { ndkExecute } from "./ndkExecute";
import { ndkFunction } from "./ndkFunction";
import { ndkRequest } from "./ndkRequest";
import { nrcBase } from "../../frame/nrcBase";
import { nrApp } from "../../frame/Shoelace/nrApp";
import { nrGrid } from "../../frame/nrGrid";

// 动作
var ndkAction = {

    /**
     * 动作命令
     * @param {*} cmd
     * @param {*} args
     */
    actionRun: function (cmd, args) {
        var dval = typeof (args) == "object" ? args.dataset.val : args;
        switch (cmd) {
            //主题
            case "theme":
                {
                    ndkVary.theme = dval;
                    ndkVary.domMenu.querySelectorAll('[data-cmd="theme"]').forEach(dom => {
                        dom.checked = dval == dom.dataset.val;
                    })
                    nrApp.setTheme(ndkVary.theme);
                    nrcBase.saveTheme(ndkVary.theme);
                    ndkStep.stepSave();
                }
                break;
            //语言
            case "language":
                {
                    ndkI18n.language = dval;
                    ndkVary.domMenu.querySelectorAll('[data-cmd="language"]').forEach(item => {
                        item.checked = item.getAttribute('data-val') == ndkI18n.language;
                    });
                    //点击菜单时
                    if (typeof (args) == "object") {
                        ndkFunction.output(ndkI18n.lg.reloadDone);
                        ndkStep.stepSave();
                    }
                }
                break;
            //设置
            case "setting-manager":
                {
                    ndkVary.domDialogSetting.show();
                    //回填
                    ndkVary.domTextApiServer.value = ndkVary.apiServer;

                    var pchtm = [];
                    for (const key in ndkVary.parameterConfig) {
                        const item = ndkVary.parameterConfig[key], label = ndkI18n.lg[`parameterKey_${key}`];
                        switch (item.type) {
                            case "number":
                                pchtm.push(`<div><sl-input name="${key}" label="${label}" type="${item.type}" required placeholder="${ndkI18n.lg.default} ${item.defaultValue}" value="${item.value}"></sl-input></div>`);
                                break;
                            case "select":
                            case "boolean":
                                {
                                    pchtm.push(`<div><sl-select name="${key}" label="${label}" value="${item.value}" hoist="true">`);
                                    item.list.forEach(obj => {
                                        var txt = ndkI18n.lg[`parameterKey_${key}_${obj.val}`.replace('-', '_')];
                                        var ic = item.defaultValue == obj.val ? "class='nrc-item-default'" : "";
                                        pchtm.push(`<sl-option ${ic} value="${obj.val}">${txt}</sl-option>`);
                                    })
                                    pchtm.push(`</sl-select></div>`);
                                }
                                break;
                        }
                    }
                    pchtm.push(`<div><sl-button size="large" pill>${ndkVary.emoji.success}${ndkI18n.lg.save}</sl-button></div>`);
                    ndkVary.domParameterConfig.innerHTML = pchtm.join('');

                    if (ndkVary.domDialogSetting.getAttribute('event-bind') != 1) {
                        ndkVary.domDialogSetting.setAttribute('event-bind', 1);

                        //接口服务
                        ndkVary.domTextApiServer.addEventListener('input', function () {
                            ndkVary.apiServer = this.value;
                            ndkStep.stepSave();
                        }, false);
                        ndkVary.domTextApiServer.addEventListener('blur', function () {
                            if (this.value.endsWith('/')) {
                                ndkVary.apiServer = this.value.substring(0, this.value.length - 1);
                                this.value = ndkVary.apiServer;
                            }
                            ndkStep.stepSave();
                        }, false);

                        //参数保存
                        ndkVary.domParameterConfig.addEventListener('click', function (e) {
                            if (e.target.tagName == "SL-BUTTON") {
                                for (const key in ndkVary.parameterConfig) {
                                    const item = ndkVary.parameterConfig[key];
                                    let val = ndkVary.domParameterConfig.querySelector(`[name="${key}"]`).value;
                                    if (item.type == "number") {
                                        val = parseInt(val);
                                    } else if (item.type == "boolean") {
                                        val = val == "true";
                                    }
                                    item.value = val;
                                }
                                ndkStep.stepSave();
                                ndkFunction.output(ndkI18n.lg.done);
                            }
                        }, false);
                    }
                }
                break;
            //重置
            case "reset":
                {
                    ndkFunction.alert(`<sl-button style="width:100%" size="large" variant="danger">${ndkI18n.lg.deleteAllData}</sl-button>`, ndkI18n.lg.reset, '25vw');
                    ndkVary.domAlert.querySelector('sl-button').addEventListener('click', async () => {
                        if (confirm(ndkI18n.lg.deleteAllData)) {
                            await ndkStorage.instanceCache.dropInstance();
                            await ndkStorage.instanceConfig.dropInstance();
                            location.reload(false);
                        }
                    })
                }
                break;
            case "about":
                {
                    ndkFunction.alert(`<div>${ndkVary.name} <sl-badge>v${nrcBase.version}</sl-badge></div>
<p><a href="https://github.com/netnr/np" target="_blank">https://github.com/netnr/np</a></p>
<a href="https://zme.ink" target="_blank">Sponsors</a>`, ndkI18n.lg.about, '25vw');
                }
                break;
            //设置接口服务
            case "set-api-server":
                {
                    ndkVary.domTextApiServer.value = ndkVary.apiServer = dval;
                    ndkStep.stepSave();
                }
                break;
            //测试接口服务
            case "test-api-server":
                {
                    var url = `${ndkVary.domTextApiServer.value}${ndkVary.apiServiceStatus}`;
                    ndkVary.domTestApiServer.loading = true;
                    ndkRequest.reqServiceStatus(url).then(isOk => {
                        ndkVary.domTestApiServer.loading = false;
                        ndkFunction.alert(isOk ? ndkI18n.lg.success : ndkI18n.lg.serverError)
                    })
                }
                break;
            //导出配置
            //导出配置（剪贴板）
            case "config-export":
            case "config-export-clipboard":
                {
                    //导出项
                    var keys = [], parr = [], configObj = {
                        date: ndkFunction.now(),
                        version: nrcBase.version
                    };
                    ndkVary.domConfigExportItems.querySelectorAll('sl-checkbox').forEach(item => {
                        if (item.checked) {
                            var val = item.getAttribute('data-val');
                            keys.push(val);
                            var keyo = Object.values(ndkStorage.keys).find(x => x.key == val);
                            parr.push(ndkStorage.instanceConfig.getItem(keyo.key))
                        }
                    });
                    Promise.all(parr).then(arr => {
                        for (var i = 0; i < arr.length; i++) {
                            configObj[keys[i]] = arr[i];
                        }

                        //配置加密
                        ndkAction.safeConfig(configObj, "AESEncrypt").then(() => {
                            if (cmd.endsWith("clipboard")) {
                                var content = JSON.stringify(configObj, null, 4);
                                ndkFunction.clipboard(content).then(() => {
                                    ndkFunction.output(ndkI18n.lg.done)
                                    ndkFunction.toast(ndkI18n.lg.done)
                                })
                            } else {
                                var content = JSON.stringify(configObj);
                                ndkFunction.download(content, "ndk.json");
                            }
                        });
                    })
                }
                break;
            //导入配置
            //导入配置（剪贴板）
            case "config-import":
            case "config-import-clipboard":
                {
                    try {
                        new Promise((resolve) => {
                            if (cmd.endsWith("clipboard")) {
                                ndkFunction.clipboard().then(content => {
                                    resolve(content);
                                })
                            } else {
                                resolve(args.previousElementSibling.value);
                            }
                        }).then(content => {
                            if (content.trim() == "") {
                                ndkFunction.output(ndkI18n.lg.contentNotEmpty);
                            } else {
                                var configObj = JSON.parse(content), parr = [];

                                //配置解密
                                ndkAction.safeConfig(configObj, "AESDecrypt").then(() => {
                                    var keys = Object.keys(configObj);
                                    Object.values(ndkStorage.keys).forEach(keyo => {
                                        if (keys.includes(keyo.key)) {
                                            parr.push(ndkStorage.instanceConfig.setItem(keyo.key, configObj[keyo.key]))
                                        }
                                    })

                                    Promise.all(parr).then(() => {
                                        ndkFunction.output(ndkI18n.lg.done);
                                        ndkFunction.toast(ndkI18n.lg.done)
                                        location.reload(false);
                                    })
                                });
                            }
                        });
                    } catch (e) {
                        console.debug(e)
                        ndkFunction.output(e)
                    }
                }
                break;
            //新建查询
            case "new-query":
                {
                    switch (ndkVary.domTabGroupTree.activeTab.panel) {
                        case "tp1-conns":
                            {
                                //选中或范围项
                                var edata = ndkVary.gridOpsConns.getSelectedRows()[0];
                                if (edata) {
                                    var tabTitle = ndkVary.iconSvg("plug-fill", edata.alias);
                                    var cp = { cobj: edata };
                                    ndkTab.tabBuildFast_sql(cp, tabTitle);
                                } else {
                                    ndkFunction.output(ndkI18n.lg.selectDataRows);
                                }
                            }
                            break;
                        case "tp1-database":
                            {
                                var cp = ndkStep.cpGet(1);

                                //选中或范围项
                                var edata = ndkVary.gridOpsDatabase.getSelectedRows()[0];
                                if (edata) {
                                    var tabTitle = ndkVary.iconSvg("server", edata.DatabaseName);
                                    cp.databaseName = edata.DatabaseName;
                                    ndkTab.tabBuildFast_sql(cp, tabTitle);
                                } else {
                                    edata = cp.cobj;
                                    var tabTitle = ndkVary.iconSvg("plug-fill", edata.alias);
                                    ndkTab.tabBuildFast_sql(cp, tabTitle);
                                }
                            }
                            break;
                        case "tp1-table":
                        case "tp1-column":
                            {
                                var cp = ndkStep.cpGet(1);
                                if (cp && cp.cobj && cp.databaseName) {
                                    var tabTitle = ndkVary.iconSvg("server", cp.databaseName);
                                    ndkTab.tabBuildFast_sql(cp, tabTitle);
                                } else {
                                    ndkFunction.output(ndkI18n.lg.selectDataRows);
                                }
                            }
                            break;
                    }
                }
                break;
            //执行选中或全部脚本
            case "sql-execute-selected-or-all":
                {
                    var tpkey = args.getAttribute('panel');
                    var tpobj = ndkTab.tabKeys[tpkey];

                    ndkExecute.editorSql(tpkey)
                }
                break;
            //格式化SQL
            case "sql-formatting":
                {
                    var tpkey = args.getAttribute('panel');
                    var tpobj = ndkTab.tabKeys[tpkey];

                    ndkEditor.formatter(tpobj.editor);
                }
                break
            //生成代码
            case "quick-generate-code":
                {
                    var cp = ndkStep.cpCurrent();
                    if (cp) {
                        var edata = cp.cobj;
                        ndkTab.tabBuild(10099, ndkVary.iconSvg("file-earmark-code", ndkI18n.lg.generateCode), ndkTab.tabType.code).then(tpkey => {
                            ndkVary.domTabGroupBody.show(tpkey);//显示选项卡
                            ndkStep.cpSet(tpkey, edata); //记录连接
                            ndkStep.cpInfo(tpkey); //显示连接
                        })
                    }
                }
                break;
            //生成假数据
            case "quick-generate-faker":
                {
                    var cp = ndkStep.cpCurrent();
                    if (cp) {
                        var edata = cp.cobj;
                        ndkTab.tabBuild(10098, ndkVary.iconSvg("box", ndkI18n.lg.generateFaker), ndkTab.tabType.faker).then(tpkey => {
                            ndkVary.domTabGroupBody.show(tpkey);//显示选项卡
                            ndkStep.cpSet(tpkey, edata); //记录连接
                            ndkStep.cpInfo(tpkey); //显示连接
                        })
                    }
                }
                break;
            //打开链接
            case "open-link":
                {
                    var gourl = document.createElement("a");
                    gourl.href = dval;
                    gourl.target = "_blank";
                    gourl.click();
                }
                break;
            //SQL 笔记
            case "sql-notes":
                {
                    var tpkey = args.getAttribute('panel');
                    var cp = ndkStep.cpGet(tpkey);
                    ndkView.viewExecuteSql({ Item1: { "sql-notes": ndkNoteSQL[cp.cobj.type] } }, tpkey)
                }
                break
            //SQL 历史
            case "sql-historys":
                {
                    var tpkey = args.getAttribute('panel');
                    var cp = ndkStep.cpGet(tpkey);
                    ndkStorage.historysGet().then(historys => {
                        ndkView.viewExecuteSql({ Item1: { "sql-historys": historys } }, tpkey)
                    })
                }
                break
            // 查看单元格-base64
            case "view-cell-base64":
                {
                    var ftinfo = ndkFunction.base64Detect(dval);
                    var dmime = 'application/octet-stream';
                    var detectItems = '<sl-menu-item>未知</sl-menu-item>';
                    if (ftinfo.length) {
                        dmime = ftinfo[0].mime;
                        detectItems = ftinfo.map(x => x.mime == null ? "" : `<sl-menu-item value="${x.mime}">.${x.extension} → ${x.mime}</sl-menu-item>`).join('');
                    }

                    var html = `
<sl-button-group>
    <sl-input class="view-cell-mime" placeholder="MIME-Type" value="${dmime}"></sl-input>
    <sl-button class="view-cell-tofile" variant="warning">${ndkI18n.lg.viewBase64ToFile}</sl-button>
    <sl-dropdown>
        <sl-button slot="trigger" caret variant="default">${ndkI18n.lg.viewBase64Detect}</sl-button>
        <sl-menu>${detectItems}</sl-menu>
    </sl-dropdown>
</sl-button-group>
<sl-button class="view-cell-decode" variant="warning">${ndkI18n.lg.viewBase64Decode}</sl-button>
<div class="view-cell-body" style="margin-top:1em;min-height:40vh"></div>`;
                    ndkFunction.alert(html, null, 'full');

                    var domViewMime = ndkVary.domAlert.querySelector('.view-cell-mime');
                    var domViewBode = ndkVary.domAlert.querySelector('.view-cell-body');
                    var domViewTofile = ndkVary.domAlert.querySelector('.view-cell-tofile');
                    var domViewDecode = ndkVary.domAlert.querySelector('.view-cell-decode');

                    // base64 to text
                    ndkVary.domAlert.querySelector('sl-dropdown').addEventListener('sl-select', event => {
                        if (this.contains(event.target)) {
                            const domItem = event.detail.item;
                            domViewMime.value = domItem.value;
                            domViewTofile.click();
                        }
                    });

                    // base64 to file
                    domViewTofile.addEventListener('click', function () {
                        var mime = ndkVary.domAlert.querySelector('.view-cell-mime').value.trim();
                        if (mime == "") {
                            mime = "application/octet-stream";
                        }

                        var bin = window.atob(dval);
                        var len = bin.length;
                        var arr = new Uint8Array(len);
                        for (var i = 0; i < len; i++) {
                            arr[i] = bin.charCodeAt(i);
                        }

                        var blob = new Blob([arr], { type: mime });

                        var vnode;
                        if (blob.type.indexOf("image") >= 0) {
                            vnode = document.createElement("img");
                        }
                        if (blob.type.indexOf("audio") >= 0) {
                            vnode = document.createElement("audio");
                            vnode.controls = true;
                        }
                        if (blob.type.indexOf("video") >= 0) {
                            vnode = document.createElement("video");
                            vnode.controls = true;
                        }
                        if (vnode) {
                            vnode.src = URL.createObjectURL(blob);
                        } else {
                            vnode = document.createElement("a");
                            vnode.href = URL.createObjectURL(blob);
                            vnode.download = "file.bin";
                            vnode.innerHTML = "下载";
                        }
                        vnode.style.maxWidth = "100%";

                        domViewBode.innerHTML = "";
                        domViewBode.appendChild(vnode);
                    });

                    // base64 to text
                    domViewDecode.addEventListener('click', function () {
                        try {
                            domViewBode.innerHTML = '<sl-textarea></sl-textarea>';
                            ndkVary.domAlert.querySelector('sl-textarea').value = window.atob(dval);
                        } catch (error) {
                            ndkFunction.output(error)
                        }
                    });
                }
                break;
        }
    },

    /**
     * 设置分离器大小
     * @param {*} domSpliter 
     * @param {*} isShow 
     */
    setSpliterSize: (domSpliter, isShow = 'no') => {
        //是水平
        var isHorizontal = domSpliter.classList.contains('nrc-spliter-horizontal'),
            domSpliterChild = domSpliter.children;

        //自动显示第二项
        if (isShow == 'auto') {
            if (isHorizontal && domSpliterChild[3].clientWidth / domSpliter.clientWidth < 0.15) {
                isShow = '50%';
            } else if (!isHorizontal && domSpliterChild[3].clientHeight / domSpliter.clientHeight < 0.15) {
                isShow = '30%';
            } else {
                return false;
            }
        }

        //显示结果
        if (isShow == 'yes') {
            isShow = '30%';
        } else if (isShow == 'no') {
            isShow = `calc(100% - var(--nrc-spliter-width))`;
        }

        if (isHorizontal) {
            domSpliterChild[0].style.width = isShow;
            domSpliterChild[2].style.left = isShow;
            domSpliterChild[3].style.width = `calc(100% - var(--nrc-spliter-width) - ${isShow})`;
        } else {
            domSpliterChild[0].style.height = isShow;
            domSpliterChild[2].style.top = isShow;
            domSpliterChild[3].style.height = `calc(100% - var(--nrc-spliter-width) - ${isShow})`;
        }
    },

    /**
     * 获取分离器大小
     * @param {*} domSpliter 
     */
    getSpliterSize: (domSpliter) => {
        //是水平
        var isHorizontal = domSpliter.classList.contains('nrc-spliter-horizontal');
        if (isHorizontal) {
            var cw = parseFloat(getComputedStyle(domSpliter.firstElementChild).width),
                sw = parseFloat(getComputedStyle(domSpliter).width);
            return (cw / sw * 100).toFixed(2) + '%';
        } else {
            var ch = parseFloat(getComputedStyle(domSpliter.firstElementChild).height),
                sh = parseFloat(getComputedStyle(domSpliter).height);
            return (ch / sh * 100).toFixed(2) + '%';
        }
    },

    /**
     * 配置安全
     * @param {*} configObj 
     * @param {*} type 
     * @returns 
     */
    safeConfig: (configObj, type) => new Promise(resolve => {

        //加密
        let conns = configObj[ndkStorage.keys.keyConns.key] || [];
        let ci = 0;
        conns.forEach(item => {
            ndkFunction[type](item.conn, ndkVary.key).then(conn => {
                ci++;
                item.conn = conn
            }).catch(() => {
                ci++;
            })
        });

        ndkVary.defer.safeConfig = setInterval(() => {
            if (conns.length == ci) {
                clearInterval(ndkVary.defer.safeConfig);

                resolve(configObj);
            }
        }, 100);
    }),

    //自适应大小 (动画结束后调用)
    size: function () {
        var vh = document.documentElement.clientHeight;

        //分离容器高度
        ndkVary.domSpliterBody.style.height = `${vh - ndkVary.domSpliterBody.getBoundingClientRect().top - 5}px`;

        ['Conns', 'Database', 'Table', 'Column'].forEach(vkey => {
            var dom = ndkVary[`domGrid${vkey}`];
            if (getComputedStyle(dom.parentElement).display != "none") {
                var gt = dom.getBoundingClientRect().top + 5;
                dom.style.height = `${vh - gt}px`;
            }
        });

        //选项卡 窗口 执行SQL
        ndkVary.domTabGroupBody.querySelectorAll('.nrg-spliter-sql').forEach(node => {
            var pnode = node.parentElement;
            if (getComputedStyle(pnode).display != "none") {
                var gt = node.getBoundingClientRect().top + 5;
                node.style.height = `${vh - gt}px`;

                //选项卡（SQL执行结果）执行结果
                var tpkey = ndkTab.tabKeys[pnode.name];

                if (tpkey && tpkey.grids) {
                    tpkey.grids.forEach(grid => {
                        if (getComputedStyle(grid.domTabPanel).display != "none") {
                            var tableh = `${vh - grid.domGridExecuteSql.getBoundingClientRect().top - 5}px`;
                            grid.domGridExecuteSql.style.height = tableh;

                            //显示表格
                            if (!grid.gridApi && grid.opsExecuteSql) {
                                nrGrid.buildDom(grid.domGridExecuteSql);
                                let gridApi = nrGrid.createGrid(grid.domGridExecuteSql, grid.opsExecuteSql);

                                //快捷搜索
                                nrApp.setQuickFilter(grid.domFilterExecuteSql, gridApi)

                                grid.gridApi = gridApi;
                            }
                        }
                    })
                }
            }
        });

        //选项卡 窗口 生成代码
        ndkVary.domTabGroupBody.querySelectorAll('.nrg-spliter-code').forEach(node => {
            var pnode = node.parentElement;
            if (getComputedStyle(pnode).display != "none") {
                var gt = node.getBoundingClientRect().top + 5;
                node.style.height = `${vh - gt}px`;

                var tpkey = ndkTab.tabKeys[pnode.name];

                tpkey.domEditor1.style.height = `${vh - gt - tpkey.domTool1.clientHeight}px`;
                tpkey.domEditor2.style.height = `${vh - gt - tpkey.domTool2.clientHeight}px`;
            }
        });
    }
}

export { ndkAction };