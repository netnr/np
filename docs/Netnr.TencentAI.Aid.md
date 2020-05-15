# 辅助
图片编码转换、提取页面编码词典

### 图片base64编码转换在线工具
<https://ss.netnr.com/filetobase64>

### 提取字典
- 图片识别>物体识别（物体标识定义）<https://ai.qq.com/doc/vision_object.shtml>
- 图片识别>场景识别（场景标识定义）<https://ai.qq.com/doc/vision_scene.shtml>

```
var kvs = [], arr = [];
$('table').last().find('td').each(function () { kvs.push(this.innerHTML) })
for (var i = 0; i < kvs.length; i++) {
  arr[kvs[i]] = kvs[++i]
}
console.log(arr.join(','))
```

- <https://ai.qq.com/doc/ptuimgfilter.shtml>
- 图片特效>图片滤镜（天天P图）（预设滤镜效果编码）

```
var kvs = [], arr = [];
$('table').eq(6).find('td').each(function () { kvs.push(this.innerHTML) })
for (var i = 0; i < kvs.length; i++) {
  var obj = {};
  obj.id = kvs[i];
  obj.name = kvs[++i];
  if (obj.id != "") {
    obj.id = parseInt(obj.id);
    var img = kvs[++i];
    img.replace(/\/.*?"/g, function (x) {
      obj.image_url = x.substr(0, x.length - 1)
    })
    arr.push(obj);
  }
}
arr.sort(function (a, b) { return a.id - b.id })
console.log(JSON.stringify(arr))
```

- <https://ai.qq.com/doc/ptuimgfilter.shtml>
- 图片特效>图片滤镜（AI Lab）（预设滤镜效果编码）

```
var kvs = [], arr = [];
$('table').last().find('td').each(function () { kvs.push(this.innerHTML) })
for (var i = 0; i < kvs.length; i++) {
  var img = kvs[i + 1];
  img.replace(/\/.*?"/g, function (x) {
    arr[kvs[i]] = x.substr(0, x.length - 1);
  })
  i++;
}
console.log(arr.join(','))
```

- 图片特效>人脸美妆（人脸美妆编码）<https://ai.qq.com/doc/facecosmetic.shtml>
```
var kvs = [], arr = [];
$('table').last().find('td').each(function () { kvs.push(this.innerHTML) })
for (var i = 0; i < kvs.length; i++) {
  var obj = {};
  obj.id = kvs[i];
  obj.type = kvs[++i];
  obj.name = kvs[++i];
  if (obj.id != "") {
    obj.id = parseInt(obj.id);
    var img = kvs[++i];
    img.replace(/\/.*?"/g, function (x) {
      obj.image_url = x.substr(0, x.length - 1)
    })
    arr.push(obj);
  }
}
arr.sort(function (a, b) { return a.id - b.id })
console.log(JSON.stringify(arr))
```

- 图片特效>人脸变妆（人脸变妆编码）<https://ai.qq.com/doc/facedecoration.shtml>
- 图片特效>大头贴（大头贴编码）<https://ai.qq.com/doc/facesticker.shtml>
```
var kvs = [], arr = [];
$('table').last().find('td').each(function () { kvs.push(this.innerHTML) })
for (var i = 0; i < kvs.length; i++) {
  var obj = {};
  obj.id = kvs[i];
  obj.name = kvs[++i];
  if (obj.id != "") {
    obj.id = parseInt(obj.id);
    var img = kvs[++i];
    img.replace(/\/.*?"/g, function (x) {
      obj.image_url = x.substr(0, x.length - 1)
    })
    arr.push(obj);
  }
}
arr.sort(function (a, b) { return a.id - b.id })
console.log(JSON.stringify(arr))
```