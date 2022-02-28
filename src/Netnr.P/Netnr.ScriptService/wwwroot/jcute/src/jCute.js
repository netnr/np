/*!
 * jCute JavaScript Library
 *
 * Date: 2018-05
 * Author: netnr
 */

var jCute = function (selector) { return new jCute.fn.init(selector); };

jCute.fn = jCute.prototype = {
    init: function (selector) {
        //TO DO

        return this;
    }
};

jCute.fn.init.prototype = jCute.fn;

window.jCute = window.cu = jCute;

