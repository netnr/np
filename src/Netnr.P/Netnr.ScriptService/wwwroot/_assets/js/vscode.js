var editor,
    defaultLang = ss.lsStr("vscode-lang") || 'javascript',
    defaultContent = ss.lsStr("vscode-content") || 'console.log("Hello world!");',
    defaultTheme = ss.lsStr("vscode-theme") || 'vs';

require(['vs/editor/editor.main'], function () {
    //xml formatter
    monaco.languages.html.registerHTMLLanguageService('xml', {}, { documentFormattingEdits: true });

    var modesIds = monaco.languages.getLanguages().map(lang => lang.id).sort();
    modesIds = modesIds.filter(x => !x.includes('.'));

    var te = $("#editor"), selang = $('#selanguage'), languagehtm = [];

    for (var i = 0; i < modesIds.length; i++) {
        var mo = modesIds[i];
        languagehtm.push('<option>' + mo + '</option>');
    }
    selang.children()[0].innerHTML = languagehtm.join('');

    editor = monaco.editor.create(te[0], ss.meConfig({
        value: defaultContent,
        language: defaultLang,
        theme: defaultTheme
    }));

    document.querySelector('.nr-vscode-box').classList.remove("d-none");

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

    editor.addCommand(monaco.KeyCode.PauseBreak, function () {
        $('#btnRun')[0].click();
    })

    $(window).on("load resize", function () {
        var ch = $(window).height() - $('#editor').offset().top - 20;
        $('#editor').css('height', Math.max(200, ch));
    });
});

$('#btnRun').click(function () {
    switch (editor.getModel().getLanguageId()) {
        case "javascript":
            try {
                window.ee = new Function(editor.getValue());
                ee();
            } catch (e) {
                console.error(e);
            }
            break;
    }
});

//接收文件
ss.receiveFiles(function (files) {
    var file = files[0];
    var reader = new FileReader();
    reader.onload = function (e) {
        editor.setValue(e.target.result);
    };
    reader.readAsText(file);
});