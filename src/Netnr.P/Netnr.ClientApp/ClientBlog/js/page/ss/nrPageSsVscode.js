import { nrEditor } from "../../../../frame/nrEditor";
import { nrcFile } from "../../../../frame/nrcFile";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrVary } from "../../nrVary";
import { nrcSnowflake } from "../../../../frame/nrcSnowflake";

let nrPage = {
    pathname: "/ss/vscode",

    ckeyLanguage: "/ss/vscode/language",
    ckeyContent: "/ss/vscode/content",

    ckeySelected: "/ss/vscode/selected",
    ckeyFile: "/ss/vscode/file",
    cacheSelected: 0,
    cacheFile: [],
    cacheLanguages: [],

    init: async () => {
        //编辑器
        nrVary.domEditor.innerHTML = nrApp.tsLoadingHtml;
        await nrEditor.init();
        nrVary.domEditor.innerHTML = '';

        let modesIds = monaco.languages.getLanguages().map(lang => lang.id).sort();
        modesIds = modesIds.filter(x => !x.includes('.'));
        nrPage.cacheLanguages = modesIds;
        // 语言列表
        modesIds.forEach(lang => {
            let domItem = document.createElement('option');
            domItem.value = lang;
            domItem.innerHTML = lang;
            nrVary.domSeLanguage.appendChild(domItem);
        });

        nrVary.domSeLanguage.classList.remove('invisible');

        nrApp.tsEditor = nrEditor.create(nrVary.domEditor);
        nrVary.domEditor.editor = nrApp.tsEditor;

        nrVary.domEditor.classList.add('border');

        nrcBase.setHeightFromBottom(nrVary.domEditor);

        //列表
        nrPage.cacheFile = await nrStorage.getItem(nrPage.ckeyFile) || [];
        nrPage.cacheFile.forEach(item => delete item.content); //删除历史遗留问题
        nrPage.fileView();

        nrPage.bindEvent();
    },

    flagWorking: false,

    bindEvent: () => {
        //显示列表
        nrVary.domDdlList.addEventListener('shown.bs.dropdown', async function () {
            let mtop = nrVary.domEditor.getBoundingClientRect().top + nrcBase.tsBottomKeepHeight;
            nrVary.domListFile.style.maxHeight = `calc(100vh - ${mtop}px)`;
        })

        //新增
        nrVary.domBtnNew.addEventListener('click', async function () {
            let item = { id: nrcSnowflake.id(), title: `file ${nrcSnowflake.id().toString().slice(-4)}`, language: nrVary.domSeLanguage.value, create: Date.now() };
            nrPage.cacheFile.push(item);
            await nrStorage.setItem(nrPage.ckeyFile, nrPage.cacheFile);

            nrPage.fileAdd(item);
            nrPage.showList();

            nrEditor.keepSetValue(nrApp.tsEditor, "");
            nrPage.fileSelected(item.id);
        });

        //导出当前
        nrVary.domBtnExport.addEventListener('click', async function () {
            let domItem = nrVary.domListFile.querySelector('li.active');
            await nrPage.fileDownload(domItem);
        });
        //导出全部
        nrVary.domBtnExportAll.addEventListener('click', async function () {
            if (await nrApp.confirm("确定导出全部？")) {
                let domItems = nrVary.domListFile.querySelectorAll('li');
                for (const domItem of domItems) {
                    await nrPage.fileDownload(domItem);
                }
            }
        });

        //点击列表
        nrVary.domListFile.addEventListener('click', async function (e) {
            if (nrPage.flagWorking) {
                console.debug('working')
                return;
            }
            nrPage.flagWorking = true;

            let target = e.target;

            let listNode = nrVary.domListFile.children;
            for (const domItem of listNode) {
                if (domItem.contains(target)) {
                    {
                        let id = domItem.dataset.id;
                        if (target.classList.contains('flag-update')) {
                            await nrPage.fileUpdate(id, domItem);
                        } else if (target.classList.contains('flag-remove')) {
                            //删除
                            await nrPage.fileRmove(id);
                        } else {
                            //选中
                            await nrPage.fileSelected(id);
                        }
                    }
                    break;
                }
            }
            nrPage.flagWorking = false;
        });

        //修改语言
        nrVary.domSeLanguage.addEventListener('input', async function () {
            nrEditor.setLanguage(nrApp.tsEditor, this.value);

            //更新列表选中项的语言
            for (const item of nrPage.cacheFile) {
                if (item.id == nrPage.cacheSelected) {
                    item.language = this.value;
                    nrVary.domListFile.querySelector('li.active').dataset.language = this.value;
                    break;
                }
            }
            await nrStorage.setItem(nrPage.ckeyFile, nrPage.cacheFile);

            //联动
            nrVary.domCardActive.querySelectorAll('*').forEach(item => item.classList.add('d-none'));
            if (this.value == "javascript") {
                nrVary.domBtnRun.classList.remove('d-none');
            } else if (this.value == "markdown") {
                nrVary.domBtnPreview.classList.remove('d-none');
            }
        });

        //变动自动保存
        nrEditor.onChange(nrApp.tsEditor, (value) => {
            if (nrPage.flagWorking == false) {
                nrStorage.setItem(`${nrPage.ckeyFile}-${nrPage.cacheSelected}`, value);
            }
        });

        //运行
        nrApp.tsEditor.addCommand(monaco.KeyCode.PauseBreak, () => {
            nrVary.domBtnRun.click();
        });
        nrVary.domBtnRun.addEventListener('click', async function () {
            switch (nrEditor.getLanguage(nrApp.tsEditor)) {
                case "javascript":
                    try {
                        window.ee = new Function(nrApp.tsEditor.getValue());
                        ee();
                    } catch (ex) {
                        console.error(ex);
                    }
                    break;
            }
        });

        //预览
        nrVary.domBtnPreview.addEventListener('click', async function () {
            nrApp.alert(nrApp.tsLoadingHtml, '预览 Preview', '98vw');
            await nrcRely.remote('netnrmd');
            let html = netnrmd.render(nrApp.tsEditor.getValue());
            nrApp.domAlert.querySelector('.modal-body').innerHTML = `<div class="markdown-body">${html}</div>`;
        });

        //接收文件
        nrcFile.init(async (files) => {
            nrPage.flagWorking = true;
            let lastItem;
            for (const file of files) {
                try {
                    if (file.size > 1024 * 1024 * 50) {
                        if (!await nrApp.confirm(file.name, '文件过大是否继续打开')) {
                            continue;
                        }
                    }
                    let text = await nrcFile.reader(file);

                    let item = {
                        id: nrcSnowflake.id(),
                        title: file.name,
                        language: 'plaintext',
                        create: file.lastModifiedDate
                    };

                    //根据扩展名称识别语言
                    let ext = file.name.split('.').pop();
                    if (nrPage.cacheLanguages.includes(ext)) {
                        item.language = ext;
                    } else {
                        for (const key in nrPage.languageExt) {
                            if (nrPage.languageExt[key] == ext) {
                                item.language = key;
                                break;
                            }
                        }
                    }

                    //内容
                    await nrStorage.setItem(`${nrPage.ckeyFile}-${item.id}`, text);

                    nrPage.cacheFile.push(item);
                    lastItem = item;

                    nrPage.fileAdd(item);
                    nrPage.showList();
                } catch (error) {
                    console.debug(error);
                    nrApp.toast(`打开文件（${file.name}）失败`);
                }
            }

            if (lastItem != null) {
                nrEditor.keepSetValue(nrApp.tsEditor, await nrStorage.getItem(`${nrPage.ckeyFile}-${lastItem.id}`));
                nrApp.tsEditor.revealLine(1); //滚动到第一行
                nrPage.fileSelected(lastItem.id);
                await nrStorage.setItem(nrPage.ckeyFile, nrPage.cacheFile);
            }
            nrPage.flagWorking = false;
        })
    },

    showList: () => (new bootstrap.Dropdown(nrVary.domDdlList)).show(),

    fileView: async () => {
        //默认记录
        if (nrPage.cacheFile.length == 0) {
            let dvContent = await nrStorage.getItem(nrPage.ckeyContent) || 'console.log("hello world")';
            let dvLanguage = await nrStorage.getItem(nrPage.ckeyLanguage) || "javascript";
            nrPage.cacheFile.push({ id: 0, title: "default", language: dvLanguage, create: Date.now() });

            //更新到列表，并删除默认记录
            await nrStorage.setItem(nrPage.ckeyFile, nrPage.cacheFile);
            await nrStorage.setItem(`${nrPage.ckeyFile}-0`, dvContent);
            await nrStorage.removeItem(nrPage.ckeyContent);
            await nrStorage.removeItem(nrPage.ckeyLanguage);
        } else {
            nrPage.cacheSelected = (await nrStorage.getItem(nrPage.ckeySelected)) || nrPage.cacheFile[0].id;
        }

        nrVary.domListFile.innerHTML = "";
        nrPage.cacheFile.forEach(item => nrPage.fileAdd(item));

        //选中
        nrPage.fileSelected(nrPage.cacheSelected);
    },

    fileSelected: async (id) => {
        let listNode = nrVary.domListFile.children;
        for (let index = 0; index < listNode.length; index++) {
            const domItem = listNode[index];
            if (domItem.dataset.id == id) {
                //选中
                domItem.classList.add('active');
                nrPage.cacheSelected = id;
                nrStorage.setItem(nrPage.ckeySelected, id);

                //赋值语言
                nrVary.domSeLanguage.value = domItem.dataset.language;
                nrcBase.dispatchEvent('input', nrVary.domSeLanguage);

                //赋值内容
                let fileContent = await nrStorage.getItem(`${nrPage.ckeyFile}-${id}`);
                nrEditor.keepSetValue(nrApp.tsEditor, fileContent);
                //滚动到顶部
                nrApp.tsEditor.setScrollPosition({ scrollTop: 0, scrollLeft: 0 });
            } else {
                domItem.classList.remove('active');
            }
        }
    },

    fileAdd: (item) => {
        var domItem = document.createElement('li');
        domItem.className = `list-group-item list-group-item-action d-flex`;
        domItem.dataset.id = item.id;
        domItem.title = item.title;
        domItem.dataset.language = item.language || "plaintext";
        domItem.innerHTML = `<div class="ms-2 me-auto"><div class="text-truncate" style="width:12em">${nrcBase.htmlEncode(item.title)}</div></div>
        <div><a class="flag-update text-decoration-none" href="javascript:void(0)" title="编辑 update">📝</a><a class="flag-remove text-decoration-none" href="javascript:void(0)" title="删除 remove">❌</a></div>`
        nrVary.domListFile.appendChild(domItem);
    },

    fileRmove: async (id) => {
        for (let index = 0; index < nrPage.cacheFile.length; index++) {
            let item = nrPage.cacheFile[index];
            if (item.id == id) {
                //删除
                nrPage.cacheFile.splice(index, 1);
                await nrStorage.setItem(nrPage.ckeyFile, nrPage.cacheFile);
                await nrStorage.removeItem(`${nrPage.ckeyFile}-${id}`);

                //选中第一个
                if (item.id == nrPage.cacheSelected && nrPage.cacheFile.length > 0) {
                    nrPage.fileSelected(nrPage.cacheFile[0].id);
                } else {
                    nrPage.cacheSelected = 0;
                }
                break;
            }
        }

        await nrPage.fileView();
    },

    fileUpdate: async (id, domItem) => {
        let promiseConfirm = nrApp.confirm(`<input class="form-control" value="${domItem.title}" plaeholder="名称" />`, '修改名称');
        let domInput = nrApp.domConfirm.querySelector('input');
        domInput.focus();
        domInput.select();

        if (await promiseConfirm) {
            let title = domInput.value.trim();
            if (title != "") {
                domItem.title = title;
                domItem.querySelector('.text-truncate').innerText = title;

                for (let index = 0; index < nrPage.cacheFile.length; index++) {
                    let item = nrPage.cacheFile[index];
                    if (item.id == id) {
                        item.title = title;
                        await nrStorage.setItem(nrPage.ckeyFile, nrPage.cacheFile);
                        break;
                    }
                }
            }
        }
    },

    fileDownload: async (domItem) => {
        let ext = nrPage.languageExt[domItem.dataset.language] || domItem.dataset.language;
        let fileName = domItem.title + (domItem.title.endsWith(`.${ext}`) ? '' : `.${ext}`);
        let fileContent = await nrStorage.getItem(`${nrPage.ckeyFile}-${domItem.dataset.id}`);
        nrcBase.download(fileContent, fileName);
    },

    //vscode 语言对应文件扩展名称的所有键值对
    languageExt: {
        "cameligo": "migo", "clojure": "clj", "coffeescript": "coffee", "csharp": "cs",
        "cypher": "cyp", "elixir": "ex", "freemarker2": "ftl", "fsharp": "fs", "graphql": "gql",
        "handlebars": "hbs", "javascript": "js", "json": "jsonc", "julia": "jl", "kotlin": "kt",
        "lexon": "lex", "markdown": "md", "mips": "s", "mysql": "sql", "objective-c": "m",
        "pascal": "pas", "pascaligo": "pasi", "perl": "pl", "pgsql": "sql", "plaintext": "txt",
        "postiats": "dats", "powerquery": "pq", "powershell": "ps1", "python": "py", "qsharp": "qs",
        "razor": "cshtml", "restructuredtext": "rst", "ruby": "rb", "rust": "rs", "scheme": "scm",
        "shell": "sh", "sparql": "rq", "sql": "sql", "st": "st", "swift": "swift", "systemverilog": "sv",
        "tcl": "tcl", "twig": "twig", "typescript": "ts", "verilog": "v"
    }
}

export { nrPage };