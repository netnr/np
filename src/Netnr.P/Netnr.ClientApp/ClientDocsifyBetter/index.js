import './css/bootstrap-reboot.css';
import './css/vue.css';
import './css/toc.css';

import '../ClientMD/css/nmd-hljs.css';
import '../ClientMD/css/nmd-markdown.css';
import '../ClientMD/css/nmd-toc.css';

import tocbot from 'tocbot';
import hljs from 'highlight.js/lib/common';
import 'docsify/lib/docsify';

let nrcDocsify = {
    init: () => {
        document.querySelector('.app-nav').style.removeProperty('display');
        document.body.removeAttribute('style')

        nrcDocsify.setThemeIcon(nrcDocsify.isDark() ? "dark" : "light");
    },

    /**
     * 是暗黑主题
     * @returns 
     */
    isDark: () => document.documentElement.dataset.bsTheme == 'dark',

    /**
     * 设置主题
     * @param {*} theme 
     */
    setTheme: (theme) => {
        let domHtml = document.documentElement;
        let oldTheme = theme == "dark" ? "light" : "dark";
        domHtml.className = domHtml.className.replace(oldTheme, theme);
        domHtml.dataset.bsTheme = theme;

        let d = new Date();
        d.setFullYear(d.getFullYear() + 1);
        document.cookie = `.theme=${theme};path=/;expires=${d.toGMTString()}`;

        nrcDocsify.setThemeIcon(theme);
    },

    /**
     * 设置主题图标
     * @param {*} theme 
     */
    setThemeIcon: (theme) => {
        let domTheme = document.querySelector(".flag-theme");
        if (domTheme) {
            let pathd = theme == "dark"
                ? "M8 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zM8 0a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 0zm0 13a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 13zm8-5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2a.5.5 0 0 1 .5.5zM3 8a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2A.5.5 0 0 1 3 8zm10.657-5.657a.5.5 0 0 1 0 .707l-1.414 1.415a.5.5 0 1 1-.707-.708l1.414-1.414a.5.5 0 0 1 .707 0zm-9.193 9.193a.5.5 0 0 1 0 .707L3.05 13.657a.5.5 0 0 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0zm9.193 2.121a.5.5 0 0 1-.707 0l-1.414-1.414a.5.5 0 0 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707zM4.464 4.465a.5.5 0 0 1-.707 0L2.343 3.05a.5.5 0 1 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .708z"
                : "M6 .278a.768.768 0 0 1 .08.858 7.208 7.208 0 0 0-.878 3.46c0 4.021 3.278 7.277 7.318 7.277.527 0 1.04-.055 1.533-.16a.787.787 0 0 1 .81.316.733.733 0 0 1-.031.893A8.349 8.349 0 0 1 8.344 16C3.734 16 0 12.286 0 7.71 0 4.266 2.114 1.312 5.124.06A.752.752 0 0 1 6 .278zM4.858 1.311A7.269 7.269 0 0 0 1.025 7.71c0 4.02 3.279 7.276 7.319 7.276a7.316 7.316 0 0 0 5.205-2.162c-.337.042-.68.063-1.029.063-4.61 0-8.343-3.714-8.343-8.29 0-1.167.242-2.278.681-3.286z";
            domTheme.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" fill="currentColor" viewBox="0 0 16 16"><path d="${pathd}"/></svg>`

            if (!domTheme.dataset.bind) {
                domTheme.dataset.bind = true;
                domTheme.addEventListener('click', function () {
                    nrcDocsify.setTheme(nrcDocsify.isDark() ? "light" : "dark");
                })
            }
        }
    },

    tsLoaded: {},
    /**
     * 引入远程脚本
     * @param {*} src 
     * @param {*} type 
     * @returns 
     */
    importScript: async (src, type) => {
        let pout = nrcDocsify.tsLoaded[src];
        if (!pout) {
            nrcDocsify.tsLoaded[src] = pout = new Promise((resolve, reject) => {

                let ds = document.scripts;
                let domSrc;
                for (let index = 0; index < ds.length; index++) {
                    let si = ds[index];
                    if (si.src.includes(src)) {
                        domSrc = si;
                        break;
                    }
                }

                if (domSrc) {
                    resolve();
                } else {
                    let ele = document.createElement("SCRIPT");
                    ele.src = src;
                    ele.type = type || "text/javascript";
                    ele.onerror = function (ex) {
                        reject(ex)
                    }
                    ele.onload = function () {
                        resolve();
                    }

                    document.head.appendChild(ele);
                }
            });
        }
        return pout;
    },
}

Object.assign(window, { tocbot, hljs, nrcDocsify });

document.readyState == "loading" ? document.addEventListener("DOMContentLoaded", nrcDocsify.init) : nrcDocsify.init();