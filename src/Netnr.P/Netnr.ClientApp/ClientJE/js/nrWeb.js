import { nrcFile } from "../../frame/nrcFile";
import { nrcBase } from "../../frame/nrcBase";
import { nrStorage } from "../../frame/nrStorage";
import { nrcSplit } from "../../frame/nrcSplit";
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

        //全局样式
        nrWeb.addStyle();

        //主题
        let isDark = nrcBase.isDark();
        nrWeb.setTheme(isDark ? "dark" : "light");

        await nrStorage.init();
        //资源依赖
        nrcBase.tsIcons["caret-right-fill"] = ['m12.14 8.753-5.482 4.796c-.646.566-1.658.106-1.658-.753V3.204a1 1 0 0 1 1.659-.753l5.48 4.796a1 1 0 0 1 0 1.506z'];
        nrcBase.tsIcons["caret-left-fill"] = ['m3.86 8.753 5.482 4.796c.646.566 1.658.106 1.658-.753V3.204a1 1 0 0 0-1.659-.753l-5.48 4.796a1 1 0 0 0 0 1.506z'];
        nrcBase.tsIcons["download"] = [
            'M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z',
            'M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z'
        ];
        nrcBase.tsIcons['tree'] = ['M 0,32 v 64 h 416 v -64 z M 160,160 v 64 h 352 v -64 z M 160,288 v 64 h 288 v -64 z M 0,416 v 64 h 320 v -64 z'];
        const { JSONEditor } = await import('vanilla-jsoneditor');
        Object.assign(window, { nrWeb, nrVary, JSONEditor });

        //渲染
        await nrWeb.render();

        //构建
        nrVary.editorText = new JSONEditor({
            target: nrVary.domText,
            props: {
                mode: 'text', onChange: (editorVal) => {
                    let text = editorVal.text;
                    if (text == null && editorVal.json) {
                        text = JSON.stringify(editorVal.json);
                    }
                    nrStorage.setItem('/JSONEditor/text', text);
                }
            }
        });
        nrVary.editorTree = new JSONEditor({
            target: nrVary.domTree,
            props: {
                mode: 'tree', onChange: (editorVal) => {
                    let text = editorVal.text;
                    if (text == null && editorVal.json) {
                        text = JSON.stringify(editorVal.json);
                    }
                    nrStorage.setItem('/JSONEditor/tree', text);
                }
            }
        });

        //读取记录
        let recordText = await nrStorage.getItem('/JSONEditor/text');
        if (recordText != null && recordText.trim() != "") {
            nrVary.editorText.set({ text: recordText })
        } else {
            //demo
            nrVary.editorText.set({ json: { "array": [1, 2, 3], "boolean": true, "color": "#82b92c", "null": null, "number": 123, "object": { "a": "b", "c": "d", "e": "f" }, "string": "Hello World" } })
        }
        let recordTree = await nrStorage.getItem('/JSONEditor/tree');
        if (recordTree != null && recordTree.trim() != "") {
            nrVary.editorTree.set({ text: recordTree })
        }

        nrWeb.bindEvent();

        //关闭提示
        document.getElementById('style0').remove();
        nrVary.domBox.style.removeProperty('visibility');
    },

    render: async () => {
        let domLayout = nrVary.domLayout = document.createElement("div");
        domLayout.innerHTML = `
<div class="nrg-box nrc-split-horizontal" style="visibility:hidden">
    <div style="display:flex">
        <div class="nrg-text"></div>
        <div class="nrg-split">
            <button class="nrg-btn-to-tree" title="">${nrcBase.getIconHtml('caret-right-fill', 20)}</button>
            <button class="nrg-btn-to-text" title="">${nrcBase.getIconHtml('caret-left-fill', 20)}</button>
            <br/>
            <button class="nrg-btn-to-escape1" title="转义双引号">+\\</button>
            <button class="nrg-btn-to-escape2" title="清理转义">-\\</button>
            <br/>
            <button class="nrg-btn-to-lossless1" title="Number.MAX_SAFE_INTEGER">BigInt<br/>indent(2)</button>
            <button class="nrg-btn-to-lossless2" title="Number.MAX_SAFE_INTEGER">BigInt<br/>indent(0)</button>
            <br/>
            <button class="nrg-btn-download" title="download">${nrcBase.getIconHtml('download', 20)}</button>        
            <button class="nrg-btn-theme" title="theme">${nrcBase.getIconHtml(nrcBase.isDark() ? 'sum-fill' : 'moon', 20)}</button>
        </div>
    </div>
    <div class="nrc-split-divider"></div>
    <div class="nrg-tree"></div>
</div>
`;
        document.body.appendChild(domLayout);

        nrcBase.readDOM(document.body, "nrg", nrVary);

        let getPos = await nrStorage.getItem('/JSONEditor/position');
        nrcSplit.create(nrVary.domBox, getPos, () => {
            let setPos = nrcSplit.getPosition(nrVary.domBox);
            nrStorage.setItem('/JSONEditor/position', setPos);
        });
    },

    bindEvent: () => {
        //text to tree
        nrVary.domBtnToTree.addEventListener('click', () => {
            let editorVal = nrVary.editorText.get();
            let text = editorVal.text;
            if (text == null && editorVal.json) {
                text = JSON.stringify(editorVal.json);
            }
            nrVary.editorTree.set({ text: text });
        });
        //tree to text
        nrVary.domBtnToText.addEventListener('click', () => {
            let editorVal = nrVary.editorTree.get();
            let text = editorVal.text;
            if (text == null && editorVal.json) {
                text = JSON.stringify(editorVal.json);
            }
            try {
                nrVary.editorText.set({ json: JSON.parse(text) });
            } catch (error) {
                nrVary.editorText.set({ text: text });
            }
        });

        //转义
        nrVary.domBtnToEscape1.addEventListener('click', () => {
            try {
                let editorVal = nrVary.editorText.get();
                let text = editorVal.text;
                if (text == null && editorVal.json) {
                    text = JSON.stringify(editorVal.json);
                }
                try {
                    let val = JSON.stringify({ a: JSON.stringify(JSON.parse(text)) });
                    nrVary.editorText.set({ text: val.substring(6, val.length - 2) });
                } catch (ex) {
                    console.debug(ex)
                    nrVary.editorText.set({ text: text.replace(/\"/g, '\\"') });
                }
            } catch (error) {
                console.debug(error);
                alert('转义失败');
            }
        });
        //去除转义
        nrVary.domBtnToEscape2.addEventListener('click', () => {
            try {
                let editorVal = nrVary.editorText.get();
                let text = editorVal.text;
                if (text == null && editorVal.json) {
                    text = JSON.stringify(editorVal.json);
                }
                try {
                    let val = JSON.parse('{"a":"' + text + '"}').a;
                    nrVary.editorText.set({ text: val });
                } catch (ex) {
                    console.debug(ex);
                    nrVary.editorText.set({ text: text.replace(/\\"/g, '"') });
                }
            } catch (error) {
                console.debug(error);
                alert('去除转义失败');
            }
        });
        //无损格式化
        nrVary.domBtnToLossless1.addEventListener('click', async function () {
            await nrWeb.losslessStringify()
        })
        //无损字符串化
        nrVary.domBtnToLossless2.addEventListener('click', async function () {
            await nrWeb.losslessStringify(0);
        })
        //下载
        nrVary.domBtnDownload.addEventListener('click', function () {
            let editorVal = nrVary.editorText.get();
            let text = editorVal.text;
            if (text == null && editorVal.json) {
                text = JSON.stringify(editorVal.json);
            }
            nrcBase.download(text, 'code.json');
        })
        //主题
        nrVary.domBtnTheme.addEventListener('click', async function () {
            await nrWeb.setTheme(nrcBase.isDark() ? "light" : "dark")
        });

        //拖拽
        nrcFile.init(async (files) => {
            if (nrVary.editorText) {
                nrVary.editorText.set({ text: await nrcFile.reader(files[0]) });
            }
        })
    },

    setTheme: async (theme) => {
        nrcBase.saveTheme(theme);
        let isDark = nrcBase.isDark();

        if (isDark) {
            await import('vanilla-jsoneditor/themes/jse-theme-dark.css');
            document.body.classList.add('jse-theme-dark')
        } else {
            document.body.classList.remove('jse-theme-dark')
        }

        if (nrVary.domBtnTheme) {
            nrVary.domBtnTheme.innerHTML = nrcBase.getIconHtml(isDark ? 'sum-fill' : 'moon', 20)
        }
    },

    losslessStringify: async (indent = 2) => {
        let editorVal = nrVary.editorText.get();
        let text = editorVal.text;
        if (editorVal.json) {
            nrVary.editorText.set({ text: JSON.stringify(editorVal.json, null, indent) })
        } else {
            let JSONBin = window["JSONBin"]
            if (!JSONBin) {
                window['_JSON'] = JSON;
                JSONBin = (await import('json-bigint')).default;
                Object.assign(window, { JSONBin });
                Object.assign(JSON, { parse: JSONBin.parse, stringify: JSONBin.stringify });
            }
            nrVary.editorText.set({ text: JSONBin.stringify(JSONBin.parse(text), null, indent) });
        }
    },

    addStyle: () => {
        if (!nrVary.domStyle) {
            nrVary.domStyle = document.createElement("style");
            nrVary.domStyle.innerHTML = `
/* light */
:root {
    --global-border: #d8dee4;
    --global-color: #000000;
    --global-bg: #ffffff;
}

/* dark */
body.jse-theme-dark {
    color-scheme: dark;

    --global-border: #585b60;
    --global-color: #d4d4d4;
    --global-bg: #0d1117;

    --jse-menu-color: #d4d4d4;
    --jse-theme-color:#2b3035;

    color: #d4d4d4;
    background-color: #0d1117;
}

* {
    outline: none;
    box-sizing: border-box;
}

html,
body {
    margin: 0;
    padding: 0;
    overflow: hidden;
    --jse-font-size-mono: 18px;
}

.nrg-box {
    --nrc-divider-color: var(--global-border);
}

.nrg-text {
    height: 100vh;
    flex: 1 0 0%;
}

.nrg-split {
    display: flex;
    flex-direction: column;
    justify-content: center;
}

.nrg-tree {
    height: 100vh;
}

.nrg-split button {
    outline: none;
    padding: 0.3em;
    cursor: pointer;
    border-radius: 3px;
    margin: 0.2em 0.6em;
    color: var(--global-color);
    background-color: transparent;
    border: 1px solid var(--global-border);
}

.nrg-split button:hover {
    border-color: deeppink;
}`;
            document.head.appendChild(nrVary.domStyle);
        }
    },
}

export { nrWeb };