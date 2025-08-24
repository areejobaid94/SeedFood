import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ReceiptDetailsServiceProxy, CreateOrEditReceiptDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { MasterDetailChild_Receipt_ReceiptDetailAccountBillingLookupTableModalComponent } from './masterDetailChild_Receipt_receiptDetail-accountBilling-lookup-table-modal.component';

@Component({
    selector: 'masterDetailChild_Receipt_createOrEditReceiptDetailModal',
    templateUrl: './masterDetailChild_Receipt_create-or-edit-receiptDetail-modal.component.html'
})
export class MasterDetailChild_Receipt_CreateOrEditReceiptDetailModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('receiptDetailAccountBillingLookupTableModal', { static: true }) receiptDetailAccountBillingLookupTableModal: MasterDetailChild_Receipt_ReceiptDetailAccountBillingLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    receiptDetail: CreateOrEditReceiptDetailDto = new CreateOrEditReceiptDetailDto();

    accountBillingBillID = '';


    constructor(
        injector: Injector,
        private _receiptDetailsServiceProxy: ReceiptDetailsServiceProxy
    ) {
        super(injector);
    }
    
                 receiptId: any;
             
    show(
                 receiptId: any,
             receiptDetailId?: number): void {
    
                 this.receiptId = receiptId;
             

        if (!receiptDetailId) {
            this.receiptDetail = new CreateOrEditReceiptDetailDto();
            this.receiptDetail.id = receiptDetailId;
            this.receiptDetail.billDateFrom = moment().startOf('day');
            this.receiptDetail.billDateTo = moment().startOf('day');
            this.accountBillingBillID = '';

            this.active = true;
            this.modal.show();
        } else {
            this._receiptDetailsServiceProxy.getReceiptDetailForEdit(receiptDetailId).subscribe(result => {
                this.receiptDetail = result.receiptDetail;

                this.accountBillingBillID = result.accountBillingBillID;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
                this.receiptDetail.receiptId = this.receiptId;
            
            this._receiptDetailsServiceProxy.createOrEdit(this.receiptDetail)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectAccountBillingModal() {
        this.receiptDetailAccountBillingLookupTableModal.id = this.receiptDetail.accountBillingId;
        this.receiptDetailAccountBillingLookupTableModal.displayName = this.accountBillingBillID;
        this.receiptDetailAccountBillingLookupTableModal.show();
    }


    setAccountBillingIdNull() {
        this.receiptDetail.accountBillingId = null;
        this.accountBillingBillID = '';
    }


    getNewAccountBillingId() {
        this.receiptDetail.accountBillingId = this.receiptDetailAccountBillingLookupTableModal.id;
        this.accountBillingBillID = this.receiptDetailAccountBillingLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
