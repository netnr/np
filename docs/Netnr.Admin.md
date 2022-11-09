### 组件
- @shoelace-style/shoelace 常见组件包
- ag-grid 表格
- bootstrap 常用、辅助样式、栅格、布局等，勿用组件（避免主题冲突）
- page 路由
- json-bigint 解析 JSON (支持超出 Number.MAX_SAFE_INTEGER)
- localforage 本地存储（有需要）
- echarts 图表


### 结构
- assets 资源路径，静态文件
- css 样式目录
- ### js 脚本，全局
- js/index.js 打包入口
- js/naFunction.js 公共方法
- js/naGlobal.js 全局处理
- js/naGrid.js 表格相关
- js/naVary.js 变量、配置
- ### js/page 所有页面
- js/page/index 首页、错误页面
- js/page/user 个人用户
- js/page/fast 快捷表管理
- js/page/dev 开发相关
- js/page/admin 系统管理
- ### js/pagePack 页面分模块打包


### 指南
- 使用异步 async
- 组件采用 imprt 按需加载，在 `naGlobal.getPackage` 维护，默认暴露给 window 对象
- 页面按 pagePack 目录下模块加载，注意控制模块大小，模块加载方法为 `naGlobal.getPagePack`
- 设置节点 `data-href` 属性打开链接
- 设置节点 `data-action` 属性触发行为，如切换主题，需在 `naGlobal.action` 添加响应
- 样式以 `na-` 如 `<div class="na-card-one"></div>` 可从 `naVary.domCardOne` 获取对象  
如 `<div class="mb-2 na-card-one"></div>` 则不能获取  
样式以 `nap-` 开头为页面用，需要手动调用     `naGlobal.buildObjNode(naVary.domPanel, "nap-", naPage)`  
同时避免写 `nap-` 开头的样式，以免冲突，需要写样式建议以 `na-{module}-{class}` 格式，如：`na-index-card-one` 表示首页模块的一个卡片


### 页面脚本遵循
```js
var naPage = {
    //（必须）设置路径，唯一，路由匹配则运行该脚本，调用方法 render()
    // admin 表示所属模块 且须放置 page/admin 目录下
    pathname: '/admin/log',

    //（必须）渲染，naVary.domPanel 指向页面面板，默认刷新或重新打开会清空面板
    render: async () => {
        naVary.domPanel.innerHTML = `<div class="nap-card">Hello World</div>`;
        
        // 构建节点对象（在面板渲染后）
        naGlobal.buildObjNode(naVary.domPanel, "nap-", naPage);
        
        //定时任务
        var tn1 = setInterval(() => {
            naPage.domCard.innerHTML = new Date().toISOString();
        }, 1000);
        //加入标记，用于清理选项卡时清理任务
        naGlobal.pageTiming(naPage.pathname, tn1);
    },

    /*（可选）变更主题*/
    event_theme: (theme) => {
        console.debug(theme)
    },
    /*（可选）窗口大小*/
    event_resize: () => {
        console.debug('resize')
    },
}

export { naPage };
```

对象前缀命名遵循  
- naPage.domXXX 节点对象
- naPage.gridXXX 表格对象
- naPage.chartXXX 图表对象
- naPage.viewXXX 显示、弹出方法
- naPage.buildXXX 构建方法
- naPage.getXXX 获取方法
- naPage.reqXXX 请求接口方法
- naPage.bindXXX 绑定事件方法
- naPage.event_XXX 事件全局回调方法

刷新、关闭页面（选项卡）清理对象前缀配置 `naVary.flagTabClearPrefix`


### 快捷表管理
建表、建列、搞定。

表操作（新增、修改、删除）
```
[
  {
    "method": "POST",
    "path": "tableName",
    "body": {
      "field1": "a",
      "field2": 2
    }
  },
  {
    "method": "PUT",
    "path": "tableName/dataId",
    "body": {
      "field1": "b",
      "field2": 22
    }
  },
  {
    "method": "DELETE",
    "path": "tableName/dataId"
  }
]
```