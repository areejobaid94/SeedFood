import { Component, Injector, OnInit, ViewEncapsulation, NgZone, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NotificationServiceProxy, UserNotification } from '@shared/service-proxies/service-proxies';
import { IFormattedUserNotification, UserNotificationHelper } from './UserNotificationHelper';
import * as _ from 'lodash';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { Howl } from 'howler';
import { EvaluationSignalRService } from '@app/main/evaluation/evaluation-signalR.service';
import { OrderSignalRService } from '@app/main/order/order-signalR.service';
import { TeamInboxSignalRService } from '@app/main/teamInbox/teaminbox-signalR.service';

@Component({
    templateUrl: './header-notifications.component.html',
    selector: 'header-notifications',
    encapsulation: ViewEncapsulation.None
})
export class HeaderNotificationsComponent extends AppComponentBase implements OnInit {

    notifications: IFormattedUserNotification[] = [];
    unreadNotificationCount = 0;
    @Input() isDropup = false;
    @Input() customStyle = 'btn btn-icon btn-dropdown btn-clean btn-lg mr-1';

    constructor(
        injector: Injector,
        private evaluationSignalR: EvaluationSignalRService,
        private orderSignalR: OrderSignalRService,
        private teamInboxSignalR: TeamInboxSignalRService,
        private _notificationService: NotificationServiceProxy,
        private _userNotificationHelper: UserNotificationHelper,
        public _zone: NgZone
    ) {
        super(injector);
    }

    ngOnInit(): void {
        // this.evaluationSignalR.startConnection();
        // this.evaluationSignalR.addBroadcastAgentEvaluationListener();
        // this.evaluationSignalR.addBroadcastBotEvaluationListener();

        // this.orderSignalR.startConnection();
        // this.orderSignalR.addBroadcastAgentOrderListener();
        // this.orderSignalR.addBroadcastBotOrderListener();

        // this.teamInboxSignalR.startConnection();
        // this.teamInboxSignalR.addBroadcastAgentMessagesListener();
        // this.teamInboxSignalR.addBroadcastEndUserMessagesListener();
        this.loadNotifications();
        this.registerToEvents();

    }

    loadNotifications(): void {
        if (UrlHelper.isInstallUrl(location.href)) {
            return;
        }
        this._notificationService.getUserNotifications(undefined, undefined, undefined, 3, 0).subscribe(result => {
            this.unreadNotificationCount = result.unreadCount;
            this.notifications = [];
            _.forEach(result.items, (item: UserNotification) => {
                //                  
                //  item.notification.creationTime.hour()-7;
                //                  
                this.notifications.push(this._userNotificationHelper.format(<any>item));
            });
        });
    }

    registerToEvents() {
        let self = this;

        function onNotificationReceived(userNotification) {
            self._userNotificationHelper.show(userNotification);
            self.loadNotifications();
        }

        abp.event.on('abp.notifications.received', userNotification => {

                    
                    if(this.appSession.user.id==userNotification.userId){
                        
                        var sound = new Howl({
                            src: ['../assets/common/sound/Bell.mp3']
                          });
                          
                          sound.play();
                    }

            self._zone.run(() => {
                onNotificationReceived(userNotification);
            });
        });

        function onNotificationsRefresh() {
        
            self.loadNotifications();
        }

        abp.event.on('app.notifications.refresh', () => {
            self._zone.run(() => {
                onNotificationsRefresh();
            });
        });

        function onNotificationsRead(userNotificationId) {
            for (let i = 0; i < self.notifications.length; i++) {
                if (self.notifications[i].userNotificationId === userNotificationId) {
                    self.notifications[i].state = 'READ';
                }
            }

            self.unreadNotificationCount -= 1;
        }

        abp.event.on('app.notifications.read', userNotificationId => {
            self._zone.run(() => {
                onNotificationsRead(userNotificationId);
            });
        });
    }

    setAllNotificationsAsRead(): void {
        this._userNotificationHelper.setAllAsRead();
    }

    openNotificationSettingsModal(): void {
        this._userNotificationHelper.openSettingsModal();
    }

    setNotificationAsRead(userNotification: IFormattedUserNotification): void {

        this._userNotificationHelper.setAsRead(userNotification.userNotificationId);
    }

    gotoUrl(url): void {
        if (url) {
            location.href = url;
        }
    }
}
