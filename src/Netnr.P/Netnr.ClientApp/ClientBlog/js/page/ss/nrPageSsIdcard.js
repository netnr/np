import { nrcBase } from "../../../../frame/nrcBase";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/idcard",

    init: async () => {
        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domTxtQuery.addEventListener('keydown', function (event) {
            if (event.keyCode == 13) {
                nrVary.domBtnQuery.click();
            }
        });

        nrVary.domBtnQuery.addEventListener('click', async function (event) {
            let val = nrVary.domTxtQuery.value.trim(), cc = false;

            if (val.length == 15 || val.length == 18) {
                if (val.length == 18 && !isNaN(val.substr(0, 17))) {
                    cc = true;
                }
            }

            if (cc) {
                //校验
                let ee;
                let mod;
                let valide = [1, 0, 'X', 9, 8, 7, 6, 5, 4, 3, 2];
                if (/^\d{17}[\dxX]$/.test(val)) {
                    let a = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2];
                    let s = 0;
                    for (let i = 0; i < val.length - 1; i++) {
                        s += val.charAt(i) * a[i];
                    }
                    mod = s % 11;
                    if (valide[mod] != val.charAt(17).toUpperCase()) {
                        ee = 1;
                    }
                    else {
                        ee = 0;
                    }
                }

                let y = '', m = '', d = '', s = '', sex = '男';
                let bianhao = val.substr(0, 6);
                if (val.length == 15) {
                    y = val.substr(6, 2);
                    m = val.substr(8, 2);
                    d = val.substr(10, 2);
                    s = val.substr(14, 1);
                } else if (val.length == 18) {
                    y = val.substr(6, 4);
                    m = val.substr(10, 2);
                    d = val.substr(12, 2);
                    s = val.substr(16, 1);
                }

                if (s % 2 == 0) {
                    sex = '女';
                }

                //月最大天数
                let mn = { "02": 29 };
                [1, 3, 5, 7, 8, 10, 12].forEach(i => mn[`${i}`.padStart(2, '0')] = 31);
                [4, 6, 9, 11].forEach(i => mn[`${i}`.padStart(2, '0')] = 30);

                //区域
                await nrcBase.importScript('/file/data-idcard.js');

                if (dataIdCard[bianhao] == undefined || mn[m] == undefined || parseInt(d, 10) < 1 || parseInt(d, 10) > mn[m]) {
                    nrApp.toast('身份证信息有误');
                } else if (ee == 0) {
                    nrVary.domNodeCode.innerHTML = val;
                    nrVary.domNodeDiqu.innerHTML = dataIdCard[bianhao];
                    nrVary.domNodeShengri.innerHTML = y + '年' + m + '月' + d + '日';
                    nrVary.domNodeXingbie.innerHTML = sex;

                    nrVary.domCardInfo.classList.remove('d-none');
                    nrVary.domCardEinfo.classList.add('d-none');
                }
                else {
                    nrVary.domNodeEcode.innerHTML = val.substr(0, 17) + valide[mod];
                    nrVary.domNodeEcodebad.innerHTML = val;
                    nrVary.domNodeEdiqu.innerHTML = dataIdCard[bianhao];
                    nrVary.domNodeEshengri.innerHTML = y + '年' + m + '月' + d + '日';
                    nrVary.domNodeExingbie.innerHTML = sex;

                    nrVary.domCardInfo.classList.add('d-none');
                    nrVary.domCardEinfo.classList.remove('d-none');
                }
            } else {
                nrApp.toast('请输入 15 或 18 位正确的身份证号码');
            }
        })
    },

}

export { nrPage };