# NetnrMD 编辑器
Monaco Editor 编辑器 + Marked 解析 + DOMPurify 清洗 + highlight 代码高亮 + pangu 间隙

> <https://md.js.org>  
> <https://md.netnr.eu.org>

### Install 安装

<https://www.jsdelivr.com/package/npm/netnrmd>

```html
<div>
    <div id="editor">Loading ...</div>
</div>

<link href="https://npm.elemecdn.com/netnrmd@3.0.2/src/netnrmd.css" rel="stylesheet" />
<script src="https://npm.elemecdn.com/netnrmd@3.0.2/src/netnrmd.bundle.js"></script>

<script src="https://npm.elemecdn.com/monaco-editor@0.31.1/min/vs/loader.js"></script>

<script>
    require.config({
        paths: {
            vs: 'https://npm.elemecdn.com/monaco-editor@0.31.1/min/vs'
        },
        'vs/nls': { availableLanguages: { '*': 'zh-cn' } }
    });

    require(['vs/editor/editor.main'], function () {

        // 初始化
        window.nmd = new netnrmd('#editor');
    });
</script>
```

### Options 选项

```js
var nmd = new netnrmd('#editor', {
    viewmodel: 2,       // 视图：1 输入，2 分屏，3 预览，默认 2
    fullscreen: 1,      // 全屏
    fontsize: 16,       // 编辑器字体大小
    height: 300,        // 高度
    defer: 500,         // 延迟解析（毫秒）
    storekey: "key",    // 自动保存键，默认 {location.pathname}_netnrmd_markdown，一个页面有多 netnrmd 编辑器时需要对应配置
    autosave: true,     // 默认有变化自动保存

    // 渲染前回调
    viewbefore: function () {
        console.log(this);
    },

    // 编辑器变动时回调
    input: function () {
        console.log(this);
    },

	// 触发命令回调
    cmdcallback: function (cmd) {
        console.log(this);
    }
});
```

### Function 方法

```js
var nmd = new netnrmd('#editor');
console.log(nmd);

nmd.setmd(md);          //set markdown 赋值
nmd.getmd();            //get markdown 取值
nmd.sethtml(html);      //set html 赋值
nmd.gethtml();          //get html 取值

nmd.render();           //render 渲染

nmd.focus();            //focus 焦点选中
nmd.height(200);        //set height 设置高度

nmd.toggleView();       //toggle View 视图切换，默认 2、1、3 循环
nmd.toggleView(1);      //输入
nmd.toggleView(2);      //分屏
nmd.toggleView(3);      //预览

nmd.hide();             //hide 隐藏
nmd.hide('toolbar');    //hide 工具条
nmd.show();             //show 显示
nmd.show('toolbar');    //show 工具条

nmd.setstore();         //set store 写入本地保存
nmd.getstore();         //get store 获取本地保存

netnrmd.render(md)      // 解析 Markdown
netnrmd.getSelectText(me)   // 获取 Monaco Editor 选中文本，me => nmd.obj.me
netnrmd.insertAfterText(me, text)   // 在光标后插入文本
netnrmd.keepSetValue(me, text)   // 保留赋值（可撤回）
netnrmd.spacing(text)   // 文字、数字、符号、英文添加空格间隙
netnrmd.popup(title, content)   // 弹出层
```