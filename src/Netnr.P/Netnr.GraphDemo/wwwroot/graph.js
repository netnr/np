var config = [
    {
        name: "System.Drawing.Common",
        link: null,
        captcha: "/SystemDrawingCommon/Captcha",
        resize: "/SystemDrawingCommon/Resize",
        watermarkForText: "/SystemDrawingCommon/Watermark"
    }, {
        name: "SkiaSharp",
        link: "https://github.com/mono/SkiaSharp",
        captcha: "/SkiaSharp/Captcha",
        resize: "/SkiaSharp/Resize",
        watermarkForText: "/SkiaSharp/Watermark"
    },
    {
        name: "SixLabors.ImageSharp.Drawing",
        link: "https://github.com/SixLabors/ImageSharp",
        captcha: "/SixLaborsImageSharpDrawing/Captcha",
        resize: null
    }
];

var box = document.querySelector('.nr-graph');

function initCaptcha() {
    var htm = [];

    config.forEach(item => {

        //请求
        var uri = item.captcha;
        if (uri) {
            if (item.link) {
                htm.push(`<td><a href="${item.link}" target="_blank"><p>${item.name}</p></a><img data-src="${uri}" /><p></p></td>`)
            } else {
                htm.push(`<td><p>${item.name}</p><img data-src="${uri}" /><p></p></td>`)
            }

            var xhr = new XMLHttpRequest();
            xhr.open("get", uri, true);
            xhr.responseType = "blob";
            xhr.onload = function () {
                var img = document.querySelector('img[data-src="' + uri + '"]');

                if (this.status == 200) {
                    var blob = this.response;

                    var imgNode = new Image();
                    imgNode.onload = function () {

                        img.src = URL.createObjectURL(blob);

                        xhr.getAllResponseHeaders().replace(/content-length: (.*)/, function () {
                            img.nextElementSibling.innerHTML = `Size: ${imgNode.width}*${imgNode.height} ，${arguments[1]} Bytes`;
                        })
                    };
                    imgNode.src = URL.createObjectURL(blob);
                } else {
                    img.nextElementSibling.innerHTML = "exception"
                }
            }
            xhr.send();
        }
    });

    var table = document.createElement("table");
    table.innerHTML = `<table><tr>${htm.join('')}</tr></table>`;
    box.appendChild(table);
}

function initResize() {
    var htm = [];

    htm.push(`<td><p>原图</p><img src="/bird.jpg"/><p></p></td>`);

    config.forEach(item => {

        //请求
        var uri = item.resize;
        if (uri) {
            if (item.link) {
                htm.push(`<td><a href="${item.link}" target="_blank"><p>${item.name}</p></a><img data-src="${uri}" src="/bird.jpg" /><p></p></td>`)
            } else {
                htm.push(`<td><p>${item.name}</p><img data-src="${uri}" src="/bird.jpg" /><p></p></td>`)
            }

            var xhr = new XMLHttpRequest();
            xhr.open("get", uri, true);
            xhr.responseType = "blob";
            xhr.onload = function () {
                var img = document.querySelector('img[data-src="' + uri + '"]');

                if (this.status == 200) {
                    var blob = this.response;

                    var imgNode = new Image();
                    imgNode.onload = function () {

                        img.src = URL.createObjectURL(blob);

                        xhr.getAllResponseHeaders().replace(/content-length: (.*)/, function () {
                            img.nextElementSibling.innerHTML = `Size: ${imgNode.width}*${imgNode.height} ，${arguments[1]} Bytes`;
                        })
                    };
                    imgNode.src = URL.createObjectURL(blob);
                } else {
                    img.nextElementSibling.innerHTML = "exception"
                }
            }
            xhr.send();
        }
    });

    var table = document.createElement("table");
    table.innerHTML = `<table><tr>${htm.join('')}</tr></table>`;
    box.appendChild(table);

    //原图
    if (true) {
        var img = document.querySelector('img');

        var xhr = new XMLHttpRequest();
        xhr.open("get", img.src, true);
        xhr.responseType = "blob";
        xhr.onload = function () {
            if (this.status == 200) {
                var blob = this.response;

                var imgNode = new Image();
                imgNode.onload = function () {
                    xhr.getAllResponseHeaders().replace(/content-length: (.*)/, function () {
                        img.nextElementSibling.innerHTML = `Size: ${imgNode.width}*${imgNode.height} ，${arguments[1]} Bytes`;
                    })
                };
                imgNode.src = URL.createObjectURL(blob);
            }
        }
        xhr.send();
    }
}

function initWatermark() {
    var htm = [];

    config.forEach(item => {

        //请求
        var uri = item.watermarkForText;
        if (uri) {
            if (item.link) {
                htm.push(`<td><a href="${item.link}" target="_blank"><p>${item.name}</p></a><img data-src="${uri}" /><p></p></td>`)
            } else {
                htm.push(`<td><p>${item.name}</p><img data-src="${uri}" /><p></p></td>`)
            }

            var xhr = new XMLHttpRequest();
            xhr.open("get", uri, true);
            xhr.responseType = "blob";
            xhr.onload = function () {
                var img = document.querySelector('img[data-src="' + uri + '"]');

                if (this.status == 200) {
                    var blob = this.response;

                    var imgNode = new Image();
                    imgNode.onload = function () {
                        img.src = URL.createObjectURL(blob);

                        xhr.getAllResponseHeaders().replace(/content-length: (.*)/, function () {
                            img.nextElementSibling.innerHTML = `Size: ${imgNode.width}*${imgNode.height} ，${arguments[1]} Bytes`;
                        })
                    };
                    imgNode.src = URL.createObjectURL(blob);
                } else {
                    img.nextElementSibling.innerHTML = "exception"
                }
            }
            xhr.send();
        }
    });

    var table = document.createElement("table");
    table.innerHTML = `<table><tr>${htm.join('')}</tr></table>`;
    box.appendChild(table);
}

(function () {
    if (location.pathname.toLowerCase().includes("captcha")) {
        initCaptcha()
    }
    if (location.pathname.toLowerCase().includes("resize")) {
        initResize()
    }
    if (location.pathname.toLowerCase().includes("watermark")) {
        initWatermark()
    }
})();