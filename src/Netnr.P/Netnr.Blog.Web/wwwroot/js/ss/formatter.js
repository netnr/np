nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor.getBoundingClientRect().top - 30;
    nr.domEditor.style.height = vh + "px";
}

nr.onReady = function () {
    ss.loading(true);
    me.init().then(() => {
        me.editor = me.create(nr.domEditor, {
            language: "javascript",
            value: "// 粘贴或拖拽代码"
        });

        nr.domEditor.classList.add('border');
        nr.domCardBox.classList.remove('invisible');
        ss.loading(false);

        //格式化
        me.editor.addCommand(monaco.KeyMod.Shift | monaco.KeyMod.Alt | monaco.KeyCode.KeyF, function () {
            codeFormatter()
        });
        nr.domBtnFormat.addEventListener('click', function () {
            codeFormatter()
        });

        nr.domSeLanguage.addEventListener('sl-change', function () {
            me.setLanguage(me.editor, this.value);
            codeFormatter()
        });
        nr.domTxtNumber.addEventListener('input', function () {
            codeFormatter()
        });

        //接收文件
        nr.receiveFiles(function (files) {
            var file = files[0];
            var reader = new FileReader();
            reader.onload = function (e) {
                me.keepSetValue(me.editor, e.target.result);
                nr.domTxtFile.value = "";
            };
            reader.readAsText(file);
        }, nr.domTxtFile);
    });
}

//格式化
function codeFormatter() {
    var lang = nr.domSeLanguage.value;
    var code = me.editor.getValue();
    if (code.trim() != "") {
        try {
            var blang = lang == "javascript" ? "js" : lang;
            var result = beautifier[blang](code, {
                indent_size: nr.domTxtNumber.value * 1,
                "max-preserve-newlines": 2
            });
            me.keepSetValue(me.editor, result);
        } catch (ex) {
            console.debug(ex);
            nr.alert("格式化失败");
        }
    }
}