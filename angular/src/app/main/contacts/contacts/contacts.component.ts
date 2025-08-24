import { BackupAllConversationModalComponent } from './../backup-all-conversation-modal/backup-all-conversation-modal.component';
import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ContactsServiceProxy, ContactDto, ContactsInterestedOfModel, LoyaltyServiceProxy, CustomerBehaviourServiceProxy, CustomerBehaviourModel  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
// import { CreateOrEditContactModalComponent } from './create-or-edit-contact-modal.component';
import { ViewContactModalComponent } from './view-contact-modal.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../../services/dark-mode.service';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import * as rtlDetect from 'rtl-detect';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ConfirmationService } from 'primeng/api';


@Component({
    templateUrl: './contacts.component.html',
    encapsulation: ViewEncapsulation.None,
    styleUrls: ['./contacts.component.less'],
    animations: [appModuleAnimation()],

})
export class ContactsComponent extends AppComponentBase {


    private cancelRequest$: Subject<void> = new Subject<void>();
    private messageService: MessageService
    theme:string;
    @ViewChild('myModalClose') modalClose;
    // @ViewChild('createOrEditContactModal', { static: true }) createOrEditContactModal: CreateOrEditContactModalComponent;
    @ViewChild('viewContactModalComponent', { static: true }) viewContactModal: ViewContactModalComponent;   
    @ViewChild('backupAllConversationModal', { static: true }) BackupAllModal: BackupAllConversationModalComponent;   
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    //filterText = '';
    filterName = '';
    filterPhoneNumber = '';
    isLockedByAgentFilter = -1;
    lockedByAgentNameFilter = '';
    isOpenFilter = -1;
    emailAddressFilter = '';
    userIdFilter = '';
    chatStatuseChatStatusNameFilter = '';
    contactStatuseContactStatusNameFilter = '';
    loadingTable : boolean = false;
    isTenantLoyal=false;
    interestedOf: ContactsInterestedOfModel[] = [new ContactsInterestedOfModel()];
    isArabic= false;
    selectedStatus: number | null = null;

    constructor(
        public darkModeService : DarkModeService,
        injector: Injector,
        private _contactsServiceProxy: ContactsServiceProxy,
        private _customerBehaviourServiceProxy: CustomerBehaviourServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private confirmationService: ConfirmationService,
    ) {
        super(injector);
    }
    async ngOnInit() {
        this.theme= ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        await this.getIsAdmin();
    }

    statusOptions = [
        { label: this.l('All'), value: null },
        { label: this.l('Neutral'), value: 0 },
        { label: this.l('Subscribed'), value: 2 },
        { label: this.l('Unsubscribed'), value: 1 }
    ];
    getContacts(event?: LazyLoadEvent) {
        // if (this.primengTableHelper.shouldResetPaging(event)) {
        //     this.paginator.changePage(0);
        //     return;
        // }
        this.loadingTable = true;
        this.cancelRequest$.next(); 
        // this.primengTableHelper.showLoadingIndicator();
       
        this._contactsServiceProxy.getContact(
            event?.first,
            event?.rows,
            this.filterName,
            this.filterPhoneNumber,
            this.selectedStatus
        ).pipe(
            takeUntil(this.cancelRequest$)
        ).subscribe(result => {
            // this.primengTableHelper.hideLoadingIndicator();
            this.loadingTable = false;
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.lstContacts;
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }


    deleteContact(contactId: number): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._contactsServiceProxy.delete(contactId)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }
    deleteContactChat(contact: ContactDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._contactsServiceProxy.deleteContactChat(contact.id)
                        .subscribe(response => {
                            if(response === false){
                                this.notify.error(this.l('thisContactNotExist'));
                            }else{
                            // this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                            }
                          
                        });
                }
            }
        );
    }
    BackUpConversation(contact: ContactDto): void {
        this._contactsServiceProxy.backUpConversation(contact)
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
            this.reloadPage();
            this.notify.success(this.l('SuccessfullyBackUp'));
         });
    }
   
    block(contactId: number,flag : boolean): void {
            this._contactsServiceProxy.blockContact(contactId,flag,this.appSession.user.userName).subscribe(result => {
                let contactArray = []
                contactArray.push(result);
                this.primengTableHelper.records = contactArray;
            });
    }
       
    exportToExcel(event?: LazyLoadEvent): void {
        this._contactsServiceProxy.getContactsToExcel(
            this.dataTable._first,
            this.dataTable._rows,
            this.filterName,
            this.filterPhoneNumber,
            this.selectedStatus
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }


    BackUpConversationForAll(){
        this.BackupAllModal.show()

    }

    handleModalResult(result: boolean) {
        if (result) {
          this.backUpAll();
        }
      }

    backUpAll() {
        this._contactsServiceProxy.backUpConversationForAll()
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
            this.reloadPage();
            this.notify.success(this.l('SuccessfullyBackUp'));
         });
    }

changeBehaviour(contact: any): void {
  const isCurrentlySubscribedOrNeutral = contact.customerOPT === 2 || contact.customerOPT === 0;

  const confirmMessage = isCurrentlySubscribedOrNeutral
    ? 'This contact will be marked as Unsubscribed.\nThey will no longer receive campaigns or template messages,\nbut your team can still chat with them via the Team Inbox.'
    : 'This contact will be marked as Neutral.\nThey will be able to receive campaigns and templates again.';

  this.confirmationService.confirm({
    header: isCurrentlySubscribedOrNeutral ? 'Unsubscribe Contact' : 'Reset to Neutral',
    message: confirmMessage,
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Confirm',
    rejectLabel: 'Cancel',
    acceptButtonStyleClass: 'p-button-sm custom-confirm-btn',
    rejectButtonStyleClass: 'p-button-sm custom-cancel-btn',
    accept: () => this._handleBehaviourChange(contact, isCurrentlySubscribedOrNeutral),
    reject: () => {} // optional
  });
}

private _handleBehaviourChange(contact: any, isCurrentlySubscribedOrNeutral: boolean): void {
    this.loadingTable = true;
    this.cancelRequest$.next();
    const behaviorModel = new CustomerBehaviourModel({
        tenantID: contact.tenantId,
        contactId: contact.id,
        customerOPt: contact.customerOPT,
        start: false,
        stop: false
    });

    this._customerBehaviourServiceProxy.updateCustomerBehaviorByUser(behaviorModel)
        .pipe(takeUntil(this.cancelRequest$))
        .subscribe({
            next: () => {
                this.loadingTable = false;
                const message = isCurrentlySubscribedOrNeutral
                    ? "Contact has been unsubscribed."
                    : "Contact has been set to neutral.";
                this.notify.success(message);
                this.getContacts(); // Refresh the list
            },
            error: () => {
                this.loadingTable = false;
                const message = isCurrentlySubscribedOrNeutral
                    ? "Failed to unsubscribe contact."
                    : "Failed to reset contact.";
                this.notify.error(message);
            }
        });
}

}
