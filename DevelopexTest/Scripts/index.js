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

//method for string formatting
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

$(function () {
    var statuses = {
        InProgress: 1,
        Finded: 2,
        NotFound: 3,
        Error: 4
    }
    var resultTable = $("#result-table"),
        rowTemplate = '<tr data-url="{url}"><td class="url"><a href="{url}">{url}</a></td><td class="progress"><progress max="100" value="0"></progress></td><td class="status">{status}</td></tr>';

    resultTable.find(".alert").hide();
    initSignalR();

    function initSignalR() {
        var connection = $.hubConnection();
        var searchHubProxy = connection.createHubProxy('searchHub');
        searchHubProxy.on("progressChanged", onProgressChanged);
        searchHubProxy.on("appError", onAppError);

        connection.qs = { "guid": $("#guid").val() }
        connection.logging = true;
        connection.start({ pingInterval: 20000 });
        connection.disconnected(function () {
            setTimeout(function () {
                connection.start({ pingInterval: 20000 });
                console.log("reconnected");
            }, 5000);
        });


    }

    $("#search-form").on("submit", function (e) {

        resultTable.find(".alert").hide();

        resultTable.find("tbody").empty();
    });

    $("#cancel-btn").on("click", function (e) {
        var guid = $("#guid").val();
        $.post("api/cancel", { "": guid }, function(resp) {
            resultTable.find(".alert").show().text("Search Canceled");
        });
    });

    function onAppError(errorMessage) {
       resultTable.find(".alert").show().text(errorMessage);
    }

    function onProgressChanged(urlHtml, status, errorMessage) {
        var url = urlHtml.replace(/&amp;/gi, "&").replace(/&#x3D;/gi, "=");
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
    }

    function addRow(data) {
        resultTable.find("tbody").append(rowTemplate.supplant(data));
        var row = resultTable.find('tr[data-url="{0}"]'.format(data.url)),
            rowProgress = row.find('progress');
        rowProgress.animate({ value: 95 }, 1000);
    }

    function complete(url, status) {
        var row = resultTable.find('tr[data-url="{0}"]'.format(url));
        row.find('progress').stop(false, true).val(100);
        row.find('td.status').text(status == statuses.Finded ? "Finded" : "Not Found");
    };

    function searchError(url, errorMessage) {
        var row = resultTable.find('tr[data-url="{0}"]'.format(url));
        row.find('progress').stop(false, true).val(100).addClass("search-error");
        row.find('td.status').text("Error: {0}".format(errorMessage));
        row.find('td.status').prop("title", errorMessage).text("Error: {0}".format(errorMessage));

    }

});