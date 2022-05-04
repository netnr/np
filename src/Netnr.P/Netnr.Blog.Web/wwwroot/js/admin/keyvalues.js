nr.onReady = function () {
    //Grab
    nr.domBtnRun1.addEventListener('click', function () {
        if (nr.domTxt1.value.trim() != "") {
            page.run('Grab', nr.domTxt1.value.split('\n').join(','))
        }
    })

    //Synonym
    nr.domBtnRun2.addEventListener('click', function () {
        if (nr.domTxt2.value.trim() != "") {
            page.run('Synonym', nr.domTxt2.value.split('\n').join(','))
        }
    })

    //Tag
    nr.domBtnRun3.addEventListener('click', function () {
        page.run('Tag', nr.domTxt3.value.split('\n').join(','))
    })
}

var page = {
    run: function (cmd, keys) {
        page.setLoading(true)
        fetch('/Admin/KeyVal/' + cmd + "?keys=" + encodeURIComponent(keys)).then(resp => resp.json()).then(res => {
            page.setLoading(false)
            nr.domTxtResult.value = res.log.join('\n');
        }).catch(ex => {
            console.debug(ex)
            page.setLoading(false)
            nr.alert(ex);
        });
    },
    setLoading: function (isLoading) {
        nr.domBtnRun1.loading = isLoading;
        nr.domBtnRun2.loading = isLoading;
        nr.domBtnRun3.loading = isLoading;
    }
};