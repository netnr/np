nr.onReady = function () {
    if (nr.domHidIsme.value == "1") {
        nr.domBtnEditSay.classList.remove('d-none');

        //编辑 say
        nr.domBtnEditSay.addEventListener('click', function () {
            nr.domCardSay1.classList.add('d-none');
            nr.domCardSay2.classList.remove('d-none');

            nr.domTxtSay.value = nr.domCardSay1.innerText;
        })

        //保存 say
        nr.domBtnSaveSay.addEventListener('click', function () {
            var obj = { UserSay: nr.domTxtSay.value }

            nr.domBtnSaveSay.loading = true;
            fetch('/User/UpdateUserSay', {
                method: 'POST',
                body: nr.toFormData(obj),
            }).then(resp => resp.json()).then(res => {
                nr.domBtnSaveSay.loading = false;
                if (res.code == 200) {
                    nr.domCardSay1.innerText = nr.domTxtSay.value;
                    nr.domCardSay1.classList.remove('d-none');
                    nr.domCardSay2.classList.add('d-none');
                } else {
                    nr.alert(res.msg);
                }
            }).catch(ex => {
                console.debug(ex);
                nr.domBtnSaveSay.loading = false;
                nr.alert("网络错误");
            })
        })

        //取消 say
        nr.domBtnCancelSay.addEventListener('click', function () {
            nr.domCardSay1.classList.remove('d-none');
            nr.domCardSay2.classList.add('d-none');
        })

        //编辑 avatar
        nr.domImgAvatar.addEventListener('click', function () {
            nr.domDialogForm.show();
        })

        //获取 avatar
        nr.domBtnGetAvatar.addEventListener('click', function () {
            if (nr.domTxtEmail.value.trim() == "") {
                nr.alert("请输入邮箱");
            } else {
                nr.domBtnGetAvatar.loading = true;

                var img = new Image();
                img.onload = function () {
                    nr.domBtnGetAvatar.loading = false;
                    nr.domBtnSaveAvatar.disabled = false;
                    nr.domImgPreviewAvatar.src = img.src;
                }
                img.onerror = function () {
                    nr.domBtnGetAvatar.loading = false;
                    nr.domBtnSaveAvatar.disabled = true;
                    nr.alert("获取头像失败");
                }
                img.src = nr.domHidGravatarUrl.value + md5(nr.domTxtEmail.value.trim()) + "?s=400";
            }
        })

        //保存 avatar
        nr.domBtnSaveAvatar.addEventListener('click', function () {
            var obj = {
                type: "link",
                source: nr.domImgPreviewAvatar.src
            }

            nr.domBtnSaveAvatar.loading = true;
            fetch('/User/UpdateUserAvatar', {
                method: 'POST',
                body: nr.toFormData(obj),
            }).then(resp => resp.json()).then(res => {
                nr.domBtnSaveAvatar.loading = false;
                if (res.code == 200) {
                    nr.domImgAvatar.src = nr.domImgPreviewAvatar.src;
                    nr.domDialogForm.hide();
                } else {
                    nr.alert(res.msg);
                }
            }).catch(ex => {
                console.debug(ex);
                nr.domBtnSaveAvatar.loading = false;
                nr.alert("网络错误");
            })
        })
    }
}