import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTenantServiceForViewDto, TenantServiceDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewTenantServiceModal',
    templateUrl: './view-tenantService-modal.component.html'
})
export class ViewTenantServiceModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetTenantServiceForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetTenantServiceForViewDto();
        this.item.tenantService = new TenantServiceDto();
    }

    show(item: GetTenantServiceForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
