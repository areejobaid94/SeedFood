import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ServiceFrquenciesServiceProxy, CreateOrEditServiceFrquencyDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditServiceFrquencyModal',
    templateUrl: './create-or-edit-serviceFrquency-modal.component.html'
})
export class CreateOrEditServiceFrquencyModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    serviceFrquency: CreateOrEditServiceFrquencyDto = new CreateOrEditServiceFrquencyDto();



    constructor(
        injector: Injector,
        private _serviceFrquenciesServiceProxy: ServiceFrquenciesServiceProxy
    ) {
        super(injector);
    }
    
    show(serviceFrquencyId?: number): void {
    

        if (!serviceFrquencyId) {
            this.serviceFrquency = new CreateOrEditServiceFrquencyDto();
            this.serviceFrquency.id = serviceFrquencyId;
            this.serviceFrquency.creationDate = moment().startOf('day');

            this.active = true;
            this.modal.show();
        } else {
            this._serviceFrquenciesServiceProxy.getServiceFrquencyForEdit(serviceFrquencyId).subscribe(result => {
                this.serviceFrquency = result.serviceFrquency;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._serviceFrquenciesServiceProxy.createOrEdit(this.serviceFrquency)
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
