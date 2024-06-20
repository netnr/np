import { nrApp } from "../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../frame/nrcBase";
import { nrRouter } from "../../frame/nrRouter";
import { nrStorage } from "../../frame/nrStorage";
import { nrVary } from "./nrVary";

let nrWeb = {
    init: async () => {
        console.log('https://www.netnr.com\r\nhttps://github.com/netnr');

        Object.assign(window, { nrWeb, nrVary });

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
        let pathArray = pathname.toLowerCase().split('/');

        let routerController = pathArray[1] || "home";
        let routerAction = pathArray[2] || "index";
        let routerId = pathArray[3] || null;

        let packName = `nrPack_${nrcBase.humpToUnderline(routerController)}`;

        //页面脚本对象
        let nrPage = await nrRouter.findPage(pathname, {
            pack: await nrWeb.getPack(packName),
            routerController, routerAction, routerId,
        });

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
            return { code: vm.resp.status, msg: vm.resp.statusText || `请求失败：HTTP ${vm.resp.status}`, resp: vm.resp }
        } else {
            return vm.result;
        }
    },
}

export { nrWeb }