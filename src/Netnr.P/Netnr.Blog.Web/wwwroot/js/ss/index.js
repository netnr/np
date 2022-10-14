nr.onReady = function () {
    page.iconLoad();

    nr.domTxtSearch.addEventListener('input', function () {
        page.search();
    })
}

var page = {
    iconLoad: function () {
        var path = nr.domHidIcon.value;
        var hash = path.split('?').pop();
        var obj = nr.lsObj("icon");
        if (obj && obj.hash == hash) {
            page.iconView(obj.content);
        } else {
            fetch(path).then(resp => resp.text()).then(res => {
                page.iconView(res);

                nr.ls["icon"] = { hash: hash, content: res };
                nr.lsSave();
            })
        }
    },
    iconView: function (content) {
        var hdom = document.createElement('div');
        hdom.innerHTML = content;
        hdom.classList.add('d-none');
        document.body.appendChild(hdom);
    },
    search: () => {
        var keywords = nr.domTxtSearch.value.trim().toLowerCase();

        nr.domCardNav.querySelectorAll('a').forEach(node => {
            var col = node.parentElement;
            if (keywords != "") {
                if (node.innerText.toLowerCase().includes(keywords) || node.href.toLowerCase().includes(keywords)) {
                    col.classList.remove('d-none');
                } else {
                    col.classList.add('d-none');
                }
            } else {
                col.classList.remove('d-none');
            }
        });

        nr.domCardNav.querySelectorAll('.row').forEach(node => {
            var cols = node.children;
            node.classList.remove('d-none');
            if (node.querySelectorAll('.d-none').length + 1 >= cols.length) {
                cols[0].classList.add('d-none');
            } else {
                cols[0].classList.remove('d-none');
            }
        });
    }
}