//变动大小
nr.onChangeSize = function (ch) {
    if (page.gridOps) {
        var vh = ch - nr.domGrid.getBoundingClientRect().top - 15;
        nr.domGrid.style.height = vh + "px";
    }

    if (nr.nmd) {
        var vh = ch - 280;
        nr.nmd.height(Math.max(100, vh));
    }
}

nr.onReady = function () {
    page.load();
    page.init();
}

var page = {
    init: () => {
        nr.nmd = netnrmd.init('.nr-editor', {
            storekey: "md_autosave_note",
            input: function () {
                nr.domWordCount.innerHTML = `共 <b>${this.getmd().length}</b> 个字`;
            }
        });

        nr.changeTheme();
        nr.changeSize();

        //快捷键
        nr.nmd.addCommand("Ctrl+S", () => nr.domBtnSave.click());

        //保存编辑器视图
        var vm = parseInt(localStorage.getItem('note_md_viewmodel'));
        if ([1, 2, 3].indexOf(vm) >= 0) {
            nr.nmd.toggleView(vm);
        }
        window.onbeforeunload = function () {
            localStorage.setItem('note_md_viewmodel', nr.nmd.objOptions.viewmodel);
        }

        //新增
        nr.domBtnAdd.addEventListener("click", function () {
            nr.nmd.setmd('');
            nr.keyId = 0;
            nr.domTxtTitle.value = "";
            nr.domTxtTitle.focus();
            nr.domTimeInfo.innerHTML = "";

            nr.domDialogForm.label = "新增记事";
            nr.domDialogForm.show();
        }, false);

        //编辑
        nr.domBtnEdit.addEventListener("click", function () {
            var srow = page.gridOps.api.getSelectedRows();
            if (srow.length == 0) {
                nr.alert("请选择一条记录");
            } else {
                page.editGrid(srow[0].NoteId);
            }
        }, false);

        //保存
        nr.domBtnSave.addEventListener("click", function () {
            var title = nr.domTxtTitle.value.trim();
            var md = nr.nmd.getmd();
            var errmsg = [];
            if (title == "") {
                errmsg.push("请输入 标题");
            }
            if (md.length < 2) {
                errmsg.push("多写一点内容哦");
            }
            if (errmsg.length > 0) {
                nr.alert(errmsg.join('<br/>'));
                return false;
            }

            nr.domBtnSave.disabled = true;

            var fd = new FormData();
            fd.append("NoteId", nr.keyId);
            fd.append("NoteTitle", title);
            fd.append("NoteContent", md);

            fetch('/Note/SaveNote', {
                method: 'POST',
                body: fd
            }).then(x => x.json()).then(res => {
                nr.domBtnSave.disabled = false;
                if (res.code == 200) {
                    page.load();
                    nr.keyId = res.data;
                    nr.domDialogForm.hide();
                } else {
                    nr.alert(res.msg);
                }
            }).catch(x => {
                nr.domBtnSave.disabled = false;
                nr.alert(x);
            });
        }, false);

        //删除
        nr.domBtnDel.addEventListener("click", function () {
            var srow = page.gridOps.api.getSelectedRows();
            if (srow.length) {
                if (confirm("确定要删除吗？")) {
                    fetch(`/Note/DelNote?ids=${srow.map(x => x.NoteId).join(',')}`).then(x => x.json()).then(res => {
                        if (res.code == 200) {
                            page.load();
                        } else {
                            nr.alert(res.msg);
                        }
                    }).catch(x => {
                        nr.alert(x);
                    });
                }
            } else {
                nr.alert("请选择一行再操作");
            }
        }, false);
    },
    load: () => {
        let gridOptions = {
            localeText: ag.localeText, //语言
            defaultColDef: {
                filter: 'agTextColumnFilter', floatingFilter: true,
                sortable: true, resizable: true, width: 200
            },
            getRowId: event => event.data.NoteId,
            columnDefs: [
                ag.numberCol({ headerCheckboxSelection: false }),
                { field: "NoteTitle", flex: 1, minWidth: 200 },
                { field: "NoteCreateTime", filter: 'agDateColumnFilter', },
                { field: "NoteUpdateTime", filter: 'agDateColumnFilter', },
            ],
            suppressRowClickSelection: true,
            rowSelection: 'multiple',
            cacheBlockSize: 30, //请求行数
            enableRangeSelection: true, //范围选择
            animateRows: true, //动画
            rowModelType: 'infinite', //无限行模式
            //数据源
            datasource: {
                getRows: params => {
                    //默认排序
                    if (params.sortModel.length == 0) {
                        params.sortModel.push({ colId: "NoteCreateTime", sort: "desc" });
                    }

                    fetch(`/Note/NoteList?grp=${encodeURIComponent(JSON.stringify(params))}`).then(x => x.json()).then(res => {
                        if (res.code == 200) {
                            params.successCallback(res.data.RowsThisBlock, res.data.LastRow)
                        } else {
                            params.failCallback();
                        }
                    }).catch(err => {
                        console.log(err);
                        params.failCallback();
                    })
                }
            },
            onCellDoubleClicked: event => {
                page.editGrid(event.data.NoteId);
            },
            onGridReady: () => {
                //自适应
                nr.changeSize();
            },
            getContextMenuItems: (params) => {
                var result = [
                    { name: "Reload", icon: "🔄", action: page.load },
                    'autoSizeAll',
                    'resetColumns',
                    'separator',
                    'copy',
                    'copyWithHeaders'
                ];

                return result;
            },
        };

        if (page.gridOps) {
            page.gridOps.api.destroy();
        }
        page.gridOps = new agGrid.Grid(nr.domGrid, gridOptions).gridOptions;
    },
    editGrid: (id) => {
        nr.keyId = id;

        nr.domTxtTitle.value = "";
        nr.nmd.setmd("Loading...");
        nr.domTimeInfo.innerHTML = '';

        nr.domDialogForm.label = "编辑记事";
        nr.domDialogForm.show();

        fetch(`/Note/QueryNoteOne?id=${id}`).then(x => x.json()).then(res => {
            if (res.code == 200) {
                var item = res.data;
                nr.domTxtTitle.value = item.NoteTitle;
                nr.nmd.setmd(item.NoteContent);
                nr.domTimeInfo.innerHTML = `创建时间：${item.NoteCreateTime} ，更新时间：${item.NoteUpdateTime || item.NoteCreateTime}`;
            } else {
                console.debug(res);
            }
        });
    }
}