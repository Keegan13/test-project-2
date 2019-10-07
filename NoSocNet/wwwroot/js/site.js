// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {
    $.fn.hub = function (config) {
        const NotificationTypes = {
            Message: 0,
            ChatJoin: 1,
            NewChat: 2,
            Typing: 3
        };
        let _config = $.extend({
            onMessage: function (message) {
                console.log("Received new message noitification from hub");
                console.log(message);
                console.log("Provide callback handler for 'onMessage' notifications");
            },
            onNewChat: function (chat) {
                console.log("Received new chat notification from hub ");
                console.log(chat);
                console.log("Provide callback handler for 'onNewChat' notifications");
            },
            onChatJoin: function (chatJoin) {
                console.log("Received new user joined from hub ");
                console.log(chatJoin);
                console.log("Provide callback handler for 'onChatJoin' notifications");
            },
            onTyping: function (typing) {
                console.log("Received typing notification from hub ");
                console.log(typing);
                console.log("Provide callback handler for 'onTyping' notifications");
            },
            mode: "LongPooling",
            host: null
        }, config);

        if (!config.host) {
            console.log("Host is not defined in configuration, please set 'host' in call to hub({host:'https://myurl'})");
        }

        let _onMessageHandler = function (data) {
            if (!data) {
                console.log("Empty response");
            }
            else {
                switch (data.type) {
                    case NotificationTypes.Message: _config.onMessage(data.notification); break;
                    case NotificationTypes.NewChat: _config.onNewChat(data.notification); break;
                    case NotificationTypes.ChatJoin: _config.onChatJoin(data.notification); break;
                    case NotificationTypes.Typing: _config.NotificationTypes(data.notification); break;
                    default: console.log("Unknown message type" + data.type); break;
                }
            }
        };

        window._onMessageReceived = function (notification) {
            _onMessageHandler(notification);
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
    $(document).on('click', 'a[data-toggle="tab"]', function (e) {

        $(e.target).removeClass("updated");
        let tabId = $(e.target).attr("aria-controls");
        var regex = /[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}/g;
        let roomId = regex.exec(tabId);

        //notify that current user have read all messages in current chat room
        $.ajax({
            url: '/chat/seen',
            type: 'PUT',
            data: `roomId=${roomId}`
        });

        setTimeout(() => {
            $(`#${tabId}`).find(".chat-messages").scrollTop(10000);

        }, 500);

        // what you want to happen when mouseover and mouseout 
        // occurs on elements that match '.dosomething'
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
function onMessageSent(data) {
    let roomid = data.chatRoomId;
    $(`#chat${roomid} form textarea`).val("");
}

function clearModal() {
    console.log("Closing modal");
    $.modal.close();
    $('#shared-modal').html('');
}

function onNewChatSuccess() {
    //$(this).remove();
}

function onNewParticipants(content) {
    console.log(content);
}