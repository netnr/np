function loadOSinfo() {
    $.ajax({
        url: "/Mix/AboutServerStatus",
        type: 'post',
        data: {
            __nolog: "true",
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        dataType: 'json',
        success: function (data, _status, xhr) {

            var arh = xhr.getAllResponseHeaders();
            data = data.data;

            var htm = [];
            htm.push('<table class="table table-bordered">');

            htm.push('<tr>');
            htm.push('<td>运行</td>');
            htm.push('<td>' + document.getElementById("hid_rt").value + ' Days</td>');
            htm.push('</tr>');

            arh.replace(/server: (.*)/, function () {
                htm.push('<tr>');
                htm.push('<td>服务</td>');
                htm.push('<td>' + arguments[1] + '</td>');
                htm.push('</tr>');
            })

            htm.push('<tr>');
            htm.push('<td>框架</td>');
            htm.push('<td>' + data.FrameworkDescription + '</td>');
            htm.push('</tr>');

            if (data.Model && data.Model != "System Product Name") {
                htm.push('<tr>');
                htm.push('<td>型号</td>');
                htm.push('<td>' + data.Model + '</td>');
                htm.push('</tr>');
            }

            htm.push('<tr>');
            htm.push('<td>系统</td>');
            htm.push('<td>' + (data.OperatingSystem || data.OS) + ' ，' + data.OSVersion.VersionString + ' ，' + data.UserName + (data.Is64BitOperatingSystem ? " ，64Bit" : "") + '</td>');
            htm.push('</tr>');

            htm.push('<tr>');
            htm.push('<td>CPU</td>');
            htm.push('<td>' + data.ProcessorName + ' ，' + data.ProcessorCount + ' Core</td>');
            htm.push('</tr>');

            htm.push('<tr>');
            htm.push('<td>内存</td>');
            var p1 = (data.FreePhysicalMemory / 1024 / 1024 / 1024).toFixed(1), p2 = (data.TotalPhysicalMemory / 1024 / 1024 / 1024).toFixed(1);
            var p3 = ((p2 - p1) / p2 * 100).toFixed(1);
            var pp = '<div class="progress mt-2"><div class="progress-bar bg-warning" aria-valuenow="' + p3 + '" aria-valuemin="0" aria-valuemax="100" style="width: ' + p3 + '%;">' + p3 + '%</div></div>';
            htm.push('<td>' + pp + (p2 - p1).toFixed(1) + ' / ' + p2 + ' GB</td>');
            htm.push('</tr>');

            if (data.SwapTotal) {
                htm.push('<tr>');
                htm.push('<td>Swap</td>');
                var p1 = (data.SwapFree / 1024 / 1024 / 1024).toFixed(1), p2 = (data.SwapTotal / 1024 / 1024 / 1024).toFixed(1);
                var p3 = ((p2 - p1) / p2 * 100).toFixed(1);
                var pp = '<div class="progress"><div class="progress-bar bg-warning" aria-valuenow="' + p3 + '" aria-valuemin="0" aria-valuemax="100" style="width: ' + p3 + '%;">' + p3 + '%</div></div>';
                htm.push('<td>' + pp + (p2 - p1).toFixed(1) + ' / ' + p2 + ' GB</td>');
                htm.push('</tr>');
            }

            htm.push('<tr>');
            htm.push('<td>磁盘</td>');
            htm.push('<td>');
            $.each(data.LogicalDisk, function () {
                var diskitem = this;
                p1 = (diskitem.FreeSpace / 1024 / 1024 / 1024).toFixed(0);
                p2 = (diskitem.Size / 1024 / 1024 / 1024).toFixed(0);
                p3 = ((p2 - p1) / p2 * 100).toFixed(0);
                pp = '<div class="progress mt-2"><div class="progress-bar bg-warning" aria-valuenow="' + p3 + '" aria-valuemin="0" aria-valuemax="100" style="width: ' + p3 + '%;">' + p3 + '%</div></div>';
                var dn = diskitem.Name || "";
                dn = dn.length > 20 ? dn.substring(0, 20) + "..." : dn;
                if (diskitem.VolumeName) {
                    dn = diskitem.VolumeName + " (" + dn + ")";
                }
                htm.push(pp + '<div>' + dn + ' &nbsp; ' + (p2 - p1) + ' / ' + p2 + ' GB</div>');
            });
            htm.push('</td>');
            htm.push('</tr>');

            htm.push('<tr>');
            htm.push('<td>开机</td>');

            var p1 = new Date(data.TickCount - 8 * 1000 * 3600);
            var p2 = (p1.getFullYear() - 1970).toString();
            while (p2.length < 4) {
                p2 = "0" + p2;
            }
            var p3 = p1.getMonth() + 1 - 1;
            p3 < 10 && (p3 = "0" + p3);
            var p4 = p1.getDate() - 1;
            p4 < 10 && (p4 = "0" + p4);
            var p5 = p1.getHours();
            p5 < 10 && (p5 = "0" + p5);
            var p6 = p1.getMinutes();
            p6 < 10 && (p6 = "0" + p6);
            var p7 = p1.getSeconds();
            p7 < 10 && (p7 = "0" + p7);

            var pp = p2 + "-" + p3 + "-" + p4 + " " + p5 + ":" + p6 + ":" + p7;

            htm.push('<td style="letter-spacing:1px">' + pp + '</td>');
            htm.push('</tr>');

            htm.push('</table>');
            $('#divAs').html(htm.join(''));

            //自动刷新
            setTimeout(loadOSinfo, 1000 * 10);
        },
        error: function () {
            $('#divAs').html('<h4 class="text-center text-danger">获取服务器信息异常</h4>');

            //自动刷新
            setTimeout(loadOSinfo, 1000 * 10);
        }
    });
}

loadOSinfo();