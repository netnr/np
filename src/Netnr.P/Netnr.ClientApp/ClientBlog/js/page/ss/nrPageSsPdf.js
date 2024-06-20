import { nrcFile } from "../../../../frame/nrcFile";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/pdf",

    init: async () => {
        nrPage.bindEvent();
    },

    bindEvent: () => {
        //接收文件
        nrcFile.init(async (files) => {
            for (const file of files) {
                if (file.type == "application/pdf") {
                    let dataURL = await nrcFile.reader(file, 'DataURL');
                    await nrPage.loadUrl(dataURL);

                    break;
                }
            }
            nrVary.domTxtFile.value = '';
        }, nrVary.domTxtFile);

        //上一页
        nrVary.domBtnPrev.addEventListener('click', async function () {
            let index = parseInt(nrVary.domTxtNumber.value) - 1;
            await nrPage.getPage(index)
        });
        //下一页
        nrVary.domBtnNext.addEventListener('click', async function () {
            let index = parseInt(nrVary.domTxtNumber.value) + 1;
            await nrPage.getPage(index)
        });
        //跳转
        nrVary.domTxtNumber.addEventListener('input', async function () {
            let index = parseInt(nrVary.domTxtNumber.value);
            if (!isNaN(index)) {
                await nrPage.getPage(index)
            }
        });
        nrVary.domTxtNumber.addEventListener('blur', function () {
            let index = parseInt(nrVary.domTxtNumber.value);
            if (isNaN(index) || index < 1) {
                nrVary.domTxtNumber.value = 1;
            } else if (index > nrPage.pdf.numPages) {
                nrVary.domTxtNumber.value = nrPage.pdf.numPages;
            }
        });
        //最后一页
        nrVary.domBtnLast.addEventListener('click', async function () {
            await nrPage.getPage(nrPage.pdf.numPages)
        });
        //缩放
        nrVary.domSeScale.addEventListener('input', async function () {
            let index = parseInt(nrVary.domTxtNumber.value);
            await nrPage.getPage(index)
        });
        //旋转
        nrVary.domBtnRotate.addEventListener('click', async function () {
            nrPage.rotate += 90;
            if (nrPage.rotate % 360 == 0) {
                nrPage.rotate = 0;
            }
            let index = parseInt(nrVary.domTxtNumber.value);
            await nrPage.getPage(index)
        });

        //demo
        nrVary.domBtnDemo.addEventListener('click', async function () {
            await nrPage.loadUrl('https://gs.zme.ink/2019/07/06/181724f50f.pdf')
        });

        //导出图片
        document.body.addEventListener('click', async function (e) {
            let action = e.target.dataset.action;
            switch (action) {
                case "export-image-page":
                    {
                        if (nrPage.pdf == null) {
                            nrApp.alert("请选择 PDF 文件");
                        } else {
                            let canvas = nrVary.domCardView.querySelector('canvas');
                            nrcBase.downloadCanvas(canvas, `page-${nrVary.domTxtNumber.value}.png`);
                        }
                    }
                    break;
                case "export-image-all":
                    {
                        if (nrPage.pdf == null) {
                            nrApp.alert("请选择 PDF 文件");
                        } else {
                            if (nrPage.pdf.numPages == 1) {
                                let canvas = nrVary.domCardView.querySelector('canvas');
                                nrcBase.downloadCanvas(canvas, `page-${nrVary.domTxtNumber.value}.png`);
                            } else {
                                await nrPage.exportImage(1);
                            }
                        }
                    }
                    break;
            }
        });
    },

    PDF_TO_CSS_UNITS: 96.0 / 72.0,
    rotate: 0,
    pdf: null,

    /**
     * 获取页
     * @param {*} index 
     */
    getPage: async (index) => {
        index = parseInt(index);
        if (isNaN(index)) {
            index = 1;
        }

        if (index > 0 && index <= nrPage.pdf.numPages) {
            nrVary.domTxtNumber.value = index;

            let pdfPage = await nrPage.pdf.getPage(index);
            await nrPage.viewPage(pdfPage);
        }
    },

    viewPage: async (pdfPage) => {
        let txtScale = nrVary.domSeScale.value;
        if (txtScale == "auto") {
            txtScale = document.documentElement.clientWidth / pdfPage.view[3];
        }
        let viewport = pdfPage.getViewport({ scale: parseFloat(txtScale) * nrPage.PDF_TO_CSS_UNITS, rotation: nrPage.rotate });

        let domView = document.createElement('canvas');
        domView.width = viewport.width;
        domView.height = viewport.height;
        domView.classList.add('border', 'mw-100');;
        domView.style.boxShadow = "rgba(99, 99, 99, 0.2) 0px 2px 8px 0px";
        const ctx = domView.getContext("2d");

        nrVary.domCardView.innerHTML = "";
        nrVary.domCardView.appendChild(domView);

        return pdfPage.render({ canvasContext: ctx, viewport, }).promise;
    },

    loadUrl: async (url) => {
        nrApp.setLoading(nrVary.domBtnDemo);

        try {
            await nrcRely.remote('pdf.js');

            let pdf = await pdfjsLib.getDocument(url).promise;

            nrVary.domTxtNumber.max = pdf.numPages;
            nrVary.domBtnLast.innerHTML = `/ ${pdf.numPages}`;
            document.querySelectorAll('.flag-hide').forEach(dom => {
                dom.classList.remove('d-none')
            })

            nrPage.pdf = pdf;
            await nrPage.getPage(1);
        } catch (ex) {
            nrApp.logError(ex, '网络错误');
        }

        nrApp.setLoading(nrVary.domBtnDemo, true);
    },

    exportImage: async (index) => {
        index = index || 1;

        if (index == 1) {
            nrApp.setLoading(nrVary.domBtnDemo);
            nrApp.setLoading(nrVary.domBtnDdExport);
            nrVary.domCardView.classList.add('invisible');

            await nrcRely.remote('jszip.js');

            nrPage.zip = new JSZip();
        }

        await nrPage.getPage(index);

        let imgData = nrVary.domCardView.querySelector('canvas').toDataURL("image/png");
        nrPage.zip.file(`page-${index}.png`, imgData.split(',')[1], { base64: true });

        if (index < nrPage.pdf.numPages) {
            await nrPage.exportImage(index + 1);
        } else {
            nrApp.setLoading(nrVary.domBtnDemo, true);
            nrApp.setLoading(nrVary.domBtnDdExport, true);
            nrVary.domCardView.classList.remove('invisible');

            let content = await nrPage.zip.generateAsync({ type: "blob" });
            nrcBase.downloadBlob(content, `pages-${nrPage.pdf.numPages}.zip`);
        }
    }
}

export { nrPage };