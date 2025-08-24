import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetReceiptForViewDto, ReceiptDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewReceiptModal',
    templateUrl: './view-receipt-modal.component.html'
})
export class ViewReceiptModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetReceiptForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetReceiptForViewDto();
        this.item.receipt = new ReceiptDto();
    }

    show(item: GetReceiptForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
