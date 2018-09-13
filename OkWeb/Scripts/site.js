jQuery(document).ready(function () {
    jQuery("input[type=button]").click(function () {
        var sort = jQuery("input[type=radio]:checked").val();
        var source = jQuery("select option:selected").val();
        if (source < 0) {
            source = 1;
        }

        jQuery("#result").empty();
        setData(sort, source, 1);

        jQuery("#pagination").empty();
        setPagination(sort, source, 1);

        return false;
    });

    jQuery("#pagination").delegate("a",
        "click",
        function(e) {
            var sort = jQuery("input[type=radio]:checked").val();
            var source = jQuery("select option:selected").val();
            if (source < 0) {
                source = 1;
            }

            var page = jQuery(e.currentTarget).text();
            if (parseInt(page) < 1) {
                page = 1;
            }

            jQuery("#result").empty();
            jQuery("#pagination").empty();
            setData(sort, source, page);
            setPagination(sort, source, page);
        });
});

function setData(sort, source, page) {
    jQuery.ajax({
        type: "post",
        url: "/News/AjaxJson",
        data: "source=" + source + "&sort=" + sort + "&page=" + page
    }).done(function (data) {
        var result = jQuery.parseJSON(data);
        for (var i = 0; i < result.length; i++) {
            var source = result[i]["Source"];
            var tr = jQuery("<tr></tr>").append(
                jQuery("<td></td>").text(source.Name),
                jQuery("<td></td>").text(result[i].Title),
                jQuery("<td style=\"word-break:break-all;\"></td>").text(result[i].Description),
                jQuery("<td>"+ result[i].Date + "</td>"));
            jQuery("#result").append(tr);
        }
    }).fail(function (data) {
        
    });
}

function setPagination(sort, source, page) {
    jQuery.ajax({
        type: "post",
        url: "/News/PaginationJson",
        data: "source=" + source + "&sort=" + sort + "&page=" + page
    }).done(function (data) {
        jQuery("#pagination").css("display", "");
        for (var i = 1; i <= data.TotalPages; i++) {
            if (i === data.CurrentPage) {
                jQuery("#pagination").append(jQuery("<li class=\"page-item active\"><a class=\"page-link\">" + i + "</a></li>"));
            } else {
                jQuery("#pagination").append(jQuery("<li class=\"page-item\"><a class=\"page-link\">" + i + "</a></li>"));
            }
        }
        }).fail(function(data) {

    });
}