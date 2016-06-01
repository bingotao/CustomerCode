; (function (window, undefined) {
    utilities = window.utilities || {};

    utilities.form = {};
    utilities.form.SetJsonToPage = function (filter, entity) {
        var $rootDom = $(filter);
        for (var name in entity) {
            var $item = $rootDom.find('[name=' + name + ']');
            if ($item.length) {
                var value = entity[name];
                $item.text(value = value ? value : ($item.hasClass('number') ? 0 : ''));
            }
        }
    };

    utilities.url = {};
    utilities.url.GetQueryString = function (name) {
        var url = window.location.search.toUpperCase();
        var n = name.toUpperCase();
        var reg = new RegExp("(^|&)" + n + "=([^&]*)(&|$)", "i");
        var r = url.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return null;
    };

    utilities.url.GetRequest = function () {
        var url = location.search.toLocaleUpperCase();
        var theRequest = new Object();
        if (url.indexOf("?") != -1) {
            var str = url.substr(1);
            strs = str.split("&");
            for (var i = 0; i < strs.length; i++) {
                theRequest[strs[i].split("=")[0].toUpperCase()] = unescape(strs[i].split("=")[1]);
            }
        }
        return theRequest;
    };

    utilities.format = {};
    utilities.format.NumberFormat = function (value, setting) {
        var val = 0;
        if (value) {
            val = value;
            if (setting && setting.multiple != undefined) {
                val = val * setting.multiple;
            }
            if (setting && setting.fixed != undefined) {
                val = val.toFixed(setting.fixed) - 0;
            }
        }
        return val;
    };

    utilities.excelHelper = {};
    utilities.excelHelper.GetHtmlTable = function (filter) {
        var headerFilter = " .datagrid-view .datagrid-header table";
        var bodyFilter = " .datagrid-view .datagrid-body table";
        var regexReplace = '<tbody>|</tbody>|<div.*?>|</div>|<span>|</span>|&nbsp;|<span.*?>|field=".*?"|class=".*?"|id=".*?"|datagrid-row-index=".*?"|style="height:.*?"|<td[^/]*none.*?/td>';
        var regex = new RegExp(regexReplace, "g");

        function headerFormatter() {
            var resultHtml = "";
            var headers = $(tableContainerFilter + headerFilter);
            var header1 = headers.first().html().replace(regex, "").replace(/td/g, "th");
            var header2 = headers.last().html().replace(regex, "").replace(/td/g, "th");
            header1 = $(header1);
            header2 = $(header2);
            for (var i = 0; i < header2.length; i++) {
                resultHtml += "<tr>" + header1[i].innerHTML + header2[i].innerHTML + "</tr>";
            }
            return resultHtml;
        }

        function bodyFormatter() {
            var resultHtml = "";
            var headers = $(tableContainerFilter + bodyFilter);
            var header1 = headers.first().html().replace(regex, "");
            var header2 = headers.last().html().replace(regex, "");
            header1 = $(header1);
            header2 = $(header2);
            for (var i = 0; i < header2.length; i++) {
                resultHtml += "<tr>" + header1[i].innerHTML + header2[i].innerHTML + "</tr>";
            }
            return resultHtml;
        }
        var resultHtml = "<table>" + headerFormatter() + bodyFormatter() + "</table>";
        return resultHtml;
    }
})(window);