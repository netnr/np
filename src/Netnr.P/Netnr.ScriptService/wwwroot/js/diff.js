var editor, originContent = '', varyContent = '';

$('#chkDiffMode').click(changeDiffMode);
function changeDiffMode() {
    if (editor) {
        editor.updateOptions({
            renderSideBySide: !$('#chkDiffMode').is(':checked')
        });
    }
}
function createDiffEditor(text1, text2) {
    $('#editor').empty();
    require(['vs/editor/editor.main'], function () {
        var originalModel = monaco.editor.createModel(text1 || "", "text/plain");
        var modifiedModel = monaco.editor.createModel(text2 || "", "text/plain");
        editor = monaco.editor.createDiffEditor(document.getElementById("editor"), {
            automaticLayout: true,
            scrollbar: {
                verticalScrollbarSize: 6,
                horizontalScrollbarSize: 6
            },
            minimap: {
                enabled: false
            }
        });
        editor.setModel({
            original: originalModel,
            modified: modifiedModel
        });
        changeDiffMode();
    });
}
$('#fileOrigin').change(function () {
    if (this.files.length) {
        var reader = new FileReader();
        reader.onload = function () {
            createDiffEditor(originContent = this.result, varyContent);
        };
        reader.readAsText(this.files[0]);
    }
});
$('#txtOrigin').on('input', function () {
    createDiffEditor(originContent = this.value, varyContent);
});
$('#fileVary').change(function () {
    if (this.files.length) {
        var reader = new FileReader();
        reader.onload = function () {
            createDiffEditor(originContent, varyContent = this.result);
        };
        reader.readAsText(this.files[0]);
    }
});
$('#txtVary').on('input', function () {
    createDiffEditor(originContent, varyContent = this.value);
});

function AutoHeight() {
    var ch = $(window).height() - $('#editor').offset().top - 15;
    $('#editor').css('height', Math.max(200, ch));
}
$(window).on('load resize', AutoHeight);