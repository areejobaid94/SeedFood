import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { BanksServiceProxy, CreateOrEditBankDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditBankModal',
    templateUrl: './create-or-edit-bank-modal.component.html'
})
export class CreateOrEditBankModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    bank: CreateOrEditBankDto = new CreateOrEditBankDto();



    constructor(
        injector: Injector,
        private _banksServiceProxy: BanksServiceProxy
    ) {
        super(injector);
    }
    
    show(bankId?: number): void {
    

        if (!bankId) {
            this.bank = new CreateOrEditBankDto();
            this.bank.id = bankId;

            this.active = true;
            this.modal.show();
        } else {
            this._banksServiceProxy.getBankForEdit(bankId).subscribe(result => {
                this.bank = result.bank;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._banksServiceProxy.createOrEdit(this.bank)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccefully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
