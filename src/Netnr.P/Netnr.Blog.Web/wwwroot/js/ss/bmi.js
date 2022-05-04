nr.onReady = function () {
    nr.domTxtNumber1.addEventListener('input', function () {
        page.bmi();
    });
    nr.domTxtNumber2.addEventListener('input', function () {
        page.bmi();
    });
}

var page = {
    bmi: function () {
        var height = nr.domTxtNumber1.value * 1;
        var weight = nr.domTxtNumber2.value * 1;

        var bmi = (weight / Math.pow(height / 100, 2)).toFixed(2);
        page.view(bmi);
    },
    view: function (bmi) {
        var remark = "", color = "";
        if (bmi < 18.5) {
            remark = "低重";
            color = "neutral";
        } else if (bmi < 24) {
            remark = "正常";
            color = "success";
        } else if (bmi < 28) {
            remark = "超重";
            color = "warning";
        } else {
            remark = "肥胖";
            color = "danger";
        }

        nr.domTxtNumber3.value = bmi + " " + remark;
        nr.domTxtNumber3.style.color = `var(--sl-color-${color}-600)`;
    }
}