function callback(message:any) {
    parent._onMessageReceived(message);
}

function Connection(options:any) {
    let _url = options.url;
    let _connectionId = options.connectionId;
    let count = 0;
    const _onMessageReceived = function (data: any) {
        count++;
        var p = document.createElement("pre");
        p.innerHTML = JSON.stringify(data);
        document.body.appendChild(p);
        callback(data);
    };

    let _send = function () {
        const xhr = new XMLHttpRequest();
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

function connectLive(url: string, connectionId: string) {
    var connection: Connection = new Connection({
        url,
        connectionId
    });

    connection.start();
}