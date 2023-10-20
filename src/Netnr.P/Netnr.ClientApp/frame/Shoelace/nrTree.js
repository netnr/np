let nrTree = {

    /**
     * 上下级关系的数据 转为 [{value,data,indent}]
     * @param {*} data 原始数据
     * @param {*} pidField 父级字段
     * @param {*} idField 主键段
     * @param {*} startPid 起始父级数组
     * @param {*} orderField 排序字段
     * @param {*} indent 缩进
     * @returns 
     */
    fromDataToIndent: (data, pidField, idField, startPid, orderField, indent) => {
        indent = indent || 0;

        //查询当前父级的子节点
        let fdata = data.filter(x => startPid.includes(x[pidField]));
        //排序
        fdata.sort((a, b) => {
            if (a[orderField] < b[orderField]) {
                return -1;
            } else if (a[orderField] > b[orderField]) {
                return 1;
            }
            return 0;
        })

        let arr = [];
        fdata.forEach(item => {
            arr.push({ value: item[idField], data: item, indent: indent });
            //存在子节点
            let sdata = data.filter(x => x[pidField] == item[idField]);
            if (sdata.length) {
                arr = arr.concat(nrTree.fromDataToIndent(data, pidField, idField, [item[idField]], orderField, indent + 1));
            }
        });

        return arr;
    },

    /**
     * 构建 sl-tree-item
     * @param {*} data 
     * @param {*} pidField 
     * @param {*} idField 
     * @param {*} startPid 
     * @param {*} orderField 
     * @param {*} itemCallback item 回调 返回一项 html 不含结尾 </sl-tree-item>
     * @returns 
     */
    buildTreeItem: (data, pidField, idField, startPid, orderField, itemCallback) => {
        let arr = [];

        //查询当前父级的子节点
        let fdata = data.filter(x => startPid.includes(x[pidField]));
        //排序
        fdata.sort((a, b) => {
            if (a[orderField] < b[orderField]) {
                return -1;
            } else if (a[orderField] > b[orderField]) {
                return 1;
            }
            return 0;
        })
        fdata.forEach(item => {
            //当前项
            let itemHtml = itemCallback(item);
            arr.push(itemHtml);

            //存在子节点
            let sfdata = data.filter(x => x[pidField] == item[idField]);
            if (sfdata.length) {
                arr = arr.concat(nrTree.buildTreeItem(data, pidField, idField, [item[idField]], orderField, itemCallback));
            }

            arr.push('</sl-tree-item>');
        });

        return arr;
    },
}

Object.assign(window, { nrTree });
export { nrTree }