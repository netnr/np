let nrcViewer = {

    domContainer: null,
    domCanvas: null,
    context: null,

    // Canvas
    mouseDown: false,
    mousePos: [0, 0],

    // Defaults
    DEFAULT_ZOOM: 1,
    MAX_ZOOM: 4,
    MIN_ZOOM: .2,
    ZOOM_STEP: .1,
    DRAW_POS: [0, 0],
    ANGLE: 0,

    image: new Image(),
    loaded: false,
    isShow: false,
    drawPos: [0, 0],
    scale: 1,
    rotationAngle: 0, // 图片旋转角度
    /**
     * 角度转为弧度
     * @param {*} degrees 
     * @returns 
     */
    degreesToRadians: (degrees) => degrees * Math.PI / 180,

    /**
     * 
     * @param {*} option src 资源
     */
    init: (option) => {
        nrcViewer.isShow = true;
        document.documentElement.style.overflow = "hidden";

        if (!nrcViewer.domContainer) {
            nrcViewer.domContainer = document.createElement("div");
            nrcViewer.domContainer.className = "nrc-viewer";
            let domStyle = document.createElement("style");
            domStyle.innerHTML = `
            .nrc-viewer { position:fixed;z-index:99;top:0;width:100%;height:100%;background-color:rgba(0,0,0,.8); }
            .nrc-viewer canvas { cursor:move }
            .nrc-viewer-loading::after { content: "loading..";font-size:3em;color:orange;display:block;text-align:center;line-height:90vh; }
            .nrc-viewer-loading canvas { display:none }
            .nrc-viewer span { position:absolute;top:0.2em;right:0.3em;font-size:1.2em;user-select:none;cursor:pointer; }
            `;
            nrcViewer.domContainer.innerHTML = `<span title="Click to close or press ESC\r\nPress the arrow keys to rotate ← ↑ → ↓">❌</span>`;
            document.head.appendChild(domStyle);

            document.body.appendChild(nrcViewer.domContainer);

            nrcViewer.domCanvas = document.createElement("canvas");
            nrcViewer.domContainer.appendChild(nrcViewer.domCanvas)
            nrcViewer.context = nrcViewer.domCanvas.getContext('2d');

            nrcViewer.domCanvas.addEventListener('mousewheel', function (e) {
                e.preventDefault();

                let isZoomIn = e.wheelDelta > 0;
                if (isZoomIn) {
                    if (nrcViewer.scale < nrcViewer.MAX_ZOOM) {
                        nrcViewer.scale += nrcViewer.ZOOM_STEP;
                    }
                } else {
                    if (nrcViewer.scale > nrcViewer.MIN_ZOOM) {
                        nrcViewer.scale -= nrcViewer.ZOOM_STEP;
                    }
                }

                nrcViewer.view();
            });
            nrcViewer.domCanvas.addEventListener('mousedown', function (e) {
                nrcViewer.mouseDown = true;
                nrcViewer.mousePos = [e.x, e.y];
            });
            nrcViewer.domCanvas.addEventListener('mouseup', function () {
                nrcViewer.mouseDown = false;
            });
            nrcViewer.domCanvas.addEventListener('mousemove', function (e) {
                if (nrcViewer.mouseDown) {
                    let delta = [e.x - nrcViewer.mousePos[0], e.y - nrcViewer.mousePos[1]];
                    nrcViewer.drawPos = [nrcViewer.drawPos[0] + delta[0], nrcViewer.drawPos[1] + delta[1]];
                    nrcViewer.mousePos = [e.x, e.y];

                    nrcViewer.view();
                }
            });
            nrcViewer.domCanvas.addEventListener('dblclick', nrcViewer.reset);
            nrcViewer.domContainer.querySelector('span').addEventListener('click', nrcViewer.hide)

            window.addEventListener('resize', function () {
                Object.assign(nrcViewer.domCanvas, {
                    width: nrcViewer.domContainer.clientWidth,
                    height: nrcViewer.domContainer.clientHeight
                })

                nrcViewer.view();
            })
            document.body.addEventListener('keydown', function (e) {
                console.debug(e);
                if (e.code == "Escape") {
                    nrcViewer.hide();
                } else if (e.code == "ArrowLeft") {
                    nrcViewer.rotate(-90);
                } else if (e.code == "ArrowRight") {
                    nrcViewer.rotate(-270);
                } else if (e.code == "ArrowDown") {
                    nrcViewer.rotate(-180);
                } else if (e.code == "ArrowUp") {
                    nrcViewer.rotate(0);
                }
            })

            nrcViewer.image.onload = function () {
                nrcViewer.loaded = true;
                nrcViewer.domContainer.classList.remove('nrc-viewer-loading');

                Object.assign(nrcViewer.domCanvas, {
                    width: nrcViewer.domContainer.clientWidth,
                    height: nrcViewer.domContainer.clientHeight
                })

                nrcViewer.DRAW_POS = [nrcViewer.domCanvas.clientWidth / 2, nrcViewer.domCanvas.clientHeight / 2]
                nrcViewer.drawPos = nrcViewer.DRAW_POS;
                nrcViewer.scale = nrcViewer.DEFAULT_ZOOM;
                nrcViewer.rotationAngle = nrcViewer.ANGLE;

                nrcViewer.view();
            }
        }

        nrcViewer.loaded = false;
        nrcViewer.domContainer.style.removeProperty('display');
        nrcViewer.domContainer.classList.add('nrc-viewer-loading');
        nrcViewer.image.src = option.src;
    },

    view: () => {
        // Draw the canvas
        nrcViewer.context.fillStyle = "#212529";
        nrcViewer.context.fillRect(0, 0, nrcViewer.domCanvas.width, nrcViewer.domCanvas.height);

        // 设置图片旋转
        let radian = nrcViewer.degreesToRadians(nrcViewer.rotationAngle);
        nrcViewer.context.setTransform(Math.cos(radian) * nrcViewer.scale, Math.sin(radian) * nrcViewer.scale,
            -Math.sin(radian) * nrcViewer.scale, Math.cos(radian) * nrcViewer.scale, nrcViewer.drawPos[0], nrcViewer.drawPos[1]);

        // 绘制图片
        let w = nrcViewer.image.width;
        let h = nrcViewer.image.height;
        let x = -(w / 2);
        let y = -(h / 2);
        nrcViewer.context.drawImage(nrcViewer.image, x, y, w, h);

        // 恢复原始变换矩阵
        nrcViewer.context.setTransform(1, 0, 0, 1, 0, 0);
    },

    // 新增的旋转函数
    rotate: (angle) => {
        //角度转换为弧度
        nrcViewer.rotationAngle = angle;
        nrcViewer.view();
    },

    hide: () => {
        nrcViewer.isShow = false;
        document.documentElement.style.removeProperty('overflow')
        nrcViewer.domContainer.style.display = "none";
    },

    reset: () => {
        nrcViewer.DRAW_POS = [nrcViewer.domCanvas.clientWidth / 2, nrcViewer.domCanvas.clientHeight / 2]
        nrcViewer.drawPos = nrcViewer.DRAW_POS;
        nrcViewer.scale = nrcViewer.DEFAULT_ZOOM;
        nrcViewer.rotationAngle = 0;

        nrcViewer.view();
    },

}

export { nrcViewer }
