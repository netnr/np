### 权限设计
- 用户对应多个角色，每个角色配置对应的权限
- 角色配置菜单和菜单对应按钮，此为页面权限
- 角色配置模块和接口，此为接口权限
- 数据权限未做通配，可分多个方法或在方法中过滤来实现

---

### 系统模块

#### 表格管理
建表、建列，单表的简单应用

#### 监控管理
以主机 HOST 为首，关联 HTTP SSL Port Database 等监测项，加入任务调度，配置报警规则及联系人

#### 运维管理
记录运维人员的操作记录

#### 视频切片管理
视频上传、使用 ffmpeg 切片为 m3u8 格式，使用 hls.js 播放

---

### 后端
略

### 前端

#### 开发事项
- 使用异步 async
- 模块名称小写
- 设置节点 `data-href` 属性打开链接
- 设置节点 `data-action` 属性触发行为，如切换主题
- 全局样式前缀 `nrg-` 如 `<div class="nrg-card-one"></div>` 可从 `nrVary.domCardOne` 获取对象
- 页面样式前缀 `nrp-` 需要手动调用 `nrGlobal.buildObjNode(nrVary.domPanel, "nrp", nrPage)`
- 样式格式建议 `nrp-{module}-{class}` 如：`nrp-index-card-one` 表示首页模块的一个卡片
- 表格 Grid 列字段以 `$` 开头为构建变量，以 `#` 开头为虚拟列

#### 目录结构
- file 公共文件
- frame 公共框架
- frame/Bootstrap 和 frame/Shoelace 为两套框架，区分样式
- frame/Shared 公共框架共享
- frame/nrcXxx.js 独立组件 .nrc- 开头组件样式类
- frame/nrXxx.js 依赖组件的封装 .nrp- 开头具体页面 .nrg- 开头项目层面

### 更新记录
- button 表样式类从 nap 改为 nrp
- 新增 base_archive base_stack 表
- 新增 系统消息，在配置文件指定用户ID