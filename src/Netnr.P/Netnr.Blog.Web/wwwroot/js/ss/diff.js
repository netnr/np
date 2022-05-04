nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor.getBoundingClientRect().top - 30;
    nr.domEditor.style.height = vh + "px";
}

nr.onReady = function () {
    page.init()
}

var page = {
    isHideMenu: location.hash == "#hide-menu",
    init: function () {

        if (page.isHideMenu) {
            nr.domNavbar.classList.add('d-none');
        }

        me.init().then(() => {
            page.originalModel = monaco.editor.createModel("just some text\n\nHello World\n\nSome more text", "text/plain");
            page.modifiedModel = monaco.editor.createModel("just some Text\n\nHello World\n\nSome more changes", "text/plain");

            page.diffEditor = monaco.editor.createDiffEditor(nr.domEditor, me.config({
                originalEditable: true
            }));

            page.diffEditor.setModel({
                original: page.originalModel,
                modified: page.modifiedModel
            });
            nr.domEditor.classList.add('border');
            nr.domCardBox.classList.remove('invisible');

            page.den = monaco.editor.createDiffNavigator(page.diffEditor, {
                followsCaret: true,
                ignoreCharChanges: true
            });

            nr.domSeDiff.addEventListener('sl-change', function () {
                page.diffEditor.updateOptions({
                    renderSideBySide: this.value != 1
                });
            });

            nr.domBtnNext.addEventListener('click', function () {
                page.den.next();
            });

            //接收消息
            page.receiveMessage();

            //接收文件
            nr.receiveFiles(function (files) {
                var domDialog = nr.dialog(`<div class="text-center">
                    <sl-button class="me-4" data-cmd="left">左边 Left</sl-button>
                    <sl-button data-cmd="right">右边 Right</sl-button>
                </div>`);

                domDialog.addEventListener('click', function (e) {
                    var target = e.target, cmd = target.dataset.cmd;
                    if (["left", "right"].includes(cmd)) {
                        var file = files[0];
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            if (cmd == "left") {
                                page.diffEditor.getOriginalEditor().setValue(e.target.result);
                            } else {
                                page.diffEditor.getModifiedEditor().setValue(e.target.result);
                            }
                            nr.domTxtFile.value = "";
                        };
                        reader.readAsText(file);

                        domDialog.remove();
                    }
                })
            }, nr.domTxtFile);

        });
    },
    receiveMessage: function () {
        window.addEventListener("message", function (event) {
            try {
                var jsonData = JSON.parse(event.data);
                if (jsonData.origin != null && jsonData.vary != null) {
                    page.diffEditor.getOriginalEditor().setValue(jsonData.origin);
                    page.diffEditor.getModifiedEditor().setValue(jsonData.vary);
                }
            } catch (e) {
                console.error(e);
            }
        }, false);
    }
}