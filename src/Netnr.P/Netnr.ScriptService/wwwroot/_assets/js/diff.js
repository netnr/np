var de = {
    editor: document.getElementById("editor"),
    isHideMenu: location.hash == "#hide-menu",
    init: function () {

        if (de.isHideMenu) {
            $('.mobile-offcanvas').hide();
        }

        de.resize();
        $(window).on('resize', de.resize);

        require(['vs/editor/editor.main'], function () {
            de.originalModel = monaco.editor.createModel("just some text\n\nHello World\n\nSome more text", "text/plain");
            de.modifiedModel = monaco.editor.createModel("just some Text\n\nHello World\n\nSome more changes", "text/plain");

            de.diffEditor = monaco.editor.createDiffEditor(de.editor, ss.meConfig({
                originalEditable: true
            }));

            de.diffEditor.setModel({
                original: de.originalModel,
                modified: de.modifiedModel
            });

            de.den = monaco.editor.createDiffNavigator(de.diffEditor, {
                followsCaret: true,
                ignoreCharChanges: true
            });

            $('.nr-diff-inline').change(function () {
                de.diffEditor.updateOptions({
                    renderSideBySide: this.value != 1
                });
            });

            $('.nr-diff-next').click(function () {
                de.den.next();
            });

            de.receiveMessage();
        });
    },
    receiveMessage: function () {
        window.addEventListener("message", function (event) {
            try {
                var jsonData = JSON.parse(event.data);
                if (jsonData.origin != null && jsonData.vary != null) {
                    de.diffEditor.getOriginalEditor().setValue(jsonData.origin);
                    de.diffEditor.getModifiedEditor().setValue(jsonData.vary);
                }
            } catch (e) {
                console.error(e);
            }
        }, false);
    },
    resize: function () {
        var ch = $(window).height() - $('#editor').offset().top - (de.isHideMenu ? 5 : 20);
        $('#editor').height(Math.max(100, ch));
    }
}

de.init();

//接收文件
ss.receiveFiles(function (files) {
    bs.alert(`<div class="text-center"><button type="button" class="btn btn-danger me-3" data-cmd="left">左边 Left</button><button type="button" class="btn btn-info" data-cmd="right">右边 Right</button></div>`);
    bs.obj.alert._dialog.querySelector('.modal-footer').remove();
    bs.obj.alert._dialog.onclick = function (e) {
        var target = e.target, cmd = target.getAttribute("data-cmd");
        if (["left", "right"].includes(cmd)) {
            var file = files[0];
            var reader = new FileReader();
            reader.onload = function (e) {
                if (cmd == "left") {
                    de.diffEditor.getOriginalEditor().setValue(e.target.result);
                } else {
                    de.diffEditor.getModifiedEditor().setValue(e.target.result);
                }
                $("#txtFile").val('');
            };
            reader.readAsText(file);

            bs.obj.alert.hide();
        }
    }
}, "#txtFile");