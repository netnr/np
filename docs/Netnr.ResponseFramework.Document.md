# z.Grid()
方法、属性与官方的文档基本一致

```js
var gd1 = new z.Grid();

gd1.isinit  //第一次为 true ,绑定完成后始终为 false
gd1.id  //表格容器#ID，默认 #Grid1
gd1.type    //表格类型，默认 datagrid ，支持：datagrid、treegrid、propertygrid、datalist
gd1.autosize    //自动调整大小，默认 xy ，支持：xy(宽满高沉底)、x、y、p(填充父容器)
gd1.autosizePid //父容器#ID，默认 #myBody，为实现自动调整大小所需要
gd1.columnsExists   //是否查询列配置，第一次查询列配置后赋值为 true ，为true 则后面不在查询列配置
gd1.dataCache   //异步请求得到的数据

gd1.onComplete(function(obj){ })    //完成回调，可多次调用，注意：完成里面再次调用绑定会造成死循环，需标记跳出

gd1.bind()  //本地绑定方法
gd1.load()  //加载方法，载入后会自动调用 bind 方法

gd1.func()  //方法调用，与官方提供的方法一致，例子：
gd1.func('getSelected') //获取选中行
gd1.func('updateRow', {index:1, row:{}}) //更新行

//查询面板组件拓展
gd1.queryMark   //创建查询标记，默认 true
gd1.queryData   //查询条件项，默认提取标题列，可自定义（array类型）；也可以设置（虚拟）表名（string类型），即先输入查询条件再查询数据的情况，配置表名会同步请求表配置得到查询条件项
gd1.QueryBuild()    //创建查询面板（含初始化查询面板，并赋值 gd1.query ，只有 gd1.query 对象为空时才会创建 ）
gd1.QueryOpen(field)    //打开查询面板（自动调用 gd1.QueryBuild 方法）; field，可选，指向查询条件字段名，定位行和启用编辑
gd1.QueryWhere()    //获取查询面板的条件
gd1.QueryOk = function () { }   //点击查询面板确定的回调，可选，自定义确定事件，不写该事件则默认获取查询面板条件请求第一页
gd1.query   //query对象为查询面板对象；需调用 gd1.QueryBuild() 或 gd1.QueryOpen()方法后才会有值
z.queryin   //指向打开的查询面板,调用 gd1.QueryOpen() 方法时，z.queryin = gd1.query，打开其他的查询面板会对应的指向
gd1.query.grid  //查询面板表格，与 gd1 同类型对象
gd1.query.id    //指向查询面板表格的容器ID
gd1.query.md    //查询面板模态框对象，new z.Modal() 返回的对象
```

查询面板组件拓展

### z.GridQueryMark(gd)

> 创建查询标记

### z.GridQueryBuild(gd)

> 创建查询面板

### z.GridQueryOpen(gd, field)

> 打开查询面板，自动调用 GridQueryBuild 创建方法，参数：field 指定某个查询列字段，可选

### z.GridQueryWhere(gd)

> 得到查询面板的查询条件，组建 z.SqlQuery 对象



### z.GridFormat()

> 公用的格式化方法，如性别、日期、状态、金额等

### z.GridAuto(gd)

> Grid调整大小的方法

### z.GridLoading(gd)

> 第一次加载表格时，显示加载图标，以后的加载提示由 DataGrid 组件提供

### z.GridEditor(gd, index, field, row)

> Grid编辑配置，参数 row 可选，具体看脚本注释

### z.GridEditorBlank(gd)

> 点空白结束Grid编辑


### z.Combo()

> 方法、属性与官方的文档基本一致，与 z.Grid 一样的形式

```js
var cb1 = new z.Combo();

cb1.type    //类型，默认 combobox ，支持：combobox、combotree tree

cb1.onComplete(function(obj){ })    //完成回调，可多次调用，注意：完成里面再次调用绑定会造成死循环，需标记跳出

cb1.bind()  //本地绑定方法
cb1.load()  //加载方法，载入后会自动调用 bind 方法

cb1.func()  //方法调用，与官方提供的方法一致，例子：
cb1.func('getValue') //取值
cb1.func('setValue', 1) //赋值
```

### z.TreeVagueCheck(cb, values)

> Tree模糊选中，用于一个节点的子节点有部分选中，赋值后子节点被全部选中，用此方法处理，示例参考角色权限设置

### z.FormAttrAjax()

> formui表单 请求返回数据源&回调，用于初始化异步请求绑定的数据来源，与 z.FormAttrBind 方法配合使用

### z.FormAttrBind(target)

> formui表单 类型源绑定，用于初始化根据不同的类型调用对应的组件方法 与 z.FormAttrAjax 方法配合使用

### z.FormRequired(color, FormId, dialog)

> 表单必填验证，用于保存时

### z.FindTreeNode(data, value, key)

> 查找树节点

### z.FormEdit(rowData)

> 回填表单，用于选中表格的一行，直接赋值表单编辑

### z.FormToJson(FormId)

> 表单转为 JSON , 用于编辑保存后，不用刷新加载，直接获取表单值更新表格的一行数据

### z.FormClear(FormId)

> 清空表单，用于新增

### z.FormDisabled(dd, FormId)

> 禁用表单，用于查看

### z.FormAutoHeight()

> 模态框表单调整高度，用于模态框高度自适应

### z.FormTitle(ops)

> 表单标题设置 ops示例：{id:"#id", title:"新增", required:true}
* icon 标题图标
* title 标题
* id 标题容器ID或对象
* required 是否显示必填的提示文字，默认 true

### z.Modal()

> 创建模态框

```js
var md1 = new z.Modal();

md1.okText  //ok按钮文本        
md1.cancelText  //cancel按钮文本        
md1.okClick //Ok点击回调
        
md1.cancelClick //Cancel点击回调
md1.title   //标题内容
md1.content //主体内容
md1.src //iframe地址，覆盖content属性
md1.heightIframe    //iframe高度
md1.complete    //iframe加载完成回调
md1.size    //模态大小 默认2 可选：1|2|3|4 ;分别对应（sm、md、lg、full）
md1.showClose   //显示右上角关闭按钮
md1.showFooter  //显示页脚
md1.showCancel  //显示Cancel按钮

md1.okClick = function(){}   //确定事件
md1.cancelClick = function(){}   //取消

md1.append()    //追加到 body 上，改方法在最后属性赋值后调用
md1.show()  //显示
md1.hide()  //隐藏
md1.remove()    //移除

md1.modal   //指向模态框的jQuery对象，如：md1.modal.find('div.modal-body') 找到模态框内容主体
```

### z.SqlQuery()

> 用于表示SQL查询条件的对象

```
// id1='1' AND id2 IN('1','2') AND id2 LIKE '%5%' AND id3>='5' AND id3<='10'
[
    {
        field: "id1",
        relation: "Equal",
        value: 1
    },
    {
        field: "id2",
        relation: "In",
        value: [1, 2]
    },
    {
        field: "id2",
        relation: "Contains",
        value: "5"
    },
    {
        field: "id3",
        relation: "BetweenAnd",
        value: [5, 10]
    }
]

// relation 关系符说明
{
    Equal: '{0} = {1}',
    NotEqual: '{0} != {1}',
    LessThan: '{0} < {1}',
    GreaterThan: '{0} > {1}',
    LessThanOrEqual: '{0} <= {1}',
    GreaterThanOrEqual: '{0} >= {1}',
    BetweenAnd: '{0} >= {1} AND {0} <= {2}',
    Contains: '%{0}%',
    StartsWith: '{0}%',
    EndsWith: '%{0}',
    In: 'IN',
    NotIn: 'NOT IN'
}
```

### z.DC

> 页面数据缓存，包括组件、数据源等，所有的东西都在里面
* 下拉列表，根据请求url的地址小写作为键，存储下拉列表组件的信息

### z.btnTrigger

> 按钮触发标识，点击对应的功能按钮，赋值对应按钮的ID

### z.button(type, fn)

> 点击按钮事件（如需要禁用某些功能按钮，为按钮添加 disabled 样式即可生效，而不是设置按钮禁用属性），不适用二级按钮（弹出的下拉菜单按钮）
```js
z.button('add',functin(){ console.log('新增事件') })
```

### z.buttonClick(type)

> 模拟操作按钮点击，如：z.buttonClick('add') 模拟点击新增

### art(content, fnOk, fnCacle)

> 消息提示、询问提示，依赖于 z.Modal
```js
art('保存成功');    //类似于alert方法
//有几个关键词做了转换：fail、error、success、select
art('fail') //操作失败，一般用于返回的结果是失败
art('error')    //网络错误，一般用于异步请求error事件
art('success')  //操作成功，一般用于保存成功
//为所有提示保持风格统一，避免一些：操作有误、操作失败、 系统错误等各种提示

art('确定删除吗？', function(){
    //确定，发起删除请求
})
art('是否覆盖？', function(){
    //覆盖
},function(){
    //不覆盖
})
```