import { Connection } from "./Connection";

declare global {
    interface Window {
        _onMessageReceived: Function;
        connectLive: Function;
    }
}

function callback(message: any):void {
    parent._onMessageReceived(message);
}

window.connectLive = function (url: string, connectionId: string): void {
    const connection: Connection = new Connection(url, connectionId, callback);
    connection.start();
}