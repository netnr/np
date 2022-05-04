nr.onReady = function () {
    page.build();

    nr.domTxtSign.addEventListener('input', function () {
        page.build();
    });
    nr.domTxtSize.addEventListener('input', function () {
        page.build();
    });

    document.addEventListener('click', function (e) {
        var target = e.target, action = target.getAttribute('data-action');
        switch (action) {
            case "svg1":
                nr.download(nr.domCardView1.innerHTML, "identicon.svg");
                break;
            case "image1":
                nr.domCardView1.classList.toggle('border');
                html2canvas(nr.domCardView1).then(function (canvas) {
                    nr.download(canvas, "identicon.jpg");
                    nr.domCardView1.classList.toggle('border');
                })
                break;
            case "svg2":
                nr.download(nr.domCardView2.innerHTML, "identicon.svg");
                break;
            case "image2":
                nr.domCardView2.classList.toggle('border');
                html2canvas(nr.domCardView2).then(function (canvas) {
                    nr.download(canvas, "identicon.jpg");
                    nr.domCardView2.classList.toggle('border');
                })
                break;
        }
    });
}

var page = {
    build: function () {
        clearTimeout(page.defer);

        page.defer = setTimeout(function () {
            var value = nr.domTxtSign.value;
            var size = nr.domTxtSize.value * 1 || 320;

            var svg1 = iisvg({ value, size });
            nr.domCardView1.innerHTML = '';
            nr.domCardView1.classList.add('border');
            nr.domCardView1.appendChild(svg1);
            nr.domCardView1.parentNode.classList.remove('d-none');

            var svg2 = jdenticon.toSvg(value, size);
            nr.domCardView2.innerHTML = svg2;
            nr.domCardView2.classList.add('border');
            nr.domCardView2.parentNode.classList.remove('d-none');
        }, 500)
    }
}