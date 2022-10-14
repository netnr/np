# spider
爬虫

### 说明
- nrSpider.js 为爬取基础依赖，拷贝到控制台先执行再跑对应的脚本
- 爬取抓取网页内容后存储于浏览器本地，再次请求从本地读取，失败的链接不缓存
- 爬取默认限制 1QPS，已存储本地链接不限制
- 导出数据有 JSON、SQL（SQLite）、Excel（量大请通过 SQLite 导出），（如果有）失败的数据 catch-*.json

### 🧡 spider_zoning.js

统计用区划和城乡划分代码  
<http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/>  

统计数据截止 2021-10-31 于 2021-12-30 发布  
<http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2021>

共 5 级

**0.json** 深度 1、2、3  
**1101.json** 深度 4、5  
**6590.json** 深度 4、5  

**stats-zoning-3.json** 爬虫三级总数据 就是 **0.json**  
**stats-zoning-4.json** 爬虫四级总数据  
**stats-zoning-5.json** 爬虫五级总数据  

```
npm install zoningjs
```

列信息

字段 | 类型 | 说明
---- | ---- | ----
id | string | 区划代码，唯一
txt | string | 名称
pid | string | 父级区划代码
sid | string | 简短区划代码，唯一
spid | string | 简短父级区划代码
ct | string | 城乡分类代码
num | int | 同级排序
leaf | int | 是叶节点（1：是；2：否）
deep | int | 爬取深度


### 🧡 spider_product_category.js

统计用产品分类目录  
http://www.stats.gov.cn/tjsj/tjbz/tjypflml/index.html

最新发布时间 2010-06-17 ，共 5 级，爬 4 级错误链接 1 个，爬 5 级错误链接 87 个  
已根据代码 ID 去重，去除子节点与父节点相同的项

**0.json** 一级大分类  
**01.json** 一级下所有子分类  
**97.json** 一级下所有子分类  

**stats-product-category-3.json** 爬虫三级总数据  
**stats-product-category-4.json** 爬虫四级总数据  
**stats-product-category-5.json** 爬虫五级总数据  

**catch-5.json** 爬虫五级异常记录（失败的链接）

```
npm install stats-product-category
```
**Releases** https://github.com/netnr/stats/releases/tag/1.0.0

列信息

字段 | 类型 | 说明
---- | ---- | ----
id | string | 代码，唯一
txt | string | 名称
pid | string | 父级代码
num | int | 同级排序
deep | int | 爬取深度


### 🧡 spider_product_unit.js

产品计量单位  
http://tjj.hubei.gov.cn/bsfw/lwzb/ywzn/202005/t20200521_2282796.shtml

湖北省统计局产品计量单位 2020-04-28 ，分 通用计量单位 和 专用计量单位

**stats-product-unit.json** 全部单位  
**stats-product-unit-common.json** 通用计量单位，不含专用计量单位

**Releases** https://github.com/netnr/stats/releases/tag/1.0.1

列信息

字段 | 类型 | 说明
---- | ---- | ----
id | string | 代码，唯一
txt | string | 名称
remark | string | 说明