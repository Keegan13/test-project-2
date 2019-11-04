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

    $.fn.appSearch = function ({
        onUser,
        onChat
    }) {
        const RESULT_TYPE = {
            USER: 'user',
            CHAT_ROOM: "chat-room",
            USER_IN_ROOM: "user-in-room",
            MESSAGE: "message"
        };
        const element = $(this);
        var isTimeOut = false;
        var timer = null;
        let userChunk = 1;
        let roomChunk = 1;
        var activeRequest = null;
        $('<div id="searchResult"></div>').insertAfter(element);
        var searchResult = $("#searchResult").css({ "position": "relative" });
        searchResult.hide();

        const reset = () => {
            searchResult.hide();
            searchResult.html("");
            userChunk = 1;
            roomChunk = 1;
            if (timer) {
                clearTimeout(timer);
            }
            isTimeOut = false;
            if (activeRequest) {
                activeRequest.abort();
            }
        };

        const renderUserResultItem = ({ userName, id }) => {
            return `<div class="autocomplete-item" data-text="${userName}" data-id="${id}" data-type="${RESULT_TYPE.USER}"><i class="fa fa-search"></i><span>${userName}<span></div>`;
        };

        const renderChatRoomResultItem = ({ id, roomName, isPrivate, participants }) => {
            const renderParticipant = ({ userName, id }) => {
                return `<div class='user-lists'>${userName}</div>`
            };

            return `<div class="autocomplete-item" data-id="${id}" data-type="${RESULT_TYPE.CHAT_ROOM}"><div><i class="fa ${(isPrivate ? 'fa-user' : 'fa-group')}"></i><span>${roomName}<span></div><div class="participants">${participants.reduce((html, user) => {
                html += renderParticipant(user);
                return html;
            }, "")}</div></div>`
        };

        const itemRenderer = {
            [RESULT_TYPE.USER]: renderUserResultItem,
            [RESULT_TYPE.CHAT_ROOM]: renderChatRoomResultItem
        };


        const prependMoreResults = (items, type) => {
            const moreButton = $(`.load-next-chunk[data-type=${type}]`);
            if (items.length === 0) {
                alert("No more items");
                moreButton.remove();
                return;
            }

            if (moreButton) {
                const render = itemRenderer[type];
                const htmlStr = items.reduce((html, item) => {
                    html += render(item);
                    return html;
                }, "");

                $(htmlStr).insertBefore(moreButton);
            }
        }

        //const appendUserResult = (users) => {
        //    const str = users.reduce((html, user) => {
        //        html += renderUserResultItem(user);
        //        return html;
        //    }, "");

        //    $(html).insertBefore();
        //};

        const loadMoreUsers = () => {
            const keywords = element.val();

            const request = $.ajax({
                type: "GET",
                url: "api/search/users?keywords=" + keywords + "&chunk=" + (userChunk + 1),
                success: function (data) {
                    debugger;
                    userChunk += 1;
                    prependMoreResults(data, RESULT_TYPE.USER);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    activeRequest = null;
                }
            });
        };

        const loadMoreChatRooms = () => {
            const keywords = element.val();
            const request = $.ajax({
                type: "GET",
                url: "api/search/chat-rooms?keywords=" + keywords + "&chunk=" + (roomChunk + 1),
                success: function (data) {
                    debugger;
                    roomChunk += 1;
                    prependMoreResults(data, RESULT_TYPE.CHAT_ROOM);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    activeRequest = null;
                }
            });
        };


        $(document).on('click', '.autocomplete-item', function ($event) {
            const text = $(this).data("text");
            const type = $(this).data("type");
            const id = $(this).data("id");
            switch (type) {
                case RESULT_TYPE.USER: onUser(id); break;
                case RESULT_TYPE.CHAT_ROOM: onChat(id); break;
            }
            reset();
        });

        $(document).on('click', ".load-next-chunk", function ($event) {
            const type = $(this).data("type");
            switch (type) {
                case RESULT_TYPE.USER: loadMoreUsers(); break;
                case RESULT_TYPE.CHAT_ROOM: loadMoreChatRooms(); break;
            };
        });

        const renderAutocomplete = (data) => {
            let userStr = data.users.reduce((html, user) => {
                html += renderUserResultItem(user);
                return html;
            }, "");

            if (userStr && userStr !== "") {
                userStr += `<a class="btn btn-default load-next-chunk" data-type="${RESULT_TYPE.USER}">more...</a>`;
            }

            let chatRoomStr = data.chatRooms.reduce((html, room) => {
                html += renderChatRoomResultItem(room);
                return html;
            }, "");

            if (chatRoomStr && chatRoomStr !== "") {
                chatRoomStr += `<a class="btn btn-default load-next-chunk" data-type="${RESULT_TYPE.CHAT_ROOM}">more...</a>`
            }

            searchResult.html(`<div class="autocomplete scrollable" style="position:absolute;width:100%;">${chatRoomStr}${userStr}</div>`).show();
        };

        element.on("input", function ($event) {

            if (isTimeOut) {
                clearTimeout(timer);
                isTimeOut = false;
            }
            isTimeOut = true;
            timer = setTimeout(() => {
                const keywords = $event.target.value || "";
                searchResult.hide();

                if (activeRequest) {
                    activeRequest.abort();
                }

                roomChunk = 1;
                userChunk = 1;

                activeRequest = $.ajax({
                    type: "GET",
                    url: "api/search?keywords=" + keywords,
                    success: function (data) {
                        renderAutocomplete(data);
                        activeRequest = null;
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        activeRequest = null;
                    }
                });

                isTimeOut = false;
            }, 500);

        });

        //element.on("blur", function ($event) {
        //    searchResult.hide();
        //})

        element.on("search", function () {
            console.log("search event is triggered");
        })

    };
})(jQuery);

(function () {

    $(document).on('click', 'a[data-toggle="tab"]', function (e) {

        $(e.target).removeClass("updated");
        let tabId = $(e.target).attr("aria-controls");
        var regex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}/g;
        let roomId = regex.exec(tabId);

        //notify that current user have read all messages in current chat room
        $.ajax({
            url: '/chat/seen',
            type: 'PUT',
            data: `roomId = ${roomId} `
        });

        setTimeout(() => {
            $(`#${tabId} `).find(".chat-messages").scrollTop(10000000);
        }, 500);

        // what you want to happen when mouseover and mouseout 
        // occurs on elements that match '.dosomething'
    });

    $(document).on('chat.on.message', ".chat-messages", function (e) {
        let scrollHeight = 0;
        $(this).children().each(function () {
            scrollHeight = scrollHeight + $(this).outerHeight(true);
        });
        console.log('scrolling with height: ' + scrollHeight);
        $(this).scrollTop(scrollHeight);

        if (!e.message) return;
        let chatTab = $(`a#chat${e.message.chatRoomId} -tab`);
        if (chatTab && !chatTab.hasClass("active")) {
            chatTab.addClass("updated");
        }
    });

    //$('.chat-messages').on('scroll', function (e) {
    //    console.log('scroll event');
    //});
    $(document).on('scroll', '.chat-messages', function (e) {
        console.log('catching scroll event');
        //let position = $(e.target).scrollTop();
        //if (position < 200) {

        //}
        //console.log();
    });

    $("#search-chats-form").on('change', function (e) {
        if (e.target.value == "") {
            $(".chat-tab-link.restore").removeClass("restore").show();
        }
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
    $(this).remove();
}

function onNewParticipants(content) {
    console.log(content);
}

function remove(element) {
    $(element).remove();
}

function onLoadMoreMessages(data) {

    if (data && data.length > 0) {

        // inserst messages
        let content = data.reduce((agg, next) => agg += renderMessage(next), '');
        $(content).insertAfter(this);

        //change the tail id
        let tailId = data[0].id;
        let url = $(this).attr('data-ajax-url');
        console.log(url);
        $(this).attr('data-ajax-url', url.replace(/tail=([0-9]+)/gi, () => ('tail=' + tailId.toString())));
    }
    if (data.length == 0) {
        $(this).remove();
        alert('no more messages');
    }


}

function onLoadUsers(data) {
    if (data && data.length > 0) {
        //insert tab link
        var html = data.map(el => {
            return `<a class="nav-item nav-link w-100 chat-tab-user-link" href = "/" data-ajax="true" method="GET" data-ajax-success="onNewChatSuccess" data-ajax-url="/Chat/Private/${el.id}"> <i class="fa fa-plus"></i>${el.userName}</a>`;
        }).reduce((curr, next) => curr += next);

        $(html).insertBefore("#more-users-btn");



        let url = $('#more-users-btn').attr('data-ajax-url');
        let tailId = data[data.length - 1].id;
        $(this).attr('data-ajax-url', url.replace(/tailId=(.*)/gi, () => ('tailId=' + tailId.toString())));
    }

    if (data.length == 0) {
        $(this).remove();
        alert('no more users');
    }

}

function container() {
    return $('#nav-tabContent');
}

function tabLink() {
    return $('#nav-tab');
}

function getCurrentUser() {
    return $.currentUserId;
}

function appendChatLinkTo(element, chat) {
    const { id, roomName } = chat;
    if (!roomName) {
        roomName = chat.participants.filter(x => x.id != $.currentUserId).reduce((agg, next) => agg += ", " + next);
    }

    $(`<a class="nav-item nav-link d-block w-100 chat-tab-link" id="chat${id}-tab" data-toggle="tab" href="#chat${id}" role="tab" aria-controls="chat${id}" aria-selected="false"> <i class="fa fa-user"></i> ${roomName}</a> `).prependTo(element);
}

function appendChatTo(element, chat) {
    $(`<div class="tab-pane fade" id = "chat${chat.id}" role = "tabpanel" aria-labelledby="chat${chat.id}-tab" > <div class="mb-3 mt-2"><h4 class="chat-heading d-inline-block align-middle">${chat.roomName}</h4><a data-ajax="true" class="btn btn-primary btn-invite d-inline-block align-middle float-right" data-ajax-success="partialModal" data-ajax-method="GET" data-ajax-url="/Chat/Invite/${chat.id}"><i class="fa fa-plus-circle"></i></a></div> <div class="chat-messages"></div> <form method="post" data-ajax="true" data-ajax-method="post" data-ajax-url="/Chat/Sent" data-ajax-success="onMessageSent" class="mt-2"><div class="input-group"><input type="hidden" name="roomid" value="${chat.id}" /><textarea class="form-control" name="text" value="" placeholder="Type a message" required></textarea><button type="submit" class="btn"><i class="fa fa-telegram" style="font-size:36px;color:cornflowerblue"></i></button></div></form></div> `)
        .appendTo(element);
}

function appendChat(chat) {
    const tabContainer = container();
    const tabLinkContainer = tabLink();

    let userIdregex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}/g;

    //remove any link to user
    //tabLinkContainer.find("a[data-ajax-url]").filter((idx, item) => {
    //    let match = userIdregex.exec($(item).attr("data-ajax-url"));
    //    let userId = match && match.length > 0 ? match[0] : "";
    //    return chat.participants.some(p => p.id == userId);
    //}).remove();

    if ($(`#chat${chat.id} `).length == 0) {
        appendChatTo(tabContainer, chat);
    }

    if ($(`#chat${chat.id} -tab`).length == 0) {
        appendChatLinkTo(tabLinkContainer, chat);
    };

    if (chat.isFocus) {
        $(`#chat${chat.id} -tab`).tab('show');
    }
}

function renderMessage(msg) {
    let senderCls = getCurrentUser() === msg.senderId ? 'outcomming' : 'incomming';
    return `<div class="alert alert-primary ${senderCls}" role = "alert" > <p>${msg.text}</p> <hr><p class="message-info"><span class="message-date">${msg.sendDate}</span> ${msg.senderUserName}</p></div>`;
}
function appenMessageTo(chatTab, msg) {
    msg.sendDate = moment(new Date(msg.sendDate)).format("YYYY-MM-DD hh:mm");

    $(renderMessage(msg)).appendTo(chatTab)
    $(chatTab).trigger({
        type: 'chat.on.message',
        message: msg
    });
}




function appenMessage(msg) {
    var chatTabsContainer = container();
    var chatTab = chatTabsContainer.find("#chat" + msg.chatRoomId + " .chat-messages");

    if (chatTab.length > 0) {
        appenMessageTo(chatTab, msg);
        return;
    }

    $.ajax({
        url: '/Chat/Room',
        type: 'post',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            roomId: msg.chatRoomId
        }
    }).then(
        function (data) {
            appendChat(data);
            appenMessage(msg);
        },
        function (jqXHR, textStatus, errorThrown) {
            //bad status code
            return;
        }
    ).catch(function (error) {
        // ...
    });
};


function onLoadMessages(data) {
    let container = $();
    $("<p>Messages</p>").prepend(container)
}

var roomLinkTemplate = function (data) {
    const { id, roomName } = data
    return `<a class="nav-item nav-link w-100 chat-tab-link" id = "chat${id}-tab" data-toggle="tab" href="#chat${id}" role="tab" aria-controls="chat${id}" aria-selected="false" > <i class="fa fa-user"></i> ${roomName}</a > `;
}

var roomTemplate = function (data) {
    const { id, roomName } = data
    return `<div class="tab-pane fade" id = "chat${id}" role = "tabpanel" aria-labelledby="chat${id}-tab" > <div class="mb-3 mt-2"><h4 class="chat-heading d-inline-block align-middle">${roomName}</h4><a data-ajax="true" class="btn btn-primary btn-invite d-inline-block align-middle float-right" data-ajax-success="partialModal" data-ajax-method="GET" data-ajax-url="/Chat/Invite/${id}"><i class="fa fa-plus-circle"></i></a></div> <div class="chat-messages"></div> <form method="post" data-ajax="true" data-ajax-method="post" data-ajax-url="/Chat/Sent" data-ajax-success="onMessageSent" class="mt-2"><div class="input-group"><input type="hidden" name="roomid" value="${id}" /><textarea class="form-control" name="text" value="" placeholder="Type a message" required></textarea><button type="submit" class="btn"><i class="fa fa-telegram" style="font-size:36px;color:cornflowerblue"></i></button></div></form></div > `;
}

function renderTabLinksToString(data) {
    if (data && data.length > 0) {
        return data.map(el => roomLinkTemplate(el)).reduce((agg, next) => agg += next);
    }

    return '';
}

function renderTabToString(data) {
    if (data && data.length > 0) {
        return data.map(el => roomTemplate(el)).reduce((agg, next) => agg += next);
    }

    return '';
}

function onLoadChats(data) {
    let form = $('#more-chats-form');

    if (data.length == 0) {
        form.find("button").remove();
        return;
    }

    let last = data[data.length - 1];

    form.find("input[name='tailId']").val(last.id);

    var linksHtml = renderTabLinksToString(data);
    $(linksHtml).insertBefore(form);


    var tabsHtml = renderTabToString(data);
    $(tabsHtml).appendTo(container());

}

function onChatSearch(data) {
    $(".chat-tab-link").addClass("restore").hide();
    if (data && data.length > 0) {
        data.forEach(function (item) {
            appendChat(item);
        });
    }
}