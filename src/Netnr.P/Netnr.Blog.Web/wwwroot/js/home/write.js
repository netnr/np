//变动大小
nr.onChangeSize = function (ch) {
    //新增
    if (location.pathname == "/home/write") {
        if (nr.nmd) {
            var vh = ch - nr.nmd.obj.container.getBoundingClientRect().top - 20;
            nr.nmd.height(Math.max(100, vh));
        }
    } else {
        if (page.gridOps) {
            var vh = ch - nr.domGrid.getBoundingClientRect().top - 15;
            nr.domGrid.style.height = vh + "px";
        }

        if (nr.nmd) {
            var vh = ch - 280;
            nr.nmd.height(Math.max(100, vh));
        }
    }
}

nr.onReady = function () {
    //标签
    fetch("/Home/TagSelect").then(x => x.json()).then(res => {
        nr.tomSelect = new TomSelect('.nr-tags', {
            maxItems: 3,
            maxOptions: 100,
            valueField: 'TagId',
            labelField: 'TagName',
            searchField: 'TagName',
            sortField: 'TagName',
            options: res,
            create: false
        });
    }).catch(err => {
        console.log(err);
        nr.alert(err)
    });

    require(['vs/editor/editor.main'], function () {
        nr.nmd = new netnrmd('.nr-editor');

        nr.changeTheme();
        nr.changeSize();

        //快捷键
        nr.nmd.obj.me.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyS, function () {
            domBtnSave.click();
        });
    });

    //编辑
    if (location.pathname == "/user/write") {
        page.init();
    }

    //保存
    nr.domBtnSave.addEventListener("click", function () {
        var obj = {
            UwId: nr.keyId || 0,
            UwTitle: nr.domTxtTitle.value,
            UwCategory: 0,
            UwContent: nr.nmd.gethtml(),
            UwContentMd: nr.nmd.getmd(),
            TagIds: nr.tomSelect.getValue().join(',')
        }

        var errMsg = [];
        if (obj.UwTitle == "") {
            errMsg.push("请输入 标题");
        }
        if (obj.TagIds == "") {
            errMsg.push("请选择 标签");
        }
        if (obj.UwContentMd.length < 20) {
            errMsg.push("多写一点内容哦");
        }
        if (errMsg.length > 0) {
            nr.alert(errMsg.join('<br/>'));
            return false;
        }

        nr.domBtnSave.loading = true;

        //新增
        if (location.pathname == "/home/write") {
            fetch("/Home/WriteSave", {
                method: "POST",
                body: nr.toFormData(obj)
            }).then(x => x.json()).then(res => {
                nr.domBtnSave.loading = false;
                if (res.code == 200) {
                    nr.nmd.setmd('');
                    location.href = "/home/list/" + res.data;
                } else {
                    nr.alert(res.msg);
                }
            }).catch(err => {
                nr.domBtnSave.loading = false;
                console.log(err);
                nr.alert(err);
            })
        } else {
            //编辑
            fetch("/User/WriteSave", {
                method: "POST",
                body: nr.toFormData(obj)
            }).then(x => x.json()).then(res => {
                nr.domBtnSave.loading = false;
                if (res.code == 200) {
                    nr.nmd.setmd('');
                    nr.domDialogForm.hide();
                    page.load();
                } else {
                    nr.alert(res.msg);
                }
            }).catch(err => {
                nr.domBtnSave.loading = false;
                console.log(err);
                nr.alert(err);
            })
        }
    }, false);
}