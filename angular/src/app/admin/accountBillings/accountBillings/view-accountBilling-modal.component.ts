import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetAccountBillingForViewDto, AccountBillingDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAccountBillingModal',
    templateUrl: './view-accountBilling-modal.component.html'
})
export class ViewAccountBillingModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAccountBillingForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAccountBillingForViewDto();
        this.item.accountBilling = new AccountBillingDto();
    }

    show(item: GetAccountBillingForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
