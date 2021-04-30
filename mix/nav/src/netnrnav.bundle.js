/*
 * Fork: https://github.com/zhoufengjob/SuiNav
 * 
 * Date: 2019-08-18 - 2021-04-27
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

    netnrnav.css = `.netnrnav{position:relative;color:#333;z-index:10}.netnrnav a{color:#333;text-decoration:none}.netnrnav a:active,.netnrnav a:hover{color:#333;text-decoration:none}.netnrnav a:focus{outline:0;text-decoration:none}@media (max-width:768px){.netnrnav{display:none}}.netnrnav .netnrnav-wrapper-fluid>ul,.netnrnav .netnrnav-wrapper>ul{padding-left:0;margin-bottom:15px}.netnrnav .netnrnav-wrapper-fluid>ul.float-right,.netnrnav .netnrnav-wrapper>ul.float-right{float:none!important}.netnrnav ul li a{width:100%;height:100%;white-space:nowrap;display:inline-block}.netnrnav ul li a:hover{color:orange;background-color:#f5f5f5}.netnrnav ul{cursor:pointer;list-style:none;padding-left:15px}.netnrnav ul>li{line-height:45px;position:relative}.netnrnav ul>li.divider{height:1px;margin:9px 0;overflow:hidden;background-color:#e5e5e5}.netnrnav ul>li>a{padding:0 15px}.netnrnav ul>li span.indicator{width:15px;float:right;margin-top:10px;text-align:right}.netnrnav li>ul{display:none}.netnrnav li>ul>li{float:none;position:relative}.netnrnav li>ul>li:hover{background-color:#fff}.netnrnav li.active>ul{display:block}@media (max-width:768px){.hide-in-mobile{display:none}}.show-in-mobile{display:none!important}@media (max-width:768px){.show-in-mobile{display:block!important}}.netnrnav .show-in-horizontal{display:none}.netnrnav.slide-netnrnav{position:fixed;top:0;left:0;z-index:9999;background-color:#fff;width:100%;height:100%;overflow-y:auto;box-shadow:none;-webkit-transform:translateX(-100%);-ms-transform:translateX(-100%);transform:translateX(-100%);-webkit-transition:-webkit-transform .4s,box-shadow .4s;transition:transform .4s,box-shadow .4s;max-width:260px}@media (max-width:768px){.netnrnav.slide-netnrnav{display:block}}.netnrnav.slide-netnrnav.active{box-shadow:0 2px 8px rgba(0,0,0,.8);-webkit-transform:translateX(0);-ms-transform:translateX(0);transform:translateX(0)}.netnrnav.netnrnav-mask{display:block;position:fixed;top:-50%;left:-50%;z-index:9090;width:200%;height:200%;background-color:rgba(0,0,0,.8);visibility:hidden;opacity:0;-webkit-transition:opacity .4s,visibility .4s;transition:opacity .4s,visibility .4s}.netnrnav.netnrnav-mask.active{visibility:visible;opacity:.6}.netnrnav.horizontal{width:100%;clear:both;z-index:999;min-height:48px;border-top:3px solid orange;box-shadow:0 0 8px 0 rgba(0,0,0,.1),0 1px rgba(0,0,0,.1)}.netnrnav.horizontal ul{padding:0!important;background-color:#fff;margin-bottom:0}.netnrnav.horizontal ul>li{line-height:45px}.netnrnav.horizontal .netnrnav-wrapper{margin:0 auto;max-width:99%}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul,.netnrnav.horizontal .netnrnav-wrapper>ul{position:relative;float:left;list-style:none;background-color:transparent}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul>li,.netnrnav.horizontal .netnrnav-wrapper>ul>li{position:relative;float:left;cursor:pointer;border-bottom:none;min-width:initial}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul>li>a,.netnrnav.horizontal .netnrnav-wrapper>ul>li>a{padding:0 15px}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul>li>ul,.netnrnav.horizontal .netnrnav-wrapper>ul>li>ul{float:none;position:absolute;left:0;border:1px solid #e5e5e5}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul>li>ul>li,.netnrnav.horizontal .netnrnav-wrapper>ul>li>ul>li{float:none}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul>li>ul>li ul,.netnrnav.horizontal .netnrnav-wrapper>ul>li>ul>li ul{position:absolute;left:100%;top:0;border:1px solid #e5e5e5}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul.float-right,.netnrnav.horizontal .netnrnav-wrapper>ul.float-right{float:right!important}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul.float-right>li>ul,.netnrnav.horizontal .netnrnav-wrapper>ul.float-right>li>ul{right:0;left:auto}.netnrnav.horizontal .netnrnav-wrapper-fluid>ul.float-right>li>ul>li ul,.netnrnav.horizontal .netnrnav-wrapper>ul.float-right>li>ul>li ul{left:-101%}.netnrnav.horizontal .show-in-horizontal{display:block!important}.netnrnav.horizontal .hide-in-horizontal{display:none!important}`;

    netnrnav.addStyle = function (css) {
        var s = document.createElement('style');
        s.innerHTML = css;
        document.head.appendChild(s);
    }
    netnrnav.addStyle(netnrnav.css);

})(window);

$(function () {
    $.nrnav = netnrnav(".netnrnav");
    $('.MenuToggle').click(function () {
        netnrnav.toggle($.nrnav);
    });
});