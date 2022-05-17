nr.onReady = function () {
    page.init()
}

var page = {
    files: [],
    init: function () {

        nr.domTxtOr.value = nr.lsStr('or');
        nr.domTxtToken.value = nr.lsStr('token');

        nr.domTxtOr.addEventListener('input', function () {
            nr.ls['or'] = this.value;
            nr.lsSave();
        });
        nr.domTxtToken.addEventListener('input', function () {
            if (this.value == "" || this.value.length == 40) {
                nr.ls['token'] = this.value;
                nr.lsSave();
            }
        });

        //接收文件
        nr.receiveFiles(function (files) {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                page.add(file);
            }
            nr.domTxtFile.value = "";
        }, nr.domTxtFile);

        //上传
        nr.domBtnUpload.addEventListener('click', function () {
            if (page.uploading) {
                nr.alert('Uploading ...');
            } else if (page.files.length == 0) {
                nr.alert('Please select or drag and drop files');
            } else {
                page.uploading = true;
                page.fi = 0;

                var next = function () {
                    if (page.fi < page.files.length) {
                        var fobj = page.files[page.fi++];
                        page.upload(fobj).then(() => next()).catch(() => next());
                    } else {
                        page.uploading = false;
                    }
                }
                next();
            }
        }, false);
    },
    setStatus: function (fobj, status) {
        fobj.status = status;

        switch (status) {
            case "ok":
                fobj.node.label = "访问路径"
                break;
            case "ing":
                fobj.node.label = "上传中..."
                break;
            case "fail":
                fobj.node.label = "上传失败"
                break;
        }
    },
    pad: function (s, len) {
        return (s + '').padStart(len || 2, '0');
    },
    getFileName: function (file) {
        var now = new Date();
        var ext = file.split('.').pop();
        var filename = `${now.getFullYear()}/${page.pad(now.getMonth() + 1)}${page.pad(now.getDate())}${page.pad(now.getHours())}${page.pad(now.getMinutes())}${page.pad(now.getSeconds())}${page.pad(now.getMilliseconds(), 3)}.${ext}`
        return filename;
    },
    add: function (file) {
        return new Promise(function (resolve) {
            var r = new FileReader();
            r.onload = function (e) {

                var cover = e.target.result;
                if (!cover.includes(":image")) {
                    cover = '/favicon.svg';
                }

                var domCol = document.createElement('div');
                domCol.classList.add('col-xxl-4', 'col-xl-6', 'mt-4');
                domCol.innerHTML = `<div class="d-flex">
                    <img src='${cover}' class="rounded me-3" style="width:4.8em;height:4.8em" />
                    <sl-input class="w-100" size="large" label="path（存储路径）" placeholder="yyyy/MMddxx.ext" value="${page.getFileName(file.name)}"></sl-input>
                </div>`;
                nr.domCardList.appendChild(domCol);

                page.files.push({
                    file: file,
                    node: domCol.querySelector('sl-input'),
                    base64: e.target.result.split(',').pop(),
                    status: 'ready'
                })

                resolve();
            }
            r.readAsDataURL(file);
        });
    },
    upload: function (fobj) {
        return new Promise(function (resolve, reject) {
            if (fobj.status == "ok") {
                resolve();
            } else {
                var or = nr.domTxtOr.value.trim(),
                    token = nr.domTxtToken.value.trim(),
                    path = fobj.node.value.trim();

                if (or.length > 2 && token.length == 40 && path != "") {
                    page.setStatus(fobj, 'ing');

                    var url = `https://api.github.com/repos/${or}/contents/${path}`;
                    var proxy = nr.domSeProxy.value;
                    if (proxy != "") {
                        url = proxy + encodeURIComponent(url);
                    }

                    fetch(url, {
                        method: 'PUT',
                        headers: {
                            Accept: 'application/vnd.github.v3+json',
                            Authorization: 'token ' + token,
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            message: 'm',
                            content: fobj.base64
                        })
                    }).then(resp => resp.json()).then(res => {
                        console.log(res);

                        if (res.message) {
                            nr.alert(res.message);
                            page.setStatus(fobj, 'fail');
                            reject();
                        } else {
                            fobj.result = res;
                            page.setStatus(fobj, 'ok');

                            var key_github = res.content.download_url;

                            fobj.node.readonly = true;
                            fobj.node.filled = true;
                            fobj.node.value = key_github;

                            resolve();
                        }
                    }).catch(ex => {
                        console.debug(ex);
                        page.setStatus(fobj, 'fail');
                        nr.alert(ex + "");
                        reject();
                    })
                } else {
                    nr.alert('Check : owner repo token path');
                    reject();
                }
            }
        })
    }
}