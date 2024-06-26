﻿# NetnrMD 编辑器
Monaco Editor 编辑器 + Marked 解析 + DOMPurify 清洗 + highlight 代码高亮 + tocbot 目录 + pangu 间隙

> <https://md.js.org>

### Usage 使用

<https://www.npmjs.com/package/netnrmd>

```html
<div class="nrg-md">Loading ...</div>

<link href="dist/netnrmd.css" rel="stylesheet" />
<script src="dist/monaco.js"></script> <!-- or monaco-editor cdn -->
<script src="dist/netnrmd.js"></script>

<script>
    let nmd = netnrmd.init('.nrg-md');
</script>
```

### Options 选项

```js
let nmd = netnrmd.init('.nrg-md', {
    theme: 'light', // 主题 light(default), dark
    ph: '预览区域', // 预览区域提示
    toc: true, // 是否显示目录
    viewmodel: 2, // 视图：1 输入，2 分屏，3 预览
    fullscreen: 0, // 1 全屏，0 窗口
    fontsize: 16, // 编辑器字体大小
    height: 300, // 高度
    defer: 500, // 延迟解析（毫秒）
    autosave: true, // 默认有变化自动保存
    storekey: `${location.pathname}_netnrmd_content`, // 自动保存键，一个页面有多个编辑器时需要对应配置

    // 编辑器变动时回调
    input: function () {
        console.log(this.getmd());
    },

    // 渲染前回调
    viewbefore: function () {
        console.log(this);
    },

	// 触发命令前回调
    cmdbefore: function (cmd) {
        console.log(this);
    },

    // 调整高度
    resize: function (ch) {
        this.height(ch - 20);
    },

    // 文件上传（拖拽、粘贴）
    file: async (files) => {
        // for of 循环执行 netnrmd.upload 上传
        return 'file link';
    }
});
```

### Function 方法

```js
let nmd = netnrmd.init('.nrg-md');
console.log(nmd);

nmd.setmd(md);          //set markdown 赋值
nmd.getmd();            //get markdown 取值
nmd.sethtml(html);      //set html 赋值
nmd.gethtml();          //get html 取值

nmd.render();           //render 渲染
nmd.focus();            //focus 焦点选中
nmd.insert("text");     //insert 插入文本
nmd.replace("text");    //insert 替换文本
nmd.height(300);        //get/set height 获取或设置高度（非全屏模式时）

nmd.toggleView();       //toggle View 视图切换，默认 2、1、3 循环
nmd.toggleView(1);      //输入
nmd.toggleView(2);      //分屏
nmd.toggleView(3);      //预览

nmd.toggleTheme();         //切换主题
nmd.toggleTheme("light");  //浅色主题
nmd.toggleTheme("dark");   //暗黑主题 <html class="netnrmd-dark">

nmd.hide();             //hide 隐藏
nmd.hide('toolbar');    //hide 工具条
nmd.show();             //show 显示
nmd.show('toolbar');    //show 工具条

nmd.setstore();         //set store 写入本地保存
nmd.getstore();         //get store 获取本地保存

nmd.tooltip("Upload...")    //提示，倒计时5秒关闭
nmd.tooltip("90%", 20000)   //20秒关闭
nmd.closeTooltip()          //关闭提示

nmd.objOptions          //配置选项
nmd.objWrite            //编辑器对象，更多信息参考 monaco-editor 文档
nmd.objWrite.getValue() //获取编辑器值

nmd.save(format, filename) //保存 (markdown html png pdf)
nmd.addCommand("Ctrl+S", () => { /* save */ }) //快捷键

//全局对象
netnrmd.render(md)      // 解析 Markdown, 清洗+高亮
netnrmd.spacing(text)   // 文字、数字、符号、英文添加空格间隙
netnrmd.popup(title, content)   // 弹出层（emoji、about）
netnrmd.upload(options) // 上传 {url,method,body,onprogress}
netnrmd.emoji           // 表情
netnrmd.hljs            // 高亮组件
netnrmd.DOMPurify       // 清洗组件
netnrmd.marked          // markdown 组件
```