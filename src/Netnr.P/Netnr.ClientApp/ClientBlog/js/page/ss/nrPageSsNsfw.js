import { nrcFile } from "../../../../frame/nrcFile";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/nsfw",

    init: async () => {
        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrcFile.init((files) => {
            let isImg = false;
            for (let i = 0; i < files.length; i++) {
                let file = files[i];
                nrPage.isgif = file.type == "image/gif";

                if (file.type.indexOf("image") != -1) {
                    isImg = true;
                    nrPage.viewImage(file);
                    break;
                }
            }

            if (!isImg) {
                nrApp.toast('不是图片哦');
            }
        }, nrVary.domTxtFile);
    },

    img: null,
    isgif: false,
    model: null,
    modelVersion: null,

    viewImage: function (file) {
        let img = new Image();
        img.classList.add('mw-100', 'rounded');
        img.style.maxHeight = '32em';

        img.onload = async function () {
            nrVary.domSpinner.classList.remove('d-none');

            if (!nrPage.model) {
                await nrcRely.remote('nsfwjs');
                nrPage.model = await nsfwjs.load(nrPage.modelPath());
            }
            nrVary.domTxtResult.value = "正在识别...";

            let mf = nrPage.isgif ? "classifyGif" : "classify";
            let predictions = await nrPage.model[mf](nrPage.img);
            console.debug(predictions);
            nrVary.domTxtResult.value = JSON.stringify(predictions, null, 2);

            nrVary.domSpinner.classList.add('d-none');
            nrVary.domCardRight.classList.remove('invisible');
        }

        img.src = URL.createObjectURL(file);
        nrPage.img = img;

        nrVary.domCardView.style.height = '33.7em';
        nrVary.domCardView.innerHTML = "";
        nrVary.domCardView.appendChild(img);
        nrVary.domCardView.classList.add('p-3', 'border', 'rounded');
    },

    modelPath: function () {
        if (!nrPage.modelVersion) {
            document.scripts.forEach(si => {
                if (si.src.includes('/nsfwjs@')) {
                    nrPage.modelVersion = /nsfwjs(@\d+.\d+.\d+)/.exec(si.src)[0];
                }
            })
            nrPage.modelVersion = 'nsfwjs@2.4.0';
        }
        return 'https://cdn.staticaly.com/gh/infinitered/nsfwjs/master/example/nsfw_demo/public/quant_nsfw_mobilenet/';
    },
}

export { nrPage };