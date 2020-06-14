var editor, dv = ss.lsStr('txt') || 'SELECT * FROM table1';

require(['vs/editor/editor.main'], function () {
    var te = $("#editor");

    editor = monaco.editor.create(te[0], {
        value: dv,
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
            ss.ls.txt = editor.getValue();
            ss.lsSave();
        }, 1000 * 1)
    });
});

$(window).resize(AutoHeight);

$('#btnSqlFormatter').click(function () {
    var sf = sqlFormatter.format(editor.getValue(), { language: "sql" });
    editor.setValue(sf);
})
$('#btnPgFormatter').click(function () {
    var that = this;
    that.disabled = true;
    ss.ajax({
        url: "https://sqlformat.darold.net/",
        data: {
            colorize: 1,
            uc_keyword: 2,
            uc_function: 0,
            uc_type: 1,
            spaces: 2,
            wrap_after: 0,
            show_example: 0,
            load_from_file: 0,
            code_upload: "",
            content: editor.getValue(),
            original_content: ""
        },
        success: function (data) {
            editor.setValue($.trim($(data).find('#sql').text()));
        },
        error: function () {
            alert('请求服务失败');
        },
        complete: function () {
            that.disabled = false;
        }
    })
})