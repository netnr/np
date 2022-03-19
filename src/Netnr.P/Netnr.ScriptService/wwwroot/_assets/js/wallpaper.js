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
    domTxtSearch: document.querySelector('.nr-txt-search'),
    domBtnSearch: document.querySelector('.nr-btn-search'),
    domWallpaper: document.querySelector('.nr-wallpaper'),
    domToolbar: document.querySelector('.position-sticky'),
    init: function () {

        //点击代理
        wp.domSeProxy.addEventListener('change', function () {
            ss.ls["useProxy"] = this.value;
            ss.lsSave();
            location.reload(false);
        }, false);
        wp.domSeProxy.value = ss.lsStr("useProxy") || 0;

        //搜索
        wp.domBtnSearch.addEventListener('click', function () {
            wp.clear();
            wp.config.search = wp.domTxtSearch.value;
            wp.load();
        }, false);
        wp.domTxtSearch.addEventListener('keydown', function (e) {
            if (e.keyCode == 13) {
                wp.domBtnSearch.click();
            }
        }, false);

        //点击分类
        wp.domSeType.addEventListener('change', function () {
            wp.clear();
            wp.domTxtSearch.value = "";
            wp.config.search = "";

            wp.config.cid = this.value;
            location.hash = this.value;
            wp.load();
        }, false);

        //滚动加载
        'scroll mousewheel DOMMouseScroll'.split(' ').forEach(en => {
            window.addEventListener(en, function () {
                var sb = document.documentElement.scrollHeight - document.documentElement.clientHeight - document.documentElement.scrollTop;
                if (sb < 1200) {
                    wp.load();
                }

                wp.domToolbar.style.opacity = document.documentElement.scrollTop < 15 ? 1 : 0.4;
            }, false);
        });

        //热词
        wp.hotKey();

        //初始化
        wp.domSeType.value = location.hash.substring(1);
        if (wp.domSeType.selectedIndex == -1) {
            wp.domSeType.value = '';
        }
        wp.config.cid = wp.domSeType.value;
        wp.load();
    },
    hotKey: function () {
        ss.ajax({
            url: "http://openbox.mobilem.360.cn/html/api/wallpaperhot.html",
            dataType: 'json',
            success: function (data) {
                var hotList = document.createElement('datalist');
                data.data.forEach(hot => {
                    hotList.appendChild(new Option(hot, hot));
                })
                hotList.id = 'hotList';
                wp.domTxtSearch.setAttribute('list', 'hotList');
                wp.domTxtSearch.parentElement.appendChild(hotList);
            }
        })
    },
    //载入
    load: function () {
        if (!wp.config.isLoading && !wp.config.isEnd) {
            wp.config.isLoading = true;
            if (wp.config.search == "") {
                document.title = `${wp.domSeType.options[wp.domSeType.selectedIndex].text} 在线壁纸 NET牛人`;
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

            ss.loading();
            ss.ajax({
                url: url,
                dataType: 'json',
                success: function (data) {
                    data = ss.datalocation(data);
                    if (data.data.length == 0 || wp.config.total == data.data.length) {
                        wp.config.isEnd = true;
                        wp.viewEnd();
                    } else {
                        wp.config.total = data.total * 1;
                        wp.config.start += wp.config.count;

                        wp.viewList(data);
                    }
                },
                complete: function () {
                    wp.config.isLoading = false;
                    ss.loading(0);
                }
            });
        }
    },
    viewEnd: function () {
        var div = document.createElement('div');
        div.className = 'col-12 text-center h4 py-3 text-muted';
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
                var downList = [`<a href='${url.replace("__85", "__100")}' target='_blank' class='list-group-item p-1' download>${item.resolution}</a>`];
                Object.keys(item).filter(x => x.startsWith('img_')).forEach(k => {
                    downList.push(`<a href='${wp.urlProxy(item[k].replace("_85", "_100"))}' target='_blank' class='list-group-item p-1' download>${k.replace('img_', '')}</a>`);
                })

                var col = document.createElement('div');
                col.className = 'col-sm-12 col-md-6 p-1 nr-cell';
                col.innerHTML = `<img data-url='${url}' title='${title}' alt='${title}' src='${minUrl}' /><div class='list-group small'>${downList.join('')}</div>`;
                wp.domWallpaper.appendChild(col);
            });

            //图片查看器
            if (wp.viewer && !wp.viewer.isShown) {
                wp.viewer.destroy();
            }
            wp.viewer = new Viewer(wp.domWallpaper, {
                url: 'data-url',
            });
        } else {
            console.log(json.errmsg);
        }
    },
    clear: function () {
        wp.config.start = 1;
        wp.config.isEnd = false;
        wp.config.isLoading = false;
        wp.domWallpaper.innerHTML = '';
    },
    //代理
    urlProxy: function (url) {
        if (ss.lsStr("useProxy") == "1") {
            url = "https://image.baidu.com/search/down?tn=download&word=download&ie=utf8&fr=detail&url=" + url;
        } else {
            url = url.replace("http://", "https://");
        }
        return url;
    }
}

wp.init();