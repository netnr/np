# Netnr.TencentAI
Tencent AI SDK

# 1.0.0-Beta1发布，测试项目返回 “待测试” 字样的方法如有问题需等待版本更新

### 安装 (NuGet)
```
Install-Package Netnr.TencentAI
```

### 方法命名
- 根据接口/分割，最后是方法名，前面的是类、子类
- 在使用时，看API地址就知道怎么调用方法
- 如：https://api.ai.qq.com/fcgi-bin/ocr/ocr_generalocr
- 调用：`fcgi_bin.Ocr.Ocr_GeneralOcr()`

### [辅助-提取页面字典](Netnr.TencentAI.Aid.md)
- 词典、键值对 `Static.Dic.对应方法名_编码字段()`
- 例如：`Static.Dic.Vision_Scener_Label()`
- 返回为数组时，索引为对应的编码

### 接口
```
{
  "敏感信息审核": {
    "Aai_EvilAudio": "音频鉴黄/敏感词检测【待测试】",
    "Image_Terrorism": "暴恐识别",
    "Vision_Porn": "图片鉴黄"
  },
  "语音识别": {
    "Aai_Asr": "语音识别-echo版【待测试】",
    "Aai_Asrs": "语音识别-流式版（AI Lab）【待测试】",
    "Aai_WxAsrs": "流式版(WeChat AI)【待测试】",
    "Aai_WxAsrLong": " 长语音识别【待测试】",
    "Aai_DetectKeyWord": "关键词检索【待测试】"
  },
  "语音合成": {
    "Aai_Tts": "语音合成（AI Lab）",
    "Aai_Tta": "语音合成（优图）"
  },
  "人脸与人体识别": {
    "Face_DetectFace": "人脸检测与分析",
    "Face_DetectMultiFace": "多人脸检测",
    "Face_DetectCrossAgeFace": "跨年 龄人脸识别",
    "Face_FaceShape": "五官定位",
    "Face_FaceCompare": "人脸对比",
    "Face_FaceIdentify": "人脸搜索 > 人脸搜索",
    "Face_NewPerson": "人脸搜索 > 个体创建",
    "Face_DelPerson": "人脸搜索 > 删除个体",
    "Face_AddFace": "人脸搜索 > 增加人脸",
    "Face_DelFace": "人脸搜索 > 删除人脸",
    "Face_SetInfo": "人脸搜索 > 设置信息【待测试】",
    "Face_GetInfo": "人脸搜索 > 获取信息",
    "Face_GetGroupIds": "人脸搜索 > 获取组列表",
    "Face_GetPersonIds": "人脸搜索 > 获取个体列表",
    "Face_GetFaceIds": "人脸搜索 > 获取人脸列表",
    "Face_GetFaceInfo": "人脸搜索 > 获取人脸信息",
    "Face_FaceVerify": "人脸验证【待测试】"
  },
  "图片识别": {
    "Image_Tag": "多标签识别",
    "Image_Fuzzy": "模糊图片检测",
    "Image_Food": "美食图片识别",
    "Vision_ImgToText": "看图说话",
    "Vision_Scener": "场景识别",
    "Vision_Objectr": "物体识别"
  },
  "智能闲聊": {
    "Nlp_TextChat": "智能闲聊"
  },
  "机器翻译": {
    "Nlp_TextTrans": "文本翻译（AI Lab）",
    "Nlp_TextTranslate": "文本翻译（翻译君）",
    "Nlp_SpeechTranslate": "语音翻译",
    "Nlp_ImageTranslate": "图片翻译",
    "Nlp_TextDetect": "语种识别"
  },
  "基础文本 分析": {
    "Nlp_WordSeg": "分词",
    "Nlp_WordPos": "词性",
    "Nlp_WordNer": "专有名词",
    "Nlp_WordSyn": "同义词"
  },
  "语义解析": {
    "Nlp_WordCom": "意图成分",
    "Nlp_TextPolar": "情感分析"
  },
  "OCR": {
    "Ocr_IdCardOcr": "身份证OCR",
    "Ocr_DriverLicenseOcr": "行驶证驾驶证OCR",
    "Ocr_GeneralOcr": "通用OCR",
    "Ocr_BizLicenseOcr": "营业执照OCR",
    "Ocr_CreditCardOcr": "银行卡OCR",
    "Ocr_HandWritingOcr": "手写体OCR",
    "Ocr_PlateOcr": "车牌体OCR",
    "Ocr_BcOcr": "名片OCR"
  },
  "图片特效": {
    "Ptu_ImgFilter": "图片滤镜（天天P图）,更适合人物图片",
    "Ptu_FaceCosmetic": "人脸美妆",
    "Ptu_FaceDecoration": "人脸变妆",
    "Ptu_FaceSticker": "大头贴",
    "Ptu_FaceAge": "颜龄检测",
    "Vision_ImgFilter": "图片滤镜（AI Lab）,更适合风景图片"
  }
}
```

### 框架
- .NETStandard 2.0
- .NETFramework 4.0