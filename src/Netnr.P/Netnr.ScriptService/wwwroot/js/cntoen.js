$('#txtChinese').keyup(function () {
    var select = document.getElementById('sePinyin');
    select.options.length = 0;
    if (this.value != "") {
        var arrPY = PY.QueryArray(this.value);
        for (var i = 0; i < arrPY.length; i++) {
            select.options.add(new Option(arrPY[i], arrPY[i]))
        }
    }
});