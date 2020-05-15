var colors = "286CC1 F4463D FDC027 0DC969 FF7300 B034AF 5FE3D6 529756 01532F FDC7D4",
    color = function () { return colors.split(' ')[Math.ceil(Math.random() * 10) - 1] };

function creatediv(id) {
    var htm = '<div style="background-color:#' + color() + ';height:' + (Math.random() * 200 + 90) + 'px"> ' + Math.random() + ' </div>'
        + '<div style="background-color:#' + color() + ';height:' + (Math.random() * 220 + 80) + 'px"> ' + Math.random() + ' </div>'
        + '<div style="background-color:#' + color() + ';height:' + (Math.random() * 260 + 60) + 'px"> ' + Math.random() + ' </div>'
        + '<div style="background-color:#' + color() + ';height:' + (Math.random() * 290 + 40) + 'px"> ' + Math.random() + ' </div>';
    document.getElementById(id).innerHTML = htm;
}


for (var i = 1; i < 4; i++) {
    creatediv("jc" + i);

    //调用插件
    jc.init({ id: "jc" + i });
}