let nrcRandomatic = {
    /**
     * 生成
     * https://github.com/jonschlinkert/randomatic
     * @param {any} pattern 源
     * @param {any} length 长度
     * @param {any} options 选项
     */
    randomize: function (pattern, length, options) {
        let tobj = {
            lower: 'abcdefghijklmnopqrstuvwxyz',
            upper: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ',
            number: '0123456789',
            special: "~!@#$%^&()_+-={}[];',."
        };
        let tall = Object.values(tobj).join('');

        let custom = false;
        if (length == null && options == null) {
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

        let opts = options || {};
        let mask = '';
        let res = '';

        // 包含的字符串
        if (pattern.indexOf('?') !== -1) mask += opts.chars;
        if (pattern.indexOf('a') !== -1) mask += tobj.lower;
        if (pattern.indexOf('A') !== -1) mask += tobj.upper;
        if (pattern.indexOf('0') !== -1) mask += tobj.number;
        if (pattern.indexOf('!') !== -1) mask += tobj.special;
        if (pattern.indexOf('*') !== -1) mask += tall;
        if (custom) mask += pattern;

        // 排除字符串
        if (opts.exclude) {
            let exclude = typeof opts.exclude === 'string' ? opts.exclude : opts.exclude.join('');
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
        let tj = `
            // 自定义字符串

            // 将从中包含的字母生成20个字符的随机字符串jonschlinkert
            randomize('?', 20, {chars: 'jonschlinkert'})

            // 将根据中包含的字母生成13个字符的随机字符串jonschlinkert
            randomize('?', {chars: 'jonschlinkert'})

            // ---

            // 指定一个字符串或字符数组可以从用于生成随机字符串的可能字符中排除

            // 将使用所有可能的字符（排除0oOiIlL1）生成一个20个字符的随机字符串
            randomize('*', 20, { exclude: '0oOiIlL1' })

            // ---

            // （对空格敏感）随机生成的4位大写字母，例如ZAKH，UJSL...等
            randomize('A', 4)

            // 相当于 randomize('A', 4)
            randomize('AAAA')

            // 相同于 randomize('AA00')、randomize('A0A0')、randomize('A0', 4)
            randomize('AAA0')

            // 生成两位数，随机，小写字母（abcdefghijklmnopqrstuvwxyz）
            randomize('aa')

            // 生成三位数的随机大写字母（ABCDEFGHIJKLMNOPQRSTUVWXYZ）
            randomize('AAA')

            // 生成六位数的随机数（0123456789）
            randomize('0', 6)

            // 生成一位随机有效的非字母字符（\`〜！@＃$％^＆（）_ +-= { } []
            randomize('!', 5)

            // 生成9位数的随机字符（以上任意一项）
            randomize('A!a0', 9)
     `;
        let tjs = tj.split('\n');
        for (let i = 0; i < tjs.length; i++) {
            let ti = tjs[i].trim();
            let tw = ti;
            if (ti.indexOf('//') == 0) {
                tw = tw.substring(2).trim();
            }
            console.log(tw);
            if (ti.indexOf('//') != 0 && ti != "") {
                console.debug('RESULT：' + eval('(' + ti + ')'));
            }
        }

        return 'Complete';
    }
};

export { nrcRandomatic }