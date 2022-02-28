var seal = {
    imgs: [],
    init: function () {

        //接收文件
        ss.receiveFiles(function (files) {
            for (var j = 0; j < files.length; j++) {
                var file = files[j]
                if (file.type.indexOf("image") == 0) {
                    seal.imgs.push({
                        fileOrigin: file,
                        fileOutput: null
                    })
                }
            }
            seal.draw();
            seal.showImgs();
        }, "#txtFile");

        seal.cogAdd();

        $('input,textarea').each(function () {
            if (this.placeholder != "") {
                this.title = this.placeholder;
            }
        });

        $(window).dblclick(function () {
            seal.draw();
        });
    },
    cogAdd: function (cog) {
        var nrc = $('.nrCogs'), newCog = nrc.children().first().clone().removeClass('d-none');
        newCog.find('.nrCogImage').addClass('d-none');
        newCog.find('.nrCogText').removeClass('d-none');
        if (cog) {
            cog.after(newCog);
        } else {
            nrc.append(newCog);
        }
    },
    changeCogType: function (that) {
        var cog = $(that).parent().parent(), ov = that.getAttribute('data-ov');;
        switch (that.value) {
            case "add":
                seal.cogAdd(cog);
                that.value = ov;
                seal.very();
                break;
            case "del":
                if (cog.parent().children().length == 2) {
                    seal.cogAdd();
                }
                cog.remove();
                seal.very();
                break;
            case "text":
                cog.find('.nrCogText').removeClass('d-none');
                cog.find('.nrCogImage').addClass('d-none');
                that.setAttribute('data-ov', that.value);
                break;
            case "image":
                cog.find('.nrCogText').addClass('d-none');
                cog.find('.nrCogImage').removeClass('d-none');
                that.setAttribute('data-ov', that.value);
                break;
        }
    },
    changeCogLogo: function (that) {
        var file = that.files[0];
        if (file) {
            if (file.type.indexOf("image") == 0) {
                var img = new Image();
                img.onload = function () {
                    var cwh = $(that).parent().parent();
                    cwh.find('.nrCogLogoWidth').val(this.width);
                    cwh.find('.nrCogLogoHeight').val(this.height);

                    seal.very();
                }
                img.src = URL.createObjectURL(file);
            } else {
                that.value = "";
            }
        }
    },
    very: function () {
        clearTimeout(seal.si1);
        seal.si1 = setTimeout(function () {
            seal.draw();
        }, 50)
    },
    getCogs: function () {
        var arr = [];
        $('.nrCogs').children().each(function (i, cog) {
            if (i) {
                cog = $(cog);
                var obj = {
                    type: cog.find('.nrCogType').val(),
                    content: cog.find('.nrCogContent').val(),
                    fontSize: cog.find('.nrCogFontSize').val(),
                    color: cog.find('.nrCogColor').val(),
                    logo: cog.find('.nrCogLogo')[0].files[0],
                    logoWidth: cog.find('.nrCogLogoWidth').val() * 1,
                    logoHeight: cog.find('.nrCogLogoHeight').val() * 1,
                    x: cog.find('.nrCogX').val() * 1,
                    y: cog.find('.nrCogY').val() * 1,
                    alpha: cog.find('.nrCogAlpha').val() * 1,
                    angle: cog.find('.nrCogAngle').val() * 1
                }
                arr.push(obj)
            }
        })
        return arr;
    },
    draw: function () {
        var cogs = seal.getCogs();
        for (var i = 0; i < seal.imgs.length; i++) {
            var fi = seal.imgs[i];
            seal.drawDeep(cogs, fi.fileOrigin, null, i, 0);
        }
    },
    drawDeep: function (cogs, fileOrigin, fileOutput, i, deep, fn) {
        deep = deep || 0;
        if (deep < cogs.length) {
            var cog = cogs[deep++];

            switch (cog.type) {
                case 'text':
                    watermark([fileOutput || fileOrigin]).image(function (target) {
                        var context = target.getContext('2d');
                        context.globalAlpha = cog.alpha;
                        context.fillStyle = cog.color;
                        context.font = cog.fontSize;
                        metrics = context.measureText(cog.content);
                        context.rotate(-1 * cog.angle * Math.PI / 180)
                        context.fillText(cog.content, cog.x, cog.y + metrics.actualBoundingBoxAscent);
                        return target;
                    }).then(function (outimg) {
                        fileOutput = seal.base64ToFile(outimg.src, fileOrigin.name, fileOrigin.type);
                        seal.drawDeep(cogs, fileOrigin, fileOutput, i, deep, fn)
                    });
                    break;
                case 'image':
                    watermark([fileOutput || fileOrigin, cog.logo || '/favicon.ico']).image(function (target, logo) {
                        var context = target.getContext('2d')
                        context.globalAlpha = cog.alpha;
                        context.rotate(-1 * cog.angle * Math.PI / 180);
                        context.drawImage(logo, cog.x, cog.y, cog.logoWidth, cog.logoHeight)
                        return target
                    }).then(function (outimg) {
                        fileOutput = seal.base64ToFile(outimg.src, fileOrigin.name, fileOrigin.type);
                        seal.drawDeep(cogs, fileOrigin, fileOutput, i, deep, fn)
                    });
                    break;
            }
        } else {
            seal.imgs[i].fileOutput = fileOutput;
            seal.showImgs(i);
            typeof fn == "function" && fn(fileOutput, fileOrigin);
        }
    },
    showImgs: function (si) {
        if (si == null) {
            $('.nrView').empty();
            seal.imgs.forEach(fi => {
                var img = new Image();
                img.className = "mw-100 border";
                img.style.maxHeight = "300px";
                if (fi.fileOutput) {
                    img.src = URL.createObjectURL(fi.fileOutput);
                }
                var col = $('<div class="col-auto mb-3 text-center nrOutput"></div>').append(img);
                $('.nrView').append(col);
            })
        } else {
            $('.nrView').children().eq(si).find('img').attr('src', URL.createObjectURL(seal.imgs[si].fileOutput));
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

seal.init();