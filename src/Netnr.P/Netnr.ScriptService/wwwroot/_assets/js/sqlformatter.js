function AutoHeight() {
    var ch = $(window).height() - $('#editor').offset().top - 15;
    $('#editor').css('height', Math.max(200, ch));
}
AutoHeight();


var editor, dv = ss.lsStr('txt') || 'SELECT * FROM table1 表名',
    scLanguage = ss.lsStr('sql-config-language') || 'sql',
    scUpperCase = ss.lsStr('sql-config-uppercase') || "1";

$('.nrSqlConfigLanguage').change(function () {
    ss.ls["sql-config-language"] = this.value;
    ss.lsSave();
}).val(scLanguage);

$('.nrSqlConfigUpperCase').change(function () {
    ss.ls["sql-config-uppercase"] = this.value;
    ss.lsSave();
}).val(scUpperCase);

require(['vs/editor/editor.main'], function () {
    var te = $("#editor");

    editor = monaco.editor.create(te[0], ss.meConfig({
        value: dv,
        language: 'sql'
    }));

    editor.onDidChangeModelContent(function (e) {
        clearTimeout(window.defer1);
        window.defer1 = setTimeout(function () {
            ss.ls.txt = editor.getValue();
            ss.lsSave();
        }, 1000 * 1)
    });

    //格式化
    monaco.languages.registerDocumentFormattingEditProvider('sql', {
        provideDocumentFormattingEdits: function (model, _options, _token) {
            return [{
                text: formatterSQL(model.getValue()),
                range: model.getFullModelRange()
            }];
        }
    });
});

$(window).resize(AutoHeight);

function formatterSQL(text) {
    return sqlFormatter.format(text, {
        language: document.querySelector('.nrSqlConfigLanguage').value,
        uppercase: document.querySelector('.nrSqlConfigUpperCase').value == "1"
    });
}

$('#btnSqlFormatter').click(function () {
    editor.trigger('a', 'editor.action.formatDocument')
})

//接收文件
ss.receiveFiles(function (files) {
    var file = files[0];
    var reader = new FileReader();
    reader.onload = function (e) {
        editor.setValue(e.target.result);
    };
    reader.readAsText(file);
});