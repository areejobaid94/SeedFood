import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetMenuDetailForViewDto, MenuDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewMenuDetailModal',
    templateUrl: './view-menuDetail-modal.component.html'
})
export class ViewMenuDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetMenuDetailForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetMenuDetailForViewDto();
        this.item.menuDetail = new MenuDetailDto();
    }

    show(item: GetMenuDetailForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
