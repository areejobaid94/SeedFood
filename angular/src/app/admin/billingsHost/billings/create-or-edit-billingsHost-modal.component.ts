import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { BillingsServiceProxy, CreateOrEditBillingDto, TenantServiceModalDto, TenantServicesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { BillingHostCurrencyLookupTableModalComponent } from './billingsHost-currency-lookup-table-modal.component';
import { FormControl } from '@angular/forms';


@Component({
    selector: 'createOrEditBillingHostModal',
    templateUrl: './create-or-edit-billingsHost-modal.component.html'
})
export class CreateOrEditBillingHostModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('billingHostCurrencyLookupTableModal', { static: true }) billingHostCurrencyLookupTableModal: BillingHostCurrencyLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    billing: CreateOrEditBillingDto = new CreateOrEditBillingDto();

    currencyCurrencyName = '';
    services: TenantServiceModalDto[];

    constructor(
        injector: Injector,
        private _billingsServiceProxy: BillingsServiceProxy,
        private _tenantServiceService: TenantServicesServiceProxy
    ) {
        super(injector);
    }
    
    show(billingId?: number,tenenetId?:number): void {
    

        if (!billingId) {
            this.billing = new CreateOrEditBillingDto();
            this.billing.id = billingId;
            this.billing.billingDate = moment().startOf('day');
            this.billing.billPeriodTo = moment().startOf('day');
            this.billing.billPeriodFrom = moment().startOf('day');
            this.billing.dueDate = moment().startOf('day');
            this.currencyCurrencyName = '';
            this.billing.tenantId=tenenetId;





            this._tenantServiceService.getTenatServices(tenenetId).subscribe((result) => {
        
                this.services = result;
    
                this.billing.tenantService=result;
                // this.services.forEach(element => {
                   
                //     if(element.isSelected){
                //         this.activeCheckboxFormArray.push(new FormControl(element));
                //     }else{
                //         let index = this.activeCheckboxFormArray.controls.findIndex(x => x.value == element)
                //         if(index>=0)
                //         this.activeCheckboxFormArray.removeAt(index);
                //     }
                    
                // });
    
            });


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

			//this.billing.tenantService=
			
            this._billingsServiceProxy.createBilling(this.billing)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectCurrencyModal() {
        this.billingHostCurrencyLookupTableModal.id = this.billing.currencyId;
        this.billingHostCurrencyLookupTableModal.displayName = this.currencyCurrencyName;
        this.billingHostCurrencyLookupTableModal.show();
    }


    setCurrencyIdNull() {
        this.billing.currencyId = null;
        this.currencyCurrencyName = '';
    }


    getNewCurrencyId() {
        this.billing.currencyId = this.billingHostCurrencyLookupTableModal.id;
        this.currencyCurrencyName = this.billingHostCurrencyLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
