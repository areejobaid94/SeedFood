import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { InfoSeedServicesServiceProxy, CreateOrEditInfoSeedServiceDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { InfoSeedServiceServiceTypeLookupTableModalComponent } from './infoSeedService-serviceType-lookup-table-modal.component';
import { InfoSeedServiceServiceStatusLookupTableModalComponent } from './infoSeedService-serviceStatus-lookup-table-modal.component';
import { InfoSeedServiceServiceFrquencyLookupTableModalComponent } from './infoSeedService-serviceFrquency-lookup-table-modal.component';

@Component({
    selector: 'createOrEditInfoSeedServiceModal',
    templateUrl: './create-or-edit-infoSeedService-modal.component.html'
})
export class CreateOrEditInfoSeedServiceModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('infoSeedServiceServiceTypeLookupTableModal', { static: true }) infoSeedServiceServiceTypeLookupTableModal: InfoSeedServiceServiceTypeLookupTableModalComponent;
    @ViewChild('infoSeedServiceServiceStatusLookupTableModal', { static: true }) infoSeedServiceServiceStatusLookupTableModal: InfoSeedServiceServiceStatusLookupTableModalComponent;
    @ViewChild('infoSeedServiceServiceFrquencyLookupTableModal', { static: true }) infoSeedServiceServiceFrquencyLookupTableModal: InfoSeedServiceServiceFrquencyLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    infoSeedService: CreateOrEditInfoSeedServiceDto = new CreateOrEditInfoSeedServiceDto();

    serviceTypeServicetypeName = '';
    serviceStatusServiceStatusName = '';
    serviceFrquencyServiceFrequencyName = '';


    constructor(
        injector: Injector,
        private _infoSeedServicesServiceProxy: InfoSeedServicesServiceProxy
    ) {
        super(injector);
    }
    
    show(infoSeedServiceId?: number): void {
    

        if (!infoSeedServiceId) {
            this.infoSeedService = new CreateOrEditInfoSeedServiceDto();
            this.infoSeedService.id = infoSeedServiceId;
            this.infoSeedService.serviceCreationDate = moment().startOf('day');
            this.infoSeedService.serviceStoppingDate = moment().startOf('day');
            this.serviceTypeServicetypeName = '';
            this.serviceStatusServiceStatusName = '';
            this.serviceFrquencyServiceFrequencyName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._infoSeedServicesServiceProxy.getInfoSeedServiceForEdit(infoSeedServiceId).subscribe(result => {
                this.infoSeedService = result.infoSeedService;

                this.serviceTypeServicetypeName = result.serviceTypeServicetypeName;
                this.serviceStatusServiceStatusName = result.serviceStatusServiceStatusName;
                this.serviceFrquencyServiceFrequencyName = result.serviceFrquencyServiceFrequencyName;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._infoSeedServicesServiceProxy.createOrEdit(this.infoSeedService)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectServiceTypeModal() {
        this.infoSeedServiceServiceTypeLookupTableModal.id = this.infoSeedService.serviceTypeId;
        this.infoSeedServiceServiceTypeLookupTableModal.displayName = this.serviceTypeServicetypeName;
        this.infoSeedServiceServiceTypeLookupTableModal.show();
    }
    openSelectServiceStatusModal() {
        this.infoSeedServiceServiceStatusLookupTableModal.id = this.infoSeedService.serviceStatusId;
        this.infoSeedServiceServiceStatusLookupTableModal.displayName = this.serviceStatusServiceStatusName;
        this.infoSeedServiceServiceStatusLookupTableModal.show();
    }
    openSelectServiceFrquencyModal() {
        this.infoSeedServiceServiceFrquencyLookupTableModal.id = this.infoSeedService.serviceFrquencyId;
        this.infoSeedServiceServiceFrquencyLookupTableModal.displayName = this.serviceFrquencyServiceFrequencyName;
        this.infoSeedServiceServiceFrquencyLookupTableModal.show();
    }


    setServiceTypeIdNull() {
        this.infoSeedService.serviceTypeId = null;
        this.serviceTypeServicetypeName = '';
    }
    setServiceStatusIdNull() {
        this.infoSeedService.serviceStatusId = null;
        this.serviceStatusServiceStatusName = '';
    }
    setServiceFrquencyIdNull() {
        this.infoSeedService.serviceFrquencyId = null;
        this.serviceFrquencyServiceFrequencyName = '';
    }


    getNewServiceTypeId() {
        this.infoSeedService.serviceTypeId = this.infoSeedServiceServiceTypeLookupTableModal.id;
        this.serviceTypeServicetypeName = this.infoSeedServiceServiceTypeLookupTableModal.displayName;
    }
    getNewServiceStatusId() {
        this.infoSeedService.serviceStatusId = this.infoSeedServiceServiceStatusLookupTableModal.id;
        this.serviceStatusServiceStatusName = this.infoSeedServiceServiceStatusLookupTableModal.displayName;
    }
    getNewServiceFrquencyId() {
        this.infoSeedService.serviceFrquencyId = this.infoSeedServiceServiceFrquencyLookupTableModal.id;
        this.serviceFrquencyServiceFrequencyName = this.infoSeedServiceServiceFrquencyLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
