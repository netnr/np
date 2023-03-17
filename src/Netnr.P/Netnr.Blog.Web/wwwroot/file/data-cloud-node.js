let dataCloudNode = [
    {
        "name": "AWS", "alias": "亚马逊云", "update": "2023-01-18", "link": "https://aws.amazon.com/",
        "list": [{ "area": "us-east-1", "alias": "North Virginia", "endpoint": "https://dynamodb.us-east-1.amazonaws.com/" }, { "area": "us-west-1", "alias": "North California", "endpoint": "https://dynamodb.us-west-1.amazonaws.com/" }, { "area": "ca-central-1", "alias": "Central", "endpoint": "https://dynamodb.ca-central-1.amazonaws.com/" }, { "area": "sa-east-1", "alias": "São Paulo", "endpoint": "https://dynamodb.sa-east-1.amazonaws.com/" }, { "area": "eu-west-1", "alias": "Ireland", "endpoint": "https://dynamodb.eu-west-1.amazonaws.com/" }, { "area": "af-south-1", "alias": "Cape Town", "endpoint": "https://dynamodb.af-south-1.amazonaws.com/" }, { "area": "me-south-1", "alias": "Bahrain", "endpoint": "https://dynamodb.me-south-1.amazonaws.com/" }, { "area": "ap-northeast-1", "alias": "Tokyo", "endpoint": "https://dynamodb.ap-northeast-1.amazonaws.com/" }, { "area": "cn-north-1", "alias": "Beijing", "endpoint": "https://dynamodb.cn-north-1.amazonaws.com/" }]
    }, {
        "name": "Aliyun", "alias": "阿里云", "update": "2023-01-18", "link": "https://www.aliyun.com/",
        "list": [{ "area": "华北1", "alias": "青岛", "endpoint": "https://oss-cn-qingdao.aliyuncs.com/" }, { "area": "华北2", "alias": "北京", "endpoint": "https://oss-cn-beijing.aliyuncs.com/" }, { "area": "华北3", "alias": "张家口", "endpoint": "https://oss-cn-zhangjiakou.aliyuncs.com/" }, { "area": "华北5", "alias": "呼和浩特", "endpoint": "https://oss-cn-huhehaote.aliyuncs.com/" }, { "area": "华北6", "alias": "乌兰察布", "endpoint": "https://oss-cn-wulanchabu.aliyuncs.com/" }, { "area": "华东1", "alias": "杭州", "endpoint": "https://oss-cn-hangzhou.aliyuncs.com/" }, { "area": "华东2", "alias": "上海", "endpoint": "https://oss-cn-shanghai.aliyuncs.com/" }, { "area": "华东5 ", "alias": "南京-本地地域", "endpoint": "https://oss-cn-nanjing.aliyuncs.com/" }, { "area": "华东6", "alias": "福州-本地地域", "endpoint": "https://oss-cn-fuzhou.aliyuncs.com/" }, { "area": "华南1", "alias": "深圳", "endpoint": "https://oss-cn-shenzhen.aliyuncs.com/" }, { "area": "华南2", "alias": "河源", "endpoint": "https://oss-cn-heyuan.aliyuncs.com/" }, { "area": "华南3", "alias": "广州", "endpoint": "https://oss-cn-guangzhou.aliyuncs.com/" }, { "area": "西南1", "alias": "成都", "endpoint": "https://oss-cn-chengdu.aliyuncs.com/" }, { "area": "中国香港", "alias": "", "endpoint": "https://oss-cn-hongkong.aliyuncs.com/" }, { "area": "新加坡", "alias": "", "endpoint": "https://oss-ap-southeast-1.aliyuncs.com/" }, { "area": "澳大利亚", "alias": "悉尼", "endpoint": "https://oss-ap-southeast-2.aliyuncs.com/" }, { "area": "马来西亚", "alias": "吉隆坡", "endpoint": "https://oss-ap-southeast-3.aliyuncs.com/" }, { "area": "印度尼西亚", "alias": "雅加达", "endpoint": "https://oss-ap-southeast-5.aliyuncs.com/" }, { "area": "菲律宾", "alias": "马尼拉", "endpoint": "https://oss-ap-southeast-6.aliyuncs.com/" }, { "area": "泰国", "alias": "曼谷", "endpoint": "https://oss-ap-southeast-7.aliyuncs.com/" }, { "area": "印度", "alias": "孟买", "endpoint": "https://oss-ap-south-1.aliyuncs.com/" }, { "area": "日本", "alias": "东京", "endpoint": "https://oss-ap-northeast-1.aliyuncs.com/" }, { "area": "韩国", "alias": "首尔", "endpoint": "https://oss-ap-northeast-2.aliyuncs.com/" }, { "area": "美国", "alias": "硅谷", "endpoint": "https://oss-us-west-1.aliyuncs.com/" }, { "area": "美国", "alias": "弗吉尼亚", "endpoint": "https://oss-us-east-1.aliyuncs.com/" }, { "area": "德国", "alias": "法兰克福", "endpoint": "https://oss-eu-central-1.aliyuncs.com/" }, { "area": "英国", "alias": "伦敦", "endpoint": "https://oss-eu-west-1.aliyuncs.com/" }, { "area": "阿联酋", "alias": "迪拜", "endpoint": "https://oss-me-east-1.aliyuncs.com/" }, { "area": "华东1 金融云", "alias": "", "endpoint": "https://oss-cn-hangzhou.aliyuncs.com/" }, { "area": "华东2 金融云", "alias": "", "endpoint": "https://oss-cn-shanghai-finance-1.aliyuncs.com/" }, { "area": "华南1 金融云", "alias": "", "endpoint": "https://oss-cn-shenzhen-finance-1.aliyuncs.com/" }, { "area": "华北2 金融云", "alias": "", "endpoint": "https://oss-cn-beijing-finance-1.aliyuncs.com/" }, { "area": "华北2 阿里政务云1", "alias": "", "endpoint": "https://oss-cn-north-2-gov-1.aliyuncs.com/" }]
    }, {
        "name": "Baidu", "alias": "百度云", "update": "2023-01-18", "link": "https://cloud.baidu.com",
        "list": [{ "area": "北京", "alias": "北京", "endpoint": "https://bj.bcebos.com/" }, { "area": "保定", "alias": "保定", "endpoint": "https://bd.bcebos.com/" }, { "area": "苏州", "alias": "苏州", "endpoint": "https://su.bcebos.com/" }, { "area": "广州", "alias": "广州", "endpoint": "https://gz.bcebos.com/" }, { "area": "香港", "alias": "香港", "endpoint": "https://hkg.bcebos.com/" }, { "area": "金融云武汉专区", "alias": "金融云武汉专区", "endpoint": "https://fwh.bcebos.com/" }, { "area": "金融云上海专区", "alias": "金融云上海专区", "endpoint": "https://fsh.bcebos.com/" }]
    }, {
        "name": "Tencent", "alias": "腾讯云", "update": "2023-01-18", "link": "https://cloud.tencent.com/",
        "list": [{ "area": "华南地区", "alias": "广州", "endpoint": "https://cos.ap-guangzhou.myqcloud.com/" }, { "area": "华南地区", "alias": "深圳金融", "endpoint": "https://cos.ap-shenzhen-fsi.myqcloud.com/" }, { "area": "华东地区", "alias": "上海", "endpoint": "https://cos.ap-shanghai.myqcloud.com/" }, { "area": "华东地区", "alias": "上海金融", "endpoint": "https://cos.ap-shanghai-fsi.myqcloud.com/" }, { "area": "华东地区", "alias": "南京", "endpoint": "https://cos.ap-nanjing.myqcloud.com/" }, { "area": "华北地区", "alias": "北京", "endpoint": "https://cos.ap-beijing.myqcloud.com/" }, { "area": "西南地区", "alias": "成都", "endpoint": "https://cos.ap-chengdu.myqcloud.com/" }, { "area": "西南地区", "alias": "重庆", "endpoint": "https://cos.ap-chongqing.myqcloud.com/" }, { "area": "港澳台地区", "alias": "中国香港", "endpoint": "https://cos.ap-hongkong.myqcloud.com/" }, { "area": "亚太东南", "alias": "新加坡", "endpoint": "https://cos.ap-singapore.myqcloud.com/" }, { "area": "亚太东南", "alias": "雅加达", "endpoint": "https://cos.ap-jakarta.myqcloud.com/" }, { "area": "亚太东北", "alias": "首尔", "endpoint": "https://cos.ap-seoul.myqcloud.com/" }, { "area": "亚太东北", "alias": "东京", "endpoint": "https://cos.ap-tokyo.myqcloud.com/" }, { "area": "亚太南部", "alias": "孟买", "endpoint": "https://cos.ap-mumbai.myqcloud.com/" }, { "area": "亚太东南", "alias": "曼谷", "endpoint": "https://cos.ap-bangkok.myqcloud.com/" }, { "area": "北美地区", "alias": "多伦多", "endpoint": "https://cos.na-toronto.myqcloud.com/" }, { "area": "南美地区", "alias": "圣保罗", "endpoint": "https://cos.sa-saopaulo.myqcloud.com/" }, { "area": "美国西部", "alias": "硅谷", "endpoint": "https://cos.na-siliconvalley.myqcloud.com/" }, { "area": "美国东部", "alias": "弗吉尼亚", "endpoint": "https://cos.na-ashburn.myqcloud.com/" }, { "area": "欧洲地区", "alias": "法兰克福", "endpoint": "https://cos.eu-frankfurt.myqcloud.com/" }, { "area": "欧洲地区", "alias": "莫斯科", "endpoint": "https://cos.eu-moscow.myqcloud.com/" }]
    }, {
        "name": "Huawei", "alias": "华为云", "update": "2023-01-18", "link": "https://www.huaweicloud.com/",
        "list": [{ "area": "非洲", "alias": "约翰内斯堡", "endpoint": "https://obs.af-south-1.myhuaweicloud.com/" }, { "area": "华北", "alias": "北京四", "endpoint": "https://obs.cn-north-4.myhuaweicloud.com/" }, { "area": "华北", "alias": "北京一", "endpoint": "https://obs.cn-north-1.myhuaweicloud.com/" }, { "area": "华北", "alias": "乌兰察布一", "endpoint": "https://obs.cn-north-9.myhuaweicloud.com/" }, { "area": "华东", "alias": "上海二", "endpoint": "https://obs.cn-east-2.myhuaweicloud.com/" }, { "area": "华东", "alias": "上海一", "endpoint": "https://obs.cn-east-3.myhuaweicloud.com/" }, { "area": "华南", "alias": "广州", "endpoint": "https://obs.cn-south-1.myhuaweicloud.com/" }, { "area": "拉美", "alias": "墨西哥城二", "endpoint": "https://obs.la-north-2.myhuaweicloud.com/" }, { "area": "拉美", "alias": "墨西哥城一", "endpoint": "https://obs.na-mexico-1.myhuaweicloud.com/" }, { "area": "拉美", "alias": "圣保罗一", "endpoint": "https://obs.sa-brazil-1.myhuaweicloud.com/" }, { "area": "拉美", "alias": "圣地亚哥", "endpoint": "https://obs.la-south-2.myhuaweicloud.com/" }, { "area": "西南", "alias": "贵阳一", "endpoint": "https://obs.cn-southwest-2.myhuaweicloud.com/" }, { "area": "亚太", "alias": "曼谷", "endpoint": "https://obs.ap-southeast-2.myhuaweicloud.com/" }, { "area": "亚太", "alias": "新加坡", "endpoint": "https://obs.ap-southeast-3.myhuaweicloud.com/" }, { "area": "中国", "alias": "香港", "endpoint": "https://obs.ap-southeast-1.myhuaweicloud.com/" }]
    }, {
        "name": "UCloud", "alias": "优刻得", "update": "2023-01-18", "link": "https://www.ucloud.cn",
        "list": [{ "area": "华北", "alias": "cn-bj", "endpoint": "https://www.cn-bj.ufileos.com/" }, { "area": "广州", "alias": "cn-gd", "endpoint": "https://www.cn-gd.ufileos.com/" }, { "area": "福建", "alias": "cn-qz", "endpoint": "https://www.cn-qz.ufileos.com/" }, { "area": "上海", "alias": "cn-sh", "endpoint": "https://www.cn-sh.ufileos.com/" }, { "area": "华北", "alias": "cn-wlcb", "endpoint": "https://www.cn-wlcb.ufileos.com/" }, { "area": "拉各斯", "alias": "afr-nigeria", "endpoint": "https://www.afr-nigeria.ufileos.com/" }, { "area": "圣保罗", "alias": "bra-saopaulo", "endpoint": "https://www.bra-saopaulo.ufileos.com/" }, { "area": "法兰克福", "alias": "ge-fra", "endpoint": "https://www.ge-fra.ufileos.com/" }, { "area": "香港", "alias": "hk", "endpoint": "https://www.hk.ufileos.com/" }, { "area": "雅加达", "alias": "idn-jakarta", "endpoint": "https://www.idn-jakarta.ufileos.com/" }, { "area": "孟买", "alias": "ind-mumbai", "endpoint": "https://www.ind-mumbai.ufileos.com/" }, { "area": "东京", "alias": "jpn-tky", "endpoint": "https://www.jpn-tky.ufileos.com/" }, { "area": "首尔", "alias": "kr-seoul", "endpoint": "https://www.kr-seoul.ufileos.com/" }, { "area": "马尼拉", "alias": "ph-mnl", "endpoint": "https://www.ph-mnl.ufileos.com/" }, { "area": "莫斯科", "alias": "rus-mosc", "endpoint": "https://www.rus-mosc.ufileos.com/" }, { "area": "新加坡", "alias": "sg", "endpoint": "https://www.sg.ufileos.com/" }, { "area": "曼谷", "alias": "th-bkk", "endpoint": "https://www.th-bkk.ufileos.com/" }, { "area": "台北", "alias": "tw-tp", "endpoint": "https://www.tw-tp.ufileos.com/" }, { "area": "迪拜", "alias": "uae-dubai", "endpoint": "https://www.uae-dubai.ufileos.com/" }, { "area": "伦敦", "alias": "uk-london", "endpoint": "https://www.uk-london.ufileos.com/" }, { "area": "洛杉矶", "alias": "us-ca", "endpoint": "https://www.us-ca.ufileos.com/" }, { "area": "华盛顿", "alias": "us-ws", "endpoint": "https://www.us-ws.ufileos.com/" }, { "area": "胡志明市", "alias": "vn-sng", "endpoint": "https://www.vn-sng.ufileos.com/" }]
    }, {
        "name": "Ksyun", "alias": "金山云", "update": "2023-01-18", "link": "https://www.ksyun.com",
        "list": [{ "area": "北京", "alias": "BEIJING", "endpoint": "https://ks3-cn-beijing.ksyuncs.com/" }, { "area": "上海", "alias": "SHANGHAI", "endpoint": "https://ks3-cn-shanghai.ksyuncs.com/" }, { "area": "广州", "alias": "GUANGZHOU", "endpoint": "https://ks3-cn-guangzhou.ksyuncs.com/" }, { "area": "香港", "alias": "HONGKONG", "endpoint": "https://ks3-cn-hk-1.ksyuncs.com/" }, { "area": "俄罗斯", "alias": "RUSSIA", "endpoint": "https://ks3-rus.ksyuncs.com/" }, { "area": "新加坡", "alias": "SINGAPORE", "endpoint": "https://ks3-sgp.ksyuncs.com/" }, { "area": "北京", "alias": "JR_BEIJING", "endpoint": "https://ks3-jr-beijing.ksyuncs.com/" }, { "area": "上海", "alias": "JR_SHANGHAI", "endpoint": "https://ks3-jr-shanghai.ksyuncs.com/" }, { "area": "北京", "alias": "GOV_BEIJING", "endpoint": "https://ks3-gov-beijing.ksyuncs.com/" }]
    }, {
        "name": "Qiniu", "alias": "七牛云", "update": "2023-01-18", "link": "https://www.qiniu.com/",
        "list": [{ "area": "华东", "alias": "浙江", "endpoint": "https://upload-z0.qiniup.com/" }, { "area": "华东", "alias": "浙江2", "endpoint": "https://upload-cn-east-2.qiniup.com/" }, { "area": "华北", "alias": "河北", "endpoint": "https://upload-z1.qiniup.com/" }, { "area": "华南", "alias": "广东", "endpoint": "https://upload-z2.qiniup.com/" }, { "area": "北美", "alias": "洛杉矶", "endpoint": "https://upload-na0.qiniup.com/" }, { "area": "亚太", "alias": "新加坡（原东南亚）", "endpoint": "https://upload-as0.qiniup.com/" }, { "area": "亚太", "alias": "首尔", "endpoint": "https://upload-ap-northeast-1.qiniup.com/" }]
    }, {
        "name": "QingCloud", "alias": "青云", "update": "2023-01-18", "link": "https://www.qingcloud.com",
        "list": [{ "area": "北京3区-A", "alias": "pek3a", "endpoint": "https://pek3a.qingstor.com/" }, { "area": "上海1区-A", "alias": "sh1a", "endpoint": "https://sh1a.qingstor.com/" }, { "area": "北京3区-B", "alias": "pek3b", "endpoint": "https://pek3b.qingstor.com/" }, { "area": "广东2区", "alias": "gd2", "endpoint": "https://gd2.qingstor.com/" }, { "area": "雅加达区", "alias": "ap3", "endpoint": "https://ap3.qingstor.com/" }]
    }, {
        "name": "Microsoft", "alias": "微软云", "update": "2023-01-18", "link": "https://azure.microsoft.com/",
        "list": [{ "area": "Australia Central", "alias": "Canberra", "endpoint": "https://australiacentral.blob.core.windows.net" }, { "area": "Australia Central 2", "alias": "Canberra", "endpoint": "https://australiacentral2.blob.core.windows.net" }, { "area": "Australia Southeast", "alias": "New South Wales", "endpoint": "https://australiaeast.blob.core.windows.net" }, { "area": "Australia East", "alias": "Victoria", "endpoint": "https://australiasoutheast.blob.core.windows.net" }, { "area": "South Central US", "alias": "Sao Paulo State", "endpoint": "https://brazilsouth.blob.core.windows.net" }, { "area": "Brazil South", "alias": "Rio", "endpoint": "https://brazilsoutheast.blob.core.windows.net" }, { "area": "Canada East", "alias": "Toronto", "endpoint": "https://canadacentral.blob.core.windows.net" }, { "area": "Canada Central", "alias": "Quebec", "endpoint": "https://canadaeast.blob.core.windows.net" }, { "area": "South India", "alias": "Pune", "endpoint": "https://centralindia.blob.core.windows.net" }, { "area": "East US 2", "alias": "Iowa", "endpoint": "https://centralus.blob.core.windows.net" }, { "area": "Southeast Asia", "alias": "Hong Kong", "endpoint": "https://eastasia.blob.core.windows.net" }, { "area": "West US", "alias": "Virginia", "endpoint": "https://eastus.blob.core.windows.net" }, { "area": "Central US", "alias": "Virginia", "endpoint": "https://eastus2.blob.core.windows.net" }, { "area": "France South", "alias": "Paris", "endpoint": "https://francecentral.blob.core.windows.net" }, { "area": "France Central", "alias": "Marseille", "endpoint": "https://francesouth.blob.core.windows.net" }, { "area": "Germany West Central", "alias": "Berlin", "endpoint": "https://germanynorth.blob.core.windows.net" }, { "area": "Germany North", "alias": "Frankfurt", "endpoint": "https://germanywestcentral.blob.core.windows.net" }, { "area": "Japan West", "alias": "Tokyo, Saitama", "endpoint": "https://japaneast.blob.core.windows.net" }, { "area": "Japan East", "alias": "Osaka", "endpoint": "https://japanwest.blob.core.windows.net" }, { "area": "Jio India West", "alias": "Nagpur", "endpoint": "https://jioindiacentral.blob.core.windows.net" }, { "area": "Jio India Central", "alias": "Jamnagar", "endpoint": "https://jioindiawest.blob.core.windows.net" }, { "area": "Korea South", "alias": "Seoul", "endpoint": "https://koreacentral.blob.core.windows.net" }, { "area": "Korea Central", "alias": "Busan", "endpoint": "https://koreasouth.blob.core.windows.net" }, { "area": "South Central US", "alias": "Illinois", "endpoint": "https://northcentralus.blob.core.windows.net" }, { "area": "West Europe", "alias": "Ireland", "endpoint": "https://northeurope.blob.core.windows.net" }, { "area": "Norway West", "alias": "Norway", "endpoint": "https://norwayeast.blob.core.windows.net" }, { "area": "Norway East", "alias": "Norway", "endpoint": "https://norwaywest.blob.core.windows.net" }, { "area": "South Africa West", "alias": "Johannesburg", "endpoint": "https://southafricanorth.blob.core.windows.net" }, { "area": "South Africa North", "alias": "Cape Town", "endpoint": "https://southafricawest.blob.core.windows.net" }, { "area": "North Central US", "alias": "Texas", "endpoint": "https://southcentralus.blob.core.windows.net" }, { "area": "East Asia", "alias": "Singapore", "endpoint": "https://southeastasia.blob.core.windows.net" }, { "area": "Central India", "alias": "Chennai", "endpoint": "https://southindia.blob.core.windows.net" }, { "area": null, "alias": "GÃ¤vle", "endpoint": "https://swedencentral.blob.core.windows.net" }, { "area": "Switzerland West", "alias": "Zurich", "endpoint": "https://switzerlandnorth.blob.core.windows.net" }, { "area": "Switzerland North", "alias": "Geneva", "endpoint": "https://switzerlandwest.blob.core.windows.net" }, { "area": "UAE North", "alias": "Abu Dhabi", "endpoint": "https://uaecentral.blob.core.windows.net" }, { "area": "UAE Central", "alias": "Dubai", "endpoint": "https://uaenorth.blob.core.windows.net" }, { "area": "UK West", "alias": "London", "endpoint": "https://uksouth.blob.core.windows.net" }, { "area": "UK South", "alias": "Cardiff", "endpoint": "https://ukwest.blob.core.windows.net" }, { "area": "West US 2", "alias": "Wyoming", "endpoint": "https://westcentralus.blob.core.windows.net" }, { "area": "North Europe", "alias": "Netherlands", "endpoint": "https://westeurope.blob.core.windows.net" }, { "area": "South India", "alias": "Mumbai", "endpoint": "https://westindia.blob.core.windows.net" }, { "area": "East US", "alias": "California", "endpoint": "https://westus.blob.core.windows.net" }, { "area": "West Central US", "alias": "Washington", "endpoint": "https://westus2.blob.core.windows.net" }, { "area": "East US", "alias": "Phoenix", "endpoint": "https://westus3.blob.core.windows.net" }, { "area": "", "alias": "", "endpoint": "https://chinaeast.blob.core.windows.net" }, { "area": "", "alias": "", "endpoint": "https://chinaeast2.blob.core.windows.net" }, { "area": "", "alias": "", "endpoint": "https://chinaeast3.blob.core.windows.net" }, { "area": "", "alias": "", "endpoint": "https://chinanorth.blob.core.windows.net" }, { "area": "", "alias": "", "endpoint": "https://chinanorth2.blob.core.windows.net" }, { "area": "", "alias": "", "endpoint": "https://chinanorth3.blob.core.windows.net" }]
    }, {
        "name": "Google", "alias": "谷歌云", "update": "2023-01-18", "link": "https://cloud.google.com/",
        "list": [{ "area": "asia-east1", "alias": "台湾彰化", "endpoint": "https://asia-east1-gce.cloudharmony.net/" }, { "area": "asia-east2", "alias": "香港", "endpoint": "https://asia-east2-gce.cloudharmony.net/" }, { "area": "asia-northeast1", "alias": "日本东京", "endpoint": "https://asia-northeast1-gce.cloudharmony.net/" }, { "area": "asia-northeast2", "alias": "日本大阪", "endpoint": "https://asia-northeast2-gce.cloudharmony.net/" }, { "area": "asia-northeast3", "alias": "韩国首尔", "endpoint": "https://asia-northeast3-gce.cloudharmony.net/" }, { "area": "asia-south1", "alias": "印度孟买", "endpoint": "https://asia-south1-gce.cloudharmony.net/" }, { "area": "asia-south2", "alias": "印度德里", "endpoint": "https://asia-south2-gce.cloudharmony.net/" }, { "area": "asia-southeast1", "alias": "新加坡裕廊西", "endpoint": "https://asia-southeast1-gce.cloudharmony.net/" }, { "area": "asia-southeast2", "alias": "印度尼西亚雅加达", "endpoint": "https://asia-southeast2-gce.cloudharmony.net/" }, { "area": "australia-southeast1", "alias": "澳大利亚悉尼", "endpoint": "https://australia-southeast1-gce.cloudharmony.net/" }, { "area": "australia-southeast2", "alias": "澳大利亚墨尔本", "endpoint": "https://australia-southeast2-gce.cloudharmony.net/" }, { "area": "europe-north1", "alias": "芬兰哈米纳", "endpoint": "https://europe-north1-gce.cloudharmony.net/" }, { "area": "europe-central2", "alias": "波兰华沙", "endpoint": "https://europe-central2-gce.cloudharmony.net/" }, { "area": "europe-southwest1", "alias": "西班牙马德里", "endpoint": "https://europe-southwest1-gce.cloudharmony.net/" }, { "area": "europe-west1", "alias": "比利时圣吉斯兰", "endpoint": "https://europe-west1-gce.cloudharmony.net/" }, { "area": "europe-west2", "alias": "英国伦敦", "endpoint": "https://europe-west2-gce.cloudharmony.net/" }, { "area": "europe-west3", "alias": "德国法兰克福", "endpoint": "https://europe-west3-gce.cloudharmony.net/" }, { "area": "europe-west4", "alias": "荷兰埃姆斯哈文", "endpoint": "https://europe-west4-gce.cloudharmony.net/" }, { "area": "europe-west6", "alias": "瑞士苏黎世", "endpoint": "https://europe-west6-gce.cloudharmony.net/" }, { "area": "europe-west8", "alias": "意大利米兰", "endpoint": "https://europe-west8-gce.cloudharmony.net/" }, { "area": "europe-west9", "alias": "法国巴黎", "endpoint": "https://europe-west9-gce.cloudharmony.net/" }, { "area": "northamerica-northeast1", "alias": "魁北克省蒙特利尔", "endpoint": "https://northamerica-northeast1-gce.cloudharmony.net/" }, { "area": "northamerica-northeast2", "alias": "安大略省多伦多", "endpoint": "https://northamerica-northeast2-gce.cloudharmony.net/" }, { "area": "southamerica-east1", "alias": "巴西圣保罗省奥萨斯库", "endpoint": "https://southamerica-east1-gce.cloudharmony.net/" }, { "area": "southamerica-west1", "alias": "智利圣地亚哥", "endpoint": "https://southamerica-west1-gce.cloudharmony.net/" }, { "area": "us-central1", "alias": "爱荷华州康瑟布拉夫斯", "endpoint": "https://us-central1-gce.cloudharmony.net/" }, { "area": "us-east1", "alias": "南卡罗来纳州蒙克斯科纳", "endpoint": "https://us-east1-gce.cloudharmony.net/" }, { "area": "us-east4", "alias": "弗吉尼亚阿什本", "endpoint": "https://us-east4-gce.cloudharmony.net/" }, { "area": "us-east5", "alias": "俄亥俄州哥伦布", "endpoint": "https://us-east5-gce.cloudharmony.net/" }, { "area": "us-west1", "alias": "俄勒冈州达尔斯", "endpoint": "https://us-west1-gce.cloudharmony.net/" }, { "area": "us-west2", "alias": "加利福尼亚州洛杉矶", "endpoint": "https://us-west2-gce.cloudharmony.net/" }, { "area": "us-west3", "alias": "犹他州盐湖城", "endpoint": "https://us-west3-gce.cloudharmony.net/" }, { "area": "us-west4", "alias": "内华达州拉斯维加斯", "endpoint": "https://us-west4-gce.cloudharmony.net/" }, { "area": "us-south1", "alias": "德克萨斯州达拉斯", "endpoint": "https://us-south1-gce.cloudharmony.net/" }, { "area": "me-west1", "alias": "特拉维夫、以色列、中东", "endpoint": "https://me-west1-gce.cloudharmony.net/" }]
    }, {
        "name": "DigitalOcean", "alias": "DO", "update": "2023-01-18", "link": "https://www.digitalocean.com/",
        "list": [{ "area": "NYC", "alias": "NYC", "endpoint": "http://speedtest-nyc1.digitalocean.com/" }, { "area": "AMS", "alias": "AMS", "endpoint": "http://speedtest-ams2.digitalocean.com/" }, { "area": "SFO", "alias": "SFO", "endpoint": "http://speedtest-sfo1.digitalocean.com/" }, { "area": "SGP", "alias": "SGP", "endpoint": "http://speedtest-sgp1.digitalocean.com/" }, { "area": "LON", "alias": "LON", "endpoint": "http://speedtest-lon1.digitalocean.com/" }, { "area": "FRA", "alias": "FRA", "endpoint": "http://speedtest-fra1.digitalocean.com/" }, { "area": "TOR", "alias": "TOR", "endpoint": "http://speedtest-tor1.digitalocean.com/" }, { "area": "BLR", "alias": "BLR", "endpoint": "http://speedtest-blr1.digitalocean.com/" }]
    }, {
        "name": "Linode", "alias": "Linode", "update": "2023-01-18", "link": "https://www.linode.com/",
        "list": [{ "area": "Sydney", "alias": " Australia", "endpoint": "https://speedtest.syd1.linode.com/" }, { "area": "Mumbai", "alias": " India", "endpoint": "https://speedtest.mumbai1.linode.com/" }, { "area": "Tokyo", "alias": " Japan", "endpoint": "https://speedtest.tokyo2.linode.com/" }, { "area": "Singapore", "alias": " Singapore", "endpoint": "https://speedtest.singapore.linode.com/" }, { "area": "Frankfurt", "alias": " Germany", "endpoint": "https://speedtest.frankfurt.linode.com/" }, { "area": "London", "alias": " United Kingdom", "endpoint": "https://speedtest.london.linode.com/" }, { "area": "Toronto", "alias": " Canada", "endpoint": "https://speedtest.toronto1.linode.com/" }, { "area": "Newark", "alias": " United States", "endpoint": "https://speedtest.newark.linode.com/" }, { "area": "Atlanta", "alias": " United States", "endpoint": "https://speedtest.atlanta.linode.com/" }, { "area": "Dallas", "alias": " United States", "endpoint": "https://speedtest.dallas.linode.com/" }, { "area": "Fremont", "alias": " United States", "endpoint": "https://speedtest.fremont.linode.com/" }]
    }, {
        "name": "Vultr", "alias": "Vultr", "update": "2023-01-18", "link": "https://www.vultr.com/",
        "list": [{ "area": "Madrid", "alias": "Madrid", "endpoint": "https://mad-es-ping.vultr.com/" }, { "area": "Warsaw", "alias": " Poland", "endpoint": "https://waw-pl-ping.vultr.com/" }, { "area": "Frankfurt", "alias": " DE", "endpoint": "https://fra-de-ping.vultr.com/" }, { "area": "Paris", "alias": " France", "endpoint": "https://par-fr-ping.vultr.com/" }, { "area": "Amsterdam", "alias": " NL", "endpoint": "https://ams-nl-ping.vultr.com/" }, { "area": "London", "alias": " UK", "endpoint": "https://lon-gb-ping.vultr.com/" }, { "area": "Stockholm", "alias": " Sweden", "endpoint": "https://sto-se-ping.vultr.com/" }, { "area": "Mumbai", "alias": " India", "endpoint": "https://bom-in-ping.vultr.com/" }, { "area": "New York (NJ)", "alias": "New York (NJ)", "endpoint": "https://nj-us-ping.vultr.com/" }, { "area": "São Paulo", "alias": " Brazil", "endpoint": "https://sao-br-ping.vultr.com/" }, { "area": "Singapore", "alias": "Singapore", "endpoint": "https://sgp-ping.vultr.com/" }, { "area": "Toronto", "alias": " Canada", "endpoint": "https://tor-ca-ping.vultr.com/" }, { "area": "Seoul", "alias": " South Korea", "endpoint": "https://sel-kor-ping.vultr.com/" }, { "area": "Chicago", "alias": " Illinois", "endpoint": "https://il-us-ping.vultr.com/" }, { "area": "Atlanta", "alias": " Georgia", "endpoint": "https://ga-us-ping.vultr.com/" }, { "area": "Miami", "alias": " Florida", "endpoint": "https://fl-us-ping.vultr.com/" }, { "area": "Tokyo", "alias": " Japan", "endpoint": "https://hnd-jp-ping.vultr.com/" }, { "area": "Dallas", "alias": " Texas", "endpoint": "https://tx-us-ping.vultr.com/" }, { "area": "Seattle", "alias": " Washington", "endpoint": "https://wa-us-ping.vultr.com/" }, { "area": "Mexico City", "alias": " Mexico", "endpoint": "https://mex-mx-ping.vultr.com/" }, { "area": "Silicon Valley", "alias": " California", "endpoint": "https://sjo-ca-us-ping.vultr.com/" }, { "area": "Los Angeles", "alias": " California", "endpoint": "https://lax-ca-us-ping.vultr.com/" }, { "area": "Honolulu", "alias": " Hawaii", "endpoint": "https://hon-hi-us-ping.vultr.com/" }, { "area": "Melbourne", "alias": " Australia", "endpoint": "https://mel-au-ping.vultr.com/" }, { "area": "Sydney", "alias": " Australia", "endpoint": "https://syd-au-ping.vultr.com/" }]
    }, {
        "name": "Bandwagon", "alias": "搬瓦工", "update": "2023-01-18", "link": "https://www.bwh88.net/",
        "list": [{ "area": "HKHK_8", "alias": "香港CN2 GIA", "endpoint": "https://hk.bwg.wiki" }, { "area": "JPOS_1", "alias": "日本软银Softbank", "endpoint": "https://jp.bwg.wiki" }, { "area": "USCA_6", "alias": "洛杉矶 CN2 GIA", "endpoint": "https://dc6.bwg.wiki" }, { "area": "USCA_9", "alias": "洛杉矶 CN2 GIA", "endpoint": "https://dc9.bwg.wiki" }, { "area": "USCA_2", "alias": "洛杉矶 QNET", "endpoint": "https://dc2.bwg.wiki" }, { "area": "USCA_3", "alias": "洛杉矶 CN2 (QN)", "endpoint": "https://dc3.bwg.wiki" }, { "area": "USCA_4", "alias": "洛杉矶 MCOM", "endpoint": "https://dc4.bwg.wiki" }, { "area": "USCA_8", "alias": "洛杉矶 ZNET", "endpoint": "https://dc8.bwg.wiki" }, { "area": "USCA_FMT", "alias": "弗里蒙特", "endpoint": "https://fmt.bwg.wiki" }, { "area": "USNJ", "alias": "新泽西", "endpoint": "http://23.29.138.5" }, { "area": "USNY_2", "alias": "纽约", "endpoint": "http://208.167.227.122" }, { "area": "EUNL_3", "alias": "荷兰", "endpoint": "http://45.62.120.202" }, { "area": "EUNL_3", "alias": "荷兰联通高级线路", "endpoint": "http://104.255.64.1" }]
    }];

let spiderData = {
    diff: function (vender, arr) {
        for (let item of dataCloudNode) {
            if (item.name == vender) {
                let content = JSON.stringify(item.list);
                let newContent = JSON.stringify(arr);
                if (content == newContent) {
                    console.debug(`${vender} is the latest`)
                } else {
                    item.list = arr;

                    console.debug(`${vender} changed !`);
                    console.debug(newContent);
                }
                break;
            }
        }
    },
    fetch: async (url, option) => {
        url = `https://cors.eu.org/${encodeURIComponent(url)}`;
        let resp = await fetch(url, option);
        let result = await resp.text();
        return result;
    },
    xmlParse: (html) => {
        let dp = new DOMParser();
        let dom = dp.parseFromString(html, "text/html");
        return dom;
    },
    reqVender: async (vender) => {
        var arr = []
        switch (vender) {
            case "AWS":
                {
                    let result = await spiderData.fetch("http://ec2-reachability.amazonaws.com/");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll("div.panel").forEach(panel => {
                        var country = panel.children[0].innerText.toLowerCase();
                        var domAlias = panel.children[1].querySelector('th.region-heading');
                        var alias = domAlias.innerText.trim();
                        var detail = domAlias.parentElement.nextElementSibling.nextElementSibling.children;
                        var region = detail[0].innerText.trim();
                        arr.push({
                            "area": region, "alias": alias,
                            "endpoint": `https://dynamodb.${region}.amazonaws.com${country == "china" ? ".cn" : ""}/`
                        })
                    })

                    spiderData.diff(vender, arr);
                }
                break;
            case "Aliyun":
                {
                    let result = await spiderData.fetch("https://help.aliyun.com/document_detail/40654.html");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('ul.ul li.li').forEach(li => {
                        li.querySelectorAll('tr').forEach(tr => {
                            var tds = tr.querySelectorAll('td');
                            if (tds.length) {
                                let td0 = tds[0].innerText.trim().split('（');
                                let alias = td0.length == 2 ? td0[1].split('）')[0].trim() : "";
                                arr.push({
                                    "area": td0[0],
                                    "alias": alias,
                                    "endpoint": `https://oss-${tds[1].innerText.trim()}.aliyuncs.com/`
                                })
                            }
                        })
                    });

                    spiderData.diff(vender, arr);
                }
                break;
            case "Baidu":
                {
                    let result = await spiderData.fetch("https://cloud.baidu.com/doc/BOS/s/lk24fdmgt");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('table tr').forEach(tr => {
                        var tds = tr.querySelectorAll('td');
                        if (tds.length == 2) {
                            arr.push({
                                "area": tds[0].innerText,
                                "alias": tds[0].innerText,
                                "endpoint": `https://${tds[1].innerText}/`
                            })
                        }
                    });

                    spiderData.diff(vender, arr);
                }
                break;
            case "Tencent":
                {
                    let result = await spiderData.fetch("https://cloud.tencent.com/document/product/213/6091");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('table.table-striped').forEach(table => {
                        table.querySelectorAll('tr').forEach(tr => {
                            var tds = tr.querySelectorAll('td');
                            if (tds.length == 2) {
                                var td0 = tds[0].innerHTML;
                                arr.push({
                                    "area": td0.match(/(\W+)（/)[1],
                                    "alias": td0.match(/（(\W+)）/)[1],
                                    "endpoint": `https://cos.${td0.match(/>(.*)/)[1].trim()}.myqcloud.com/`
                                })
                            }
                        })
                    });

                    spiderData.diff(vender, arr);
                }
                break;
            case "Huawei":
                {
                    let result = await spiderData.fetch("https://developer.huaweicloud.com/endpoint?OBS");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('tr').forEach(tr => {
                        var tds = tr.querySelectorAll('td');
                        if (tds.length == 4 && tds[2].innerHTML.startsWith("obs.")) {
                            arr.push({
                                "area": tds[0].innerHTML.split('-')[0],
                                "alias": tds[0].innerHTML.split('-')[1],
                                "endpoint": `https://obs.${tds[1].innerHTML}.myhuaweicloud.com/`
                            })
                        }
                    })

                    spiderData.diff(vender, arr);
                }
                break;
            case "Ksyun":
                {
                    let result = await spiderData.fetch("https://docs.ksyun.com/documents/6761");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('tr').forEach(tr => {
                        var tds = tr.querySelectorAll('td');
                        if (tds.length == 4) {
                            var aa = tds[0].innerHTML.match(/（(\W+)）/);
                            arr.push({
                                "area": aa ? aa[1] : tds[0].innerHTML,
                                "alias": tds[1].innerHTML,
                                "endpoint": `https://${tds[2].innerHTML}/`
                            })
                        }
                    })

                    spiderData.diff(vender, arr);
                }
                break;
            case "Qiniu":
                {
                    let result = await spiderData.fetch("https://developer.qiniu.com/kodo/1671/region-endpoint-fq");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('tr').forEach(tr => {
                        var tds = tr.querySelectorAll('td');
                        if (tds.length == 3) {
                            arr.push({
                                "area": tds[0].innerText.split('-')[0].trim(),
                                "alias": tds[0].innerText.split('-').pop().trim(),
                                "endpoint": `https://upload-${tds[1].innerHTML}.qiniup.com/`
                            })
                        }
                    })

                    spiderData.diff(vender, arr);
                }
                break;
            case "QingCloud":
                {
                    let result = await spiderData.fetch("https://docs.qingcloud.com/qingstor/api/common/overview.html");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('tr').forEach(tr => {
                        var tds = tr.querySelectorAll('td');
                        if (tds.length == 3) {
                            arr.push({
                                "area": tds[0].innerHTML,
                                "alias": tds[1].innerHTML,
                                "endpoint": `https://${tds[1].innerHTML}.qingstor.com/`
                            })
                        }
                    });

                    spiderData.diff(vender, arr);
                }
                break;
            case "Microsoft":
                {
                    let result = await spiderData.fetch("https://www.azurespeed.com");
                    let dom = spiderData.xmlParse(result);

                    for (var i = 0; i < dom.scripts.length; i++) {
                        var si = dom.scripts[i];
                        if (si.src) {
                            var pn = new URL(si.src).pathname;
                            if (pn.startsWith("/main.")) {
                                var jsurl = "https://www.azurespeed.com/" + pn;
                                spiderData.fetch(jsurl).then(res2 => {
                                    var arr = [];
                                    var rm = res2.match(/JSON\.parse\('\[{(.*)}]'\)/g);
                                    var json = eval(rm[0]);

                                    json.forEach(item => {
                                        arr.push({
                                            area: item.pairedRegion,
                                            alias: item.physicalLocation,
                                            endpoint: `https://${item.regionName}.blob.core.windows.net`
                                        })
                                    })

                                    spiderData.diff(vender, arr);
                                })
                                break;
                            }
                        }
                    }
                }
                break;
            case "UCloud":
                {
                    // https://docs.ucloud.cn/api/summary/regionlist
                    let result = await spiderData.fetch("https://raw.githubusercontent.com/UCloudDocs/api/master/summary/regionlist.md");
                    let dom = result.match(/\|(.*)\|(.*)\|/g);

                    let tmp = {};
                    for (var i = 2; i < dom.length; i++) {
                        var tds = dom[i].split('|');
                        if (tds.length == 4) {
                            var sd = tds[1].trim().replace(/\d+/, '');
                            var area = tds[2].trim().replace(/[一|二|三|四|五|六|七|八|九|十]+$/, '');
                            if (!(sd in tmp)) {
                                tmp[sd] = area;

                                arr.push({
                                    "area": area,
                                    "alias": sd,
                                    "endpoint": `https://www.${sd}.ufileos.com/`
                                })
                            }
                        }
                    }

                    spiderData.diff(vender, arr);
                }
                break;
            case "Google":
                {
                    let result = await spiderData.fetch("https://cloud.google.com/compute/docs/regions-zones/");
                    let dom = spiderData.xmlParse(result);

                    var tmp = {};
                    var ra = ["亚太地区", "欧洲", "北美洲", "南美洲", "	北美洲"];
                    dom.querySelectorAll('tr').forEach(tr => {
                        var tds = tr.querySelectorAll('td');
                        if (tds.length == 6) {
                            var ep = tds[0].children[0].innerHTML.split('-');
                            ep.length = ep.length - 1;
                            ep = ep.join('-');
                            var alias = tds[1].innerHTML;
                            ra.map(x => alias = alias.replace(x, ""));
                            if (!(ep in tmp)) {
                                tmp[ep] = 1;

                                arr.push({
                                    "area": ep,
                                    "alias": alias,
                                    "endpoint": `https://${ep}-gce.cloudharmony.net/`
                                })
                            }
                        }
                    })

                    spiderData.diff(vender, arr);
                }
                break;
            case "DigitalOcean":
                {
                    let result = await spiderData.fetch("http://speedtest-fra1.digitalocean.com/");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('h2').forEach(h2 => {
                        var alias = h2.innerHTML;
                        var anode = h2.nextElementSibling.querySelector('a');
                        arr.push({
                            "area": alias,
                            "alias": alias,
                            "endpoint": anode.href
                        })
                    })

                    spiderData.diff(vender, arr);
                }
                break;
            case "Linode":
                {
                    let result = await spiderData.fetch("https://www.linode.com/speed-test/");
                    let dom = spiderData.xmlParse(result);

                    dom.querySelectorAll('div.c-speed-test__links>a').forEach(a => {
                        arr.push({
                            "area": a.innerHTML.split(',')[0],
                            "alias": a.innerHTML.split(',').pop(),
                            "endpoint": a.href.replace('http:', 'https:')
                        })
                    })

                    spiderData.diff(vender, arr);
                }
                break;
            case "Vultr":
                {
                    console.debug("反爬，请打开 URL https://www.vultr.com/resources/faq/#downloadspeedtests 后在控制台手动执行脚本");

                    //打开 url 控制台执行以下脚本，反爬
                    var arr = [];
                    $('#speedtest_v4').children().each(function () {
                        var tds = $(this).children(), td0 = tds[0].innerHTML.trim().split(',');
                        arr.push({
                            "area": td0[0],
                            "alias": td0.pop(),
                            "endpoint": `https://${tds[1].children[0].innerHTML}/`
                        })
                    });
                    console.log(JSON.stringify(arr))
                }
                break;
            case "Bandwagon":
                {
                    console.debug("暂不支持");
                }
                break;
        }
    }
}