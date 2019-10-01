// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {
    $.fn.hub = function (config) {

        let _config = $.extend({
            onMessage: function (message) {
                console.log("Received new message from hub ");
                console.log(message);
                console.log("Define onMessageHandler to receive notifications");
            },
            mode: "LongPooling",
            host: null
        }, config);

        if (!config.host) {
            console.log("Host is not defined in configuration, please set 'host' in call to hub({host:'https://myurl'})");
        }

        let _onMessageHandler = function (message) {
            if (!message) {
                console.log("Empty response");
            }
            else {
                _config.onMessage(message);
            }
        };

        window._onMessageReceived = function (message) {
            _onMessageHandler(message);
        };

        let _url = `${config.host}?callback=_responseHandler`;

        if (_config.mode == "ForeverFrame") {
            _url += `&mode=foreverFrame`;
        }

        var frame = $(`<iframe name="background-worker" id="background-worker" src="${_url}"></iframe>`)
            .appendTo(this);
    };
})(jQuery);

(function () {
    $('a[data-toggle="tab"]').on('click', function (e) {
        $(e.target).removeClass("updated");
    });
})();


function partialModal(data) {
    //when partial view returned to modal
    $("#shared-modal").html(data).modal("show");
}

function onAjaxError() {

}
function onComplete(data) {
    console.log("Implement onComplete")
}
function onNewParticipants(data) {
    //when new users join chat

}

function clearModal() {
    console.log("Closing modal");
    $.modal.close();
    $('#shared-modal').html('');
}

function onNewParticipants(content) {
    console.log(content);
}