nr.onReady = function () {
    nr.domTxtContent.addEventListener('input', function () {
        qrc.encode();
    });

    nr.domBtnConfig.addEventListener('click', function () {
        nr.domTxtConfig.classList.toggle('d-none');
        nr.domTxtConfig.value = JSON.stringify(qrc.config, null, 2);
    });
    nr.domTxtConfig.addEventListener('input', function () {
        try {
            qrc.config = JSON.parse(nr.domTxtConfig.value);
        } catch (e) { }
        qrc.encode();
    });

    //支持hash接口
    window.addEventListener('hashchange', function () {
        qrc.handlerHash()
    });
    qrc.handlerHash();

    //接收
    nr.receiveFiles(function (files) {
        var file = files[0];
        qrc.decode(file);
    }, nr.domTxtFile);
}

var qrc = {
    config: {
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
    handlerHash: function () {
        var pars = new URLSearchParams(location.hash.substring(1));
        var text = pars.get('text');
        if (text != null && text != "") {
            nr.domTxtContent.value = text;
            qrc.encode();
        }
    },
    encode: function () {
        var txt = nr.domTxtContent.value;
        if (txt == "") {
            return
        }
        QRCode.toDataURL(txt, qrc.config, function (err, qrout) {
            if (err) {
                console.debug(err);
                nr.alert("生成出错");
            } else {
                var img = new Image();
                img.src = qrout;
                img.classList.add("border", "rounded", "mw-100");

                var domCard = document.createElement("div");
                domCard.classList.add("mt-3");
                var domDown = document.createElement("sl-button");
                domDown.variant = "warning";
                domDown.href = qrout;
                domDown.download = "qrcode.jpg";
                domDown.innerText = "下载二维码";
                domCard.appendChild(domDown);

                nr.domCardResult1.innerHTML = "";
                nr.domCardResult1.appendChild(img);
                nr.domCardResult1.appendChild(domCard);
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
                    nr.domTxtResult2.classList.remove('d-none');
                    nr.domTxtResult2.value = code.data;
                } else {
                    nr.alert("识别二维码失败")
                }
            }
        }
    }
}