/**
 * by netnr
 * 2019-09-14
 * */
var rt = {
    //配置
    config: {
        notes: true,//带注释
        indent: 6,//间隙
        typeMapStringToDateTime: true,//类型映射，string时，是时间，则对应datetime
        classNameFirstBig: true,//类名首字母大写
        classNameAppend: "Entity"//类名末尾追加
    },
    //判断类型
    type: function (obj) {
        var tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },
    //生成空格（缩进）
    createSpace: function (n) {
        var i = 0, arr = [];
        for (; i < n; i++) {
            arr.push(" ");
        }
        return arr.join('');
    },
    //首字母大写
    convertFirstUpper: function (str) {
        return str[0].toUpperCase() + str.substring(1);
    },
    //数字映射
    mapNumber: function (num) {
        var outType = 'int';
        if (parseFloat(num) == parseInt(num)) {
            if (num > Math.pow(2, 31) - 1) {
                outType = "long";
            }
        } else {
            outType = "decimal";
        }
        return outType;
    },
    //数组、对象映射、OA
    mapArrayOrObject: function (arrobj, key, isOA) {
        var outType, i = 0, len = arrobj.length;
        if (rt.type(arrobj) == "Array") {
            for (; i < len;) {
                var stype = rt.type(arrobj[i++]);
                if ("Object Array".indexOf(stype) >= 0) {
                    isOA = "Array";
                    break;
                }
            }
        } else {
            isOA = "Object";
        }

        if (isOA == "Array") {
            if (rt.config.classNameFirstBig) {
                key = rt.convertFirstUpper(key);
            }
            outType = "List<" + key + rt.config.classNameAppend + ">";
        } else if (isOA == "Object") {
            if (rt.config.classNameFirstBig) {
                key = rt.convertFirstUpper(key);
            }
            outType = key + rt.config.classNameAppend;
        } else {
            i = 0;
            for (; i < len;) {
                var ival = arrobj[i++], itype = rt.type(ival);
                if (itype == "String") {
                    outType = "List<string>";
                } else if (itype == "Boolean") {
                    outType = "List<bool>";
                } else if (itype == "Null") {
                    outType = "object";
                } else if (itype == "Number") {
                    outType = "List<" + rt.mapNumber(ival) + ">";
                }
            }
        }
        return outType;
    },
    //生成一个属性，类型、变量名、是否带注释行
    createProperty: function (type, varname, notes) {
        var arr = [];
        if (notes != false) {
            arr.push("/// <summary>");
            arr.push("/// ");
            arr.push("/// </summary>");
        }
        arr.push("public " + type + " " + varname + " { get; set; }");
        return arr;
    },
    //生成一个类，类名，属性数组，缩进
    createObject: function (classname, pros, indent) {
        var arr = [], i = 0, ilen = pros.length;
        if (rt.config.classNameFirstBig) {
            classname = rt.convertFirstUpper(classname);
        }
        arr.push("public class " + classname);
        arr.push("{");
        if (!ilen) {
            arr.push(rt.createSpace(indent) + "");
        }
        for (; i < ilen;) {
            var prs = pros[i++], jlen = prs.length, j = 0;
            for (; j < jlen;) {
                arr.push(rt.createSpace(indent) + prs[j++]);
            }
        }
        arr.push("}");
        return arr;
    },
    //类型映射，键、值
    typeMap: function (key, value) {
        var vtype = rt.type(value), outType = 'object';
        switch (vtype) {
            case "String":
                outType = "string";
                if (rt.config.typeMapStringToDateTime && value.length > 8 && value.length < 30) {
                    var dt = new Date(value);
                    if (dt != "Invalid Date") {
                        outType = "DateTime";
                    }
                }
                break;
            case "Null":
                outType = "string";
                break;
            case "Boolean":
                outType = "bool";
                break;
            case "Number":
                outType = rt.mapNumber(value);
                break;
            case "Array":
            case "Object":
                outType = rt.mapArrayOrObject(value, key);
                break;
        }
        return outType || "object";
    },
    //需要生成的对象
    arrEntity: [],
    //SON层级 转为类数组
    eachJson: function (json, cn) {
        var obj = {
            cn: cn + rt.config.classNameAppend,
            obj: json
        };
        rt.arrEntity.push(obj);
        for (var i in json) {
            var ji = json[i], vt = rt.type(ji);
            if (vt == "Object") {
                rt.eachJson(ji, i);
            }
            if (vt == "Array") {
                var si = ji[0], svt = rt.type(si);
                if ("Object Array".indexOf(svt) >= 0) {
                    rt.eachJson(si, i);
                }
            }
        }
    },
    //构建 JSON、类名
    builder: function (obj, cn) {
        var ae = [];
        for (var i in obj) {
            var tm = rt.typeMap(i, obj[i]),
                cp = rt.createProperty(tm, i, rt.config.notes);
            ae.push(cp);
        }
        ae = rt.createObject(cn, ae, rt.config.indent);
        return ae;
    },
    //入口
    init: function (json) {
        rt.arrEntity = [];
        rt.eachJson(json, "root");
        var i = 0, len = rt.arrEntity.length, outArr = [];
        for (; i < len;) {
            var ae = rt.arrEntity[i++];
            var rb = rt.builder(ae.obj, ae.cn);
            outArr.push(rb.join('\r\n') + "\r\n");
        }
        if (outArr.length) {
            return outArr.join('\r\n');
        } else {
            return "";
        }
    }
}

nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor1.getBoundingClientRect().top - 30;
    nr.domEditor1.style.height = vh + "px";
    nr.domEditor2.style.height = vh + "px";
}

var editor1, editor2;

nr.onReady = function () {
    ss.loading(true);

    me.init().then(() => {

        editor1 = me.create(nr.domEditor1, {
            value: JSON.stringify({
                "site": {
                    "title": "NET牛人",
                    "domain": "https://www.netnr.com",
                    "mirror": "https://www.netnr.eu.org",
                    "createtime": "2014.01.01"
                },
                "about": {
                    "name": "netnr",
                    "sex": "男",
                    "injob": "2012.03.01",
                    "live": "中国重庆",
                    "mail": "netnr@netnr.com",
                    "git": [
                        {
                            "name": "github",
                            "url": "https://github.com/netnr"
                        },
                        {
                            "name": "gitee",
                            "url": "https://gitee.com/netnr"
                        }
                    ]
                },
                "updaet": "2022.04.21",
                "version": "v.1.0.0"
            }, null, 2),
            language: 'json',
            scrollBeyondLastLine: false
        });

        editor2 = me.create(nr.domEditor2, {
            value: "",
            language: 'csharp',
            scrollBeyondLastLine: false
        });

        nr.domEditor1.classList.add('border');
        nr.domEditor2.classList.add('border');
        nr.domCardBox.classList.remove('invisible');
        ss.loading(false);

        //生成
        nr.domBtnBuild.addEventListener('click', function () {
            var json = editor1.getValue().trim();
            if (json == "") {
                me.keepSetValue(editor2, "请输入 JSON");
                nr.alert("请输入 JSON");
            } else {
                try {
                    json = JSON.parse(json);

                    rt.config.notes = nr.domSeComment.value == 1;
                    rt.config.classNameFirstBig = nr.domSeCapital.value == 1;

                    var result = rt.init(json);
                    me.keepSetValue(editor2, result);
                } catch (ex) {
                    me.keepSetValue(editor2, ex);
                }
            }
        });

        //接收文件
        nr.receiveFiles(function (files) {
            var file = files[0];
            var reader = new FileReader();
            reader.onload = function (e) {
                me.keepSetValue(editor1, e.target.result);
            };
            reader.readAsText(file);
        });
    })
}