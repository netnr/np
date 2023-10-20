let swg = {
    config: {
        //仅保留 Json 格式
        onlyJson: true
    },

    /**
     * openAPI JSON => Markdown
     * @param {any} openAPI json
     */
    toMarkdown: function (openAPI) {

        var mds = [];

        var title = swg.jk("info:title", openAPI);
        var version = swg.jk("info:version", openAPI);
        if (title != null) {
            mds.push(swg.formatter.title(title, version));
        }

        var description = swg.jk("info:description", openAPI);
        if (description != null) {
            mds.push(swg.formatter.description(description));
        }

        var mdg = [];
        var pathsTags = [];
        var pathsTagKey = {};

        var paths = openAPI["paths"];
        if (paths) {
            for (var d1 in paths) {
                var path = paths[d1];
                for (var d2 in path) {
                    var i2 = path[d2];
                    var tags = swg.jk("tags", i2);
                    var mdi = [];

                    mdi.push(swg.formatter.path(d2, d1));
                    if ("summary" in i2) {
                        mdi.push(swg.formatter.summary(i2.summary));
                    }

                    if ("parameters" in i2) {
                        mdi.push(swg.formatter.parameters(i2.parameters, openAPI));
                    }

                    if ("requestBody" in i2) {
                        var requestBody = i2.requestBody;
                        mdi.push(swg.formatter.requestBody(requestBody, openAPI));
                    }

                    if ("responses" in i2) {
                        mdi.push(swg.formatter.responses(i2.responses, openAPI));
                    }

                    if (!(tags[0] in pathsTagKey)) {
                        pathsTagKey[tags[0]] = 1;
                        pathsTags.push({ name: tags[0], description: tags[0] })
                    }

                    mdg.push({ tag: tags[0], mdi });
                }
            }
        }

        var tags = swg.jk("tags", openAPI);
        if (tags == null) {
            tags = pathsTags
        }
        tags.forEach(tag => {
            mds.push(swg.formatter.tags_item(tag));

            mdg.filter(x => x.tag == tag.name).forEach(x => mds.push(x.mdi.join("\n")));
        });

        mdg.filter(x => x.tag == null).forEach(x => mds.push(x.mdi.join("\n")));

        return mds.join("\n");
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
            return swg.mdEncode(description);
        },

        /**
         * 分组标签
         * @param {any} tag 分组标签一项
         */
        tags_item: function (tag) {
            var name = swg.jk("name", tag);
            var description = swg.mdEncode(swg.jk("description", tag) || "");

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
            mds.push(`### ${method.toUpperCase()} ${path}`);
            return mds.join('\n');
        },

        /**
         * 描述
         * @param {any} summary 描述
         */
        summary: function (summary) {
            var mds = [];
            mds.push("");
            //mds.push("#### 描述（Description）");
            mds.push(swg.mdEncode(summary));
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

            swg.mdTableHeader(mds, "名称,类型,位置,说明".split(','));
            parameters.forEach(p => {
                var type = [swg.jk("type", p), swg.jk("schema:type", p), swg.jk("schema:format", p), swg.jk("schema:items:type", p), swg.jk("schema:items:enum", p) ? "enum" : null];
                type = type.filter(x => x != null);
                var refModel;
                if (type.length) {
                    type = type.join(' / ');
                } else {
                    var refName = swg.jk("schema:$ref", p);
                    if (refName) {
                        type = swg.refForType(refName, swaggerJson);

                        //引用对象
                        refModel = swg.buildRefTree(refName, swaggerJson);
                    } else {
                        type = "";
                    }
                }

                var description = swg.mdEncode(p.description || ""),
                    minimum = swg.jk("schema:minimum", p),
                    maximum = swg.jk("schema:maximum", p), mm = [];

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
                if (refModel) {
                    mds = mds.concat(swg.fieldsAsView(refModel.fields, p.in));
                }
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
            var rb_description = swg.jk("description", requestBody);

            var mds = [];
            mds.push("");
            mds.push("#### 请求主体（Request Body）");
            if (rb_description != null) {
                mds.push(swg.mdEncode(rb_description));
            }

            if (swg.config.onlyJson) {
                rb_content = swg.onlyJsonForContent(rb_content);
            }

            for (var i in rb_content) {
                mds.push("");
                mds.push("Content-Type: **" + i.replace(/\*/g, "\\*") + "**");
                mds.push("");
                mds.push("内容（Content）");
                mds.push("");

                var cformat = i.split('/')[1];
                if (["json", "xml", "html"].indexOf(cformat) == -1) {
                    cformat = "";
                }

                var properties = swg.jk("schema:properties", rb_content[i]);

                if (properties != null) {
                    mds.push("");
                    mds.push("说明（Description）");
                    mds.push("");

                    swg.mdTableHeader(mds, "名称,类型,说明".split(','));

                    for (var j in properties) {
                        var pj = properties[j];

                        var type = swg.jk("format", pj) || swg.jk("type", pj);
                        if (type == null) {
                            var refName = swg.jk("$ref", pj);
                            if (refName) {
                                type = swg.refForType(refName, swaggerJson);
                            }
                        }

                        var description = swg.jk("description", pj) || "";
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
                    var rbcRefName1 = swg.jk("schema:$ref", rb_content[i]);
                    var rbcRefName2 = swg.jk("schema:items:$ref", rb_content[i]);
                    if (rbcRefName1 != null) {
                        var rout = swg.buildRefTree(rbcRefName1, swaggerJson);
                        var ov = rout.obj;

                        if (cformat == "xml") {
                            ov = swg.jsonToXml(ov, rbcRefName1.split("/").pop())
                        } else {
                            ov = JSON.stringify(ov, null, 2);
                            cformat = "json";
                        }

                        mds.push("```" + cformat);
                        mds.push(ov);
                        mds.push("```");
                        mds.push("");
                        mds.push("说明（Description）");
                        mds.push("");

                        swg.mdTableHeader(mds, "名称,类型,说明".split(','));
                        mds = mds.concat(swg.fieldsAsView(rout.fields));
                    } else if (rbcRefName2 != null) {
                        var rout = swg.buildRefTree(rbcRefName2, swaggerJson);
                        var ov = rout.obj;

                        var rbcSchemaType = swg.jk("schema:type", rb_content[i]);
                        if (rbcSchemaType == "array") {
                            ov = [ov];
                        }

                        if (cformat == "xml") {
                            ov = swg.jsonToXml(ov, rbcRefName2.split("/").pop())
                        } else {
                            ov = JSON.stringify(ov, null, 2);
                            cformat = "json";
                        }

                        mds.push("```" + cformat);
                        mds.push(ov);
                        mds.push("```");
                        mds.push("");
                        mds.push("说明（Description）");
                        mds.push("");

                        swg.mdTableHeader(mds, "名称,类型,说明".split(','));
                        mds = mds.concat(swg.fieldsAsView(rout.fields));
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
                mds.push(`StatusCode: **${i}** ${ri.description}`);

                if ("content" in ri) {
                    var content = ri.content;

                    if (swg.config.onlyJson) {
                        content = swg.onlyJsonForContent(content);
                    }

                    for (var j in content) {
                        mds.push("");
                        mds.push("Content-Type: **" + j.replace(/\*/g, "\\*") + "**");
                        mds.push("");
                        mds.push("内容（Content）");
                        mds.push("");

                        var cformat = j.split('/')[1];
                        if (["json", "xml", "html"].indexOf(cformat) == -1) {
                            cformat = "";
                        }

                        var cSchemaRefName = swg.jk("schema:$ref", content[j]);

                        var cSchemaType = swg.jk("schema:type", content[j]);

                        var cSchemaItemsType = swg.jk("schema:items:type", content[j]);

                        var cSchemaItemsRefName = swg.jk("schema:items:$ref", content[j]);
                        if (cSchemaRefName) {
                            var rout = swg.buildRefTree(cSchemaRefName, swaggerJson);
                            var ov = rout.obj;

                            if (cformat == "xml") {
                                ov = swg.jsonToXml(ov, cSchemaRefName.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                                cformat = "json";
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                            mds.push("");
                            mds.push("说明（Description）");
                            mds.push("");

                            swg.mdTableHeader(mds, "名称,类型,说明".split(','));
                            mds = mds.concat(swg.fieldsAsView(rout.fields));
                        } else if (cSchemaItemsType) {
                            var ov = swg.buildTypeData(cSchemaItemsType);
                            if (cSchemaType == "array") {
                                ov = [ov];
                            }

                            if (cformat == "xml") {
                                ov = swg.jsonToXml(ov, cSchemaItemsType.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                                cformat = "json";
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                        } else if (cSchemaItemsRefName) {
                            var rout = swg.buildRefTree(cSchemaItemsRefName, swaggerJson);
                            var ov = rout.obj;

                            if (cSchemaType == "array") {
                                ov = [ov];
                            }

                            if (cformat == "xml") {
                                ov = swg.jsonToXml(ov, cSchemaItemsRefName.split("/").pop())
                            } else {
                                ov = JSON.stringify(ov, null, 2);
                                cformat = "json";
                            }

                            mds.push("```" + cformat);
                            mds.push(ov);
                            mds.push("```");
                            mds.push("");
                            mds.push("说明（Description）");
                            mds.push("");

                            swg.mdTableHeader(mds, "名称,类型,说明".split(','));
                            mds = mds.concat(swg.fieldsAsView(rout.fields));
                        } else {
                            var cstype = swg.jk("schema:type", content[j]);
                            var csAdditionalProperties = swg.jk("schema:additionalProperties", content[j]);
                            if (cstype && csAdditionalProperties) {

                                var cSchema = swg.jk("schema", content[j]);
                                var rout = swg.buildRefTree(cSchema, swaggerJson);
                                var ov = rout.obj;
                                if (cformat == "xml") {
                                    ov = swg.jsonToXml(ov)
                                } else {
                                    ov = JSON.stringify(ov, null, 2);
                                    cformat = "json";
                                }

                                mds.push("```" + cformat);
                                mds.push(ov);
                                mds.push("```");
                                mds.push("");
                                mds.push("说明（Description）");
                                mds.push("");

                                swg.mdTableHeader(mds, "名称,类型,说明".split(','));
                                mds = mds.concat(swg.fieldsAsView(rout.fields));
                            }
                        }
                    }
                }

                if ("headers" in ri) {
                    var headers = ri.headers;

                    mds.push("");
                    mds.push("**头部（Headers）**");
                    mds.push("");

                    swg.mdTableHeader(mds, "名称,类型,说明".split(','));

                    for (var j in headers) {
                        var type = [swg.jk("schema:type", headers[j]), swg.jk("schema:format", headers[j])].join(' / ');
                        var description = swg.mdEncode(swg.jk("description", headers[j]));
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
        var refCobj = swg.jk(swg.refAsKey(refName), swaggerJson);
        var type = swg.jk("type", refCobj);
        if ("enum" in refCobj) {
            type = "enum(" + refName.split("/").pop() + ")";
        }
        return type;
    },

    /**
     * JSON => XML
     * @param {any} json
     */
    jsonToXml: (json) => {
        console.warn('Please override this method')
        return json;
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
        xml.split('\r\n').forEach((node) => {
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
     * @param {any} parameterIn 可选，参数位置
     * @param {any} deep
     */
    fieldsAsView: function (fields, parameterIn, deep) {
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

            if (parameterIn == null) {
                list.push(`|${empty.join('')} ${item.field} | ${type} | ${description} |`);
            } else {
                list.push(`|${empty.join('')} ${item.field} | ${type} | ${parameterIn} | ${description} |`);
            }
            if (item.children) {
                list = list.concat(arguments.callee(item.children, parameterIn, deep + 1))
            }
        });
        return list;
    },

    /**
     * 根据 $ref 引用构建树
     * @param {any} refName 引用名 或 additionalSchema
     * @param {any} swaggerJson 源
     */
    buildRefTree: function (refName, swaggerJson) {

        var typeObj = refName;
        if (typeof refName == "string") {
            typeObj = swg.jk(swg.refAsKey(refName), swaggerJson);
        }

        var properties = swg.jk("properties", typeObj);
        var aProperties = swg.jk("additionalProperties", typeObj);
        if (properties == null && aProperties) {
            properties = {};
            for (var f = 1; f < 4; f++) {
                properties["additionalProp" + f] = aProperties;
            }
        }

        var obj = {}, fields = [];

        for (var k in properties) {

            var ptype = swg.jk("type", properties[k]);
            var pformat = swg.jk("format", properties[k]);
            var pnullable = swg.jk("nullable", properties[k]);
            var pdescription = swg.jk("description", properties[k]);
            var pdefault = swg.jk("default", properties[k]);
            var penum = swg.jk("enum", properties[k]);

            var pexample = swg.jk("example", properties[k]);

            var objRefName = swg.jk("$ref", properties[k]), ov;
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
                        var arrRefName = swg.jk("items:$ref", properties[k]);
                        var arrItemsType = swg.jk("items:type", properties[k]);

                        if (arrRefName) {
                            if (arrRefName == refName) {
                                ov.push("circular references 循环引用");
                            } else {
                                var rout = arguments.callee(arrRefName, swaggerJson)
                                fieldObj.children = rout.fields;
                                ov.push(rout.obj);
                            }
                        } else if (arrItemsType) {
                            ov.push([swg.buildTypeData(arrItemsType)])
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
            typeObj = swg.jk(swg.refAsKey(refName), swaggerJson);
        }

        var properties = swg.jk("properties", typeObj);
        var aProperties = swg.jk("additionalProperties", typeObj);
        if (properties == null && aProperties) {
            properties = {};
            for (var f = 1; f < 4; f++) {
                properties["additionalProp" + f] = aProperties;
            }
        }

        var obj = {};
        for (var k in properties) {
            var ptype = swg.jk("type", properties[k]);

            var objRefName = swg.jk("$ref", properties[k]);
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
                        var ve = swg.jk("enum", properties[k]);
                        if (ve) {
                            ov = ve[0];
                        }

                        var format = swg.jk("format", properties[k]);
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
                        var vd = swg.jk("default", properties[k]);
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
                        var arrRefName = swg.jk("items:$ref", properties[k]);
                        var arrItemsType = swg.jk("items:type", properties[k]);
                        if (arrRefName) {
                            ov.push(swg.buildRefData(arrRefName, swaggerJson));
                        } else if (arrItemsType) {
                            ov.push([arguments.callee(arrItemsType)])
                        }
                    }
                    break;
                default: ov = {};
            }
            var example = swg.jk("example", properties[k]);
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
}