nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor.getBoundingClientRect().top - 30;
    nr.domEditor.style.height = vh + "px";
}

nr.onReady = function () {
    ss.loading(true);

    let defaultLang = nr.lsStr("vscode-lang") || 'javascript';
    let defaultContent = nr.lsStr("vscode-content") || 'console.log("Hello world!");';

    me.init().then(() => {
        var modesIds = monaco.languages.getLanguages().map(lang => lang.id).sort();
        modesIds = modesIds.filter(x => !x.includes('.'));

        // 语言列表
        modesIds.forEach(lang => {
            let domItem = document.createElement('sl-menu-item');
            domItem.value = lang;
            domItem.innerHTML = lang;
            nr.domSeLanguage.appendChild(domItem);
        });

        var editor = me.create(nr.domEditor, {
            value: defaultContent,
            language: defaultLang,
        });
        nr.domEditor.classList.add('border');
        ss.loading(false);
        nr.domCardBox.classList.remove('invisible');

        nr.domSeLanguage.value = defaultLang;
        nr.domSeLanguage.addEventListener('sl-change', function () {
            me.setLanguage(editor, this.value);

            nr.ls["vscode-lang"] = this.value;
            nr.lsSave();

            if (this.value == "javascript") {
                nr.domBtnRun.classList.remove('d-none');
            } else {
                nr.domBtnRun.classList.add('d-none');
            }
        })

        editor.onDidChangeModelContent(function (e) {
            clearTimeout(window.defer1);
            window.defer1 = setTimeout(function () {
                nr.ls["vscode-content"] = editor.getValue();
                nr.lsSave();
            }, 1000 * 1)
        });

        editor.addCommand(monaco.KeyCode.PauseBreak, function () {
            nr.domBtnRun.click();
        })

        nr.domBtnRun.addEventListener('click', function () {
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
        nr.receiveFiles(function (files) {
            var file = files[0];
            var reader = new FileReader();
            reader.onload = function (e) {
                editor.setValue(e.target.result);
            };
            reader.readAsText(file);
        });
    });
}