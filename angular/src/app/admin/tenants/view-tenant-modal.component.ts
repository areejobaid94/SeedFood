import { Component, ElementRef, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { TeamInboxService } from '@app/main/teamInbox/teaminbox.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CommonLookupServiceProxy, ItemsServiceProxy, SubscribableEditionComboboxItemDto, TenantEditDto, TenantServiceProxy } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Paginator } from 'primeng/paginator';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'viewTenantModal',
    templateUrl: './view-tenant-modal.component.html'
})
export class viewTenantModalComponent extends AppComponentBase {

    @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
    @ViewChild('viewModal', { static: true }) modal: ModalDirective;
    @ViewChild('SubscriptionEndDateUtc') subscriptionEndDateUtc: ElementRef;
    @ViewChild('paginator', {static: true}) paginator: Paginator;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    isUnlimited = false;
    subscriptionEndDateUtcIsValid = false;
    imagSrc:string;
    tenant: TenantEditDto ;
    currentConnectionString: string;
    editions: SubscribableEditionComboboxItemDto[] = [];
    isSubscriptionFieldsVisible = false;

    fileUrl:string;
    fileUrl2:string;
    tenantId2:number;
    fromFileUplode: any;

     name :string;
    tenancyName:string;
    
    
    phoneNumber:string;
    website:string;
    email:string;
    address:string;



    constructor(
        injector: Injector,
        private _itemsServiceProxy: ItemsServiceProxy,
        private teamService: TeamInboxService,
        private _tenantService: TenantServiceProxy,
        private _commonLookupService: CommonLookupServiceProxy
    ) {
        super(injector);
    }
  

    
    show(tenantId: number): void {

        this.tenant=new TenantEditDto;
        this.tenantId2=tenantId;
        this.imagSrc="";
        this.active = true;

        this._commonLookupService.getEditionsForCombobox(false).subscribe(editionsResult => {
            this.editions = editionsResult.items;
            let notSelectedEdition = new SubscribableEditionComboboxItemDto();
            notSelectedEdition.displayText = this.l('NotAssigned');
            notSelectedEdition.value = '';
            this.editions.unshift(notSelectedEdition);

            this._tenantService.getTenantForEdit(tenantId).subscribe((tenantResult) => {
                    


               this.name =tenantResult.name;
               this.tenancyName =tenantResult.tenancyName; 
               this.phoneNumber =tenantResult.phoneNumber;
               this.website =tenantResult.website;
               this.email =tenantResult.email;
               this.address =tenantResult.address;


                this.fileUrl=tenantResult.fileUrl;
                this.tenant = tenantResult;
                this.currentConnectionString = tenantResult.connectionString;
                this.tenant.editionId = this.tenant.editionId || 0;
                this.isUnlimited = !this.tenant.subscriptionEndDateUtc;
                this.subscriptionEndDateUtcIsValid = this.isUnlimited || this.tenant.subscriptionEndDateUtc !== undefined;
                this.modal.show();
                this.toggleSubscriptionFields();
            });
        });
    }

    onShown(): void {
        // document.getElementById('Name').focus();

        // if (this.tenant.subscriptionEndDateUtc) {
        //     (this.subscriptionEndDateUtc.nativeElement as any).value = moment(this.tenant.subscriptionEndDateUtc).format('L');
        // }
    }

    subscriptionEndDateChange(e): void {
        this.subscriptionEndDateUtcIsValid = e && e.date !== false || this.isUnlimited;
    }

    selectedEditionIsFree(): boolean {
        if (!this.tenant.editionId) {
            return true;
        }

        let selectedEditions = _.filter(this.editions, { value: this.tenant.editionId + '' });
        if (selectedEditions.length !== 1) {
            return true;
        }

        let selectedEdition = selectedEditions[0];
        return selectedEdition.isFree;
    }

    save(): void {
        this.saving = true;
        if (this.tenant.editionId === 0) {
            this.tenant.editionId = null;
        }

        if (this.isUnlimited) {
            this.tenant.isInTrialPeriod = false;
        }

        //take selected date as UTC
        if (this.isUnlimited || !this.tenant.editionId) {
            this.tenant.subscriptionEndDateUtc = null;
        }

        this._tenantService.updateTenant(this.tenant)
            .pipe(finalize(() => this.saving = false))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
                this.reloadPage();
            });
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    onEditionChange(): void {
        if (this.selectedEditionIsFree()) {
            this.tenant.isInTrialPeriod = false;
        }

        this.toggleSubscriptionFields();
    }

    onBotChange(): void {
        if (this.selectedEditionIsFree()) {
            this.tenant.isInTrialPeriod = false;
        }

        this.toggleSubscriptionFields();
    }

    onUnlimitedChange(): void {
        if (this.isUnlimited) {
            this.tenant.subscriptionEndDateUtc = null;
            this.subscriptionEndDateUtcIsValid = true;
            this.tenant.isInTrialPeriod = false;
        } else {
            if (!this.tenant.subscriptionEndDateUtc) {
                this.subscriptionEndDateUtcIsValid = false;
            }
        }
    }

    toggleSubscriptionFields() {
        if (this.tenant.editionId > 0) {
            this.isSubscriptionFieldsVisible = true;
        } else {
            this.isSubscriptionFieldsVisible = false;
        }
    }


    exportToExcel(): void {


    }

    openFileUploder2() {
             
        // this.categoryIndex = category;
        // this.itemIndex = index;
        // this.fromFileUplode = true;
         document.getElementById('uplode2').click();
              
    }
    onFileChange2(event) {
             
        const reader = new FileReader();

        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
                 

            let form = new FormData();
            form.append('FormFile', file);
            this._itemsServiceProxy.getFileUrl(form).subscribe(res => {
                this.imagSrc=res['result'];
                this.fileUrl=res['result'];



            });
        }
    }

    onSaveButton() {
     
        this._tenantService.updateTenantFile(this.imagSrc,this.tenantId2).subscribe(res2 => {
                 
            this.fileUrl=this.imagSrc;
            this.fromFileUplode = false;
            this.notify.success(this.l('Successfully Unlocked'));
        });
    }


    openFileUploder1() {
         document.getElementById('uplode1').click();
       
    }


}
