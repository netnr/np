var fr = {
    build: function (se) {
        var box = document.querySelector(se), si, defer = 500, repairClass = 'flex-repair-item';

        new ResizeObserver(function () {
            //移除修补
            box.querySelectorAll('.' + repairClass).forEach(item => item.remove());

            clearTimeout(si);

            si = setTimeout(function () {
                var childs = box.children, len = childs.length;

                var index = 0, one = childs[0];
                for (var i = 0; i < len; i++) {
                    var node = childs[i];
                    if (node.offsetTop != one.offsetTop) {
                        index = i;
                        break;
                    }
                }
                var repair = index > 1 ? index - len % index : 0;
                if (repair == i) {
                    repair = 0;
                }

                //第一行不足补齐
                if (repair == 0) {
                    var pw = getComputedStyle(box, null).padding.match(/\d+/)[0] * 2;
                    var mw = getComputedStyle(one, null).margin.match(/\d+/)[0] * 2;
                    var rownum = Math.floor((box.parentNode.clientWidth - pw) / (one.offsetWidth + mw));
                    if (len < rownum) {
                        repair = rownum - len;
                    }
                }

                //补齐
                while (repair--) {
                    var nn = one.cloneNode();
                    nn.className = '';
                    nn.classList.add(repairClass);
                    box.appendChild(nn)
                }
            }, defer);
        }).observe(box);
    }
}

fr.build('.nr-list');

var gs = {
    files: [],
    init: function () {

        //缓存 or、token
        document.querySelector('.nr-or').value = ss.lsStr('or');
        document.querySelector('.nr-token').value = ss.lsStr('token');

        document.querySelector('.nr-or').addEventListener('input', function () {
            ss.ls["or"] = this.value;
            ss.lsSave();
        }, false);
        document.querySelector('.nr-token').addEventListener('input', function () {
            if (this.value == "" || this.value.length == 40) {
                ss.ls["token"] = this.value;
                ss.lsSave();
            }
        }, false);

        //拖拽
        "dragleave dragenter dragover drop".split(' ').forEach(en => {
            document.addEventListener(en, function (e) {
                e.stopPropagation();
                e.preventDefault();
            }, false)
        });
        document.addEventListener('drop', function (e) {
            var files = (e.dataTransfer || e.originalEvent.dataTransfer).files;
            if (files && files.length) {
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    gs.add(file);
                }
            }
        }, false);

        //粘贴
        document.addEventListener('paste', function (event) {
            if (event.clipboardData || event.originalEvent) {
                var clipboardData = (event.clipboardData || event.originalEvent.clipboardData);
                if (clipboardData.items) {
                    var items = clipboardData.items, len = items.length;
                    for (var i = 0; i < len; i++) {
                        var blob = items[i].getAsFile();
                        if (blob) {
                            gs.add(blob);
                        }
                    }
                }
            }
        });

        //选择文件
        document.querySelector('.nr-file').addEventListener('change', function () {
            var files = this.files;
            for (var i = 0; i < files.length; i++) {
                gs.add(files[i]);
            }
        }, false);

        //上传
        document.querySelector('.nr-btn-upload').addEventListener('click', function () {
            if (gs.uploading) {
                gs.msg('Uploading ...');
            } else if (gs.files.length == 0) {
                gs.msg('Please select or drag and drop files');
            } else {
                gs.uploading = true;
                gs.fi = 0;

                var next = function () {
                    if (gs.fi < gs.files.length) {
                        var fobj = gs.files[gs.fi++];
                        gs.upload(fobj).then(() => next()).catch(() => next());
                    } else {
                        gs.uploading = false;
                    }
                }
                next();
            }
        }, false);

        //点击
        document.querySelector('.nr-links').addEventListener('click', function (e) {
            var target = e.target, cmd = target.dataset.cmd;
            if (cmd == "copy") {
                navigator.clipboard.writeText(target.previousElementSibling.value);
            } else if (["github", "jsdelivr"].includes(cmd)) {
                var itembox = target.parentElement.parentElement.parentElement;
                var val = target.dataset.value;
                itembox.querySelector('input').value = val;
                itembox.querySelector('[data-cmd="open"]').href = val;
            }
        }, false);
    },
    setStatus: function (fobj, status) {
        fobj.status = status;

        fobj.node.classList.remove('nr-upload-status-ok');
        fobj.node.classList.remove('nr-upload-status-ing');
        fobj.node.classList.remove('nr-upload-status-fail');

        fobj.node.classList.add('nr-upload-status-' + status);
    },
    pad: function (s, len) {
        return (s + '').padStart(len || 2, '0');
    },
    getFileName: function (file) {
        var now = new Date();
        var ext = file.split('.').pop();
        var filename = `${now.getFullYear()}/${gs.pad(now.getMonth() + 1)}/${gs.pad(now.getDate())}/${gs.pad(now.getHours())}${gs.pad(now.getMinutes())}${gs.pad(now.getSeconds())}${gs.pad(now.getMilliseconds(), 3)}.${ext}`
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

                var li = document.createElement('li');
                li.innerHTML = `<img src='${cover}'><input class="nr-path" placeholder="yyyy/MM/dd/xx.ext" value="${gs.getFileName(file.name)}">`;
                document.querySelector('.nr-list').insertBefore(li, document.querySelector('.nr-upload-choose'));

                gs.files.push({
                    file: file,
                    node: li,
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
                var or = document.querySelector('.nr-or').value,
                    token = document.querySelector('.nr-token').value,
                    path = fobj.node.querySelector('.nr-path').value;

                if (or != "" && or.length > 2 && token.length == 40 && path != "") {
                    fobj.node.classList.add('nr-upload-status-ing');
                    var url = `https://api.github.com/repos/${or}/contents/${path}`;
                    var proxy = document.querySelector('.nr-proxy').value;
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
                    }).then(x => x.json()).then(res => {
                        console.log(res);

                        if (res.message) {
                            gs.msg(res.message);
                            gs.setStatus(fobj, 'fail');
                            reject();
                        } else {
                            gs.setStatus(fobj, 'ok');

                            var key_github = res.content.download_url,
                                key_jsdelivr = "https://cdn.jsdelivr.net/gh/" + or + "/" + res.content.path;
                            var linkitem = document.querySelector('.nr-link-template').cloneNode(true);
                            linkitem.classList.remove('nr-link-template');

                            linkitem.querySelector('input').value = key_jsdelivr;
                            linkitem.querySelector('[data-cmd="open"]').href = key_jsdelivr;

                            linkitem.querySelector('[data-cmd="jsdelivr"]').setAttribute('data-value', key_jsdelivr);
                            linkitem.querySelector('[data-cmd="github"]').setAttribute('data-value', key_github);

                            document.querySelector('.nr-links').appendChild(linkitem);

                            resolve();
                        }
                    }).catch(err => {
                        console.log(arguments);
                        gs.setStatus(fobj, 'fail');
                        gs.msg(err + "");
                        reject();
                    })
                } else {
                    gs.msg('Check : owner repo token path');
                    reject();
                }
            }
        })
    },
    msg: function (content) {
        bs.msg(content);
    }
}

gs.init();