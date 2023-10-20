let nrRouter = {

    /**
     * 获取页面脚本
     * @param {any} pathname
     */
    findPage: async (pathname, options) => {

        let pathArray = pathname.toLowerCase().split('/');
        options = Object.assign({
            pack: null, // 包对象
            routerController: pathArray[1],
            routerAction: pathArray[2],
            routerId: pathArray[3]
        }, options);

        let nrPage; //页面脚本对象

        if (options.pack) {
            for (const packItem of options.pack) {
                if (nrPage) break;

                let packObj = packItem["nrPage"]; //提取模块输出对象
                if (packObj && packObj.pathname) {

                    let paths = packObj.pathname;
                    if (({}.toString.call(paths)).includes("String")) {
                        paths = [paths];
                    }

                    for (const path of paths) {
                        if (path == pathname) {
                            nrPage = packObj;
                            break;
                        } else {
                            let pagePaths = path.split('/');
                            if (options.routerController == pagePaths[1]) {
                                if (options.routerAction == pagePaths[2] || (pagePaths[2] == "*" && options.routerAction != null)) {
                                    if (pagePaths[3] == null || (pagePaths[3] == "*" && options.routerId != null)) {
                                        nrPage = packObj;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return nrPage;
    }

}

Object.assign(window, { nrRouter });
export { nrRouter }