﻿import { nrPolyfill } from '../../frame/nrPolyfill';
import { nrcBase } from '../../frame/nrcBase';

let nrWeb = {
    init: async () => {
        //注册 sw
        let packageName = "netnr";
        if (isSecureContext && !window[`webpackHotUpdate${packageName}`]) {
            navigator.serviceWorker.register('/sw.js')
                .then(reg => console.debug('SW registered: ', reg))
                .catch(ex => console.debug('SW failed: ', ex));
        }

        await nrPolyfill.init();

        //载入组件
        await import('../index');
        let { monaco } = await import("../../frame/monaco-old");
        Object.assign(window, { monaco });

        //扩展
        nrWeb.expand();

        //关闭提示
        document.getElementById('style0').remove();

        //读取主题
        let isDark = nrcBase.isDark();

        let domMd = document.createElement('div');
        document.body.appendChild(domMd);

        //初始化编辑器
        let nmd = window.nmd = netnrmd.init(domMd, {
            theme: isDark ? "dark" : "light",

            // 渲染前回调
            viewbefore: function () {
                let insertIndex = this.objToolbarIcons.findIndex(x => x.cmd == "export");
                this.objToolbarIcons.splice(insertIndex, 0, netnrmd.uploadExtend)
            },

            // 触发命令前回调
            cmdbefore: function (cmd) {
                if (cmd == 'theme') {
                    //保存主题
                    nrcBase.saveTheme(this.objOptions.theme == "dark" ? 'light' : 'dark');
                }
            },

            //调整大小
            resize: function (ch) {
                this.height(ch - 20);
            },

            //文件
            file: async (files) => {
                let that = nmd;
                let links = [];
                for (let file of files) {
                    that.tooltip(`${file.name} 正在上传...`);

                    let fd = new FormData();
                    fd.append('file', file);
                    let xhr = await netnrmd.upload({
                        url: "https://netnr.zme.ink/api/v1/Upload", body: fd,
                        onprogress: (pp) => {
                            that.tooltip(`${file.name} 已上传 ${pp}%`, pp == 100 ? 9999 : 30000);
                        }
                    });

                    if (xhr.status == 200) {
                        let res = JSON.parse(xhr.responseText);
                        if (res.code == 200) {
                            //上传成功，插入链接
                            let link = `[${file.name}](https://netnr.zme.ink/${res.data})`;
                            if (file.type.startsWith("image")) {
                                link = "!" + link;
                            }
                            links.push(link);
                            that.closeTooltip();
                        } else {
                            that.tooltip(`上传失败 ${file.name}, ${res.msg}`, 20000);
                        }
                    } else {
                        that.tooltip(`上传失败 ${file.name}, HTTP ${xhr.statusText || xhr.status}`, 20000);
                    }
                }
                return links.join('\r\n');
            }
        });

        //载入 README.md
        if (nmd.getmd().trim() == "") {
            let resp = await fetch('README.md');
            let result = await resp.text();
            nmd.setmd(result);
        }
    },

    /**
     * 功能扩展
     */
    expand: () => {
        Object.assign(netnrmd, {
            //上传
            uploadExtend: {
                title: '上传', cmd: 'upload', svg: '<path fill-rule="evenodd" d="M7.646 5.146a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 6.707V10.5a.5.5 0 0 1-1 0V6.707L6.354 7.854a.5.5 0 1 1-.708-.708l2-2z"/><path d="M4.406 3.342A5.53 5.53 0 0 1 8 2c2.69 0 4.923 2 5.166 4.579C14.758 6.804 16 8.137 16 9.773 16 11.569 14.502 13 12.687 13H3.781C1.708 13 0 11.366 0 9.318c0-1.763 1.266-3.223 2.942-3.593.143-.863.698-1.723 1.464-2.383zm.653.757c-.757.653-1.153 1.44-1.153 2.056v.448l-.445.049C2.064 6.805 1 7.952 1 9.318 1 10.785 2.23 12 3.781 12h8.906C13.98 12 15 10.988 15 9.773c0-1.216-1.02-2.228-2.313-2.228h-.5v-.5C12.188 4.825 10.328 3 8 3a4.53 4.53 0 0 0-2.941 1.1z"/>',
                action: function (that) {
                    if (!that.domUpload) {
                        //构建弹出内容
                        let htm = [];
                        htm.push('<div style="height:5em;margin:1em;border:3px dashed var(--nmd-border);">');
                        htm.push('<input type="file" style="width:100%;height:100%;opacity:.6" />');
                        htm.push('</div>');

                        //保存创建的上传弹出
                        that.domUpload = netnrmd.popup("上传", htm.join(''));
                        let ptitle = that.domUpload.querySelector('.np-header').querySelector('span');

                        //选择文件上传，该上传接口仅为演示使用，仅支持图片格式的附件
                        that.domUpload.querySelector('input').addEventListener('change', function () {
                            let file = this.files[0];
                            if (file) {
                                let fd = new FormData();
                                fd.append('file', file);

                                //发起上传
                                let xhr = new XMLHttpRequest();
                                xhr.upload.onprogress = function (event) {
                                    if (event.lengthComputable) {
                                        //上传百分比
                                        let per = ((event.loaded / event.total) * 100).toFixed(2);
                                        if (per < 100) {
                                            ptitle.innerHTML = netnrmd.uploadExtend.title + " （" + per + "%）";
                                        } else {
                                            ptitle.innerHTML = netnrmd.uploadExtend.title;
                                        }
                                    }
                                };

                                xhr.open("POST", "https://netnr.zme.ink/api/v1/Upload", true);
                                xhr.send(fd);
                                xhr.onreadystatechange = function () {
                                    if (xhr.readyState == 4) {
                                        if (xhr.status == 200) {
                                            console.log(xhr.responseText)
                                            let res = JSON.parse(xhr.responseText);
                                            if (res.code == 200) {
                                                //上传成功，插入链接
                                                let link = `[${file.name}](https://netnr.zme.ink/${res.data})`;
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
    }
}

export { nrWeb };