var ids = ['txt1', 'txt2'];
for (var i = 0; i < ids.length; i++) {
    var id = ids[i];
    document.getElementById(id).oninput = function () {
        var kg = document.getElementById('txt1').value,
            m = document.getElementById('txt2').value;
        kg = parseFloat(kg) || 0;
        m = parseFloat(m) || 0;
        var bmi = '', bgc = '';
        if (kg && m) {
            m = m / 100;
            bmi = (kg / Math.pow(m, 2)).toFixed(2);
            if (bmi >= 28) {
                bgc = 'danger';
            }
            if (bmi < 28) {
                bgc = 'warning';
            }
            if (bmi < 24) {
                bgc = 'success';
            }
            if (bmi < 18.5) {
                bgc = 'dark';
            }
            bgc = 'text-white bg-' + bgc;
        }
        var t3 = document.getElementById('txt3');
        t3.value = bmi;
        t3.className = 'form-control ' + bgc;
    }
}