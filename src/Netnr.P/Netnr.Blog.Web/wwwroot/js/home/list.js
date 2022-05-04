nr.onReady = function () {
    //目录
    var toc = {
        index: 1,
        init: function () {
            var mb = document.querySelector('.markdown-body');
            var nas = mb.getElementsByTagName('*'), hasHeader = false;
            for (var i = 0; i < nas.length; i++) {
                var na = nas[i];
                switch (na.nodeName) {
                    case "H1":
                    case "H2":
                    case "H3":
                    case "H4":
                    case "H5":
                    case "H6":
                        na.id = "toc_" + toc.index++;
                        hasHeader = true;
                        break;
                }
            }

            if (hasHeader) {
                mb.parentElement.classList.add('nr-toc');

                tocbot.init({
                    tocSelector: '.toc',
                    contentSelector: '.markdown-body',
                    headingSelector: 'h1, h2, h3, h4, h5, h6',
                    hasInnerContainers: true,
                });

            }
        }
    }
    toc.init();

    //保存
    if (nr.domBtnReply) {
        nr.domBtnReply.addEventListener('click', function () {
            if (nr.domEditor.value.trim() == "") {
                nr.alert("写点什么...")
            } else {
                var obj = {
                    Uid: nr.domHidUserId.value,
                    UrTargetId: nr.domHidWriteId.value,
                    UrContent: netnrmd.render(nr.domEditor.value),
                    UrContentMd: nr.domEditor.value
                }

                nr.domBtnReply.disabled = true;
                fetch('/Home/ReplySave', {
                    method: 'POST',
                    body: nr.toFormData(obj)
                }).then(x => x.json()).then(res => {
                    nr.domBtnReply.disabled = false;
                    if (res.code == 200) {
                        nr.domEditor.value = "";
                        location.reload(false);
                    } else {
                        nr.alert(res.msg);
                    }
                }).catch(ex => {
                    nr.domBtnReply.disabled = false;
                    console.debug(ex);
                    nr.alert("网络错误")
                })
            }
        })

        //预览
        nr.domBtnPreview.addEventListener('click', function () {
            nr.domPreview.innerHTML = netnrmd.render(nr.domEditor.value);
        })

        //点赞
        nr.domBtnLaud.addEventListener('click', function () {
            ConnSave(1)
        })
        //收藏
        nr.domBtnMark.addEventListener('click', function () {
            ConnSave(2)
        })
        function ConnSave(action) {
            nr.domBtnLaud.loading = true;
            nr.domBtnMark.loading = true;

            fetch(`/Home/ConnSave/${nr.domHidWriteId.value}?a=${action}`).then(resp => {
                nr.domBtnLaud.loading = false;
                nr.domBtnMark.loading = false;
                return resp.json()
            }).then(res => {
                if (res.code == 200) {
                    location.reload(false);
                } else {
                    nr.alert(res.msg);
                }
            }).catch(ex => {
                console.debug(ex);
                nr.alert("网络错误")
            })
        }
    }

    //代码可编辑
    document.querySelector('.markdown-body').querySelectorAll("pre>code").forEach(node => {
        node.innerHTML = node.innerHTML.trim();
        node.setAttribute("contenteditable", true);
    })

    //阅读量
    if (location.hostname != "localhost" && sessionStorage.getItem("wid") != nr.domHidWriteId.value) {
        sessionStorage.setItem("wid", nr.domHidWriteId.value);
        fetch("/Home/ReadPlus/" + nr.domHidWriteId.value);
    }
}