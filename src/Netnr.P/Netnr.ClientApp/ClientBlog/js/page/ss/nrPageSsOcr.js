import { nrcFile } from "../../../../frame/nrcFile";
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/ocr",

    init: async () => {
        nrPage.domImg = new Image();
        nrVary.domCardView.appendChild(nrPage.domImg);

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domBtnRotateRight.addEventListener('click', function () {
            if (nrPage.cp) {
                nrPage.cp.rotate(5)
            } else {
                nrApp.alert('请先选择图片')
            }
        });
        nrVary.domBtnRotateLeft.addEventListener('click', function () {
            if (nrPage.cp) {
                nrPage.cp.rotate(-5)
            } else {
                nrApp.alert('请先选择图片')
            }
        });
        nrVary.domBtnReset.addEventListener('click', function () {
            if (nrPage.cp) {
                nrPage.cp.reset();
            } else {
                nrApp.alert('请先选择图片')
            }
        });
        nrVary.domBtnOcr.addEventListener('click', async function () {
            if (nrPage.cp) {
                await nrPage.scan()
            } else {
                nrApp.alert('请先选择图片')
            }
        });

        //接收文件
        nrcFile.init((files) => {
            let isImg = false;
            for (let i = 0; i < files.length; i++) {
                let file = files[i];
                if (file.type.startsWith("image")) {
                    nrPage.domImg.src = URL.createObjectURL(file);
                    nrPage.domImg.onload = async function () {
                        if (nrPage.cp) {
                            nrPage.cp.destroy();
                        } else {
                            await nrcRely.remote('cropperjs');
                        }
                        nrPage.cp = new Cropper(nrPage.domImg, { aspectRatio: NaN });
                    }
                    isImg = true;
                    break;
                }
            }
            if (!isImg) {
                nrApp.alert('不是图片哦');
            }
        }, nrVary.domTxtFile);
    },

    //识别
    scan: async () => {
        if (nrPage.cp) {
            nrApp.setLoading(nrVary.domBtnOcr);

            let blob = await nrcFile.canvasToBlob(nrPage.cp.getCroppedCanvas());
            //上传
            let fd = new FormData();
            fd.append("file", blob);

            let url = `https://netnr.zme.ink/api/v1/OCR`;
            let result = await nrWeb.reqServer(url, { method: "POST", body: fd });
            nrApp.setLoading(nrVary.domBtnOcr, true);

            if (result.code == 200 && result.data.words_result) {
                let wrs = [];
                result.data.words_result.map(item => wrs.push(item.words));
                nrVary.domTxtResult.value = wrs.join("\n");
                nrVary.domTxtResult.classList.remove("d-none");

                document.documentElement.scrollTo(0, document.body.scrollHeight)
            } else {
                nrApp.alert('识别失败');
            }
        } else {
            nrApp.alert('请先选择图片')
        }
    }
}

export { nrPage };