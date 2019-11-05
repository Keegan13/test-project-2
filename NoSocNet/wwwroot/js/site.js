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

            let str = chatRoomStr + userStr || "Nothing found";

            searchResult.html(`<div class="autocomplete scrollable" style="position:absolute;width:100%;">${str}</div>`).show();
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

        $(document).on("mouseup", function (e) {
            if (!element.is(e.target) && element.has(e.target).length === 0 && !searchResult.is(e.target) && searchResult.has(e.target).length === 0) {
                reset();
            }
        });

        element.on("search", function () {
            console.log("search event is triggered");
        })

    };

    const getRoomName = ({ id, roomName, currentUserId, participants, isPrivate }) => {
        if (roomName && roomName !== "") {
            return roomName;
        }

        const unknownChat = `Unknown chat ${id}`;

        if (participants && participants.length > 0) {

            const others = participants.filter((x) => x.id !== currentUserId);

            if (isPrivate) {
                return others.length > 0 ? others[0].userName : unknownChat;
            }
            else {
                return others.length > 0 ? others.reduce((name, user) => {
                    name += `${user.userName},`;
                    return name;
                }, `# (${participants.length}) `) : unknownChat;
            }
        }

        return unknownChat;
    };

    const renderChatRoomTab = ({ id, roomName, participants, isPrivate, icon = "fa-user", active = false, updated = false, currentUserId }) => {
        const _roomName = getRoomName({ id, roomName, participants, isPrivate, currentUserId })

        return `<a class="nav-item nav-link d-block w-100 chat-tab-link" id="chat${id}-tab" data-toggle="tab" href="#chat${id}" role="tab" aria-controls="chat${id}" data-id="${id}" aria-selected="false"> <i class="fa fa-user"></i> ${_roomName}</a> `;

        //return `<div class="nav-item nav-link w-100 chat-tab-link ${(active ? "active" : "")} ${(updated ? "updated" : "")}"><i class="fa ${icon}"></i> ${roomName}</a > `;
    };

    const renderChatRoomMessages = ({ id, roomName, messages, currentUserId }) => {
        const messagesHtml = messages.reduce((html, message) => {
            html += renderMessage({ ...message, currentUserId });
            return html;
        }, "");
        return `<div class="tab-pane chat-messages scrollable fade" id="chat${id}" role="tabpanel" aria-labelledby="chat${id}-tab" data-id="${id}" >${messagesHtml}</div >`;
    };

    const renderMessage = ({ id, text, sendDate, senderId, senderUserName, currentUserId }) => {
        const _sendDate = moment(new Date(sendDate)).format("YYYY-MM-DD hh:mm");
        const senderCls = currentUserId === senderId ? 'outcomming' : 'incomming';

        return `<div class="alert alert-primary ${senderCls} chat-message" role="alert" data-id="${id}"> <p>${text}</p> <hr><p class="message-info"><span class="message-date">${_sendDate}</span> ${senderUserName}</p></div>`;
    };

    const getFullHeight = (element) => {
        let scrollHeight = 0;
        $(element).children().each(function () {
            scrollHeight = scrollHeight + $(this).outerHeight(true);
        });
        return scrollHeight;
    }



    $.fn.chat = function (config) {
        const GUID = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}/g;
        const EVENTS = {
            NEW_MESSAGE: 'chat.message',
            NEW_CHAT: 'chat.room',
            NEW_USER: 'chat.user',
            TYPING: 'chat.typing',
            NEW_MESSAGES: 'chat.messages',
            ON_MESSAGE: 'chat.on.message',
            ON_MESSAGE_SENT: 'chat.on.sent'
        };
        const container = $(this);

        let _config = $.extend({
            activeChatRoomId: null,
            userId: null,
            loadMessage: function (id) {
                alert(`No handler for loadMessage, can't load message with id ${id}`);
            },
            loadChat: function (id) {

            },
            loadUser: function (id) {

            },
            messagesSeen: function ({ chatRoomId }) { console.log("mesages seen"); },
            loadMessages: function ({ chatRoomId, tailMessageId }) {
            },
            sendMessage: function (data) {
                console.log("Sending message");
                console.log(data);
            },
            inviteUsers: function (data) {
                console.log("Sending invite users");
                console.log(data);
            },
            tabSelector: "#chat-tabs",
            messageContainerSelector: ".chat-messages",
            inviteFormSelector: "#invite-form",
            sendFormSelector: "#send-form"

        }, config);

        const USER_ID = _config.userId;

        let activeRoomId = _config.activeChatRoomId;


        const selectTabs = () => {
            const tabsContainer = container.find(_config.tabSelector);
            if (!tabsContainer.length) {
                throw new Error(`Tab container was not found using selector "${_config.tabSelector}" in container ${container.toString()}`);
            }

            return tabsContainer;
        };

        const selectMessages = () => {
            const messageContainer = container.find(_config.messageContainerSelector);
            if (!messageContainer.length) {
                throw new Error(`Messages container was not found using selector "${_config.messageContainerSelector}" in container ${container.toString()}`);
            }

            return messageContainer;
        };

        const selectSendForm = () => {
            const sendForm = container.find(_config.sendFormSelector);
            if (!sendForm.length) {
                throw new Error(`Send form was not found using selector "${_config.sendFormSelector}" in container ${container.toString()}`);
            }

            return sendForm;
        };

        const selectInviteForm = () => {
            const inviteForm = container.find(_config.inviteFormSelector);
            if (!inviteForm.length) {
                throw new Error(`Invite form was not found using selector "${_config.inviteFormSelector}" in container ${container.toString()}`);
            }

            return inviteForm;
        };


        const selectActiveChat = () => {
        };

        const loadEarlierMessages = () => {
            const messages = selectMessages();
            const tail = messages.find(`#chat${activeRoomId}`).find('.chat-message').first();

            if (tail && tail.id) {
                loadMessages(activeRoomId, tail.id).then((data) => {
                    if (data && data.length > 0) {
                        const chatMessages = messages.find(`#chat${data[0].chatRoomId}`);
                        if (chatMessages) {
                            const html = data.reduce((html, message) => {
                                html += renderMessage({ ...message, currentUserId, USER_ID });
                                return html;
                            }, "");
                            $(html).prependTo(chatMessages);
                        }
                    }
                });
            }
        };

        const messageEmmiter = (message) => {
            container.trigger(EVENTS.NEW_MESSAGE, { payload: message });
        };

        const newChatEmmiter = (chat) => {
            container.trigger(EVENTS.NEW_CHAT, { payload: chat });
        };

        const newUserEmmiter = (user) => {
            container.trigger(EVENTS.NEW_USER, { payload: user });
        };

        const typingEmmiter = (typing) => {
            container.trigger(EVENTS.TYPING, { payload: typing });
        };


        $(document).on(EVENTS.NEW_MESSAGE, function ($event, { payload }) {
            const message = payload;
            message.chatRoomId = message.chatRoomId.toUpperCase();
            const messages = selectMessages();
            const chatMessages = messages.find(`#chat${message.chatRoomId}`);

            if (chatMessages.length === 1) {
                const html = renderMessage({ ...message, currentUserId: _config.userId })

                $(html).appendTo(chatMessages);
                $(chatMessages).trigger(EVENTS.ON_MESSAGE, { payload: message });
            }

            if (chatMessages.length === 0) {
                _config.loadChat(message.chatRoomId).then((data) => {
                    newChatEmmiter(data);
                })
            }
        });

        $(document).on(EVENTS.ON_MESSAGE, _config.messageContainerSelector, function ($event, { payload }) {
            const message = payload;
            if (activeRoomId !== message.chatRoomId) {
                const chatTab = selectTabs().find(`#chat${message.chatRoomId}-tab`);
                if (chatTab) {
                    chatTab.addClass("updated");
                }
            }
            else {
                const messageTab = selectMessages().find(`#chat${message.chatRoomId}`);
                if (messageTab) {
                    let h = getFullHeight(messageTab);
                    messageTab.scrollTop(h);
                }
            }
        });

        selectSendForm().on('submit', ($event) => {
            $event.preventDefault();
            $event.stopPropagation();
            _config.sendMessage({ text: $(this).find("[name=text]").val(), chatRoomId: activeRoomId })
                .then(data => {
                    $(this).find("[name=text]").val("");
                });
        });

        selectInviteForm().on('submit', ($event) => {
            $event.preventDefault();
            $event.stopPropagation();
            _config.inviteUsers({ chatRoomId: activeRoomId });
        });

        const bindScroll = () => {
            $(_config.messageContainerSelector).on('scroll', ".chat-messages", function (e) {
                console.log('catching scroll event');

                const fullHeight = getFullHeight(this);
                const elementHeight = $(this).outerHeight(true);
                const scrollHeight = $(this).prop('scrollHeight');
                const almostTop = () => {
                    return true;
                };
                const almostBottom = () => {
                    return true;
                };

                if (almostTop()) {
                    loadEarlierMessages();
                }
                if (almostBottom()) {
                    messagesSeen({ chatRoomId: activeRoomId });
                }
                ///load messages here
                //let position = $(e.target).scrollTop();
                //if (position < 200) {

                //}
                //console.log();
            });
        }

        bindScroll();

        $(document).on(EVENTS.NEW_CHAT, function ($event, { payload }) {
            const chat = payload;
            chat.id = chat.id.toUpperCase();
            const tabs = selectTabs();
            const messages = selectMessages();

            if (messages.find(`#chat${chat.id}`).length == 0) {
                const html = renderChatRoomMessages({ ...chat, currentUserId: USER_ID });
                $(html).appendTo(messages);
            }

            if (tabs.find(`#chat${chat.id}-tab`).length == 0) {
                const html = renderChatRoomTab({ ...chat, currentUserId: USER_ID });

                const last = tabs.find(".chat-tab-link").last();
                if (last.length > 0) {
                    $(html).insertAfter(last);
                }
                else {
                    $(html).prependTo(tabs);
                }
            };

            if (chat.isFocus) {
                tabs.find(`#chat${chat.id}-tab`).tab('show');
                activeRoomId = chat.id;
            }

        });
        $(document).on(EVENTS.NEW_USER, function ($event, { payload }) { });

        $(document).on(EVENTS.TYPING, function ($event, { payload }) { });

        $(document).on(EVENTS.ON_MESSAGE_SENT, function ($event, { payload }) {
            const sentForm = selectSendForm();
            sentForm.find("textarea").val("");
        });

        $(document).on('click', 'a[data-toggle="tab"]', function (e) {
            $(e.target).removeClass("updated");
            const id = $(this).data('id');
            activeRoomId = id;
            _config.messagesSeen({ chatRoomId: id });
        });


        return {
            events: {
                triggerNewMessage: messageEmmiter,
                triggerNewChat: newChatEmmiter,
                triggerNewUser: newUserEmmiter,
                triggerNewTyping: typingEmmiter
            }
        }
    }
})(jQuery);

function partialModal(data) {
    //when partial view returned to modal
    $("#shared-modal").html(data).modal("show");
}

function clearModal() {
    console.log("Closing modal");
    $.modal.close();
    $('#shared-modal').html('');
}