nr.onReady = function () {

    //接收文件
    nr.receiveFiles(function (files) {
        page.addFiles(files);
    }, nr.domTxtFile);

    //调整配置
    nr.domTxtWidth.addEventListener('input', function () {
        page.options.width = parseInt(this.value);
    });
    nr.domTxtHeight.addEventListener('input', function () {
        page.options.height = parseInt(this.value);
    });
    nr.domTxtQuality.addEventListener('input', function () {
        page.options.quality = parseFloat(this.value);
    });

    document.addEventListener('click', function (e) {
        var target = e.target, action = target.getAttribute('data-action');
        switch (action) {
            case 'delete':
                {
                    var val = target.getAttribute('data-value');
                    for (let index = 0; index < page.result.length; index++) {
                        var item = page.result[index];
                        if (item.aid == val) {
                            page.result.splice(index, 1);
                            break;
                        }
                    }
                    page.viewZIP();
                    document.getElementById(val).remove();
                }
                break;
            case 'download-zip':
                {
                    var zip = new JSZip();
                    page.result.forEach(item => {
                        zip.file(item.fullPath, item.blob);
                    })
                    zip.generateAsync({ type: "blob" }).then(function (content) {
                        nr.download(content, "images.zip");
                    });
                }
                break;
        }
    });
}

var page = {
    //压缩配置
    options: {
        width: null,
        height: null,
        quality: 0.7
    },
    result: [],

    //添加文件
    addFiles: function (files) {
        for (var i = 0; i < files.length; i++) {
            let file = files[i];
            if (file.type.startsWith('image/')) {
                page.compressToBlob(file).then(blob => {
                    if (nr.domIsSkip.checked && blob.size >= file.size) {
                        nr.alert(`${file.name} 压缩失败`);
                    }
                    else {
                        var fullPath = file.fullPath || file.webkitRelativePath;
                        if (fullPath == "") {
                            fullPath = file.name;
                        }
                        var ahref = URL.createObjectURL(blob);
                        var aid = ahref.split('/').pop();
                        var reduce = (-1 * (file.size - blob.size) / file.size * 100).toFixed(2) + "%";
                        page.result.push({
                            aid, fullPath, reduce, name: file.name, blob,
                            fileSize: file.size, blobSize: blob.size
                        });
                        page.viewZIP();

                        var domItem = document.createElement('div');
                        domItem.classList.add('col-xxl-3', 'col-xl-4', 'col-md-6', 'mt-4', 'd-flex');
                        domItem.id = aid;
                        domItem.innerHTML = `
                        <div style="width: 6em;height:4.5em;line-height:4.5em;" class="me-3 text-center">
                            <img src="${URL.createObjectURL(file)}" class="mw-100 mh-100 rounded">
                        </div>
                        <div>
                            <span class="d-inline-block text-truncate" style="max-width: 13em;">${file.name}</span>
                            <div>
                                <span class="me-2">${nr.formatByteSize(file.size)}</span>
                                <b class="me-2">${nr.formatByteSize(blob.size)}</b>
                                <b class="me-2">${reduce}</b>
                            </div>
                            <div>
                                <a class="me-2" href="${ahref}" target="_blank">查看</a>
                                <a class="me-2" href="${ahref}" target="_blank" download="${file.name}">下载</a>
                                <span class="text-danger" data-value="${aid}" data-action="delete" role="button">删除</span>
                            </div>
                        </div>`;
                        nr.domCardResult.appendChild(domItem);
                    }
                })
            }
        }
        nr.domTxtFile.value = "";
    },

    viewZIP: function () {
        if (page.result.length == 0) {
            nr.domCardZip.innerHTML = "";
        } else {
            var fileSize = 0, blobSize = 0;
            page.result.forEach(item => {
                fileSize += item.fileSize;
                blobSize += item.blobSize;
            })
            var reduce = (-1 * (fileSize - blobSize) / fileSize * 100).toFixed(2) + "%";
            nr.domCardZip.innerHTML = `<span class="me-2 text-primary" role="button" data-action="download-zip">下载 images.zip</span>
            <span class="me-2">${nr.formatByteSize(fileSize)}</span>
            <b class="me-2">${nr.formatByteSize(blobSize)}</b>
            <b class="me-2">${reduce}</b>
            `;
        }
    },

    compressToBlob: function (file) {
        return new Promise(function (resolve) {
            var img = new Image();
            img.onload = function () {
                var canvas = page.imageToCanvas(img, page.options.width, page.options.height);
                var base64 = canvas.toDataURL(file.type, page.options.quality);

                var bin = atob(base64.split(',').pop());
                var len = bin.length;
                var arr = new Uint8Array(len);
                for (var i = 0; i < len; i++) {
                    arr[i] = bin.charCodeAt(i);
                }
                var blob = new Blob([arr], { type: file.type });
                resolve(blob);
            }
            img.src = URL.createObjectURL(file);
        });
    },
    imageToCanvas(img, maxWidth, maxHeight) {
        var canvas = document.createElement('canvas');
        var width = img.width;
        var height = img.height;

        if (maxWidth == null || maxWidth == "") {
            maxWidth = width;
        }
        if (maxHeight == null || maxHeight == "") {
            maxHeight = height;
        }

        if (width > height) {
            if (width > maxWidth) {
                height = Math.round(height *= maxWidth / width);
                width = maxWidth;
            }
        } else {
            if (height > maxHeight) {
                width = Math.round(width *= maxHeight / height);
                height = maxHeight;
            }
        }

        canvas.width = width;
        canvas.height = height;
        var ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0, width, height);

        return canvas;
    },

    checkSafeName: function (filename) {
        var newfilename = filename, num = 1, index = filename.lastIndexOf("."),
            name = filename.substr(0, index), ext = filename.substr(index);
        while (newfilename in page.zip.files) {
            newfilename = name + "(" + (num++) + ")" + ext;
        }
        return newfilename;
    },
}