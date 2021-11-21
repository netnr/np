﻿function Alert(msg) {
    var ta = $('#myModalBody').find('textarea');
    var nowTime = new Date().valueOf(), preTime = window.preTime || nowTime;
    window.preTime = nowTime;
    if (nowTime - preTime < 5000) {
        ta.val(ta.val() + msg + '\n\n');
    } else {
        ta.val(msg + '\n\n');
    }
    $('#myModal').modal();
}


var $wrap = $('#uploader'),

    // 图片容器
    $queue = $('<ul class="filelist"></ul>')
        .appendTo($wrap.find('.queueList')),

    // 状态栏，包括进度和控制按钮
    $statusBar = $wrap.find('.statusBar'),

    // 文件总体选择信息。
    $info = $statusBar.find('.info'),

    // 上传按钮
    $upload = $wrap.find('.uploadBtn'),

    // 没选择文件之前的内容。
    $placeHolder = $wrap.find('.up-placeholder'),

    $progress = $statusBar.find('.progress').hide(),

    // 添加的文件数量
    fileCount = 0,

    // 添加的文件总大小
    fileSize = 0,

    // 优化retina, 在retina下这个值是2
    ratio = window.devicePixelRatio || 1,

    // 缩略图大小
    thumbnailWidth = 110 * ratio,
    thumbnailHeight = 110 * ratio,

    // 可能有pedding, ready, uploading, confirm, done.
    state = 'pedding',

    // 所有文件的进度信息，key为file id
    percentages = {},

    // WebUploader实例
    uploader = WebUploader.create({
        pick: {
            id: '#filePicker',
            label: '选择文件'
        },
        formData: {
        },
        dnd: '#dndArea',
        paste: document.body,
        _chunked: true,
        _chunkSize: 2 * 1024 * 1024, // 切片 2 M
        _chunkRetry: 1,
        threads: 1,
        server: '',
        //禁用图片压缩
        compress: false,
        // runtimeOrder: 'flash',

        //accept: {
        //    title: 'Images',
        //    extensions: 'gif,jpg,jpeg,bmp,png',
        //    mimeTypes: 'image/*'
        //},

        // 禁掉全局的拖拽功能。这样不会出现图片拖进页面的时候，把图片打开。
        disableGlobalDnd: true,
        fileNumLimit: 999,
        fileSizeLimit: 500 * 1024 * 1024,    // 500 M
        fileSingleSizeLimit: 50 * 1024 * 1024    // 50 M
    });


// 添加“添加文件”的按钮，
uploader.addButton({
    id: '#filePicker2',
    label: '继续添加'
});

//单个文件上传时
uploader.on('uploadStart', function (file) {
    var up = $('#seup').val();
    uploader.options.server = up;
    switch (up) {
        case "https://imgbb.com/json":
            {
                uploader.options.fileVal = "source";

                uploader.options.formData.auth_token = uploader.token_imgbbcom;
                uploader.options.formData.timestamp = new Date().valueOf();
                uploader.options.formData.action = "upload";

                file.type = "file";
            }
            break;
        case "https://uploadbeta.com/api/pictures/upload/file/":
            {
                uploader.options.formData.title = "netnr-bed";
                uploader.options.formData.desc = "This picture is uploaded from netnr";
                uploader.options.formData.cat = "netnr-category";
                uploader.options.formData.group = "netnr-group";
            }
            break;
    }
});

//上传成功
uploader.on('uploadSuccess', function (file, res) {
    console.log(res);

    var vurl, up = $('#seup').val();

    switch (up) {
        case "https://imgbb.com/json":
            {
                if (res.status_code == 200) {
                    vurl = res.image.url
                }
            }
            break;
        case "https://uploadbeta.com/api/pictures/upload/file/":
            {
                if (res.img) {
                    vurl = res.img
                }
                else if (res.error.length < 15 && res.error.indexOf('-') >= 0) {
                    vurl = "https://uploadbeta.com/share-image/" + res.error.split('-')[1];
                }
            }
            break;
    }

    if (vurl) {
        $('#divnt').append(`
            <div class="input-group mt-3">
	            <div>
                    <a class="btn btn-warning" href="`+ vurl + `" target="_blank">打开</a>
	            </div>
	            <input type="text" class="form-control" value="`+ vurl + `" onfocus="this.select()" />
            </div>
        `)
    } else {
        Alert(JSON.stringify(res, null, 4));
    }
});

// 当有文件添加进来时执行，负责view的创建
function addFile(file) {
    var $li = $('<li id="' + file.id + '">' +
        '<p class="title">' + file.name + '</p>' +
        '<p class="imgWrap"></p>' +
        '<p class="progress"><span></span></p>' +
        '</li>'),

        $btns = $('<div class="file-panel">' +
            '<span class="cancel">删除</span>' +
            '<span class="rotateRight">向右旋转</span>' +
            '<span class="rotateLeft">向左旋转</span></div>').appendTo($li),
        $prgress = $li.find('p.progress span'),
        $wrap = $li.find('p.imgWrap'),
        $info = $('<p class="error"></p>'),

        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '文件大小超出';
                    break;

                case 'interrupt':
                    text = '上传暂停';
                    break;

                default:
                    text = '上传失败，请重试';
                    break;
            }

            $info.text(text).appendTo($li);
        };

    if (file.getStatus() === 'invalid') {
        showError(file.statusText);
    } else {
        // @todo lazyload
        $wrap.text('预览中');
        uploader.makeThumb(file, function (error, src) {
            var img;

            if (error) {
                $wrap.text('不能预览');
                return;
            }

            img = $('<img src="' + src + '">');
            $wrap.empty().append(img);
        }, thumbnailWidth, thumbnailHeight);

        percentages[file.id] = [file.size, 0];
        file.rotation = 0;
    }

    file.on('statuschange', function (cur, prev) {
        if (prev === 'progress') {
            $prgress.hide().width(0);
        } else if (prev === 'queued') {
            $li.off('mouseenter mouseleave');
            $btns.remove();
        }

        // 成功
        if (cur === 'error' || cur === 'invalid') {
            showError(file.statusText);
            percentages[file.id][1] = 1;
        } else if (cur === 'interrupt') {
            showError('interrupt');
        } else if (cur === 'queued') {
            $info.remove();
            $prgress.css('display', 'block');
            percentages[file.id][1] = 0;
        } else if (cur === 'progress') {
            $info.remove();
            $prgress.css('display', 'block');
        } else if (cur === 'complete') {
            $prgress.hide().width(0);
            $li.append('<span class="success"></span>');
        }

        $li.removeClass('state-' + prev).addClass('state-' + cur);
    });

    $li.on('mouseenter', function () {
        $btns.stop().animate({ height: 30 });
    });

    $li.on('mouseleave', function () {
        $btns.stop().animate({ height: 0 });
    });

    $btns.on('click', 'span', function () {
        var index = $(this).index(),
            deg;

        switch (index) {
            case 0:
                uploader.removeFile(file);
                return;

            case 1:
                file.rotation += 90;
                break;

            case 2:
                file.rotation -= 90;
                break;
        }

        deg = 'rotate(' + file.rotation + 'deg)';
        $wrap.css({
            '-webkit-transform': deg,
            '-mos-transform': deg,
            '-o-transform': deg,
            'transform': deg
        });
    });

    $li.appendTo($queue);
}

// 负责view的销毁
function removeFile(file) {
    var $li = $('#' + file.id);

    delete percentages[file.id];
    updateTotalProgress();
    $li.off().find('.file-panel').off().end().remove();
}

function updateTotalProgress() {
    var loaded = 0,
        total = 0,
        spans = $progress.children(),
        percent;

    $.each(percentages, function (k, v) {
        total += v[0];
        loaded += v[0] * v[1];
    });

    percent = total ? loaded / total : 0;


    spans.eq(0).text(Math.round(percent * 100) + '%');
    spans.eq(1).css('width', Math.round(percent * 100) + '%');
    updateStatus();
}

function updateStatus() {
    var text = '', stats;

    if (state === 'ready') {
        text = '选中' + fileCount + '个文件，共' +
            WebUploader.formatSize(fileSize) + '。';
    } else if (state === 'confirm') {
        stats = uploader.getStats();
        if (stats.uploadFailNum) {
            text = '已成功上传' + stats.successNum + '个文件，' +
                + '个文件上传失败，<a class="retry" href="javascript:void(0);">重新上传</a>失败文件或<a class="ignore" href="javascript:void(0);">忽略</a>'
        }

    } else {
        stats = uploader.getStats();
        text = '共' + fileCount + '个文件（' +
            WebUploader.formatSize(fileSize) +
            '），已上传' + stats.successNum + '个文件';

        if (stats.uploadFailNum) {
            text += '，失败' + stats.uploadFailNum + '个文件';
        }
    }

    $info.html(text);
}

function setState(val) {
    var file, stats;

    if (val === state) {
        return;
    }

    $upload.removeClass('state-' + state);
    $upload.addClass('state-' + val);
    state = val;

    switch (state) {
        case 'pedding':
            $placeHolder.removeClass('element-invisible');
            $queue.hide();
            $statusBar.addClass('element-invisible');
            uploader.refresh();
            break;

        case 'ready':
            $placeHolder.addClass('element-invisible');
            $('#filePicker2').removeClass('element-invisible');
            $queue.show();
            $statusBar.removeClass('element-invisible');
            uploader.refresh();
            break;

        case 'uploading':
            $('#filePicker2').addClass('element-invisible');
            $progress.show();
            $upload.text('暂停上传');
            break;

        case 'paused':
            $progress.show();
            $upload.text('继续上传');
            break;

        case 'confirm':
            $progress.hide();
            $('#filePicker2').removeClass('element-invisible');
            $upload.text('开始上传');

            stats = uploader.getStats();
            if (stats.successNum && !stats.uploadFailNum) {
                setState('finish');
                return;
            }
            break;
        case 'finish':
            stats = uploader.getStats();
            if (stats.successNum) {
                //Alert( '上传成功' );
            } else {
                // 没有成功的图片，重设
                state = 'done';
                location.reload();
            }
            break;
    }

    updateStatus();
}

uploader.onUploadProgress = function (file, percentage) {
    var $li = $('#' + file.id),
        $percent = $li.find('.progress span');

    $percent.css('width', percentage * 100 + '%');
    percentages[file.id][1] = percentage;
    updateTotalProgress();
};

//文件添加队列
uploader.onFileQueued = function (file) {
    fileCount++;
    fileSize += file.size;

    if (fileCount === 1) {
        $placeHolder.addClass('element-invisible');
        $statusBar.show();
    }

    addFile(file);
    setState('ready');
    updateTotalProgress();

    //获取Token
    getToken();
};

//文件移除队列
uploader.onFileDequeued = function (file) {
    fileCount--;
    fileSize -= file.size;

    if (!fileCount) {
        setState('pedding');
    }

    removeFile(file);
    updateTotalProgress();
};

uploader.on('all', function (type) {
    switch (type) {
        case 'uploadFinished':
            setState('confirm');
            break;

        case 'startUpload':
            setState('uploading');
            break;

        case 'stopUpload':
            setState('paused');
            break;
    }
});

uploader.onError = function (code) {
    Alert('Eroor: ' + code);
};

$upload.on('click', function () {
    if ($(this).hasClass('disabled')) {
        return false;
    }

    if (state === 'ready') {
        uploader.upload();
    } else if (state === 'paused') {
        uploader.upload();
    } else if (state === 'uploading') {
        uploader.stop();
    }
});

//重试
$info.on('click', '.retry', function () {
    uploader.retry();
});

//忽略
$info.on('click', '.ignore', function () {

});

$upload.addClass('state-' + state);
updateTotalProgress();

//记录 上传的源
var lsup = ss.lsStr("uploadProvider");
$('#seup').change(function () {
    ss.ls["uploadProvider"] = this.value;
    ss.lsSave();
}).find('option').each(function () {
    if (this.value == lsup) {
        $('#seup').val(lsup);
        return false;
    }
})

//获取 token
function getToken() {
    if (!uploader.token_imgbbcom) {
        $.get('https://cors.eu.org/imgbb.com', null, function (html) {
            html.replace(/auth_token=".*";/, function (at) {
                uploader.token_imgbbcom = at.split('"')[1];
            })
        })
    }
}