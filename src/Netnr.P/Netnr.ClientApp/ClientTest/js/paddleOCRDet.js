import * as ocr from '@paddlejs-models/ocrdet';

// 方法
let paddleOCRDet = {
    init: async () => {
        document.body.appendChild(paddleOCRDet.canvasOutput);

        let domInput = document.createElement("input");
        domInput.type = "file";
        domInput.accept = "image/*";
        domInput.onchange = (e) => {
            let file = e.target.files[0];
            if (file) {
                paddleOCRDet.imgElement = new Image();
                paddleOCRDet.imgElement.src = URL.createObjectURL(file);
                paddleOCRDet.imgElement.onload = () => {
                    ocr.detect(paddleOCRDet.imgElement).then(res => {
                        console.debug(res);
                        paddleOCRDet.drawBox(res);
                    }).catch(ex => {
                        console.error(ex);
                    });
                }
            }
        };
        document.body.appendChild(domInput);

        console.debug("init");
        ocr.load().then(() => {
            console.debug("init done");
        });
    },

    canvasOutput: document.createElement("canvas"),
    imgElement: null,

    drawBox: (points) => {
        let canvasOutput = paddleOCRDet.canvasOutput;
        let imgElement = paddleOCRDet.imgElement;

        canvasOutput.width = imgElement.naturalWidth;
        canvasOutput.height = imgElement.naturalHeight;
        const ctx = canvasOutput.getContext('2d');
        ctx.drawImage(imgElement, 0, 0, canvasOutput.width, canvasOutput.height);
        points.forEach(point => {
            // 开始一个新的绘制路径
            ctx.beginPath();
            // 设置线条颜色为蓝色
            ctx.strokeStyle = 'blue';
            // 设置路径起点坐标
            ctx.moveTo(point[0][0], point[0][1]);
            ctx.lineTo(point[1][0], point[1][1]);
            ctx.lineTo(point[2][0], point[2][1]);
            ctx.lineTo(point[3][0], point[3][1]);
            ctx.closePath();
            ctx.stroke();
        });
    }
}

Object.assign(window, { paddleOCRDet });
export { paddleOCRDet };