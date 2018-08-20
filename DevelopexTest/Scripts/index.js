// A simple templating method for replacing placeholders enclosed in curly braces.
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

var entityMap = {
    '&': '&amp;',
    //'<': '&lt;',
    //'>': '&gt;',
    //'"': '&quot;',
    //"'": '&#39;',
    //'/': '&#x2F;',
    //'`': '&#x60;',
    //'=': '&#x3D;'
};

function escapeHtml(string) {
    return String(string).replace(/&amp\//g, function (s) {
        return entityMap[s];
    });
}

$(function () {
    var statuses = {
        InProgress: 1,
        Finded: 2,
        NotFound: 3,
        Error: 4
    }
    var ticker = $.connection.searchHub, // the generated client-side hub proxy
        up = '▲',
        down = '▼',
        stockTable = $('#stockTable'),
        rowTemplate = '<tr data-url="{url}"><td class="url">{url}</td><td><progress max="100" value="0"></progress></td><td class="status">{status}</td></tr>';

    // Start the connection
    var connection = $.hubConnection();
    var searchHubProxy = connection.createHubProxy('searchHub');
    connection.qs = { "guid": $("#guid").val() }
    connection.start();

    $("#search-form").on("submit", function(e) {
        $("#stockTable tbody").empty();
    });

    searchHubProxy.on("addChatMessage", function (urlHtml, status, errorMessage) {
        var url = urlHtml.replace(/&amp\//g, "&");
        if (status == statuses.InProgress) {
            var data = {
                url: url,
                status: "In Progress"
            };

            addRow(data);
        }

        if (status == statuses.Finded || status == statuses.NotFound) {
            complete(url, status);
        }
        if (status == statuses.Error) {
            searchError(url, errorMessage);
        }


    });

    function addRow(data) {
        stockTable.find("tbody").append(rowTemplate.supplant(data));
        var row = stockTable.find('tr[data-url="{0}"]'.format(data.url)),
            rowProgress = row.find('progress');
            rowProgress.animate({ value: 95 }, 1000);
        //rowVal = rowProgress.val();

        //while (rowVal <= 97) {
        //    setInterval(function () {
        //        rowProgress.val(rowVal++);
        //    }, 100);
        //}
    }

    function complete(url, status) {
        var row = stockTable.find('tr[data-url="{0}"]'.format(url));
        row.find('progress').stop(false, true).val(100);
        row.find('td.status').text(status == statuses.Finded ? "Finded" : "Not Found");
    };

    function searchError(url, errorMessage) {
        var row = stockTable.find('tr[data-url="{0}"]'.format(url));
        row.find('progress').stop(false, true).val(100).addClass("search-error");
        row.find('td.status').text("Error: {0}".format(errorMessage));


    }

});