import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { TemplateMessagesServiceProxy, TemplateMessageDto  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditTemplateMessageModalComponent } from './create-or-edit-templateMessage-modal.component';
import { ViewTemplateMessageModalComponent } from './view-templateMessage-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../../services/dark-mode.service';
import * as rtlDetect from 'rtl-detect';
import 'moment-hijri'; // Hijri extension for moment.js
import moment from 'moment';
const { toGregorian } = require('hijri-converter');
@Component({
    templateUrl: './templateMessages.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class TemplateMessagesComponent extends AppComponentBase {
    theme:string;

    
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('createOrEditTemplateMessageModal', { static: true }) createOrEditTemplateMessageModal: CreateOrEditTemplateMessageModalComponent;
    @ViewChild('viewTemplateMessageModalComponent', { static: true }) viewTemplateMessageModal: ViewTemplateMessageModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    templateMessageNameFilter = '';
    maxMessageCreationDateFilter : moment.Moment;
		minMessageCreationDateFilter : moment.Moment;
        templateMessagePurposePurposeFilter = '';


    _entityTypeFullName = 'Infoseed.MessagingPortal.TemplateMessages.TemplateMessage';
    entityHistoryEnabled = false;
    isArabic= false;



    constructor(
        injector: Injector,
        private _templateMessagesServiceProxy: TemplateMessagesServiceProxy,
        private _fileDownloadService: FileDownloadService,
        public darkModeService : DarkModeService,

    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.theme= ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);

       // this.teamInboxSignalR.closeConnection();
        // this.evaluationSignalR.startConnection();
        // this.evaluationSignalR.addBroadcastAgentEvaluationListener();
        // this.evaluationSignalR.addBroadcastBotEvaluationListener();

        // this.orderSignalR.startConnection();
        // this.orderSignalR.addBroadcastAgentOrderListener();
        // this.orderSignalR.addBroadcastBotOrderListener();

        // this.teamInboxSignalR.startConnection();
        // this.teamInboxSignalR.addBroadcastAgentMessagesListener();
        // this.teamInboxSignalR.addBroadcastEndUserMessagesListener();
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return this.isGrantedAny('Pages.Administration.AuditLogs') && customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getTemplateMessages(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._templateMessagesServiceProxy.getAll(
            this.filterText,
            this.templateMessageNameFilter,
            this.maxMessageCreationDateFilter === undefined ? this.maxMessageCreationDateFilter : moment(this.maxMessageCreationDateFilter).endOf('day'),
            this.minMessageCreationDateFilter === undefined ? this.minMessageCreationDateFilter : moment(this.minMessageCreationDateFilter).startOf('day'),
            this.templateMessagePurposePurposeFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            if(this.isArabic){
                result.items.forEach(element => {
                    element.templateMessage.messageCreationDate = moment(this.convertHijriToGregorian(moment(element.templateMessage.messageCreationDate ).locale('en').format('YYYY-MM-DDTHH:mm:ss')))
                });
            }
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }
    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split('T');
        const [year, month, day] = hijriDateString.split('-').map(Number);
        const gregorianDate = toGregorian(year, month, day);
    
        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, '0'),
            String(gregorianDate.gd).padStart(2, '0'),
        ].join('-');
    
        return formattedDate + 'T' + time;
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createTemplateMessage(): void {
        this.createOrEditTemplateMessageModal.show();        
    }


    showHistory(templateMessage: TemplateMessageDto): void {
        this.entityTypeHistoryModal.show({
            entityId: templateMessage.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteTemplateMessage(templateMessage: TemplateMessageDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._templateMessagesServiceProxy.delete(templateMessage.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullydeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._templateMessagesServiceProxy.getTemplateMessagesToExcel(
        this.filterText,
            this.templateMessageNameFilter,
            this.maxMessageCreationDateFilter === undefined ? this.maxMessageCreationDateFilter : moment(this.maxMessageCreationDateFilter).endOf('day'),
            this.minMessageCreationDateFilter === undefined ? this.minMessageCreationDateFilter : moment(this.minMessageCreationDateFilter).startOf('day'),
            this.templateMessagePurposePurposeFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
    
}
