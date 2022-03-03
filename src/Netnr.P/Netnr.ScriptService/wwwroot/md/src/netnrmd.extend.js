//NetnrMD编辑器 功能扩展

netnrmd.extend = {
    //关于
    about: {
        //按钮
        button: { title: '关于', cmd: 'about', svg: '<path d="m10.277 5.433-4.031.505-.145.67.794.145c.516.123.619.309.505.824L6.101 13.68c-.34 1.578.186 2.32 1.423 2.32.959 0 2.072-.443 2.577-1.052l.155-.732c-.35.31-.866.434-1.206.434-.485 0-.66-.34-.536-.939l1.763-8.278zm.122-3.673a1.76 1.76 0 1 1-3.52 0 1.76 1.76 0 0 1 3.52 0z"/>' },
        //动作
        action: function (that) {
            if (!that.aboutpopup) {
                //构建弹出内容
                var htm = [];
                htm.push("<h3>NetnrMD 编辑器</h3>");
                htm.push("<p>Monaco Editor 编辑器 + Marked 解析 + DOMPurify 清洗 + highlight 代码高亮 + pangu 间隙</p>");
                htm.push("<p><a href='https://github.com/netnr'>https://github.com/netnr</a></p>");
                htm.push("<p>&copy; <a href='https://www.netnr.com' target='_blank'>netnr</a></p>");
                //弹出
                that.aboutpopup = netnrmd.popup("关于", htm.join(''));
            }
            that.aboutpopup.style.display = '';
        }
    },
    //上传
    upload: {
        //按钮
        button: { title: '上传', cmd: 'upload', svg: '<path fill-rule="evenodd" d="M7.646 5.146a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 6.707V10.5a.5.5 0 0 1-1 0V6.707L6.354 7.854a.5.5 0 1 1-.708-.708l2-2z"/><path d="M4.406 3.342A5.53 5.53 0 0 1 8 2c2.69 0 4.923 2 5.166 4.579C14.758 6.804 16 8.137 16 9.773 16 11.569 14.502 13 12.687 13H3.781C1.708 13 0 11.366 0 9.318c0-1.763 1.266-3.223 2.942-3.593.143-.863.698-1.723 1.464-2.383zm.653.757c-.757.653-1.153 1.44-1.153 2.056v.448l-.445.049C2.064 6.805 1 7.952 1 9.318 1 10.785 2.23 12 3.781 12h8.906C13.98 12 15 10.988 15 9.773c0-1.216-1.02-2.228-2.313-2.228h-.5v-.5C12.188 4.825 10.328 3 8 3a4.53 4.53 0 0 0-2.941 1.1z"/>' },
        //动作
        action: function (that) {
            if (!that.uploadpopup) {
                //构建弹出内容
                var htm = [];
                htm.push('<div style="height:100px;margin:15px;border:3px dashed #ddd">');
                htm.push('<input type="file" style="width:100%;height:100%;" />');
                htm.push('</div>');

                //保存创建的上传弹出
                that.uploadpopup = netnrmd.popup("上传", htm.join(''));
                var ptitle = that.uploadpopup.querySelector('.np-header').querySelector('span');

                //选择文件上传，该上传接口仅为演示使用，仅支持图片格式的附件
                that.uploadpopup.querySelector('input').addEventListener('change', function () {
                    var file = this.files[0];
                    if (file) {
                        if (file.size > 1024 * 1024 * 5) {
                            alert('文件过大 （MAX 5 MB）')
                            this.value = "";
                            return;
                        }

                        var fd = new FormData();
                        fd.append('file', file);

                        //发起上传
                        var xhr = new XMLHttpRequest();
                        xhr.upload.onprogress = function (event) {
                            if (event.lengthComputable) {
                                //上传百分比
                                var per = ((event.loaded / event.total) * 100).toFixed(2);
                                if (per < 100) {
                                    ptitle.innerHTML = netnrmd.extend.upload.button.title + " （" + per + "%）";
                                } else {
                                    ptitle.innerHTML = netnrmd.extend.upload.button.title;
                                }
                            }
                        };

                        xhr.open("POST", "https://www.netnr.eu.org/api/v1/Upload", true);
                        xhr.send(fd);
                        xhr.onreadystatechange = function () {
                            if (xhr.readyState == 4) {
                                if (xhr.status == 200) {
                                    console.log(xhr.responseText)
                                    var res = JSON.parse(xhr.responseText);
                                    if (res.code == 200) {
                                        let iat = `[${file.name}](https://www.netnr.eu.org${res.data.path})`;
                                        if (file.type.startsWith("image")) {
                                            iat = "!" + iat;
                                        }
                                        //上传成功，插入链接
                                        netnrmd.insertAfterText(that.obj.me, iat);
                                        that.uploadpopup.style.display = "none";
                                    } else {
                                        alert('上传失败');
                                    }
                                } else {
                                    alert('上传失败');
                                }
                            }
                        }
                    }
                }, false)
            }
            that.uploadpopup.style.display = "";
            that.uploadpopup.querySelector('input').value = '';
        }
    },
    //导出
    import: {
        //按钮
        button: { title: '导出', cmd: 'import', svg: '<path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/><path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z"/>' },
        //动作
        action: function (that) {
            if (!that.importpopup) {
                //构建弹出内容
                var htm = [];
                htm.push("<div style='text-align:center;'>")
                "Markdown Html Word PDF Png".split(' ').map(function (x) {
                    htm.push(' <button style="margin:10px;font-size:1.5rem;">' + x + '</button> ');
                });
                htm.push("</div>");
                //弹出
                that.importpopup = netnrmd.popup("导出", htm.join(''));
                that.importpopup.addEventListener('click', function (e) {
                    var target = e.target;
                    if (target.nodeName == "BUTTON") {
                        var bv = target.innerHTML.toLowerCase();
                        switch (bv) {
                            case "markdown":
                                netnrmd.down(that.getmd(), 'nmd.md')
                                break;
                            case "html":
                            case "word":
                                {
                                    var netnrmd_body = that.gethtml();
                                    fetch('src/netnrmd.css').then(x => x.text()).then(netnrmd_style => {
                                        var html = `
                                                <!DOCTYPE html>
                                                <html>
                                                    <head>
                                                    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
                                                    <style type="text/css">
                                                        ${netnrmd_style}
                                                    </style>
                                                    </head>
                                                    <body>
                                                    <div class="markdown-body">${netnrmd_body}</div>
                                                    </body>
                                                </html>
                                            `;

                                        if (bv == "html") {
                                            netnrmd.down(html, 'nmd.html');
                                        }
                                        else if (bv == "word") {
                                            require(['https://npm.elemecdn.com/html-docx-js@0.3.1/dist/html-docx.js'], function (module) {
                                                netnrmd.down(module.asBlob(html), "nmd.docx");
                                            });
                                        }
                                    })
                                }
                                break;
                            case "pdf":
                                {
                                    var uri = ["localhost", "ss.netnr.com"].includes(location.hostname)
                                        ? "https://s1.netnr.com/libs/mix/html2pdf.bundle.min.js"
                                        : "https://s1.netnr.eu.org/libs/mix/html2pdf.bundle.min.js";
                                    require([uri], function (module) {
                                        var ch = that.obj.view.clientHeight;
                                        that.obj.view.style.height = 'auto';
                                        var vm = that.obj.viewmodel;
                                        that.toggleView(3);
                                        module(that.obj.view, {
                                            margin: 3,
                                            filename: 'nmd.pdf',
                                            html2canvas: { scale: 1.5 }
                                        }).then(function () {
                                            that.obj.view.height = ch + 'px';
                                            that.toggleView(vm);
                                        })
                                    })
                                }
                                break;
                            case "png":
                                {
                                    var backvm = false;
                                    if (that.obj.viewmodel == 1) {
                                        that.toggleView(2);
                                        backvm = true;
                                    }

                                    require(['https://npm.elemecdn.com/html2canvas@1.4.1/dist/html2canvas.min.js'], function (module) {
                                        var ch = that.obj.view.clientHeight;
                                        that.obj.view.style.height = 'auto';
                                        module(that.obj.view, {
                                            scale: 1.5,
                                            margin: 15
                                        }).then(function (canvas) {
                                            that.obj.view.height = ch + 'px';
                                            netnrmd.down(canvas, "nmd.png");

                                            if (backvm) {
                                                that.toggleView(1);
                                            }
                                        })
                                    })
                                }
                                break;
                        }

                        this.style.display = 'none';
                    }
                }, false)
            }
            that.importpopup.style.display = '';
        }
    }
}

netnrmd.down = function (content, file) {
    var aTag = document.createElement('a');
    aTag.download = file;
    if (content.nodeType == 1) {
        aTag.href = content.toDataURL();
    } else {
        var blob = new Blob([content]);
        aTag.href = URL.createObjectURL(blob);
    }
    document.body.appendChild(aTag);
    aTag.click();
    aTag.remove();
}
