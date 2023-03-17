import { nrcFile } from "../../../../frame/nrcFile";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/qrcode",

    init: async () => {
        nrPage.bindEvent();
    },

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

    bindEvent: () => {
        nrVary.domTxtContent.addEventListener('input', async function () {
            await nrPage.encode();
        });

        nrVary.domBtnConfig.addEventListener('click', function () {
            nrVary.domTxtConfig.classList.toggle('d-none');
            nrVary.domTxtConfig.value = JSON.stringify(nrPage.config, null, 2);
        });
        nrVary.domTxtConfig.addEventListener('input', async function () {
            try {
                nrPage.config = JSON.parse(nrVary.domTxtConfig.value);
            } catch (e) { }
            await nrPage.encode();
        });

        //支持hash接口
        window.addEventListener('hashchange', async function () {
            console.debug('hash')
            let pars = new URLSearchParams(location.hash.substring(1));
            let text = pars.get('text');
            if (text != null && text != "") {
                nrVary.domTxtContent.value = text;
                await nrPage.encode();
            }
        });
        

        //接收
        nrcFile.init(async (files) => {
            await nrPage.decode(files[0]);
        }, nrVary.domTxtFile);
    },

    /**
     * 生成
     */
    encode: async () => {
        let txt = nrVary.domTxtContent.value;
        if (txt != "") {
            await nrcRely.remote('qrcode.js');

            QRCode.toDataURL(txt, nrPage.config, function (err, qrout) {
                if (err) {
                    console.debug(err);
                    nrApp.alert("生成出错");
                } else {
                    let img = new Image();
                    img.src = qrout;
                    img.classList.add("border", "rounded", "mw-100");

                    let domCard = document.createElement("div");
                    let domDown = document.createElement("a");
                    Object.assign(domDown, {
                        href: qrout,
                        className: "btn btn-warning mt-2",
                        download: "qrcode.jpg",
                        innerText: "下载二维码",
                    })
                    domCard.appendChild(domDown);

                    nrVary.domCardResult1.innerHTML = "";
                    nrVary.domCardResult1.appendChild(img);
                    nrVary.domCardResult1.appendChild(domCard);
                }
            })
        }
    },

    /**
     * 识别
     * @param {*} file 
     */
    decode: async (file) => {
        let img = new Image();
        img.src = await nrcFile.reader(file, 'DataURL');;
        img.onload = async () => {
            let domCanvas = document.createElement("canvas");
            let ctx = domCanvas.getContext("2d");
            domCanvas.width = img.width;
            domCanvas.height = img.height;
            ctx.drawImage(img, 0, 0, img.width, img.height);
            let imageData = ctx.getImageData(0, 0, img.width, img.height);

            await nrcRely.remote('jsqr.js');
            let code = jsQR(imageData.data, imageData.width, imageData.height, {
                inversionAttempts: "dontInvert",
            });
            if (code) {
                nrVary.domTxtResult2.classList.remove('d-none');
                nrVary.domTxtResult2.value = code.data;
            } else {
                nrApp.alert("识别二维码失败")
            }
        }
    }
}

export { nrPage };