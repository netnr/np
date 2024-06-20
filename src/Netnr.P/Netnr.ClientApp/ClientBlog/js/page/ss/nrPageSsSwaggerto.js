import { nrcFile } from "../../../../frame/nrcFile";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrEditor } from "../../../../frame/nrEditor";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/swaggerto",
    ckey: "/ss/swaggerto/content",

    init: async () => {
        nrVary.domEditor1.innerHTML = nrApp.tsLoadingHtml;

        window["jsyaml"] = await import('js-yaml');
        await nrEditor.rely();
        nrVary.domEditor1.innerHTML = "";

        nrcBase.setHeightFromBottom(nrVary.domEditor1);
        let content = await nrStorage.getItem(nrPage.ckey);
        nrApp.tsEditor = nrEditor.create(nrVary.domEditor1, { language: 'json', value: content || "" })
        nrVary.domEditor1.classList.add('border');

        nrPage.editor2 = nrEditor.create(nrVary.domEditor2, { language: "json" });
        nrVary.domEditor2.classList.add('border');
        nrVary.domSwaggerui.classList.add('border');

        await nrPage.openUrl();

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrEditor.onChange(nrApp.tsEditor, async value => {
            await nrStorage.setItem(nrPage.ckey, value);
        })

        //接收文件
        nrcFile.init(async (files) => {
            for (const file of files) {
                let text = await nrcFile.reader(file);
                nrEditor.keepSetValue(nrApp.tsEditor, text);

                break;
            }
        });

        //swagger ui
        nrVary.domBtnSwaggerui.addEventListener('click', async function () {
            nrApp.setLoading(this)
            try {
                nrPage.viewToggle("swagger");

                nrcBase.setHeightFromBottom(nrVary.domSwaggerui)

                let joy = await nrPage.parseJsonOrYaml(nrApp.tsEditor.getValue());
                if (nrEditor.getLanguage(nrApp.tsEditor) != joy.lang) {
                    nrEditor.setLanguage(nrApp.tsEditor, joy.lang);
                }

                nrPage.swaggerUI = SwaggerUIBundle({
                    dom_id: ".nrg-swaggerui",
                    presets: [SwaggerUIBundle.presets.apis, SwaggerUIStandalonePreset],
                    plugins: [SwaggerUIBundle.plugins.DownloadUrl],
                    layout: "StandaloneLayout",
                    spec: joy.data
                });
            } catch (ex) {
                nrApp.logError(ex, '解析错误');
            }

            nrApp.setLoading(this, true)
        })

        // click
        document.body.addEventListener('click', async function (e) {
            let target = e.target;
            let action = target.dataset.action;

            switch (action) {
                case "yaml-json":
                    {
                        nrApp.setLoading(nrVary.domBtnSwaggerui);

                        try {
                            nrPage.viewToggle("editor");
                            nrEditor.setLanguage(nrApp.tsEditor, 'yaml');
                            nrEditor.setLanguage(nrPage.editor2, 'json');

                            let yaml = jsyaml.load(nrApp.tsEditor.getValue());
                            nrEditor.keepSetValue(nrPage.editor2, JSON.stringify(yaml, null, 2));
                        } catch (ex) {
                            nrEditor.keepSetValue(nrPage.editor2, `转换出错：${ex}`);
                        }

                        nrApp.setLoading(nrVary.domBtnSwaggerui, true);
                    }
                    break;
                case "json-yaml":
                    {
                        nrApp.setLoading(nrVary.domBtnSwaggerui);

                        try {
                            nrPage.viewToggle("editor");
                            nrEditor.setLanguage(nrApp.tsEditor, 'json');
                            nrEditor.setLanguage(nrPage.editor2, 'yaml');

                            let json = jsyaml.dump(JSON.parse(nrApp.tsEditor.getValue()));
                            nrEditor.keepSetValue(nrPage.editor2, json);
                        } catch (ex) {
                            nrEditor.keepSetValue(nrPage.editor2, `转换出错：${ex}`);
                        }

                        nrApp.setLoading(nrVary.domBtnSwaggerui, true);
                    }
                    break;
                case "swagger-openapi":
                    {
                        nrApp.setLoading(nrVary.domBtnSwaggerui);

                        try {
                            nrPage.viewToggle("editor");
                            let code1 = nrApp.tsEditor.getValue();
                            let joy = await nrPage.parseJsonOrYaml(code1);
                            nrEditor.setLanguage(nrApp.tsEditor, joy.lang);
                            nrEditor.setLanguage(nrPage.editor2, 'json');

                            let result = await nrPage.swaggerToOpenAPIOnline(code1, joy.lang);
                            nrEditor.keepSetValue(nrPage.editor2, JSON.stringify(result, null, 2));
                        } catch (ex) {
                            nrEditor.keepSetValue(nrPage.editor2, `转换出错：${ex}`);
                        }

                        nrApp.setLoading(nrVary.domBtnSwaggerui, true);
                    }
                    break;

                case "download-markdown":
                case "download-html":
                case "download-png":
                case "download-pdf":
                    {
                        if (nrApp.tsMd && !nrVary.domMarkdown.classList.contains('d-none')) {
                            await nrApp.tsMd.save(action.split('-').pop());
                        } else {
                            nrApp.alert("请先点击 转文档 再下载");
                        }
                    }
                    break;
            }
        });

        //to doc
        nrVary.domBtnTodoc.addEventListener('click', async function () {
            nrApp.setLoading(this);

            try {
                nrPage.viewToggle("markdown");

                if (nrApp.tsMd == null) {

                    //markdown 编辑器
                    await nrcRely.remote("netnrmdEditor");
                    await nrcRely.remote("netnrmd");

                    nrApp.tsMd = netnrmd.init(".nrg-markdown", {
                        theme: nrcBase.isDark() ? "dark" : "light",
                        autosave: false
                    });
                }
                nrApp.tsMd.height(nrVary.domEditor1.clientHeight)

                let result = await nrPage.toOpenAPI(nrApp.tsEditor.getValue());
                if (result) {
                    swg.config.onlyJson = nrVary.domSeOnlyjson.value == "1";
                    nrApp.tsMd.setmd(swg.toMarkdown(result.data));
                } else {
                    nrApp.tsMd.setmd('### 转换出错');
                }
            } catch (ex) {
                nrApp.logError(ex, '转换出错')
            }

            nrApp.setLoading(this, true);
        });

        //demo
        nrVary.domBtnDemo.addEventListener('click', async function () {
            nrApp.setLoading(this);

            await nrPage.openUrl("https://httpbin.org/spec.json")

            nrApp.setLoading(this, true);
        });
    },

    viewToggle: function (name) {

        nrVary.domEditor2.classList.add('d-none');
        nrVary.domSwaggerui.classList.add('d-none');
        nrVary.domMarkdown.classList.add('d-none');

        let col2 = nrVary.domEditor2.parentElement;
        let col1 = col2.previousElementSibling;
        col1.className = "col-md-6";
        col2.className = "col-md-6";

        switch (name) {
            case "editor":
                {
                    nrVary.domEditor2.classList.remove('d-none');
                    nrcBase.setHeightFromBottom(nrVary.domEditor2);
                }
                break;
            case "swagger":
                {
                    nrVary.domSwaggerui.classList.remove('d-none');
                }
                break;
            case "markdown":
                {
                    col1.className = "col-md-4";
                    col2.className = "col-md-8";
                    nrVary.domMarkdown.classList.remove('d-none');
                }
                break;
        }
    },

    /**
     * 从链接打开
     * @param {any} url
     */
    openUrl: async (url) => {
        if (!url) {
            url = location.hash.substring(1);
        }

        if (url.length > 4) {
            nrEditor.keepSetValue(nrApp.tsEditor, '/* loading ... */');

            let result = await nrWeb.reqServer(url, { type: "text" });
            nrEditor.keepSetValue(nrApp.tsEditor, result);
            nrVary.domBtnSwaggerui.click();
            location.hash = "";
        }
    },

    /**
     * 解析 json 或 yaml
     * @param {any} txt
     */
    parseJsonOrYaml: async (txt) => {
        let isFirst = !window["swg"];

        await nrcRely.remote('swaggerui');
        await nrcBase.importScript('/file/ss-swaggerto.js?202307251050');

        //首次，重写方法（该方法依赖异步加载组件）
        if (isFirst) {
            window["FastXmlParser"] = await import('fast-xml-parser');
            swg.jsonToXml = function (json) {
                let result = new FastXmlParser.XMLBuilder().build(json);
                return swg.formatXml(result)
            }
        }

        let pout = { lang: null, data: null, err: null };
        if (txt != null && txt != "") {
            try {
                pout.data = JSON.parse(txt);
                pout.lang = "json";
            } catch (e) {
                try {
                    pout.data = jsyaml.load(txt);
                    pout.lang = "yaml";
                } catch (e) {
                    pout.err = e;
                }
            }
        }
        return pout;
    },

    /**
     * swagger 转 openapi （线上接口转换）
     * @param {any} txt
     * @param {any} lang
     */
    swaggerToOpenAPIOnline: async (txt, lang) => {
        let result = await nrWeb.reqServer('https://converter.swagger.io/api/convert', {
            method: "POST", headers: { "Content-Type": "application/" + lang }, body: txt
        })
        return result;
    },

    /**
     * 转换为 OpenAPI
     * @param {any} txt
     */
    toOpenAPI: async (txt) => {
        let joy = await nrPage.parseJsonOrYaml(txt);

        if (joy.lang) {
            let version = swg.jk("openapi", joy.data) || swg.jk("swagger", joy.data);
            if (version) {
                if (version.split(".")[0] >= 3) {
                    return joy;
                } else {
                    joy.data = await nrPage.swaggerToOpenAPIOnline(txt, joy.lang);
                    return joy;
                }
            }
        }
    },
}

export { nrPage };