/**
 * 解析字符串为xml
 * @param {string} data 字符串
 */
jCute.parseXML = function (data) {
    var xmldom = null;
    if (typeof DOMParser != "undefined") {
        xmldom = new DOMParser();
        xmldom = xmldom.parseFromString(data, "text/xml");
        var errors = xmldom.getElementsByTagName("parsererror");
        if (errors.length) { throw new Error(xmldom.parseError.reason); }
    } else if (typeof ActiveXObject != "undefined") {
        var versions = ["MSXML2.DOMDocument.6.0", "MSXML2.DOMDocument.3.0", "MSXML2.DOMDocument"];
        for (var i = 0; i < versions.length; i++) {
            try {
                new ActiveXObject(versions[i]);
                arguments.callee.activeXString = versions[i]; break;
            } catch (e) { }
        }
        xmldom = new ActiveXObject(arguments.callee.activeXString);
        xmldom.loadXML(data);
        if (xmldom.parseError != 0) { throw new Error(xmldom.parseError.reason); }
    } else { throw new Error("XML parse error"); }
    return xmldom;
}

/**
 * XML转字符串
 * @param {object} xmlDoc XML对象
 */
jCute.XMLSerializer = function (xmlDoc) {
    if (typeof XMLSerializer != "undefined") {
        return (new XMLSerializer()).serializeToString(xmlDoc);
    } else if (typeof xmlDoc.xml != "undefined") {
        return xmlDoc.xml;
    } else {
        throw new Error("XML serialize error");
    }
}

