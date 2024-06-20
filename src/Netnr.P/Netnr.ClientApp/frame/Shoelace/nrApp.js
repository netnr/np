import { nrcBase } from "../nrcBase";
import { nrECharts } from "../nrECharts";

// 方法
let nrApp = {
    tsGrid: null, // ag-grid

    /* 加载提示 */
    tsLoadingHtml: '<sl-spinner class="fs-3 m-3"></sl-spinner>',
    /* 失败提示 */
    tsFailHtml: '<sl-button variant="text">网络错误</sl-button>',
    tsHrHtml: '<sl-divider></sl-divider>',

    /**
     * 标签提示
     * @param {*} text 
     * @param {*} size 
     * @returns 
     */
    eleTips: (text, size) => htmlraw`<sl-button variant="text" size="${size || "medium"}">${text}</sl-button>`,

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

        document.documentElement.dataset.bsTheme = isDark ? "dark" : "light";

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

        //uppy
        document.querySelectorAll('.uppy-Dashboard').forEach(dom => {
            dom.dataset.uppyTheme = theme
        });
    },

    /**
     * 全局错误处理
     */
    globalError: () => {
        window.addEventListener('error', function (event) {
            console.debug(event);
            let msg = event.message;

            if (window["shoelace"] || window["nrTranslations"]) {
                if (!msg.startsWith("ResizeObserver")) {
                    nrApp.toast(msg);
                }
            }

            // 上报
            // let obj = { time: nrcBase.now() };
            // ["filename", "lineno", "message"].forEach(x => obj[x] = event[x]);
        });
    },

    /**
     * 异常
     * @param {any} error 
     * @param {any} tips
     */
    logError: (error, tips) => {
        if (tips != null && (window["shoelace"] || window["nrTranslations"])) {
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
        let domArray = [];
        let tagArray = ["sl-button", "sl-input", "sl-select", "sl-range"];
        if (tagArray.includes(domCard.nodeName.toLowerCase())) {
            domArray.push(domCard);
        } else {
            domArray = domCard.querySelectorAll(tagArray.join(","));
        }

        if (isLoading) {
            window.clearTimeout(domCard["to"]);
            domArray.forEach(dom => {
                if (dom.nodeName == "SL-BUTTON" && dom.variant != "text") {
                    dom.loading = isLoading;
                    dom.disabled = isLoading;
                } else if (["SL-SELECT", "SL-RANGE"].includes(dom.nodeName)) {
                    dom.disabled = isLoading;
                }
                else {
                    dom.readonly = isLoading;
                }
            })
        } else {
            domCard["to"] = setTimeout(() => {
                domArray.forEach(dom => {
                    if (dom.nodeName == "SL-BUTTON" && dom.variant != "text") {
                        dom.loading = isLoading;
                        dom.disabled = isLoading;
                    } else if (["SL-SELECT", "SL-RANGE"].includes(dom.nodeName)) {
                        dom.disabled = isLoading;
                    }
                    else {
                        dom.readonly = isLoading;
                    }
                })
            }, 500);
        }
    },

    /**
     * 设置 grid 列处于加载状态
     * @param {*} columnDefs 
     */
    setGridColumnLoading: (columnDefs) => {
        for (let i = 0; i < columnDefs.length; i++) {
            let col = columnDefs[i];
            if (col.field != "#line_number" && col.hide != true) {
                if (col.cellRenderer == null) {
                    col.cellRenderer = (params) => {
                        return params.value === undefined ? '<sl-spinner class="fs-4"></sl-spinner>' : params.value
                    }
                }
                break;
            }
        }
    },

    /**
     * 设置本地过滤
     * @param {*} domTxt 
     * @param {*} gridApi 
     */
    setQuickFilter: (domTxt, gridApi) => {
        domTxt.addEventListener("sl-input", async function (event) {
            if (event.target == this && gridApi) {
                (gridApi.gos.eGridDiv.gridApi || gridApi).updateGridOptions({ quickFilterText: this.value });
                // 模拟引用传递的效果
                // 通过 nrGrid.createGrid 方法创建的对象会在 DOM 记录 gridApi
                // 获取 DOM 存储的 gridApi 对象再调用，解决 gridApi 对象被覆盖后不生效的问题
                // gridApi.updateGridOptions({ quickFilterText: this.value })
            }
        });
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
            innerHTML: `<sl-icon name="${icon}" slot="icon"></sl-icon><div class="text-break">${nrcBase.xssOf(message)}</div>`
        });

        document.body.append(alert);
        if (alert.toast) {
            return alert.toast();
        }
    },

    domAlert: null,
    /**
     * 提示
     * @param {any} content 内容
     * @param {any} title 标题
     * @param {any} width
     */
    alert: (content, title, width = "60em") => {
        if (nrApp.domAlert == null) {
            nrApp.domAlert = document.createElement('sl-dialog');
            nrApp.domAlert.classList.add("work-break-all");
            document.body.appendChild(nrApp.domAlert);
        }
        nrApp.domAlert.label = title || "消息提示";

        if (width == "full") {
            width = "98vw";
        }
        nrcBase.cssvar(nrApp.domAlert, '--width', width);

        nrApp.domAlert.innerHTML = '';
        try {
            if (typeof content == "string") {
                content = JSON.stringify(JSON.parse(content), null, 2);
            } else {
                content = JSON.stringify(content, null, 2);
            }

            let dom = document.createElement("pre");
            dom.className = "m-0 fs-6";
            dom.style.whiteSpace = 'pre-wrap';
            dom.textContent = content;

            nrApp.domAlert.appendChild(dom);
        } catch (error) {
            nrApp.domAlert.innerHTML = nrcBase.xssOf(content);
        }

        if (nrApp.domAlert.show) {
            nrApp.domAlert.show();
        }
    },

    domConfirm: null,
    /**
     * 确认
     * @param {any} message 提示内容
     * @param {any} title 标题
     * @param {any} width
     */
    confirm: (message, title, width = "60em") => new Promise((resolve) => {
        if (nrApp.domConfirm == null) {
            nrApp.domConfirm = document.createElement('sl-dialog');
            nrApp.domConfirm.innerHTML = `
<sl-button slot="header-actions" class="nrg-confirm-ok" variant="primary" outline size="small">确定</sl-button>
<div class="nrg-confirm-message"></div>`;
            document.body.appendChild(nrApp.domConfirm);
        }
        nrApp.domConfirm.label = title || "确认";
        nrcBase.cssvar(nrApp.domConfirm, '--width', width);
        nrApp.domConfirm.querySelector(`div.nrg-confirm-message`).innerHTML = nrcBase.xssOf(message);

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
Object.assign(window, { nrApp });
export { nrApp };