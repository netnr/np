import { me } from "./me";
import { sqlFor } from './sqlFor';
import { step } from "./step";
import { tab } from "./tab";
import { vary } from './vary';

var fn = {

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
    msg: function (message, type = 'warning', icon = 'exclamation-triangle', duration = 9000) {

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
        if (vary.domConfirm == null) {
            vary.domConfirm = document.createElement('sl-dialog');
            document.body.appendChild(vary.domConfirm);
            vary.domConfirm.addEventListener('click', function (e) {
                if (e.target.type == "primary") {
                    vary.domConfirm.hide();
                    ok && ok();
                } else if (e.target.type == "default") {
                    vary.domConfirm.hide()
                }
            }, false)
        }
        vary.domConfirm.innerHTML = `${tip}
        <sl-button slot="footer" type="default">取消</sl-button>
        <sl-button slot="footer" type="primary">确定</sl-button>`;
        vary.domConfirm.show();
    },

    /**
     * 请求状态
     * @param {*} isShow 
     */
    requestStatus: function (isShow = true) {
        vary.domRequestStatus.loading = isShow;
    },

    /**
     * 新的连接ID
     * @param {*} s
     * @param {*} e 
     * @returns 
     */
    random: function (s = 20000, e = 99999) { return Math.floor(Math.random() * (e - s + 1) + s) },

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
                    vary.theme = cmd.split('-').pop();
                    fn.themeGrid(vary.theme);
                    fn.themeEditor(vary.theme);
                    if (vary.theme == "dark") {
                        document.querySelector('sl-menu-item[data-cmd="theme-light"]').checked = false;
                        document.querySelector('sl-menu-item[data-cmd="theme-dark"]').checked = true;
                        document.documentElement.classList.add('sl-theme-dark');
                    } else {
                        document.querySelector('sl-menu-item[data-cmd="theme-light"]').checked = true;
                        document.querySelector('sl-menu-item[data-cmd="theme-dark"]').checked = false;
                        document.documentElement.classList.remove('sl-theme-dark');
                    }
                    step.stepSave();
                }
                break;
            //分离器1大小
            case "box1-size":
                {
                    fn.cssvar(vary.domMain, '--box1-width', args);
                    step.stepSave();
                }
                break;
            //执行选中脚本
            //执行全部脚本
            case "sql-execute-selected":
            case "sql-execute-all":
                {
                    var tpkey = args.getAttribute('panel');
                    var tpobj = tab.tabKeys[tpkey];

                    var isSelected = cmd.includes("selected");
                    tab.tabEditorExecuteSql(tpkey, isSelected)
                }
                break;
            //格式化SQL
            case "sql-formatting":
                {
                    var tpkey = args.getAttribute('panel');
                    var tpobj = tab.tabKeys[tpkey];

                    me.meFormatter(tpobj.editor);
                }
                break
            //常用脚本
            case "sql-common":
                {
                    var tpkey = args.getAttribute('panel');
                    var cp = step.cpGet(tpkey);
                    db.viewExecuteSql({ Item1: { "sql-common": sqlFor[`sqlFor${cp.cobj.type}`] } }, tpkey)
                }
                break
        }
    },

    /**
     * 设置主题
     * @param {any} theme
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

        for (const key in vary) {
            if (key.startsWith("domGrid")) {
                var dom = vary[key];
                setTheme(dom, theme);
            }
        }

        //选项卡2 表格
        vary.domTabGroup2.querySelectorAll('.nr-grid-execute-sql').forEach(dom => {
            setTheme(dom, theme);
        })

        //选项卡3 表格
        for (var i in tab.tabKeys) {
            var tpkey = tab.tabKeys[i];
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

    //自适应大小
    size: function () {
        var vh = document.documentElement.clientHeight;

        //分离容器高度
        vary.domSpliter1.style.height = `${vh - vary.domSpliter1.getBoundingClientRect().top - 5}px`;

        ['Conns', 'Database', 'Table', 'Column'].forEach(vkey => {
            var dom = vary[`domGrid${vkey}`];
            if (getComputedStyle(dom.parentElement).display != "none") {
                var gt = dom.getBoundingClientRect().top + 5;
                dom.style.height = `${vh - gt}px`;
            }
        });

        //选项卡2
        vary.domTabGroup2.querySelectorAll('.nr-spliter2').forEach(node => {
            var pnode = node.parentElement;
            if (getComputedStyle(pnode).display != "none") {
                var gt = node.getBoundingClientRect().top + 5;
                node.style.height = `${vh - gt}px`;

                //选项卡3
                var tpkey = tab.tabKeys[pnode.name];
                if (tpkey && tpkey.grids) {
                    setTimeout(() => {
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
                    }, 200)
                }
            }
        });
    }
}

export { fn };