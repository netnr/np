import * as ocr from '@paddlejs-models/ocr';

// 方法
let paddleOCR = {
    init: async () => {
        let domCanvas = document.createElement("canvas");
        document.body.appendChild(domCanvas);

        let domInput = document.createElement("input");
        domInput.type = "file";
        domInput.accept = "image/*";
        domInput.onchange = (e) => {
            let file = e.target.files[0];
            if (file) {
                var img = new Image();
                img.src = URL.createObjectURL(file);
                img.onload = () => {
                    ocr.recognize(img, { canvas: domCanvas }).then(res => {
                        console.debug(res);
                    }).catch(ex => {
                        console.error(ex);
                    });
                }
            }
        };
        document.body.appendChild(domInput);
        console.debug("ocr init start");

        ocr.init().then(() => {
            console.debug("ocr init success");
        });
    },
}

Object.assign(window, { paddleOCR });
export { paddleOCR };