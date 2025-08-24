import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetMenuItemStatusForViewDto, MenuItemStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewMenuItemStatusModal',
    templateUrl: './view-menuItemStatus-modal.component.html'
})
export class ViewMenuItemStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetMenuItemStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetMenuItemStatusForViewDto();
        this.item.menuItemStatus = new MenuItemStatusDto();
    }

    show(item: GetMenuItemStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
