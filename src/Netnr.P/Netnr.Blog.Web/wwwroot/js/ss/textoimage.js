nr.onReady = function () {
    ss.loading(true);

    me.init().then(() => {
        me.editor = me.create(nr.domEditor, {
            wordWrap: "on"
        });
        nr.domEditor.classList.add('border');
        nr.domEditor.style.height = "20em";

        nr.domCardBox.classList.remove('invisible');
        ss.loading(false);
    });

    // text to image
    nr.domBtnToImage.addEventListener('click', function () {
        var txt = me.editor.getValue();
        if (txt != "") {
            page.showImage(txt);
        }
    });

    // text to image
    nr.domBtnToText.addEventListener('click', function () {
        var imageLink = nr.domTxtImageLink.value.trim();
        var file = nr.domTxtFile.files[0];
        if (file && file.type.startsWith("image")) {
            page.showText(URL.createObjectURL(file));
        } else if (imageLink.length > 3) {
            page.showText(imageLink);
        }
    });

    //demo
    nr.domBtnDemo.addEventListener('click', function () {
        nr.domTxtImageLink.value = 'https://img13.360buyimg.com/ddimg/jfs/t1/204958/32/5863/175214/613ab6d5E0e894894/10cf6ad9fc63e04e.png';
        nr.domTxtFile.value = '';
        nr.domBtnToText.click();
    });
}

var page = {
    tti: new tti(),
    showText: function (src) {
        me.editor.setValue('正在解析为文本，请稍等 ... \r\n非文字转成的图片解析后是乱码 ... \r\n图片越大（亮）解析越慢 ... ');

        var domImg = new Image();
        domImg.src = src;
        domImg.style.height = "18em";
        domImg.style.maxWidth = "100%";
        domImg.onload = function () {
            try {
                page.tti.asText(domImg, function (txt) {
                    me.editor.setValue(txt);
                });
            } catch (ex) {
                console.debug(ex);
                me.editor.setValue(ex + "");
            }
        }
        nr.domCardView.innerHTML = '';
        nr.domCardView.appendChild(domImg);
    },
    showImage: function (txt) {
        var $base64 = page.tti.asImage(txt).toImage();
        nr.domCardView.innerHTML = `<div><img style="height:18em;max-width:100%" src="${$base64}"></div>
        <a download="tti.png" href="${$base64}">下载图片</a>`;
    }
}