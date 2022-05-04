nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor.getBoundingClientRect().top - 30;
    nr.domEditor.style.height = vh + "px";
}

nr.onReady = function () {
    ss.loading(true);

    var dv = nr.lsStr('txt') || 'SELECT * FROM table1 表名',
        scLanguage = nr.lsStr('sql-config-language') || 'sql',
        scUpperCase = nr.lsStr('sql-config-uppercase') || "1";
        
    nr.domSeLanguage.value = scLanguage;
    nr.domSeUppercase.value = scUpperCase;

    me.init().then(() => {

        me.editor = me.create(nr.domEditor, {
            language: "sql",
            value: dv
        });

        nr.domEditor.classList.add('border');
        nr.domCardBox.classList.remove('invisible');
        ss.loading(false);

        me.onChange(me.editor, value => {
            nr.ls['txt'] = value;
            nr.lsSave();
        });

        //格式化
        me.editor.addCommand(monaco.KeyMod.Shift | monaco.KeyMod.Alt | monaco.KeyCode.KeyF, function () {
            codeFormatter()
        });
        nr.domBtnFormat.addEventListener('click', function () {
            codeFormatter()
        });

        nr.domSeLanguage.addEventListener('sl-change', function () {
            codeFormatter()
            nr.ls['sql-config-language'] = this.value;
            nr.lsSave();
        });
        nr.domSeUppercase.addEventListener('sl-change', function () {
            codeFormatter()
            nr.ls['sql-config-uppercase'] = this.value;
            nr.lsSave();
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
    var code = me.editor.getValue();
    if (code.trim() != "") {
        try {
            var result = sqlFormatter.format(code, {
                language: nr.domSeLanguage.value,
                uppercase: nr.domSeUppercase.value == "1"
            });
            me.keepSetValue(me.editor, result);
        } catch (ex) {
            console.debug(ex);
            nr.alert("格式化失败");
        }
    }
}