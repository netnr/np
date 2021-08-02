var st = {

    init: function () {

        $(window).on('load resize', function () {

            var es1 = $(".nrEditorSwagger1"), es2 = $(".nrEditorSwagger2"), sui = $('.nrSwaggerUI');
            var vh = Math.max(100, $(window).height() - es1.offset().top - 20);
            es1.height(vh);
            es2.height(vh);
            sui.height(vh);

            if ("nmd" in window) {
                var ch = $(window).height() - nmd.obj.container.getBoundingClientRect().top - 20;
                nmd.height(Math.max(200, ch));
            }
        }).on('hashchange', function () {
            st.openUrl();
        });

        require(['vs/editor/editor.main'], function () {

            window.editor1 = monaco.editor.create(document.querySelector(".nrEditorSwagger1"), ss.meConfig({
                value: ss.lsStr("swaggerto-content"),
                language: "json"
            }));

            window.editor2 = monaco.editor.create(document.querySelector(".nrEditorSwagger2"), ss.meConfig({
                language: "json"
            }));

            //自动保存
            editor1.onDidChangeModelContent(function () {
                clearTimeout(window.defer1);
                window.defer1 = setTimeout(function () {
                    ss.ls["swaggerto-content"] = editor1.getValue();
                    ss.lsSave();
                }, 1000 * 1)
            });

            $('.nrBtnSwaggerUI').click(function () {
                $('.nrOutBox').children().addClass("d-none");
                $('.nrSwaggerUI').removeClass("d-none");
                $('.nrOutBox').removeClass("col-md-8").addClass("col-md-6").prev().removeClass("col-md-4").addClass("col-md-6");

                var txt = editor1.getValue();
                var pout = st.parseJsonOrYaml(txt);
                if (editor1.getModel()._languageIdentifier.language != pout.lang) {
                    st.setEditorLanguage(editor1, pout.lang);
                }

                var suiops = {
                    dom_id: ".nrSwaggerUI",
                    presets: [
                        SwaggerUIBundle.presets.apis,
                        SwaggerUIStandalonePreset
                    ],
                    plugins: [
                        SwaggerUIBundle.plugins.DownloadUrl
                    ],
                    layout: "StandaloneLayout"
                };
                if (pout.lang) {
                    suiops.spec = pout.data;
                }
                st.swaggerUI = SwaggerUIBundle(suiops);
            });

            $('.nrBtnYamlToJson').click(function () {
                $('.nrOutBox').children().addClass("d-none");
                $('.nrEditorSwagger2').removeClass("d-none");
                $('.nrOutBox').removeClass("col-md-8").addClass("col-md-6").prev().removeClass("col-md-4").addClass("col-md-6");

                st.setEditorLanguage(editor1, "yaml");
                st.setEditorLanguage(editor2, "json");
                try {
                    var yaml = jsyaml.load(editor1.getValue());
                    editor2.setValue(JSON.stringify(yaml, null, 4));
                } catch (e) {
                    editor2.setValue(e + "");
                }
            });

            $('.nrBtnJsonToYaml').click(function () {
                $('.nrOutBox').children().addClass("d-none");
                $('.nrEditorSwagger2').removeClass("d-none");
                $('.nrOutBox').removeClass("col-md-8").addClass("col-md-6").prev().removeClass("col-md-4").addClass("col-md-6");

                st.setEditorLanguage(editor1, "json");
                st.setEditorLanguage(editor2, "yaml");
                try {
                    var json = jsyaml.safeDump(JSON.parse(editor1.getValue()));
                    editor2.setValue(json);
                } catch (e) {
                    editor2.setValue(e + "");
                }
            });

            $('.nrBtnSwaggerToOpenAPI').click(function () {

                $('.nrOutBox').children().addClass("d-none");
                $('.nrEditorSwagger2').removeClass("d-none");
                $('.nrOutBox').removeClass("col-md-8").addClass("col-md-6").prev().removeClass("col-md-4").addClass("col-md-6");

                var txt = editor1.getValue();
                var pout = st.parseJsonOrYaml(txt);
                if (pout.lang != null) {
                    ss.loading(1);

                    st.setEditorLanguage(editor1, pout.lang);
                    st.setEditorLanguage(editor2, "json");

                    st.swaggerConvert(txt, pout.lang).then(res => {
                        ss.loading(0);

                        editor2.setValue(JSON.stringify(res, null, 4));
                    }).catch(err => {
                        ss.loading(0);

                        editor2.setValue(err + "");
                    })
                } else {
                    editor2.setValue("不是有效的 Json 或 Yaml");
                }
            });

            $('.nrTo').click(function () {

                $('.nrOutBox').children().addClass("d-none");
                $('.nrEditorSwagger3').removeClass("d-none");
                $('.nrOutBox').removeClass("col-md-6").addClass("col-md-8").prev().removeClass("col-md-6").addClass("col-md-4");

                if (!("nmd" in window)) {
                    window.nmd = new netnrmd(".nrMarkdownEditor", {
                        autosave: false
                    });

                    var ch = $(window).height() - nmd.obj.container.getBoundingClientRect().top - 20;
                    nmd.height(Math.max(200, ch));
                }

                ss.loading(1);
                st.convertOpenAPI(editor1.getValue()).then(res => {
                    ss.loading(0);
                    st.swaggerJson = res.data;
                    nmd.setmd(st.jsonToMarkdown(st.swaggerJson))
                }).catch(err => {
                    ss.loading(0);
                    nmd.setmd(err + "");
                })
            });

            $('.nrConfigOnlyJson').change(function () {
                st.config.onlyJson = this.value == "1";
            });

            netnrmd.down = function (content, file) {
                var aTag = document.createElement('a');
                aTag.download = file;
                if (content.nodeType == 1) {
                    aTag.href = content.toDataURL();
                } else {
                    var blob = new Blob([content]);
                    aTag.href = URL.createObjectURL(blob);
                }
                document.body.appendChild(aTag);
                aTag.click();
                aTag.remove();
            }

            $('.nrDown').click(function (e) {
                
                if (("nmd" in window) && !$('.nrEditorSwagger3').hasClass("d-none")) {
                    var target = e.target;
                    var that = nmd, bv = target.innerHTML.toLowerCase();
                    switch (bv) {
                        case "markdown":
                            netnrmd.down(that.getmd(), 'swagger.md')
                            break;
                        case "html":
                        case "word":
                            {
                                var netnrmd_body = that.gethtml();
                                $.get("https://code.bdstatic.com/npm/netnrmd@2.6.3/src/netnrmd.css", null, function (netnrmd_style) {
                                    var html = `
                                                <!DOCTYPE html>
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
                                                </html>
                                            `;

                                    if (bv == "html") {
                                        netnrmd.down(html, 'swagger.html');
                                    }
                                    else if (bv == "word") {
                                        require(['https://code.bdstatic.com/npm/html-docx-js@0.3.1/dist/html-docx.min.js'], function (module) {
                                            netnrmd.down(module.asBlob(html), "swagger.docx");
                                        });
                                    }
                                });
                            }
                            break;
                        case "pdf":
                            require(['https://cdn.netnr.eu.org/libs/html2pdf/0.9.3/html2pdf.bundle.min.js'], function (module) {
                                var ch = that.obj.view.height();
                                that.obj.view.height('auto');
                                var vm = that.obj.viewmodel;
                                that.toggleView(3);
                                module(that.obj.view[0], {
                                    margin: 3,
                                    filename: 'swagger.pdf',
                                    html2canvas: { scale: 1 }
                                }).then(function () {
                                    that.obj.view.height(ch);
                                    that.toggleView(vm);
                                })
                            })
                            break;
                        case "png":
                            {
                                var backvm = false;
                                if (that.obj.viewmodel == 1) {
                                    that.toggleView(2);
                                    backvm = true;
                                }

                                require(['https://code.bdstatic.com/npm/html2canvas@1.0.0-rc.7/dist/html2canvas.min.js'], function (module) {
                                    var ch = that.obj.view.height();
                                    that.obj.view.height('auto');
                                    module(that.obj.view[0], {
                                        scale: 1,
                                        margin: 15
                                    }).then(function (canvas) {
                                        that.obj.view.height(ch);
                                        netnrmd.down(canvas, "swagger.png");

                                        if (backvm) {
                                            that.toggleView(1);
                                        }
                                    })
                                })
                            }
                            break;
                    }
                } else {
                    bs.alert("<h4>请先点击 转文档 再下载</h4>")
                }
            });

            //demo
            $('.nr-demo').click(function () {
                location.hash = "https://httpbin.org/spec.json";
            });

            //拖拽打开
            ss.receiveFiles(function (files) {
                var file = files[0];
                var reader = new FileReader();
                reader.onload = function (e) {
                    editor1.setValue(e.target.result);
                    $('.nrBtnSwaggerUI')[0].click();
                };
                reader.readAsText(file);
            });

            st.openUrl();
        });
    },

    config: {
        //仅保留 Json 格式
        onlyJson: true
    },

    /**
     * 从链接打开
     * @param {any} url
     */
    openUrl: function (url) {
        if (url == null) {
            url = location.hash.substr(1);
        }
        if (url.length < 4) {
            $('.nrBtnSwaggerUI')[0].click();
            return;
        }
        editor1.setValue("Loading ...");

        fetch(url).then(x => x.text()).then(res => {
            editor1.setValue(res);
            $('.nrBtnSwaggerUI')[0].click();

            location.hash = "";
        }).catch(e => {
            console.log(e);
            editor1.setValue(e + "");
        })
    },

    /**
     * Swagger JSON => Markdown
     * @param {any} swaggerJson
     */
    jsonToMarkdown: function (swaggerJson) {

        var mds = [];

        var title = st.jk("info:title", swaggerJson);
        var version = st.jk("info:version", swaggerJson);
        if (title != null) {
            mds.push(st.formatter.title(title, version));
        }

        var description = st.jk("info:description", swaggerJson);
        if (description != null) {
            mds.push(st.formatter.description(description));
        }

        var mdg = [];

        var paths = swaggerJson["paths"];
        if (paths) {
            for (var d1 in paths) {
                var path = paths[d1];
                for (var d2 in path) {
                    var i2 = path[d2];
                    var tags = st.jk("tags", i2);
                    var mdi = [];

                    mdi.push(st.formatter.path(d2, d1));
                    if ("summary" in i2) {
                        mdi.push(st.formatter.summary(i2.summary));
                    }

                    if ("parameters" in i2) {
                        mdi.push(st.formatter.parameters(i2.parameters, swaggerJson));
                    }

                    if ("requestBody" in i2) {
                        var requestBody = i2.requestBody;
                        mdi.push(st.formatter.requestBody(requestBody, swaggerJson));
                    }

                    if ("responses" in i2) {
                        mdi.push(st.formatter.responses(i2.responses, swaggerJson));
                    }

                    mdg.push({ tag: tags[0], mdi });
                }
            }
        }

        var tags = st.jk("tags", swaggerJson);
        tags.forEach(tag => {
            mds.push(st.formatter.tags_item(tag));

            mdg.filter(x => x.tag == tag.name).forEach(x => mds.push(x.mdi.join("\n")));
        });

        mdg.filter(x => x.tag == null).forEach(x => mds.push(x.mdi.join("\n")));

        return mds.join("\n");
    },

    /**
     * 设置编辑器语言
     * @param {any} editor
     * @param {any} lang
     */
    setEditorLanguage: function (editor, lang) {
        var oldModel = editor.getModel();
        var newModel = monaco.editor.createModel(editor.getValue(), lang);
        editor.setModel(newModel);
        if (oldModel) {
            oldModel.dispose();
        }
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
        var po = st.parseJsonOrYaml(txt);
        return APISpecConverter.convert({
            from: 'swagger_' + (po.data.swagger.indexOf("1.") == 0 ? "1" : "2"),
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
            var pout = st.parseJsonOrYaml(txt);
            if (pout.lang) {
                var version = st.jk("openapi", pout.data) || st.jk("swagger", pout.data);
                if (version) {
                    if (version.split(".")[0] >= 3) {
                        resolve(pout);
                    } else {
                        st.swaggerConvert(txt, pout.lang).then(res => {
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
            if (version != null) {
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
            return st.mdEncode(description);
        },

        /**
         * 分组标签
         * @param {any} tag 分组标签一项
         */
        tags_item: function (tag) {
            var name = st.jk("name", tag);
            var description = st.mdEncode(st.jk("description", tag) || "");

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
            mds.push(st.mdEncode(summary));
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

            st.mdTableHeader(mds, "名称,类型,位置,说明".split(','));
            parameters.forEach(p => {
                var type = [st.jk("type", p), st.jk("schema:type", p), st.jk("schema:format", p), st.jk("schema:items:type", p), st.jk("schema:items:enum", p) ? "enum" : null];
                type = type.filter(x => x != null);
                if (type.length) {
                    type = type.join(' / ');
                } else {
                    var refName = st.jk("schema:$ref", p);
                    if (refName) {
                        type = st.refForType(refName, swaggerJson);
                    } else {
                        type = "";
                    }
                }

                var description = st.mdEncode(p.description || ""),
                    minimum = st.jk("schema:minimum", p),
                    maximum = st.jk("schema:maximum", p), mm = [];

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
            var rb_description = st.jk("description", requestBody);

            var mds = [];
            mds.push("");
            mds.push("#### 请求主体（RequestBody）");
            if (rb_description != null) {
                mds.push(st.mdEncode(rb_description));
            }

            if (st.config.onlyJson) {
                rb_content = st.onlyJsonForContent(rb_content);
            }

            for (var i in rb_content) {
                mds.push("");
                mds.push("**" + i.replace(/\*/g, "\\*") + "**");
                mds.push("");

                var cformat = i.split('/')[1];
                if (["json", "xml", "html"].indexOf(cformat) == -1) {
                    cformat = "";
                }

                var properties = st.jk("schema:properties", rb_content[i]);

                if (properties != null) {

                    st.mdTableHeader(mds, "名称,类型,说明".split(','));

                    for (var j in properties) {
                        var pj = properties[j];

                        var type = st.jk("format", pj) || st.jk("type", pj);
                        if (type == null) {
                            var refName = st.jk("$ref", pj);
                            if (refName) {
                                type = st.refForType(refName, swaggerJson);
                            }
                        }

                        var description = st.jk("description", pj) || "";
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
                    var rbcRefName1 = st.jk("schema:$ref", rb_content[i]);
                    var rbcRefName2 = st.jk("schema:items:$ref", rb_content[i]);
                    if (rbcRefName1 != null) {
                        var rout = st.buildRefTree(rbcRefName1, swaggerJson);
                        var ov = rout.obj;

                        if (cformat == "xml") {
                            ov = st.jsonToXml(ov, rbcRefName1.split("/").pop())
                        } else {
                            ov = JSON.stringify(ov, null, 2);
                        }

                        mds.push("```" + cformat);
                        mds.push(ov);
                        mds.push("```");
                        mds.push("");

                        st.mdTableHeader(mds, "名称,类型,说明".split(','));
                        mds = mds.concat(st.fieldsAsView(rout.fields));
                    } else if (rbcRefName2 != null) {
                        var rout = st.buildRefTree(rbcRefName2, swaggerJson);
                        var ov = rout.obj;

                        var rbcSchemaType = st.jk("schema:type", rb_content[i]);
                        if (rbcSchemaType == "array") {
                            ov = [ov];
                        }

                        if (cformat == "xml") {
                            ov = st.jsonToXml(ov, rbcRefName2.split("/").pop())
                        } else {
                            ov = JSON.stringify(ov, null, 2);
                        }

                        mds.push("```" + cformat);
                        mds.push(ov);
                        mds.push("```");
                        mds.push("");

                        st.mdTableHeader(mds, "名称,类型,说明".split(','));
                        mds = mds.concat(st.fieldsAsView(rout.fields));
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

                    if (st.config.onlyJson) {
                        content = st.onlyJsonForContent(content);
                    }

                    for (var j in content) {
                        mds.push("");
                        mds.push("**" + j.replace(/\*/g, "\\*") + "**");
                        mds.push("");

                        var cformat = j.split('/')[1];
                        if (["json", "xml", "html"].indexOf(cformat) == -1) {
                            cformat = "";
                        }

                        var cSchemaRefName = st.jk("schema:$ref", content[j]);

                        var cSchemaType = st.jk("schema:type", content[j]);

                        var cSchemaItemsType = st.jk("schema:items:type", content[j]);

                        var cSchemaItemsRefName = st.jk("schema:items:$ref", content[j]);
                        if (cSchemaRefName) {
                            var rout = st.buildRefTree(cSchemaRefName, swaggerJson);
                            var ov = rout.obj;

                            if (cformat == "xml") {
                                ov = st.jsonToXml(ov, cSchemaRefName.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                            mds.push("");

                            st.mdTableHeader(mds, "名称,类型,说明".split(','));
                            mds = mds.concat(st.fieldsAsView(rout.fields));
                        } else if (cSchemaItemsType) {
                            var ov = st.buildTypeData(cSchemaItemsType);
                            if (cSchemaType == "array") {
                                ov = [ov];
                            }

                            if (cformat == "xml") {
                                ov = st.jsonToXml(ov, cSchemaItemsType.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                        } else if (cSchemaItemsRefName) {
                            var rout = st.buildRefTree(cSchemaItemsRefName, swaggerJson);
                            var ov = rout.obj;

                            if (cSchemaType == "array") {
                                ov = [ov];
                            }

                            if (cformat == "xml") {
                                ov = st.jsonToXml(ov, cSchemaItemsRefName.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                            mds.push("");

                            st.mdTableHeader(mds, "名称,类型,说明".split(','));
                            mds = mds.concat(st.fieldsAsView(rout.fields));
                        } else {
                            var cstype = st.jk("schema:type", content[j]);
                            var csAdditionalProperties = st.jk("schema:additionalProperties", content[j]);
                            if (cstype && csAdditionalProperties) {

                                var cSchema = st.jk("schema", content[j]);
                                var rout = st.buildRefTree(cSchema, swaggerJson);
                                var ov = rout.obj;
                                if (cformat == "xml") {
                                    ov = st.jsonToXml(ov)
                                } else {
                                    ov = JSON.stringify(ov, null, 2);
                                }

                                mds.push("```" + cformat);
                                mds.push(ov);
                                mds.push("```");
                                mds.push("");

                                st.mdTableHeader(mds, "名称,类型,说明".split(','));
                                mds = mds.concat(st.fieldsAsView(rout.fields));
                            }
                        }
                    }
                }

                if ("headers" in ri) {
                    var headers = ri.headers;

                    mds.push("");
                    mds.push("**头部（Headers）**");
                    mds.push("");

                    st.mdTableHeader(mds, "名称,类型,说明".split(','));

                    for (var j in headers) {
                        var type = [st.jk("schema:type", headers[j]), st.jk("schema:format", headers[j])].join(' / ');
                        var description = st.mdEncode(st.jk("description", headers[j]));
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
        var refCobj = st.jk(st.refAsKey(refName), swaggerJson);
        var type = st.jk("type", refCobj);
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

        return st.formatXml(xml);
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
            typeObj = st.jk(st.refAsKey(refName), swaggerJson);
        }

        var properties = st.jk("properties", typeObj);
        var aProperties = st.jk("additionalProperties", typeObj);
        if (properties == null && aProperties) {
            properties = {};
            for (var f = 1; f < 4; f++) {
                properties["additionalProp" + f] = aProperties;
            }
        }

        var obj = {}, fields = [];

        for (var k in properties) {

            var ptype = st.jk("type", properties[k]);
            var pformat = st.jk("format", properties[k]);
            var pnullable = st.jk("nullable", properties[k]);
            var pdescription = st.jk("description", properties[k]);
            var pdefault = st.jk("default", properties[k]);
            var penum = st.jk("enum", properties[k]);

            var pexample = st.jk("example", properties[k]);

            var objRefName = st.jk("$ref", properties[k]), ov;
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
                        var arrRefName = st.jk("items:$ref", properties[k]);
                        var arrItemsType = st.jk("items:type", properties[k]);

                        if (arrRefName) {
                            if (arrRefName == refName) {
                                ov.push("circular references 循环引用");
                            } else {
                                var rout = arguments.callee(arrRefName, swaggerJson)
                                fieldObj.children = rout.fields;
                                ov.push(rout.obj);
                            }
                        } else if (arrItemsType) {
                            ov.push([st.buildTypeData(arrItemsType)])
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
            typeObj = st.jk(st.refAsKey(refName), swaggerJson);
        }

        var properties = st.jk("properties", typeObj);
        var aProperties = st.jk("additionalProperties", typeObj);
        if (properties == null && aProperties) {
            properties = {};
            for (var f = 1; f < 4; f++) {
                properties["additionalProp" + f] = aProperties;
            }
        }

        var obj = {};
        for (var k in properties) {
            var ptype = st.jk("type", properties[k]);

            var objRefName = st.jk("$ref", properties[k]);
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
                        var ve = st.jk("enum", properties[k]);
                        if (ve) {
                            ov = ve[0];
                        }

                        var format = st.jk("format", properties[k]);
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
                        var vd = st.jk("default", properties[k]);
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
                        var arrRefName = st.jk("items:$ref", properties[k]);
                        var arrItemsType = st.jk("items:type", properties[k]);
                        if (arrRefName) {
                            ov.push(st.buildRefData(arrRefName, swaggerJson));
                        } else if (arrItemsType) {
                            ov.push([arguments.callee(arrItemsType)])
                        }
                    }
                    break;
                default: ov = {};
            }
            var example = st.jk("example", properties[k]);
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

st.init();