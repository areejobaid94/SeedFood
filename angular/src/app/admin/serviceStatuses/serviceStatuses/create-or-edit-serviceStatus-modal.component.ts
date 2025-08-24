import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ServiceStatusesServiceProxy, CreateOrEditServiceStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditServiceStatusModal',
    templateUrl: './create-or-edit-serviceStatus-modal.component.html'
})
export class CreateOrEditServiceStatusModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    serviceStatus: CreateOrEditServiceStatusDto = new CreateOrEditServiceStatusDto();



    constructor(
        injector: Injector,
        private _serviceStatusesServiceProxy: ServiceStatusesServiceProxy
    ) {
        super(injector);
    }
    
    show(serviceStatusId?: number): void {
    

        if (!serviceStatusId) {
            this.serviceStatus = new CreateOrEditServiceStatusDto();
            this.serviceStatus.id = serviceStatusId;
            this.serviceStatus.creationDate = moment().startOf('day');

            this.active = true;
            this.modal.show();
        } else {
            this._serviceStatusesServiceProxy.getServiceStatusForEdit(serviceStatusId).subscribe(result => {
                this.serviceStatus = result.serviceStatus;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._serviceStatusesServiceProxy.createOrEdit(this.serviceStatus)
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
