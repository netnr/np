import { ndkEditor } from "./ndkEditor";
import { ndkStep } from "./ndkStep";
import { ndkTab } from "./ndkTab";
import { ndkVary } from './ndkVary';
import { ndkDb } from './ndkDb';
import { ndkLs } from "./ndkLs";
import { ndkSqlNote } from "./ndkSqlNote";

var ndkFn = {

    /**
     * 首字母大写
     * @param {*} c 
     * @returns 
     */
    fu: c => c.substring(0, 1).toUpperCase() + c.substring(1),

    /**
     * 获取或设置CSS变量
     * @param {any} dom
     * @param {any} k
     * @param {any} v
     */
    cssvar: (dom, k, v) => {
        if (v == null) {
            return getComputedStyle(dom).getPropertyValue(k);
        } else {
            dom.style.setProperty(k, v);
        }
    },

    /**
     * 类型判断
     * @param {*} obj 
     * @returns 
     */
    type: function (obj) {
        var tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },

    /**
     * 数组去重
     * @param {any} arr
     */
    arrayDistinct: arr => Array.from(new Set(arr)),

    /**
     * 可视化大小
     * @param {*} size 
     * @param {*} keep 
     * @param {*} rate 
     * @returns 
     */
    formatByteSize: function (size, keep = 2, rate = 1024) {
        if (Math.abs(size) < rate) {
            return size + ' B';
        }

        const units = rate == 1000 ? ['KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'] : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
        let u = -1;
        const r = 10 ** keep;

        do {
            size /= rate;
            ++u;
        } while (Math.round(Math.abs(size) * r) / r >= rate && u < units.length - 1);

        return size.toFixed(keep) + ' ' + units[u];
    },

    /**
     * 分组
     * @param {*} array 
     * @param {*} f 
     * @returns 
     */
    groupBy: function (array, f) {
        var groups = {};
        array.forEach(o => {
            var key = f(o);
            groups[key] = groups[key] || [];
            groups[key].push(o);
        });
        return Object.keys(groups);
    },

    /**
     * 时间格式化
     * @param {any} fdt
     * @param {any} date
     */
    formatDateTime: (fdt, date) => {
        date = date || Date.now();
        var options = {
            hour12: false, timeZone: 'Asia/Shanghai'
        };
        switch (fdt) {
            case "date":
                options = Object.assign({
                    year: 'numeric', month: '2-digit', day: '2-digit'
                }, options)
                break;
            case "time":
                options = Object.assign({
                    hour: 'numeric', minute: '2-digit', second: '2-digit'
                }, options)
                break;
            case "year":
                return new Date(date).getFullYear();
            case "datetime":
            default:
                options = Object.assign({
                    year: 'numeric', month: '2-digit', day: '2-digit',
                    hour: 'numeric', minute: '2-digit', second: '2-digit'
                }, options)
                break;
        }
        return new Intl.DateTimeFormat('zh-CN', options).format(date);
    },

    /**
     * 消息
     * @param {*} message 
     * @param {*} type 
     * @param {*} icon 
     * @param {*} duration 
     * @returns 
     */
    msg: function (message, type = 'primary', icon = 'info-circle', duration = 8000) {
        const alert = Object.assign(document.createElement('sl-alert'), {
            type: type,
            closable: true,
            duration: duration,
            innerHTML: `
          <sl-icon name="${icon}" slot="icon"></sl-icon>
          ${message}
        `
        });

        document.body.append(alert);
        return alert.toast();
    },

    /**
     * 询问
     * @param {*} tip 提示
     * @param {*} ok 确定
     */
    confirm: (tip, ok) => {
        if (ndkVary.domConfirm == null) {
            ndkVary.domConfirm = document.createElement('sl-dialog');
            document.body.appendChild(ndkVary.domConfirm);
            ndkVary.domConfirm.addEventListener('click', function (e) {
                if (e.target.type == "primary") {
                    ndkVary.domConfirm.hide();
                    ok && ok();
                } else if (e.target.type == "default") {
                    ndkVary.domConfirm.hide()
                }
            }, false)
        }
        ndkVary.domConfirm.innerHTML = `${tip}
        <sl-button slot="footer" type="default">取消</sl-button>
        <sl-button slot="footer" type="primary">确定</sl-button>`;
        ndkVary.domConfirm.show();
    },

    /**
     * 请求状态
     * @param {*} isShow 
     */
    requestStatus: (isShow = true) => {
        ndkVary.domRequestStatus.loading = isShow;
        isShow ? ndkFn.requestStatusTitle.run() : ndkFn.requestStatusTitle.stop();
    },
    requestStatusTitle: {
        title: document.title,
        si: null,
        run: (index = 1, dir = true) => {
            var arr = [], icon = "🛑";
            for (var i = 0; i < index; i++) {
                arr.push(icon);
            }
            document.title = " " + arr.join(" ");
            dir ? index++ : index--;
            dir = index == 9 ? false : dir;
            dir = index == 1 ? true : dir;

            ndkFn.requestStatusTitle.si = setTimeout(() => {
                ndkFn.requestStatusTitle.run(index, dir);
            }, 600);
        },
        stop: () => {
            clearTimeout(ndkFn.requestStatusTitle.si);
            document.title = ndkFn.requestStatusTitle.title;
        }
    },

    /**
     * 新的连接ID
     * @param {*} s
     * @param {*} e 
     * @returns 
     */
    random: (s = 20000, e = 99999) => Math.floor(Math.random() * (e - s + 1) + s),

    /**
     * 下载
     * @param {any} content
     * @param {any} filename
     */
    download: function (content, filename) {
        var aTag = document.createElement('a');
        var blob = new Blob([content]);
        aTag.download = filename;
        aTag.href = URL.createObjectURL(blob);
        document.body.appendChild(aTag);
        aTag.click();
        URL.revokeObjectURL(blob);
        aTag.remove();
    },

    /**
     * 动作命令
     * @param {*} cmd
     * @param {*} args
     */
    actionRun: function (cmd, args) {
        switch (cmd) {
            //主题
            case "theme-light":
            case "theme-dark":
                {
                    ndkVary.theme = cmd.split('-').pop();
                    ndkFn.themeGrid(ndkVary.theme);
                    ndkFn.themeEditor(ndkVary.theme);
                    if (ndkVary.theme == "dark") {
                        document.querySelector('sl-menu-item[data-cmd="theme-light"]').checked = false;
                        document.querySelector('sl-menu-item[data-cmd="theme-dark"]').checked = true;
                        document.documentElement.classList.add('sl-theme-dark');
                    } else {
                        document.querySelector('sl-menu-item[data-cmd="theme-light"]').checked = true;
                        document.querySelector('sl-menu-item[data-cmd="theme-dark"]').checked = false;
                        document.documentElement.classList.remove('sl-theme-dark');
                    }
                    ndkStep.stepSave();
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
                        const item = ndkVary.parameterConfig[key];
                        switch (item.type) {
                            case "number":
                                pchtm.push(`<div><sl-input name="${key}" label="${item.label}" type="${item.type}" required value="${item.value}"></sl-input></div>`);
                                break;
                            case "select":
                            case "boolean":
                                {
                                    pchtm.push(`<div><sl-select name="${key}" label="${item.label}" value="${item.value}" hoist="true">`);
                                    item.list.forEach(obj => {
                                        pchtm.push(`<sl-menu-item value="${obj.val}">${obj.txt}</sl-menu-item>`);
                                    })
                                    pchtm.push(`</sl-select></div>`);
                                }
                                break;
                        }
                    }
                    pchtm.push(`<div><sl-button type="default" outline>${ndkVary.icons.success}保存</sl-button></div>`);
                    ndkVary.domParameterConfig.innerHTML = pchtm.join('');

                    if (ndkVary.domDialogSetting.getAttribute('event-bind') != 1) {
                        ndkVary.domDialogSetting.setAttribute('event-bind', 1);

                        //接口服务
                        ndkVary.domTextApiServer.addEventListener('input', function () {
                            ndkVary.apiServer = this.value;
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
                                ndkFn.msg("保存成功");
                            }
                        }, false);
                    }
                }
                break;
            //设置接口服务
            case "set-api-server":
                {
                    var val = args.innerHTML;
                    switch (val) {
                        case "当前":
                            val = location.origin;
                            break;
                    }
                    ndkVary.domTextApiServer.value = ndkVary.apiServer = val;
                    ndkStep.stepSave();
                }
                break;
            //导出配置
            //导出配置到剪贴板
            case "config-export-file":
            case "config-export-clipboard":
                {
                    ndkLs.storeConfig.keys().then(keys => {
                        var parr = [], configObj = {};
                        keys.forEach(key => parr.push(ndkLs.storeConfig.getItem(key)))
                        Promise.all(parr).then(arr => {
                            for (var i = 0; i < arr.length; i++) {
                                configObj[keys[i]] = arr[i];
                            }
                            if (cmd.endsWith("clipboard")) {
                                var content = JSON.stringify(configObj, null, 4);
                                navigator.clipboard.writeText(content).then(() => {
                                    ndkFn.msg("导出完成")
                                })
                            } else {
                                var content = JSON.stringify(configObj);
                                ndkFn.download(content, "ndk.json")
                            }
                        })
                    })
                }
                break;
            //导入配置
            case "config-import":
                {
                    var content = args.previousElementSibling.value;
                    try {
                        if (content.trim() != "") {
                            var configObj = JSON.parse(content), parr = [];
                            for (let key in configObj) {
                                let val = configObj[key];
                                parr.push(ndkLs.storeConfig.setItem(key, val));
                            }
                            Promise.all(parr).then(() => {
                                ndkFn.msg("导入完成");
                                location.reload(false);
                            })
                        } else {
                            ndkFn.msg("配置内容不能为空");
                        }
                    } catch (e) {
                        console.debug(e)
                        ndkFn.msg(e)
                    }
                }
                break;
                break;
            //分离器1大小
            case "box1-size":
                {
                    ndkFn.cssvar(ndkVary.domMain, '--box1-width', args);
                    ndkStep.stepSave();
                }
                break;
            //执行选中脚本
            //执行全部脚本
            case "sql-execute-selected":
            case "sql-execute-all":
                {
                    var tpkey = args.getAttribute('panel');
                    var tpobj = ndkTab.tabKeys[tpkey];

                    var isSelected = cmd.includes("selected");
                    ndkTab.tabEditorExecuteSql(tpkey, isSelected)
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
            //SQL 笔记
            case "sql-note":
                {
                    var tpkey = args.getAttribute('panel');
                    var cp = ndkStep.cpGet(tpkey);
                    ndkDb.viewExecuteSql({ Item1: { "sql-note": ndkSqlNote[cp.cobj.type] } }, tpkey)
                }
                break
        }
    },

    /**
     * 设置主题
     * @param {*} theme 
     */
    themeGrid: function (theme) {

        var setTheme = (dom, theme) => {
            dom.classList.remove('ag-theme-alpine');
            dom.classList.remove('ag-theme-alpine-dark');
            switch (theme) {
                case "dark":
                    dom.classList.add("ag-theme-alpine-dark");
                    break;
                default:
                    dom.classList.add("ag-theme-alpine");
            }
        }

        for (const key in ndkVary) {
            if (key.startsWith("domGrid")) {
                var dom = ndkVary[key];
                setTheme(dom, theme);
            }
        }

        //选项卡2 表格
        ndkVary.domTabGroup2.querySelectorAll('.nr-grid-execute-sql').forEach(dom => {
            setTheme(dom, theme);
        })

        //选项卡3 表格
        for (var i in ndkTab.tabKeys) {
            var tpkey = ndkTab.tabKeys[i];
            if (tpkey.grids) {
                tpkey.grids.forEach(grid => {
                    var dom = grid.domGridExecuteSql;
                    setTheme(dom, theme);
                })
            }
        }
    },

    /**
     * 设置主题
     * @param {any} theme
     */
    themeEditor: function (theme) {
        if (window.monaco) {
            switch (theme) {
                case "dark":
                    monaco.editor.setTheme("vs-dark");
                    break;
                default:
                    monaco.editor.setTheme("vs");
            }
        }
    },

    //自适应大小 (动画结束后调用)
    size: function () {
        var vh = document.documentElement.clientHeight;

        //分离容器高度
        ndkVary.domSpliter1.style.height = `${vh - ndkVary.domSpliter1.getBoundingClientRect().top - 5}px`;

        ['Conns', 'Database', 'Table', 'Column'].forEach(vkey => {
            var dom = ndkVary[`domGrid${vkey}`];
            if (getComputedStyle(dom.parentElement).display != "none") {
                var gt = dom.getBoundingClientRect().top + 5;
                dom.style.height = `${vh - gt}px`;
            }
        });

        //选项卡2
        ndkVary.domTabGroup2.querySelectorAll('.nr-spliter2').forEach(node => {
            var pnode = node.parentElement;
            if (getComputedStyle(pnode).display != "none") {
                var gt = node.getBoundingClientRect().top + 5;
                node.style.height = `${vh - gt}px`;

                //选项卡3
                var tpkey = ndkTab.tabKeys[pnode.name];
                if (tpkey && tpkey.grids) {
                    tpkey.grids.forEach(grid => {
                        if (getComputedStyle(grid.domTabPanel).display != "none") {
                            var tableh = `${vh - grid.domGridExecuteSql.getBoundingClientRect().top - 5}px`;
                            grid.domGridExecuteSql.style.height = tableh;

                            //显示表格
                            if (!grid.gridOps && grid.opsExecuteSql) {
                                var gridOps = new agGrid.Grid(grid.domGridExecuteSql, grid.opsExecuteSql).gridOptions;

                                //快捷搜索
                                grid.domFilterExecuteSql.addEventListener('input', function () {
                                    gridOps.api.setQuickFilter(this.value);
                                }, false);

                                grid.gridOps = gridOps;
                            }
                        }
                    })
                }
            }
        });
    }
}

export { ndkFn };