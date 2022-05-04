nr.onChangeSize = function (ch) {
    var vh = ch - nr.domIframe.getBoundingClientRect().top - 30;
    nr.domIframe.style.height = vh + 'px';
}

nr.onReady = function () {

    nr.domTxtUrl.addEventListener('input', function () {
        page.view(this.value)
    });

    //接收文件
    nr.receiveFiles(function (files) {
        page.upload(files[0]);
    }, nr.domTxtFile);
}

var page = {
    api: "https://view.officeapps.live.com/op/embed.aspx?src=",
    view: function (url) {
        if (url && url != "") {
            url = page.api + decodeURIComponent(url);
            nr.domAurl.href = url;
            nr.domAurl.innerText = url;
            nr.domIframe.src = url;
            nr.domAurl.parentNode.classList.remove("invisible");
            nr.domIframe.classList.remove("invisible");
        } else {
            nr.domAurl.href = 'javascript:void(0);';
            nr.domAurl.innerText = page.api;
            nr.domIframe.src = 'about:blank';
            nr.domAurl.parentNode.classList.add("invisible");
            nr.domIframe.classList.add("invisible");
        }
    },
    upload: function (file) {
        var err = [];
        if (file.size > 1024 * 1024 * 20) {
            err.push('文档大小限制 20MB')
        }
        if (file.type.indexOf('application') == -1 || ".doc docx .xls xlsx .ppt pptx".indexOf(file.name.slice(-4).toLowerCase()) == -1) {
            err.push('请选择 Office文档')
        }
        nr.domTxtFile.value = '';

        if (err.length) {
            nr.alert(err.join('<br/>'));
        } else {
            //上传
            var formData = new FormData();
            formData.append("file", file);

            ss.loading(true);
            fetch(`${ss.apiServer}/api/v1/Upload`, {
                method: 'POST',
                body: formData
            }).then(resp => resp.json()).then(res => {
                ss.loading(false);
                if (res.code == 200) {
                    page.view(`${ss.apiServer}/${res.data.prp}${res.data.path}`);
                } else {
                    nr.alert(res.msg);
                }
            }).catch(ex => {
                console.debug(ex);
                ss.loading(false);
                nr.alert('网络错误');
            })
        }
    }
}