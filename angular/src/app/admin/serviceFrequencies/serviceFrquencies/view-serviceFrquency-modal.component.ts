import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetServiceFrquencyForViewDto, ServiceFrquencyDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewServiceFrquencyModal',
    templateUrl: './view-serviceFrquency-modal.component.html'
})
export class ViewServiceFrquencyModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetServiceFrquencyForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetServiceFrquencyForViewDto();
        this.item.serviceFrquency = new ServiceFrquencyDto();
    }

    show(item: GetServiceFrquencyForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
