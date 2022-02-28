//接收文件
ss.receiveFiles(function (files) {
    fileAsBase64(files[0])
    $('#txtFile').val('')
}, "#txtFile");

var editor;
function showCode(code) {
    if (editor) {
        editor.setValue(code);
    } else {
        require(['vs/editor/editor.main'], function () {
            $('.nrEbox').removeClass('d-none');
            $('#btnBase64ToFile').parent().removeClass('d-none');

            editor = monaco.editor.create(document.getElementById("editor"), ss.meConfig({
                value: code,
                language: 'html',
                wordWrap: "on"
            }));
        });
    }
}

function fileAsBase64(file) {
    bs.msg("正在处理...");
    setTimeout(function () {
        var r = new FileReader();
        r.onload = function () {
            showCode(this.result);
            $('#labSize').html("大小：" + (this.result.length / 1024).toFixed(1) + " K");
            bs.obj.msgbox.firstChild.toast.hide();
        }
        r.readAsDataURL(file);
    }, 100)
}

function base64AsBlob(code) {
    var parts = code.split(';base64,');
    var contentType = parts[0].split(':')[1];
    var raw = window.atob(parts[1]);
    var rawLength = raw.length;
    var uInt8Array = new Uint8Array(rawLength);
    for (var i = 0; i < rawLength; ++i) {
        uInt8Array[i] = raw.charCodeAt(i);
    }
    return new Blob([uInt8Array], {
        type: contentType
    });
};

$('#btnBase64ToFile').click(function () {
    var code = editor.getValue();
    var blob = base64AsBlob(code);
    console.log(blob);
    var vbase = $('#viewBase64'), vnode;
    vbase.html('');
    if (blob.type.indexOf("image") >= 0) {
        vnode = document.createElement("img");
    }
    if (blob.type.indexOf("audio") >= 0) {
        vnode = document.createElement("audio");
        vnode.controls = true;
    }
    if (blob.type.indexOf("video") >= 0) {
        vnode = document.createElement("video");
        vnode.controls = true;
    }
    if (vnode) {
        vnode.src = URL.createObjectURL(blob);
    } else {
        vnode = document.createElement("a");
        vnode.href = URL.createObjectURL(blob);
        vnode.innerHTML = "下载";
    }
    vnode.style.maxWidth = "100%";
    vbase.append(vnode);
});