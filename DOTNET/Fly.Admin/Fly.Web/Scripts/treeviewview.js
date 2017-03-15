// treegird 插件
(function () {
    $.extend($.fn.treegrid.methods, {
        createDetailView: function (jq) {
            function create(rows) {
                for (var i = 0; i < rows.length; i++) {
                    if (jq.treegrid('getChildren', $(rows[i]).attr('node-id')).length == 0) {
                        $("<tr  data-nodeid=\"" + $(rows[i]).attr('node-id') + "\" class=\"datagrid-detail-row\" style=\"display:none;\" ><td colspan=" + $(jq).treegrid('getColumnFields').length + " class=\"datagrid-detail\" style=\"border:none\"><div></div>  </td> </tr>").insertAfter(rows[i]);
                    }
                }
            }
            var fview = jq.parent().find('div.datagrid-view1');
            var dview = jq.parent().find('div.datagrid-view2');
            var rows = dview.find(".datagrid-row");
            create(rows);
            rows = fview.find(".datagrid-row");
            create(rows);
        },
        expandDetail: function (jq, row) {
            jq.parent().find('div.datagrid-view2,div.datagrid-view1').find("[node-id=" + row[jq.treegrid('options').idField] + "].datagrid-row").parent().find('>[data-nodeid=' + row.id + "].datagrid-detail-row").fadeIn(200);
        },
        collapseDetail: function (jq, row) {
            jq.parent().find('div.datagrid-view2,div.datagrid-view1').find("[node-id=" + row[jq.treegrid('options').idField] + "].datagrid-row").parent().find('>[data-nodeid=' + row.id + "].datagrid-detail-row").fadeOut(200);
        },
        collapseDetailAll: function (jq) {
            jq.parent().find('div.datagrid-view2,div.datagrid-view1').find("[data-nodeid].datagrid-detail-row").fadeOut(200);
        },
        fixDetailRowHeight: function (jq, row) {
            jq.parent().find('div.datagrid-view1').find("[node-id=" + row[jq.treegrid('options').idField] + "].datagrid-row").parent().find('>[data-nodeid=' + row.id + "].datagrid-detail-row").height(jq.parent().find('div.datagrid-view2').find("[node-id=" + row[jq.treegrid('options').idField] + "].datagrid-row").parent().find('>[data-nodeid=' + row.id + "].datagrid-detail-row").height());
        },
        getRowDetail: function (jq, row) {
            return jq.parent().find('div.datagrid-view2').find("[node-id=" + row[jq.treegrid('options').idField] + "].datagrid-row").parent().find('>[data-nodeid=' + row.id + "].datagrid-detail-row .datagrid-detail >div");
        }
    })
})(jQuery)