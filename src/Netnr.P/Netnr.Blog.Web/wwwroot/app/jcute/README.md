# jCute
JavaScript Library

### [jCute.all.js](view.html?jcute.all.js)

# Module


### [jCute.ajax.js](view.html?jcute.ajax.js)
```js
/**
 * ajax请求
 * @param {JQuery.AjaxSettings} settings 类似于jQuery传参
 * 
 * @param {string} url 请求地址
 * @param {string} type 请求方式，默认：GET
 * @param {boolean} async 异步请求，默认：true
 * @param {string} data 发送内容，字符串 | 键值对 | FormData
 * @param {string} contentType 内容编码类型，默认：application/x-www-form-urlencoded
 * @param {string} dataType 返回类型：json/xml/text，默认：text
 * @param {string} headers 消息头，键值对
 * @param {string} timeout 超时，单位：毫秒，默认设置超时
 * @param {string} progress function(p){ }，FormData上传进度回调方法 p：0-100
 * @param {string} success function(data,status,xhr){ }，成功回调方法，data：返回数据，
 * @param {string} error function(xhr,status){ }，错误回调方法
 * @param {string} complete:function(xhr,status){ }，完成回调
 */
jCute.ajax(settings)

```

### [jCute.convert.js](view.html?jcute.convert.js)
```js
/**
 * Unicode编码
 * @param {string} str 字符串
 */
jCute.toUnicode(str)

/**
 * Unicode解码
 * @param {string} str 字符串
 */
jCute.toUnicodeUn(str)

/**
 * Ascii编码
 * @param {string} str 字符串
 */
jCute.toAscii(str)

/**
 * Ascii解码
 * @param {string} str 字符串
 */
jCute.toAsciiUn(str)

```

### [jCute.cookie.js](view.html?jcute.cookie.js)
```js
/**
 * Cookie获取、设置、删除
 * @param {string} key 键
 * @param {string} value 值
 * @param {number} time 过期时间（默认不指定过期时间），单位：毫秒，小于0删除
 */
jCute.cookie(key, value, time)

```

### [jCute.load.js](view.html?jcute.load.js)
```js
/**
 * 载入js脚本，并回调
 * @param {string} src js脚本路径
 * @param {function} success 载入成功回调方法
 */
jCute.getScript(src, success)

/**
 * 载入css样式
 * @param {string} href css样式路径
 */
jCute.getStyle(href)

```

### [jCute.support.js](view.html?jcute.support.js)
```js
/** 
 * IE8- 
 */
jCute.oldIE()

/**
 * 移除空字符
 * @param {string} str 字符串
 */
jCute.trim(str)

/**
 * event
 * @param {event} e 事件流
 */
jCute.event(e)

/**
 * target
 * @param {event} e 事件流
 */
jCute.target(e)

/**
 * 阻止事件冒泡
 * @param {event} e 事件流
 */
jCute.stopEvent(e)

/**
 * 阻止浏览器默认行为
 * @param {event} e 事件流
 */
jCute.stopDefault(e)

/**
 * 按键ASCII值
 * @param {event} e 事件流
 */
jCute.key(e)

/**
 * 检测类型
 * @param {any} obj 对象
 */
jCute.type(obj)

```

### [jCute.xml.js](view.html?jcute.xml.js)
```js
/**
 * 解析字符串为xml
 * @param {string} data 字符串
 */
jCute.parseXML(data)

/**
 * XML转字符串
 * @param {object} xmlDoc XML对象
 */
jCute.XMLSerializer(xmlDoc)

```