function AutoHeight() {
    var ch = $(window).height() - $('#txt1').offset().top - 15;
    $('#txt1').height(Math.max(200, ch));
    $('#txt2').height(Math.max(200, ch));
}
AutoHeight();


var editor1, editor2;

require(['vs/editor/editor.main'], function () {
    //xml formatter
    monaco.languages.html.registerHTMLLanguageService('xml', {}, { documentFormattingEdits: true })

    var txt1 = document.getElementById('txt1');
    var tv1 = txt1.innerHTML;
    txt1.innerHTML = '';

    editor1 = monaco.editor.create(txt1, ss.meConfig({
        value: tv1,
        language: 'json',
        scrollBeyondLastLine: false
    }));
    $(txt1).removeClass('pme');

    editor2 = monaco.editor.create($("#txt2")[0], ss.meConfig({
        value: '',
        language: 'xml',
        scrollBeyondLastLine: false
    }));
});

$(window).resize(AutoHeight);

$("#btnToJson").click(function (e) {
    var xmlText = editor2.getValue();
    if (xmlText == "") {
        bs.msg('<h4>XML 不能为空</h4>');
        return false;
    }

    try {
        var jsonObj = new fastXmlParser.parse(xmlText);
        editor1.setValue(JSON.stringify(jsonObj, null, 2));
    } catch (err) {
        bs.msg('<h4>转换错误</h4>');
        console.log(err);
    }
});

$("#btnToXml").click(function () {
    var jsonText = editor1.getValue();
    if (jsonText == "") {
        bs.msg('<h4>JSON 不能为空</h4>');
        return false;
    }

    var jsonObj = JSON.parse(jsonText);
    var xml = new fastXmlParser.j2xParser().parse(jsonObj);
    editor2.setValue(xml);
    editor2.trigger('a', 'editor.action.formatDocument')
});