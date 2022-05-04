nr.onReady = function () {
    page.setConfig(nr.lsObj('config'));

    page.codeBuild();
    nr.domBtnBuild.addEventListener('click', page.codeBuild);
    nr.domBtnReset.addEventListener('click', function () {
        [nr.domTxtRange1, nr.domTxtRange2, nr.domTxtCount, nr.domTxtGroup, nr.domSeOnly].forEach(function (dom) {
            dom.value = dom.defaultValue;
        });
        page.codeBuild();
    });
}

var page = {
    setConfig: function (config) {
        [nr.domTxtRange1, nr.domTxtRange2, nr.domTxtCount, nr.domTxtGroup].forEach(function (dom) {
            if (dom.defaultValue == null) {
                dom.defaultValue = dom.value;
                dom.addEventListener('input', page.codeBuild);
            }
            var val = config[dom.classList[0]];
            if (val != null) {
                dom.value = val;
            }
        });
        [nr.domSeOnly].forEach(function (dom) {
            if (dom.defaultValue == null) {
                dom.defaultValue = dom.value;
                dom.addEventListener('sl-change', page.codeBuild);
            }
            var val = config[dom.classList[0]];
            if (val != null) {
                dom.value = val + "";
            }
        });
    },
    codeBuild: function () {
        var vm = { err: [], data: [] };
        try {
            var r1 = nr.domTxtRange1.value * 1,
                r2 = nr.domTxtRange2.value * 1,
                count = nr.domTxtCount.value * 1,
                isOnly = nr.domSeOnly.value * 1,
                group = (nr.domTxtGroup.value * 1) || 0;

            var config = {
                'nr-txt-range1': r1,
                'nr-txt-range2': r2,
                'nr-txt-count': count,
                'nr-txt-group': group,
                'nr-se-only': isOnly,
            }

            if (isNaN(r1) || isNaN(r2) || isNaN(count)) {
                vm.err.push("请输入有效的数字");
            }
            if (r1 > r2) {
                vm.err.push("随机范围有误");
            }
            if (r2 - r1 < (count - 1) && isOnly == 1) {
                vm.err.push("随机个数须小于等于范围数量");
            }

            if (!vm.err.length) {
                var rr = r2 - r1, rv = [];

                while (rv.length < count) {
                    var ri = Math.floor(Math.random() * (rr + 1));
                    ri = r1 + ri;
                    if (isOnly == 1 && rv.indexOf(ri) >= 0) {
                        continue;
                    } else {
                        rv.push(ri);
                    }
                }

                var ni = 0, ci = 1;
                while (ni++ < rv.length) {
                    if (ni == rv.length) {
                        continue;
                    }
                    if (ci++ == group) {
                        ci = 1;
                        rv.splice(ni++, 0, "\r\n");
                    } else {
                        rv.splice(ni++, 0, "\t");
                    }
                }
                vm.data = rv;

                nr.ls['config'] = config;
                nr.lsSave();
            }
        } catch (e) {
            console.log(e);
            vm.err.push("操作太骚，报错了")
        }

        if (vm.err.length) {
            nr.alert(vm.err.join("<br/>"));
        } else {
            nr.domTxtResult.value = vm.data.join("");
        }
    },
};