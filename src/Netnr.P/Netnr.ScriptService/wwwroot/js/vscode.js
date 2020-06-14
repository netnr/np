var editor,
    defaultLang = ss.lsStr("vscode-lang") || 'javascript',
    defaultContent = ss.lsStr("vscode-content") || 'console.log("Hello world!");',
    defaultTheme = ss.lsStr("vscode-theme") || 'vs';

require(['vs/editor/editor.main'], function () {
    var modesIds = monaco.languages.getLanguages().map(function (lang) { return lang.id }).sort();

    var te = $("#editor"), selang = $('#selanguage'),
        languagehtm = [];
    for (var i = 0; i < modesIds.length; i++) {
        var mo = modesIds[i];
        languagehtm.push('<option>' + mo + '</option>');
    }
    selang.children()[0].innerHTML = languagehtm.join('');

    editor = monaco.editor.create(te[0], {
        value: defaultContent,
        language: defaultLang,
        automaticLayout: true,
        theme: defaultTheme,
        scrollbar: {
            verticalScrollbarSize: 6,
            horizontalScrollbarSize: 6
        },
        minimap: {
            _enabled: false
        }
    });

    selang.change(function () {
        var oldModel = editor.getModel();
        var newModel = monaco.editor.createModel(editor.getValue(), this.value);
        editor.setModel(newModel);
        if (oldModel) {
            oldModel.dispose();
        }
        ss.ls["vscode-lang"] = this.value;
        ss.lsSave();

        if (this.value == "javascript") {
            $('#btnRun').removeClass('d-none');
        } else {
            $('#btnRun').addClass('d-none');
        }
    }).val(defaultLang);
    if (defaultLang == "javascript") {
        $('#btnRun').removeClass('d-none');
    } else {
        $('#btnRun').addClass('d-none');
    }

    $('#setheme').change(function () {
        monaco.editor.setTheme(this.value);
        ss.ls["vscode-theme"] = this.value;
        ss.lsSave();
    }).val(defaultTheme);

    editor.onDidChangeModelContent(function (e) {
        clearTimeout(window.defer1);
        window.defer1 = setTimeout(function () {
            ss.ls["vscode-content"] = editor.getValue();
            ss.lsSave();
        }, 1000 * 1)
    });
});

$('#btnRun').click(function () {
    try {
        window.ee = new Function(editor.getValue());
        ee();
    } catch (e) {
        console.error(e);
    }
});

$(window).resize(AutoHeight);