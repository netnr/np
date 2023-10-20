let nrcSplit = {
    addStyle: () => {
        if (!nrcSplit.domStyle) {
            nrcSplit.domStyle = document.createElement("style");
            nrcSplit.domStyle.innerHTML = `
html {
    --nrc-divider-color: #2b3035;
}

.nrc-split-horizontal,
.nrc-split-vertical {
    display: flex;
    overflow: hidden;
    box-sizing: border-box;
}

.nrc-split-horizontal>*,
.nrc-split-vertical>* {
    overflow: hidden;
    position: relative;
}

.nrc-split-horizontal {
    height: 100%;
}

.nrc-split-vertical {
    flex-direction: column;
}

.nrc-split-horizontal>.nrc-split-divider,
.nrc-split-vertical>.nrc-split-divider {
    position: relative;
    padding: 3px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: var(--nrc-divider-color);
}

.nrc-split-horizontal>.nrc-split-divider {
    z-index: 2;
    cursor: col-resize;
}

.nrc-split-horizontal>.nrc-split-divider::before,
.nrc-split-horizontal>.nrc-split-divider::after {
    content: "";
    width: 2px;
    height: 16px;
    margin: 2px;
    background: currentColor;
}

.nrc-split-vertical>.nrc-split-divider {
    z-index: 1;
    cursor: row-resize;
    flex-direction: column;
}

.nrc-split-vertical>.nrc-split-divider::before,
.nrc-split-vertical>.nrc-split-divider::after {
    content: "";
    width: 16px;
    height: 2px;
    margin: 2px;
    background: currentColor;
}`;
            document.head.appendChild(nrcSplit.domStyle);
        }
    },

    /**
     * 创建
     * @param {*} domSplit 容器节点
     * @param {*} position 位置，默认60
     * @param {*} doneCall 变更回调
     */
    create: (domSplit, position, doneCall) => {
        nrcSplit.addStyle();

        let isVertical = domSplit.classList.contains("nrc-split-vertical");
        let domDivider = domSplit.children[1];

        if (isVertical) {
            nrcSplit.sideVertical(domDivider, doneCall);
        } else {
            nrcSplit.sideHorizontal(domDivider, doneCall);
        }

        nrcSplit.setPosition(domSplit, position || 60);
    },

    /**
     * 设置位置
     * @param {*} domSplit 
     * @param {*} position 
     */
    setPosition: (domSplit, position) => {
        domSplit.children[0].style.flex = `0 ${position}%`;
        domSplit.children[2].style.flex = "1 0";
    },

    /**
     * 获取位置
     * @param {*} domSplit 
     * @returns 
     */
    getPosition: (domSplit) => {
        let fvals = getComputedStyle(domSplit.children[0]).flex.split(' ');
        let pos;
        if (fvals[1] == 1) {
            pos = parseFloat(fvals.pop());
        } else {
            fvals = getComputedStyle(domSplit.children[2]).flex.split(' ');
            pos = 100 - parseFloat(fvals.pop());
        }

        return pos;
    },

    /**
     * 水平滑动
     * @param {*} domDivider 滑动节点
     * @param {*} doneCall 变更回调
     */
    sideHorizontal: (domDivider, doneCall) => {
        domDivider.addEventListener("mousedown", onmousedown);
        domDivider.addEventListener("touchstart", ontouchstart);

        const l = domDivider.previousElementSibling;
        const r = domDivider.nextElementSibling;

        // for mobile
        function ontouchstart(e) {
            e.preventDefault();

            l.style.pointerEvents = 'none';
            r.style.pointerEvents = 'none';

            domDivider.addEventListener("touchmove", ontouchmove);
            domDivider.addEventListener("touchend", ontouchend);
        }
        function ontouchmove(e) {
            e.preventDefault();

            const clientX = e.touches[0].clientX;
            const deltaX = clientX - (domDivider._clientX || clientX);
            domDivider._clientX = clientX;

            // LEFT
            if (deltaX < 0) {
                let w = Math.round(parseInt(getComputedStyle(l).width) + deltaX);
                if (w < 10) {
                    w = 0;
                } else {
                    w = (w / domDivider.parentNode.clientWidth * 100).toFixed(9);
                }

                l.style.flex = `0 ${w}%`;
                r.style.flex = "1 0";
            }
            // RIGHT
            if (deltaX > 0) {
                let w = Math.round(parseInt(getComputedStyle(r).width) - deltaX);
                if (w < 10) {
                    w = 0;
                } else {
                    w = (w / domDivider.parentNode.clientWidth * 100).toFixed(9);
                }

                r.style.flex = `0 ${w}%`;
                l.style.flex = "1 0";
            }
        }
        function ontouchend(e) {
            e.preventDefault();

            l.style.removeProperty('pointer-events');
            r.style.removeProperty('pointer-events');

            domDivider.removeEventListener("touchmove", ontouchmove);
            domDivider.removeEventListener("touchend", ontouchend);

            delete domDivider._clientX;

            if (doneCall) {
                doneCall()
            }
        }

        // for desktop
        function onmousedown(e) {
            e.preventDefault();

            l.style.pointerEvents = 'none';
            r.style.pointerEvents = 'none';

            document.addEventListener("mousemove", onmousemove);
            document.addEventListener("mouseup", onmouseup);
        }
        function onmousemove(e) {
            e.preventDefault();

            const clientX = e.clientX;
            const deltaX = clientX - (domDivider._clientX || clientX);
            domDivider._clientX = clientX;

            // LEFT
            if (deltaX < 0) {
                let w = Math.round(parseInt(getComputedStyle(l).width) + deltaX);
                if (w < 10) {
                    w = 0;
                } else {
                    w = (w / domDivider.parentNode.clientWidth * 100).toFixed(9);
                }

                l.style.flex = `0 ${w}%`;
                r.style.flex = "1 0";
            }

            // RIGHT
            if (deltaX > 0) {
                let w = Math.round(parseInt(getComputedStyle(r).width) - deltaX);
                if (w < 10) {
                    w = 0;
                } else {
                    w = (w / domDivider.parentNode.clientWidth * 100).toFixed(9);
                }

                r.style.flex = `0 ${w}%`;
                l.style.flex = "1 0";
            }
        }

        function onmouseup(e) {
            e.preventDefault();

            l.style.removeProperty('pointer-events');
            r.style.removeProperty('pointer-events');

            document.removeEventListener("mousemove", onmousemove);
            document.removeEventListener("mouseup", onmouseup);

            delete domDivider._clientX;

            if (doneCall) {
                doneCall()
            }
        }
    },

    /**
     * 垂直滑动
     * @param {*} domDivider 滑动节点
     * @param {*} doneCall 变更回调
     */
    sideVertical: (domDivider, doneCall) => {
        domDivider.addEventListener("mousedown", onmousedown);
        domDivider.addEventListener("touchstart", ontouchstart);

        const t = domDivider.previousElementSibling;
        const b = domDivider.nextElementSibling;

        // for mobile
        function ontouchstart(e) {
            e.preventDefault();

            t.style.pointerEvents = 'none';
            b.style.pointerEvents = 'none';

            domDivider.addEventListener("touchmove", ontouchmove);
            domDivider.addEventListener("touchend", ontouchend);
        }
        function ontouchmove(e) {
            e.preventDefault();

            const clientY = e.touches[0].clientY;
            const deltaY = clientY - (domDivider._clientY || clientY);
            domDivider._clientY = clientY;

            // UP
            if (deltaY < 0) {
                let h = t.clientHeight + deltaY;
                if (h < 10) {
                    h = 0;
                } else {
                    h = (h / domDivider.parentNode.clientHeight * 100).toFixed(9);
                }

                t.style.flex = `0 ${h}%`;
                b.style.flex = "1 0";
            }

            // DOWN
            if (deltaY > 0) {
                let h = b.clientHeight - deltaY;
                if (h < 10) {
                    h = 0;
                } else {
                    h = (h / domDivider.parentNode.clientHeight * 100).toFixed(9);
                }

                b.style.flex = `0 ${h}%`;
                t.style.flex = "1 0";
            }
        }

        function ontouchend(e) {
            e.preventDefault();

            t.style.removeProperty('pointer-events');
            b.style.removeProperty('pointer-events');

            domDivider.removeEventListener("touchmove", ontouchmove);
            domDivider.removeEventListener("touchend", ontouchend);

            delete domDivider._clientY;

            if (doneCall) {
                doneCall()
            }
        }

        // for desktop
        function onmousedown(e) {
            e.preventDefault();

            t.style.pointerEvents = 'none';
            b.style.pointerEvents = 'none';

            document.addEventListener("mousemove", onmousemove);
            document.addEventListener("mouseup", onmouseup);
        }
        function onmousemove(e) {
            e.preventDefault();

            const clientY = e.clientY;
            const deltaY = clientY - (domDivider._clientY || clientY);
            domDivider._clientY = clientY;

            // UP
            if (deltaY < 0) {
                let h = t.clientHeight + deltaY;
                if (h < 10) {
                    h = 0;
                } else {
                    h = (h / domDivider.parentNode.clientHeight * 100).toFixed(9);
                }

                t.style.flex = `0 ${h}%`;
                b.style.flex = "1 0";
            }

            // DOWN
            if (deltaY > 0) {
                let h = b.clientHeight - deltaY;
                if (h < 10) {
                    h = 0;
                } else {
                    h = (h / domDivider.parentNode.clientHeight * 100).toFixed(9);
                }

                b.style.flex = `0 ${h}%`;
                t.style.flex = "1 0";
            }
        }
        function onmouseup(e) {
            e.preventDefault();

            t.style.removeProperty('pointer-events');
            b.style.removeProperty('pointer-events');

            document.removeEventListener("mousemove", onmousemove);
            document.removeEventListener("mouseup", onmouseup);

            delete domDivider._clientY;

            if (doneCall) {
                doneCall()
            }
        }
    }
}

Object.assign(window, { nrcSplit });
export { nrcSplit };