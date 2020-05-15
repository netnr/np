var editor, defaultContent = localStorage["sqlformatter-content"] || 'SELECT * FROM table1';

require(['vs/editor/editor.main'], function () {
    var te = $("#editor");

    editor = monaco.editor.create(te[0], {
        value: defaultContent,
        language: 'sql',
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

    editor.onDidChangeModelContent(function (e) {
        clearTimeout(window.defer1);
        window.defer1 = setTimeout(function () {
            localStorage["sqlformatter-content"] = editor.getValue();
        }, 1000 * 1)
    });
});

$(window).resize(AutoHeight);

$('#btnSqlFormatter').click(function () {
    let language = document.getElementById('selanguage');
    var sf = sqlFormatter.format(editor.getValue(), { language: language.value });
    editor.setValue(sf);
})