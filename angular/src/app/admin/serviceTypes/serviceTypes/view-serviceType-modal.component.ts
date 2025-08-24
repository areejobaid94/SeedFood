import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetServiceTypeForViewDto, ServiceTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewServiceTypeModal',
    templateUrl: './view-serviceType-modal.component.html'
})
export class ViewServiceTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetServiceTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetServiceTypeForViewDto();
        this.item.serviceType = new ServiceTypeDto();
    }

    show(item: GetServiceTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
