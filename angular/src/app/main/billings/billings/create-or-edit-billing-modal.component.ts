import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { BillingsServiceProxy, CreateOrEditBillingDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { BillingCurrencyLookupTableModalComponent } from './billing-currency-lookup-table-modal.component';

@Component({
    selector: 'createOrEditBillingModal',
    templateUrl: './create-or-edit-billing-modal.component.html'
})
export class CreateOrEditBillingModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('billingCurrencyLookupTableModal', { static: true }) billingCurrencyLookupTableModal: BillingCurrencyLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    billing: CreateOrEditBillingDto = new CreateOrEditBillingDto();

    currencyCurrencyName = '';


    constructor(
        injector: Injector,
        private _billingsServiceProxy: BillingsServiceProxy
    ) {
        super(injector);
    }
    
    show(billingId?: number): void {
    

        if (!billingId) {
            this.billing = new CreateOrEditBillingDto();
            this.billing.id = billingId;
            this.billing.billingDate = moment().startOf('day');
            this.billing.billPeriodTo = moment().startOf('day');
            this.billing.billPeriodFrom = moment().startOf('day');
            this.billing.dueDate = moment().startOf('day');
            this.currencyCurrencyName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._billingsServiceProxy.getBillingForEdit(billingId).subscribe(result => {
                this.billing = result.billing;

                this.currencyCurrencyName = result.currencyCurrencyName;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._billingsServiceProxy.createOrEdit(this.billing)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectCurrencyModal() {
        this.billingCurrencyLookupTableModal.id = this.billing.currencyId;
        this.billingCurrencyLookupTableModal.displayName = this.currencyCurrencyName;
        this.billingCurrencyLookupTableModal.show();
    }


    setCurrencyIdNull() {
        this.billing.currencyId = null;
        this.currencyCurrencyName = '';
    }


    getNewCurrencyId() {
        this.billing.currencyId = this.billingCurrencyLookupTableModal.id;
        this.currencyCurrencyName = this.billingCurrencyLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
