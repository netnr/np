// Source: https://github.com/FrankFang/canvas-demo-2017

let st = document.createElement("STYLE");
st.innerHTML = `ol,ul{list-style:none}*{margin:0;padding:0}.icon{width:1em;height:1em;vertical-align:-.15em;fill:currentColor;overflow:hidden}#xxx{display:block;background:#fff;position:fixed;top:0;left:0}body{overflow:hidden}.actions{position:fixed;top:0;left:0;padding:20px}.actions svg{width:1.5em;height:1.5em;transition:all .3s;margin:0 10px}.actions svg.active{fill:red;transform:scale(1.2)}.colors{position:fixed;top:60px;left:28px}.colors>li{width:20px;height:20px;border-radius:50%;box-shadow:0 0 3px rgba(0,0,0,.25);margin:10px 0;transition:all .3s}.colors>li.red{background:red}.colors>li.green{background:green}.colors>li.blue{background:#00f}.colors>li.active{box-shadow:0 0 3px rgba(0,0,0,.95);transform:scale(1.2)}.sizes{position:fixed;right:20px;top:10px}.sizes>li{margin:20px 0}.sizes>.thin{height:0;width:20px;border-top:3px solid #000}.sizes>.thick{height:0;width:20px;border-top:6px solid #000}`;
document.head.appendChild(st);

var yyy = document.getElementById('xxx');
var context = yyy.getContext('2d');
var lineWidth = 5

autoSetCanvasSize(yyy)
listenToUser(yyy)

var eraserEnabled = false
pen.onclick = function () {
    eraserEnabled = false
    pen.classList.add('active')
    eraser.classList.remove('active')
}
eraser.onclick = function () {
    eraserEnabled = true
    eraser.classList.add('active')
    pen.classList.remove('active')
}
clear.onclick = function () {
    context.clearRect(0, 0, yyy.width, yyy.height);
}
download.onclick = function () {
    var url = yyy.toDataURL("image/png")
    var a = document.createElement('a')
    document.body.appendChild(a)
    a.href = url
    a.download = 'brush.png'
    a.target = '_blank'
    a.click()
}


red.onclick = function () {
    context.fillStyle = 'red'
    context.strokeStyle = 'red'
    red.classList.add('active')
    green.classList.remove('active')
    blue.classList.remove('active')
}
green.onclick = function () {
    context.fillStyle = 'green'
    context.strokeStyle = 'green'
    red.classList.remove('active')
    green.classList.add('active')
    blue.classList.remove('active')
}
blue.onclick = function () {
    context.fillStyle = 'blue'
    context.strokeStyle = 'blue'
    red.classList.remove('active')
    green.classList.remove('active')
    blue.classList.add('active')
}

thin.onclick = function () {
    lineWidth = 5
}
thick.onclick = function () {
    lineWidth = 10
}

function autoSetCanvasSize(canvas) {
    setCanvasSize()

    window.onresize = function () {
        setCanvasSize()
    }

    function setCanvasSize() {
        var pageWidth = document.documentElement.clientWidth
        var pageHeight = document.documentElement.clientHeight

        canvas.width = pageWidth
        canvas.height = pageHeight
    }
}

function drawCircle(x, y, radius) {
    context.beginPath()
    context.arc(x, y, radius, 0, Math.PI * 2);
    context.fill()
}

function drawLine(x1, y1, x2, y2) {
    context.beginPath();
    context.moveTo(x1, y1) // 起点
    context.lineWidth = lineWidth
    context.lineTo(x2, y2) // 终点
    context.stroke()
    context.closePath()
}

function listenToUser(canvas) {
    var using = false
    var lastPoint = {
        x: undefined,
        y: undefined
    }
    // 特性检测
    if (document.body.ontouchstart !== undefined) {
        // 触屏设备
        canvas.ontouchstart = function (aaa) {
            var x = aaa.touches[0].clientX
            var y = aaa.touches[0].clientY
            
            using = true
            if (eraserEnabled) {
                context.clearRect(x - 5, y - 5, 10, 10)
            } else {
                lastPoint = {
                    "x": x,
                    "y": y
                }
            }
        }
        canvas.ontouchmove = function (aaa) {
            var x = aaa.touches[0].clientX
            var y = aaa.touches[0].clientY

            if (!using) { return }

            if (eraserEnabled) {
                context.clearRect(x - 5, y - 5, 10, 10)
            } else {
                var newPoint = {
                    "x": x,
                    "y": y
                }
                drawLine(lastPoint.x, lastPoint.y, newPoint.x, newPoint.y)
                lastPoint = newPoint
            }
        }
        canvas.ontouchend = function () {
            using = false
        }
    } else {
        // 非触屏设备
        canvas.onmousedown = function (aaa) {
            var x = aaa.clientX
            var y = aaa.clientY
            using = true
            if (eraserEnabled) {
                context.clearRect(x - 5, y - 5, 10, 10)
            } else {
                lastPoint = {
                    "x": x,
                    "y": y
                }
            }
        }
        canvas.onmousemove = function (aaa) {
            var x = aaa.clientX
            var y = aaa.clientY

            if (!using) { return }

            if (eraserEnabled) {
                context.clearRect(x - 5, y - 5, 10, 10)
            } else {
                var newPoint = {
                    "x": x,
                    "y": y
                }
                drawLine(lastPoint.x, lastPoint.y, newPoint.x, newPoint.y)
                lastPoint = newPoint
            }

        }
        canvas.onmouseup = function (aaa) {
            using = false
        }
    }
}