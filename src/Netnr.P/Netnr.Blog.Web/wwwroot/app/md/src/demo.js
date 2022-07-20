//NetnrMD编辑器 功能扩展
Object.assign(netnrmd, {
    //上传
    uploadExtend: {
        title: '上传', cmd: 'upload', svg: '<path fill-rule="evenodd" d="M7.646 5.146a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 6.707V10.5a.5.5 0 0 1-1 0V6.707L6.354 7.854a.5.5 0 1 1-.708-.708l2-2z"/><path d="M4.406 3.342A5.53 5.53 0 0 1 8 2c2.69 0 4.923 2 5.166 4.579C14.758 6.804 16 8.137 16 9.773 16 11.569 14.502 13 12.687 13H3.781C1.708 13 0 11.366 0 9.318c0-1.763 1.266-3.223 2.942-3.593.143-.863.698-1.723 1.464-2.383zm.653.757c-.757.653-1.153 1.44-1.153 2.056v.448l-.445.049C2.064 6.805 1 7.952 1 9.318 1 10.785 2.23 12 3.781 12h8.906C13.98 12 15 10.988 15 9.773c0-1.216-1.02-2.228-2.313-2.228h-.5v-.5C12.188 4.825 10.328 3 8 3a4.53 4.53 0 0 0-2.941 1.1z"/>',
        action: function (that) {
            if (!that.domUpload) {
                //构建弹出内容
                var htm = [];
                htm.push('<div style="height:5em;margin:1em;border:3px dashed var(--nmd-editor-border);">');
                htm.push('<input type="file" style="width:100%;height:100%;opacity:.6" />');
                htm.push('</div>');

                //保存创建的上传弹出
                that.domUpload = netnrmd.popup("上传", htm.join(''));
                var ptitle = that.domUpload.querySelector('.np-header').querySelector('span');

                //选择文件上传，该上传接口仅为演示使用，仅支持图片格式的附件
                that.domUpload.querySelector('input').addEventListener('change', function () {
                    var file = this.files[0];
                    if (file) {
                        var fd = new FormData();
                        fd.append('json', 'true');
                        fd.append('file', file);

                        //发起上传
                        var xhr = new XMLHttpRequest();
                        xhr.upload.onprogress = function (event) {
                            if (event.lengthComputable) {
                                //上传百分比
                                var per = ((event.loaded / event.total) * 100).toFixed(2);
                                if (per < 100) {
                                    ptitle.innerHTML = netnrmd.uploadExtend.title + " （" + per + "%）";
                                } else {
                                    ptitle.innerHTML = netnrmd.uploadExtend.title;
                                }
                            }
                        };

                        xhr.open("POST", "https://bashupload.com/", true);
                        xhr.send(fd);
                        xhr.onreadystatechange = function () {
                            if (xhr.readyState == 4) {
                                if (xhr.status == 200) {
                                    console.log(xhr.responseText)
                                    var res = JSON.parse(xhr.responseText);
                                    if (res.file) {
                                        //上传成功，插入链接
                                        var link = `[${file.name}](${res.file.url}?download=1)`;
                                        if (file.type.startsWith("image")) {
                                            link = "!" + link;
                                        }
                                        that.insert(link);
                                        that.domUpload.style.display = "none";
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
            that.domUpload.style.display = "";
            that.domUpload.querySelector('input').value = '';
        }
    }
})

// 初始化编辑器
var nmd = netnrmd.init('.nr-editor', {
    // 渲染前回调
    viewbefore: function () {
        var insertIndex = this.objToolbarIcons.findIndex(x => x.cmd == "export");
        this.objToolbarIcons.splice(insertIndex, 0, netnrmd.uploadExtend)
    },

    //调整大小
    resize: function (ch) {
        this.height(ch - 20);
    }
});
//载入 README.md
if (nmd.getmd().trim() == "") {
    fetch('README.md').then(resp => resp.text()).then(res => {
        nmd.setmd(res)
    })
}