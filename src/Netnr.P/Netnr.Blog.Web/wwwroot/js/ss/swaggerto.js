nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor1.getBoundingClientRect().top - 30;
    nr.domEditor1.style.height = vh + 'px';
    nr.domEditor2.style.height = vh + 'px';
    nr.domSwaggerui.style.height = vh + 'px';
    if (window["nmd"]) {
        vh = ch - nmd.obj.container.getBoundingClientRect().top - 30;
        nmd.height(Math.max(200, vh));
    }
}

nr.onReady = function () {

    ss.loading(true);
    me.init().then(() => {

        me.editor1 = me.create(nr.domEditor1, {
            language: 'json',
            value: nr.lsStr('swaggerto-content')
        });
        nr.domEditor1.classList.add('border');

        me.editor2 = me.create(nr.domEditor2, {
            language: 'json',
        });
        nr.domEditor2.classList.add('border');
        nr.domSwaggerui.classList.add('border');

        ss.loading(false);

        me.onChange(me.editor1, function () {
            nr.ls["swaggerto-content"] = me.editor1.getValue();
            nr.lsSave();
        });

        nr.domSeOnlyjson.addEventListener('sl-change', function () {
            page.config.onlyJson = this.value == "1";
        });

        //接收文件
        nr.receiveFiles(function (files) {
            var file = files[0];
            var reader = new FileReader();
            reader.onload = function (e) {
                me.editor1.setValue(e.target.result);
                //
            };
            reader.readAsText(file);
        });

        page.openUrl();

        // Convert
        nr.domBtnConvert.addEventListener('sl-select', function (e) {
            var action = e.detail.item.innerText;
            switch (action) {
                case "YAML → JSON":
                    {
                        page.viewToggle("editor");
                        me.setLanguage(me.editor1, 'yaml');
                        me.setLanguage(me.editor2, 'json');

                        try {
                            var yaml = jsyaml.load(me.editor1.getValue());
                            me.editor2.setValue(JSON.stringify(yaml, null, 2));
                        } catch (e) {
                            me.editor2.setValue(e + "");
                        }
                    }
                    break;
                case "JSON → YAML":
                    {
                        page.viewToggle("editor");
                        me.setLanguage(me.editor1, 'json');
                        me.setLanguage(me.editor2, 'yaml');

                        try {
                            var json = jsyaml.dump(JSON.parse(me.editor1.getValue()));
                            me.editor2.setValue(json);
                        } catch (e) {
                            me.editor2.setValue(e + "");
                        }
                    }
                    break;
                case "Swagger → OpenAPI":
                    {
                        page.viewToggle("editor");

                        var code1 = me.editor1.getValue();
                        var pobj = page.parseJsonOrYaml(code1);
                        if (pobj.lang != null) {
                            ss.loading(true);

                            me.setLanguage(me.editor1, pobj.lang);
                            me.setLanguage(me.editor2, 'json');

                            page.swaggerConvert(code1, pobj.lang).then(res => {
                                ss.loading(false);
                                me.editor2.setValue(JSON.stringify(res, null, 2));
                            }).catch(err => {
                                ss.loading(false);

                                me.editor2.setValue(err + "");
                            })
                        } else {
                            me.editor2.setValue("不是有效的 Json 或 Yaml");
                        }
                    }
                    break;
            }
        });

        document.body.addEventListener('click', function (e) {
            var target = e.target;
            if ('SL-BUTTON' == target.nodeName) {
                var action = target.innerText;
                switch (action) {
                    case 'SwaggerUI':
                        {
                            page.viewSwaggerUI();
                        }
                        break;
                    case "转文档":
                        {
                            page.viewToggle("markdown");

                            if (!("nmd" in window)) {
                                window.nmd = new netnrmd(".nr-markdown", { autosave: false });
                                nr.changeSize()
                            }

                            ss.loading(true);
                            page.convertOpenAPI(me.editor1.getValue()).then(res => {
                                ss.loading(false);
                                page.swaggerJson = res.data;
                                nmd.setmd(page.jsonToMarkdown(page.swaggerJson))
                            }).catch(err => {
                                ss.loading(false);
                                nmd.setmd(err + "");
                            })
                        }
                        break;
                    case "Demo":
                        {
                            page.openUrl("https://httpbin.org/spec.json")
                        }
                        break;
                }
            }
        });

        //download
        nr.domBtnDownload.addEventListener('sl-select', function (e) {
            if (window["nmd"] == null || nr.domMarkdown.classList.contains("d-none")) {
                nr.alert("请先点击 转文档 再下载");
            } else {
                var action = e.detail.item.innerText.toLowerCase();
                switch (action) {
                    case "markdown":
                        nr.download(nmd.getmd(), "swagger.md");
                        break;
                    case "html":
                    case "word":
                        {
                            var netnrmd_body = nmd.gethtml();
                            fetch("https://npm.elemecdn.com/netnrmd@3.0.3/src/netnrmd.css").then(resp => resp.text()).then(netnrmd_style => {
                                var html = `<!DOCTYPE html>
                                <html>
                                    <head>
                                    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
                                    <style type="text/css">
                                        ${netnrmd_style}
                                    </style>
                                    </head>
                                    <body>
                                    <div class="markdown-body">${netnrmd_body}</div>
                                    </body>
                                </html>`;

                                if (action == "html") {
                                    nr.download(html, "swagger.html");
                                } else if (action == "word") {
                                    require(['https://npm.elemecdn.com/html-docx-js@0.3.1/dist/html-docx.js'], function (module) {
                                        nr.download(module.asBlob(html), "swagger.docx");
                                    });
                                }
                            })
                        }
                        break;
                    case "pdf":
                        {
                            require(["https://lf6-cdn-tos.bytecdntp.com/cdn/expire-1-y/html2pdf.js/0.9.3/html2pdf.bundle.min.js"], function (module) {
                                var ch = nmd.obj.view.clientHeight;
                                nmd.obj.view.style.height = 'auto';
                                var vm = nmd.obj.viewmodel;
                                nmd.toggleView(3);
                                module(nmd.obj.view, {
                                    margin: 3,
                                    filename: 'swagger.pdf',
                                    html2canvas: { scale: 1 }
                                }).then(function () {
                                    nmd.obj.view.style.height = ch + 'px';
                                    nmd.toggleView(vm);
                                })
                            })
                        }
                        break;
                    case "png":
                        {
                            var backvm = false;
                            if (nmd.obj.viewmodel == 1) {
                                nmd.toggleView(2);
                                backvm = true;
                            }

                            require(['https://npm.elemecdn.com/html2canvas@1.4.1/dist/html2canvas.min.js'], function (module) {
                                var ch = nmd.obj.view.clientHeight;
                                nmd.obj.view.style.height = 'auto';
                                module(nmd.obj.view, {
                                    scale: 1,
                                    margin: 15
                                }).then(function (canvas) {
                                    nmd.obj.view.style.height = ch + 'px';
                                    nr.download(canvas, "swagger.png");

                                    if (backvm) {
                                        nmd.toggleView(1);
                                    }
                                })
                            })
                        }
                        break;
                }
            }
        });
    });
}

var page = {
    config: {
        //仅保留 Json 格式
        onlyJson: true
    },

    viewToggle: function (name) {

        nr.domEditor2.classList.add('d-none');
        nr.domSwaggerui.classList.add('d-none');
        nr.domMarkdown.classList.add('d-none');

        var col2 = nr.domEditor2.parentElement;
        var col1 = col2.previousElementSibling;
        col1.className = "col-md-6";
        col2.className = "col-md-6";

        switch (name) {
            case "editor":
                {
                    nr.domEditor2.classList.remove('d-none');
                }
                break;
            case "swagger":
                {
                    nr.domSwaggerui.classList.remove('d-none');
                }
                break;
            case "markdown":
                {
                    col1.className = "col-md-4";
                    col2.className = "col-md-8";
                    nr.domMarkdown.classList.remove('d-none');
                }
                break;
        }
    },

    viewSwaggerUI: function () {
        page.viewToggle("swagger");

        var code1 = me.editor1.getValue();
        var pobj = page.parseJsonOrYaml(code1);
        if (me.getLanguage(me.editor1) != pobj.lang) {
            me.setLanguage(me.editor1, pobj.lang);
        }

        var suiops = {
            dom_id: ".nr-swaggerui",
            presets: [
                SwaggerUIBundle.presets.apis,
                SwaggerUIStandalonePreset
            ],
            plugins: [
                SwaggerUIBundle.plugins.DownloadUrl
            ],
            layout: "StandaloneLayout"
        };
        if (pobj.lang) {
            suiops.spec = pobj.data;
        }
        page.swaggerUI = SwaggerUIBundle(suiops);
    },

    /**
     * 从链接打开
     * @param {any} url
     */
    openUrl: function (url) {
        if (url == null) {
            url = location.hash.substring(1);
        }

        if (url.length > 4) {
            me.editor1.setValue("Loading ...");

            fetch(url).then(resp => resp.text()).then(res => {
                me.editor1.setValue(res);
                page.viewSwaggerUI();

                location.hash = "";
            }).catch(ex => {
                console.debug(ex);
                me.editor1.setValue(ex + "");
            })
        }
    },

    /**
     * Swagger JSON => Markdown
     * @param {any} swaggerJson
     */
    jsonToMarkdown: function (swaggerJson) {

        var mds = [];

        var title = page.jk("info:title", swaggerJson);
        var version = page.jk("info:version", swaggerJson);
        if (title != null) {
            mds.push(page.formatter.title(title, version));
        }

        var description = page.jk("info:description", swaggerJson);
        if (description != null) {
            mds.push(page.formatter.description(description));
        }

        var mdg = [];

        var paths = swaggerJson["paths"];
        if (paths) {
            for (var d1 in paths) {
                var path = paths[d1];
                for (var d2 in path) {
                    var i2 = path[d2];
                    var tags = page.jk("tags", i2);
                    var mdi = [];

                    mdi.push(page.formatter.path(d2, d1));
                    if ("summary" in i2) {
                        mdi.push(page.formatter.summary(i2.summary));
                    }

                    if ("parameters" in i2) {
                        mdi.push(page.formatter.parameters(i2.parameters, swaggerJson));
                    }

                    if ("requestBody" in i2) {
                        var requestBody = i2.requestBody;
                        mdi.push(page.formatter.requestBody(requestBody, swaggerJson));
                    }

                    if ("responses" in i2) {
                        mdi.push(page.formatter.responses(i2.responses, swaggerJson));
                    }

                    mdg.push({ tag: tags[0], mdi });
                }
            }
        }

        var tags = page.jk("tags", swaggerJson);
        tags.forEach(tag => {
            mds.push(page.formatter.tags_item(tag));

            mdg.filter(x => x.tag == tag.name).forEach(x => mds.push(x.mdi.join("\n")));
        });

        mdg.filter(x => x.tag == null).forEach(x => mds.push(x.mdi.join("\n")));

        return mds.join("\n");
    },

    /**
     * 解析 json 或 yaml
     * @param {any} txt
     */
    parseJsonOrYaml: function (txt) {
        var pout = { lang: null, data: null, err: null };
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
    swaggerConvertOnline: function (txt, lang) {
        return fetch("https://converter.swagger.io/api/convert", {
            method: "POST",
            body: txt,
            headers: {
                "Content-Type": "application/" + lang
            }
        }).then(x => x.json())
    },

    /**
     * swagger 转 openapi
     * @param {any} txt
     */
    swaggerConvert: function (txt) {
        var po = page.parseJsonOrYaml(txt);
        return APISpecConverter.convert({
            from: 'swagger_' + ((po.data.swagger && po.data.swagger.indexOf("1.") == 0) ? "1" : "2"),
            to: 'openapi_3',
            source: po.data,
        }).then(c => {
            return c.spec;
        });
    },

    /**
     * 转换为 OpenAPI
     * @param {any} txt
     */
    convertOpenAPI: function (txt) {
        return new Promise(function (resolve, reject) {
            var pout = page.parseJsonOrYaml(txt);
            if (pout.lang) {
                var version = page.jk("openapi", pout.data) || page.jk("swagger", pout.data);
                if (version) {
                    if (version.split(".")[0] >= 3) {
                        resolve(pout);
                    } else {
                        page.swaggerConvert(txt, pout.lang).then(res => {
                            pout.data = res;
                            resolve(pout);
                        }).catch(err => {
                            reject(err);
                        })
                    }
                } else {
                    reject("bad");
                }
            } else {
                reject("");
            }
        });
    },

    //格式化
    formatter: {

        /**
         * 标题
         * @param {any} title 标题
         * @param {any} version 版本
         */
        title: function (title, version) {
            if (version != null && version != "") {
                version = " <sup>`" + version + "`</sup>";
            } else {
                version = "";
            }

            var mds = [];
            mds.push("# " + title + version);
            mds.push("");
            return mds.join('\n');
        },

        /**
         * 描述
         * @param {any} description 描述
         */
        description: function (description) {
            return page.mdEncode(description);
        },

        /**
         * 分组标签
         * @param {any} tag 分组标签一项
         */
        tags_item: function (tag) {
            var name = page.jk("name", tag);
            var description = page.mdEncode(page.jk("description", tag) || "");

            var mds = [];
            mds.push("");
            mds.push("## " + name);
            mds.push(description);
            return mds.join('\n');
        },

        /**
         * 路径
         * @param {any} method 请求方式
         * @param {any} path 路径
         */
        path: function (method, path) {
            var mds = [];
            mds.push("");
            mds.push("### " + method.toUpperCase() + " " + path);
            return mds.join('\n');
        },

        /**
         * 描述
         * @param {any} summary 描述
         */
        summary: function (summary) {
            var mds = [];
            mds.push("");
            mds.push("#### 描述（Summary）");
            mds.push(page.mdEncode(summary));
            return mds.join('\n');
        },

        /**
         * 请求参数
         * @param {any} parameters 请求参数主体
         */
        parameters: function (parameters, swaggerJson) {
            var mds = [];
            mds.push("");
            mds.push("#### 参数（Parameters）");

            page.mdTableHeader(mds, "名称,类型,位置,说明".split(','));
            parameters.forEach(p => {
                var type = [page.jk("type", p), page.jk("schema:type", p), page.jk("schema:format", p), page.jk("schema:items:type", p), page.jk("schema:items:enum", p) ? "enum" : null];
                type = type.filter(x => x != null);
                if (type.length) {
                    type = type.join(' / ');
                } else {
                    var refName = page.jk("schema:$ref", p);
                    if (refName) {
                        type = page.refForType(refName, swaggerJson);
                    } else {
                        type = "";
                    }
                }

                var description = page.mdEncode(p.description || ""),
                    minimum = page.jk("schema:minimum", p),
                    maximum = page.jk("schema:maximum", p), mm = [];

                if (minimum != null) {
                    mm.push(minimum);
                }
                mm.push(p.name);
                if (maximum != null) {
                    mm.push(maximum);
                }
                if (mm.length > 1) {
                    description = "**_" + mm.join(" ≤ ") + "_** 。 " + description;
                }
                if (p.required) {
                    description = "**必填** 。 " + description;
                }
                mds.push(`| ${p.name} | ${type} | ${p.in} | ${description} |`);
            });
            return mds.join('\n');
        },

        /**
         * 请求主体
         * @param {any} requestBody 请求主体
         * @param {any} swaggerJson 源
         */
        requestBody: function (requestBody, swaggerJson) {
            var rb_content = requestBody.content;
            var rb_description = page.jk("description", requestBody);

            var mds = [];
            mds.push("");
            mds.push("#### 请求主体（RequestBody）");
            if (rb_description != null) {
                mds.push(page.mdEncode(rb_description));
            }

            if (page.config.onlyJson) {
                rb_content = page.onlyJsonForContent(rb_content);
            }

            for (var i in rb_content) {
                mds.push("");
                mds.push("**" + i.replace(/\*/g, "\\*") + "**");
                mds.push("");

                var cformat = i.split('/')[1];
                if (["json", "xml", "html"].indexOf(cformat) == -1) {
                    cformat = "";
                }

                var properties = page.jk("schema:properties", rb_content[i]);

                if (properties != null) {

                    page.mdTableHeader(mds, "名称,类型,说明".split(','));

                    for (var j in properties) {
                        var pj = properties[j];

                        var type = page.jk("format", pj) || page.jk("type", pj);
                        if (type == null) {
                            var refName = page.jk("$ref", pj);
                            if (refName) {
                                type = page.refForType(refName, swaggerJson);
                            }
                        }

                        var description = page.jk("description", pj) || "";
                        if ("default" in pj) {
                            var dv = "**默认值**：";
                            if (pj.default == "") {
                                dv += '"" 。 ';
                            } else {
                                dv += "`" + pj.default + "` 。 ";
                            }
                            description = dv + description;
                        }

                        mds.push(`| ${j} | ${type || ""} | ${description} |`);
                    }
                } else {
                    var rbcRefName1 = page.jk("schema:$ref", rb_content[i]);
                    var rbcRefName2 = page.jk("schema:items:$ref", rb_content[i]);
                    if (rbcRefName1 != null) {
                        var rout = page.buildRefTree(rbcRefName1, swaggerJson);
                        var ov = rout.obj;

                        if (cformat == "xml") {
                            ov = page.jsonToXml(ov, rbcRefName1.split("/").pop())
                        } else {
                            ov = JSON.stringify(ov, null, 2);
                        }

                        mds.push("```" + cformat);
                        mds.push(ov);
                        mds.push("```");
                        mds.push("");

                        page.mdTableHeader(mds, "名称,类型,说明".split(','));
                        mds = mds.concat(page.fieldsAsView(rout.fields));
                    } else if (rbcRefName2 != null) {
                        var rout = page.buildRefTree(rbcRefName2, swaggerJson);
                        var ov = rout.obj;

                        var rbcSchemaType = page.jk("schema:type", rb_content[i]);
                        if (rbcSchemaType == "array") {
                            ov = [ov];
                        }

                        if (cformat == "xml") {
                            ov = page.jsonToXml(ov, rbcRefName2.split("/").pop())
                        } else {
                            ov = JSON.stringify(ov, null, 2);
                        }

                        mds.push("```" + cformat);
                        mds.push(ov);
                        mds.push("```");
                        mds.push("");

                        page.mdTableHeader(mds, "名称,类型,说明".split(','));
                        mds = mds.concat(page.fieldsAsView(rout.fields));
                    }
                }
            }

            return mds.join('\n');
        },

        /**
         * 响应
         * @param {any} responses 响应主体
         * @param {any} swaggerJson swagger源
         */
        responses: function (responses, swaggerJson) {
            var mds = [];
            mds.push("");
            mds.push("#### 响应（Responses）");
            for (var i in responses) {
                var ri = responses[i];
                mds.push("");
                mds.push("```html");
                mds.push(`Status: ${i} ${ri.description}`);
                mds.push("```");

                if ("content" in ri) {
                    var content = ri.content;

                    if (page.config.onlyJson) {
                        content = page.onlyJsonForContent(content);
                    }

                    for (var j in content) {
                        mds.push("");
                        mds.push("**" + j.replace(/\*/g, "\\*") + "**");
                        mds.push("");

                        var cformat = j.split('/')[1];
                        if (["json", "xml", "html"].indexOf(cformat) == -1) {
                            cformat = "";
                        }

                        var cSchemaRefName = page.jk("schema:$ref", content[j]);

                        var cSchemaType = page.jk("schema:type", content[j]);

                        var cSchemaItemsType = page.jk("schema:items:type", content[j]);

                        var cSchemaItemsRefName = page.jk("schema:items:$ref", content[j]);
                        if (cSchemaRefName) {
                            var rout = page.buildRefTree(cSchemaRefName, swaggerJson);
                            var ov = rout.obj;

                            if (cformat == "xml") {
                                ov = page.jsonToXml(ov, cSchemaRefName.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                            mds.push("");

                            page.mdTableHeader(mds, "名称,类型,说明".split(','));
                            mds = mds.concat(page.fieldsAsView(rout.fields));
                        } else if (cSchemaItemsType) {
                            var ov = page.buildTypeData(cSchemaItemsType);
                            if (cSchemaType == "array") {
                                ov = [ov];
                            }

                            if (cformat == "xml") {
                                ov = page.jsonToXml(ov, cSchemaItemsType.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                        } else if (cSchemaItemsRefName) {
                            var rout = page.buildRefTree(cSchemaItemsRefName, swaggerJson);
                            var ov = rout.obj;

                            if (cSchemaType == "array") {
                                ov = [ov];
                            }

                            if (cformat == "xml") {
                                ov = page.jsonToXml(ov, cSchemaItemsRefName.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                            mds.push("");

                            page.mdTableHeader(mds, "名称,类型,说明".split(','));
                            mds = mds.concat(page.fieldsAsView(rout.fields));
                        } else {
                            var cstype = page.jk("schema:type", content[j]);
                            var csAdditionalProperties = page.jk("schema:additionalProperties", content[j]);
                            if (cstype && csAdditionalProperties) {

                                var cSchema = page.jk("schema", content[j]);
                                var rout = page.buildRefTree(cSchema, swaggerJson);
                                var ov = rout.obj;
                                if (cformat == "xml") {
                                    ov = page.jsonToXml(ov)
                                } else {
                                    ov = JSON.stringify(ov, null, 2);
                                }

                                mds.push("```" + cformat);
                                mds.push(ov);
                                mds.push("```");
                                mds.push("");

                                page.mdTableHeader(mds, "名称,类型,说明".split(','));
                                mds = mds.concat(page.fieldsAsView(rout.fields));
                            }
                        }
                    }
                }

                if ("headers" in ri) {
                    var headers = ri.headers;

                    mds.push("");
                    mds.push("**头部（Headers）**");
                    mds.push("");

                    page.mdTableHeader(mds, "名称,类型,说明".split(','));

                    for (var j in headers) {
                        var type = [page.jk("schema:type", headers[j]), page.jk("schema:format", headers[j])].join(' / ');
                        var description = page.mdEncode(page.jk("description", headers[j]));
                        mds.push(`| ${j} | ${type} | ${description} |`);
                    }
                    mds.push("");
                }
            }
            return mds.join('\n');
        }
    },

    /**
     * 仅保留 JSON
     * @param {any} content
     */
    onlyJsonForContent: function (content) {
        if (content) {
            var ctk = Object.keys(content), keepKey;
            if (ctk.indexOf("application/json") >= 0) {
                keepKey = "application/json";
            } else if (ctk.indexOf("text/json") >= 0) {
                keepKey = "text/json";
            }
            if (keepKey) {
                ctk.forEach(k => {
                    if (k != keepKey) {
                        delete content[k]
                    }
                })
            }
        }

        return content;
    },

    /**
     * Markdown 编码
     * @param {any} md
     */
    mdEncode: function (md) {
        if (md != null) {
            md = md.replace(/\~/g, "\\~").replace(/\`/g, "\\`");
        }
        return md;
    },

    /**
     * 获取值
     * @param {any} key 键，以:分隔子对象
     * @param {any} json
     */
    jk: function (key, json) {
        var cj = json;
        key.split(':').forEach(k => {
            if (cj != null) {
                cj = cj[k];
            }
        })
        return cj;
    },

    /**
     * $ref 转为 key1:key2:key3
     * @param {any} refName
     */
    refAsKey: function (refName) {
        return refName.substr(2).replace(/\//g, ":");
    },

    /**
     * 获取 $ref 类型
     * @param {any} refName
     * @param {any} swaggerJson
     */
    refForType: function (refName, swaggerJson) {
        var refCobj = page.jk(page.refAsKey(refName), swaggerJson);
        var type = page.jk("type", refCobj);
        if ("enum" in refCobj) {
            type = "enum(" + refName.split("/").pop() + ")";
        }
        return type;
    },

    /**
     * JSON => XML
     * @param {any} json
     * @param {any} rootName xml root name
     */
    jsonToXml: function (json, rootName) {
        if (rootName != null) {
            var obj = {};
            obj[rootName] = json;
            json = obj;
        }

        var xml = new X2JS({
            attributePrefix: "@"
        }).json2xml_str(json);

        return page.formatXml(xml);
    },

    /**
     * 表格头部
     * @param {any} cols
     * @param {any} cols
     */
    mdTableHeader: function (mds, cols) {
        var md1 = [], md2 = [];

        cols.splice(0, 0, '');
        cols.push('');
        cols.forEach(col => {
            md1.push(col);
            var hrs = [];
            while (hrs.length < col.length) {
                hrs.push('--');
            }
            md2.push(hrs.join(''));
        })

        mds.push(md1.join(' | ').trim());
        mds.push(md2.join(' | ').trim());
    },

    /**
     * 字段
     * @param {any} fields
     * @param {any} deep
     */
    fieldsAsView: function (fields, deep) {
        var list = [], deep = deep || 0, empty = [];
        while (empty.length < deep) {
            empty.push(' —');
        }
        fields.forEach(item => {
            var type = [item.type];
            if (item.format) {
                type.push(item.format)
            }
            type = type.join(' / ');

            var description = item.description || "";
            if (item.default != null) {
                description = "**默认值**：" + (item.default === '' ? '"" 。 ' : '`' + item.default + '` 。 ') + description;
            }

            list.push(`|${empty} ${item.field} | ${type} | ${description} |`);
            if (item.children) {
                list = list.concat(arguments.callee(item.children, deep + 1))
            }
        });
        return list;
    },

    /**
     * 格式化
     * @param {any} xml
     */
    formatXml: function (xml) {
        var formatted = '';
        var reg = /(>)(<)(\/*)/g;
        xml = xml.replace(reg, '$1\r\n$2$3');
        var pad = 0;
        $.each(xml.split('\r\n'), function (index, node) {
            var indent = 0;
            if (node.match(/.+<\/\w[^>]*>$/)) {
                indent = 0;
            } else if (node.match(/^<\/\w/)) {
                if (pad != 0) {
                    pad -= 1;
                }
            } else if (node.match(/^<\w[^>]*[^\/]>.*$/)) {
                indent = 1;
            } else {
                indent = 0;
            }

            var padding = '';
            for (var i = 0; i < pad; i++) {
                padding += '  ';
            }

            formatted += padding + node + '\r\n';
            pad += indent;
        });

        return formatted;
    },

    /**
     * 根据 $ref 引用构建树
     * @param {any} refName 引用名 或 additionalSchema
     * @param {any} swaggerJson 源
     */
    buildRefTree: function (refName, swaggerJson) {

        var typeObj = refName;
        if (typeof refName == "string") {
            typeObj = page.jk(page.refAsKey(refName), swaggerJson);
        }

        var properties = page.jk("properties", typeObj);
        var aProperties = page.jk("additionalProperties", typeObj);
        if (properties == null && aProperties) {
            properties = {};
            for (var f = 1; f < 4; f++) {
                properties["additionalProp" + f] = aProperties;
            }
        }

        var obj = {}, fields = [];

        for (var k in properties) {

            var ptype = page.jk("type", properties[k]);
            var pformat = page.jk("format", properties[k]);
            var pnullable = page.jk("nullable", properties[k]);
            var pdescription = page.jk("description", properties[k]);
            var pdefault = page.jk("default", properties[k]);
            var penum = page.jk("enum", properties[k]);

            var pexample = page.jk("example", properties[k]);

            var objRefName = page.jk("$ref", properties[k]), ov;
            if (objRefName) {
                ptype = "object";
            }

            var fieldObj = { field: k, type: ptype, format: pformat, nullable: pnullable, default: pdefault, description: pdescription, children: null };

            switch (ptype) {
                case "number":
                case "integer":
                    ov = 0;
                    break;
                case "string":
                    {
                        if (penum) {
                            ov = penum[0];
                            fieldObj.format = "enum";
                        } else {
                            switch (pformat) {
                                case "date-time":
                                    ov = new Date().toISOString();
                                    break;
                                default:
                                    ov = "string";
                            }
                        }
                    }
                    break;
                case "boolean":
                    ov = pdefault == null ? false : pdefault;
                    break;
                case "object":
                    {
                        var rout = arguments.callee(objRefName, swaggerJson);
                        ov = rout.obj;
                        fieldObj.children = rout.fields;
                    }
                    break;
                case "array":
                    {
                        ov = [];
                        var arrRefName = page.jk("items:$ref", properties[k]);
                        var arrItemsType = page.jk("items:type", properties[k]);

                        if (arrRefName) {
                            if (arrRefName == refName) {
                                ov.push("circular references 循环引用");
                            } else {
                                var rout = arguments.callee(arrRefName, swaggerJson)
                                fieldObj.children = rout.fields;
                                ov.push(rout.obj);
                            }
                        } else if (arrItemsType) {
                            ov.push([page.buildTypeData(arrItemsType)])
                        }
                    }
                    break;
                default: {
                    ov = {};
                    fieldObj.type = "object";
                };
            }

            if (pexample != null) {
                ov = pexample;
            }

            obj[k] = ov;
            fields.push(fieldObj);
        }

        return { obj, fields };
    },

    /**
     * 根据 $ref 引用构建数据
     * @param {any} refName 引用名 或 additionalSchema
     * @param {any} swaggerJson 源
     */
    buildRefData: function (refName, swaggerJson) {

        var typeObj = refName;
        if (typeof refName == "string") {
            typeObj = page.jk(page.refAsKey(refName), swaggerJson);
        }

        var properties = page.jk("properties", typeObj);
        var aProperties = page.jk("additionalProperties", typeObj);
        if (properties == null && aProperties) {
            properties = {};
            for (var f = 1; f < 4; f++) {
                properties["additionalProp" + f] = aProperties;
            }
        }

        var obj = {};
        for (var k in properties) {
            var ptype = page.jk("type", properties[k]);

            var objRefName = page.jk("$ref", properties[k]);
            var ov;
            if (objRefName) {
                ptype = "object";
            }

            switch (ptype) {
                case "number":
                case "integer":
                    ov = 0;
                    break;
                case "string":
                    {
                        var ve = page.jk("enum", properties[k]);
                        if (ve) {
                            ov = ve[0];
                        }

                        var format = page.jk("format", properties[k]);
                        switch (format == "date-time") {
                            case "date-time":
                                ov = new Date().toISOString();
                                break;
                            default:
                                ov = "string";
                        }
                    }
                    break;
                case "boolean":
                    {
                        var vd = page.jk("default", properties[k]);
                        if (vd != null) {
                            ov = vd;
                        } else {
                            ov = false;
                        }
                    }
                    break;
                case "object":
                    ov = arguments.callee(objRefName, swaggerJson);
                    break;
                case "array":
                    {
                        ov = [];
                        var arrRefName = page.jk("items:$ref", properties[k]);
                        var arrItemsType = page.jk("items:type", properties[k]);
                        if (arrRefName) {
                            ov.push(page.buildRefData(arrRefName, swaggerJson));
                        } else if (arrItemsType) {
                            ov.push([arguments.callee(arrItemsType)])
                        }
                    }
                    break;
                default: ov = {};
            }
            var example = page.jk("example", properties[k]);
            if (example != null) {
                ov = example;
            }

            obj[k] = ov;
        }

        return obj;
    },

    /**
     * 构建类型数据
     * @param {any} type 类型
     */
    buildTypeData: function (type) {
        switch (type) {
            case "string":
                return "string";
            case "integer":
                return 0;
            case "boolean":
                return false;
        }
        return null;
    }
};