//拖拽
$(document).on("dragleave dragenter dragover", function (e) {
    if (e && e.stopPropagation) { e.stopPropagation() } else { window.event.cancelBubble = true }
    if (e && e.preventDefault) { e.preventDefault() } else { window.event.returnValue = false }
}).on("drop", function (e) {
    if (e && e.preventDefault) { e.preventDefault() } else { window.event.returnValue = false }
    e = e || window.event;
    var files = (e.dataTransfer || e.originalEvent.dataTransfer).files;
    if (files && files.length) {
        fileAsBase64(files[0])
    }
});

$('#txtFile').change(function () {
    if (this.files.length) {
        fileAsBase64(this.files[0])
        $('#txtFile').val('')
    }
});

function fileAsBase64(file) {
    if (file.size / 1024 / 1024 > 3) {
        var msgs = [
            "<div style='font-size:1rem'>",
            "正在处理，请稍等，^_^",
            "<hr/>",
            "文件较大，浏览器会出现卡顿的情况",
            "</div>"
        ];
        jz.alert(msgs.join('<br/>'), { time: 5, ok: false })
    }

    var r = new FileReader();
    r.onload = function () {
        $('#txtBase64').val(this.result)
        $('#labSize').html("大小：" + (this.result.length / 1024).toFixed(1) + " K");
    }
    r.readAsDataURL(file);
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
    var code = $('#txtBase64').val();
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

$(window).on('load', function () {
    if (typeof (FileReader) === 'undefined') {
        jz.alert("你的浏览器不支持 FileReader <br />请使用现代浏览器操作！");
        $('#txtFile')[0].disabled = true;
    }
})