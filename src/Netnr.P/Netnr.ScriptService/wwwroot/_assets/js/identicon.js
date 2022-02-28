
var ii = {
    init: function () {
        $('.nr-size').on('input', function () {
            ii.build();
        });
        $('.nr-value').on('input', function () {
            ii.build();
        });

        $('.nr-down').click(function () {
            var type = this.getAttribute('data-type');
            switch (type) {
                case "svg1":
                    ss.dowload($('.nr-view-1').html(), "identicon.svg");
                    break;
                case "image1":
                    $('.nr-view-1').addClass('border-transparent');
                    html2canvas($('.nr-view-1')[0]).then(function (canvas) {
                        ss.dowload(canvas, "identicon.jpg");
                        $('.nr-view-1').removeClass('border-transparent')
                    })
                    break;
                case "svg2":
                    ss.dowload($('.nr-view-2').html(), "identicon.svg");
                    break;
                case "image2":
                    $('.nr-view-2').addClass('border-transparent');
                    html2canvas($('.nr-view-2')[0]).then(function (canvas) {
                        ss.dowload(canvas, "identicon.jpg");
                        $('.nr-view-2').removeClass('border-transparent');
                    })
                    break;
            }
        });

        ii.build();
    },

    build: function () {
        window.clearTimeout(ii.defer);

        ii.defer = setTimeout(function () {
            var value = $('.nr-value').val();
            var size = $('.nr-size').val() || 320;

            var svgicon = iisvg({ value, size });
            $('.nr-view-1').empty().append(svgicon).parent().removeClass('d-none');

            var o2 = jdenticon.toSvg(value, size);
            $('.nr-view-2').html(o2).parent().removeClass('d-none');
        }, 500)
    }
}

ii.init();