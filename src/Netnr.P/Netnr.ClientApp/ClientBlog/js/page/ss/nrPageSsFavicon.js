import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrcFile } from "../../../../frame/nrcFile";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/favicon",

    cacheFile: null,
    init: async () => {

        //接收图片
        nrcFile.init(async files => {
            nrPage.cacheFile = files[0];
            nrVary.domImgPreview.src = await nrcFile.reader(nrPage.cacheFile, 'DataURL');

            nrVary.domTxtFile.value = "";
        }, nrVary.domTxtFile);

        //下载
        nrVary.domBtnDownload.addEventListener('click', function () {
            nrApp.setLoading(this);

            let size = nrVary.domSeSize.value * 1;

            let domCanvas = document.createElement('canvas');
            domCanvas.width = size;
            domCanvas.height = size;

            let ctx = domCanvas.getContext("2d");
            ctx.drawImage(nrVary.domImgPreview, 0, 0, nrVary.domImgPreview.naturalWidth, nrVary.domImgPreview.naturalHeight, 0, 0, size, size);
            domCanvas.toBlob(blob => {
                console.debug(blob)
                nrcBase.downloadBlob(blob, 'favicon.ico')
            }, 'image/vnd.microsoft.icon', '-moz-parse-options:format=bmp;bpp=32');

            nrApp.setLoading(this, true);
        });
    },

}

export { nrPage };