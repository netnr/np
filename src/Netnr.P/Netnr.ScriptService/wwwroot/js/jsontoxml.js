var editor1, editor2;

require(['vs/editor/editor.main'], function () {

    var txt1 = document.getElementById('txt1');
    var tv1 = txt1.innerHTML;
    txt1.innerHTML = '';

    editor1 = monaco.editor.create(txt1, {
        value: tv1,
        language: 'json',
        scrollBeyondLastLine: false,
        automaticLayout: true,
        theme: 'vs',
        scrollbar: {
            verticalScrollbarSize: 6,
            horizontalScrollbarSize: 6
        },
        minimap: {
            enabled: false
        }
    });

    editor1.onDidChangeModelContent(function (e) {
        if ($('#seautoformatter1').val() == "1") {
            window.clearTimeout(window.defer1)
            window.defer1 = setTimeout(function () {
                try {
                    if (window.very1 == 'self') {
                        window.very1 = '';
                    } else {
                        var val = JSON.stringify(JSON.parse(editor1.getValue()), null, 2);
                        window.very1 = 'self';
                        editor1.setValue(val);
                    }
                } catch (e) { }
            }, 20)
        }
    });

    editor2 = monaco.editor.create($("#txt2")[0], {
        value: '',
        language: 'xml',
        automaticLayout: true,
        scrollBeyondLastLine: false,
        theme: 'vs',
        scrollbar: {
            verticalScrollbarSize: 6,
            horizontalScrollbarSize: 6
        },
        minimap: {
            enabled: false
        }
    });

    editor2.onDidChangeModelContent(function (e) {
        if ($('#seautoformatter2').val() == "1") {
            window.clearTimeout(window.defer2)
            window.defer2 = setTimeout(function () {
                try {
                    if (window.very2 == 'self') {
                        window.very2 = '';
                    } else {
                        var val = formatXml(editor2.getValue());
                        window.very2 = 'self';
                        editor2.setValue(val);
                    }
                } catch (e) { }
            }, 20);
        }
    });
});

$(window).resize(AutoHeight);

$("#btnToJson").click(function (e) {
    var x2js = new X2JS({
        attributePrefix: "@"
    });
    var xmlText = editor2.getValue();
    if (xmlText == "") {
        jz.msg('XML 不能为空');
        return false;
    }
    var jsonObj = x2js.xml_str2json(xmlText);
    if (jsonObj == null && $(xmlText).length) {
        xmlText = '<root>' + xmlText + '</root>';
        jsonObj = x2js.xml_str2json(xmlText);
    }
    if (jsonObj == null) {
        jz.msg('转换错误');
        return false;
    }

    editor1.setValue(JSON.stringify(jsonObj));
});

$("#btnToXml").click(function () {
    var x2js = new X2JS({
        attributePrefix: "@"
    });
    var jsonText = editor1.getValue();
    if (jsonText == "") {
        jz.msg('JSON 不能为空');
        return false;
    }
    var jsonObj = $.parseJSON(jsonText);
    var xmlAsStr = x2js.json2xml_str(jsonObj);

    editor2.setValue(xmlAsStr);
});

function formatXml(xml) {
    var formatted = '';
    var reg = /(>)(<)(\/*)/g;
    xml = xml.replace(reg, '$1\r\n$2$3');
    var pad = 0;
    jQuery.each(xml.split('\r\n'), function (index, node) {
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
}

//提示
$('.fa-info-circle').tooltip();