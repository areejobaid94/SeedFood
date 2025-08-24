import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PaymentMethodsServiceProxy, CreateOrEditPaymentMethodDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditPaymentMethodModal',
    templateUrl: './create-or-edit-paymentMethod-modal.component.html'
})
export class CreateOrEditPaymentMethodModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    paymentMethod: CreateOrEditPaymentMethodDto = new CreateOrEditPaymentMethodDto();



    constructor(
        injector: Injector,
        private _paymentMethodsServiceProxy: PaymentMethodsServiceProxy
    ) {
        super(injector);
    }
    
    show(paymentMethodId?: number): void {
    

        if (!paymentMethodId) {
            this.paymentMethod = new CreateOrEditPaymentMethodDto();
            this.paymentMethod.id = paymentMethodId;

            this.active = true;
            this.modal.show();
        } else {
            this._paymentMethodsServiceProxy.getPaymentMethodForEdit(paymentMethodId).subscribe(result => {
                this.paymentMethod = result.paymentMethod;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._paymentMethodsServiceProxy.createOrEdit(this.paymentMethod)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
