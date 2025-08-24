import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetReceiptDetailForViewDto, ReceiptDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'masterDetailChild_Receipt_viewReceiptDetailModal',
    templateUrl: './masterDetailChild_Receipt_view-receiptDetail-modal.component.html'
})
export class MasterDetailChild_Receipt_ViewReceiptDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetReceiptDetailForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetReceiptDetailForViewDto();
        this.item.receiptDetail = new ReceiptDetailDto();
    }

    show(item: GetReceiptDetailForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
