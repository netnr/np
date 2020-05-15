//上传
function AjaxUpload(fd, url, success) {
    var xhr = new XMLHttpRequest();
    xhr.upload.onprogress = function (event) {
        if (event.lengthComputable) {
            var per = (event.loaded / event.total) * 100;
            $('#divProgress').show().css('width', per + "%");
            if (per >= 100) {
                $('#divProgress').hide().css('width', "0");
            }
        }
    };

    xhr.open("post", url, true);
    //xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
    xhr.send(fd);
    xhr.onreadystatechange = function (e) {
        if (xhr.readyState == 4) {
            if (xhr.status == 200) {
                success($.parseJSON(xhr.responseText), xhr)
            } else {
                $('#divProgress').hide().css('width', "0");
                jz.msg("上传异常");
            }
        }
    }
}

function CheckNewPageOutputResult() {
    if ($('#chk1')[0].checked) {
        $('#form1').attr('target', "_blank");
    } else {
        $('#form1').attr('target', "");
    }
}
$('#btnUpload1').click(function () {
    CheckNewPageOutputResult();
});



$('#dropupload2').on("dragleave dragenter dragover", function (e) {
    if (e && e.stopPropagation) { e.stopPropagation() } else { window.event.cancelBubble = true }
    if (e && e.preventDefault) { e.preventDefault() } else { window.event.returnValue = false }
}).on("drop", function (e) {
    if (e && e.preventDefault) { e.preventDefault() } else { window.event.returnValue = false }
    e = e || window.event;
    var files = (e.dataTransfer || e.originalEvent.dataTransfer).files;
    ActionUpload(files);
});
$('#btnUpload2').click(function () {
    if (typeof FormData == "function") {
        var fm = $('#form2');
        var files = fm.find('input[type="file"]')[0].files;
        ActionUpload(files);
        return false;
    }
});
function ActionUpload(files) {
    if (files && files.length) {
        var fd = new FormData();

        fd.append("file", files[0]);

        var fm = $('#form2');
        fd.append("title", fm.find('input[name="title"]').val());
        fd.append("desc", fm.find('input[name="desc"]').val());
        fd.append("cat", fm.find('input[name="cat"]').val());
        fd.append("group", fm.find('input[name="group"]').val());

        AjaxUpload(fd, fm.attr('action'), function (data) {
            if (data.img) {
                jz.msg("上传成功");
                $('<p><input class="form-control" value="' + data.img + '" /></p>').appendTo($('#divUploadResult2'));
            } else {
                if (data.error && data.error.length < 15 && data.error.indexOf('-') >= 0) {
                    var imgu = "https://uploadbeta.com/share-image/" + data.error.split('-')[1];
                    jz.alert({
                        title: "已经存在",
                        content: '<a href="' + imgu + '" target="_blank">' + imgu + '</a>'
                    });
                } else {
                    jz.alert(data.error || data.result || "上传失败");
                }
            }
        });
        return false;
    }
}