import { nrcBase } from "../nrcBase";
import { nrECharts } from "../nrECharts";

// 方法
let nrApp = {
    tsGrid: null, // ag-grid

    /* 加载提示 */
    tsLoadingHtml: '<sl-spinner class="fs-3 m-3"></sl-spinner>',
    /* 失败提示 */
    tsFailHtml: '<sl-button variant="text">网络错误</sl-button>',

    /**
     * 是暗黑主题
     */
    isDark: () => document.documentElement.classList.contains('sl-theme-dark'),

    /**
     * 设置主题
     * @param {any} theme 
     */
    setTheme: async (theme) => {
        let domHtml = document.documentElement;
        ["sl-theme-light", "sl-theme-dark"].forEach(c => domHtml.classList.remove(c));
        domHtml.classList.add(`sl-theme-${theme}`);
        
        nrcBase.saveTheme(theme);
        let isDark = nrcBase.isDark();

        //monaco-editor
        if (window["monaco"]) {
            if (isDark) {
                monaco.editor.setTheme('vs-dark');
            } else {
                monaco.editor.setTheme('vs');
            }
        }

        //netnrmd
        if (nrApp.tsMd) {
            nrApp.tsMd.toggleTheme(isDark ? "dark" : "light");
        }

        //agGrid
        document.querySelectorAll('.nrg-grid').forEach(dom => {
            if (isDark) {
                dom.classList.remove("ag-theme-alpine")
                dom.classList.add("ag-theme-alpine-dark");
            } else {
                dom.classList.remove("ag-theme-alpine-dark")
                dom.classList.add("ag-theme-alpine");
            }
        });

        //echarts
        document.querySelectorAll('.nrg-chart').forEach(dom => {
            nrECharts.setTheme(dom);
        });
    },

    /**
     * 异常
     * @param {any} error 
     * @param {any} tips
     */
    logError: (error, tips) => {
        if (tips != null && window["shoelace"]) {
            nrApp.toast(tips);
        }
        console.debug(error);
    },

    /**
     * 弹层抑制隐藏：点击遮罩层、按 ESC
     * @param {any} domDialog
     */
    dialogSuppressHide: (domDialog) => {
        domDialog.addEventListener('sl-request-close', event => {
            if ("keyboard,overlay".includes(event.detail.source)) {
                event.preventDefault();
            }
        });
    },

    /**
     * 设置元素处于加载状态
     * @param {any} domCard 
     * @param {any} isLoading 
     */
    setLoading: (domCard, isLoading) => {
        domCard.querySelectorAll('sl-button,sl-input,sl-select').forEach(dom => {
            if (dom.nodeName == "SL-BUTTON" && dom.variant != "text") {
                dom.loading = isLoading;
            } else if (dom.nodeName == "SL-SELECT") {
                dom.disabled = isLoading;
            }
            else {
                dom.readonly = isLoading;
            }
        })
    },

    /**
     * 设置本地过滤
     * @param {*} domTxt 
     * @param {*} grid 
     */
    setQuickFilter: (domTxt, grid) => {
        domTxt.addEventListener('input', async function () {
            if (grid && grid.api) {
                grid.api.setQuickFilter(this.value);
            }
        });
        domTxt.addEventListener('sl-clear', async function (e) {
            if (e.target == this) {
                grid.api.setQuickFilter(this.value);
            }
        })
    },

    /**
     * 消息
     * @param {any} message 
     * @param {any} type 
     * @param {any} icon 
     * @param {any} duration 
     * @returns 
     */
    toast: function (message, type = 'primary', icon = 'info-circle', duration = 6000) {
        const alert = Object.assign(document.createElement('sl-alert'), {
            type: type,
            closable: true,
            duration: duration,
            innerHTML: `<sl-icon name="${icon}" slot="icon"></sl-icon><div class="text-break">${message}</div>`
        });

        document.body.append(alert);
        if (alert.toast) {
            return alert.toast();
        }
    },

    /**
     * 提示
     * @param {any} content 内容
     * @param {any} title 标题
     * @param {any} width
     */
    alert: (content, title, width = "50em") => {
        if (nrApp.domAlert == null) {
            nrApp.domAlert = document.createElement('sl-dialog');
            document.body.appendChild(nrApp.domAlert);
        }
        nrApp.domAlert.label = title || "消息提示";

        if (width == "full") {
            width = "98vw";
        }
        nrcBase.cssvar(nrApp.domAlert, '--width', width);

        nrApp.domAlert.innerHTML = '';
        try {
            let code = JSON.stringify(JSON.parse(content), null, 2);
            let dom = document.createElement("pre");
            dom.className = "m-0";
            dom.innerText = code;
            nrApp.domAlert.appendChild(dom);
        } catch (error) {
            nrApp.domAlert.innerHTML = content;
        }

        if (nrApp.domAlert.show) {
            nrApp.domAlert.show();
        }
    },

    /**
     * 确认
     * @param {any} message 提示内容
     * @param {any} title 标题
     * @param {any} width
     */
    confirm: (message, title, width = "50em") => new Promise((resolve) => {
        if (nrApp.domConfirm == null) {
            nrApp.domConfirm = document.createElement('sl-dialog');
            nrApp.domConfirm.innerHTML = `
<sl-button slot="header-actions" class="nrg-confirm-ok" variant="primary" outline size="small">确定</sl-button>
<div class="nrg-confirm-message"></div>`;
            document.body.appendChild(nrApp.domConfirm);
        }
        nrApp.domConfirm.label = title || "确认";
        nrcBase.cssvar(nrApp.domConfirm, '--width', width);
        nrApp.domConfirm.querySelector(`div.nrg-confirm-message`).innerHTML = message;

        //取消
        let cancelEvent = function () {
            nrApp.domConfirm.removeEventListener('sl-request-close', cancelEvent);

            nrApp.domConfirm.hide();
            resolve(false);
        }
        nrApp.domConfirm.addEventListener('sl-request-close', cancelEvent);

        //确定
        nrApp.domConfirm.querySelector(`sl-button.nrg-confirm-ok`).onclick = () => {
            nrApp.domConfirm.hide();
            resolve(true);
        };

        nrApp.domConfirm.show();
    }),
}

export { nrApp };