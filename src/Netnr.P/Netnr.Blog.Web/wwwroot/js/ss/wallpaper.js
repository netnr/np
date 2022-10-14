nr.onReady = function () {

    //切换供应商
    nr.domSeVender.addEventListener('sl-change', function () {

        document.querySelectorAll('.nr-vender-360').forEach(node => {
            if (this.value == "360") {
                node.classList.remove('d-none');
            } else {
                node.classList.add('d-none');
            }
        });

        document.querySelectorAll('.nr-vender-adesk').forEach(node => {
            if (this.value == "adesk") {
                node.classList.remove('d-none');
            } else {
                node.classList.add('d-none');
            }
        });

        document.querySelectorAll('.nr-vender-bing').forEach(node => {
            if (this.value == "bing") {
                node.classList.remove('d-none');
            } else {
                node.classList.add('d-none');
            }
        });

        wp.clear();

        switch (this.value) {
            case "bing":
                wpBing.init();
                break;
            case "adesk":
                wpAdesk.load();
                break;
            default:
                wp.load();
                break;
        }
    });

    //adesk 切换类型
    nr.domSeTypeAdesk.addEventListener('sl-change', function () {
        wp.clear();
        wpAdesk.config.cid = this.value;
        wpAdesk.load();
    });

    //bing 加载模式
    nr.domSeTypeBing.addEventListener('sl-change', function () {
        wp.clear();
        wpBing.init();
    });

    //滚动加载
    'scroll mousewheel DOMMouseScroll'.split(' ').forEach(en => {
        window.addEventListener(en, function () {
            let scrollHeight = document.documentElement.scrollHeight || document.body.scrollHeight
            let windowHeight = document.documentElement.clientHeight || document.body.clientHeight
            let scrollTop = document.documentElement.scrollTop || document.body.scrollTop
            // 底部距离
            var scrollBottom = scrollHeight - windowHeight - scrollTop
            if (scrollBottom < 1200) {
                switch (nr.domSeVender.value) {
                    case "bing":
                        wpBing.init();
                        break;
                    case "adesk":
                        wpAdesk.load();
                        break;
                    default:
                        wp.load();
                        break;
                }
            }
        }, false);
    });

    wp.init()
}

var wp = {
    config: {
        cid: '',
        start: 1,
        count: 12,
        total: 0,
        search: '',
        isLoading: false,
        isEnd: false,
    },
    domSeProxy: document.querySelector('.nr-se-proxy'),
    domSeType: document.querySelector('.nr-se-type'),
    domTxtQuery: document.querySelector('.nr-txt-query'),
    domBtnQuery: document.querySelector('.nr-btn-query'),
    domWallpaper: document.querySelector('.nr-wallpaper'),
    init: function () {
        //初始化
        wp.domSeType.value = location.hash.substring(1);
        wp.config.cid = wp.domSeType.value;
        wp.domSeProxy.value = nr.lsStr("useProxy") || "0";
        setTimeout(() => {
            //点击代理
            wp.domSeProxy.addEventListener('sl-change', function () {
                nr.ls["useProxy"] = this.value;
                nr.lsSave();
                location.reload(false);
            }, false);

            //点击分类
            wp.domSeType.addEventListener('sl-change', function () {
                wp.clear();
                wp.domTxtQuery.value = "";
                wp.config.search = "";

                wp.config.cid = this.value;
                location.hash = this.value;
                wp.load();
            }, false);
        }, 500);

        //搜索
        wp.domBtnQuery.addEventListener('click', function () {
            wp.clear();
            wp.config.search = wp.domTxtQuery.value;
            wp.load();
        }, false);
        wp.domTxtQuery.addEventListener('keydown', function (e) {
            if (e.keyCode == 13) {
                wp.domBtnQuery.click();
            }
        }, false);

        wp.load();
    },
    //载入
    load: function () {
        if (!wp.config.isLoading && !wp.config.isEnd) {

            if (wp.config.search == "") {
                var domItem = wp.domSeType.querySelector('sl-menu-item[value="' + wp.domSeType.value + '"]');
                document.title = `${domItem.innerText} 360壁纸 NET牛人`;
            } else {
                document.title = `搜索壁纸 NET牛人`;
            }

            var url = `http://wallpaper.apc.360.cn/index.php?c=WallPaper&start=${wp.config.start}&count=${wp.config.count}&from=360chrome`;
            if (wp.config.search != "") {
                url += `&a=search&kw=${encodeURIComponent(wp.config.search)}`;
            } else if (wp.config.cid != '') {
                url += `&a=getAppsByCategory&cid=${wp.config.cid}`;
            } else {
                url += '&a=getAppsByOrder&order=create_time';
            }

            wp.setLoading(true);
            ss.fetch({ url: url }).then(res => {
                wp.setLoading(false);

                res = JSON.parse(res);
                console.debug(res)
                if (res.data.length == 0 || wp.config.total == res.data.length || wp.config.start >= res.total) {
                    wp.config.isEnd = true;
                    wp.viewEnd();
                } else {
                    wp.config.total = res.total * 1;
                    wp.config.start += wp.config.count;

                    wp.viewList(res);
                }
            }).catch(ex => {
                console.debug(ex);
                wp.setLoading(false);
                nr.alert("加载失败，请刷新重试！");
            })
        }
    },
    setLoading: function (isLoading) {
        ss.loading(isLoading);
        wp.config.isLoading = isLoading;
        wpAdesk.config.isLoading = isLoading;
        
        nr.domBtnQuery.loading = isLoading;
        nr.domSeVender.disabled = isLoading;
        nr.domSeType.disabled = isLoading;
        nr.domSeProxy.disabled = isLoading;
        nr.domSeTypeAdesk.disabled = isLoading;
    },
    viewEnd: function () {
        var div = document.createElement('div');
        div.className = 'col-12 text-center h4 py-3 opacity-75';
        div.innerHTML = '已经到底了';
        wp.domWallpaper.appendChild(div);
    },
    viewList: function (data) {
        if (data.errno == "0") {
            data.data.forEach(item => {
                var url = wp.urlProxy(item.url),
                    minUrl = url.replace("/bdr/__85", "/bdm/1000_618_85"),
                    title = item.utag || item.tag.replaceAll('_category_', '').replaceAll('_', '');

                //下载项
                var downList = [`<a href='${url.replace("__85", "__100")}' target='_blank' class='text-nowrap me-3 lh-lg' download>${item.resolution}</a>`];
                Object.keys(item).filter(x => x.startsWith('img_')).forEach(k => {
                    downList.push(`<a href='${wp.urlProxy(item[k].replace("_85", "_100"))}' target='_blank' class='text-nowrap me-3 lh-lg' download>${k.replace('img_', '').replace('_', 'x')}</a>`);
                })

                var col = document.createElement('div');
                col.className = 'col-md-6 mb-3 mt-1';
                col.innerHTML = `<img class="mw-100 rounded" style="cursor:zoom-in" data-url='${url}' title='${title}' alt='${title}' src='${minUrl}' />
                <div class='text-break small opacity-75'>${downList.join('')}</div>`;
                wp.domWallpaper.appendChild(col);
            });

            //图片查看器
            if (wp.viewer && !wp.viewer.isShown) {
                wp.viewer.destroy();
            }
            wp.viewer = new Viewer(wp.domWallpaper, {
                url: 'data-url',
            });
        }
    },
    clear: function () {
        Object.assign(wp.config, {
            start: 0,
            isLoading: false,
            isEnd: false,
        })

        wpBing.config.start = 0;

        Object.assign(wpAdesk.config, {
            start: 0,
            isLoading: false,
            isEnd: false,
            prevCid: '',
            prevId: '',
        })
        wp.domWallpaper.innerHTML = '';
    },
    //代理
    urlProxy: function (url, isForce) {
        if (nr.lsStr("useProxy") == "1" || isForce == true) {
            url = "https://image.baidu.com/search/down?tn=download&word=download&ie=utf8&fr=detail&url=" + encodeURIComponent(url);
        } else {
            url = url.replace("http://", "https://");
        }
        return url;
    }
}

//https://www.eee.dog/tech/rand-pic-api.html
var wpBing = {
    config: {
        start: 0,
        count: 6,
        list: [],
    },
    init: () => {
        if (wpBing.config.list.length == 0) {
            fetch('/file/ss/bing-wallpaper.json?v1.121.76').then(resp => resp.json()).then(res => {
                wpBing.config.list = res.reverse();
                wpBing.viewList();
            })
        } else {
            wpBing.viewList();
        }
    },
    viewList: () => {
        document.title = `Bing壁纸 NET牛人`;

        if (wpBing.config.start + 1 >= wpBing.config.list.length) {
            wp.viewEnd();
        } else {
            var list = wpBing.config.list;
            if (nr.domSeTypeBing.value == "random") {
                list = wpBing.shuffle([].concat(list))
            }

            var items = list.slice(wpBing.config.start, wpBing.config.start += wpBing.config.count);
            items.forEach(item => {
                var vd = item.split('/');
                var ver = vd[0], day = vd[1];
                var npmPath = `ushio-api-img-wallpaper@${ver}/img_${day}_1920x1080_96_background_normal.jpg`;
                var url = `https://unpkg.com/${npmPath}`;

                var col = document.createElement('div');
                col.className = 'col-12 mb-3 mt-1';
                col.innerHTML = `<img class="mw-100 rounded" alt='${day}' title="${day}" src='${url}'/>`;
                wp.domWallpaper.appendChild(col);
            });
        }
    },
    shuffle: (array) => {
        let counter = array.length;
        while (counter > 0) {
            let index = Math.floor(Math.random() * counter);
            counter--;

            let temp = array[counter];
            array[counter] = array[index];
            array[index] = temp;
        }

        return array;
    }
}

//https://www.jianshu.com/p/fb1d1ad58a0b
var wpAdesk = {
    config: {
        cid: '',
        start: 0,
        count: 24,
        isLoading: false,
        isEnd: false,
        prevCid: "",
        prevId: "",
    },
    //载入
    load: function () {
        if (!wpAdesk.config.isLoading && !wpAdesk.config.isEnd) {
            document.title = `手机壁纸 NET牛人`;

            var url = "http://service.picasso.adesk.com/v1/vertical/";
            if (wpAdesk.config.cid != '') {
                url += `category/${wpAdesk.config.cid}/vertical?limit=${wpAdesk.config.count}&skip=${wpAdesk.config.start}&adult=false&first=1&order=new`;
            } else {
                url += `vertical?limit=${wpAdesk.config.count}&skip=${wpAdesk.config.start}&adult=false&first=0&order=hot`
            }
            
            wp.setLoading(true);
            ss.fetch({ url: url }).then(res => {
                wp.setLoading(false);

                res = JSON.parse(res);
                console.debug(res)
                var data = res.res.vertical;
                var lastId = data[data.length - 1].id;
                if (wpAdesk.config.prevCid == nr.domSeTypeAdesk.value && lastId == wpAdesk.config.prevId) {
                    wpAdesk.config.isEnd = true;
                    wp.viewEnd();
                } else {
                    wpAdesk.config.start += wpAdesk.config.count;
                    wpAdesk.config.prevCid = nr.domSeTypeAdesk.value;
                    wpAdesk.config.prevId = lastId;
                    wpAdesk.viewList(data);
                }
            }).catch(ex => {
                console.debug(ex);
                wp.setLoading(false);
                nr.alert("加载失败，请刷新重试！");
            })
        }
    },
    viewList: function (data) {
        data.forEach(item => {
            var url = wp.urlProxy(item.wp, true),
                minUrl = wp.urlProxy(item.thumb, true),
                title = item.tag.join(' ').trim();

            //下载项
            var downList = [`<a href='${url}' target='_blank' class='text-nowrap me-3 lh-lg' download>下载</a>`];

            var col = document.createElement('div');
            col.className = 'col-xxl-3 col-lg-4 col-md-6 mb-3 mt-1';
            col.innerHTML = `<img class="w-100 rounded" style="cursor:zoom-in" data-url='${url}' title='${title}' alt='${title}' src='${minUrl}' />
                <div class='text-break small opacity-75'>${downList.join('')}</div>`;
            wp.domWallpaper.appendChild(col);
        });

        //图片查看器
        if (wp.viewer && !wp.viewer.isShown) {
            wp.viewer.destroy();
        }
        wp.viewer = new Viewer(wp.domWallpaper, {
            url: 'data-url',
        });
    },
}