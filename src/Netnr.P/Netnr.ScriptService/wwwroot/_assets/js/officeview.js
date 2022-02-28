var ov = {
    api: "https://view.officeapps.live.com/op/embed.aspx?src=",
    init: function () {

        //接收文件
        ss.receiveFiles(function (files) {
            ov.upload(files[0]);
        }, "#txtFile");

        $('#txtUrl').on('input', function () {
            ov.view(this.value);
        });

        $(window).on('load resize', function () {
            ov.size();
        });
    },
    view: function (url) {
        if (url && url != "") {
            url = decodeURIComponent(url);
            $('#auri').attr('href', ov.api + url);
            $('#auri').html(ov.api + url);
            $('iframe').removeClass("d-none").attr('src', ov.api + url);
        } else {
            $('#auri').attr('href', 'javascript:void(0);');
            $('#auri').html(ov.api);
            $('iframe').addClass("d-none").attr('src', 'about:blank');
        }
        ov.size();
    },
    size: function () {
        var h = $(window).height() - $('iframe').offset().top - 15;
        $('iframe').height(Math.max(200, h)).width('100%');
    },
    upload: function (file) {
        var err = [];
        if (file.size > 1024 * 1024 * 20) {
            err.push('文档大小限制 20MB')
        }
        if (file.type.indexOf('application') == -1 || ".doc docx .xls xlsx .ppt pptx".indexOf(file.name.slice(-4).toLowerCase()) == -1) {
            err.push('请选择 Office文档')
        }
        if (err.length) {
            bs.alert(err.join('<br/>'));
            $("#txtFile").val('')
        } else {
            //上传
            var formData = new FormData();
            formData.append("file", file);

            ss.loading(1);
            $.ajax({
                url: "https://www.netnr.eu.org/api/v1/Upload",
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                dataType: 'json',
                xhr: function () {
                    xhr = $.ajaxSettings.xhr();
                    xhr.upload.addEventListener('progress', function (e) {
                        var rate = ((e.loaded / e.total) * 100).toFixed();
                        if (rate < 100) {
                            $('.nr-update-progress').removeClass('d-none').html('已上传 ' + rate + ' %');
                        } else {
                            $('.nr-update-progress').addClass('d-none');
                        }
                    })
                    return xhr;
                },
                success: function (data) {
                    console.log(data);
                    if (data.code == 200) {
                        ov.view("https://www.netnr.eu.org" + data.data.path);
                    } else {
                        bs.msg("<h4>" + data.msg + "</h4>");
                    }
                },
                error: function () {
                    bs.msg("<h4>上传出错</h4>");
                },
                complete: function () {
                    $('#txtFile').val('');
                    ss.loading(0);
                }
            });
        }
    }
}

ov.init();