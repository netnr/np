import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/index",

    ckey: "/ss/index/icon",

    init: async () => {
        await nrPage.iconLoad();

        nrPage.bindEvent();
    },

    bindEvent: () => {
        //搜索
        nrVary.domTxtSearch.addEventListener('input', async function (event) {
            let search = this.value.toLowerCase();

            nrVary.domCardLink.querySelectorAll('a').forEach(node => {
                let col = node.parentElement;
                if (search != "") {
                    if (node.innerText.toLowerCase().includes(search) || node.href.toLowerCase().includes(search)) {
                        col.classList.remove('d-none');
                    } else {
                        col.classList.add('d-none');
                    }
                } else {
                    col.classList.remove('d-none');
                }
            });

            nrVary.domCardLink.querySelectorAll('.row').forEach(node => {
                let cols = node.children;
                node.classList.remove('d-none');
                if (node.querySelectorAll('.d-none').length + 1 >= cols.length) {
                    cols[0].classList.add('d-none');
                } else {
                    cols[0].classList.remove('d-none');
                }
            });
        })
    },

    iconLoad: async () => {
        let path = nrVary.domHidIcon.value;
        let hash = path.split('?').pop();

        let cval = await nrStorage.getItem(nrPage.ckey);

        if (cval && cval.hash == hash) {
            nrPage.iconView(cval.content);
        } else {
            let resp = await fetch(path);
            let result = await resp.text();
            nrPage.iconView(result);

            await nrStorage.setItem(nrPage.ckey, { hash, content: result });
        }
    },
    iconView: function (content) {
        let hdom = document.createElement('div');
        hdom.innerHTML = content;
        hdom.classList.add('d-none');
        document.body.appendChild(hdom);
    },
}

export { nrPage };