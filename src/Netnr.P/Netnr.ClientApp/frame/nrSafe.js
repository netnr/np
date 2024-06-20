let nrSafe = {
    tsLoaded: null,
    rely: async () => {
        if (nrSafe.tsLoaded == null) {
            nrSafe.tsLoaded = (async () => {
                const litHTML = await import('lit-html');

                /**
                 * 渲染成 HTML 原始字符串
                 * @param {*} strings 
                 * @param  {...any} values 
                 * @returns 
                 */
                function htmlraw(strings, ...values) {
                    let template = litHTML.html(strings, ...values);
                    let dom = document.createElement("div");
                    litHTML.render(template, dom);
                    removeCommentNodes(dom);
                    return dom.innerHTML;
                }

                /**
                 * 删除注释
                 * @param {*} element 
                 */
                function removeCommentNodes(element) {
                    var childNodes = element.childNodes;

                    for (var i = childNodes.length - 1; i >= 0; i--) {
                        var node = childNodes[i];

                        if (node.nodeType === Node.COMMENT_NODE) {
                            element.removeChild(node);
                        } else if (node.nodeType === Node.ELEMENT_NODE) {
                            removeCommentNodes(node);
                        }
                    }
                }

                /**
                 * html模板 数组拼接
                 * @param {*} htmls 
                 * @param {*} separator 
                 * @returns 
                 */
                function htmljoin(htmls, separator = '') {
                    return htmls.map((item, index) => litHTML.html`${item}${index < htmls.length - 1 ? litHTML.html`${separator}` : ''}`);
                }

                const DOMPurify = (await import('dompurify')).default;
                // 配置 DOMPurify，设置允许的标签
                DOMPurify.addHook('uponSanitizeElement', (node, data) => {
                    if (node.nodeName.startsWith("SL-")) {
                        data.allowedTags[data.tagName] = true;
                    }
                });

                Object.assign(window, {
                    litHTML, html: litHTML.html,
                    htmlraw, htmljoin,
                    DOMPurify
                })
            })();
        }

        return nrSafe.tsLoaded;
    }
}

export { nrSafe };