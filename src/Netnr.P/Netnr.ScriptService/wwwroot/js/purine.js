var purine = {
    rowStyle: function (row) {
        var classes = [
            'text-success',
            'text-warning',
            'text-danger'
        ], ci = 0;
        if (row.number > 25) {
            ci = 1;
        }
        if (row.number > 150) {
            ci = 2;
        }
        return {
            classes: classes[ci]
        }
    },
    resize: function () {
        $('#table').bootstrapTable('resetView', {
            height: $(window).height() - $('#PGrid').offset().top - 15
        });
    },
    load: function () {
        $('#table').bootstrapTable({
            classes: 'table table-sm table-bordered',
            data: JSON.parse($('#source').html())
        });

        $(window).resize(purine.resize);

        purine.resize();
    }
}

purine.load();