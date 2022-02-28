var rm = {
    random: function (r1, r2, num, isUnique, newLine) {
        var vm = { err: [], data: [] };
        try {
            r1 = parseInt(r1);
            r2 = parseInt(r2);
            num = parseInt(num);
            isUnique = parseInt(isUnique);
            newLine = parseInt(newLine) || 0;

            if (isNaN(r1) || isNaN(r2) || isNaN(num)) {
                vm.err.push("请输入有效的数字");
            }
            if (r1 > r2) {
                vm.err.push("随机范围有误");
            }
            if (r2 - r1 < (num - 1) && isUnique == 1) {
                vm.err.push("随机个数须小于等于范围数量");
            }

            if (!vm.err.length) {
                var rr = r2 - r1, rv = [];

                while (rv.length < num) {
                    var ri = Math.floor(Math.random() * (rr + 1));
                    ri = r1 + ri;
                    if (isUnique == 1 && rv.indexOf(ri) >= 0) {
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
                    if (ci++ == newLine) {
                        ci = 1;
                        rv.splice(ni++, 0, "\r\n");
                    } else {
                        rv.splice(ni++, 0, "\t");
                    }
                }
                vm.data = rv;

                var obj = {};
                $.each('.nrRange1 .nrRange2 .nrGetNum .nrRepeat .nrNewLine'.split(' '), function () {
                    var jn = $('' + this), val = jn.val();
                    obj[this.substr(1)] = val;
                });
                ss.ls.config = obj;
                ss.lsSave();
            }
        } catch (e) {
            console.log(e);
            vm.err.push("操作太骚，报错了")
        }
        return vm;
    }
};

try {
    var cg = ss.lsObj('config');
    $.each('.nrRange1 .nrRange2 .nrGetNum .nrRepeat .nrNewLine'.split(' '), function () {
        if (this.substr(1) in cg) {
            var jn = $('' + this);
            jn.val(cg[this.substr(1)])
        }
    });
} catch (e) { }

$('.nrRm').click(function () {
    var vm = rm.random($('.nrRange1').val(), $('.nrRange2').val(), $('.nrGetNum').val(), $('.nrRepeat').val(), $('.nrNewLine').val());
    console.log(vm);
    if (vm.err.length) {
        $('.nrRv').val(vm.err.join('\r\n'));
    } else {
        $('.nrRv').val(vm.data.join(''));
    }
})[0].click();

$('.nrRange1,.nrRange2,.nrGetNum,.nrRepeat,.nrNewLine').on('input', function () {
    $('.nrRm')[0].click();
})

$('.nrReset').click(function () {
    $.each('.nrRange1 .nrRange2 .nrGetNum .nrRepeat .nrNewLine'.split(' '), function () {
        var jn = $('' + this);
        if (jn[0].type == "select-one") {
            jn[0].selectedIndex = 0;
        } else {
            jn.val(jn[0].defaultValue);
        }
    });
    $('.nrRm')[0].click();
});