import { nrcFile } from "../../../../frame/nrcFile";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrEditor } from "../../../../frame/nrEditor";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/svgo",

    init: async () => {
        nrcBase.setHeightFromBottom(nrVary.domEditor);
        nrcBase.setHeightFromBottom(nrVary.domCardPreview);

        //接收文件
        nrcFile.init(async (files) => {
            nrPage.svgJson = [];
            for (const file of files) {
                let text = await nrcFile.reader(file);
                nrPage.svgJson.push({ name: file.name, text })
            }
            if (nrPage.svgJson.length > 1) {
                nrVary.domBtnMerge.click();
            } else {
                nrVary.domBtnOptimize.click();
            }
        }, nrVary.domTxtFile);

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domBtnMerge.addEventListener('click', async function () {
            nrApp.setLoading(this);

            nrPage.svgOut = await nrPage.optimization(nrPage.svgJson, true);
            await nrPage.viewEditor();

            nrApp.setLoading(this, true);
        });

        nrVary.domBtnOptimize.addEventListener('click', async function () {
            nrApp.setLoading(this);

            nrPage.svgOut = await nrPage.optimization(nrPage.svgJson);
            await nrPage.viewEditor();

            nrApp.setLoading(this, true);
        });

        //下载
        nrVary.domBtnDownload.addEventListener('click', async function () {
            if (nrPage.svgOut) {
                nrApp.setLoading(this)

                if (typeof nrPage.svgOut == "string") {
                    nrcBase.download(nrPage.svgOut, "icon.svg");
                } else {
                    if (nrPage.svgOut.length == 1) {
                        nrcBase.download(nrPage.svgOut[0].data, nrPage.svgOut[0].name);
                    } else {
                        await nrcRely.remote('jszip.js');

                        let zip = new JSZip();
                        nrPage.svgOut.forEach(item => zip.file(item.name, item.data));
                        let content = await zip.generateAsync({ type: "blob" });
                        nrcBase.download(content, "icon.zip");
                    }
                }

                nrApp.setLoading(this, true);
            } else {
                nrApp.toast("请选择或拖拽 SVG 文件后再下载")
            }
        });
    },

    svgJson: [],
    svgOut: null,
    svgMerge: null,

    /**
     * 优化
     * @param {*} json 
     * @param {*} isMerge 是合并
     * @returns 
     */
    optimization: async (json, isMerge) => {
        nrPage.svgMerge = isMerge;
        let results = [];

        await nrcRely.remote('svgo.js');

        json.forEach(item => {
            let result = svgo.optimize(item.text, { multipass: true, name: item.name });
            result.name = item.name;

            result.data = result.data.replace(' class="icon" ', ' ').replace("<defs><style/></defs>", "");

            //合并
            if (isMerge) {
                result.data = result.data.replace('<svg ', `<symbol id="${item.name.replace(".svg", "")}" `)
                    .replace(' xmlns="http://www.w3.org/2000/svg"', '')
                    .replace(/ width="(\d+)"/, "")
                    .replace(/ height="(\d+)"/, "")
                    .replace(/ width="(\d+.\d+)"/, "")
                    .replace(/ height="(\d+.\d+)"/, "")
                    .replace("</svg>", "</symbol>");
                results.push(result.data);
            } else {
                results.push(result);
            }
        })

        //合并
        if (isMerge) {
            results.splice(0, 0, '<svg version="1.1" xmlns="http://www.w3.org/2000/svg" style="display:none;">');
            results.push('</svg>');
            results = results.join('');
        }

        return results;
    },

    /**
     * 显示编辑器
     */
    viewEditor: async () => {
        if (!nrApp.tsEditor) {
            await nrEditor.init();

            nrApp.tsEditor = nrEditor.create(nrVary.domEditor, { language: 'html', wordWrap: 'on' });

            nrApp.tsEditor.addCommand(monaco.KeyCode.PauseBreak, function () {
                nrPage.preview();
            });
        }

        nrVary.domEditor.classList.remove('invisible');
        nrVary.domEditor.classList.add('border');
        nrVary.domCardPreview.classList.remove('invisible');

        let html = [];
        if (nrPage.svgMerge) {
            html.push(nrPage.svgOut);
            html.push("\n<!--使用-->");

            let vdom = document.createElement('div');
            vdom.innerHTML = nrPage.svgOut;
            vdom.querySelectorAll('symbol').forEach(node => {
                let id = node.getAttribute('id');
                html.push(`<svg><use xlink:href="#${node.id}"></use></svg>`);
            });
        } else {
            nrPage.svgOut.forEach(item => {
                html.push(item.data + "\n");
            });
        }

        //赋值
        nrEditor.keepSetValue(nrApp.tsEditor, html.join('\n'));
        //预览
        nrPage.preview();
    },

    preview: () => {
        if (nrApp.tsEditor) {
            let content = nrApp.tsEditor.getValue();

            nrVary.domCardPreview.innerHTML = `<style>
            .nrg-card-preview svg {
                height: 4em;
                margin: .8em 0 .8em .8em;
                max-width: 4em;
            }
            
            .nrg-card-preview svg:hover {
                transition: transform 1.5s;
                transform: rotate(360deg);
            }
            </style> ${content}`;
        }
    }
}

export { nrPage };