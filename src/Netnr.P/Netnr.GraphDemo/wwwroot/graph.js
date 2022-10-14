var config = [
    {
        name: "SkiaSharp",
        link: "https://github.com/mono/SkiaSharp",
        captcha: "/SkiaSharp/Captcha",
        resize: "/SkiaSharp/Resize",
        watermarkForText: "/SkiaSharp/Watermark"
    },
    {
        name: "NetVips",
        link: "https://github.com/kleisauke/net-vips",
        captcha: "/NetVips/Captcha",
    },
    {
        name: "SixLabors.ImageSharp.Drawing",
        link: "https://github.com/SixLabors/ImageSharp",
        captcha: "/SixLaborsImageSharpDrawing/Captcha",
        resize: null
    },
    {
        name: "Magick.NET",
        link: "https://github.com/dlemstra/Magick.NET",
        captcha: "/MagickNET/Captcha",
        resize: null
    },
    {
        name: "System.Drawing.Common",
        captcha: "/SystemDrawingCommon/Captcha",
        resize: "/SystemDrawingCommon/Resize",
        watermarkForText: "/SystemDrawingCommon/Watermark"
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
                htm.push(`<li><a href="${item.link}" target="_blank"><p>${item.name}</p></a><img data-src="${uri}" /><p></p></li>`)
            } else {
                htm.push(`<li><p>${item.name}</p><img data-src="${uri}" /><p></p></li>`)
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

    var table = document.createElement("div");
    table.innerHTML = `<ul style="display:flex;flex-wrap:warp;justify-content:space-around">${htm.join('')}</ul>`;
    box.appendChild(table);
}

function initResize() {
    var htm = [];

    htm.push(`<li><p>原图</p><img src="/netnr_avatar.jpg"/><p></p></li>`);

    config.forEach(item => {

        //请求
        var uri = item.resize;
        if (uri) {
            if (item.link) {
                htm.push(`<li><a href="${item.link}" target="_blank"><p>${item.name}</p></a><img data-src="${uri}" src="/netnr_avatar.jpg" /><p></p></li>`)
            } else {
                htm.push(`<li><p>${item.name}</p><img data-src="${uri}" src="/netnr_avatar.jpg" /><p></p></li>`)
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

    var table = document.createElement("div");
    table.innerHTML = `<ul style="display:flex;flex-wrap:warp;justify-content:space-around">${htm.join('')}</ul>`;
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
                htm.push(`<li><a href="${item.link}" target="_blank"><p>${item.name}</p></a><img data-src="${uri}" /><p></p></li>`)
            } else {
                htm.push(`<li><p>${item.name}</p><img data-src="${uri}" /><p></p></li>`)
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

    var table = document.createElement("div");
    table.innerHTML = `<ul style="display:flex;flex-wrap:warp;justify-content:space-around">${htm.join('')}</ul>`;
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