var pg = {
    init: function () {

        $('#btnRotateLeft').click(function () {
            if (pg.cp) {
                pg.cp.rotate(-5)
            }
        });

        $('#btnRotateRight').click(function () {
            if (pg.cp) {
                pg.cp.rotate(5)
            }
        });

        $('#btnReset').click(function () {
            if (pg.cp) {
                pg.cp.reset()
            }
        });

        $('#btnOcr').click(function () {
            pg.scan();
        });

        //接收文件
        ss.receiveFiles(function (files) {
            var isImg = false;
            for (var i = 0; i < files.length; i++) {
                if (files[i].type.indexOf("image") != -1) {
                    isImg = true;
                    pg.iv.src = URL.createObjectURL(files[i]);
                    pg.crop();
                    break;
                }
            }
            if (!isImg) {
                bs.alert('<h4>不是图片哦</h4>');
            }
        }, "#txtFile");
    },

    iv: document.getElementById('imgview'),

    crop: function () {
        if (pg.cp) {
            pg.cp.destroy();
        }
        pg.cp = new Cropper(pg.iv, { aspectRatio: NaN });
    },

    //将base64转换为blob
    dataURLtoBlob: function (dataurl) {
        var arr = dataurl.split(','),
            mime = arr[0].match(/:(.*?);/)[1],
            bstr = atob(arr[1]),
            n = bstr.length,
            u8arr = new Uint8Array(n);
        while (n--) {
            u8arr[n] = bstr.charCodeAt(n);
        }
        return new Blob([u8arr], { type: mime });
    },
    //将blob转换为file
    blobToFile: function (theBlob, fileName) {
        theBlob.lastModifiedDate = new Date();
        theBlob.name = fileName;
        return theBlob;
    },

    //扫码识别
    scan: function () {
        if (pg.cp) {

            ss.loading(1);

            var base64Data = pg.cp.getCroppedCanvas().toDataURL('image/jpeg');
            var blob = pg.dataURLtoBlob(base64Data);
            var file = pg.blobToFile(blob, "ocr.jpg");

            //上传
            var formData = new FormData();
            formData.append("file", file);

            var tmpu = `${ss.apiServer}/api/v1/OCR`;
            //tmpu = 'https://api.netnr.eu.org/aip/ocr';

            fetch(tmpu, {
                method: 'POST',
                body: formData
            }).then(x => x.json()).then(res => {
                ss.loading(0);

                console.log(res);
                if (res.code == 200 && res.data.words_result) {
                    var wrs = [];
                    $.each(res.data.words_result, function () {
                        wrs.push(this.words);
                    });
                    $('#txtResult').removeClass('d-none').val(wrs.join('\r\n'));
                    document.documentElement.scrollTo(0, document.body.scrollHeight)
                } else {
                    bs.alert('<h4>接口异常</h4>');
                }
            }).catch(err => {
                ss.loading(0);

                console.log(err);
                bs.alert('<h4>上传出错</h4>');
            })
        }
    }
}

pg.init();