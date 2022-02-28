require(['vs/editor/editor.main'], function () {

    var defaultContent = ss.lsStr("vscode-content") || '',
        defaultLang = ss.lsStr("vscode-lang") || 'javascript',
        formatterLang = ss.lsStr("formatter-lang") || 'javascript';

    window.ee = monaco.editor.create(document.getElementById("editor"), ss.meConfig({
        value: defaultContent,
        language: defaultLang
    }));

    ee.onDidChangeModelContent(function (e) {
        clearTimeout(window.defer1);
        window.defer1 = setTimeout(function () {
            var txt = ee.getValue();
            ss.ls["vscode-content"] = txt;
            ss.lsSave();
        }, 1000 * 1)
    });

    //格式化
    ee.addCommand(monaco.KeyMod.Shift | monaco.KeyMod.Alt | monaco.KeyCode.KeyF, function () {
        ss.keepSetValue(ee, codeFormatter(ee.getValue()))
    })

    //切换语言
    $('.nr-lang').change(function () {
        var lang = this.value.split('-')[0];

        var oldModel = ee.getModel();
        var newModel = monaco.editor.createModel(ee.getValue(), lang);
        ee.setModel(newModel);
        if (oldModel) {
            oldModel.dispose();
        }

        ss.ls["formatter-lang"] = this.value;
        ss.ls["vscode-lang"] = lang;
        ss.lsSave();
    }).val(formatterLang);

    //格式化
    $('#btnFormatter').click(function () {
        ss.keepSetValue(ee, codeFormatter(ee.getValue()))
    });
    $('.nr-tab-width').on('input', function () {
        $('#btnFormatter')[0].click();
    });

    $(window).on('load resize', function () {
        var ch = $(window).height() - $('#editor').offset().top - 15;
        $('#editor').css('height', Math.max(200, ch));
    });

    //接收文件
    ss.receiveFiles(function (files) {
        var file = files[0];
        var reader = new FileReader();
        reader.onload = function (e) {
            ee.setValue(e.target.result);
        };
        reader.readAsText(file);
    });
});

//格式化
function codeFormatter(code, lang, options) {
    lang = lang || document.querySelector('.nr-lang').value;
    if (code != "") {
        try {
            var ops = {
                indent_size: document.querySelector('.nr-tab-width').value * 1,
                "max-preserve-newlines": 2
            };
            var blang = lang == "javascript" ? "js" : lang;
            return beautifier[blang](code, ops);
        } catch (e) {
            console.log(e);
            bs.msg("<h4>ERROR</h4>");
        }
    }
}