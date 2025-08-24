import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {BookingServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'createOrEditBookingModal',
    templateUrl: './create-or-edit-booking-modal.component.html'
})
export class CreateOrEditBookingModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    TypeId:number;

    constructor(
        injector: Injector,
        private _bookingServiceProxy: BookingServiceProxy
    ) {
        super(injector);
    }


}
