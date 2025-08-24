import { UserServiceService } from '@app/shared/layout/notifications/UserService.service';
import { Component, EventEmitter, Injector, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NotificationServiceProxy, UserNotification, UserNotificationState } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { IFormattedUserNotification, UserNotificationHelper } from './UserNotificationHelper';
import { finalize } from 'rxjs/operators';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../../services/dark-mode.service';
import { NotificationsService } from './../../../services/notifications.service';

import * as rtlDetect from 'rtl-detect';
import { Router } from '@node_modules/@angular/router';

@Component({
    selector: 'app-notifications',
    templateUrl: './notifications.component.html',
    styleUrls: ['./notifications.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class NotificationsComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    notifications: IFormattedUserNotification[] = [];

    readStateFilter = 'ALL';
    dateRange: Date[] = [
        moment().subtract(1, 'year').startOf('day').toDate(),  // Start of the same date last year
        moment().toDate()  // Current date and time
    ];
    
    loading = false;
    theme: string;
    isArabic= false;

    constructor(
        injector: Injector,
        private _notificationService: NotificationServiceProxy,
        private _userNotificationHelper: UserNotificationHelper,
        public darkModeService: DarkModeService,
        public notificationsService : NotificationsService,
        private userService: UserServiceService,
        private router: Router,
        public notificationService: NotificationsService,

    ) {
        super(injector);
    }


    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    setAsRead(record: any): void {
        this.setNotificationAsRead(record, () => {
            this.reloadPage();
        });
        
        setTimeout(()=>{
            this.notificationService.loadNotifications(); 
        },100
        )


    }

    isRead(record: any): boolean {
        return record.formattedNotification.state === 'READ';
    }

    fromNow(date: moment.Moment): string {
        return moment(date).fromNow();
    }

    formatRecord(record: any): IFormattedUserNotification {
        return this._userNotificationHelper.format(record, false);
    }

    formatNotification(record: any): string {
        const formattedRecord = this.formatRecord(record);
        return abp.utils.truncateStringWithPostfix(formattedRecord.text, 120);
    }

    formatNotifications(records: any[]): any[] {
        const formattedRecords = [];
        for (const record of records) {
            record.formattedNotification = this.formatRecord(record);
            formattedRecords.push(record);
        }
        return formattedRecords;
    }

    truncateString(text: any, length: number): string {
        return abp.utils.truncateStringWithPostfix(text, length);
    }

    getNotifications(event?: LazyLoadEvent): void {
        if (event && this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
    
        this.primengTableHelper.showLoadingIndicator();
        const startDate = this.dateRange && this.dateRange[0] ? moment(this.dateRange[0]).startOf('day') : moment().startOf('year').subtract(1, 'year');
        const endDate = this.dateRange && this.dateRange[1] ? moment(this.dateRange[1]).endOf('day') : moment('2025-02-02').endOf('day');

        this._notificationService.getUserNotifications(
            this.readStateFilter === 'ALL' ? undefined : UserNotificationState.Unread,
            startDate,
            endDate,
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.primengTableHelper.getSkipCount(this.paginator, event)
        ).pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator())).subscribe((result) => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = this.formatNotifications(result.items);
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    setAllNotificationsAsRead(): void {
        this._userNotificationHelper.setAllAsRead(() => {
            this.getNotifications();
        });
    }

    openNotificationSettingsModal(): void {
        this._userNotificationHelper.openSettingsModal();
    }

    setNotificationAsRead(userNotification: UserNotification, callback: () => void): void {
        this._userNotificationHelper
            .setAsRead(userNotification.id, () => {
                if (callback) {
                    callback();
                }
            });
    }

    deleteNotification(userNotification: UserNotification): void {
        this.message.confirm(
            this.l('NotificationDeleteWarningMessage'),
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._notificationService.deleteNotification(userNotification.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                            this.notificationsService.loadNotifications();
                        });
                }
            }
        );
    }

    deleteNotifications() {
        this.message.confirm(
            this.l('DeleteListedNotificationsWarningMessage'),
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._notificationService.deleteAllUserNotifications(
                        this.readStateFilter === 'ALL' ? undefined : UserNotificationState.Unread,
                        moment(this.dateRange[0]),
                        moment(this.dateRange[1]).endOf('day')).subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                            this.notificationsService.loadNotifications();
                        });
                }
            }
        );
    }

    public getRowClass(formattedRecord: IFormattedUserNotification): string {
        return formattedRecord.state === 'READ' ? 'notification-read' : '';
    }

    getNotificationTextBySeverity(severity: abp.notifications.severity): string {
        switch (severity) {
            case abp.notifications.severity.SUCCESS:
                return this.l('Booked');
            case abp.notifications.severity.WARN:
                return this.l('Cancled');
            case abp.notifications.severity.ERROR:
                return this.l('Pending');
            case abp.notifications.severity.FATAL:
                return this.l('Pending');
            case abp.notifications.severity.INFO:
            default:
                return this.l('Booked');
        }
    }

    // onDateRangeUpdate(event?: any) {
    //     debugger
    //     if (event && event.length === 2 && event[0] !== "Invalid Date" && event[1] !== "Invalid Date") {
    //         this.dateRange = event;
    //         this.dateRange[0] = this.setToStartOfDay(this.dateRange[0]);
    //         this.dateRange[1] = this.setToEndOfDay(this.dateRange[1]);
    //         this.getNotifications();
    //     }
    // }
    

    onDateRangeUpdate() {
            this.dateRange[0] = this.setToStartOfDay(this.dateRange[0]);
            this.dateRange[1] = this.setToEndOfDay(this.dateRange[1]);
            this.getNotifications();
        
    }
    UserAssignToTeamInox(userId: string) {
        const CustomerID=userId;
        this.router.navigate(
            ["/app/main/teamInbox/teamInbox12"],
            {
                queryParams: { CustomerID },
            }
        );
    }

    getNumberAfterComma(text: string): string {
            if (!text.includes(',')) return ''; 
            return text.split(',')[1].trim(); 
        }
        

        

}
