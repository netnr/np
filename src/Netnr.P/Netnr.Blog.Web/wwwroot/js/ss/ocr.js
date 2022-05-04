nr.onReady = function () {

    nr.domImgView = new Image();
    nr.domCardView.appendChild(nr.domImgView);

    nr.domBtnRotateRight.addEventListener('click', function () {
        if (page.cp) {
            page.cp.rotate(5)
        } else {
            nr.alert('请先选择图片')
        }
    });
    nr.domBtnRotateLeft.addEventListener('click', function () {
        if (page.cp) {
            page.cp.rotate(-5)
        } else {
            nr.alert('请先选择图片')
        }
    });
    nr.domBtnReset.addEventListener('click', function () {
        if (page.cp) {
            page.cp.reset();
        } else {
            nr.alert('请先选择图片')
        }
    });
    nr.domBtnOcr.addEventListener('click', function () {
        if (page.cp) {
            page.scan()
        } else {
            nr.alert('请先选择图片')
        }
    });

    //接收文件
    nr.receiveFiles(function (files) {
        var isImg = false;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            if (file.type.startsWith("image")) {
                nr.domImgView.src = URL.createObjectURL(file);
                nr.domImgView.onload = function () {
                    page.crop();
                }
                isImg = true;
                break;
            }
        }
        if (!isImg) {
            nr.alert('不是图片哦');
        }
    }, nr.domTxtFile);
}

var page = {
    crop: function () {
        if (page.cp) {
            page.cp.destroy();
        }

        page.cp = new Cropper(nr.domImgView, { aspectRatio: NaN });
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
        if (page.cp) {

            nr.domBtnOcr.loading = true;
            ss.loading(true);

            var base64Data = page.cp.getCroppedCanvas().toDataURL('image/jpeg');
            var blob = page.dataURLtoBlob(base64Data);
            var file = page.blobToFile(blob, "ocr.jpg");

            //上传
            var formData = new FormData();
            formData.append("file", file);

            var url = `${ss.apiServer}/api/v1/OCR`;
            fetch(url, {
                method: 'POST',
                body: formData
            }).then(resp => resp.json()).then(res => {
                nr.domBtnOcr.loading = false;
                ss.loading(false);

                console.debug(res);
                if (res.code == 200 && res.data.words_result) {
                    var wrs = [];
                    res.data.words_result.map(item => wrs.push(item.words));
                    nr.domTxtResult.value = wrs.join("\n");
                    nr.domTxtResult.classList.remove("d-none");
                    document.documentElement.scrollTo(0, document.body.scrollHeight)
                } else {
                    nr.alert('识别失败');
                }
            }).catch(ex => {
                nr.domBtnOcr.loading = false;
                ss.loading(false);

                console.debug(ex);
                nr.alert('网络错误');
            })
        }
    }
}