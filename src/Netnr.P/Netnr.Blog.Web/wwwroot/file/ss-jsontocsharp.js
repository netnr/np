/**
 * by netnr
 * 2019-09-14 - 2022-08-12
 * */
let jtc = {
    //配置
    config: {
        notes: true,//带注释
        indent: 4,//间隙
        typeMapStringToDateTime: true,//类型映射，string时，是时间，则对应datetime
        bigHump: true,//大驼峰
        withPropertyName: false, //属性名
        classNameAppend: "Model"//类名末尾追加
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
        if (jtc.type(arrobj) == "Array") {
            for (; i < len;) {
                var stype = jtc.type(arrobj[i++]);
                if ("Object Array".indexOf(stype) >= 0) {
                    isOA = "Array";
                    break;
                }
            }
        } else {
            isOA = "Object";
        }

        if (isOA == "Array") {
            if (jtc.config.bigHump) {
                key = jtc.convertFirstUpper(key);
            }
            outType = "List<" + key + jtc.config.classNameAppend + ">";
        } else if (isOA == "Object") {
            if (jtc.config.bigHump) {
                key = jtc.convertFirstUpper(key);
            }
            outType = key + jtc.config.classNameAppend;
        } else {
            i = 0;
            for (; i < len;) {
                var ival = arrobj[i++], itype = jtc.type(ival);
                if (itype == "String") {
                    outType = "List<string>";
                } else if (itype == "Boolean") {
                    outType = "List<bool>";
                } else if (itype == "Null") {
                    outType = "object";
                } else if (itype == "Number") {
                    outType = "List<" + jtc.mapNumber(ival) + ">";
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
            arr.push("/// " + varname);
            arr.push("/// </summary>");
        }
        if (jtc.config.withPropertyName) {
            arr.push(`[JsonPropertyName("${varname}")]`);
        }
        if (jtc.config.bigHump) {
            varname = jtc.convertFirstUpper(varname);
        }
        arr.push(`public ${type} ${varname} { get; set; }`);
        return arr;
    },
    //生成一个类，类名，属性数组，缩进
    createObject: function (classname, pros, indent) {
        var arr = [], i = 0, ilen = pros.length;
        if (jtc.config.bigHump) {
            classname = jtc.convertFirstUpper(classname);
        }
        arr.push("public class " + classname);
        arr.push("{");
        if (!ilen) {
            arr.push(jtc.createSpace(indent) + "");
        }
        for (; i < ilen;) {
            var prs = pros[i++], jlen = prs.length, j = 0;
            for (; j < jlen;) {
                arr.push(jtc.createSpace(indent) + prs[j++]);
            }
        }
        arr.push("}");
        return arr;
    },
    //类型映射，键、值
    typeMap: function (key, value) {
        var vtype = jtc.type(value), outType = 'object';
        switch (vtype) {
            case "String":
                outType = "string";
                if (jtc.config.typeMapStringToDateTime && value.length > 8 && value.length < 30) {
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
                outType = jtc.mapNumber(value);
                break;
            case "Array":
            case "Object":
                outType = jtc.mapArrayOrObject(value, key);
                break;
        }
        return outType || "object";
    },
    //需要生成的对象
    arrEntity: [],
    //SON层级 转为类数组
    eachJson: function (json, cn) {
        var obj = {
            cn: cn + jtc.config.classNameAppend,
            obj: json
        };
        jtc.arrEntity.push(obj);
        for (var i in json) {
            var ji = json[i], vt = jtc.type(ji);
            if (vt == "Object") {
                jtc.eachJson(ji, i);
            }
            if (vt == "Array") {
                var si = ji[0], svt = jtc.type(si);
                if ("Object Array".indexOf(svt) >= 0) {
                    jtc.eachJson(si, i);
                }
            }
        }
    },
    //构建 JSON、类名
    builder: function (obj, cn) {
        var ae = [];
        for (var i in obj) {
            var tm = jtc.typeMap(i, obj[i]),
                cp = jtc.createProperty(tm, i, jtc.config.notes);
            ae.push(cp);
        }
        ae = jtc.createObject(cn, ae, jtc.config.indent);
        return ae;
    },
    //入口
    init: function (json) {
        jtc.arrEntity = [];
        jtc.eachJson(json, "root");
        var i = 0, len = jtc.arrEntity.length, outArr = [];
        for (; i < len;) {
            var ae = jtc.arrEntity[i++];
            var rb = jtc.builder(ae.obj, ae.cn);
            outArr.push(rb.join('\r\n') + "\r\n");
        }
        if (outArr.length) {
            return outArr.join('\r\n');
        } else {
            return "";
        }
    }
}