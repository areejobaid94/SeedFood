import { Injectable } from '@angular/core';
import { IFormattedUserNotification, UserNotificationHelper } from '@app/shared/layout/notifications/UserNotificationHelper';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { NotificationServiceProxy, UserNotification } from '@shared/service-proxies/service-proxies';
import _ from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
  unreadNotificationCount = 0;
  notifications: IFormattedUserNotification[] = [];


  constructor(
    private _notificationService: NotificationServiceProxy,
    private _userNotificationHelper: UserNotificationHelper,

  ) { }

  loadNotifications(): void {
    if (UrlHelper.isInstallUrl(location.href)) {
        return;
    }

    this._notificationService
        .getUserNotifications(undefined, undefined, undefined, 3, 0)
        .subscribe((result) => {

          this.unreadNotificationCount = result.unreadCount;
            this.notifications = [];
            _.forEach(result.items, (item: UserNotification) => {
                //
                //  item.notification.creationTime.hour()-7;
                //
                this.notifications.push(
                    this._userNotificationHelper.format(<any>item)
                );
            });
        });
}
}
