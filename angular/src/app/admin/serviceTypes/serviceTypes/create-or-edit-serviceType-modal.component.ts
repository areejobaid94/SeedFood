import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ServiceTypesServiceProxy, CreateOrEditServiceTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditServiceTypeModal',
    templateUrl: './create-or-edit-serviceType-modal.component.html'
})
export class CreateOrEditServiceTypeModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    serviceType: CreateOrEditServiceTypeDto = new CreateOrEditServiceTypeDto();



    constructor(
        injector: Injector,
        private _serviceTypesServiceProxy: ServiceTypesServiceProxy
    ) {
        super(injector);
    }
    
    show(serviceTypeId?: number): void {
    

        if (!serviceTypeId) {
            this.serviceType = new CreateOrEditServiceTypeDto();
            this.serviceType.id = serviceTypeId;
            this.serviceType.creationDate = moment().startOf('day');

            this.active = true;
            this.modal.show();
        } else {
            this._serviceTypesServiceProxy.getServiceTypeForEdit(serviceTypeId).subscribe(result => {
                this.serviceType = result.serviceType;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._serviceTypesServiceProxy.createOrEdit(this.serviceType)
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
