var editor;

nr.onReady = function () {
    ss.loading(true);

    me.init().then(() => {
        editor = me.create(nr.domEditor, {
            language: "text/plain",
            wordWrap: "on"
        });
        nr.domEditor.classList.add('border');
        nr.domEditor.style.height = "12em";

        nr.domCardBox.classList.remove('invisible');
        ss.loading(false);
    });

    // Base64 to file
    nr.domBtnTofile.addEventListener('click', function () {
        var code = editor.getValue();
        var mimeType = nr.domTxtMime.value;
        var blob = page.base64AsBlob(code, mimeType);
        console.log(blob);
        nr.domCardResult.innerHTML = "";
        nr.domCardInfo.innerHTML = `Base64 编码：<b>${nr.formatByteSize(code.length)}</b>，文件：<b>${nr.formatByteSize(blob.size)}</b>`;

        var vnode;
        if (blob.type.indexOf("image") >= 0) {
            vnode = document.createElement("img");
        }
        if (blob.type.indexOf("audio") >= 0) {
            vnode = document.createElement("audio");
            vnode.controls = true;
        }
        if (blob.type.indexOf("video") >= 0) {
            vnode = document.createElement("video");
            vnode.controls = true;
        }
        if (vnode) {
            vnode.src = URL.createObjectURL(blob);
        } else {
            vnode = document.createElement("a");
            vnode.href = URL.createObjectURL(blob);
            vnode.download = "file.bin";
            vnode.innerHTML = "下载";
        }
        vnode.style.maxWidth = "100%";

        nr.domCardResult.appendChild(vnode);
    });

    //Base64 decode
    nr.domBtnDecode.addEventListener('click', function () {
        try {
            var val = editor.getValue().trim();
            if (val != "") {
                var txt = atob(val);
                me.keepSetValue(editor, txt);
            } else {
                nr.alert("请输入 Base64 编码");
            }
        } catch (ex) {
            console.debug(ex);
            nr.alert("Base64 解码失败！");
        }
    });
    //Base64 encode
    nr.domBtnEncode.addEventListener('click', function () {
        try {
            var val = editor.getValue().trim();
            if (val != "") {
                var txt = btoa(val);
                me.keepSetValue(editor, txt);
            } else {
                nr.alert("请输入编码内容");
            }
        } catch (ex) {
            console.debug(ex);
            nr.alert("Base64 编码失败！");
        }
    });

    //检测 base64 
    nr.domDdDetect.addEventListener('sl-show', function () {
        var domMenu = nr.domDdDetect.querySelector('sl-menu');
        domMenu.innerHTML = '<sl-menu-item>正在检测</sl-menu-item>';

        var code = editor.getValue();
        var ftinfo = page.base64Detect(code);
        if (ftinfo.length) {
            domMenu.innerHTML = ftinfo.map(x => x.mime == null ? "" : `<sl-menu-item value="${x.mime}">.${x.extension} → ${x.mime}</sl-menu-item>`).join('');
        } else {
            domMenu.innerHTML = '<sl-menu-item>检测失败</sl-menu-item>';
        }
    });
    nr.domDdDetect.addEventListener('sl-select', event => {
        const selectedItem = event.detail.item;
        if (selectedItem.value != "") {
            if (window.isSecureContext && navigator.clipboard != null) {
                navigator.clipboard.writeText(selectedItem.value)
            } else {
                console.log(selectedItem.value);
            }
        }
    });

    //接收文件
    nr.receiveFiles(function (files) {
        page.fileAsBase64(files[0])
        nr.domTxtFile.value = "";
    }, nr.domTxtFile);
}

var page = {
    fileAsBase64: function (file) {
        ss.loading(true);
        var r = new FileReader();
        r.onload = function (e) {
            var result = e.target.result;
            me.keepSetValue(editor, result)
            nr.domCardInfo.innerHTML = `文件：<b>${nr.formatByteSize(file.size)}</b>，Base64 编码：<b>${nr.formatByteSize(result.length)}</b>`;
            nr.domTxtMime.value = result.substring(5, result.indexOf(';'));
            ss.loading(false);
        }
        r.readAsDataURL(file);
    },
    base64AsBlob: function (code, mimeType) {
        var parts = code.split(';base64,');

        var mime, base64;
        if (parts.length == 2) {
            mime = parts[0].split(':')[1];
            base64 = parts[1];
        } else {
            base64 = code;
        }

        var bin = atob(base64);
        var len = bin.length;
        var arr = new Uint8Array(len);
        for (var i = 0; i < len; i++) {
            arr[i] = bin.charCodeAt(i);
        }
        var ftinfo = magicBytes.filetypeinfo(arr);
        console.debug(`识别文件格式：\r\n${JSON.stringify(ftinfo, null, 2)}`);

        if (mime == null) {
            if (mimeType.trim() == "") {
                if (ftinfo.length) {
                    mimeType = ftinfo[0].mime
                } else {
                    mimeType = "application/octet-stream";
                }
            }
            mime = mimeType;
            nr.domTxtMime.value = mime;
        }

        return new Blob([arr], { type: mime });
    },
    base64Detect: function (code) {
        try {
            var parts = code.split(';base64,');
            var base64 = parts.pop();

            var bin = atob(base64);
            var len = bin.length;
            var arr = new Uint8Array(len);
            for (var i = 0; i < len; i++) {
                arr[i] = bin.charCodeAt(i);
            }
            var ftinfo = magicBytes.filetypeinfo(arr);
            return ftinfo;
        } catch (e) {
            return [];
        }
    }
}