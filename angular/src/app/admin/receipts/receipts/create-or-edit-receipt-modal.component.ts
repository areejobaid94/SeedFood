import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ReceiptsServiceProxy, CreateOrEditReceiptDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { ReceiptBankLookupTableModalComponent } from './receipt-bank-lookup-table-modal.component';
import { ReceiptPaymentMethodLookupTableModalComponent } from './receipt-paymentMethod-lookup-table-modal.component';

@Component({
    selector: 'createOrEditReceiptModal',
    templateUrl: './create-or-edit-receipt-modal.component.html'
})
export class CreateOrEditReceiptModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('receiptBankLookupTableModal', { static: true }) receiptBankLookupTableModal: ReceiptBankLookupTableModalComponent;
    @ViewChild('receiptPaymentMethodLookupTableModal', { static: true }) receiptPaymentMethodLookupTableModal: ReceiptPaymentMethodLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    receipt: CreateOrEditReceiptDto = new CreateOrEditReceiptDto();

    bankBankName = '';
    paymentMethodPaymnetMethod = '';


    constructor(
        injector: Injector,
        private _receiptsServiceProxy: ReceiptsServiceProxy
    ) {
        super(injector);
    }
    
    show(receiptId?: number): void {
    

        if (!receiptId) {
            this.receipt = new CreateOrEditReceiptDto();
            this.receipt.id = receiptId;
            this.receipt.receiptDate = moment().startOf('day');
            this.bankBankName = '';
            this.paymentMethodPaymnetMethod = '';

            this.active = true;
            this.modal.show();
        } else {
            this._receiptsServiceProxy.getReceiptForEdit(receiptId).subscribe(result => {
                this.receipt = result.receipt;

                this.bankBankName = result.bankBankName;
                this.paymentMethodPaymnetMethod = result.paymentMethodPaymnetMethod;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._receiptsServiceProxy.createOrEdit(this.receipt)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectBankModal() {
        this.receiptBankLookupTableModal.id = this.receipt.bankId;
        this.receiptBankLookupTableModal.displayName = this.bankBankName;
        this.receiptBankLookupTableModal.show();
    }
    openSelectPaymentMethodModal() {
        this.receiptPaymentMethodLookupTableModal.id = this.receipt.paymentMethodId;
        this.receiptPaymentMethodLookupTableModal.displayName = this.paymentMethodPaymnetMethod;
        this.receiptPaymentMethodLookupTableModal.show();
    }


    setBankIdNull() {
        this.receipt.bankId = null;
        this.bankBankName = '';
    }
    setPaymentMethodIdNull() {
        this.receipt.paymentMethodId = null;
        this.paymentMethodPaymnetMethod = '';
    }


    getNewBankId() {
        this.receipt.bankId = this.receiptBankLookupTableModal.id;
        this.bankBankName = this.receiptBankLookupTableModal.displayName;
    }
    getNewPaymentMethodId() {
        this.receipt.paymentMethodId = this.receiptPaymentMethodLookupTableModal.id;
        this.paymentMethodPaymnetMethod = this.receiptPaymentMethodLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
