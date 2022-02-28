$('#btnOk').click(function () {
    if ($('#txt1').val() != "") {
        $('#btnOk').html('稍等');
        $('#btnOk')[0].disabled = true;
        $.ajax({
            url: `${ss.apiServer}/api/v1/Analysis`,
            type: 'POST',
            data: {
                ctype: $('#sem').val(),
                content: $('#txt1').val()
            },
            dataType: 'json',
            success: function (data) {
                console.log(data);
                var result = '';
                if (data.code == 200) {
                    switch ($('#sem').val()) {
                        case "0":
                            result = data.data.join('\n');
                            break;
                        case "1":
                            {
                                var arr = [];
                                $.each(data.data, function () {
                                    arr.push(this.Word + " ：" + this.Weight.toFixed(5));
                                })
                                result = arr.join('\n');
                            }
                            break;
                    }
                } else {
                    result = JSON.stringify(data, null, 4);
                }
                $('#txt2').val(result);
            },
            error: function () {
                $('#txt2').val("网络错误");
            },
            complete: function () {
                $('#btnOk').html('提交');
                $('#btnOk')[0].disabled = false;
            }
        })
    }
})