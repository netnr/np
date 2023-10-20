import { nrcFile } from "../../frame/nrcFile";
import { nrcBase } from "../../frame/nrcBase";
import { nrStorage } from "../../frame/nrStorage";
import { nrcIndexedDB } from "../../frame/nrcIndexedDB";
import { nrApp } from "../../frame/Bootstrap/nrApp";
import { nrVary } from "./nrVary";

// 方法
let nrWeb = {
    init: async () => {
        //注册 sw
        let packageName = "netnr";
        if (isSecureContext && !window[`webpackHotUpdate${packageName}`]) {
            navigator.serviceWorker.register('/sw.js')
                .then(reg => console.debug('SW registered: ', reg))
                .catch(ex => console.debug('SW failed: ', ex));
        }

        //主题
        nrApp.setTheme(nrcBase.cookie(".theme") || "light");

        //资源依赖
        await import('bootstrap/dist/css/bootstrap.css');
        const bootstrap = await import("bootstrap/dist/js/bootstrap.bundle.js");
        Object.assign(window, { bootstrap, nrWeb, nrVary });

        //存储初始化
        nrStorage.localforage = await new nrcIndexedDB({ name: "nr-cache" }).init();
        //用户实例
        nrStorage.instanceUser = await new nrcIndexedDB({ name: "nr-user" }).init();

        //渲染
        await nrWeb.render();

        //事件
        nrWeb.bindEvent();

        //仓库路径
        let pns = (location.hash.length > 1 ? location.hash : location.pathname).substring(1).split('/');
        if (pns[0] != "") {
            nrVary.flagName = pns[0];
        }
        if (pns[1] != null && pns[1] != "") {
            nrVary.flagResp = pns[1];
        }
        if (pns[2] != null && pns[2] != "") {
            nrVary.flagLibs = pns[2];
        }

        //指定本地
        if (location.hash == '#_local') {
            nrVary.flagLocalUsed = true;
            await nrStorage.setItem('local', nrVary.flagLocalUsed);
        }
        //local
        let localUsed = await nrStorage.getItem('local');
        if (localUsed === true) {
            nrVary.flagLocalUsed = localUsed;
            document.querySelector('[data-action="local"]').classList.add('active');
        }

        //token
        let token = await nrStorage.getItem('uuid-token-github');
        if (token != null && token.length > 10) {
            nrVary.flagToken = token;
            document.querySelector('[data-action="token"]').classList.add('active');
        }

        //proxy
        let proxyUsed = await nrStorage.getItem('proxy');
        if (proxyUsed != null) {
            nrVary.flagProxyUsed = proxyUsed;
        }
        if (nrVary.flagProxyUsed) {
            document.querySelector('[data-action="proxy"]').classList.add('active');
        }

        //关闭提示        
        document.getElementById('style0').remove();
        nrVary.domLayout.classList.remove('invisible');

        await nrWeb.load();
    },

    render: async () => {
        let domLayout = document.createElement("div");
        nrVary.domLayout = domLayout;
        domLayout.className = "invisible";

        domLayout.innerHTML = `
<div class="container-fluid p-lg-4 pb-lg-0 py-4">
    <div class="row">

        <div class="col-auto mb-2">
            <div class="dropdown">
                <button class="btn btn-outline-success dropdown-toggle" data-bs-toggle="dropdown">
                    <img class="nrg-img-avatar rounded" style="height:1.3em;width:1.3em;" src="/favicon.ico" >
                </button>
                <ul class="nrg-dd-user-info dropdown-menu"></ul>
            </div>
        </div>

        <div class="col mb-2">
            <input class="nrg-txt-filter form-control" type="search" placeholder="silent search.." data-search="" title="silent search.." />
        </div>

        <div class="col-auto mb-2">
            <div class="btn-group">
                <button class="nrg-btn-reload btn btn-outline-success" data-action="reload" title="重新加载">Reload</button>
                <button type="button" class="btn btn-outline-success dropdown-toggle dropdown-toggle-split" data-bs-toggle="dropdown">
                    <span class="visually-hidden">Toggle Dropdown</span>
                </button>
                <ul class="nrg-dd-more dropdown-menu">
                    <li><button class="dropdown-item" data-action="theme">主题 Theme</button></li>
                    <li><hr class="dropdown-divider"></li>
                    <li><button class="dropdown-item" data-action="local" title="私有化部署 Privatization deployment">本地 Local</button></li>
                    <li><button class="dropdown-item" data-action="token">设置 Token</button></li>
                    <li><button class="dropdown-item" data-action="proxy" title="使用代理 use proxy">代理 Proxy</button></li>
                    <li><hr class="dropdown-divider"></li>
                    <li><button class="dropdown-item" data-action="convert" title="转换浏览器导出的 HTML 书签">转换 Convert</button></li>
                    <li><button class="dropdown-item" data-action="about">关于 About</button></li>
                </ul>
            </div>
        </div>
        <div class="col-12 position-relative">
            <div class="nrg-search border rounded d-none position-absolute"></div>
        </div>        
    </div>
    <div class="nrg-view row my-3"></div>
</div>
`;
        document.body.appendChild(domLayout);

        nrcBase.readDOM(document.body, "nrg", nrVary);
    },

    load: async () => {
        nrApp.setLoading(nrVary.domBtnReload)

        await nrWeb.viewUser();
        await nrWeb.viewLink();

        nrApp.setLoading(nrVary.domBtnReload, true);

        //自动刷新缓存
        try {
            let updateTime = await nrStorage.instanceUser.getItem(`${nrVary.flagName}:update-time`);
            if (updateTime && Date.now() - updateTime > 1000 * 3600 * 24 * 7) {
                await nrWeb.reqUser(nrVary.flagName, true);
                await nrWeb.reqLibs(nrVary.flagName, nrVary.flagResp, nrVary.flagLibs, true);
            }
        } catch (error) { }
    },

    /**
     * 事件
     */
    bindEvent: () => {
        //全局点击
        document.body.addEventListener('click', async function (e) {
            let target = e.target;

            if (target.dataset.action) {
                //data-action
                nrWeb.triggerAction(target.dataset.action, target);
            }
        });

        //搜索
        nrVary.domTxtFilter.addEventListener('keydown', function (event) {
            if (["ArrowUp", "ArrowDown"].includes(event.code)) {
                event.preventDefault();
                nrWeb.searchArrow(event.code);
            } else if (event.code == "Enter") {
                event.preventDefault();
                let domActive = nrVary.domSearch.querySelector('a.active');
                if (domActive) {
                    domActive.click();
                }
            }
        })
        nrVary.domTxtFilter.addEventListener('input', function (event) {
            nrWeb.searchLink(this.value.trim());
        });

        //全局按键
        document.body.addEventListener("keydown", function (event) {
            if (document.activeElement.nodeName != "INPUT") {
                //ctrl + q/k search
                if (event.ctrlKey && ["KeyQ", "KeyK"].includes(event.code)) {
                    event.preventDefault();
                    nrVary.domTxtFilter.focus();
                    nrVary.domTxtFilter.dataset.search = "";
                    nrVary.domTxtFilter.placeholder = nrVary.domTxtFilter.title;
                    nrWeb.searchLink("");
                } else if (event.code == "Backspace") {
                    //删除
                    event.preventDefault();
                    if (nrVary.domTxtFilter.dataset.search.length) {
                        nrVary.domTxtFilter.dataset.search = nrVary.domTxtFilter.dataset.search.slice(0, -1)
                    }
                    nrVary.domTxtFilter.placeholder = nrVary.domTxtFilter.dataset.search.length
                        ? nrVary.domTxtFilter.dataset.search
                        : nrVary.domTxtFilter.title;
                    nrWeb.searchLink(nrVary.domTxtFilter.dataset.search);
                } else if (event.code == "Enter") {
                    //确定
                    event.preventDefault();
                    if (!nrVary.domSearch.classList.contains('d-none')) {
                        let domActive = nrVary.domSearch.querySelector('a.active');
                        if (domActive) {
                            domActive.click();
                        }
                    }
                } else if (event.code == "Escape") {
                    //取消
                    event.preventDefault();
                    if (nrVary.domTxtFilter.dataset.search.length) {
                        nrVary.domTxtFilter.dataset.search = "";
                        nrVary.domTxtFilter.placeholder = nrVary.domTxtFilter.title;
                        nrWeb.searchLink("");
                    }
                } else if (["ArrowUp", "ArrowDown"].includes(event.code)) {
                    //上 下
                    event.preventDefault();
                    if (nrVary.domTxtFilter.dataset.search.length) {
                        nrWeb.searchArrow(event.code);
                    }
                } else if (/^[a-z0-9\.\_\-\/]$/i.test(event.key) && !event.ctrlKey) {
                    nrVary.domTxtFilter.value = "";
                    document.documentElement.scrollTo(0, 0);

                    nrVary.domTxtFilter.dataset.search += event.key;
                    nrVary.domTxtFilter.placeholder = nrVary.domTxtFilter.dataset.search;
                    nrWeb.searchLink(nrVary.domTxtFilter.dataset.search);
                }
            }
        })
    },

    /**
     * 动作
     * @param {any} cmd 
     * @param {any} args 
     */
    triggerAction: async (cmd, args) => {
        switch (cmd) {
            case "theme":
                {
                    //切换主题
                    nrApp.setTheme(nrcBase.isDark() ? "light" : "dark");
                }
                break;
            case "token":
                {
                    //设置 Token
                    if (nrVary.domDialogToken == null) {
                        nrVary.domDialogToken = document.createElement('div');
                        nrVary.domDialogToken.innerHTML = `
<div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
    <div class="modal-content">
        <div class="modal-header">
            <h5 class="modal-title">Token（令牌）</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
            <div>Refresh after pasting</div>
            <div>Anonymous access is limited (60 per hour)</div>
            <input class="form-control my-3" placeholder="token" />
            <div><a target="_blank" href="https://github.com/settings/tokens">https://github.com/settings/tokens</a></div>
        </div>
    </div>
</div>`;
                        nrVary.domDialogToken.className = "modal";
                        document.body.appendChild(nrVary.domDialogToken);
                        //记录Token
                        nrVary.domDialogToken.querySelector('input').addEventListener('input', async function () {
                            let domItem = nrVary.domDdMore.querySelector('[data-action="token"]');
                            if (this.value.trim() == "") {
                                domItem.classList.remove('active');
                                nrVary.flagToken = null;
                                await nrStorage.removeItem('uuid-token-github');
                            } else if (this.value.length > 10) {
                                domItem.classList.add('active');
                                nrVary.flagToken = this.value.trim();
                                await nrStorage.setItem('uuid-token-github', this.value.trim());
                            }
                        });

                        nrVary.bsDialogToken = new bootstrap.Modal(nrVary.domDialogToken);
                    }
                    nrVary.domDialogToken.querySelector('input').value = nrVary.flagToken;

                    nrVary.bsDialogToken.show();
                }
                break;
            case "local":
                {
                    let onlineHref = `https://github.com/${nrVary.flagName}/${nrVary.flagResp}`;
                    let onlineText = `线上 GitHub (<a href="${onlineHref}">${onlineHref}</a>)`;
                    let localHref = new URL(nrVary.flagLocalPath, location).href;
                    let localText = `本地 Local (<a href="${localHref}">${localHref}</a>)`;
                    let toIcon = '<svg viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" width="24" height="24"><path d="M448 789.312V0h128v789.312l234.688-234.624L896 640l-384 384-384-384 85.312-85.312L448 789.312z" fill="currentColor"></path></svg>';

                    let msg = nrVary.flagLocalUsed ? `${localText}<div class="my-2">${toIcon}</div>${onlineText}` : `${onlineText}<div class="my-2">${toIcon}</div>${localText}`;

                    if (await nrApp.confirm(msg, '切换 switch')) {
                        nrVary.flagLocalUsed = !nrVary.flagLocalUsed;
                        await nrStorage.setItem('local', nrVary.flagLocalUsed);
                        nrVary.domDdMore.querySelector('[data-action="local"]').classList.toggle('active');
                        if (location.hash == "#_local") {
                            location.hash = "";
                        }
                        location.reload();
                    }
                }
                break;
            case "proxy":
                {
                    nrVary.flagProxyUsed = !nrVary.flagProxyUsed;
                    await nrStorage.setItem('proxy', nrVary.flagProxyUsed);
                    nrVary.domDdMore.querySelector('[data-action="proxy"]').classList.toggle('active');
                }
                break;
            case "reload":
                {
                    //清空 user
                    let keys = await nrStorage.instanceUser.keys();
                    for (const key of keys) {
                        if (key.startsWith(`${nrVary.flagName}:`)) {
                            await nrStorage.instanceUser.removeItem(key);
                        }
                    }

                    await nrWeb.load();
                }
                break;
            case "convert":
                {
                    if (nrVary.domDialogConvert == null) {
                        nrVary.domDialogConvert = document.createElement('div');
                        nrVary.domDialogConvert.innerHTML = `
<div class="modal-dialog modal-fullscreen modal-dialog-centered modal-dialog-scrollable">
    <div class="modal-content">
        <div class="modal-header">
            <h5 class="modal-title">Convert HTML bookmarks（转换书签）</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
            <div class="row">
                <div class="col-lg-6">
                    <input type="file" class="mb-2 form-control" />
                    <textarea class="form-control" style="height:calc(100vh - 145px);"></textarea>
                </div>
                <div class="col-lg-6">
                    <div class="nrg-preview overflow-auto border rounded p-3" style="height:calc(100vh - 100px);">preview</div>
                </div>
            </div>
        </div>
    </div>
</div>`;
                        nrVary.domDialogConvert.className = "modal";
                        document.body.appendChild(nrVary.domDialogConvert);

                        //选择文件
                        nrVary.domDialogConvert.querySelector('input').addEventListener('change', async function () {
                            let file = this.files[0];
                            if (file) {
                                try {
                                    let content = await nrcFile.reader(file);

                                    let mds = [];
                                    let domBookmark = document.createElement('div');
                                    domBookmark.innerHTML = content;
                                    mds.push("# " + domBookmark.querySelector('h1').innerHTML);
                                    nrWeb.convertHtml(domBookmark, mds);

                                    let val = mds.join('\r\n');
                                    nrVary.domDialogConvert.querySelector('textarea').value = val;

                                    let marked = await import('marked');
                                    nrVary.domDialogConvert.querySelector('.nrg-preview').innerHTML = marked.parse(val);
                                } catch (ex) {
                                    console.debug(ex)
                                    nrApp.alert('转换失败');
                                }
                                this.value = "";
                            }
                        })

                        //渲染
                        nrVary.domDialogConvert.querySelector('textarea').addEventListener('input', async function () {
                            let marked = await import('marked');
                            nrVary.domDialogConvert.querySelector('.nrg-preview').innerHTML = marked.parse(this.value);
                        })

                        nrVary.bsDialogConvert = new bootstrap.Modal(nrVary.domDialogConvert);
                    }

                    nrVary.bsDialogConvert.show();
                }
                break;
            case "about":
                {
                    let html = `
<div>GitHub: <a href="https://github.com/netnr">https://github.com/netnr</a></div>
<div>联系打赏: <a href="https://zme.ink">https://zme.ink</a></div>
<hr/>
<div class="mt-2">缓存后可离线使用</div>
<hr/>
<div>Fork 项目，从浏览器导出书签 HTML，再转换书签为 Markdown，保存到 libs/*.md</div>
<div>私有化部署，更新索引文件 libs/index.json，页面再启用 本地 Local</div>
<hr/>
<div>uuid.fun 于 2028-11-09 8:00 到期，计划不再续费，启用子域名，<a href="https://uu.zme.ink">https://uu.zme.ink</a></div>
`;
                    nrApp.alert(html, 'About 关于');
                }
                break;
        }
    },

    /**
     * 请求用户
     * @param {*} name 用户名
     * @param {*} flush 强刷
     */
    reqUser: async (name, flush) => {
        let url = `https://api.github.com/users/${name}`;

        let ckey = `${name}:${url}`;
        let result = await nrStorage.instanceUser.getItem(ckey);
        if (result == null || flush) {
            result = await nrWeb.reqServer(url);
            if (result) {
                await nrStorage.instanceUser.setItem(ckey, result);
                await nrStorage.instanceUser.setItem(`${name}:update-time`, Date.now());
            }
        }
        return result;
    },

    /**
     * 请求 libs
     * @param {*} name 用户名
     * @param {*} resp 仓库名
     * @param {*} libs 包名
     * @param {*} flush 强刷
     */
    reqLibs: async (name, resp, libs, flush) => {
        let url = `https://api.github.com/repos/${name}/${resp}/contents/${libs}`;
        let ckey = `${name}:${url}`;
        let result = await nrStorage.instanceUser.getItem(ckey);
        if (result == null || flush) {
            result = await nrWeb.reqServer(url);
            if (result) {
                await nrStorage.instanceUser.setItem(ckey, result);
            }
        }
        return result;
    },

    /**
     * 请求 Raw
     * @param {*} name 用户名
     * @param {*} resp 仓库名
     * @param {*} libs 包名
     */
    reqRaw: async (url) => {
        let ckey = `${nrVary.flagName}:${url}`;
        let result = await nrStorage.instanceUser.getItem(ckey);
        if (result == null) {
            result = await nrWeb.reqServer(`${url}?_${nrcBase.random()}`, { type: "text" });
            if (result) {
                await nrStorage.instanceUser.setItem(ckey, result);
            }
        }
        return result;
    },

    convertHtml: (dom, mds) => {
        for (let i = 0; i < dom.children.length; i++) {
            let ele = dom.children[i];
            switch (ele.nodeName) {
                case "H3":
                    mds.push('');
                    mds.push("### " + ele.innerHTML);
                    break;
                case "DL":
                case "P":
                    nrWeb.convertHtml(ele, mds);
                    break;
                case "DT":
                    {
                        if (ele.children.length == 1) {
                            let domLink = ele.querySelector('a');
                            mds.push('- [' + domLink.innerHTML.replace(/`/g, '\\`') + '](' + domLink.href + ')');
                        } else {
                            nrWeb.convertHtml(ele, mds);
                        }
                    }
                    break;
            }
        }
    },

    /**
     * 显示用户
     */
    viewUser: async () => {
        let result;
        //本地
        if (nrVary.flagLocalUsed) {
            nrVary.flagLocalJson = (await nrWeb.reqServer(nrVary.flagLocalPath)) || {};
            result = nrVary.flagLocalJson["user"] || {};
        } else {
            result = await nrWeb.reqUser(nrVary.flagName);
        }

        if (result) {
            nrVary.domImgAvatar.onerror = function () {
                nrVary.domImgAvatar.src = '/favicon.ico';
                nrVary.domImgAvatar.onerror = false;
            }
            //头像
            nrVary.domImgAvatar.src = result.avatar_url;

            //个人信息
            let itemName = result.name == null ? result.login : `${result.login} (${result.name})`;
            let itemBio = result.bio == null ? '' : `<li><span class="dropdown-item-text" title="bio">${result.bio}</span></li>`;
            let itemCompany = result.company == null ? '' : `<li><span class="dropdown-item-text" title="company">${result.company}</span></li>`;
            let itemLocation = result.location == null ? '' : `<li><span class="dropdown-item-text" title="location">${result.location}</span></li>`;
            let itemBlog = result.blog == null ? '' : `<li><a class="dropdown-item" title="blog" href="${result.blog}">${result.blog}</a></li>`;

            nrVary.domDdUserInfo.innerHTML = `<li><a class="dropdown-item" title="name" href="https://github.com/${result.login}">${itemName}</a></li>
            ${itemBio}<li><hr class="dropdown-divider"></li>${itemCompany}${itemLocation}${itemBlog}
            `;

            //标题
            document.title = `${result.login} - ${nrVary.flagTitle}`;
        }
    },

    /**
     * 显示链接
     */
    viewLink: async () => {
        let listLibs;
        if (nrVary.flagLocalUsed) {
            listLibs = nrVary.flagLocalJson["libs"] || [];
        } else {
            listLibs = await nrWeb.reqLibs(nrVary.flagName, nrVary.flagResp, nrVary.flagLibs);
        }

        //libs
        if (listLibs) {
            let viewLibs = [];
            let libIndex = 0;

            listLibs.forEach(lib => {
                //md
                if (lib.type == "file" && lib.name.endsWith(nrVary.flagSuffix)) {
                    let fileName = lib.name.substring(0, lib.name.length - nrVary.flagSuffix.length);
                    libIndex++;

                    viewLibs.push(`
                    <div class="accordion-item">
                        <h2 class="accordion-header">
                            <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapse-${libIndex}">${fileName}</button>
                        </h2>
                        <div id="collapse-${libIndex}" class="accordion-collapse collapse show">
                            <div class="accordion-body">${nrApp.tsLoadingHtml}</div>
                        </div>
                    </div>
                    `);
                }
            });
            nrVary.domView.innerHTML = `<div class="accordion">${viewLibs.join('')}</div>`;

            libIndex = 0;
            for (const lib of listLibs) {
                if (lib.type == "file" && lib.name.endsWith(nrVary.flagSuffix)) {
                    libIndex++;

                    let domContent = nrVary.domView.querySelector(`#collapse-${libIndex}`).children[0];

                    let rawContent = await nrWeb.reqRaw(lib.download_url);
                    if (rawContent) {
                        let marked = await import('marked');
                        domContent.innerHTML = marked.parse(rawContent);
                    } else {
                        domContent.innerHTML = nrApp.tsFailHtml;
                    }
                }
            }

            nrVary.domView.dataset.ended = true;
        } else {
            nrVary.domView.innerHTML = `<div class="col-12">${nrApp.tsFailHtml}</div>`;
        }
    },

    searchLink: (key) => {
        if (nrVary.domView.dataset.ended) {
            let html = [];
            if (key.trim() != "") {
                key = key.toLowerCase()
                let domLinks = nrVary.domView.querySelectorAll('a');
                for (let index = 0; index < domLinks.length; index++) {
                    const domLink = domLinks[index];
                    let domText = `${domLink.href},${domLink.innerText},${domLink.title || ""}`.toLowerCase();
                    if (domText.includes(key)) {
                        html.push(`<a class="d-flex px-3 py-2 border-top opacity-75" href="${domLink.href}"><span class="w-75 text-truncate">${domLink.href}</span><span class="w-25 text-truncate">${domLink.innerText}</span></a>`);
                        if (html.length > 7) {
                            break;
                        }
                    }
                }
            }
            nrVary.domSearch.innerHTML = html.join('');
            if (html.length) {
                nrVary.domSearch.classList.remove('d-none');
                nrVary.domSearch.querySelector('a').classList.add('active');
            } else {
                nrVary.domSearch.classList.add('d-none');
            }
        }
    },
    searchArrow: (arrow) => {
        if (!nrVary.domSearch.classList.contains('d-none')) {
            let domActive = nrVary.domSearch.querySelector('a.active');
            let domNew = arrow == "ArrowDown" ? domActive.nextElementSibling : domActive.previousElementSibling;
            if (domNew) {
                domActive.classList.remove('active');
                domNew.classList.add('active');
            }
        }
    },

    /**
     * 请求服务
     * @param {any} url 链接
     * @param {any} options 选项 type: text blob json(default)
     * @returns 
     */
    reqServer: async (url, options) => {
        options = options || {};

        //token
        if (nrVary.flagToken != null && nrVary.flagToken.length > 10 && !url.includes("githubusercontent")) {
            options.headers = options.headers || {};
            options.headers["authorization"] = `token ${nrVary.flagToken}`;
        }

        //proxy
        if (nrVary.flagProxyUsed && !nrVary.flagLocalUsed) {
            url = `${nrVary.flagProxyServer}${encodeURIComponent(url.replace("https://", ""))}`;
        }

        let vm = await nrcBase.fetch(url, options);

        if (vm.error) {
            nrApp.logError(vm.error, '网络错误');
        } else if (vm.resp.ok == false) {
            if (vm.resp.status == 403) {
                nrApp.toast('请设置 Token');
            }
        } else if (vm.resp.ok && vm.result) {
            return vm.result;
        }
    },
}

export { nrWeb };