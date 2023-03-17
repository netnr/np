import { nrApp } from "../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../frame/nrcBase";
import { nrEditor } from "../../frame/nrEditor";
import { nrStorage } from "../../frame/nrStorage";
import { nrToy } from "./nrToy";
import { nrVary } from "./nrVary";

let nrWeb = {
    init: async () => {
        Object.assign(window, { nrcBase, nrStorage, nrApp, nrWeb, nrVary, nrEditor, nrToy });

        //存储
        await nrStorage.init();

        //dom 对象
        nrcBase.readDOM(document.body, 'nrg', nrVary);

        //bind event
        document.body.addEventListener('click', function (event) {
            let target = event.target;
            switch (target.dataset.action) {
                case "theme":
                    nrApp.setTheme(nrcBase.isDark() ? "light" : "dark");
                    break;
                case "save-form":
                    {
                        let domForm = target.parentNode.parentNode.parentNode.querySelector('form');
                        if (domForm) {
                            domForm.requestSubmit();
                        }
                    }
                    break;
            }
        });

        //页面处理包
        let pagePath = location.pathname;
        if (document.getElementById("hid_is_build_html")) {
            pagePath = `/ss${pagePath}`;
        }
        let nrPage = await nrWeb.getPage(pagePath);
        if (nrPage != null) {
            Object.assign(window, { nrPage });
            //处理
            await nrPage.init();

            //调整大小
            let fnResize = nrPage['event_resize'];
            if (nrcBase.type(fnResize).endsWith("Function")) {
                window.addEventListener("resize", async function (event) {
                    let ch = document.documentElement.clientHeight;
                    let cw = document.documentElement.clientWidth;
                    await fnResize(ch, cw, event);
                });
                nrcBase.dispatchEvent('resize')
            }
        }

        //恢复 select 赋值
        document.body.querySelectorAll('select').forEach(item => {
            if (item.dataset.value != null) {
                item.value = item.dataset.value;
            }
        });

        //设置主题
        nrApp.setTheme(nrcBase.isDark() ? "dark" : "light");

        nrToy.init();
    },

    /**
     * 获取脚本包
     * @param {any} packName
     */
    getPack: async (packName) => {
        if (!(packName in window) && !["nrPack_account", "nrPack_guff", "nrPack_draw"].includes(packName)) {
            try {
                window[packName] = (await import(`./pack/${packName}`))[packName];
            } catch (error) {
                nrApp.logError(error, `加载包 ${packName} 失败`);
            }
        }

        return window[packName];
    },

    /**
     * 获取页面脚本
     * @param {any} pathname
     */
    getPage: async (pathname) => {
        let nrPage; //页面脚本对象

        let urlRoutes = pathname.toLowerCase().split('/');
        let ctrl = urlRoutes[1] || "home";
        let action = urlRoutes[2] || "index";
        let id = urlRoutes[3];

        //分包名称，包名小写+下划线
        let packName = `nrPack_${nrcBase.humpToUnderline(ctrl)}`;
        let nrPack = await nrWeb.getPack(packName);

        if (nrPack) {
            for (const packItem of nrPack) {
                let packObj = packItem["nrPage"]; //提取模块输出对象
                if (packObj && packObj.pathname) {
                    let paths = packObj.pathname;
                    if (nrcBase.type(paths) != "Array") {
                        paths = [paths];
                    }
                    let fpath = paths.filter(path => {
                        let scriptRoutes = path.split('/');
                        if (ctrl == scriptRoutes[1] && action == scriptRoutes[2]) {
                            if (id == scriptRoutes[3] || (scriptRoutes[3] == "*" && id != null)) {
                                return true;
                            }
                        }
                        return false;
                    })
                    if (fpath.length) {
                        nrPage = packObj;
                        break;
                    }
                }
            }
        }

        return nrPage;
    },

    /**
     * 请求服务
     * @param {any} url 链接
     * @param {any} options 选项 type: text blob json(default)
     * @returns 
     */
    reqServer: async (url, options) => {
        let vm = await nrcBase.fetch(url, options);

        if (vm.error) {
            return { code: -3, msg: `请求失败：${vm.error.message}`, error: vm.error }
        } else if (vm.resp.ok == false) {
            return { code: vm.resp.status, msg: vm.resp.statusText, resp: vm.resp }
        } else {
            return vm.result;
        }
    },
}

export { nrWeb }