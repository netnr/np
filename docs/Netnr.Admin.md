### 权限设计
- 用户对应多个角色，每个角色配置对应的权限
- 角色配置菜单和菜单对应按钮，此为页面权限
- 角色配置模块和接口，此为接口权限
- 数据权限未做通配，可分多个方法或在方法中过滤来实现

---

### 系统模块

#### 快表管理
建表、建列、搞定，单表的简单应用

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

#### 运维监测
以主机 HOST 为首，关联 HTTP SSL Port Database 等监测项，加入任务调度，配置报警规则及联系人

---

### 后端
略

### 前端

#### 开发事项
- 使用异步 async
- 模块名称以小写、下划线固定格式，如 fast_table
- 设置节点 `data-href` 属性打开链接
- 设置节点 `data-action` 属性触发行为，如切换主题
- 全局样式前缀 `nrg-` 如 `<div class="nrg-card-one"></div>` 可从 `nrVary.domCardOne` 获取对象
- 页面样式前缀 `nrp-` 需要手动调用 `nrGlobal.buildObjNode(nrVary.domPanel, "nrp", nrPage)`
- 样式格式建议 `nrp-{module}-{class}` 如：`nrp-index-card-one` 表示首页模块的一个卡片
- 表格 Grid 列字段以 `$` 开头为构建变量，以 `#` 开头为虚拟列

#### 页面开发
```js
let nrPage = {
    // 设置路径，唯一，路由匹配则运行该脚本，调用方法 render()
    // admin 表示模块名称 且须放置 page/admin 目录下，模块名称使用小写、下划线
    pathname: '/admin/log',
    title: "可选，标题，默认从菜单提取",
    icon: "可选，图标，默认从菜单提取",

    //（必须）渲染，nrVary.domPanel 指向页面面板，默认刷新或重新打开会清空面板
    render: async () => {
        nrVary.domPanel.innerHTML = `<div class="nrp-card">Hello World</div>`;
        
        // 构建节点对象（在面板渲染后）
        nrGlobal.buildObjNode(nrVary.domPanel, "nrp", nrPage);
        
        //定时任务
        var tn1 = setInterval(() => {
            nrPage.domCard.innerHTML = new Date().toISOString();
        }, 1000);
        //加入标记，用于清理选项卡时清理任务
        nrGlobal.pageTiming(nrPage.pathname, tn1);
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

export { nrPage };
```

### 日志
#### 2023-03
- 新增 白名单 IP 登录
- 新增 监控拓扑图
- 新增 HTTP、Port 记录及图表统计
- 新增 Database 达梦数据库（DM），为兼容达梦数据库版本长度，更改版本字段长度为 100，共两个字段：`xops_monitor_database.backfill_version` 和 `xops_record_database.rdb_version`
- 新增 WebHook 推送推送方式，可自定义配置参数
- 修复 Port、Database 不能修改检测时间
- 修复 定时任务修改后存在问题
- 优化 改进 Host、HTTP、Port、SSL、Database 记录表为复合索引，显著提升查询

#### 2023-04
- 新增 告警策略配置（试运行）
- 新增 上报客户端设置 15 秒超时，降低单次任务耗时对下次的影响
- 新增 示例用户数据的导出及初始化数据库自动导入示例数据
- 新增 任务调度记录上次结果，并可查看结果
- 新增 双因素认证(2FA)，保护如数据库连接字符串
- 新增 SSL、Database 记录及图表统计
- 修复 异步定时任务异常拖挂服务的问题
- 修复 无数据时查询监控状态出错的问题
- 调整 取消用户角色表 base_role 两个字段（role_modules、role_methods）的必填规则

#### 2023-05
- 新增 Database 数据库查询耗时字段
- 调整 推送服务支持状态，可关闭推送，但任记录