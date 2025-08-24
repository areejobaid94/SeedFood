import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {ForcatsDto,  GetForcatsForViewDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewForcatsModal',
    templateUrl: './view-forcats-modal.component.html'
})
export class ViewForcatsModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetForcatsForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetForcatsForViewDto();
        this.item.forcats = new ForcatsDto();
    }

    show(item: GetForcatsForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
