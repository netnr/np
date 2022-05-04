nr.onReady = function () {

    //接收文件
    nr.receiveFiles(function (files) {
        for (let index = 0; index < files.length; index++) {
            var file = files[index];
            if (file.type == "application/pdf") {
                var r = new FileReader();
                r.onload = function (e) {
                    page.loadUrl(page.base64ToUint8Array(e.target.result.split(',').pop()));
                }
                r.readAsDataURL(file);

                break;
            }
        }
        nr.domTxtFile.value = '';
    }, nr.domTxtFile);

    //上一页
    nr.domBtnPrev.addEventListener('click', function () {
        var index = parseInt(nr.domTxtNumber.value) - 1;
        page.pdfPage(index)
    });
    //下一页
    nr.domBtnNext.addEventListener('click', function () {
        var index = parseInt(nr.domTxtNumber.value) + 1;
        page.pdfPage(index)
    });
    //跳转
    nr.domTxtNumber.addEventListener('input', function () {
        var index = parseInt(nr.domTxtNumber.value);
        if (!isNaN(index)) {
            page.pdfPage(index)
        }
    });
    nr.domTxtNumber.addEventListener('blur', function () {
        var index = parseInt(nr.domTxtNumber.value);
        if (isNaN(index) || index < 1) {
            nr.domTxtNumber.value = 1;
        } else if (index > page.pdf.numPages) {
            nr.domTxtNumber.value = page.pdf.numPages;
        }
    });
    //最后一页
    nr.domBtnLast.addEventListener('click', function () {
        page.pdfPage(page.pdf.numPages)
    });
    //缩放
    nr.domSeScale.addEventListener('sl-change', function () {
        var index = parseInt(nr.domTxtNumber.value);
        page.pdfPage(index)
    });
    //旋转
    nr.domBtnRotate.addEventListener('click', function () {
        page.rotate += 90;
        if (page.rotate % 360 == 0) {
            page.rotate = 0;
        }
        var index = parseInt(nr.domTxtNumber.value);
        page.pdfPage(index)
    });

    //demo
    nr.domBtnDemo.addEventListener('click', function () {
        page.loadUrl('https://s1.netnr.com/2019/07/06/181724f50f.pdf')
    });

    //导出图片
    nr.domBtnExportImage.addEventListener('sl-select', function (e) {
        if (page.pdf == null) {
            nr.alert("请选择 PDF 文件");
        } else {
            var action = e.detail.item.getAttribute('data-action');
            switch (action) {
                case "export-image-page":
                    {
                        var canvas = nr.domCardView.querySelector('canvas');
                        nr.download(canvas, `page-${nr.domTxtNumber.value}.png`);
                    }
                    break;
                case "export-image-all":
                    {
                        if (page.pdf.numPages == 1) {
                            var canvas = nr.domCardView.querySelector('canvas');
                            nr.download(canvas, `page-${nr.domTxtNumber.value}.png`);
                        } else {
                            page.exportImage(1);
                        }
                    }
                    break;
            }
        }
    });
}

var page = {
    PDF_TO_CSS_UNITS: 96.0 / 72.0,
    rotate: 0,
    pdf: null,
    pdfPage: (index) => new Promise((resolve, reject) => {
        index = parseInt(index);
        if (isNaN(index)) {
            index = 1;
        }
        if (index > 0 && index <= page.pdf.numPages) {
            nr.domTxtNumber.value = index;

            page.pdf.getPage(index).then(pdfPage => {
                page.pdfView(pdfPage, index).then(resolve)
            }).catch(ex => reject(ex))
        }
    }),
    pdfView: (pdfPage) => {
        var txtScale = nr.domSeScale.value;
        if (txtScale == "auto") {
            txtScale = document.documentElement.clientWidth / pdfPage.view[3];
        }
        var viewport = pdfPage.getViewport({ scale: parseFloat(txtScale) * page.PDF_TO_CSS_UNITS, rotation: page.rotate });

        var domView = document.createElement('canvas');
        domView.width = viewport.width;
        domView.height = viewport.height;
        domView.classList.add('border', 'mw-100');;
        domView.style.boxShadow = "rgba(99, 99, 99, 0.2) 0px 2px 8px 0px";
        const ctx = domView.getContext("2d");

        nr.domCardView.innerHTML = "";
        nr.domCardView.appendChild(domView);

        return pdfPage.render({ canvasContext: ctx, viewport, }).promise;
    },
    loadUrl: function (url) {
        nr.domBtnDemo.loading = true;
        pdfjsLib.getDocument(url).promise.then(pdf => {
            nr.domBtnDemo.loading = false;
            nr.domTxtNumber.max = pdf.numPages;
            nr.domBtnLast.innerHTML = `/ ${pdf.numPages}`;
            nr.domCardTool.classList.remove('d-none');

            page.pdf = pdf;
            page.pdfPage(1);
        }).catch(ex => {
            console.debug(ex)
            nr.domBtnDemo.loading = false;
            nr.alert("载入 PDF 失败")
        })
    },
    loadFile: function (file) {
        var r = new FileReader();
        r.onload = function (e) {
            page.loadUrl(page.base64ToUint8Array(e.target.result.split(',').pop()));
        }
        r.readAsDataURL(file);
    },
    base64ToUint8Array: function (base64) {
        var raw = atob(base64);
        var uint8Array = new Uint8Array(raw.length);
        for (var i = 0; i < raw.length; i++) {
            uint8Array[i] = raw.charCodeAt(i);
        }
        return uint8Array;
    },
    zip: new JSZip(),
    exportImage: function (index) {
        index = index || 1;
        if (index == 1) {
            page.zip = new JSZip();

            nr.domBtnExportImage.children[0].loading = true;
            nr.domCardView.classList.add('invisible');
        }

        page.pdfPage(index).then(() => {
            var imgData = nr.domCardView.querySelector('canvas').toDataURL("image/png");
            page.zip.file(`page-${index}.png`, imgData.split(',')[1], { base64: true });
            if (index < page.pdf.numPages) {
                page.exportImage(index + 1);
            } else {
                nr.domBtnExportImage.children[0].loading = false;
                nr.domCardView.classList.remove('invisible');

                page.zip.generateAsync({ type: "blob" }).then(function (content) {
                    nr.download(content, `pages-${page.pdf.numPages}.zip`);
                });
            }
        })
    }
}