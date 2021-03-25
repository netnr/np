/*
 * netnrnav v1.1.1
 * 
 * Fork: https://github.com/zhoufengjob/SuiNav
 * 
 * Date: 2019-08-18 - 2021-03-12
 * 
 */

(function (window) {

    var netnrnav = function (se) { return new netnrnav.fn.init(se); };

    netnrnav.fn = netnrnav.prototype = {
        init: function (se) {
            var that = this;
            this.ele = $(se);
            this.eventCount = 0;
            this.isHiding = false;

            if (that.ele.hasClass('horizontal')) {
                that.ele.find('li').hover(function () {
                    $(this).children('ul').show();
                }, function () {
                    $(this).children('ul').hide();
                });
            } else {
                that.ele.find('li').click(function () {
                    if (that.eventCount != 0) {
                        if ($(this).parent().parent().parent().hasClass('netnrnav')) {
                            that.eventCount = 0;
                        }
                        return;
                    }
                    if ($(this).children('ul').is(":hidden"))
                        $(this).children('ul').show();
                    else {
                        $(this).find('ul').hide();
                    }
                    that.eventCount++;
                    if ($(this).parent().parent().parent().hasClass('netnrnav')) {
                        that.eventCount = 0;
                    }
                });
            }
            return this;
        }
    };

    netnrnav.show = function (that) {
        if (!that.isHiding) {
            $(document.body).append('<div class="netnrnav slide-netnrnav"></div><div class="netnrnav netnrnav-mask"></div>');
            $('.slide-netnrnav').html(that.ele.html()).find('li').click(function () {
                if (that.eventCount != 0) {
                    if ($(this).parent().parent().parent().hasClass('netnrnav')) {
                        that.eventCount = 0;
                    }
                    return;
                }
                if ($(this).children('ul').is(":hidden"))
                    $(this).children('ul').show();
                else {
                    $(this).find('ul').hide();
                }
                that.eventCount++;
                if ($(this).parent().parent().parent().hasClass('netnrnav')) {
                    that.eventCount = 0;
                }
            });
            $('.netnrnav-mask').click(function () {
                netnrnav.hide(that);
            });
            setTimeout(function () {
                $('.slide-netnrnav').toggleClass('active');
                $('.netnrnav-mask').toggleClass('active');
            }, 20);
        }
    };

    netnrnav.hide = function (that) {
        if (!that.isHiding) {
            that.isHiding = true;
            $('.slide-netnrnav').find('li').unbind();
            $('.slide-netnrnav').removeClass('active');
            $('.netnrnav-mask').removeClass('active');
            setTimeout(function () {
                $('.slide-netnrnav').remove();
                $('.netnrnav-mask').remove();
                that.isHiding = false;
            }, 600);
        }
    };

    netnrnav.toggle = function (that) {
        $('.slide-netnrnav').length > 0 ? netnrnav.hide(that) : netnrnav.show(that)
    }

    netnrnav.fn.init.prototype = netnrnav.fn;

    window.netnrnav = netnrnav;

})(window);

$(function () {
    $.nrnav = netnrnav(".netnrnav");
    $('.MenuToggle').click(function () {
        netnrnav.toggle($.nrnav);
    });
});