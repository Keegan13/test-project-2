﻿function callback(message) {
    parent._onMessageReceived(message);
}

function Connection(options) {
    let _url = options.url;
    let _connectionId = options.connectionId;
    let count = 0;
    const _onMessageReceived = function (data) {
        count++;
        var p = document.createElement("pre");
        p.innerHTML = JSON.stringify(data);
        document.body.appendChild(p);
        callback(data);
    };

    let _send = function () {
        xhr = new XMLHttpRequest();
        xhr.open('POST', _url);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.responseType = 'json';
        xhr.onload = function (e) {
            if (this.status == 200) {
                _onMessageReceived(this.response);
                _send();
            }
        };

        xhr.onerror = function () {
            _send();
        }

        xhr.ontimeout = function () {
            _send();
        }
        try {
            xhr.send(encodeURI('connectionId=' + _connectionId));
        }
        catch (error) {
            console.log(error);
            _send();
        }
    }

    let _start = function () {
        _send();
        console.log("Starting live session " + _connectionId);
    }

    return {
        start: _start
    };
}

function connectLive(url, connectionId) {
    var connection = new Connection({
        url,
        connectionId
    });

    connection.start();
}