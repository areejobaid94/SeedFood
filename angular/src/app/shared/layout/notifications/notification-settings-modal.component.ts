import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GetNotificationSettingsOutput, NotificationServiceProxy, NotificationSubscriptionDto, UpdateNotificationSettingsInput, UserNotification } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { IFormattedUserNotification, UserNotificationHelper } from './UserNotificationHelper';


@Component({
    selector: 'notificationSettingsModal',
    templateUrl: './notification-settings-modal.component.html'
})
export class NotificationSettingsModalComponent extends AppComponentBase {
    theme:string;

    @ViewChild('modal', {static: true}) modal: ModalDirective;

    saving = false;

    settings: GetNotificationSettingsOutput;

    constructor(
        injector: Injector,
        private _notificationService: NotificationServiceProxy,
    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme= ThemeHelper.getTheme();
    }
   
    openWhatsApp() {
        const phoneNumber = '447455417485'; 
        const whatsappURL = `https://wa.me/${phoneNumber}`;
        window.open(whatsappURL, '_blank');
    }
    

    show() {
        this.getSettings(() => {
            this.modal.show();
        });
    }

    save(): void {
        const input = new UpdateNotificationSettingsInput();
        input.receiveNotifications = this.settings.receiveNotifications;
        input.notifications = _.map(this.settings.notifications,
            (n) => {
                let subscription = new NotificationSubscriptionDto();
                subscription.name = n.name;
                subscription.isSubscribed = n.isSubscribed;
                return subscription;
            });

        this.saving = true;
        this._notificationService.updateNotificationSettings(input)
            .pipe(finalize(() => this.saving = false))
            .subscribe(() => {
                this.notify.info(this.l('savedSuccessfully'));
                this.close();
            });
    }

    close(): void {
        this.modal.hide();
    }

    private getSettings(callback: () => void) {
        this._notificationService.getNotificationSettings().subscribe((result: GetNotificationSettingsOutput) => {
            this.settings = result;
            callback();
        });
    }
}
