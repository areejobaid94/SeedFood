import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ContactsServiceProxy, MessageTemplateModel, WhatsAppCampaignModel, WhatsAppEntity, WhatsAppMessageTemplateServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import * as _ from 'lodash';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '../../services/dark-mode.service';
import { LazyLoadEvent } from 'primeng/api';
import { ActivatedRoute, Router } from "@angular/router";
import { FileDownloadService } from '@shared/utils/file-download.service';
import { AbpSessionService } from 'abp-ng2-module';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    templateUrl: './external-contact.component.html',
    encapsulation: ViewEncapsulation.None,
    styleUrls: ['./external-contact.component.css'],
    animations: [appModuleAnimation()]
})
export class ExternalContactsComponent extends AppComponentBase {
    private cancelRequest$: Subject<void> = new Subject<void>();
    theme:string;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    first: number = 0;
    rows: number = 20;
    name :string= '';
    phone :string= '';
    templateId : number =null
    campaignId : number =null;
    isDialogSubmitted : boolean = false;
    phoneNumberEdited : string = ''
    campaignFlag : boolean = false;
    messageStatus : string= null;
    isSent:boolean = null
    isDelivered:boolean = null
    selectedContacttoUpdate! : any;
    loadingTable : boolean = false;
    isRead:boolean = null
    isFailed:boolean = null
    isHanged:boolean = null
    whatsAppEntity : WhatsAppEntity = new WhatsAppEntity()
    templates: MessageTemplateModel[] = [new MessageTemplateModel()];
    campaign : WhatsAppCampaignModel[] = [new WhatsAppCampaignModel()]
    constructor(
        injector: Injector,
        private _contactsServiceProxy: ContactsServiceProxy,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        public darkModeService : DarkModeService,
        private route: ActivatedRoute,
        private router: Router,
        private _fileDownloadService: FileDownloadService

    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme= ThemeHelper.getTheme();
        this.getTemplates();
        if (this.route.snapshot.queryParams["templateId"] != null) {
            this.templateId = this.route.snapshot.queryParams["templateId"];
            this.getCampaign();
        }else{
              this.templateId=null;
        }
        if (this.route.snapshot.queryParams["campaignId"] != null) {
            this.campaignId = this.route.snapshot.queryParams["campaignId"];
             this.campaignFlag = true;
             this.messageStatus = "all";
        }else{
               this.campaignId = null;
               this.campaignFlag= false;
        }
    }
    getTemplates(){
        this.templates = [new MessageTemplateModel()]
        this._whatsAppMessageTemplateServiceProxy.getWhatsAppTemplateForCampaign(0,10000,this.appSession.tenantId).subscribe(template => {
            this.templates = template.lstWhatsAppTemplateModel
        });
        
    }

    visible: boolean = false;
    showDialog(contact : any) {
        if(!contact.isFailed)
        return;
        this.phoneNumberEdited = contact.phoneNumber;
        this.visible = true;
        this.selectedContacttoUpdate = contact;
    }

    handleDeleteContact(contact : any){
        if(!contact.isFailed)
        return;
        
        this.message.confirm(
            "", this.l("Are You sure you want to delete") + ' ' + contact.phoneNumber +  " ?",
            (isConfirmed) => {
                if (isConfirmed) {
           
                    this.primengTableHelper.showLoadingIndicator();

                    this._contactsServiceProxy
                        .contactDelete(contact.id)
                        .subscribe((res) => {
                            this.primengTableHelper.hideLoadingIndicator();
                            if(res.state===2){
                                this.notify.success(this.l("succeesfull"));
                            }else if(res.state===1){
                                this.notify.warn(res.message);

                            }else{
                                this.notify.error(this.l("ErrorHappenedOnSavingModal"));

                            }
                        });
                }
            }
        );
    }

    onKeyPress(event: any) {
        const allowedKeys = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'Backspace', 'ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown', 'Delete', 'Tab'];
        if (!allowedKeys.includes(event.key)) {
            event.preventDefault();
        }
    }
    

    handleSubmitDialog(){
        this.isDialogSubmitted = true;
        this.visible = false;
        this.message.confirm(
            "", this.l("Are you sure you want to update") + ' ' +this.selectedContacttoUpdate.phoneNumber + " ?",
            (isConfirmed) => {
                if (isConfirmed) {
           
                    this.primengTableHelper.showLoadingIndicator();

                    this._contactsServiceProxy
                        .phoneNumberUpdate(this.selectedContacttoUpdate.id , this.phoneNumberEdited)
                        .subscribe((res) => {
                            this.primengTableHelper.hideLoadingIndicator();
                            if(res.state===2){
                                this.notify.success(this.l("succeesfull"));
                            }else if(res.state===1){
                                this.notify.warn(this.l("invalidFormat"));

                            }else if(res.state===3){
                                this.notify.warn(this.l("contactAlreadyexits"));

                            }else{
                                this.notify.error(this.l("ErrorHappenedOnSavingModal"));

                            }
                        });
                }else{
                    this.visible = true;
                }
            }
        );

        this.isDialogSubmitted = false;
    }

    getCampaign(){
        
        this.campaignId=null;
        this.campaignFlag = false;
        this.campaign = [new WhatsAppCampaignModel()]
        if(this.templateId.toString() != ""){
            this._whatsAppMessageTemplateServiceProxy.getCampaignByTemplateId(this.templateId).subscribe(result => {
                if (result.lstWhatsAppCampaignModel.length > 0) {                    
                    this.campaign = result.lstWhatsAppCampaignModel
                    this.campaignFlag = true;
                }
            });
        }
       // this.getContacts()
    }
    getContacts(event?: LazyLoadEvent) {
        let statusID : number = 0;
        // if (this.primengTableHelper.shouldResetPaging(event)) {
        //     this.paginator.changePage(0);
        //     return;
        // }
        this.messageStatus
        if (this.messageStatus != null) {
            
            if(this.messageStatus == "all"){
                this.isSent = null
                this.isDelivered = null
                this.isRead = null
                this.isFailed = null
                this.isHanged = null;
                statusID = 0;
            }
            if(this.messageStatus == "sent"){
                this.isSent = true
                this.isDelivered = null
                this.isRead = null
                this.isFailed = null
                this.isHanged = null;
                statusID = 1;
            }
            if(this.messageStatus == "delivered"){
                this.isSent = null
                this.isDelivered = true
                this.isRead = null
                this.isFailed = null
                this.isHanged = null;
                statusID = 2;

            }
            if(this.messageStatus == "read"){
                this.isSent = null
                this.isDelivered = null
                this.isRead = true
                this.isFailed = null
                this.isHanged = null;
                statusID = 3;
            }
            if(this.messageStatus == "failed"){
                this.isSent = null
                this.isDelivered = null
                this.isRead = null
                this.isFailed = true
                this.isHanged = null;
                statusID = 4;
            }
            if(this.messageStatus == "hanged"){
                this.isSent = null;
                this.isDelivered = null;
                this.isRead = null;
                this.isFailed = null;
                this.isHanged = true;
                statusID = 5;
            }

        }else{
            this.isSent = null
            this.isDelivered = null
            this.isRead = null
            this.isFailed = null
            this.isHanged = null
            statusID = 0;
        }
        if (isNaN(Number(this.templateId))) {
            this.templateId = null
        }
        if (isNaN(Number(this.campaignId))) {
            this.campaignId = null
        }

        this.loadingTable = true;

        this.cancelRequest$.next(); 
        this._contactsServiceProxy.contactsCampaignGet(
            event?.first,
            event?.rows,
            this.phone,
            this.templateId,
            this.campaignId,
            statusID
        ).pipe(
            takeUntil(this.cancelRequest$)
        )      
        .subscribe(result => {

            debugger
            this.loadingTable = false;
            this.primengTableHelper.hideLoadingIndicator();
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.contacts;
        });
        // this._contactsServiceProxy.getContactsCampaign(
        //     this.appSession.tenantId,
        //     this.primengTableHelper.getSkipCount(this.paginator, event),
        //     this.primengTableHelper.getMaxResultCount(this.paginator, event),
        //     this.phone,
        //     this.templateId,
        //     this.campaignId,
        //     this.isSent,
        //     this.isDelivered,
        //     this.isRead,
        //     this.isFailed,
        //     this.isHanged
        // ).subscribe(result => {
        
        //     this.primengTableHelper.hideLoadingIndicator();
        //     this.primengTableHelper.totalRecordsCount = result.totalCount;
        //     this.primengTableHelper.records = result.contacts;
        // });
    }
    generateReport(){
        this._contactsServiceProxy.exportContactCampaignToExcel(
            this.templateId,
            this.campaignId,
            this.isSent,
            this.isDelivered,
            this.isRead,
            this.isFailed,
            this.isHanged
        ).subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
        });
    }
}
