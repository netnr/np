/**
 * 2020-06-13
 * netnr
 */
var rc = {
    type: {
        lower: 'abcdefghijklmnopqrstuvwxyz',
        upper: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ',
        number: '0123456789',
        special: '~!@#$%^&()_+-={}[];\',.'
    },

    /**
     * 生成
     * https://github.com/jonschlinkert/randomatic
     * @param {any} pattern 源
     * @param {any} length 长度
     * @param {any} options 选项
     */
    randomize: function (pattern, length, options) {
        var type = this.type;
        if (!type.all) {
            type.all = type.lower + type.upper + type.number + type.special;
        }

        var custom = false;
        if (arguments.length === 1) {
            if (typeof pattern === 'string') {
                length = pattern.length;

            } else if (typeof pattern === "number") {
                options = {};
                length = pattern;
                pattern = '*';
            }
        }

        if (typeof length === 'object' && length.hasOwnProperty('chars')) {
            options = length;
            pattern = options.chars;
            length = pattern.length;
            custom = true;
        }

        var opts = options || {};
        var mask = '';
        var res = '';

        // 包含的字符串
        if (pattern.indexOf('?') !== -1) mask += opts.chars;
        if (pattern.indexOf('a') !== -1) mask += type.lower;
        if (pattern.indexOf('A') !== -1) mask += type.upper;
        if (pattern.indexOf('0') !== -1) mask += type.number;
        if (pattern.indexOf('!') !== -1) mask += type.special;
        if (pattern.indexOf('*') !== -1) mask += type.all;
        if (custom) mask += pattern;

        // 排除字符串
        if (opts.exclude) {
            var exclude = typeof opts.exclude === 'string' ? opts.exclude : opts.exclude.join('');
            exclude = exclude.replace(new RegExp('[\\]]+', 'g'), '');
            mask = mask.replace(new RegExp('[' + exclude + ']+', 'g'), '');

            if (opts.exclude.indexOf(']') !== -1) mask = mask.replace(new RegExp('[\\]]+', 'g'), '');
        }

        while (length--) {
            res += mask.charAt(parseInt(Math.random() * mask.length, 10));
        }
        return res;
    },

    /**
     * 测试
     */
    test: function () {
        var tj = `
                // 自定义字符串

                // 将从中包含的字母生成20个字符的随机字符串jonschlinkert
                rc.randomize('?', 20, {chars: 'jonschlinkert'})

                // 将根据中包含的字母生成13个字符的随机字符串jonschlinkert
                rc.randomize('?', {chars: 'jonschlinkert'})

                // ---

                // 指定一个字符串或字符数组可以从用于生成随机字符串的可能字符中排除

                // 将使用所有可能的字符（排除0oOiIlL1）生成一个20个字符的随机字符串
                rc.randomize('*', 20, { exclude: '0oOiIlL1' })

                // ---

                // （对空格敏感）随机生成的4位大写字母，例如ZAKH，UJSL...等
                rc.randomize('A', 4)

                // 相当于 randomize('A', 4)
                rc.randomize('AAAA')

                // 相同于 randomize('AA00')、randomize('A0A0')、randomize('A0', 4)
                rc.randomize('AAA0')

                // 生成两位数，随机，小写字母（abcdefghijklmnopqrstuvwxyz）
                rc.randomize('aa')

                // 生成三位数的随机大写字母（ABCDEFGHIJKLMNOPQRSTUVWXYZ）
                rc.randomize('AAA')

                // 生成六位数的随机数（0123456789）
                rc.randomize('0', 6)

                // 生成一位随机有效的非字母字符（\`〜！@＃$％^＆（）_ +-= { } []
                rc.randomize('!', 5)

                // 生成9位数的随机字符（以上任意一项）
                rc.randomize('A!a0', 9) 
         `;
        var tjs = tj.split('\n');
        for (var i = 0; i < tjs.length; i++) {
            var ti = tjs[i].trim(), tw = ti;
            if (ti.indexOf('//') == 0) {
                tw = tw.substr(2).trim();
            }
            console.log(tw);
            if (ti.indexOf('//') != 0 && ti != "") {
                console.warn('RESULT：' + eval('(' + ti + ')'));
            }
        }

        return 'Complete';
    }
};


/**
 * 页面js
 */
try {
    var cg = ss.lsObj('config');
    $.each('.nrLen .nrExclude #chkUpper #chkLower #chkNumber #chkSymbol'.split(' '), function () {
        if (this.substr(1) in cg) {
            var jn = $('' + this);
            if (this.indexOf('chk') >= 0) {
                jn[0].checked = cg[this.substr(1)] != "";
            } else {
                jn.val(cg[this.substr(1)])
            }
        }
    });
} catch (e) { }

$('.nrGk').click(function () {
    var mm = '', len = $('.nrLen').val(), te = $('.nrExclude').val().trim();
    $('input[type="checkbox"]:checked').each(function () {
        mm += this.value;
    })
    if (mm != '' && len > 0) {
        $('.nrRcv').val(rc.randomize(mm, len, { exclude: te == "" ? null : te }));

        var obj = {};
        $.each('.nrLen .nrExclude #chkUpper #chkLower #chkNumber #chkSymbol'.split(' '), function () {
            var jn = $('' + this), val = jn.val();
            if (this.indexOf('chk') >= 0) {
                val = jn[0].checked ? val : "";
            }
            obj[this.substr(1)] = val;
        });
        ss.ls.config = obj;
        ss.lsSave();
    }
})
$('.nrLen').on('input', function () {
    $('.nrGk')[0].click();
})
$('.nrExclude').on('input', function () {
    $('.nrGk')[0].click();
})
$('input[type="checkbox"]').click(function () {
    $('.nrGk')[0].click();
})
$('.nrGk')[0].click();
$('.nrRcv').focus(function () {
    this.select();
})