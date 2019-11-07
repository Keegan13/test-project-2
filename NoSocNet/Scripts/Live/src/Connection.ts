
export class Connection {
    private url: string;
    private connectionId: string;
    private onMessageCallback: Function;

    constructor(
        url: string,
        connectionId: string,
        onMessageCallback: Function) {
        this.url = url;
        this.connectionId = connectionId;
        this.onMessageCallback = onMessageCallback;
    }

    public start() {
        this.request();
        console.log("Starting live session " + this.connectionId);
    };

    private onMessage(data: any): void {
        if (this.onMessageCallback)
            this.onMessageCallback(data);
    };

    public request(): void {
        const xhr = new XMLHttpRequest();

        const reconnect = (): void => {
            this.request();
        };

        const onResponse = (data: any): void => {
            this.onMessage(data);
        };

        xhr.open('POST', this.url);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.responseType = 'json';
        xhr.onload = function (e) {
            if (this.status == 200) {
                onResponse(this.response);
                reconnect();
            }
        };

        xhr.onerror = function() {
            reconnect();
        }

        xhr.ontimeout = function () {
            reconnect();
        }

        try {
            xhr.send(encodeURI('connectionId=' + this.connectionId));
        }
        catch (error) {
            console.log(error);
            this.request();
        }
    };
}