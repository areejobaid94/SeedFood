import { Injectable, EventEmitter } from "@angular/core";

@Injectable({
    providedIn: "root",
})
export class NetworkStatusService {
    private onlineStatus: boolean = navigator.onLine;
    public onlineStatusChanged: EventEmitter<boolean> =
        new EventEmitter<boolean>();

    constructor() {
        window.addEventListener("online", this.updateOnlineStatus.bind(this));
        window.addEventListener("offline", this.updateOnlineStatus.bind(this));
    }

    private updateOnlineStatus() {
        this.onlineStatus = navigator.onLine;
        this.onlineStatusChanged.emit(this.onlineStatus);
    }

    isOnline(): boolean {
        return this.onlineStatus;
    }
}
