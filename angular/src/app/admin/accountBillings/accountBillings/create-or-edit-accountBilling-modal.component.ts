import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AccountBillingsServiceProxy, CreateOrEditAccountBillingDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { AccountBillingInfoSeedServiceLookupTableModalComponent } from './accountBilling-infoSeedService-lookup-table-modal.component';
import { AccountBillingServiceTypeLookupTableModalComponent } from './accountBilling-serviceType-lookup-table-modal.component';
import { AccountBillingCurrencyLookupTableModalComponent } from './accountBilling-currency-lookup-table-modal.component';
import { AccountBillingBillingLookupTableModalComponent } from './accountBilling-billing-lookup-table-modal.component';

@Component({
    selector: 'createOrEditAccountBillingModal',
    templateUrl: './create-or-edit-accountBilling-modal.component.html'
})
export class CreateOrEditAccountBillingModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('accountBillingInfoSeedServiceLookupTableModal', { static: true }) accountBillingInfoSeedServiceLookupTableModal: AccountBillingInfoSeedServiceLookupTableModalComponent;
    @ViewChild('accountBillingServiceTypeLookupTableModal', { static: true }) accountBillingServiceTypeLookupTableModal: AccountBillingServiceTypeLookupTableModalComponent;
    @ViewChild('accountBillingCurrencyLookupTableModal', { static: true }) accountBillingCurrencyLookupTableModal: AccountBillingCurrencyLookupTableModalComponent;
    @ViewChild('accountBillingBillingLookupTableModal', { static: true }) accountBillingBillingLookupTableModal: AccountBillingBillingLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    accountBilling: CreateOrEditAccountBillingDto = new CreateOrEditAccountBillingDto();

    infoSeedServiceServiceName = '';
    serviceTypeServicetypeName = '';
    currencyCurrencyName = '';
    billingBillingID = '';


    constructor(
        injector: Injector,
        private _accountBillingsServiceProxy: AccountBillingsServiceProxy
    ) {
        super(injector);
    }
    
    show(accountBillingId?: number): void {
    

        if (!accountBillingId) {
            this.accountBilling = new CreateOrEditAccountBillingDto();
            this.accountBilling.id = accountBillingId;
            this.accountBilling.billDateFrom = moment().startOf('day');
            this.accountBilling.billDateTo = moment().startOf('day');
            this.infoSeedServiceServiceName = '';
            this.serviceTypeServicetypeName = '';
            this.currencyCurrencyName = '';
            this.billingBillingID = '';

            this.active = true;
            this.modal.show();
        } else {
            this._accountBillingsServiceProxy.getAccountBillingForEdit(accountBillingId).subscribe(result => {
                this.accountBilling = result.accountBilling;

                this.infoSeedServiceServiceName = result.infoSeedServiceServiceName;
                this.serviceTypeServicetypeName = result.serviceTypeServicetypeName;
                this.currencyCurrencyName = result.currencyCurrencyName;
                this.billingBillingID = result.billingBillingID;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._accountBillingsServiceProxy.createOrEdit(this.accountBilling)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccefully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectInfoSeedServiceModal() {
        this.accountBillingInfoSeedServiceLookupTableModal.id = this.accountBilling.infoSeedServiceId;
        this.accountBillingInfoSeedServiceLookupTableModal.displayName = this.infoSeedServiceServiceName;
        this.accountBillingInfoSeedServiceLookupTableModal.show();
    }
    openSelectServiceTypeModal() {
        this.accountBillingServiceTypeLookupTableModal.id = this.accountBilling.serviceTypeId;
        this.accountBillingServiceTypeLookupTableModal.displayName = this.serviceTypeServicetypeName;
        this.accountBillingServiceTypeLookupTableModal.show();
    }
    openSelectCurrencyModal() {
        this.accountBillingCurrencyLookupTableModal.id = this.accountBilling.currencyId;
        this.accountBillingCurrencyLookupTableModal.displayName = this.currencyCurrencyName;
        this.accountBillingCurrencyLookupTableModal.show();
    }
    openSelectBillingModal() {
        this.accountBillingBillingLookupTableModal.id = this.accountBilling.billingId;
        this.accountBillingBillingLookupTableModal.displayName = this.billingBillingID;
        this.accountBillingBillingLookupTableModal.show();
    }


    setInfoSeedServiceIdNull() {
        this.accountBilling.infoSeedServiceId = null;
        this.infoSeedServiceServiceName = '';
    }
    setServiceTypeIdNull() {
        this.accountBilling.serviceTypeId = null;
        this.serviceTypeServicetypeName = '';
    }
    setCurrencyIdNull() {
        this.accountBilling.currencyId = null;
        this.currencyCurrencyName = '';
    }
    setBillingIdNull() {
        this.accountBilling.billingId = null;
        this.billingBillingID = '';
    }


    getNewInfoSeedServiceId() {
        this.accountBilling.infoSeedServiceId = this.accountBillingInfoSeedServiceLookupTableModal.id;
        this.infoSeedServiceServiceName = this.accountBillingInfoSeedServiceLookupTableModal.displayName;
    }
    getNewServiceTypeId() {
        this.accountBilling.serviceTypeId = this.accountBillingServiceTypeLookupTableModal.id;
        this.serviceTypeServicetypeName = this.accountBillingServiceTypeLookupTableModal.displayName;
    }
    getNewCurrencyId() {
        this.accountBilling.currencyId = this.accountBillingCurrencyLookupTableModal.id;
        this.currencyCurrencyName = this.accountBillingCurrencyLookupTableModal.displayName;
    }
    getNewBillingId() {
        this.accountBilling.billingId = this.accountBillingBillingLookupTableModal.id;
        this.billingBillingID = this.accountBillingBillingLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
