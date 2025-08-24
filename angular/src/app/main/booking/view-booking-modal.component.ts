import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewBookingModal',
    templateUrl: './view-booking-modal.component.html'
})
export class ViewBookingModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();


    constructor(
        injector: Injector
    ) {
        super(injector);
    }
}
