import { nrcBase } from "../../../../frame/nrcBase";
import { nrcSplit } from "../../../../frame/nrcSplit";
import { nrEditor } from "../../../../frame/nrEditor";
import { nrWeb } from "../../nrWeb";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: ['/run/index', '/run/edit/*'],

    ckey: "/run/index/run_last",

    init: async () => {
        //总高度
        nrVary.domCard0.style.height = `calc(100vh - ${nrVary.domCard0.getBoundingClientRect().top}px)`;

        //分割器构建
        [nrVary.domCard0, nrVary.domCard1, nrVary.domCard2].forEach(dom => {
            nrcSplit.create(dom);
        })
        nrVary.domCard0.classList.remove('invisible');
        nrApp.setLoading(nrVary.domBtnRun);

        //初始化编辑器
        await nrEditor.init();

        [nrVary.domPreHtml, nrVary.domPreCss, nrVary.domPreJs].forEach(function (dom) {
            let code = dom.innerText;
            let language = dom.dataset.language

            switch (language) {
                case "html":
                    dom.innerHTML = `<span class="position-absolute end-0 top-0 z-1 badge text-bg-danger rounded-0">HTML</span>`;
                    break;
                case "css":
                    dom.innerHTML = `<span class="position-absolute end-0 top-0 z-1 badge text-bg-info rounded-0">CSS</span>`;
                    break;
                case "javascript":
                    dom.innerHTML = `<span class="position-absolute end-0 top-0 z-1 badge text-bg-warning rounded-0">JS</span>`;
                    break;
            }

            dom.editor = nrEditor.create(dom, {
                value: code,
                language: language,
                roundedSelection: true,
                scrollBeyondLastLine: true,
                scrollbar: {
                    verticalScrollbarSize: 0,
                    horizontalScrollbarSize: 12
                },
                minimap: {
                    enabled: false
                }
            })
            dom.classList.remove('nrg-pre-code-edit');
            nrApp.setLoading(nrVary.domBtnRun, true);

            //快捷键
            dom.editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyS, function () {
                nrPage.preview();
            })
            dom.editor.addCommand(monaco.KeyCode.PauseBreak, () => nrPage.preview())

            //载入最后一次运行
            dom.editor.addAction({
                id: "meid-run-last",
                label: "还原为上次运行记录",
                keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode.KeyR],
                contextMenuGroupId: "me-01",
                run: async () => {
                    let json = await nrStorage.getItem(nrPage.ckey);
                    if (json) {
                        nrVary.domPreHtml.editor.setValue(json.html);
                        nrVary.domPreCss.editor.setValue(json.css);
                        nrVary.domPreJs.editor.setValue(json.javascript);
                    }
                }
            });
        });

        //预览
        nrPage.preview(true);

        nrPage.bindEvent();
    },

    bindEvent: () => {
        //包搜索初始化
        nrPage.libaryInit();

        //运行
        nrVary.domBtnRun.addEventListener('click', () => nrPage.preview());

        //保存
        nrVary.domBtnSave.addEventListener('click', async function () {
            let errMsg = [];

            let post = {
                RunCode: nrVary.domHidCode.value,
                RunRemark: nrVary.domTxtDescription.value,
                RunTheme: nrcBase.isDark() ? "vs-dark" : "vs",
                RunContent1: nrVary.domPreHtml.editor.getValue(),
                RunContent2: nrVary.domPreJs.editor.getValue(),
                RunContent3: nrVary.domPreCss.editor.getValue()
            };

            if (post.RunRemark.trim() == "") {
                errMsg.push('Description is empty');
            }
            let rclen = post.RunContent1 + post.RunContent2 + post.RunContent3;
            if (rclen.length == 0) {
                errMsg.push('Code is empty');
            }
            if (rclen > 10000 * 50) {
                errMsg.push('Code content is too long ( less than 500000 )');
            }

            if (errMsg.length) {
                nrApp.alert(errMsg.join('<hr/>'));
            } else {
                let fd = nrcBase.fromKeyToFormData(post);

                nrApp.setLoading(nrVary.domBtnSave);
                let result = await nrWeb.reqServer(`/Run/Save`, { method: "POST", body: fd });

                if (result.code == 200) {
                    nrApp.toast("保存成功");
                    location.href = `/run/code/${result.data}`;
                } else {
                    nrApp.alert(result.msg);
                }
            }
        });
    },

    /**
     * 预览
     * @param {*} isInit 
     */
    preview: (isInit) => {
        nrVary.domPreview.innerHTML = "";

        let codeHtml = nrVary.domPreHtml.editor.getValue();
        let codeCss = nrVary.domPreCss.editor.getValue();
        let codeJs = nrVary.domPreJs.editor.getValue();

        //存储运行
        if (!isInit) {
            nrStorage.setItem(nrPage.ckey, { html: codeHtml, javascript: codeJs, css: codeCss })
        }

        let iframe = document.createElement('iframe');
        iframe.className = "w-100 h-100";
        iframe.name = "preview";
        nrVary.domPreview.appendChild(iframe);

        if (codeHtml.includes("</head>") && codeHtml.includes("</body>")) {
            if (codeCss != "") {
                codeHtml = codeHtml.replace("</head>", `<style>${codeCss}</style></head>`);
            }
            if (codeJs != "") {
                codeHtml = codeHtml.replace("</body>", `<script>${codeJs}</script></body>`);
            }

            iframe.onload = function () {
                //提取 title
                if (!isInit && nrVary.domTxtDescription.value.trim() == "") {
                    let title = iframe.contentWindow.document.title;
                    nrVary.domTxtDescription.value = title;
                }
            }
        } else {
            iframe.onload = function () {
                if (codeCss != "") {
                    let style = document.createElement("STYLE");
                    style.innerHTML = codeCss;
                    iframe.contentWindow.document.head.appendChild(style);
                }

                if (codeJs != "") {
                    let script = document.createElement("SCRIPT");
                    script.innerHTML = codeJs;
                    iframe.contentWindow.document.body.appendChild(script);
                }

                //提取 title
                if (!isInit && nrVary.domTxtDescription.value.trim() == "") {
                    let title = iframe.contentWindow.document.title;
                    nrVary.domTxtDescription.value = title;
                }
            }
        }
        iframe.contentWindow.document.open();
        iframe.contentWindow.document.write(codeHtml);
        iframe.contentWindow.document.close();
    },

    libaryInit: () => {
        //搜索
        nrVary.domTxtSearchLibrary.addEventListener("keydown", async function (e) {
            if (e.keyCode == 13) {
                nrPage.libaryKey = this.value.trim();
                await nrPage.libarySearch();
            }
        });

        //选择
        nrVary.domListLibrary.addEventListener('click', async function (event) {
            let target = event.target;

            let children = this.children;
            let len = children.length;
            for (let index = 0; index < len; index++) {
                let child = children[index];
                if (child.contains(target)) {
                    await nrPage.libarySelected(child);
                    break;
                }
            }
        });

        nrVary.domDdLibrary.style.maxHeight = `calc(100vh - ${nrVary.domBtnLibrary.getBoundingClientRect().top + 100}px)`;
    },
    libaryKey: "",
    /**
     * 搜索
     */
    libarySearch: async () => {
        if (nrPage.libaryKey == "") {
            nrVary.domListLibrary.innerHTML = nrPage.libaryBuildItem("empty", "", "empty");
        } else {
            nrApp.setLoading(nrVary.domBtnLibrary);
            let result = await nrWeb.reqServer(`https://data.jsdelivr.com/v1/package/npm/${nrPage.libaryKey}`)
            nrApp.setLoading(nrVary.domBtnLibrary, true);

            let htm = [];
            if (result.versions) {
                htm = result.versions.map(item => nrPage.libaryBuildItem(item, "", "version"));
            } else if (result.files) {
                htm = nrPage.libaryFilesEach(result.files);

                if (result.default) {
                    if (result.default[0] == "/") {
                        result.default = result.default.substring(1);
                    }
                    htm.splice(0, 0, nrPage.libaryBuildItem(result.default));
                }
            } else {
                htm.push(nrPage.libaryBuildItem("empty", "", "empty"));
            }

            nrVary.domListLibrary.innerHTML = htm.join("");
        }
    },
    /**
     * 构建列项
     * @param {*} name 
     * @param {*} dir 
     * @param {*} type 
     * @returns 
     */
    libaryBuildItem: (name, dir, type) => {
        type = type ? type : "file";
        dir = dir ? dir : "";

        let empty = [], pn = dir == "" ? 0 : dir.split('/').length - 1;
        while (pn--) {
            empty.push(`<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16"><path d="M4 8a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 0 1h-7A.5.5 0 0 1 4 8z"/></svg>`)
        }
        if (empty.length) {
            empty = empty.join(' ') + " ";
        } else {
            empty = "";
        }

        if (["file", "version"].indexOf(type) == -1) {
            return `<button type="button" class="list-group-item list-group-item-action px-2 py-1 list-group-item-light">${empty}${name}</button>`
        } else {
            return `<button type="button" class="list-group-item list-group-item-action px-2 py-1" data-type="${type}" data-dir="${dir + name}">${empty}${name}</button>`;
        }
    },
    /**
     * 遍历文件列表
     * @param {*} files 
     * @param {*} directoryName 
     * @returns 
     */
    libaryFilesEach: (files, directoryName) => {
        directoryName = directoryName || "";

        let htm = [];
        files.forEach(item => {
            if (item.type == "file") {
                htm.push(nrPage.libaryBuildItem(item.name, directoryName));
            } else if (item.type == "directory") {
                htm.push(nrPage.libaryBuildItem(item.name, directoryName, item.type));
                htm = htm.concat(nrPage.libaryFilesEach(item.files, directoryName + item.name + "/"));
            }
        })

        return htm;
    },
    /**
     * 选中项
     * @param {*} domItem 
     */
    libarySelected: async (domItem) => {
        switch (domItem.dataset.type) {
            case "file":
                {
                    let dir = domItem.dataset.dir;
                    let text = `https://npmcdn.com/${nrPage.libaryKey}/${dir}`;
                    switch (dir.split('.').pop()) {
                        case "js":
                            text = `<script src="${text}"></script>`
                            break;
                        case "css":
                            text = `<link rel="stylesheet" href="${text}" />`
                            break;
                    }
                    text += "\n";

                    //添加到编辑器
                    let gse = nrVary.domPreHtml.editor.getSelection();
                    let range = new monaco.Range(gse.startLineNumber, gse.startColumn, gse.endLineNumber, gse.endColumn);
                    let op = { identifier: { major: 1, minor: 1 }, range: range, text: text, forceMoveMarkers: true };
                    nrVary.domPreHtml.editor.executeEdits("", [op]);
                }
                break;
            case "version":
                {
                    nrVary.domTxtSearchLibrary.value = `${nrPage.libaryKey}@${domItem.dataset.dir}`;
                    nrPage.libaryKey = nrVary.domTxtSearchLibrary.value;
                    await nrPage.libarySearch();
                }
                break;
        }
    }
}

export { nrPage };