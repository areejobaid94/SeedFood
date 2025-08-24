import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TenantServicesServiceProxy, CreateOrEditTenantServiceDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { TenantServiceInfoSeedServiceLookupTableModalComponent } from './tenantService-infoSeedService-lookup-table-modal.component';

@Component({
    selector: 'createOrEditTenantServiceModal',
    templateUrl: './create-or-edit-tenantService-modal.component.html'
})
export class CreateOrEditTenantServiceModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('tenantServiceInfoSeedServiceLookupTableModal', { static: true }) tenantServiceInfoSeedServiceLookupTableModal: TenantServiceInfoSeedServiceLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    tenantService: CreateOrEditTenantServiceDto = new CreateOrEditTenantServiceDto();

    infoSeedServiceServiceName = '';


    constructor(
        injector: Injector,
        private _tenantServicesServiceProxy: TenantServicesServiceProxy
    ) {
        super(injector);
    }
    
    show(tenantServiceId?: number): void {
    

        if (!tenantServiceId) {
            this.tenantService = new CreateOrEditTenantServiceDto();
            this.tenantService.id = tenantServiceId;
            this.infoSeedServiceServiceName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._tenantServicesServiceProxy.getTenantServiceForEdit(tenantServiceId).subscribe(result => {
                this.tenantService = result.tenantService;

                this.infoSeedServiceServiceName = result.infoSeedServiceServiceName;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._tenantServicesServiceProxy.createOrEdit(this.tenantService)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectInfoSeedServiceModal() {
        this.tenantServiceInfoSeedServiceLookupTableModal.id = this.tenantService.infoSeedServiceId;
        this.tenantServiceInfoSeedServiceLookupTableModal.displayName = this.infoSeedServiceServiceName;
        this.tenantServiceInfoSeedServiceLookupTableModal.show();
    }


    setInfoSeedServiceIdNull() {
        this.tenantService.infoSeedServiceId = null;
        this.infoSeedServiceServiceName = '';
    }


    getNewInfoSeedServiceId() {
        this.tenantService.infoSeedServiceId = this.tenantServiceInfoSeedServiceLookupTableModal.id;
        this.infoSeedServiceServiceName = this.tenantServiceInfoSeedServiceLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
