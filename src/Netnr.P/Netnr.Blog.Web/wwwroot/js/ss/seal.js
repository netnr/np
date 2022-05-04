nr.onReady = function () {
    //接收文件
    nr.receiveFiles(function (files) {
        for (var j = 0; j < files.length; j++) {
            var file = files[j]
            if (file.type.indexOf("image") == 0) {
                page.imgs.push({
                    fileOrigin: file,
                    fileOutput: null
                })
            }
        }
        page.draw();
        page.showImgs();
    }, nr.domTxtFile);

    //类型切换
    nr.domSeType.addEventListener('sl-change', function () {
        if (this.value == "text") {
            nr.domCogText.classList.remove('d-none');
            nr.domCogImage.classList.add('d-none');
        } else {
            nr.domCogText.classList.add('d-none');
            nr.domCogImage.classList.remove('d-none');
        }
        page.very();
    });

    //变动
    nr.domCogText.querySelectorAll('sl-input').forEach(dom => {
        dom.addEventListener('input', function () {
            page.very();
        });
    });
    nr.domTxtColor.addEventListener('sl-change', function () {
        page.very();
    });
    nr.domCogImage.querySelectorAll('sl-input').forEach(dom => {
        dom.addEventListener('input', function () {
            page.very();
        });
    });

    //logo 选择
    nr.domTxtLogo.addEventListener('change', function () {
        var file = this.files[0];
        if (file) {
            if (file.type.indexOf("image") == 0) {
                var img = new Image();
                img.onload = function () {
                    page.very();
                }
                img.src = URL.createObjectURL(file);
            } else {
                that.value = "";
            }
        }
    });
}

var page = {
    imgs: [],
    very: function () {
        clearTimeout(page.si1);
        page.si1 = setTimeout(function () {
            page.draw();
        }, 50)
    },
    getCog: function () {
        var obj = { type: nr.domSeType.value, }
        if (nr.domSeType.value == "text") {
            Object.assign(obj, {
                content: nr.domTxtContent.value,
                fontSize: nr.domTxtFontSize.value,
                color: nr.domTxtColor.value,
                x: nr.domCogText.querySelector('.nr-txt-x').value * 1,
                y: nr.domCogText.querySelector('.nr-txt-y').value * 1,
                alpha: nr.domCogText.querySelector('.nr-txt-alpha').value * 1,
                angle: nr.domCogText.querySelector('.nr-txt-angle').value * 1,
            });
        } else {
            Object.assign(obj, {
                logo: nr.domTxtLogo.files[0],
                logoWidth: nr.domTxtLogoWidth.value * 1,
                logoHeight: nr.domTxtLogoHeight.value * 1,
                x: nr.domCogImage.querySelector('.nr-txt-x').value * 1,
                y: nr.domCogImage.querySelector('.nr-txt-y').value * 1,
                alpha: nr.domCogImage.querySelector('.nr-txt-alpha').value * 1,
                angle: nr.domCogImage.querySelector('.nr-txt-angle').value * 1,
            });
        }
        return obj;
    },
    draw: function () {
        let cog = page.getCog();
        for (let i = 0; i < page.imgs.length; i++) {
            let fi = page.imgs[i];
            page.drawDeep(cog, fi.fileOrigin, i);
        }
    },
    drawDeep: function (cog, fileOrigin, i) {
        switch (cog.type) {
            case 'text':
                watermark([fileOrigin]).image(target => page.rotateText(target, cog)).then((outimg) => {
                    page.imgs[i].fileOutput = page.base64ToFile(outimg.src, fileOrigin.name, fileOrigin.type);
                    page.showImgs(i);
                });
                break;
            case 'image':
                watermark([fileOrigin, cog.logo || '/favicon.ico']).image(function (target, logo) {
                    return page.rotateImage(target, logo, cog);
                }).then(function (outimg) {
                    page.imgs[i].fileOutput = page.base64ToFile(outimg.src, fileOrigin.name, fileOrigin.type);
                    page.showImgs(i);
                });
                break;
        }
    },
    rotateText: function (target, cog) {
        var context = target.getContext('2d');
        context.translate(target.width * cog.x / 100, target.height * cog.y / 100);
        context.globalAlpha = cog.alpha;
        context.fillStyle = cog.color;
        context.font = cog.fontSize || '48px Josefin Slab';
        context.rotate(-1 * cog.angle * Math.PI / 180);
        context.fillText(cog.content, 0, 0);
        return target;
    },
    rotateImage: function (target, logo, cog) {
        var context = target.getContext('2d');
        context.translate(target.width * cog.x / 100, target.height * cog.y / 100);
        context.globalAlpha = cog.alpha;
        context.rotate(-1 * cog.angle * Math.PI / 180);
        context.drawImage(logo, cog.x, cog.y, logo.width * cog.logoWidth / 100, logo.height * cog.logoHeight / 100)
        return target;
    },
    showImgs: function (si) {
        if (si == null) {
            nr.domCardResult.innerHTML = "";
            page.imgs.forEach(fi => {
                var img = new Image();
                img.className = "mw-100 border rounded";
                if (fi.fileOutput) {
                    img.src = URL.createObjectURL(fi.fileOutput);
                }

                var domCol = document.createElement("div");
                domCol.className = "col-12 mt-4 text-center";
                domCol.appendChild(img);
                nr.domCardResult.appendChild(domCol);
            })
        } else {
            nr.domCardResult.children[si].querySelector('img').src = URL.createObjectURL(page.imgs[si].fileOutput);
        }
    },
    base64ToFile: function (b64, name, type) {
        var arr = b64.split(','), mime = arr[0].match(/:(.*?);/)[1],
            bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
        while (n--) {
            u8arr[n] = bstr.charCodeAt(n);
        }
        var blob = new Blob([u8arr], { type: mime });
        var file = new File([blob], name, { type: type });
        return file;
    }
}