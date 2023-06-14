import { nrcBase } from "../nrcBase";
import { nrECharts } from "../nrECharts";

let nrApp = {
    tsEditor: null, // monaco-editor 对象
    tsMd: null, // netnrmd 对象
    tsGrid: null, // ag-grid

    /* 加载提示 */
    tsLoadingHtml: '<div class="spinner-border m-3" role="status"><span class="visually-hidden">Loading...</span></div>',
    /* 失败提示 */
    tsFailHtml: '<input type="text" readonly class="form-control-plaintext" value="网络错误">',

    /**
     * 设置主题
     * @param {*} theme
     */
    setTheme: function (theme) {
        let domHtml = document.documentElement;
        let oldTheme = theme == "dark" ? "light" : "dark";
        domHtml.className = domHtml.className.replace(oldTheme, theme);
        domHtml.dataset.bsTheme = theme;

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

        //jstree
        document.querySelectorAll('.nrg-tree').forEach(dom => {
            let tree = dom.tree;
            if (tree) {
                tree.set_theme(isDark ? "default-dark" : "default");
            }
        })
    },

    /**
     * 设置加载
     * @param {*} domBtn Button 节点对象
     * @param {*} isCancel 是否取消
     */
    setLoading: (domBtn, isCancel = false) => {
        domBtn.disabled = !isCancel;

        if (isCancel) {
            domBtn.classList.remove('nrg-loading');
            let domLoadingWait = domBtn.querySelector('.nrg-loading-wait');
            domLoadingWait && domLoadingWait.remove();
        } else {
            if (!domBtn.classList.contains('nrg-loading')) {
                domBtn.classList.add('nrg-loading');

                let domLoadingWait = document.createElement('span');
                domLoadingWait.className = "nrg-loading-wait";
                domLoadingWait.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
                domBtn.appendChild(domLoadingWait);
            }
        }
    },

    /**
     * 异常
     * @param {any} error 
     * @param {any} tips
     */
    logError: (error, tips) => {
        if (tips != null) {
            nrApp.toast(tips);
        }
        console.debug(error);
    },

    /**
     * 消息
     * @param {any} message 
     * @returns 
     */
    toast: (message) => {
        if (nrApp.domToastContainer == null) {
            nrApp.domToastContainer = document.createElement('div');
            nrApp.domToastContainer.className = "toast-container position-fixed bottom-0 end-0 p-4";
            document.body.appendChild(nrApp.domToastContainer);
        }

        let domToast = document.createElement("div");
        domToast.innerHTML = `
<div class="toast-header">
    <img src="/favicon.ico" class="rounded me-2" height="18">
    <strong class="me-auto">消息提示</strong>
    <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
</div>
<div class="toast-body">${message}</div>
`;
        domToast.className = "toast";
        nrApp.domToastContainer.appendChild(domToast);

        //构建
        let bsToast = new bootstrap.Toast(domToast);
        bsToast.show();
        nrApp.domToastContainer["toast"] = bsToast;

        //移除
        domToast.addEventListener('hidden.bs.toast', function (event) {
            event.target.remove();
        })
    },

    /**
     * 提示
     * @param {any} content 内容
     * @param {any} title 标题
     * @param {any} width
     */
    alert: (content, title, width = "60em") => {
        if (nrApp.domAlert == null) {
            nrApp.domAlert = document.createElement('div');
            nrApp.domAlert.innerHTML = `
<div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
    <div class="modal-content">
        <div class="modal-header">
            <h5 class="modal-title"></h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body"></div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">关闭</button>
        </div>
    </div>
</div>
            `;
            nrApp.domAlert.className = "modal";
            document.body.appendChild(nrApp.domAlert);

            nrApp.domAlert["alert"] = new bootstrap.Modal(nrApp.domAlert);
        }
        nrApp.domAlert.querySelector('.modal-title').innerHTML = title || "消息提示";
        nrcBase.cssvar(nrApp.domAlert, '--bs-modal-width', width);

        let domBody = nrApp.domAlert.querySelector('.modal-body');
        domBody.innerHTML = "";

        try {
            if (typeof content == "string") {
                content = JSON.stringify(JSON.parse(content), null, 2);
            } else {
                content = JSON.stringify(content, null, 2);
            }

            let dom = document.createElement("pre");
            dom.className = "m-0 fs-6";
            dom.style.whiteSpace = 'pre-wrap';
            dom.innerText = content;
            
            domBody.appendChild(dom);
        } catch (error) {
            domBody.innerHTML = content;
        }

        nrApp.domAlert["alert"].show();
    },

    /**
     * 确认
     * @param {any} message 提示内容
     * @param {any} title 标题
     * @param {any} width
     */
    confirm: (message, title, width = "30em") => new Promise((resolve) => {
        if (nrApp.domConfirm == null) {
            nrApp.domConfirm = document.createElement('div');
            nrApp.domConfirm.innerHTML = `
<div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
    <div class="modal-content">
        <div class="modal-header">
            <h5 class="modal-title"></h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body"></div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">取消</button>
            <button type="button" class="btn btn-primary flag-ok">确定</button>
        </div>
    </div>
</div>
            `;
            nrApp.domConfirm.className = "modal";
            document.body.appendChild(nrApp.domConfirm);

            nrApp.domConfirm["confirm"] = new bootstrap.Modal(nrApp.domConfirm);
        }
        nrApp.domConfirm.querySelector('.modal-title').innerHTML = title || "消息提示";
        nrcBase.cssvar(nrApp.domConfirm, '--bs-modal-width', width);

        let domBody = nrApp.domConfirm.querySelector('.modal-body');
        domBody.innerHTML = message;

        //取消
        let cancelEvent = function () {
            nrApp.domConfirm.removeEventListener('hidden.bs.modal', cancelEvent);
            resolve(false);
        }
        nrApp.domConfirm.addEventListener('hidden.bs.modal', cancelEvent);

        //确定
        nrApp.domConfirm.querySelector(`.flag-ok`).onclick = () => {
            nrApp.domConfirm.removeEventListener('hidden.bs.modal', cancelEvent);
            nrApp.domConfirm["confirm"].hide();
            resolve(true);
        };

        nrApp.domConfirm["confirm"].show();
    }),
}

export { nrApp }