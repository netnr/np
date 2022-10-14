nr.onReady = function () {

    if (!Intl.Segmenter) {
        nr.domBtnLocal.remove();
    } else {
        nr.domBtnLocal.addEventListener('click', function () {
            var val = nr.domTxtContent.value.trim();

            var segm = new Intl.Segmenter('cn', { granularity: 'word' });
            var vals = Array.from(segm.segment(val));
            nr.domTxtResult.value = JSON.stringify(vals.map(x => x.segment), null, 2);
        });
    }

    nr.domBtnQuery.addEventListener('click', function () {
        var val = nr.domTxtContent.value.trim();
        if (val == "") {
            nr.domTxtResult.value = "";
        } else {
            nr.domBtnQuery.loading = true;
            fetch(`${ss.apiServer}/api/v1/Analysis`, {
                method: 'POST',
                body: nr.toFormData({ content: val })
            }).then(resp => resp.json()).then(res => {
                nr.domBtnQuery.loading = false;
                if (res.code == 200) {
                    nr.domTxtResult.value = JSON.stringify(res.data, null, 2);
                } else {
                    nr.alert(res.msg);
                }
            }).catch(ex => {
                console.debug(ex);
                nr.domBtnQuery.loading = false;
                nr.alert("网络错误");
            })
        }
    });
}