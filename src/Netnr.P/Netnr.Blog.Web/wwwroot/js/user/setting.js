nr.onReady = function () {
    //编辑个人信息
    nr.domBtnEditInfo.addEventListener('click', function () {
        nr.domDialogForm.show()
    });

    //保存个人信息
    nr.domDialogForm.querySelector('form').addEventListener('submit', e => {
        e.preventDefault();

        var fd = new FormData(e.target);
        fetch('/User/SaveUserInfo', {
            method: 'POST',
            body: fd
        }).then(resp => resp.json()).then(res => {
            if (res.code == 200) {
                nr.domDialogForm.hide();
                location.reload(false);
            } else {
                nr.alert(res.msg);
            }
        })
    });

    //修改密码
    nr.domBtnSavePwd.addEventListener('click', function () {
        var errMsg = [];
        var pwd1 = nr.domTxtPwd1.value;
        var pwd2 = nr.domTxtPwd2.value;
        var pwd3 = nr.domTxtPwd3.value;

        if (pwd1 == '') {
            errMsg.push('请输入原密码');
        }
        if (pwd2 == '') {
            errMsg.push('请输入新密码');
        }
        if (pwd2 != pwd3) {
            errMsg.push('两次输入的密码不一致');
        }
        if (errMsg.length > 0) {
            nr.alert(errMsg.join('<br/>'));
        } else {
            nr.domBtnSavePwd.loading = true;
            fetch("/User/UpdatePassword", {
                method: 'POST',
                body: nr.toFormData({
                    oldPassword: pwd1,
                    newPassword: pwd2
                })
            }).then(resp => resp.json()).then(res => {
                nr.domBtnSavePwd.loading = false;
                nr.alert(res.msg);
                if (res.code == 200) {
                    setTimeout(() => {
                        location.href = "/account/login";
                    }, 2000);
                }
            }).catch(err => {
                nr.domBtnSavePwd.loading = false;
                nr.alert(err);
            });
        }
    });
}