import { Component, OnInit } from '@angular/core';
import { NetworkStatusService } from '../services/network-status.service';

@Component({
  selector: 'app-offline-notification',
  templateUrl: './offline-notification.component.html',
  styleUrls: ['./offline-notification.component.css']
})
export class OfflineNotificationComponent implements OnInit {

  isOffline: boolean = !this.networkStatusService.isOnline();

  constructor(private networkStatusService: NetworkStatusService) {}

  ngOnInit() {
    this.networkStatusService.onlineStatusChanged.subscribe((online) => {
      if(!this.isOffline)
        this.isOffline = !online;
    });
  }

}
