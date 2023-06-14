import { nrcBase } from "../../../../frame/nrcBase";
import { nrcUpstream } from "../../../../frame/nrcUpstream";
import { nrcViewer } from "../../../../frame/nrcViewer";
import { nrWeb } from "../../nrWeb";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/wallpaper",
    ckey: "/ss/wallpaper/config",

    init: async () => {

        //读取配置
        let config = await nrStorage.getItem(nrPage.ckey);
        for (const key in config) {
            let dom = document.querySelector(`select.nrg-se-${key}`);
            dom.value = config[key]
        }

        nrPage.bindEvent();
        nrcBase.dispatchEvent('input', nrVary.domSeVender)
    },

    bindEvent: () => {
        //变动
        document.querySelectorAll('select').forEach(dom => {
            dom.addEventListener('input', async function (event) {
                let target = event.target;

                switch (target) {
                    //切换供应商
                    case nrVary.domSeVender:
                        {
                            nrPage.venders.forEach(vender => {
                                document.querySelectorAll(`.flag-vender-${vender}`).forEach(dom => {
                                    dom.classList.add('d-none')
                                })
                            });

                            document.querySelectorAll(`.flag-vender-${this.value}`).forEach(dom => {
                                dom.classList.remove('d-none')
                            });

                            //清理
                            nrPage.wpClear();
                            await nrPage.wpLoad();
                        }
                        break;

                    //type 360
                    case nrVary.domSeType360:
                        {
                            //清理
                            nrPage.wpClear();
                            nrVary.domTxtQuery360.value = "";

                            await nrPage.wpLoad();
                        }
                        break;
                    //type adesk
                    case nrVary.domSeTypeAdesk:
                        {
                            //清理
                            nrPage.wpClear();

                            await nrPage.wpLoad();
                        }
                        break;
                    //type bing
                    case nrVary.domSeTypeBing:
                        {
                            //清理
                            nrPage.wpClear();

                            await nrPage.wpLoad();
                        }
                        break;
                }

                //保存配置
                let config = {};
                document.querySelectorAll('select').forEach(node => {
                    let key = node.classList[0].substring(7);
                    config[key] = node.value;
                })
                await nrStorage.setItem(nrPage.ckey, config);
            })
        });

        //滚动加载
        'scroll mousewheel DOMMouseScroll'.split(' ').forEach(en => {
            window.addEventListener(en, async function () {
                let scrollHeight = document.documentElement.scrollHeight || document.body.scrollHeight
                let windowHeight = document.documentElement.clientHeight || document.body.clientHeight
                let scrollTop = document.documentElement.scrollTop || document.body.scrollTop
                // 底部距离
                let scrollBottom = scrollHeight - windowHeight - scrollTop
                if (scrollBottom < 1200) {
                    await nrPage.wpLoad();
                }
            });
        });

        //360 搜索
        nrVary.domBtnQuery360.addEventListener('click', async function () {
            nrPage.wpClear();
            nrPage.wpConfig.search = nrVary.domTxtQuery360.value;
            await nrPage.wpLoad();
        });
        nrVary.domTxtQuery360.addEventListener('keydown', function (e) {
            if (e.keyCode == 13) {
                nrVary.domBtnQuery360.click();
            }
        });

        //放大
        nrVary.domWallpaper.addEventListener('click', function (event) {
            let target = event.target;
            if (target.nodeName == "IMG" && target.dataset.src) {
                Object.assign(window, { nrcViewer });
                nrcViewer.init({ src: target.dataset.src })
            }
        })
    },

    venders: ['360', 'adesk', 'bing'],

    //代理
    imgProxy: (url, force) => {
        if (nrVary.domSeProxy.value == "1" || force) {
            url = "https://image.baidu.com/search/down?tn=download&word=download&ie=utf8&fr=detail&url=" + encodeURIComponent(url);
        } else {
            url = url.replace("http://", "https://");
        }
        return url;
    },

    wpEnded: function () {
        let domEnded = document.createElement('div');
        domEnded.className = 'col-12 text-center h4 py-3 opacity-75';
        domEnded.innerHTML = '已经到底了';
        nrVary.domWallpaper.appendChild(domEnded);
    },
    wpLoad: async () => {
        try {
            if (nrPage.wpConfig.loading == false && nrPage.wpConfig.ended == false) {
                nrPage.wpConfig.loading = true;

                let url;

                switch (nrVary.domSeVender.value) {
                    case "bing":
                        {
                            document.title = 'Bing壁纸 NET牛人';
                        }
                        break;
                    case "adesk":
                        {
                            document.title = `手机壁纸 NET牛人`;

                            url = "http://service.picasso.adesk.com/v1/vertical/";
                            if (nrVary.domSeTypeAdesk.value != '') {
                                url += `category/${nrVary.domSeTypeAdesk.value}/vertical?limit=${nrPage.wpConfig.size}&skip=${nrPage.wpConfig.start}&adult=false&first=1&order=new`;
                            } else {
                                url += `vertical?limit=${nrPage.wpConfig.size}&skip=${nrPage.wpConfig.start}&adult=false&first=0&order=hot`
                            }
                        }
                        break;
                    default:
                        {
                            if (nrPage.wpConfig.search == "") {
                                let typeName = nrVary.domSeType360.options[nrVary.domSeType360.selectedIndex].innerText;
                                document.title = `${typeName} 360壁纸 NET牛人`;
                            } else {
                                document.title = `搜索360壁纸 NET牛人`;
                            }

                            url = `http://wallpaper.apc.360.cn/index.php?c=WallPaper&start=${nrPage.wpConfig.start}&count=${nrPage.wpConfig.size}&from=360chrome`;
                            if (nrPage.wpConfig.search != "") {
                                url += `&a=search&kw=${encodeURIComponent(nrPage.wpConfig.search)}`;
                            } else if (nrVary.domSeType360.value != '') {
                                url += `&a=getAppsByCategory&cid=${nrVary.domSeType360.value}`;
                            } else {
                                url += '&a=getAppsByOrder&order=create_time';
                            }

                        }
                        break;
                }

                let result;
                if (url) {
                    result = await nrcUpstream.fetch(url);

                    if (nrPage.wpConfig.start < 2) {
                        nrVary.domWallpaper.innerHTML = '';
                    }
                    result = JSON.parse(result);
                    console.debug(result)
                }

                //显示
                switch (nrVary.domSeVender.value) {
                    case "bing":
                        {
                            if (nrPage.wpBingConfig.list.length == 0) {
                                result = await nrWeb.reqServer('/file/data-bing-wallpaper.json?v1.121.76');

                                nrPage.wpBingConfig.list = result.reverse();
                            }
                            if (nrPage.wpConfig.start <= nrPage.wpConfig.size) {
                                nrVary.domWallpaper.innerHTML = "";
                            }

                            let list = nrPage.wpBingConfig.list;
                            if (nrVary.domSeTypeBing.value == "random") {
                                list = nrPage.arrayRandom([].concat(list))
                            }
                            list.slice(nrPage.wpConfig.start, nrPage.wpConfig.start += nrPage.wpConfig.size).forEach(item => {
                                let vd = item.split('/');
                                let ver = vd[0];
                                let day = vd[1];
                                let npmPath = `ushio-api-img-wallpaper@${ver}/img_${day}_1920x1080_96_background_normal.jpg`;
                                let url = nrPage.imgProxy(`https://npmcdn.com/${npmPath}`);

                                let domCol = document.createElement('div');
                                domCol.className = 'col-12 my-2';
                                domCol.innerHTML = `<img class="mw-100 rounded" style="min-height:10em;cursor:zoom-in;" alt='${day}' title="${day}" data-src='${url}' src='${url}' onerror='this.src="/favicon.svg"' />`;
                                nrVary.domWallpaper.appendChild(domCol);
                            });

                            //到底
                            if (nrPage.wpConfig.start + 1 >= nrPage.wpBingConfig.list.length) {
                                nrPage.wpConfig.ended = true;

                                nrPage.wpEnded();
                            }
                        }
                        break;
                    case "adesk":
                        {
                            let data = result.res.vertical;
                            data.forEach(item => {
                                let imgURL = nrPage.imgProxy(item.wp, true);
                                let minUrl = nrPage.imgProxy(item.thumb, true);
                                let title = item.tag.join(' ').trim();

                                //下载项
                                let downList = [`<a href='${imgURL}' target='_blank' class='text-nowrap me-3 lh-lg'>下载</a>`];

                                let domCol = document.createElement('div');
                                domCol.className = 'col-xxl-3 col-lg-4 col-md-6 mb-3 mt-1';
                                domCol.innerHTML = `<img class="w-100 rounded" style="min-height:10em;cursor:zoom-in;" title='${title}' alt='${title}' data-src='${imgURL}' src='${minUrl}' onerror='this.src="/favicon.svg"' />
                                <div class='text-break small opacity-75'>${downList.join('')}</div>`;
                                nrVary.domWallpaper.appendChild(domCol);
                            });

                            nrPage.wpConfig.start += data.length;
                            let lastId = data.pop().id;
                            //到底
                            if (lastId == nrPage.wpAdeskConfig.prevId) {
                                nrPage.wpConfig.ended = true;

                                nrPage.wpEnded();
                            }
                            nrPage.wpAdeskConfig.prevId = lastId;
                        }
                        break;
                    default:
                        {
                            if (result.errno == "0") {
                                result.data.forEach(item => {
                                    let url = nrPage.imgProxy(item.url);
                                    let minUrl = url.replace("/bdr/__60", "/bdm/1000_618_60");
                                    let title = item.utag || item.tag.replaceAll('_category_', '').replaceAll('_', '');

                                    //下载项
                                    let downList = [`<a href='${url.replace("__60", "__100")}' target='_blank' class='text-nowrap me-3 lh-lg' download>${item.resolution}</a>`];
                                    Object.keys(item).filter(x => x.startsWith('img_')).forEach(k => {
                                        downList.push(`<a href='${nrPage.imgProxy(item[k].replace("_60", "_100"))}' target='_blank' class='text-nowrap me-3 lh-lg' download>${k.replace('img_', '').replace('_', 'x')}</a>`);
                                    })

                                    let domCol = document.createElement('div');
                                    domCol.className = 'col-md-6 mb-3 mt-1';
                                    domCol.innerHTML = `<img class="mw-100 rounded" style="min-height:10em;cursor:zoom-in;" title='${title}' alt='${title}' data-src='${url}' src='${minUrl}' onerror='this.src="/favicon.svg"' />
                                    <div class='text-break small opacity-75'>${downList.join('')}</div>`;
                                    nrVary.domWallpaper.appendChild(domCol);
                                });
                            }

                            nrPage.wpConfig.total = result.total * 1;
                            nrPage.wpConfig.start += result.data.length;

                            //到底
                            if (result.data.length == 0 || nrPage.wpConfig.total == result.data.length || nrPage.wpConfig.start >= result.total) {
                                nrPage.wpConfig.ended = true;

                                nrPage.wpEnded();
                            }
                        }
                        break;
                }

                nrPage.wpConfig.loading = false;
            }
        } catch (ex) {
            nrApp.logError(ex, '加载失败');
        }
    },
    wpClear: () => {
        Object.assign(nrPage.wpConfig, {
            start: 0, size: 18, total: 0, search: '',
            loading: false, ended: false
        })
        Object.assign(nrPage.wpAdeskConfig, { prevId: '' })

        nrVary.domWallpaper.innerHTML = nrApp.tsLoadingHtml;
    },

    wpConfig: {
        start: 0,
        size: 18,
        total: 0,
        search: '',
        loading: false,
        ended: false
    },

    // https://www.jianshu.com/p/fb1d1ad58a0b
    wpAdeskConfig: {
        prevId: "",
    },

    // https://www.eee.dog/tech/rand-pic-api.html
    wpBingConfig: {
        list: [],
    },

    arrayRandom: (array) => {
        let counter = array.length;
        while (counter > 0) {
            let index = Math.floor(Math.random() * counter);
            counter--;

            let temp = array[counter];
            array[counter] = array[index];
            array[index] = temp;
        }

        return array;
    },
}

export { nrPage };