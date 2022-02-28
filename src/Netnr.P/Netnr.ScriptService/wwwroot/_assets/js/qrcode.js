var qrc = {
    defaultConfig: {
        errorCorrectionLevel: 'H',
        type: 'image/jpeg',
        quality: 0.3,
        width: 300,
        margin: 2,
        color: {
            dark: "#000000",
            light: "#FFFFFF"
        }
    },
    init: function () {
        $('#txt,.nrTxtConfig').on('input', function () {
            qrc.encode();
        });

        $('.nrBtnConfig').click(function () {
            if ($('.nrTxtConfig').hasClass('d-none')) {
                $('.nrTxtConfig').removeClass('d-none')
            } else {
                $('.nrTxtConfig').addClass('d-none')
            }
            if ($('.nrTxtConfig').val() == "") {
                $('.nrTxtConfig').val(JSON.stringify(qrc.defaultConfig, null, 4));
            }
        });

        //支持hash接口
        window.onhashchange = qrc.handlerHash();
        qrc.handlerHash();

        //接收
        ss.receiveFiles(function (files) {
            var file = files[0];
            qrc.decode(file);
        }, "#txtFile");
    },
    handlerHash: function () {
        var pars = new URLSearchParams(location.hash.substring(1));
        var text = pars.get('text');
        if (text != null && text != "") {
            $('#txt').val(text);
            qrc.encode();
        }
    },
    encode: function () {
        var txt = $('#txt').val();
        var options = qrc.defaultConfig;
        try {
            options = JSON.parse($('.nrTxtConfig').val());
        } catch (e) { }

        QRCode.toDataURL(txt, options, function (err, qrout) {
            if (err) {
                console.log(err);
                bs.msg("生成出错");
            } else {
                var img = new Image();
                img.src = qrout;
                img.className = "border";
                $('#divCodeResult').empty().append(img);
            }
        })
    },
    decode: function (file) {
        var fr = new FileReader();
        fr.readAsDataURL(file);
        fr.onloadend = function (e) {

            qrc.canvas = document.createElement("canvas");

            var img = new Image();
            img.src = e.target.result;
            img.onload = function () {
                var ctx = qrc.canvas.getContext("2d");
                qrc.canvas.width = img.width;
                qrc.canvas.height = img.height;
                ctx.drawImage(img, 0, 0, img.width, img.height);
                var imageData = ctx.getImageData(0, 0, img.width, img.height);

                var code = jsQR(imageData.data, imageData.width, imageData.height, {
                    inversionAttempts: "dontInvert",
                });
                if (code) {
                    console.log(code);
                    $('.nrResultCode').removeClass('d-none').val(code.data);
                } else {
                    bs.msg("识别二维码失败")
                }
            }
        }
    }
}

qrc.init();